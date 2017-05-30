using System;
using System.Collections.Generic;
using System.Linq;
using APILibrary.JustWareAPI;
using APITrainingUtilities.Logging;
using APITrainingUtilities.Xml;

namespace APILibrary
{
	public class APIWorker
	{
		const string XML_DIRECTORY_PATH = "C:\\git\\TrainingConference\\JTIAPITrainingConference\\XML\\";
		private const string AGENCY_ADD_BY_CODE = "";
		private const string STATUS_CODE = "";
		private const string ATTOURNEY_CODE = "ATTY";
		private const string DEFENDANT_CODE = "DEF";

		private readonly ILog _logger = LogFactory.CreateForType<ILog>();
		private readonly CoreFilingMessage _message = XmlParser.CreateMessage(XML_DIRECTORY_PATH + "APIClass.xml");

		private JustWareApiClient _client;

		private Dictionary<string, Name> _nameCache;

		public APIWorker()
		{
			CreateApiClient();
		}

		public void LogXml()
		{
			_logger.Debug($"{_message.Case.CaseParticipants.FirstOrDefault().CaseParticipantRoleCode}");
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
			Identification ID = caseParticipant.EntityPerson.Identifications.FirstOrDefault();

			if (_nameCache.ContainsKey(ID.IdentificationID))
			{
				_logger.Debug($"Cached Name Used {ID.IdentificationID}");
				return _nameCache[ID.IdentificationID];
			}

			Name name = _client.FindNames($"NameAttribute = {ID.IdentificationID}", null).FirstOrDefault();
			if (name != null)
			{
				_logger.Debug($"Found Name through API for {ID.IdentificationID}");
				return name;
			}

			var entityPerson = caseParticipant.EntityPerson;
			var contactInformation = entityPerson.ContactInformation;

			name = new Name();
			name.Last = entityPerson.PersonName.PersonSurName;
			name.First = entityPerson.PersonName.PersonGivenName;
			name.Middle = entityPerson.PersonName.PersonMiddleName;
			name.Suffix = entityPerson.PersonName.PersonNameSuffixText;

			name.Phones = new List<Phone>();
			name.Phones.Add(new Phone()
			{
				Number = contactInformation.TelephoneNumber,
				Operation = OperationType.Insert
			});

			name.Addresses = new List<Address>();
			name.Addresses.Add(new Address()
			{
				StreetAddress =
					contactInformation.MailingAddress.LocationStreet + contactInformation.MailingAddress.AddressSecondaryUnitText,
				StateCode = contactInformation.MailingAddress.LocationStateName,
				City = contactInformation.MailingAddress.LocationCityName,
				Zip = contactInformation.MailingAddress.LocationPostalCode,
				Operation = OperationType.Insert
			});

			name.Emails = new List<Email>();
			name.Emails.Add(new Email()
			{
				Address = contactInformation.Email,
				Operation = OperationType.Insert
			});

			name.Numbers = new List<NameNumber>();
			name.Numbers.Add(new NameNumber()
			{
				Number = ID.IdentificationID,
				Operation = OperationType.Insert
			});

			name.TempID = "name";
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

			_logger.Debug($"Created new name: {name.Last}, {ID.IdentificationID}");
			name.ID = keys.Where(k => name.TempID.Equals(k.TempID)).Select(k => k.NewID).FirstOrDefault();
			_nameCache.Add(ID.IdentificationID, name);

			return name;
		}

		public List<CaseInvolvedName> GetCaseInvolvments()
		{
			List<CaseInvolvedName> involvments = new List<CaseInvolvedName>();
			CaseParticipant[] casePersons = _message.Case.CaseParticipants;
			foreach (var caseParticipant in casePersons)
			{
				CaseInvolvedName name = new CaseInvolvedName();
				name.Name = GetName(caseParticipant);
				name.InvolvementCode = caseParticipant.CaseParticipantRoleCode;
				involvments.Add(name);
			}
			return involvments;
		}

		public void Documents()
		{
			
		}

		public void SubmitCase()
		{
			JustWareAPI.Case newCase = new JustWareAPI.Case();
			newCase.AgencyAddedByCode = AGENCY_ADD_BY_CODE;
			newCase.ReceivedDate = DateTime.Now;
			newCase.StatusDate = _message.DocumentFiledDate.Date;
			newCase.StatusCode = STATUS_CODE;
			newCase.CaseInvolvedNames = GetCaseInvolvments();

			newCase.Operation = OperationType.Insert;
			try
			{
				_client.Submit(newCase);

			}
			catch (Exception exception)
			{
				_logger.Error($"Error Occurred during Case submit: {exception.Message}");
			}
		}
	}
}
