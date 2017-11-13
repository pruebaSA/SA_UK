namespace System.Windows.Markup
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class DictionaryKeyPropertyAttribute : Attribute
    {
        private string _name;

        public DictionaryKeyPropertyAttribute(string name)
        {
            this._name = name;
        }

        public string Name =>
            this._name;
    }
}

