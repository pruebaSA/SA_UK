namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebColorConverter : ColorConverter
    {
        private static Hashtable htmlSysColorTable;

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string str = ((string) value).Trim();
                Color empty = Color.Empty;
                if (string.IsNullOrEmpty(str))
                {
                    return empty;
                }
                if (str[0] == '#')
                {
                    return base.ConvertFrom(context, culture, value);
                }
                if (StringUtil.EqualsIgnoreCase(str, "LightGrey"))
                {
                    return Color.LightGray;
                }
                if (htmlSysColorTable == null)
                {
                    InitializeHTMLSysColorTable();
                }
                object obj2 = htmlSysColorTable[str];
                if (obj2 != null)
                {
                    return (Color) obj2;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if ((destinationType == typeof(string)) && (value != null))
            {
                Color color = (Color) value;
                if (color == Color.Empty)
                {
                    return string.Empty;
                }
                if (!color.IsKnownColor)
                {
                    StringBuilder builder = new StringBuilder("#", 7);
                    builder.Append(color.R.ToString("X2", CultureInfo.InvariantCulture));
                    builder.Append(color.G.ToString("X2", CultureInfo.InvariantCulture));
                    builder.Append(color.B.ToString("X2", CultureInfo.InvariantCulture));
                    return builder.ToString();
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static void InitializeHTMLSysColorTable()
        {
            Hashtable hashtable = new Hashtable(StringComparer.OrdinalIgnoreCase) {
                ["activeborder"] = Color.FromKnownColor(KnownColor.ActiveBorder),
                ["activecaption"] = Color.FromKnownColor(KnownColor.ActiveCaption),
                ["appworkspace"] = Color.FromKnownColor(KnownColor.AppWorkspace),
                ["background"] = Color.FromKnownColor(KnownColor.Desktop),
                ["buttonface"] = Color.FromKnownColor(KnownColor.Control),
                ["buttonhighlight"] = Color.FromKnownColor(KnownColor.ControlLightLight),
                ["buttonshadow"] = Color.FromKnownColor(KnownColor.ControlDark),
                ["buttontext"] = Color.FromKnownColor(KnownColor.ControlText),
                ["captiontext"] = Color.FromKnownColor(KnownColor.ActiveCaptionText),
                ["graytext"] = Color.FromKnownColor(KnownColor.GrayText),
                ["highlight"] = Color.FromKnownColor(KnownColor.Highlight),
                ["highlighttext"] = Color.FromKnownColor(KnownColor.HighlightText),
                ["inactiveborder"] = Color.FromKnownColor(KnownColor.InactiveBorder),
                ["inactivecaption"] = Color.FromKnownColor(KnownColor.InactiveCaption),
                ["inactivecaptiontext"] = Color.FromKnownColor(KnownColor.InactiveCaptionText),
                ["infobackground"] = Color.FromKnownColor(KnownColor.Info),
                ["infotext"] = Color.FromKnownColor(KnownColor.InfoText),
                ["menu"] = Color.FromKnownColor(KnownColor.Menu),
                ["menutext"] = Color.FromKnownColor(KnownColor.MenuText),
                ["scrollbar"] = Color.FromKnownColor(KnownColor.ScrollBar),
                ["threeddarkshadow"] = Color.FromKnownColor(KnownColor.ControlDarkDark),
                ["threedface"] = Color.FromKnownColor(KnownColor.Control),
                ["threedhighlight"] = Color.FromKnownColor(KnownColor.ControlLight),
                ["threedlightshadow"] = Color.FromKnownColor(KnownColor.ControlLightLight),
                ["window"] = Color.FromKnownColor(KnownColor.Window),
                ["windowframe"] = Color.FromKnownColor(KnownColor.WindowFrame),
                ["windowtext"] = Color.FromKnownColor(KnownColor.WindowText)
            };
            htmlSysColorTable = hashtable;
        }
    }
}

