using System;
using System.Linq;
using JPB.Console.Helper.Grid.NetCore.CommandDispatcher;

namespace JPB.Console.Helper.Grid.NetCore.Grid
{
	public class ConsoleGridControler<T> : ConsoleCommandDispatcher
	{
		public ConsoleGridControler()
		{
			ConsoleGrid = new ConsoleGrid<T>();

			Commands.Add(new DelegateCommand(ConsoleKey.DownArrow, (info) =>
			{
				if (FocusedRowIndex < ConsoleGrid.SourceList.Count)
				{
					FocusedRowIndex++;
					ConsoleGrid.FocusedItem = ConsoleGrid.SourceList[FocusedRowIndex - 1];
					ConsoleGrid.RenderGrid();
				}
			}));
			Commands.Add(new DelegateCommand(ConsoleKey.Delete, (info) =>
			{
				if (ConsoleGrid.SourceList.Any())
				{
					ConsoleGrid.SourceList.Remove(ConsoleGrid.SourceList[FocusedRowIndex - 1]);
					if (FocusedRowIndex > 1)
					{
						FocusedRowIndex--;
						ConsoleGrid.FocusedItem = ConsoleGrid.SourceList[FocusedRowIndex - 1];
					}
					else if (FocusedRowIndex < ConsoleGrid.SourceList.Count)
					{
						FocusedRowIndex++;
						ConsoleGrid.FocusedItem = ConsoleGrid.SourceList[FocusedRowIndex - 1];
					}
					ConsoleGrid.RenderGrid();
				}
			}));
			Commands.Add(new DelegateCommand(ConsoleKey.UpArrow, (info) =>
			{
				if (FocusedRowIndex > 1)
				{
					FocusedRowIndex--;
					ConsoleGrid.FocusedItem = ConsoleGrid.SourceList[FocusedRowIndex - 1];
					ConsoleGrid.RenderGrid();
				}
			}));
			Commands.Add(new DelegateCommand(ConsoleKey.Enter, (input) =>
			{
				if (input.Modifiers == ConsoleModifiers.Shift)
				{
					var max = ConsoleGrid.SelectedItems.Max(s => ConsoleGrid.SourceList.IndexOf(s));
					var min = ConsoleGrid.SelectedItems.Min(s => ConsoleGrid.SourceList.IndexOf(s));

					if (max != -1 || min != -1)
					{
						var orderAsc = FocusedRowIndex > max;

						for (int i = 0; i < ConsoleGrid.SourceList.Count; i++)
						{
							var source = ConsoleGrid.SourceList[i];
							if (ConsoleGrid.SelectedItems.Contains(source))
								continue;

							if (orderAsc)
							{
								if (i >= max && i < FocusedRowIndex)
								{
									ConsoleGrid.SelectedItems.Add(source);
								}
							}
							else
							{
								if (i >= FocusedRowIndex - 1 && i <= min)
								{
									ConsoleGrid.SelectedItems.Add(source);
								}
							}
						}
						ConsoleGrid.RenderGrid();
						return;
					}
				}

				if (input.Modifiers != ConsoleModifiers.Control)
				{
					ConsoleGrid.SelectedItems.Clear();
				}
				var val = ConsoleGrid.SourceList[FocusedRowIndex - 1];

				if (ConsoleGrid.SelectedItems.Contains(val))
				{
					ConsoleGrid.SelectedItems.Remove(val);
				}
				else
				{
					ConsoleGrid.SelectedItems.Add(val);
				}
			}));
		}

		public object FocusedRow { get; set; }
		public int FocusedRowIndex { get; set; }
		public ConsoleGrid<T> ConsoleGrid { get; set; }
	}
}