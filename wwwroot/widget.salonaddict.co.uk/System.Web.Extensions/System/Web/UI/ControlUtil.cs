namespace System.Web.UI
{
    using System;

    internal class ControlUtil
    {
        internal static Control FindTargetControl(string controlID, Control control, bool searchNamingContainers)
        {
            if (searchNamingContainers)
            {
                Control namingContainer;
                Control control2 = null;
                if (control is INamingContainer)
                {
                    namingContainer = control;
                }
                else
                {
                    namingContainer = control.NamingContainer;
                }
                do
                {
                    control2 = namingContainer.FindControl(controlID);
                    namingContainer = namingContainer.NamingContainer;
                }
                while ((control2 == null) && (namingContainer != null));
                return control2;
            }
            return control.FindControl(controlID);
        }

        internal static bool IsBuiltInHiddenField(string hiddenFieldName)
        {
            if (hiddenFieldName.Length <= 2)
            {
                return false;
            }
            if ((hiddenFieldName[0] != '_') || (hiddenFieldName[1] != '_'))
            {
                return false;
            }
            if (((!hiddenFieldName.StartsWith("__VIEWSTATE", StringComparison.Ordinal) && !string.Equals(hiddenFieldName, "__EVENTVALIDATION", StringComparison.Ordinal)) && (!string.Equals(hiddenFieldName, "__LASTFOCUS", StringComparison.Ordinal) && !string.Equals(hiddenFieldName, "__SCROLLPOSITIONX", StringComparison.Ordinal))) && ((!string.Equals(hiddenFieldName, "__SCROLLPOSITIONY", StringComparison.Ordinal) && !string.Equals(hiddenFieldName, "__EVENTTARGET", StringComparison.Ordinal)) && !string.Equals(hiddenFieldName, "__EVENTARGUMENT", StringComparison.Ordinal)))
            {
                return string.Equals(hiddenFieldName, "__PREVIOUSPAGE", StringComparison.Ordinal);
            }
            return true;
        }
    }
}

