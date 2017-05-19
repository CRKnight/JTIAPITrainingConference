using System;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace APITrainingUtilities.Xml
{
	[Serializable]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true)]
	public class Identification
	{
		[XmlElement("IdentificationID", Form = XmlSchemaForm.Unqualified)]
		public string IdentificationID { get; set; }

		[XmlElement("IdentificationCategoryText", Form = XmlSchemaForm.Unqualified)]
		public string IdentificationCategoryText { get; set; }
	}
}