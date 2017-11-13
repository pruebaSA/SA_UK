namespace System.Windows.Markup
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple=true)]
    public sealed class DependsOnAttribute : Attribute
    {
        private string _name;

        public DependsOnAttribute(string name)
        {
            this._name = name;
        }

        public string Name =>
            this._name;

        public override object TypeId =>
            this;
    }
}

