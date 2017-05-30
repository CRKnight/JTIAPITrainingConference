using System.IO;
using System.Xml.Serialization;
using APITrainingUtilities.Logging;

namespace APITrainingUtilities.Xml
{
	public class XmlParser
	{
		private static readonly ILog _logger = LogFactory.CreateForType<XmlParser>();

		public static CoreFilingMessage CreateMessage(string path)
		{
			if (!File.Exists(path))
			{
				_logger.Error("File does not exist: {Path}", path);
				return null;
			}
			var xmlSerializer = new XmlSerializer(typeof(CoreFilingMessage));
			using (StreamReader streamReader = new StreamReader(path))
			{
				return (CoreFilingMessage) xmlSerializer.Deserialize(streamReader);
			}
		}
	}
}