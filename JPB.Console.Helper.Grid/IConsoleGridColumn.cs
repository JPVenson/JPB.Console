using System;

namespace JPB.Console.Helper.Grid.NetCore.Grid
{
	public interface IConsoleGridColumn
	{
		Func<object, object> GetValue { get; set; }
		int MaxContentSize { get; set; }
		int MaxSize { get; }
		string Name { get; set; }
	}
}