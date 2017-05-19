using System;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace APITrainingUtilities.Xml
{
	[Serializable]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true)]
	public class DocumentMetadata
	{
		[XmlElement("RegisterActionDescriptionText", Form = XmlSchemaForm.Unqualified)]
		public string RegisterActionDescriptionText { get; set; }

		[XmlElement("FilerIdentification", Form = XmlSchemaForm.Unqualified)]
		public Identification FilerIdentification { get; set; }
	}
}