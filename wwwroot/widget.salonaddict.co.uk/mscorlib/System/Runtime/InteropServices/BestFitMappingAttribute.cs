namespace System.Runtime.InteropServices
{
    using System;

    [ComVisible(true), AttributeUsage(AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Assembly, Inherited=false)]
    public sealed class BestFitMappingAttribute : Attribute
    {
        internal bool _bestFitMapping;
        public bool ThrowOnUnmappableChar;

        public BestFitMappingAttribute(bool BestFitMapping)
        {
            this._bestFitMapping = BestFitMapping;
        }

        public bool BestFitMapping =>
            this._bestFitMapping;
    }
}

