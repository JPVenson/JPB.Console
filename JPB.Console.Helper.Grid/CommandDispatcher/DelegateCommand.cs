using System;

namespace JPB.Console.Helper.Grid.CommandDispatcher
{
	/// <summary>
	///		Defines an command that can be executed by the CommandDispatcher
	/// </summary>
	/// <seealso cref="DelegateCommand{T}" />
	public class DelegateCommand : DelegateCommand<string>
	{
		public DelegateCommand(string text, Action<string> callback) : base(text, callback)
		{
		}

		public DelegateCommand(string text, Func<string, bool> keyCallback) : base(text, keyCallback)
		{
		}

		public DelegateCommand(ConsoleKey key, Action<ConsoleKeyInfo> callback) : base(key, callback)
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DelegateCommand<T> : IControlerCommand
		where T : IConvertible
	{
		private readonly Func<T, bool> _fullTextCallback;
		private readonly ConsoleKey _key;
		private readonly Func<ConsoleKeyInfo, bool> _keyCallback;
		private readonly string _lookupString;

		private DelegateCommand(string lookupString)
		{
			StringComparison = StringComparison.OrdinalIgnoreCase;
			StringHandle = lookupString;
			_lookupString =
				lookupString; //.Replace(ConsoleCommandDispatcher.Placeholder, ConsoleCommandDispatcher.Pattern);
		}

		public DelegateCommand(ConsoleKey key, Action<ConsoleKeyInfo> callback) : this(key.ToString())
		{
			_key = key;
			_keyCallback = s =>
			{
				callback(s);
				return true;
			};
			HandleKey = true;
			HandleString = false;
		}

		public DelegateCommand(string text, Func<T, bool> keyCallback) : this(text)
		{
			_fullTextCallback = keyCallback;

			if (text.Length == 1)
			{
				HandleKey = true;
			}
			else
			{
				HandleString = true;
			}
		}

		public DelegateCommand(string text, Action<T> callback) : this(text, s =>
		{
			callback(s);
			return true;
		})
		{
		}

		public StringComparison StringComparison { get; set; }
		public bool HandleKey { get; set; }
		public bool HandleString { get; set; }

		public string StringHandle { get; }

		public string HelpText { get; set; }

		public bool Handle(string key)
		{
			if (key.StartsWith(_lookupString, StringComparison))
			{
				var sValue = key.Substring(StringHandle.Length);
				object convertedType;
				try
				{
					convertedType = Convert.ChangeType(sValue, typeof(T));
					if (convertedType == null)
					{
						return false;
					}
				}
				catch (Exception)
				{
					return false;
				}

				var changeType = (T) convertedType;
				return _fullTextCallback(changeType);
			}

			return false;
		}

		public bool Handle(ConsoleKeyInfo key)
		{
			if (key.Key == _key)
			{
				return _keyCallback(key);
			}

			return false;
		}

		public StringBuilderInterlaced Render()
		{
			var that = new StringBuilderInterlaced();
			that.Append("Keyword: ");
			if (HandleKey)
			{
				that.Append(_key.ToString(), ConsoleColor.Yellow);
			}
			else
			{
				that.Append(_lookupString, ConsoleColor.Yellow);
			}

			that.Append(", Argument: ");
			that.Append(typeof(T).Name, ConsoleColor.Yellow);
			if (!string.IsNullOrEmpty(HelpText))
			{
				that.Append(", Help: ");
				that.Append(HelpText, ConsoleColor.Green);
			}

			return that;
		}

		public override string ToString()
		{
			return string.Format(
				"StringComparison: {0}, HandleKey: {1}, HandleString: {2}, StringHandle: {3}, Argument Type: {4}, HelpText: {5}",
				StringComparison, HandleKey, HandleString, StringHandle, typeof(T).Name, HelpText);
		}
	}
}