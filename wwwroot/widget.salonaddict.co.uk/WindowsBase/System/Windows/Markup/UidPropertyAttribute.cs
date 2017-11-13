namespace System.Windows.Markup
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public sealed class UidPropertyAttribute : Attribute
    {
        private string _name;

        public UidPropertyAttribute(string name)
        {
            this._name = name;
        }

        public string Name =>
            this._name;
    }
}

