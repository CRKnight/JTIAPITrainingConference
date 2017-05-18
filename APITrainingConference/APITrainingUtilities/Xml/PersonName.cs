using System;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace APITrainingUtilities.Xml
{
	[Serializable]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true)]
	public class PersonName
	{
		[XmlElement("PersonGivenName", Form = XmlSchemaForm.Unqualified)]
		public string PersonGivenName { get; set; }

		[XmlElement("PersonMiddleName", Form = XmlSchemaForm.Unqualified)]
		public string PersonMiddleName { get; set; }

		[XmlElement("PersonSurName", Form = XmlSchemaForm.Unqualified)]
		public string PersonSurName { get; set; }

		[XmlElement("PersonNameSuffixText", Form = XmlSchemaForm.Unqualified)]
		public string PersonNameSuffixText { get; set; }
	}
}