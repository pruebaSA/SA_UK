namespace PdfSharp.Pdf
{
    using System;

    internal class KeyInfoAttribute : Attribute
    {
        private PdfSharp.Pdf.KeyType entryType;
        private string fixedValue;
        private Type objectType;
        private string version;

        public KeyInfoAttribute()
        {
            this.version = "1.0";
        }

        public KeyInfoAttribute(PdfSharp.Pdf.KeyType keyType)
        {
            this.version = "1.0";
            this.KeyType = keyType;
        }

        public KeyInfoAttribute(PdfSharp.Pdf.KeyType keyType, Type objectType)
        {
            this.version = "1.0";
            this.KeyType = keyType;
            this.objectType = objectType;
        }

        public KeyInfoAttribute(string version, PdfSharp.Pdf.KeyType keyType)
        {
            this.version = "1.0";
            this.version = version;
            this.KeyType = keyType;
        }

        public KeyInfoAttribute(string version, PdfSharp.Pdf.KeyType keyType, Type objectType)
        {
            this.version = "1.0";
            this.KeyType = keyType;
            this.objectType = objectType;
        }

        public string FixedValue
        {
            get => 
                this.fixedValue;
            set
            {
                this.fixedValue = value;
            }
        }

        public PdfSharp.Pdf.KeyType KeyType
        {
            get => 
                this.entryType;
            set
            {
                this.entryType = value;
            }
        }

        public Type ObjectType
        {
            get => 
                this.objectType;
            set
            {
                this.objectType = value;
            }
        }

        public string Version
        {
            get => 
                this.version;
            set
            {
                this.version = value;
            }
        }
    }
}

