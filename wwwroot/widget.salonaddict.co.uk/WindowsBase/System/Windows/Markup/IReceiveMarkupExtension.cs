namespace System.Windows.Markup
{
    using System;

    public interface IReceiveMarkupExtension
    {
        void ReceiveMarkupExtension(string property, MarkupExtension markupExtension, IServiceProvider serviceProvider);
    }
}

