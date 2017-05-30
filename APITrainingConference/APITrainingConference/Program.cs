using System;
using APITrainingUtilities.Logging;
using APITrainingUtilities.Xml;

namespace APITrainingConference
{
	class Program
	{
		private static readonly ILog _logger = LogFactory.CreateForType<Program>();
		private const string SAMPLE_1_PATH = @"Data\Sample1.xml";

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			_logger.Debug("Log file location: {0}", LogFactory.LogFilePath);
			CoreFilingMessage sample1 = XmlParser.CreateMessage(SAMPLE_1_PATH);
			_logger.Information("Processing CaseDocketID: {0}", sample1.Case.CaseDocketID);
			Console.WriteLine("Press enter to exit");
			Console.ReadLine();
		}
	}
}
