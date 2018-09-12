using System;

namespace JPB.Console.Helper.Grid.Grid
{
	public abstract class ConsoleCellStyle
	{
		public virtual ConsoleColor? GetCellForegroundColor(object data)
		{
			return null;
		}

		public virtual ConsoleColor? GetCellBackgroundColor(object data)
		{
			return null;
		}
	}

	public class DelegateConsoleCellStyle : ConsoleCellStyle
	{
		public Func<object, ConsoleColor?> Background { get; set; }
		public Func<object, ConsoleColor?> Foreground { get; set; }

		public override ConsoleColor? GetCellBackgroundColor(object data)
		{
			return Background == null ? null : Background(data);
		}

		public override ConsoleColor? GetCellForegroundColor(object data)
		{
			return Foreground == null ? null : Foreground(data);
		}
	}
}