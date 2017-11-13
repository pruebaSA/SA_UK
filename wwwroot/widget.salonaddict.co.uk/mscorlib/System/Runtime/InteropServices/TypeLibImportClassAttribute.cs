namespace System.Runtime.InteropServices
{
    using System;

    [AttributeUsage(AttributeTargets.Interface, Inherited=false), ComVisible(true)]
    public sealed class TypeLibImportClassAttribute : Attribute
    {
        internal string _importClassName;

        public TypeLibImportClassAttribute(Type importClass)
        {
            this._importClassName = importClass.ToString();
        }

        public string Value =>
            this._importClassName;
    }
}

