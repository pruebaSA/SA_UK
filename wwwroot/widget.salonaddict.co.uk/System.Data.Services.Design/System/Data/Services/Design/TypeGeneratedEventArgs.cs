namespace System.Data.Services.Design
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;

    public sealed class TypeGeneratedEventArgs : EventArgs
    {
        private List<CodeAttributeDeclaration> _additionalAttributes;
        private List<Type> _additionalInterfaces;
        private List<CodeTypeMember> _additionalMembers;
        private CodeTypeReference _baseType;
        private GlobalItem _typeSource;

        public TypeGeneratedEventArgs()
        {
            this._additionalInterfaces = new List<Type>();
            this._additionalMembers = new List<CodeTypeMember>();
            this._additionalAttributes = new List<CodeAttributeDeclaration>();
        }

        public TypeGeneratedEventArgs(GlobalItem typeSource, CodeTypeReference baseType)
        {
            this._additionalInterfaces = new List<Type>();
            this._additionalMembers = new List<CodeTypeMember>();
            this._additionalAttributes = new List<CodeAttributeDeclaration>();
            this._typeSource = typeSource;
            this._baseType = baseType;
        }

        public List<CodeAttributeDeclaration> AdditionalAttributes =>
            this._additionalAttributes;

        public List<Type> AdditionalInterfaces =>
            this._additionalInterfaces;

        public List<CodeTypeMember> AdditionalMembers =>
            this._additionalMembers;

        public CodeTypeReference BaseType
        {
            get => 
                this._baseType;
            set
            {
                this._baseType = value;
            }
        }

        public GlobalItem TypeSource =>
            this._typeSource;
    }
}

