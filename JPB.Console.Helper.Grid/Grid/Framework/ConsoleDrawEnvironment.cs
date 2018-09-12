namespace JPB.Console.Helper.Grid.Grid
{
	public class ConsoleDrawEnvironment : IDrawEnvironment
	{
		public int Width
		{
			get => System.Console.WindowWidth;
			set => System.Console.WindowWidth = value;
		}

		public int MaxWidth => System.Console.LargestWindowWidth;
	}
}