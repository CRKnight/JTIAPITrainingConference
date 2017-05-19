using System;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace APITrainingUtilities.Xml
{
	[Serializable]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true)]
	public class DocumentAttachment
	{
		[XmlElement("BinaryDescriptionText", Form = XmlSchemaForm.Unqualified)]
		public string BinaryDescriptionText { get; set; }

		[XmlElement("BinaryLocationURI", Form = XmlSchemaForm.Unqualified)]
		public string BinaryLocationURI { get; set; }

		[XmlElement("BinaryCategoryText", Form = XmlSchemaForm.Unqualified)]
		public string BinaryCategoryText { get; set; }
	}
}