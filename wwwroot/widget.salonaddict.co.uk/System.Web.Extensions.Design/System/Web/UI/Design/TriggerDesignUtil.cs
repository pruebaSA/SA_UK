namespace System.Web.UI.Design
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Web.UI;

    internal static class TriggerDesignUtil
    {
        internal static bool IsValidTarget(IComponent component, IComponent rootComponent, bool requireSite)
        {
            Control control = component as Control;
            return ((((control != null) && (!requireSite || (control.Site != null))) && ((control != rootComponent) && !string.IsNullOrEmpty(control.ID))) && (((control is INamingContainer) || (control is IPostBackDataHandler)) || (control is IPostBackEventHandler)));
        }

        private static bool WalkChildren(IComponent rootComponent, Control control, bool isFirstLevel, bool requireSite, Predicate<IComponent> visitor, Control walkLimit)
        {
            if (control != walkLimit)
            {
                if (IsValidTarget(control, rootComponent, requireSite) && visitor(control))
                {
                    return true;
                }
                if (!isFirstLevel && (control is INamingContainer))
                {
                    return false;
                }
                foreach (Control control2 in control.Controls)
                {
                    if (WalkChildren(rootComponent, control2, false, requireSite, visitor, walkLimit))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static void WalkControlTree(Control startControl, Predicate<IComponent> visitor, bool requireSite, Control walkLimit)
        {
            IDesignerHost service = (IDesignerHost) startControl.Site.GetService(typeof(IDesignerHost));
            WalkChildren(service.RootComponent, startControl, true, requireSite, visitor, walkLimit);
        }
    }
}

