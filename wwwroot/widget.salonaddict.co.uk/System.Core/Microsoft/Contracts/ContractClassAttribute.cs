namespace Microsoft.Contracts
{
    using System;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    internal sealed class ContractClassAttribute : Attribute
    {
        private System.Type _typeWithContracts;

        public ContractClassAttribute(System.Type t)
        {
            this._typeWithContracts = t;
        }

        public System.Type Type =>
            this._typeWithContracts;
    }
}

