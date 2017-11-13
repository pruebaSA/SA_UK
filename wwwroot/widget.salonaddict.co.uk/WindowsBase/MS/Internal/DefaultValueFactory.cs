namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Windows;

    [FriendAccessAllowed]
    internal abstract class DefaultValueFactory
    {
        protected DefaultValueFactory()
        {
        }

        internal abstract object CreateDefaultValue(DependencyObject owner, DependencyProperty property);

        internal abstract object DefaultValue { get; }
    }
}

