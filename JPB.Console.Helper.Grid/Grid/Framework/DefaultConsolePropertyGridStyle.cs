using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JPB.Console.Helper.Grid.Grid
{
	public class ConsoleColumnStyle
	{
		public ConsoleColor? BackgroundColorStyle { get; set; }
		public ConsoleColor? ForegroundColorStyle { get; set; }

		public ConsoleCellStyle ConsoleCellStyle { get; set; }
	}

	public class DefaultConsolePropertyGridStyle : IConsolePropertyGridStyle
	{
		private int _width;

		public DefaultConsolePropertyGridStyle()
		{
			AlternatingTextStyle = ConsoleColor.Cyan;
			AlternatingTextBackgroundStyle = ConsoleColor.DarkGray;
			SelectedItemBackgroundStyle = ConsoleColor.DarkGray;
			SelectedItemForgroundStyle = ConsoleColor.Blue;
			FocusedItemBackgroundStyle = ConsoleColor.Gray;
			FocusedItemForgroundStyle = ConsoleColor.DarkRed;

			ColumnStyles = new Dictionary<string, ConsoleColumnStyle>();
		}

		public virtual bool DrawSpace => false;
		public IDictionary<string, ConsoleColumnStyle> ColumnStyles { get; set; }

		public virtual int RenderHeader(StringBuilderInterlaced stream, IList<ConsoleGridColumn> columnHeader)
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
			for (var i = 0; i < maxRows; i++)
			{
				stream.Append(VerticalLineSeperator);

				if (DrawSpace)
				{
					stream.Append(" ");
				}

				for (var index = 0; index < columnHeader.Count; index++)
				{
					var propName = columnHeader[index];
					if (DrawSpace && index != 0)
					{
						stream.Append(" ");
					}

					var columnOverwrite = ColumnStyles.FirstOrDefault(e => e.Key.Equals(propName.Name));
					stream.Append(propName.Name, columnOverwrite.Value?.ForegroundColorStyle,
						columnOverwrite.Value?.BackgroundColorStyle);

					if (DrawSpace)
					{
						stream.Append(" ");
					}

					if (index + 1 < columnHeader.Count)
					{
						stream.Append(VerticalLineSeperator);
					}
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

		public virtual void RenderFooter(StringBuilderInterlaced stream, IList<ConsoleGridColumn> columnHeader)
		{
			stream.Append(LowerLeftBound);
			DrawHorizontalLineEx(stream, columnHeader, '┴');
			stream.Append(LowerRightBound);
			//DrawHorizontalLine(stream, LowerLeftBound, LowerRightBound);
		}

		public virtual void RenderSummary(StringBuilderInterlaced stream, int sum)
		{
			var summery = sum + " items";
			RenderOnBottom(stream, summery);
		}

		public virtual void RenderAdditionalInfos(StringBuilderInterlaced stream, StringBuilder additional)
		{
			RenderOnBottom(stream, additional.ToString());
		}

		public virtual void BeginRenderProperty(StringBuilderInterlaced stream, int elementNr, int maxElements,
			bool isSelected, bool focused)
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

		public virtual void RenderNextProperty(StringBuilderInterlaced stream, string propertyValue,
			IColumnInfo columnData,
			object item,
			int elementNr, int columnNr, bool isSelected, bool focused)
		{
			//if (columnNr != 0)
			//{
			//	stream.Append(VerticalLineSeperator);
			//	if (DrawSpace)
			//	{
			//		stream.Append(" ");
			//	}
			//}
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
				var columnOverwrite =
					ColumnStyles.FirstOrDefault(e => e.Key.Equals(columnData.ColumnElementInfo.Name.Trim()));

				var targetForeground = columnOverwrite.Value?.ConsoleCellStyle?.GetCellForegroundColor(item);
				var targetBackground = columnOverwrite.Value?.ConsoleCellStyle?.GetCellBackgroundColor(item);

				if (elementNr % 2 != 0)
				{
					stream.Append(propertyValue, targetForeground ?? AlternatingTextStyle,
						targetBackground ?? AlternatingTextBackgroundStyle);
				}
				else
				{
					stream.Append(propertyValue, targetForeground, targetBackground);
				}
			}

			if (DrawSpace)
			{
				stream.Append(" ");
			}
		}


		public virtual void EndRenderProperty(StringBuilderInterlaced stream, int elementNr, bool isSelected,
			bool focused)
		{
			stream.Append(VerticalLineSeperator);
		}

		public virtual char VerticalLineSeperator => '│';

		public virtual char HorizontalLineSeperator => '─';

		public virtual char UpperLeftBound => '┌';

		public virtual char LowerLeftBound => '└';

		public virtual char UpperRightBound => '┐';

		public virtual char LowerRightBound => '┘';

		public virtual ConsoleColor AlternatingTextStyle { get; set; }
		public virtual ConsoleColor AlternatingTextBackgroundStyle { get; set; }
		public virtual ConsoleColor SelectedItemBackgroundStyle { get; set; }
		public virtual ConsoleColor SelectedItemForgroundStyle { get; set; }
		public virtual ConsoleColor FocusedItemBackgroundStyle { get; set; }
		public virtual ConsoleColor FocusedItemForgroundStyle { get; set; }

		protected virtual void DrawHorizontalLine(StringBuilderInterlaced stream, char start, char end, bool drawSpace,
			char seperator, int width)
		{
			stream.Append(start);

			if (drawSpace)
			{
				stream.Append(" ");
			}

			for (var i = 0; i < width - 1; i++)
			{
				stream.Append(seperator);
			}

			if (drawSpace)
			{
				stream.Append(" ");
			}

			stream.Append(end);
		}

		protected virtual void DrawHorizontalLineEx(StringBuilderInterlaced stream,
			IList<ConsoleGridColumn> headerInfos, char midChar)
		{
			var localCounter = 0;
			var elemntCounter = 0;
			if (DrawSpace)
			{
				stream.Append(" ");
			}

			for (var i = 0; i < _width - 1; i++)
			{
				var targetChar = HorizontalLineSeperator;

				if (headerInfos.Count - 1 > elemntCounter)
				{
					var element = headerInfos[elemntCounter];

					var lengthWithExt = element.Name.Length;
					if (localCounter >= lengthWithExt)
					{
						elemntCounter++;
						targetChar = midChar;
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

		public virtual void RenderOnBottom(StringBuilderInterlaced stream, string value)
		{
			var rowsOfValue = value.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

			foreach (var s in rowsOfValue)
			{
				if (string.IsNullOrWhiteSpace(s))
				{
					continue;
				}

				for (var i = 0; i < Math.Max(1, s.Length / _width); i++)
				{
					var line = s.Take(_width - 1).Select(e => e.ToString()).Aggregate((e, f) => e + f);
					var toEnd = _width - line.Length - 1;

					stream.Append(VerticalLineSeperator);
					if (DrawSpace)
					{
						stream.Append(" ");
					}

					stream.Append(line);

					for (var f = 0; f < toEnd; f++)
					{
						stream.Append(" ");
					}

					if (DrawSpace)
					{
						stream.Append(" ");
					}

					stream.AppendLine(VerticalLineSeperator.ToString());
				}
			}

			DrawHorizontalLine(stream, LowerLeftBound, LowerRightBound, DrawSpace, HorizontalLineSeperator, _width);
		}
	}
}