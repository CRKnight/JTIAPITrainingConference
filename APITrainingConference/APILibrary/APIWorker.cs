using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using APILibrary.JustWareAPI;
using APITrainingUtilities;
using APITrainingUtilities.Logging;
using APITrainingUtilities.Xml;

namespace APILibrary
{
	public class APIWorker
	{
		private const string AGENCY_ADD_BY_CODE = "";
		private const string STATUS_CODE = "";
		private const string ATTOURNEY_CODE = "ATTY";

		private readonly ILog _logger = LogFactory.CreateForType<ILog>();
		private readonly CoreFilingMessage _message;

		private JustWareApiClient _client;

		private readonly Dictionary<string, Name> _nameCache = new Dictionary<string, Name>();

		public APIWorker(string xmlFilePath)
		{
			_message = XmlParser.CreateMessage(xmlFilePath);
			CreateApiClient();
		}

		public void LogXml()
		{
			_logger.Debug($"{_message.Case.CaseParticipants.FirstOrDefault().EntityPerson.PersonName.PersonSurName}");
		}

		private void CreateApiClient()
		{
			_client = new JustWareApiClient();
			_client.ClientCredentials.UserName.UserName = "tc\\Admin";
			_client.ClientCredentials.UserName.Password = "JustWare6";

			int id = _client.GetCallerNameID();
			_logger.Debug($"Caller Id = {id}");
		}

		public Name GetName(CaseParticipant caseParticipant)
		{
			var entityPerson = caseParticipant.EntityPerson;
			List<Identification> idList = entityPerson.Identifications.ToList();

			foreach (var identification in idList)
			{
				if (ATTOURNEY_CODE.Equals(caseParticipant.CaseParticipantRoleCode) && _nameCache.ContainsKey(identification.IdentificationID))
				{
					_logger.Debug($"Cached Name Used {identification.IdentificationID}");
					return _nameCache[identification.IdentificationID];
				}
			}

			List<string> queryList = new List<string>();
			foreach (Identification identification in idList)
			{
				queryList.Add($"NameAttribute = {identification.IdentificationID}");
			}
			string.Join(" && ", queryList);

			Name name = _client.FindNames(string.Join(" && ", queryList), null).FirstOrDefault();
			if (name != null)
			{
				_logger.Debug($"Found Name through API {name.ID}");

				AddContactInformation(entityPerson, ref name);

				if (name.First != entityPerson.PersonName.PersonGivenName)
				{
					name.First = entityPerson.PersonName.PersonGivenName;
					name.FirstIsChanged = true;
				}
				if (name.Middle != entityPerson.PersonName.PersonMiddleName)
				{
					name.Middle = entityPerson.PersonName.PersonMiddleName;
					name.MiddleIsChanged = true;
				}
				if (name.Suffix != entityPerson.PersonName.PersonNameSuffixText)
				{
					name.Suffix = entityPerson.PersonName.PersonNameSuffixText;
					name.SuffixIsChanged = true;
				}
				name.Operation = OperationType.Update;
				try
				{
					_client.Submit(name);
				}
				catch (Exception exception)
				{
					_logger.Error($"Error Updating Name. {exception.Message}");
				}
				if (ATTOURNEY_CODE.Equals(caseParticipant.CaseParticipantRoleCode))
				{
					foreach (NameNumber attournyNumber in name.Numbers)
					{
						_nameCache.Remove(attournyNumber.Number);
						_nameCache.Add(attournyNumber.Number, name);
					}
				}
				return name;
			}

			name = new Name();
			name.Last = entityPerson.PersonName.PersonSurName;
			name.First = entityPerson.PersonName.PersonGivenName;
			name.Middle = entityPerson.PersonName.PersonMiddleName;
			name.Suffix = entityPerson.PersonName.PersonNameSuffixText;

			AddContactInformation(entityPerson, ref name);

			name.Numbers = new List<NameNumber>();
			foreach (var id in idList)
			{
				name.Numbers.Add(new NameNumber()
				{
					Number = id.IdentificationID,
					Operation = OperationType.Insert
				});
			}
			name.TempID = caseParticipant.CaseParticipantRoleCode;
			name.Operation = OperationType.Insert;

			List<Key> keys;
			try
			{
				keys = _client.Submit(name);
			}
			catch (Exception exception)
			{
				_logger.Error($"Error submitting Name: {exception.Message}");
				return null;
			}

			_logger.Debug($"Created new name: {name.Last}, {name.ID}");
			name.ID = keys.Where(k => name.TempID.Equals(k.TempID)).Select(k => k.NewID).FirstOrDefault();

			if (ATTOURNEY_CODE.Equals(caseParticipant.CaseParticipantRoleCode))
			{
				foreach (NameNumber attournyNumber in name.Numbers)
				{
					_nameCache.Remove(attournyNumber.Number);
					_nameCache.Add(attournyNumber.Number, name);
				}
			}

			return name;
		}

