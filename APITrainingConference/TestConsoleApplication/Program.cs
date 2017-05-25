using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APILibrary;


namespace TestConsoleApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			APIWorker apiWorker = new APIWorker();
			apiWorker.LogXml();
			Console.ReadLine();
		}
	}
}
