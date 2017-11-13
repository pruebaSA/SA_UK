namespace System.Data.Metadata.Edm
{
    using System;

    public abstract class SimpleType : EdmType
    {
        internal SimpleType()
        {
        }

        internal SimpleType(string name, string namespaceName, DataSpace dataSpace) : base(name, namespaceName, dataSpace)
        {
        }
    }
}

