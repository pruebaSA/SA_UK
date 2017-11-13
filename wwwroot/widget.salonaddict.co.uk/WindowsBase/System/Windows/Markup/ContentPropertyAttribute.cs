namespace System.Windows.Markup
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class ContentPropertyAttribute : Attribute
    {
        private string _name;

        public ContentPropertyAttribute()
        {
        }

        public ContentPropertyAttribute(string name)
        {
            this._name = name;
        }

        public string Name =>
            this._name;
    }
}

