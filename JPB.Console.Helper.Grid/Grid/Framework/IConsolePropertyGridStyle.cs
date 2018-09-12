using System;
using System.Collections.Generic;
using System.Text;

namespace JPB.Console.Helper.Grid.Grid
{
	public interface IConsolePropertyGridStyle
	{
		IDictionary<string, ConsoleColumnStyle> ColumnStyles { get; }

		char VerticalLineSeperator { get; }
		char HorizontalLineSeperator { get; }

		char UpperLeftBound { get; }
		char LowerLeftBound { get; }

		char UpperRightBound { get; }
		char LowerRightBound { get; }

		ConsoleColor AlternatingTextStyle { get; set; }
		ConsoleColor AlternatingTextBackgroundStyle { get; set; }

		ConsoleColor SelectedItemBackgroundStyle { get; set; }
		ConsoleColor SelectedItemForgroundStyle { get; set; }

		ConsoleColor FocusedItemBackgroundStyle { get; set; }
		ConsoleColor FocusedItemForgroundStyle { get; set; }
		int RenderHeader(StringBuilderInterlaced stream, IList<ConsoleGridColumn> columnHeader);

		void BeginRenderProperty(StringBuilderInterlaced stream, int elementNr, int maxElements, bool isSelected,
			bool focused);

		void RenderNextProperty(StringBuilderInterlaced stream, string propertyValue, IColumnInfo column, object item,
			int elementNr,
			int columnNr, bool isSelected, bool focused);

		void EndRenderProperty(StringBuilderInterlaced stream, int elementNr, bool isSelected, bool focused);

		void RenderFooter(StringBuilderInterlaced stream, IList<ConsoleGridColumn> columnHeader);
		void RenderSummary(StringBuilderInterlaced stream, int sum);
		void RenderAdditionalInfos(StringBuilderInterlaced stream, StringBuilder additional);
	}
}