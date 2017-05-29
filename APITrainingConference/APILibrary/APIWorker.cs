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

		private readonly ILog _logger = LogFactory.CreateForType<ILog>();
		private readonly CoreFilingMessage _message = XmlParser.CreateMessage(XML_DIRECTORY_PATH + "APIClass.xml");

		private JustWareApiClient _client;

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
			Name name = new Name();
			var entityPerson = caseParticipant.EntityPerson;
			name = new Name();
			name.Last = entityPerson.PersonName.PersonSurName;
			name.First = entityPerson.PersonName.PersonGivenName;
			name.Middle = entityPerson.PersonName.PersonMiddleName;
			name.Suffix = entityPerson.PersonName.PersonNameSuffixText;
			name.Operation = OperationType.Insert;
			_logger.Debug($"Created new name: {name.Last}, {ID.IdentificationID}");
			return name;
		}
	}
}
