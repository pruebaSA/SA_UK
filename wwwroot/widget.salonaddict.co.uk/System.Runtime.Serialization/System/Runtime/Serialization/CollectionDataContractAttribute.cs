namespace System.Runtime.Serialization
{
    using System;

    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public sealed class CollectionDataContractAttribute : Attribute
    {
        private bool isItemNameSetExplicit;
        private bool isKeyNameSetExplicit;
        private bool isNameSetExplicit;
        private bool isNamespaceSetExplicit;
        private bool isReference;
        private bool isReferenceSetExplicit;
        private bool isValueNameSetExplicit;
        private string itemName;
        private string keyName;
        private string name;
        private string ns;
        private string valueName;

        internal bool IsItemNameSetExplicit =>
            this.isItemNameSetExplicit;

        internal bool IsKeyNameSetExplicit =>
            this.isKeyNameSetExplicit;

        internal bool IsNameSetExplicit =>
            this.isNameSetExplicit;

        internal bool IsNamespaceSetExplicit =>
            this.isNamespaceSetExplicit;

        public bool IsReference
        {
            get => 
                this.isReference;
            set
            {
                this.isReference = value;
                this.isReferenceSetExplicit = true;
            }
        }

        internal bool IsReferenceSetExplicit =>
            this.isReferenceSetExplicit;

        internal bool IsValueNameSetExplicit =>
            this.isValueNameSetExplicit;

        public string ItemName
        {
            get => 
                this.itemName;
            set
            {
                this.itemName = value;
                this.isItemNameSetExplicit = true;
            }
        }

        public string KeyName
        {
            get => 
                this.keyName;
            set
            {
                this.keyName = value;
                this.isKeyNameSetExplicit = true;
            }
        }

        public string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
                this.isNameSetExplicit = true;
            }
        }

        public string Namespace
        {
            get => 
                this.ns;
            set
            {
                this.ns = value;
                this.isNamespaceSetExplicit = true;
            }
        }

        public string ValueName
        {
            get => 
                this.valueName;
            set
            {
                this.valueName = value;
                this.isValueNameSetExplicit = true;
            }
        }
    }
}

