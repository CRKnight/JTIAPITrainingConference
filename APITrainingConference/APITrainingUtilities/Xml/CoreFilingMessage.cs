using System;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace APITrainingUtilities.Xml
{
	[Serializable]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "", IsNullable = false)]
	public class CoreFilingMessage
	{
		[XmlElement("Case", typeof(Case), Form = XmlSchemaForm.Unqualified)]
		public Case Case { get; set; }

		[XmlElement("DocumentFiledDate", typeof(DocumentFiledDate), Form = XmlSchemaForm.Unqualified)]
		public DocumentFiledDate DocumentFiledDate { get; set; }

		[XmlElement("FilingLeadDocument", typeof(FilingLeadDocument), Form = XmlSchemaForm.Unqualified)]
		public FilingLeadDocument FilingLeadDocument { get; set; }
	}
}