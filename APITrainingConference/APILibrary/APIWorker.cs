using System;
using System.Collections.Generic;
using System.Linq;
using APITrainingUtilities.Logging;
using APITrainingUtilities.Xml;

namespace APILibrary
{
	public class APIWorker
	{
		const string XML_DIRECTORY_PATH = "C:\\git\\TrainingConference\\JTIAPITrainingConference\\XML\\";

		private readonly ILog _logger = LogFactory.CreateForType<ILog>();
		private readonly CoreFilingMessage _message = XmlParser.CreateMessage(XML_DIRECTORY_PATH + "APIClass.xml");

		public void LogXml()
		{
			_logger.Debug($"{_message.Case.CaseParticipants.FirstOrDefault().CaseParticipantRoleCode}");
		}
	}
}
