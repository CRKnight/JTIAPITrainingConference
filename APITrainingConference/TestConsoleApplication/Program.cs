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
			APIWorker apiWorker = new APIWorker();
			apiWorker.LogXml();
			List<Name> nameList = apiWorker.GetNames();
			Console.WriteLine($"We got {nameList.Count} names");
			Console.ReadLine();
		}
	}
}
