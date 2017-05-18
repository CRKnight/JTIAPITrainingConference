using System;
using System.IO;
using System.Linq;
using APITrainingUtilities.Xml;
using Serilog;
using Serilog.Core;

namespace APITrainingUtilities.Logging
{
	public static class LogFactory
	{
		private static Logger _logger;
		private static Logger Logger => _logger ?? (_logger = InitializeLogger());

		private static string _logFilePath;
		public static string LogFilePath => _logFilePath ??
			(_logFilePath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"JournalTechnologies",
				"APITraining2017"));

		private static Logger InitializeLogger()
		{
			return new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.Destructure.ByTransforming<CoreFilingMessage>(f => new
				{
					f.Case.CaseDocketID,
					Defendant = f.Case.CaseParticipants.SingleOrDefault(p => p.CaseParticipantRoleCode.Equals("DEF", StringComparison.OrdinalIgnoreCase))?.EntityPerson?.PersonName
				})
				.WriteTo.LiterateConsole(outputTemplate: "[{Timestamp:HH:mm:ss.ffff}<{SourceContext}>{Level:u3}]{Message}{NewLine}")
				.WriteTo.RollingFile(
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.ffff(zzz)}<{SourceContext}>[{Level}]{Message}{NewLine}",
					pathFormat: Path.Combine(LogFilePath, "log-{Date}.txt"),
					fileSizeLimitBytes: 10485760 /*10 MB*/,
					retainedFileCountLimit: 10)
				.CreateLogger();
		}

		public static ILog CreateForType<T>()
		{
			return new Serilog(Logger.ForContext<T>());
		}

		public static ILog Create()
		{
			return new Serilog(Logger);
		}

		public static void CloseAndFlush()
		{
			_logger?.Dispose();
			_logger = null;
		}
	}
}