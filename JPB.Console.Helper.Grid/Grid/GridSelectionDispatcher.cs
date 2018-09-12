using System;
using System.Linq;
using JPB.Console.Helper.Grid.CommandDispatcher;

namespace JPB.Console.Helper.Grid.Grid
{
	public class GridSelectionDispatcher<T>
	{
		private readonly TextGrid<T> _dataGrid;

		public GridSelectionDispatcher(TextGrid<T> dataGrid)
		{
			_dataGrid = dataGrid;
			Init();
		}

		public ConsoleCommandDispatcher Dispatcher { get; set; }

		private void Init()
		{
			Dispatcher.Commands.Add(new DelegateCommand(ConsoleKey.DownArrow, f =>
			{
				if (_dataGrid.FocusedItem == null)
				{
					_dataGrid.FocusedItem = _dataGrid.SourceList.FirstOrDefault();
				}

				var currentIndex = _dataGrid.SourceList.IndexOf(_dataGrid.FocusedItem);
				if (currentIndex != _dataGrid.SourceList.Count)
				{
					_dataGrid.FocusedItem = _dataGrid.SourceList.ElementAt(currentIndex + 1);
				}
			}));
			Dispatcher.Commands.Add(new DelegateCommand(ConsoleKey.UpArrow, f =>
			{
				if (_dataGrid.FocusedItem == null)
				{
					_dataGrid.FocusedItem = _dataGrid.SourceList.FirstOrDefault();
				}

				var currentIndex = _dataGrid.SourceList.IndexOf(_dataGrid.FocusedItem);
				if (currentIndex != 0)
				{
					_dataGrid.FocusedItem = _dataGrid.SourceList.ElementAt(currentIndex - 1);
				}
			}));
		}
	}
}