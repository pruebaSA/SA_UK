namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.ComponentModel.Design;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ThemeProvider
    {
        private int _contentHashCode;
        private string[] _cssFiles;
        private IDesignerHost _host;
        private IDictionary _skinBuilders;
        private string _themeName;
        private string _themePath;

        public ThemeProvider(IDesignerHost host, string name, string themeDefinition, string[] cssFiles, string themePath)
        {
            this._themeName = name;
            this._themePath = themePath;
            this._cssFiles = cssFiles;
            this._host = host;
            ControlBuilder builder = DesignTimeTemplateParser.ParseTheme(host, themeDefinition, themePath);
            this._contentHashCode = themeDefinition.GetHashCode();
            ArrayList subBuilders = builder.SubBuilders;
            this._skinBuilders = new Hashtable();
            for (int i = 0; i < subBuilders.Count; i++)
            {
                ControlBuilder builder2 = subBuilders[i] as ControlBuilder;
                if (builder2 != null)
                {
                    IDictionary dictionary = this._skinBuilders[builder2.ControlType] as IDictionary;
                    if (dictionary == null)
                    {
                        dictionary = new SortedList(StringComparer.OrdinalIgnoreCase);
                        this._skinBuilders[builder2.ControlType] = dictionary;
                    }
                    Control control = builder2.BuildObject() as Control;
                    if (control != null)
                    {
                        dictionary[control.SkinID] = builder2;
                    }
                }
            }
        }

        public SkinBuilder GetSkinBuilder(Control control)
        {
            IDictionary dictionary = this._skinBuilders[control.GetType()] as IDictionary;
            if (dictionary == null)
            {
                return null;
            }
            ControlBuilder skinBuilder = dictionary[control.SkinID] as ControlBuilder;
            if (skinBuilder == null)
            {
                return null;
            }
            return new SkinBuilder(this, control, skinBuilder, this._themePath);
        }

        public IDictionary GetSkinControlBuildersForControlType(Type type) => 
            (this._skinBuilders[type] as IDictionary);

        public ICollection GetSkinsForControl(Type type)
        {
            IDictionary dictionary = this._skinBuilders[type] as IDictionary;
            return dictionary?.Keys;
        }

        public int ContentHashCode =>
            this._contentHashCode;

        public ICollection CssFiles =>
            this._cssFiles;

        public IDesignerHost DesignerHost =>
            this._host;

        public string ThemeName =>
            this._themeName;
    }
}

