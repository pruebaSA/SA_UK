namespace System.Windows.Markup
{
    using System;

    public interface IComponentConnector
    {
        void Connect(int connectionId, object target);
        void InitializeComponent();
    }
}

