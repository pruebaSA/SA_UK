namespace System.Data.Services.Client
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class MimeTypePropertyAttribute : Attribute
    {
        private readonly string dataPropertyName;
        private readonly string mimeTypePropertyName;

        public MimeTypePropertyAttribute(string dataPropertyName, string mimeTypePropertyName)
        {
            this.dataPropertyName = dataPropertyName;
            this.mimeTypePropertyName = mimeTypePropertyName;
        }

        public string DataPropertyName =>
            this.dataPropertyName;

        public string MimeTypePropertyName =>
            this.mimeTypePropertyName;
    }
}

