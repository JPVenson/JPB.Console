namespace JPB.Console.Helper.Grid.Grid
{
	public interface IDrawEnvironment
	{
		/// <summary>
		///     The max Width in Chars that can be drawn to
		/// </summary>
		int MaxWidth { get; }

		/// <summary>
		///     The Current Width the grid will be rendered in
		/// </summary>
		int Width { get; set; }
	}
}