namespace System.Web.UI.WebControls
{
    using System;

    internal class LinqDataSourceEditData
    {
        private object _newDataObject;
        private object _originalDataObject;

        public object NewDataObject
        {
            get => 
                this._newDataObject;
            set
            {
                this._newDataObject = value;
            }
        }

        public object OriginalDataObject
        {
            get => 
                this._originalDataObject;
            set
            {
                this._originalDataObject = value;
            }
        }
    }
}

