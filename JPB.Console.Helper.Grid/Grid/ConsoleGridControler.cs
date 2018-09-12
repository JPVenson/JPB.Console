using System;
using System.Linq;
using JPB.Console.Helper.Grid.CommandDispatcher;

namespace JPB.Console.Helper.Grid.Grid
{
	public class ConsoleGridControler<T> : ConsoleCommandDispatcher
	{
		public ConsoleGridControler(TextGrid<T> grid)
		{
			ProvideHistory = false;
			ProvideLookup = false;
			TextGrid = grid;
			Commands.Add(new DelegateCommand(ConsoleKey.DownArrow, info =>
			{
				if (FocusedRowIndex < TextGrid.SourceList.Count)
				{
					FocusedRowIndex++;
					TextGrid.FocusedItem = TextGrid.SourceList[FocusedRowIndex - 1];
					TextGrid.RenderGrid();
				}
			}));
			Commands.Add(new DelegateCommand(ConsoleKey.Delete, info =>
			{
				if (TextGrid.SourceList.Any())
				{
					TextGrid.SourceList.Remove(TextGrid.SourceList[FocusedRowIndex - 1]);
					if (FocusedRowIndex > 1)
					{
						FocusedRowIndex--;
						TextGrid.FocusedItem = TextGrid.SourceList[FocusedRowIndex - 1];
					}
					else if (FocusedRowIndex < TextGrid.SourceList.Count)
					{
						FocusedRowIndex++;
						TextGrid.FocusedItem = TextGrid.SourceList[FocusedRowIndex - 1];
					}

					TextGrid.RenderGrid();
				}
			}));
			Commands.Add(new DelegateCommand(ConsoleKey.UpArrow, info =>
			{
				if (FocusedRowIndex > 1)
				{
					FocusedRowIndex--;
					TextGrid.FocusedItem = TextGrid.SourceList[FocusedRowIndex - 1];
					TextGrid.RenderGrid();
				}
			}));
			Commands.Add(new DelegateCommand(ConsoleKey.Enter, input =>
			{
				if (input.Modifiers == ConsoleModifiers.Shift && AllowMultibeSelections)
				{
					var max = TextGrid.SelectedItems.Max(s => TextGrid.SourceList.IndexOf(s));
					var min = TextGrid.SelectedItems.Min(s => TextGrid.SourceList.IndexOf(s));

					if (max != -1 || min != -1)
					{
						var orderAsc = FocusedRowIndex > max;

						for (var i = 0; i < TextGrid.SourceList.Count; i++)
						{
							var source = TextGrid.SourceList[i];
							if (TextGrid.SelectedItems.Contains(source))
							{
								continue;
							}

							if (orderAsc)
							{
								if (i >= max && i < FocusedRowIndex)
								{
									TextGrid.SelectedItems.Add(source);
									OnItemSelected(source);
								}
							}
							else
							{
								if (i >= FocusedRowIndex - 1 && i <= min)
								{
									TextGrid.SelectedItems.Add(source);
									OnItemSelected(source);
								}
							}
						}

						TextGrid.RenderGrid();
						return;
					}
				}

				if (input.Modifiers != ConsoleModifiers.Control)
				{
					TextGrid.SelectedItems.Clear();
				}

				var val = TextGrid.SourceList[FocusedRowIndex - 1];

				if (TextGrid.SelectedItems.Contains(val))
				{
					TextGrid.SelectedItems.Remove(val);
				}
				else
				{
					TextGrid.SelectedItems.Add(val);
					OnItemSelected(val);
				}
			}));
		}

		public ConsoleGridControler() : this(new TextGrid<T>())
		{
		}

		public bool AllowMultibeSelections { get; set; }
		public int FocusedRowIndex { get; set; }
		public TextGrid<T> TextGrid { get; set; }

		public event EventHandler<T> ItemSelected;
		public event EventHandler<T> ItemFocused;

		protected virtual void OnItemFocused(T e)
		{
			ItemFocused?.Invoke(this, e);
		}

		protected virtual void OnItemSelected(T e)
		{
			ItemSelected?.Invoke(this, e);
		}
	}
}