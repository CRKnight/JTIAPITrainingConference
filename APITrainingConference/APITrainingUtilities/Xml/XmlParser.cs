using System.IO;
using System.Xml.Serialization;

namespace APITrainingUtilities.Xml
{
	public static class XmlParser
	{
		public static CoreFilingMessage CreateMessage(string path)
		{
			var xmlSerializer = new XmlSerializer(typeof(CoreFilingMessage));
			using (StreamReader streamReader = new StreamReader(path))
			{
				return (CoreFilingMessage) xmlSerializer.Deserialize(streamReader);
			}
		}
	}
}