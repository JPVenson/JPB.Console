using System;
using System.Collections.Generic;
using System.Linq;

namespace JPB.Console.Helper.Grid.CommandDispatcher
{
	public class UserInputIndicator : IDisposable
	{
		internal UserInputIndicator()
		{
		}

		public void Dispose()
		{
			OnDisposed();
		}

		public event EventHandler Disposed;

		protected virtual void OnDisposed()
		{
			var handler = Disposed;
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}
	}

	/// <summary>
	///		This class can be used to create a Dispatcher on the console to provider lookup for multibe actions and boundrys.
	/// </summary>
	public class ConsoleCommandDispatcher
	{
		public static readonly string Pattern = @"(?:\{\*\})";
		public static readonly string Placeholder = "{*}";
		private int _currentHistoryElement;

		public ConsoleCommandDispatcher()
		{
			Commands = new List<IControlerCommand>();
			History = new List<string>();
			ProvideLookup = true;
			ProvideHistory = true;
		}

		public bool ShowAllMatchingElements { get; set; }
		public bool ProvideLookup { get; set; }
		public bool ProvideHistory { get; set; }
		public bool StopDispatcherLoop { get; set; }

		public List<IControlerCommand> Commands { get; }
		public List<string> History { get; }
		public event EventHandler<UserInputIndicator> UserInput;

		private string[] GetLookups(string input)
		{
			var lookups = Commands
				.Where(f => f.HandleString)
				.Where(f => !f.StringHandle.Contains(Placeholder))
				.Where(f => f.StringHandle.StartsWith(input));
			return lookups.Select(f => f.StringHandle).ToArray();
		}

		private void CleanupLine(int lastWrittenBytes, int startingLeft, int startingTop)
		{
			System.Console.CursorLeft = startingLeft;
			System.Console.CursorTop = startingTop;
			var spaceArray = "";
			for (var i = 0; i < lastWrittenBytes; i++)
			{
				spaceArray += "\0";
			}

			System.Console.Write(spaceArray);
			System.Console.CursorLeft = startingLeft;
			System.Console.CursorTop = startingTop;
		}

		public void Run()
		{
			StopDispatcherLoop = false;
			while (!StopDispatcherLoop)
			{
				var userInput = new UserInputIndicator();
				var startingLeft = System.Console.CursorLeft;
				var startingTop = System.Console.CursorTop;
				var lastWrittenBytes = 0;

				var input = System.Console.ReadKey(true);
				OnUserInput(userInput);

				if (Commands.Where(f => f.HandleKey).Any(controlerCommand => controlerCommand.Handle(input)))
				{
					History.Add(input.KeyChar.ToString());
					_currentHistoryElement++;
					userInput.Dispose();
					continue;
				}

				var fullInput = "";
				if (!char.IsControl(input.KeyChar))
				{
					fullInput += input.KeyChar;
				}

				var updated = false;
				HistoryLookup(input, ref fullInput, ref updated);

				if (ProvideLookup)
				{
					var exitLoop = false;
					var nextKey = input;

					do
					{
						CleanupLine(lastWrittenBytes, startingLeft, startingTop);

						lastWrittenBytes = fullInput.Length;
						System.Console.Write(fullInput);
						var currentLeft = System.Console.CursorLeft;
						var currentHeight = System.Console.CursorTop;

						var fuzzyNexts = GetLookups(fullInput);

						var fuzzyNext = fuzzyNexts.FirstOrDefault();
						if (fuzzyNext != null)
						{
							var toAutoComplete = fuzzyNext.Substring(fullInput.Length);
							if (fuzzyNexts.Length > 1 && ShowAllMatchingElements)
							{
								toAutoComplete +=
									fuzzyNexts.Skip(1)
										.Select(f => string.Format(" | {0}", f.Remove(0, fullInput.Length)))
										.Aggregate((e, f) => e + f);
							}

							var cColor = System.Console.ForegroundColor;
							System.Console.ForegroundColor = ConsoleColor.DarkGray;
							System.Console.Write(toAutoComplete);
							lastWrittenBytes += toAutoComplete.Length;
							System.Console.ForegroundColor = cColor;
							System.Console.CursorLeft = currentLeft;
							System.Console.CursorTop = currentHeight;
						}

						nextKey = System.Console.ReadKey(true);
						HistoryLookup(nextKey, ref fullInput, ref updated);
						if (nextKey.Key == ConsoleKey.Backspace)
						{
							var index = currentLeft - 1;
							if (index >= 0)
							{
								fullInput = fullInput.Remove(index, 1);
							}
							else
							{
								exitLoop = true;
								break;
							}
						}

						if (nextKey.Key == ConsoleKey.RightArrow)
						{
							fullInput = fuzzyNext;
						}

						if (!char.IsControl(nextKey.KeyChar))
						{
							fullInput = fullInput + nextKey.KeyChar;
						}

						if (string.IsNullOrEmpty(fullInput))
						{
							CleanupLine(lastWrittenBytes + 1, startingLeft, startingTop);
							exitLoop = true;
							break;
						}
					} while (nextKey.Key != ConsoleKey.Enter);

					if (exitLoop)
					{
						continue;
					}

					System.Console.WriteLine();

					foreach (var controlerCommand in Commands.Where(f => f.HandleString))
					{
						if (controlerCommand.Handle(fullInput))
						{
							History.Add(fullInput);
							_currentHistoryElement++;
							userInput.Dispose();
							break;
						}
					}
				}
				else
				{
					System.Console.Write(fullInput);
					fullInput += System.Console.ReadLine();
					foreach (var controlerCommand in Commands.Where(f => f.HandleString))
					{
						if (controlerCommand.Handle(fullInput))
						{
							History.Add(fullInput);
							_currentHistoryElement++;
							userInput.Dispose();
							break;
						}
					}
				}
			}
		}

		private void HistoryLookup(ConsoleKeyInfo input, ref string fullInput, ref bool updated)
		{
			updated = false;
			if (ProvideHistory)
			{
				if (input.Key == ConsoleKey.UpArrow)
				{
					if (_currentHistoryElement > 0)
					{
						_currentHistoryElement--;
					}

					if (_currentHistoryElement <= History.Count - 1)
					{
						updated = true;
						fullInput = History[_currentHistoryElement];
					}
				}

				if (input.Key == ConsoleKey.DownArrow)
				{
					if (_currentHistoryElement < History.Count - 1)
					{
						_currentHistoryElement++;
					}

					if (_currentHistoryElement <= History.Count - 1)
					{
						updated = true;
						fullInput = History[_currentHistoryElement];
					}
				}
			}
		}

		protected virtual void OnUserInput(UserInputIndicator e)
		{
			var handler = UserInput;
			if (handler != null)
			{
				handler(this, e);
			}
		}
	}
}