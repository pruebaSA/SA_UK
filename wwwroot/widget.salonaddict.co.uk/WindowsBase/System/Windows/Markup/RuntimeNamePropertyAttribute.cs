namespace System.Windows.Markup
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RuntimeNamePropertyAttribute : Attribute
    {
        private string _name;

        public RuntimeNamePropertyAttribute(string name)
        {
            this._name = name;
        }

        public string Name =>
            this._name;
    }
}

