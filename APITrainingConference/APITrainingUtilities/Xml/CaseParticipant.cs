using System;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace APITrainingUtilities.Xml
{
	[Serializable]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true)]
	public class CaseParticipant
	{
		[XmlElement("CaseParticipantRoleCode", Form = XmlSchemaForm.Unqualified)]
		public string CaseParticipantRoleCode { get; set; }

		[XmlElement("EntityPerson", Form = XmlSchemaForm.Unqualified)]
		public EntityPerson EntityPerson { get; set; }
	}
}