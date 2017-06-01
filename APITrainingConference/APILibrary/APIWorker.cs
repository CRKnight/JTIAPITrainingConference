using System.Collections.Generic;
using APITrainingUtilities.Logging;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using APILibrary.JustWareAPI;
using APITrainingUtilities;
using APITrainingUtilities.Xml;

namespace APILibrary
{
	public class APIWorker
	{
		private const string CASE_TYPE_CODE = "CT001";
		private const string CASE_STATUS_CODE = "OPEN";
		private const string AGENCY_ADD_BY_CODE = "NDT";
		private const string ADDRESS_CODE = "HA";
		private const string EMAIL_CODE = "PER";
		private const string TELEPHONE_CODE = "HP";
		private const string CASE_AGENCY_CODE = "AGENX";
		private const string CASE_DOCKET_ID_NUMBER_CODE = "CDI";
		private const string ATTOURNEY_CODE = "ATTY";

		private static readonly ILog _logger = LogFactory.CreateForType<APIWorker>();
		private readonly CoreFilingMessage _message;
		private readonly string _documentDirectory;

		private JustWareApiClient _client;

		public APIWorker(CoreFilingMessage message) : this(message, null)
		{

		}

		public APIWorker(CoreFilingMessage message, string documentDirectory)
		{
			_message = message;
			CreateApiClient();
			_documentDirectory = documentDirectory;
		}

		private void CreateApiClient()
		{
			_client = new JustWareApiClient();
			_client.ClientCredentials.UserName.UserName = "tc\\User";
			_client.ClientCredentials.UserName.Password = "JustWare6";

			int id = _client.GetCallerNameID();
			_logger.Debug("Caller Id = {Id}", id);
		}

		public Name GetName(CaseParticipant caseParticipant)
		{
			Identification entityID = GetCaseParticipantID(caseParticipant);

			_logger.Debug("Processing name {Last}, {First}", caseParticipant.EntityPerson.PersonName.PersonSurName, caseParticipant.EntityPerson.PersonName.PersonGivenName);

			Name name = InitializeNameEntity(entityID);

			UpdateNameProperties(caseParticipant.EntityPerson, name);

			AddNameNumber(entityID, name);

			List<Key> keys;
			try
			{
				keys = _client.Submit(name);
			}
			catch (Exception exception)
			{
				_logger.Error("Error submitting Name: {Exception}", exception);
				return null;
			}

			if (name.Operation == OperationType.Insert)
			{
				name.ID = keys.Single(k => k.TypeName.Equals("Name")).NewID;
				_logger.Debug("Created name: {ID}", name.ID);

			}
			else
			{
				_logger.Debug("Updated name: {ID}", name.ID);
			}

			return name;
		}

		private Name InitializeNameEntity(Identification entityID)
		{
			Name returnName = new Name();
			returnName.Operation = OperationType.Insert;

			if (string.IsNullOrWhiteSpace(entityID?.IdentificationID)) return returnName;

			try
			{
				Name foundName = _client.FindNames($"Numbers.Any(Number = \"{entityID.IdentificationID}\")",
					new List<string>() {"Phones", "Emails", "Addresses", "Numbers"}).FirstOrDefault();
				if (foundName != null)
				{
					_logger.Debug("Found Name through API {ID}", foundName.ID);
					foundName.Operation = OperationType.Update;
					returnName = foundName;

				}
			}
			catch (Exception exception)
			{
				_logger.Error("Error when finding name: {Exception}", exception);
			}

			returnName.TempID = $"{entityID.IdentificationID}";
			return returnName;
		}

		private void UpdateNameProperties(EntityPerson entityPerson, Name name)
		{
			if (name != null)
			{
				name.Last = entityPerson.PersonName.PersonSurName;
				name.LastIsChanged = true;
				name.First = entityPerson.PersonName.PersonGivenName;
				name.FirstIsChanged = true;
				name.Middle = entityPerson.PersonName.PersonMiddleName;
				name.MiddleIsChanged = true;
				name.Suffix = entityPerson.PersonName.PersonNameSuffixText;
				name.SuffixIsChanged = true;

				AddContactInformation(entityPerson, name);
			}
		}

		private void AddNameNumber(Identification identification, Name name)
		{
			if (identification == null)
			{
				return;
			}
			if (name.Numbers == null)
			{
				name.Numbers = new List<NameNumber>();
			}

			if (name.Numbers.All(n => !n.Number.Equals(identification.IdentificationID)))
			{
				name.Numbers.Add(new NameNumber()
				{
					TypeCode = identification.IdentificationCategoryText,
					Number = identification.IdentificationID,
					Operation = OperationType.Insert
				});
			}
		}

		private Identification GetCaseParticipantID(CaseParticipant caseParticipant)
		{
			var entityPerson = caseParticipant.EntityPerson;

			if (entityPerson.Identifications == null) return null;

			Identification entityID = null;
			foreach (var identification in entityPerson.Identifications)
			{
				if (identification.IdentificationCategoryText.Equals("CMSID") ||
				    identification.IdentificationCategoryText.Equals("BAR"))
				{
					entityID = identification;
				}
			}
			return entityID;
		}

