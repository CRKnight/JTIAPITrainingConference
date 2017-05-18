using APITrainingUtilities.Logging;
using APITrainingUtilities.Xml;

namespace APILibrary
{
	public class APIWorker
	{
		private const string CASE_TYPE_CODE = "CT001";
		private const string CASE_STATUS_CODE = "OPEN";
		private const string AGENCY_ADD_BY_CODE = "NDT";
		private const string ADDRESS_CODE = "HA";
		private const string EMAIL_CODE = "PER";
		private const string TELEPHONE_CODE = "HP";
		private const string CASE_AGENCY_CODE = "AGENX";
		private const string CASE_DOCKET_ID_NUMBER_CODE = "CDI";

		private static readonly ILog _logger = LogFactory.CreateForType<APIWorker>();
		private readonly CoreFilingMessage _message;

		public APIWorker(CoreFilingMessage message)
		{
			_message = message;
		}

	}
}
