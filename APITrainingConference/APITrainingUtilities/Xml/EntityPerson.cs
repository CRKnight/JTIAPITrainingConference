using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace APITrainingUtilities.Xml
{
	[Serializable]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true)]
	public class EntityPerson
	{
		[XmlElement("PersonName", Form = XmlSchemaForm.Unqualified)]
		public PersonName PersonName { get; set; }

		[XmlElement("PersonOtherIdentification", Form = XmlSchemaForm.Unqualified)]
		public Identification[] Identifications { get; set; }

		[XmlElement("ContactInformation", typeof(ContactInformation), Form = XmlSchemaForm.Unqualified)]
		public ContactInformation ContactInformation { get; set; }

		public Identification GetEntityPersonID()
		{
			return Identifications?.FirstOrDefault(i => i.IdentificationCategoryText.Equals("CMSID") ||
			                                            i.IdentificationCategoryText.Equals("BAR"));
		}
	}
}