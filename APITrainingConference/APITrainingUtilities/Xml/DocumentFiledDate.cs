using System;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace APITrainingUtilities.Xml
{
	[Serializable]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true)]
	public class DocumentFiledDate
	{
		[XmlElement("DateTime", Form = XmlSchemaForm.Unqualified)]
		public DateTime Date { get; set; }
	}
}