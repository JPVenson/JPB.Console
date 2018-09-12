#region

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

#endregion

namespace JPB.Console.Helper.Grid.DoubleBufferConsole
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Coord
	{
		public short X;
		public short Y;

		public Coord(short X, short Y)
		{
			this.X = X;
			this.Y = Y;
		}
	}

	[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
	public struct CharUnion
	{
		[FieldOffset(0)] public char UnicodeChar;

		[FieldOffset(0)] public byte AsciiChar;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct CharInfo
	{
		[FieldOffset(0)] public CharUnion Char;

		[FieldOffset(2)] public short Attributes;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SmallRect
	{
		public short Left;
		public short Top;
		public short Right;
		public short Bottom;
	}

	[Flags]
	[Serializable]
	internal enum Color : short
	{
		Black = 0,
		ForegroundBlue = 1,
		ForegroundGreen = 2,
		ForegroundRed = 4,
		ForegroundYellow = ForegroundRed | ForegroundGreen, // 0x0006
		ForegroundIntensity = 8,
		BackgroundBlue = 16, // 0x0010
		BackgroundGreen = 32, // 0x0020
		BackgroundRed = 64, // 0x0040
		BackgroundYellow = BackgroundRed | BackgroundGreen, // 0x0060
		BackgroundIntensity = 128, // 0x0080
		ForegroundMask = ForegroundIntensity | ForegroundYellow | ForegroundBlue, // 0x000F
		BackgroundMask = BackgroundIntensity | BackgroundYellow | BackgroundBlue, // 0x00F0
		ColorMask = BackgroundMask | ForegroundMask // 0x00FF
	}

	public class DoubleBufferConsole : TextWriter
	{
		private CharInfo[] _buffer;

		private SmallRect _positionInConsole;

		public DoubleBufferConsole()
		{
			_buffer = new CharInfo[200];
		}

		public int Postion { get; set; }

		public ConsoleColor ForegroundColor { get; set; } = System.Console.ForegroundColor;
		public ConsoleColor BackgroundColor { get; set; } = System.Console.BackgroundColor;

		public override Encoding Encoding { get; }

		public static object LockRoot { get; } = new object();

		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern SafeFileHandle CreateFile(
			string fileName,
			[MarshalAs(UnmanagedType.U4)] uint fileAccess,
			[MarshalAs(UnmanagedType.U4)] uint fileShare,
			IntPtr securityAttributes,
			[MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
			[MarshalAs(UnmanagedType.U4)] int flags,
			IntPtr template);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern bool WriteConsoleOutput(
			SafeFileHandle hConsoleOutput,
			CharInfo[] lpBuffer,
			Coord dwBufferSize,
			Coord dwBufferCoord,
			ref SmallRect lpWriteRegion);

		private Color TranslateConsoleColorToAttributeColor(ConsoleColor color, bool isBackground)
		{
			if ((color & ~ConsoleColor.White) != ConsoleColor.Black)
			{
				throw new ArgumentException("Invalid");
			}

			var color1 = (Color) color;
			if (isBackground)
			{
				color1 = (Color) ((int) color1 << 4);
			}

			return color1;
		}

		public override void Write(string value)
		{
			Write(value, ForegroundColor, BackgroundColor);
		}

		public void Write(string value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
		{
			var escapedValue = new StringBuilder();
			for (var i = 0; i < value.Length; i++)
			{
				var currentPosition = escapedValue.Length + Postion;
				var valChar = value[i];

				if (valChar == '\r')
				{
					escapedValue.Append(" ");
					continue;
				}

				if (valChar == '\n')
				{
					var rows = currentPosition / System.Console.BufferWidth;
					var currentPositionInRow = currentPosition - rows * System.Console.BufferWidth;

					var written = System.Console.BufferWidth - currentPositionInRow;
					for (var j = 0; j < written; j++)
					{
						escapedValue.Append(" ");
					}
				}
				else
				{
					escapedValue.Append(valChar);
				}
			}

			var nvalue = escapedValue.ToString();
			Expand(nvalue.Length + 1);

			foreach (var valChar in nvalue)
			{
				var asciiByte = new byte[1];
				Encoding.ASCII.GetBytes(valChar.ToString(), 0, 1, asciiByte, 0);
				var attributes = TranslateConsoleColorToAttributeColor(foregroundColor, false) |
				                 TranslateConsoleColorToAttributeColor(backgroundColor, true);

				_buffer[Postion] = new CharInfo
				{
					Char = new CharUnion
					{
						UnicodeChar = valChar
						//AsciiChar = asciiByte[0]
					},
					Attributes = (short) attributes
				};
				Postion++;
			}
		}

		private void Expand(int by)
		{
			if (_buffer.Length <= Postion + by)
			{
				var oldBuffer = _buffer.ToArray();
				_buffer = new CharInfo[Postion + by];
				oldBuffer.CopyTo(_buffer, 0);
			}
		}

		public void Flush(bool clear)
		{
			lock (LockRoot)
			{
				using (var h = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero))
				{
					if (h.IsInvalid)
					{
						return;
					}

					var endOfEditArea = _buffer.Length;

					if (!clear)
					{
						_positionInConsole.Top = (short) System.Console.CursorTop;
						_positionInConsole.Left = (short) System.Console.CursorLeft;

						if (_buffer.Length < System.Console.BufferWidth)
						{
							_positionInConsole.Right = (short) _buffer.Length;
						}
						else
						{
							_positionInConsole.Right = (short) System.Console.BufferWidth;
						}

						_positionInConsole.Bottom = (short) Math.Max(1, _buffer.Length / System.Console.BufferWidth);
					}
					else
					{
						//Top = (short)System.Console.CursorTop,
						//Left = (short)System.Console.CursorLeft,
						//Bottom = (short) System.Console.BufferHeight,
						//Right = (short) System.Console.BufferWidth
						_positionInConsole.Top = (short) System.Console.CursorTop;
						_positionInConsole.Left = (short) System.Console.CursorLeft;
						_positionInConsole.Right = (short) System.Console.BufferWidth;
						_positionInConsole.Bottom = (short) System.Console.BufferHeight;

						var left = _positionInConsole.Right * _positionInConsole.Bottom - Postion;
						Expand(left);
					}

					WriteConsoleOutput(h, _buffer, new Coord(_positionInConsole.Right, _positionInConsole.Bottom),
						new Coord(_positionInConsole.Left, _positionInConsole.Top),
						ref _positionInConsole);

					var rows = endOfEditArea / System.Console.BufferWidth;
					var currentPositionInRow = endOfEditArea - rows * System.Console.BufferWidth;

					System.Console.CursorTop = rows;
					System.Console.CursorLeft = currentPositionInRow;
				}
			}
		}

		public override void Flush()
		{
			Flush(false);
		}
	}
}