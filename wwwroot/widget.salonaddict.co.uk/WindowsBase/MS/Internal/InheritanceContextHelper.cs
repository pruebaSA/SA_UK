namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Windows;

    internal static class InheritanceContextHelper
    {
        [FriendAccessAllowed]
        internal static void AddInheritanceContext(DependencyObject newInheritanceContext, DependencyObject value, ref bool hasMultipleInheritanceContexts, ref DependencyObject inheritanceContext)
        {
            if ((newInheritanceContext != inheritanceContext) && !hasMultipleInheritanceContexts)
            {
                if ((inheritanceContext == null) || (newInheritanceContext == null))
                {
                    inheritanceContext = newInheritanceContext;
                }
                else
                {
                    hasMultipleInheritanceContexts = true;
                    inheritanceContext = null;
                }
                value.OnInheritanceContextChanged(EventArgs.Empty);
            }
        }

        [FriendAccessAllowed]
        internal static void ProvideContextForObject(DependencyObject context, DependencyObject newValue)
        {
            if (context != null)
            {
                context.ProvideSelfAsInheritanceContext(newValue, null);
            }
        }

        [FriendAccessAllowed]
        internal static void RemoveContextFromObject(DependencyObject context, DependencyObject oldValue)
        {
            if ((context != null) && (oldValue.InheritanceContext == context))
            {
                context.RemoveSelfAsInheritanceContext(oldValue, null);
            }
        }

        [FriendAccessAllowed]
        internal static void RemoveInheritanceContext(DependencyObject oldInheritanceContext, DependencyObject value, ref bool hasMultipleInheritanceContexts, ref DependencyObject inheritanceContext)
        {
            if ((oldInheritanceContext == inheritanceContext) && !hasMultipleInheritanceContexts)
            {
                inheritanceContext = null;
                value.OnInheritanceContextChanged(EventArgs.Empty);
            }
        }
    }
}

