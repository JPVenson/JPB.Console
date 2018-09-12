namespace JPB.Console.Helper.Grid.Grid
{
	public interface IColumnInfo
	{
		ConsoleGridColumn ColumnElementInfo { get; set; }
		AlignedProperty Size { get; set; }
		string Value { get; set; }
	}
}