namespace System.Windows.Markup
{
    using System;

    public interface INameScope
    {
        object FindName(string name);
        void RegisterName(string name, object scopedElement);
        void UnregisterName(string name);
    }
}

