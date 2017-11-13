namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;
    using System.Collections;

    internal class TableFormatInfo : FormatInfo
    {
        internal SortedList bottomBorderMap;
        internal SortedList connectedRowsMap;
        internal int endColumn = -1;
        internal int endRow = -1;
        internal SortedList formattedCells;
        internal bool isEnding;
        internal int lastHeaderRow = -1;
        internal MergedCellList mergedCells;
        internal int startColumn = -1;
        internal int startRow = -1;

        internal TableFormatInfo()
        {
        }

        internal override bool EndingIsComplete =>
            this.isEnding;

        internal override bool IsComplete =>
            false;

        internal override bool IsEmpty =>
            (this.startRow < 0);

        internal override bool IsEnding =>
            this.isEnding;

        internal override bool IsStarting =>
            (this.startRow == (this.lastHeaderRow + 1));

        internal override bool StartingIsComplete =>
            (!this.IsEmpty && (this.startRow > this.lastHeaderRow));
    }
}

