namespace System.Data.Metadata.Edm
{
    using System;

    public sealed class Documentation : MetadataItem
    {
        private string _longDescription = "";
        private string _summary = "";

        internal Documentation()
        {
        }

        public override string ToString() => 
            this._summary;

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.Documentation;

        internal override string Identity =>
            "Documentation";

        public bool IsEmpty =>
            (string.IsNullOrEmpty(this._summary) && string.IsNullOrEmpty(this._longDescription));

        public string LongDescription
        {
            get => 
                this._longDescription;
            internal set
            {
                if (value != null)
                {
                    this._longDescription = value;
                }
                else
                {
                    this._longDescription = "";
                }
            }
        }

        public string Summary
        {
            get => 
                this._summary;
            internal set
            {
                if (value != null)
                {
                    this._summary = value;
                }
                else
                {
                    this._summary = "";
                }
            }
        }
    }
}

