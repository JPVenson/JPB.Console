using System;

namespace JPB.Console.Helper.Grid.NetCore.CommandDispatcher
{
	public interface IControlerCommand
	{
		bool HandleKey { get; }
		bool HandleString { get; }
		string StringHandle { get; }
		bool Handle(string key);
		bool Handle(ConsoleKeyInfo key);
	}
}