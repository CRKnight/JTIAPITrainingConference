using System;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace APITrainingUtilities.Xml
{
	[Serializable]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true)]
	public class FilingLeadDocument
	{
		[XmlElement("FilingCommentsText", Form = XmlSchemaForm.Unqualified)]
		public string FilingCommentsText { get; set; }

		[XmlElement("DocumentIdentification", Form = XmlSchemaForm.Unqualified)]
		public Identification DocumentIdentification { get; set; }

		[XmlElement("DocumentMetadata", Form = XmlSchemaForm.Unqualified)]
		public DocumentMetadata DocumentMetadata { get; set; }

		[XmlElement("DocumentAttachment", Form = XmlSchemaForm.Unqualified)]
		public DocumentAttachment DocumentAttachment { get; set; }
	}
}