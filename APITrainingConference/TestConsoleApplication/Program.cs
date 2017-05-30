using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APILibrary;
using APILibrary.JustWareAPI;


namespace TestConsoleApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			APIWorker apiWorker = new APIWorker("Path to the xml");
			apiWorker.LogXml();
			Console.ReadLine();
		}
	}
}
