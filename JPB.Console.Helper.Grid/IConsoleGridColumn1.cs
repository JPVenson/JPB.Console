using System;

namespace JPB.Console.Helper.Grid.NetCore.Grid
{
	public interface IConsoleGridColumn1
	{
		Func<object, object> GetValue { get; set; }
		int MaxContentSize { get; set; }
		int MaxSize { get; }
		string Name { get; set; }
	}
}