﻿namespace System.Windows.Forms
{
    using System;
    using System.Drawing;

    public class DataGridViewRowPostPaintEventArgs : EventArgs
    {
        private Rectangle clipBounds;
        private DataGridView dataGridView;
        private string errorText;
        private System.Drawing.Graphics graphics;
        private DataGridViewCellStyle inheritedRowStyle;
        private bool isFirstDisplayedRow;
        private bool isLastVisibleRow;
        private Rectangle rowBounds;
        private int rowIndex;
        private DataGridViewElementStates rowState;

        internal DataGridViewRowPostPaintEventArgs(DataGridView dataGridView)
        {
            this.dataGridView = dataGridView;
        }

        public DataGridViewRowPostPaintEventArgs(DataGridView dataGridView, System.Drawing.Graphics graphics, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, string errorText, DataGridViewCellStyle inheritedRowStyle, bool isFirstDisplayedRow, bool isLastVisibleRow)
        {
            if (dataGridView == null)
            {
                throw new ArgumentNullException("dataGridView");
            }
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }
            if (inheritedRowStyle == null)
            {
                throw new ArgumentNullException("inheritedRowStyle");
            }
            this.dataGridView = dataGridView;
            this.graphics = graphics;
            this.clipBounds = clipBounds;
            this.rowBounds = rowBounds;
            this.rowIndex = rowIndex;
            this.rowState = rowState;
            this.errorText = errorText;
            this.inheritedRowStyle = inheritedRowStyle;
            this.isFirstDisplayedRow = isFirstDisplayedRow;
            this.isLastVisibleRow = isLastVisibleRow;
        }

        public void DrawFocus(Rectangle bounds, bool cellsPaintSelectionBackground)
        {
            if ((this.rowIndex < 0) || (this.rowIndex >= this.dataGridView.Rows.Count))
            {
                throw new InvalidOperationException(System.Windows.Forms.SR.GetString("DataGridViewElementPaintingEventArgs_RowIndexOutOfRange"));
            }
            this.dataGridView.Rows.SharedRow(this.rowIndex).DrawFocus(this.graphics, this.clipBounds, bounds, this.rowIndex, this.rowState, this.inheritedRowStyle, cellsPaintSelectionBackground);
        }

        public void PaintCells(Rectangle clipBounds, DataGridViewPaintParts paintParts)
        {
            if ((this.rowIndex < 0) || (this.rowIndex >= this.dataGridView.Rows.Count))
            {
                throw new InvalidOperationException(System.Windows.Forms.SR.GetString("DataGridViewElementPaintingEventArgs_RowIndexOutOfRange"));
            }
            this.dataGridView.Rows.SharedRow(this.rowIndex).PaintCells(this.graphics, clipBounds, this.rowBounds, this.rowIndex, this.rowState, this.isFirstDisplayedRow, this.isLastVisibleRow, paintParts);
        }

        public void PaintCellsBackground(Rectangle clipBounds, bool cellsPaintSelectionBackground)
        {
            if ((this.rowIndex < 0) || (this.rowIndex >= this.dataGridView.Rows.Count))
            {
                throw new InvalidOperationException(System.Windows.Forms.SR.GetString("DataGridViewElementPaintingEventArgs_RowIndexOutOfRange"));
            }
            DataGridViewPaintParts paintParts = DataGridViewPaintParts.Border | DataGridViewPaintParts.Background;
            if (cellsPaintSelectionBackground)
            {
                paintParts |= DataGridViewPaintParts.SelectionBackground;
            }
            this.dataGridView.Rows.SharedRow(this.rowIndex).PaintCells(this.graphics, clipBounds, this.rowBounds, this.rowIndex, this.rowState, this.isFirstDisplayedRow, this.isLastVisibleRow, paintParts);
        }

        public void PaintCellsContent(Rectangle clipBounds)
        {
            if ((this.rowIndex < 0) || (this.rowIndex >= this.dataGridView.Rows.Count))
            {
                throw new InvalidOperationException(System.Windows.Forms.SR.GetString("DataGridViewElementPaintingEventArgs_RowIndexOutOfRange"));
            }
            this.dataGridView.Rows.SharedRow(this.rowIndex).PaintCells(this.graphics, clipBounds, this.rowBounds, this.rowIndex, this.rowState, this.isFirstDisplayedRow, this.isLastVisibleRow, DataGridViewPaintParts.ErrorIcon | DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.ContentBackground);
        }

        public void PaintHeader(bool paintSelectionBackground)
        {
            DataGridViewPaintParts paintParts = DataGridViewPaintParts.ErrorIcon | DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.Border | DataGridViewPaintParts.Background;
            if (paintSelectionBackground)
            {
                paintParts |= DataGridViewPaintParts.SelectionBackground;
            }
            this.PaintHeader(paintParts);
        }

        public void PaintHeader(DataGridViewPaintParts paintParts)
        {
            if ((this.rowIndex < 0) || (this.rowIndex >= this.dataGridView.Rows.Count))
            {
                throw new InvalidOperationException(System.Windows.Forms.SR.GetString("DataGridViewElementPaintingEventArgs_RowIndexOutOfRange"));
            }
            this.dataGridView.Rows.SharedRow(this.rowIndex).PaintHeader(this.graphics, this.clipBounds, this.rowBounds, this.rowIndex, this.rowState, this.isFirstDisplayedRow, this.isLastVisibleRow, paintParts);
        }

        internal void SetProperties(System.Drawing.Graphics graphics, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, string errorText, DataGridViewCellStyle inheritedRowStyle, bool isFirstDisplayedRow, bool isLastVisibleRow)
        {
            this.graphics = graphics;
            this.clipBounds = clipBounds;
            this.rowBounds = rowBounds;
            this.rowIndex = rowIndex;
            this.rowState = rowState;
            this.errorText = errorText;
            this.inheritedRowStyle = inheritedRowStyle;
            this.isFirstDisplayedRow = isFirstDisplayedRow;
            this.isLastVisibleRow = isLastVisibleRow;
        }

        public Rectangle ClipBounds
        {
            get => 
                this.clipBounds;
            set
            {
                this.clipBounds = value;
            }
        }

        public string ErrorText =>
            this.errorText;

        public System.Drawing.Graphics Graphics =>
            this.graphics;

        public DataGridViewCellStyle InheritedRowStyle =>
            this.inheritedRowStyle;

        public bool IsFirstDisplayedRow =>
            this.isFirstDisplayedRow;

        public bool IsLastVisibleRow =>
            this.isLastVisibleRow;

        public Rectangle RowBounds =>
            this.rowBounds;

        public int RowIndex =>
            this.rowIndex;

        public DataGridViewElementStates State =>
            this.rowState;
    }
}

