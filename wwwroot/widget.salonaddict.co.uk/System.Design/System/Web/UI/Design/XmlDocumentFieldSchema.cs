namespace System.Web.UI.Design
{
    using System;

    internal sealed class XmlDocumentFieldSchema : IDataSourceFieldSchema
    {
        private string _name;

        public XmlDocumentFieldSchema(string name)
        {
            this._name = name;
        }

        public Type DataType =>
            typeof(string);

        public bool Identity =>
            false;

        public bool IsReadOnly =>
            false;

        public bool IsUnique =>
            false;

        public int Length =>
            -1;

        public string Name =>
            this._name;

        public bool Nullable =>
            true;

        public int Precision =>
            -1;

        public bool PrimaryKey =>
            false;

        public int Scale =>
            -1;
    }
}

