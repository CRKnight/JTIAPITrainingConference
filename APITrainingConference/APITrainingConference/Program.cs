using System;
using APILibrary;
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
			_logger.Debug("Log file: {LogFilePath}", LogFactory.LogFilePath);
			CoreFilingMessage sample1 = XmlParser.CreateMessage(SAMPLE_1_PATH);
			_logger.Information("Processing: {@Sample1}", sample1);

			APIWorker apiWorker = new APIWorker(sample1);

			Console.WriteLine("Press enter to exit");
			Console.ReadLine();
		}
	}
}
