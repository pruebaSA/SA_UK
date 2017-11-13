namespace System.Windows.Markup
{
    using System;

    public abstract class MarkupExtension
    {
        protected MarkupExtension()
        {
        }

        public abstract object ProvideValue(IServiceProvider serviceProvider);
    }
}

