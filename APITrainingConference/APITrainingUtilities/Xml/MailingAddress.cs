using System;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace APITrainingUtilities.Xml
{
	[Serializable]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true)]
	public class MailingAddress
	{
		[XmlElement("LocationStreet", Form = XmlSchemaForm.Unqualified)]
		public string LocationStreet { get; set; }

		[XmlElement("AddressSecondaryUnitText", Form = XmlSchemaForm.Unqualified)]
		public string AddressSecondaryUnitText { get; set; }

		[XmlElement("LocationCityName", Form = XmlSchemaForm.Unqualified)]
		public string LocationCityName { get; set; }

		[XmlElement("LocationStateName", Form = XmlSchemaForm.Unqualified)]
		public string LocationStateName { get; set; }

		[XmlElement("LocationPostalCode", Form = XmlSchemaForm.Unqualified)]
		public string LocationPostalCode { get; set; }
	}
}