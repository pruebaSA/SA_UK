namespace System.Windows.Markup
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public sealed class XmlLangPropertyAttribute : Attribute
    {
        private string _name;

        public XmlLangPropertyAttribute(string name)
        {
            this._name = name;
        }

        public string Name =>
            this._name;
    }
}

