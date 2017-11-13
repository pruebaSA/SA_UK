namespace System.Web.UI.WebControls
{
    using System;

    internal class LinqDataSourceContextData
    {
        private object _context;
        private bool _isNewContext;
        private object _table;

        public LinqDataSourceContextData()
        {
        }

        public LinqDataSourceContextData(object context)
        {
            this._context = context;
        }

        public object Context
        {
            get => 
                this._context;
            set
            {
                this._context = value;
            }
        }

        public bool IsNewContext
        {
            get => 
                this._isNewContext;
            set
            {
                this._isNewContext = value;
            }
        }

        public object Table
        {
            get => 
                this._table;
            set
            {
                this._table = value;
            }
        }
    }
}

