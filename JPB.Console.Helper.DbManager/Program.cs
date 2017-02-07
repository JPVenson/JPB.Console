using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPB.Console.Helper.Grid.NetCore.Grid;

namespace JPB.Console.Helper.DbManager
{
	class Program
	{
		static void Main(string[] args)
		{
			var elements = new List<Tuple<string,string>>();
			elements.Add(new Tuple<string, string>("Small", "Small\r\nxxx"));
			elements.Add(new Tuple<string, string>("Small", "huge xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"));

			elements.Add(new Tuple<string, string>("Small", "Small"));
			elements.Add(new Tuple<string, string>("Small", "huge xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"));
			elements.Add(new Tuple<string, string>("Small", "Small"));
			elements.Add(new Tuple<string, string>("Small", "huge xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"));
			elements.Add(new Tuple<string, string>("Small", "Small"));
			elements.Add(new Tuple<string, string>("Small", "huge xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"));



			var grid = new ConsoleGrid<Tuple<string, string>>();
			grid.ExpandConsole = false;
			grid.RenderRowNumber = true;
			grid.SourceList = new ObservableCollection<Tuple<string, string>>(elements);
			grid.RenderGrid();
			System.Console.ReadKey();
		}
	}
}
