namespace System.Windows.Markup
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public sealed class ContentWrapperAttribute : Attribute
    {
        private Type _contentWrapper;

        public ContentWrapperAttribute(Type contentWrapper)
        {
            this._contentWrapper = contentWrapper;
        }

        public override bool Equals(object obj)
        {
            ContentWrapperAttribute attribute = obj as ContentWrapperAttribute;
            if (attribute == null)
            {
                return false;
            }
            return (this._contentWrapper == attribute._contentWrapper);
        }

        public override int GetHashCode() => 
            this._contentWrapper.GetHashCode();

        public Type ContentWrapper =>
            this._contentWrapper;

        public override object TypeId =>
            this;
    }
}

