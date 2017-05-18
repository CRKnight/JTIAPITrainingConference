using System.ServiceProcess;

namespace APITrainingConference
{
	public partial class XMLToAPIService : ServiceBase
	{
		public XMLToAPIService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
		}

		protected override void OnStop()
		{
		}
	}
}