		private static void AddContactInformation(EntityPerson entityPerson, ref Name name)
		{
			var contactInformation = entityPerson.ContactInformation;

			if (name.Phones.All(p => p.Number != contactInformation.TelephoneNumber))
			{
				name.Phones = new List<Phone>();
				name.Phones.Add(new Phone()
				{
					Number = contactInformation.TelephoneNumber,
					Operation = OperationType.Insert
				});
			}

			MailingAddress mailingAddress = contactInformation.MailingAddress;
			string streetAddress = mailingAddress.LocationStreet + mailingAddress.AddressSecondaryUnitText;

			if (name.Addresses.All(a => a.StreetAddress != streetAddress && a.StateCode != mailingAddress.LocationStateName
										&& a.City != mailingAddress.LocationCityName && a.Zip != mailingAddress.LocationPostalCode))
			{
				name.Addresses = new List<Address>();
				name.Addresses.Add(new Address()
				{
					StreetAddress = streetAddress,
					StateCode = mailingAddress.LocationStateName,
					City = mailingAddress.LocationCityName,
					Zip = mailingAddress.LocationPostalCode,
					Operation = OperationType.Insert
				});
			}

			if (name.Emails.All(p => p.Address != contactInformation.Email))
			{
				name.Emails = new List<Email>();
				name.Emails.Add(new Email()
				{
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
				CaseInvolvedName name = new CaseInvolvedName();
				name.Name = GetName(caseParticipant);
				name.InvolvementCode = caseParticipant.CaseParticipantRoleCode;
				involvements.Add(name);
			}
			return involvements;
		}

		public void SubmitCase()
		{
			JustWareAPI.Case newCase = new JustWareAPI.Case();
			newCase.AgencyAddedByCode = AGENCY_ADD_BY_CODE;
			newCase.ReceivedDate = DateTime.Now;
			newCase.StatusDate = _message.DocumentFiledDate.Date;
			newCase.StatusCode = STATUS_CODE;
			newCase.CaseInvolvedNames = GetCaseInvolvements();

			newCase.Operation = OperationType.Insert;
			List<Key> keys;
			try
			{
				keys = _client.Submit(newCase);
			}
			catch (Exception exception)
			{
				_logger.Error($"Error Occurred during Case submit: {exception.Message}");
				return;
			}

			foreach (var key in keys)
			{
				if (ATTOURNEY_CODE.Equals(key.TempID))
				{
					Name attourny = null;
					foreach (var involvement in newCase.CaseInvolvedNames)
					{
						if (involvement.InvolvementCode.Equals(ATTOURNEY_CODE))
						{
							attourny = involvement.Name;
						}
					}
					if (attourny != null)
					{
						foreach (NameNumber attournyNumber in attourny.Numbers)
						{
							_nameCache.Remove(attournyNumber.Number);
							_nameCache.Add(attournyNumber.Number, attourny);
						}
					}
				}
			}
		}

		public void UploadDocument(string caseID)
		{
			DocumentAttachment documentAttachment = _message.FilingLeadDocument.DocumentAttachment;

			var document = new CaseDocument();
			document.CaseID = caseID;
			document.FileName = documentAttachment.BinaryDescriptionText;
			document.TypeCode = documentAttachment.BinaryCategoryText;
			document.Notes = _message.FilingLeadDocument.FilingCommentsText;
			document.Operation = OperationType.Insert;
			try
			{
				_logger.Debug("Requesting upload url");
				string uploadUrl = _client.RequestFileUpload(document);

				_logger.Debug($"Uploading Document {_message.FilingLeadDocument.DocumentIdentification}");
				DocumentUploader.StreamFileToJustWare(uploadUrl, documentAttachment.BinaryLocationURI + _message.FilingLeadDocument.DocumentIdentification);

				_logger.Debug("Finalizing upload");
				_client.FinalizeFileUpload(document, uploadUrl);
				_logger.Debug("Finalizing upload completed");
			}
			catch (Exception exception)
			{
				_logger.Error($"Error uploading document {exception.Message}");
			}
		}
	}
}
