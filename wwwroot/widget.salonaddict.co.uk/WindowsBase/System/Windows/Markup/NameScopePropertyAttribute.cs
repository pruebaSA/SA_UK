namespace System.Windows.Markup
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class NameScopePropertyAttribute : Attribute
    {
        private string _name;
        private System.Type _type;

        public NameScopePropertyAttribute(string name)
        {
            this._name = name;
        }

        public NameScopePropertyAttribute(string name, System.Type type)
        {
            this._name = name;
            this._type = type;
        }

        public string Name =>
            this._name;

        public System.Type Type =>
            this._type;
    }
}

