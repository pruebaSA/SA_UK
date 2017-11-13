namespace System.Data.Metadata.Edm
{
    using System;

    internal class EdmItemError : EdmError
    {
        private MetadataItem _item;

        public EdmItemError(string message, MetadataItem item) : base(message)
        {
            this._item = item;
        }
    }
}

