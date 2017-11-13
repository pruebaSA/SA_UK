namespace System.Runtime.Serialization
{
    using System;

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public sealed class DataContractAttribute : Attribute
    {
        private bool isNameSetExplicit;
        private bool isNamespaceSetExplicit;
        private bool isReference;
        private bool isReferenceSetExplicit;
        private string name;
        private string ns;

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
    }
}

