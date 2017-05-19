using System;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace APITrainingUtilities.Xml
{
	[Serializable]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true)]
	public class Case
	{
		[XmlElement("CaseDocketID", Form = XmlSchemaForm.Unqualified)]
		public string CaseDocketID { get; set; }

		[XmlElement("CaseTypeText", Form = XmlSchemaForm.Unqualified)]
		public string CaseTypeText { get; set; }

		[XmlElement("CaseParticipant", Form = XmlSchemaForm.Unqualified)]
		public CaseParticipant[] CaseParticipants { get; set; }
	}
}