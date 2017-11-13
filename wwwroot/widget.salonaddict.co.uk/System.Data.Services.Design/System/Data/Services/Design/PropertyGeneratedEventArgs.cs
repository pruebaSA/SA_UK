namespace System.Data.Services.Design
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;

    public sealed class PropertyGeneratedEventArgs : EventArgs
    {
        private List<CodeAttributeDeclaration> _additionalAttributes;
        private List<CodeStatement> _additionalGetStatements;
        private List<CodeStatement> _additionalSetStatements;
        private List<CodeStatement> _additionalSetStatements2;
        private string _backingFieldName;
        private MetadataItem _propertySource;
        private CodeTypeReference _returnType;

        public PropertyGeneratedEventArgs()
        {
            this._additionalGetStatements = new List<CodeStatement>();
            this._additionalSetStatements = new List<CodeStatement>();
            this._additionalSetStatements2 = new List<CodeStatement>();
            this._additionalAttributes = new List<CodeAttributeDeclaration>();
        }

        public PropertyGeneratedEventArgs(MetadataItem propertySource, string backingFieldName, CodeTypeReference returnType)
        {
            this._additionalGetStatements = new List<CodeStatement>();
            this._additionalSetStatements = new List<CodeStatement>();
            this._additionalSetStatements2 = new List<CodeStatement>();
            this._additionalAttributes = new List<CodeAttributeDeclaration>();
            this._propertySource = propertySource;
            this._backingFieldName = backingFieldName;
            this._returnType = returnType;
        }

        internal List<CodeStatement> AdditionalAfterSetStatements =>
            this._additionalSetStatements2;

        public List<CodeAttributeDeclaration> AdditionalAttributes =>
            this._additionalAttributes;

        public List<CodeStatement> AdditionalGetStatements =>
            this._additionalGetStatements;

        public List<CodeStatement> AdditionalSetStatements =>
            this._additionalSetStatements;

        public string BackingFieldName =>
            this._backingFieldName;

        public MetadataItem PropertySource =>
            this._propertySource;

        public CodeTypeReference ReturnType
        {
            get => 
                this._returnType;
            set
            {
                this._returnType = value;
            }
        }
    }
}

