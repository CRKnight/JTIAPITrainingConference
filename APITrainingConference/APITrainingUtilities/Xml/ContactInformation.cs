using System;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace APITrainingUtilities.Xml
{
	[Serializable]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true)]
	public class ContactInformation
	{
		[XmlElement("TelephoneNumber", Form = XmlSchemaForm.Unqualified)]
		public string TelephoneNumber { get; set; }

		[XmlElement("Email", Form = XmlSchemaForm.Unqualified)]
		public string Email { get; set; }

		[XmlElement("MailingAddress", typeof(MailingAddress), Form = XmlSchemaForm.Unqualified)]
		public MailingAddress MailingAddress { get; set; }
	}
}