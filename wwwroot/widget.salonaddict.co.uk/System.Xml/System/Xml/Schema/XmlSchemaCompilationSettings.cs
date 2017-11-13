namespace System.Xml.Schema
{
    using System;

    public sealed class XmlSchemaCompilationSettings
    {
        private bool enableUpaCheck = true;

        public bool EnableUpaCheck
        {
            get => 
                this.enableUpaCheck;
            set
            {
                this.enableUpaCheck = value;
            }
        }
    }
}