		private static void AddContactInformation(EntityPerson entityPerson, Name name)
		{
			var contactInformation = entityPerson.ContactInformation;

			if (!string.IsNullOrWhiteSpace(contactInformation.TelephoneNumber) &&
			    (name.Phones == null || name.Phones.All(p => p.Number != contactInformation.TelephoneNumber)))
			{
				name.Phones = new List<Phone>();
				name.Phones.Add(new Phone()
				{
					TypeCode = TELEPHONE_CODE,
					Number = contactInformation.TelephoneNumber,
					Operation = OperationType.Insert
				});
			}

			MailingAddress mailingAddress = contactInformation.MailingAddress;
			string streetAddress = $"{mailingAddress.LocationStreet} {mailingAddress.AddressSecondaryUnitText}";

			if (name.Addresses == null || (name.Addresses.All(a => !streetAddress.Equals(a.StreetAddress)
			                                                       && a.StateCode != mailingAddress.LocationStateName
			                                                       && a.City != mailingAddress.LocationCityName
			                                                       && a.Zip != mailingAddress.LocationPostalCode)))
			{
				name.Addresses = new List<Address>();
				name.Addresses.Add(new Address()
				{
					StreetAddress = streetAddress,
					StateCode = mailingAddress.LocationStateName,
					City = mailingAddress.LocationCityName,
					Zip = mailingAddress.LocationPostalCode,
					TypeCode = ADDRESS_CODE,
					Operation = OperationType.Insert
				});
			}

			if (!string.IsNullOrWhiteSpace(contactInformation.Email) &&
			    (name.Emails == null || name.Emails.All(p => p.Address != contactInformation.Email)))
			{
				name.Emails = new List<Email>();
				name.Emails.Add(new Email()
				{
					TypeCode = EMAIL_CODE,
					Address = contactInformation.Email,
					Operation = OperationType.Insert
				});
			}
		}

		public List<CaseInvolvedName> GetCaseInvolvements()
		{
			List<CaseInvolvedName> involvements = new List<CaseInvolvedName>();
			CaseParticipant[] casePersons = _message.Case.CaseParticipants;
			foreach (var caseParticipant in casePersons)
			{
				var name = GetName(caseParticipant);
				CaseInvolvedName caseInvolvedName = new CaseInvolvedName();
				caseInvolvedName.Operation = OperationType.Insert;
				caseInvolvedName.TempID = name.TempID;
				caseInvolvedName.NameID = name.ID;
				caseInvolvedName.InvolvementCode = caseParticipant.CaseParticipantRoleCode;
				involvements.Add(caseInvolvedName);
			}
			return involvements;
		}

		public void SubmitCase()
		{
			JustWareAPI.Case newCase = new JustWareAPI.Case();
			newCase.AgencyAddedByCode = AGENCY_ADD_BY_CODE;
			newCase.ReceivedDate = DateTime.Now;
			newCase.StatusDate = _message.DocumentFiledDate.Date;
			newCase.StatusCode = CASE_STATUS_CODE;
			newCase.TypeCode = CASE_TYPE_CODE;

			newCase.Agencies = new List<Agency>();
			Agency agency = new Agency();
			agency.AgencyCode = CASE_AGENCY_CODE;
			agency.NumberTypeCode = CASE_DOCKET_ID_NUMBER_CODE;
			agency.Number = _message.Case.CaseDocketID;
			agency.Operation = OperationType.Insert;
			newCase.Agencies.Add(agency);

			newCase.CaseInvolvedNames = GetCaseInvolvements();

			newCase.Operation = OperationType.Insert;
			List<Key> keys;
			try
			{
				keys = _client.Submit(newCase);
			}
			catch (Exception exception)
			{
				_logger.Error("Error Occurred during Case submit: {Exception}", exception);
				return;
			}

			var caseID = keys.Single(k => k.TypeName.Equals("Case")).NewCaseID;
			_logger.Debug("Case created: {CaseID}", caseID);

			string filerID = _message.FilingLeadDocument.DocumentMetadata.FilerIdentification.IdentificationID;
			var nameID = newCase.CaseInvolvedNames.FirstOrDefault(i => i.TempID != null && i.TempID.Equals(filerID))?.NameID;
			UploadDocument(caseID, nameID);
		}

		public void UploadDocument(string caseID, int? nameID)
		{
			DocumentAttachment documentAttachment = _message.FilingLeadDocument.DocumentAttachment;

			var document = new CaseDocument();
			document.Operation = OperationType.Insert;
			document.CaseID = caseID;
			document.FileName = documentAttachment.BinaryDescriptionText;
			document.TypeCode = documentAttachment.BinaryCategoryText;
			document.Notes = _message.FilingLeadDocument.FilingCommentsText;

			List<Key> keys;
			try
			{
				_logger.Debug("Requesting upload url");
				string uploadUrl = _client.RequestFileUpload(document);

				_logger.Debug($"Uploading Document {_message.FilingLeadDocument.DocumentIdentification}");
				uploadUrl.UploadToApi(Path.Combine(_documentDirectory,
					_message.FilingLeadDocument.DocumentIdentification.IdentificationID, documentAttachment.BinaryLocationURI));

				_logger.Debug("Finalizing upload");
				keys = _client.FinalizeFileUpload(document, uploadUrl);
				_logger.Debug("Upload completed");
			}

			catch (Exception exception)
			{
				_logger.Error("Error uploading document  {Exception}", exception);
				return;
			}
			var documentID = keys.FirstOrDefault(k => k.TypeName.Equals("CaseDocument"))?.NewID;

			_logger.Debug("Adding Document Involvement");

			if (nameID.HasValue && documentID.HasValue)
			{
				var docInvName = new DocumentInvolvedName();
				docInvName.Operation = OperationType.Insert;
				docInvName.NameID = nameID.Value;
				docInvName.EventID = documentID.Value;
				try
				{
					_client.Submit(docInvName);
					_logger.Debug("Document Involvement submitted");
				}
				catch (Exception exception)
				{
					_logger.Error("Error submitting document involvement {Exception}", exception);
				}
				
			}
		}
	}
}
