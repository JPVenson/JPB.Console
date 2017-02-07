using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JPB.Console.Helper.Grid.NetCore.Grid
{
	public class DefaultConsolePropertyGridStyle : IConsolePropertyGridStyle
	{
		public DefaultConsolePropertyGridStyle()
		{
			AlternatingTextStyle = ConsoleColor.Cyan;
			AlternatingTextBackgroundStyle = ConsoleColor.DarkGray;
			SelectedItemBackgroundStyle = ConsoleColor.DarkGray;
			SelectedItemForgroundStyle = ConsoleColor.Blue;
			FocusedItemBackgroundStyle = ConsoleColor.Gray;
			FocusedItemForgroundStyle = ConsoleColor.DarkRed;
		}

		private int _width;
		public bool DrawSpace { get { return false; }}

		private void DrawHorizontalLine(StringBuilderInterlaced stream, char start, char end)
		{
			stream.Append(start);

			if (DrawSpace)
			{
				stream.Append(" ");
			}

			for (int i = 0; i < _width - 1; i++)
			{
				stream.Append(HorizontalLineSeperator);
			}

			if (DrawSpace)
			{
				stream.Append(" ");
			}

			stream.Append(end);
		}

		private void DrawHorizontalLineEx(StringBuilderInterlaced stream, List<ConsoleGridColumn> headerInfos, char midChar)
		{
			var localCounter = 0;
			var elemntCounter = 0;
			if (DrawSpace)
			{
				stream.Append(" ");
			}
			for (int i = 0; i < _width - 1; i++)
			{
				var targetChar = HorizontalLineSeperator;

				if (headerInfos.Count - 1 > elemntCounter)
				{
					var element = headerInfos[elemntCounter];

					var lengthWithExt = element.Name.Length;
					if (localCounter >= lengthWithExt)
					{
						elemntCounter++;
						targetChar = (midChar);
						localCounter = 0;
					}
					else
					{
						localCounter++;
					}
				}
				stream.Append(targetChar);
			}
			if (DrawSpace)
			{
				stream.Append(" ");
			}
		}

		public int RenderHeader(StringBuilderInterlaced stream, List<ConsoleGridColumn> columnHeader)
		{
			_width = columnHeader.Sum(s => s.Name.Length) + columnHeader.Count;
			if (DrawSpace)
			{
				_width += columnHeader.Count * 2;
				_width -= 2;
			}

			stream.Append(UpperLeftBound);

			if (DrawSpace)
			{
				stream.Append(" ");
			}
			DrawHorizontalLineEx(stream, columnHeader, '┬');

			if (DrawSpace)
			{
				stream.Append(" ");
			}

			stream.Append(UpperRightBound);
			stream.AppendLine();

			var maxRows = columnHeader.Max(f => f.AlignedProperty.Rows);
			for (int i = 0; i < maxRows; i++)
			{
				stream.Append(VerticalLineSeperator);

				if (DrawSpace)
				{
					stream.Append(" ");
				}

				for (int index = 0; index < columnHeader.Count; index++)
				{
					var propName = columnHeader[index];
					if (DrawSpace && index != 0)
					{
						stream.Append(" ");
					}

					stream.Append(propName.Name);

					if (DrawSpace)
					{
						stream.Append(" ");
					}
					if (index + 1 < columnHeader.Count)
						stream.Append(VerticalLineSeperator);
				}

				stream.Append(VerticalLineSeperator);
				stream.AppendLine();
			}
			stream.Append('├');
			DrawHorizontalLineEx(stream, columnHeader, '┼');
			stream.Append('┤');

			//DrawHorizontalLine(stream, '├', '┤');
			return _width;
		}

		public void RenderFooter(StringBuilderInterlaced stream, List<ConsoleGridColumn> columnHeader)
		{
			stream.Append(LowerLeftBound);
			DrawHorizontalLineEx(stream, columnHeader, '┴');
			stream.Append(LowerRightBound);
			//DrawHorizontalLine(stream, LowerLeftBound, LowerRightBound);
		}

		public void RenderSummary(StringBuilderInterlaced stream, int sum)
		{
			var summery = sum + " items";
			RenderOnBottom(stream, summery);
		}

		public void RenderAdditionalInfos(StringBuilderInterlaced stream, StringBuilder additional)
		{
			RenderOnBottom(stream, additional.ToString());
		}

		public void BeginRenderProperty(StringBuilderInterlaced stream, int elementNr, int maxElements, bool isSelected, bool focused)
		{
			if (isSelected)
			{
				stream.Append('x');
			}
			else if (focused)
			{
				stream.Append('>');
			}
			else
			{
				stream.Append(VerticalLineSeperator);
			}

			if (DrawSpace)
			{
				stream.Append(" ");
			}
		}

		public void RenderNextProperty(StringBuilderInterlaced stream, string propertyValue, int elementNr, bool isSelected, bool focused)
		{
			if (elementNr != 0)
			{
				stream.Append(VerticalLineSeperator);
				if (DrawSpace)
				{
					stream.Append(" ");
				}
			}
			if (focused)
			{
				stream.Append(propertyValue, FocusedItemForgroundStyle, FocusedItemBackgroundStyle);
			}
			else if (isSelected)
			{
				stream.Append(propertyValue, SelectedItemForgroundStyle, SelectedItemBackgroundStyle);
			}
			else
			{
				if (elementNr % 2 != 0)
				{
					stream.Append(propertyValue, AlternatingTextStyle, AlternatingTextBackgroundStyle);
				}
				else
				{
					stream.Append(propertyValue);
				}
			}

			if (DrawSpace)
			{
				stream.Append(" ");
			}
		}


		public void EndRenderProperty(StringBuilderInterlaced stream, int elementNr, bool isSelected, bool focused)
		{
			stream.Append(VerticalLineSeperator);
		}

		private void RenderOnBottom(StringBuilderInterlaced stream, string value)
		{
			stream.Append(VerticalLineSeperator);
			if (DrawSpace)
			{
				stream.Append(" ");
			}
			stream.Append(value);
			var toEnd = _width - value.Length;

			for (int i = 0; i < toEnd - 1; i++)
			{
				stream.Append(" ");
			}

			if (DrawSpace)
			{
				stream.Append(" ");
			}

			stream.AppendLine(VerticalLineSeperator.ToString());
			DrawHorizontalLine(stream, LowerLeftBound, LowerRightBound);
		}

		public char VerticalLineSeperator
		{
			get { return '│'; }
		}

		public char HorizontalLineSeperator
		{
			get { return '─'; }
		}

		public char UpperLeftBound
		{
			get { return '┌'; }
		}

		public char LowerLeftBound
		{
			get { return '└'; }
		}

		public char UpperRightBound
		{
			get { return '┐'; }
		}

		public char LowerRightBound
		{
			get { return '┘'; }
		}

		public ConsoleColor AlternatingTextStyle { get; set; }
		public ConsoleColor AlternatingTextBackgroundStyle { get; set; }
		public ConsoleColor SelectedItemBackgroundStyle { get; set; }
		public ConsoleColor SelectedItemForgroundStyle { get; set; }

		public ConsoleColor FocusedItemBackgroundStyle { get; set; }
		public ConsoleColor FocusedItemForgroundStyle { get; set; }
	}
}