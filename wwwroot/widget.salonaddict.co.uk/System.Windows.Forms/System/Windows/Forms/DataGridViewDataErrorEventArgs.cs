﻿namespace System.Windows.Forms
{
    using System;

    public class DataGridViewDataErrorEventArgs : DataGridViewCellCancelEventArgs
    {
        private DataGridViewDataErrorContexts context;
        private System.Exception exception;
        private bool throwException;

        public DataGridViewDataErrorEventArgs(System.Exception exception, int columnIndex, int rowIndex, DataGridViewDataErrorContexts context) : base(columnIndex, rowIndex)
        {
            this.exception = exception;
            this.context = context;
        }

        public DataGridViewDataErrorContexts Context =>
            this.context;

        public System.Exception Exception =>
            this.exception;

        public bool ThrowException
        {
            get => 
                this.throwException;
            set
            {
                if (value && (this.exception == null))
                {
                    throw new ArgumentException(System.Windows.Forms.SR.GetString("DataGridView_CannotThrowNullException"));
                }
                this.throwException = value;
            }
        }
    }
}

