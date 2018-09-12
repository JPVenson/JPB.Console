using System;

namespace JPB.Console.Helper.Grid.CommandDispatcher
{
	public interface IControlerCommand : IRenderObject
	{
		bool HandleKey { get; }
		bool HandleString { get; }
		string StringHandle { get; }
		string HelpText { get; set; }
		bool Handle(string key);
		bool Handle(ConsoleKeyInfo key);
	}

	public interface IRenderObject
	{
		StringBuilderInterlaced Render();
	}
}