namespace System.Windows.Markup
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public sealed class ConstructorArgumentAttribute : Attribute
    {
        private string _argumentName;

        public ConstructorArgumentAttribute(string argumentName)
        {
            this._argumentName = argumentName;
        }

        public string ArgumentName =>
            this._argumentName;
    }
}

