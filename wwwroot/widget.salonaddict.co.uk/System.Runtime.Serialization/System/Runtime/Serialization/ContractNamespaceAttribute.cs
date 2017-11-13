namespace System.Runtime.Serialization
{
    using System;

    [AttributeUsage(AttributeTargets.Module | AttributeTargets.Assembly, Inherited=false, AllowMultiple=true)]
    public sealed class ContractNamespaceAttribute : Attribute
    {
        private string clrNamespace;
        private string contractNamespace;

        public ContractNamespaceAttribute(string contractNamespace)
        {
            this.contractNamespace = contractNamespace;
        }

        public string ClrNamespace
        {
            get => 
                this.clrNamespace;
            set
            {
                this.clrNamespace = value;
            }
        }

        public string ContractNamespace =>
            this.contractNamespace;
    }
}

