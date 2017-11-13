namespace System.Drawing.Design
{
    using System;
    using System.Windows.Forms;

    public sealed class ToolboxItemCreator
    {
        private ToolboxItemCreatorCallback _callback;
        private string _format;

        internal ToolboxItemCreator(ToolboxItemCreatorCallback callback, string format)
        {
            this._callback = callback;
            this._format = format;
        }

        public ToolboxItem Create(IDataObject data) => 
            this._callback(data, this._format);

        public string Format =>
            this._format;
    }
}

