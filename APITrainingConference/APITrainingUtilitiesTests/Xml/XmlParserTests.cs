using System;
using APITrainingUtilities.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APITrainingUtilitiesTests.Xml
{
	[TestClass]
	[DeploymentItem(@"Xml\TestData.xml")]
	public class XmlParserTests
	{
		private static CoreFilingMessage _message;

		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			// Act for all tests
			_message = XmlParser.CreateMessage("TestData.xml");
		}

		[TestMethod]
		public void CreateMessage_DocumentFiledDate()
		{
			// Assert
			Assert.IsNotNull(_message?.DocumentFiledDate, "Expected DocumentFiledDate");
			Assert.AreEqual(new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc), _message.DocumentFiledDate.Date, "Date");
		}

		[TestMethod]
		public void CreateMessage_Case_CaseDocketID()
		{
			// Assert
			Assert.IsNotNull(_message?.Case?.CaseDocketID, "Expected value");
			Assert.AreEqual("TESTDOCKETID", _message.Case.CaseDocketID, "CaseDocketID");
		}

		[TestMethod]
		public void CreateMessage_Case_CaseTypeText()
		{
			// Assert
			Assert.IsNotNull(_message?.Case?.CaseTypeText, "Expected value");
			Assert.AreEqual("TESTCASETYPE", _message.Case.CaseTypeText, "CaseTypeText");
		}

		[TestMethod]
		public void CreateMessage_Case_CaseParticipants()
		{
			// Assert
			Assert.IsNotNull(_message?.Case?.CaseParticipants, "Expected value");
			Assert.AreEqual(3, _message.Case.CaseParticipants.Length, "Participants count");
		}

		[TestMethod]
		public void CreateMessage_Case_CaseParticipants_NoIdentifiers()
		{
			// Assert
			Assert.IsNotNull(_message?.Case?.CaseParticipants[1]?.EntityPerson, "Expected EntityPerson");
			EntityPerson entityPerson = _message.Case.CaseParticipants[1].EntityPerson;
			Assert.IsNull(entityPerson.Identifications, "Identifications");
		}

		[TestMethod]
		public void CreateMessage_Case_CaseParticipant_CaseParticipantRoleCode()
		{
			// Assert
			Assert.IsNotNull(_message?.Case?.CaseParticipants, "Expected value");
			Assert.AreEqual("TST", _message.Case.CaseParticipants[0].CaseParticipantRoleCode, "CaseParticipantRoleCode");
		}

		[TestMethod]
		public void CreateMessage_Case_CaseParticipant_EntityPerson()
		{
			// Assert
			Assert.IsNotNull(_message?.Case?.CaseParticipants[0]?.EntityPerson, "Expected EntityPerson");

			EntityPerson entityPerson = _message.Case.CaseParticipants[0].EntityPerson;
			Assert.IsNotNull(entityPerson.PersonName, "Expected PersonName");
			Assert.AreEqual("TESTGIVEN", entityPerson.PersonName.PersonGivenName, "PersonGivenName");
			Assert.AreEqual("TESTMIDDLE", entityPerson.PersonName.PersonMiddleName, "PersonMiddleName");
			Assert.AreEqual("TESTSUR", entityPerson.PersonName.PersonSurName, "PersonSurName");
			Assert.AreEqual("TESTSUFFIX", entityPerson.PersonName.PersonNameSuffixText, "PersonNameSuffixText");

			Assert.IsNotNull(entityPerson.Identifications, "Exptected Identifications");
			Assert.AreEqual(2, entityPerson.Identifications.Length, "Number of Identifications");
			Identification identity = entityPerson.Identifications[0];
			Assert.AreEqual("TESTID", identity.IdentificationID, "IdentificationID");
			Assert.AreEqual("TESTCATEGORY", identity.IdentificationCategoryText, "IdentificationCategoryText");

			Assert.IsNotNull(entityPerson.ContactInformation, "Exptected ContactInformation");
			ContactInformation contact = entityPerson.ContactInformation;
			Assert.AreEqual("(123)456-7890", contact.TelephoneNumber, "TelephoneNumber");
			Assert.AreEqual("test@test.com", contact.Email, "Email");
			Assert.IsNotNull(contact.MailingAddress, "Exptected MailingAddress");
			MailingAddress address = contact.MailingAddress;
			Assert.AreEqual("TEST STREET", address.LocationStreet, "LocationStreet");
			Assert.AreEqual("TEST SECONDARY", address.AddressSecondaryUnitText, "AddressSecondaryUnitText");
			Assert.AreEqual("TEST CITY", address.LocationCityName, "LocationCityName");
			Assert.AreEqual("TEST STATE", address.LocationStateName, "LocationStateName");
			Assert.AreEqual("TEST ZIP", address.LocationPostalCode, "LocationPostalCode");
		}

		[TestMethod]
		public void CreateMessage_FilingLeadDocument_DocumentIdentification()
		{
			// Assert
			Assert.IsNotNull(_message?.FilingLeadDocument?.DocumentIdentification, "Expected DocumentIdentification");
			Identification docIdentification = _message.FilingLeadDocument.DocumentIdentification;
			Assert.AreEqual("TEST DOC ID", docIdentification.IdentificationID, "IdentificationID");
			Assert.AreEqual("DOC ID", docIdentification.IdentificationCategoryText, "IdentificationCategoryText");
		}

		[TestMethod]
		public void CreateMessage_FilingLeadDocument_DocumentMetadata()
		{
			// Assert
			Assert.IsNotNull(_message?.FilingLeadDocument?.DocumentMetadata, "Expected DocumentMetadata");
			DocumentMetadata metaData = _message.FilingLeadDocument.DocumentMetadata;
			Assert.AreEqual("TST1", metaData.RegisterActionDescriptionText, "RegisterActionDescriptionText");
			Assert.IsNotNull(metaData.FilerIdentification, "Expected FilerIdentification");
			Identification filerId = metaData.FilerIdentification;
			Assert.AreEqual("FILER ID", filerId.IdentificationID, "IdentificationID");
			Assert.AreEqual("ATTORNEYID", filerId.IdentificationCategoryText, "IdentificationCategoryText");
		}

		[TestMethod]
		public void CreateMessage_FilingLeadDocument_DocumentAttachment()
		{
			// Assert
			Assert.IsNotNull(_message?.FilingLeadDocument?.DocumentAttachment, "Expected DocumentAttachment");
			DocumentAttachment attachment = _message.FilingLeadDocument.DocumentAttachment;
			Assert.AreEqual("test.pdf", attachment.BinaryDescriptionText, "BinaryDescriptionText");
			Assert.AreEqual("abc.pdf", attachment.BinaryLocationURI, "BinaryLocationURI");
			Assert.AreEqual("LEAD", attachment.BinaryCategoryText, "BinaryCategoryText");
		}
	}
}