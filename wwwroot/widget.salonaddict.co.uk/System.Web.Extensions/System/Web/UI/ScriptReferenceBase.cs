namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;
    using System.Web.UI.WebControls;

    [DefaultProperty("Path"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class ScriptReferenceBase
    {
        private bool _alwaysLoadBeforeUI;
        private IClientUrlResolver _clientUrlResolver;
        private Control _containingControl;
        private bool _isStaticReference;
        private bool _notifyScriptLoaded = true;
        private string _path;
        private string[] _resourceUICultures;
        private System.Web.UI.ScriptMode _scriptMode;

        protected ScriptReferenceBase()
        {
        }

        internal static string GetDebugPath(string releasePath)
        {
            string str;
            string str2;
            if (releasePath.IndexOf('?') >= 0)
            {
                int index = releasePath.IndexOf('?');
                str = releasePath.Substring(0, index);
                str2 = releasePath.Substring(index);
            }
            else
            {
                str = releasePath;
                str2 = null;
            }
            if (!str.EndsWith(".js", StringComparison.Ordinal))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, AtlasWeb.ScriptReference_InvalidReleaseScriptPath, new object[] { str }));
            }
            return (ReplaceExtension(str) + str2);
        }

        protected internal abstract string GetUrl(ScriptManager scriptManager, bool zip);
        protected internal abstract bool IsFromSystemWebExtensions();
        protected static string ReplaceExtension(string pathOrName) => 
            (pathOrName.Substring(0, pathOrName.Length - 2) + "debug.js");

        internal bool AlwaysLoadBeforeUI
        {
            get => 
                this._alwaysLoadBeforeUI;
            set
            {
                this._alwaysLoadBeforeUI = value;
            }
        }

        internal IClientUrlResolver ClientUrlResolver
        {
            get => 
                this._clientUrlResolver;
            set
            {
                this._clientUrlResolver = value;
            }
        }

        internal Control ContainingControl
        {
            get => 
                this._containingControl;
            set
            {
                this._containingControl = value;
            }
        }

        internal bool IsStaticReference
        {
            get => 
                this._isStaticReference;
            set
            {
                this._isStaticReference = value;
            }
        }

        [ResourceDescription("ScriptReference_NotifyScriptLoaded"), Category("Behavior"), DefaultValue(true), NotifyParentProperty(true)]
        public bool NotifyScriptLoaded
        {
            get => 
                this._notifyScriptLoaded;
            set
            {
                this._notifyScriptLoaded = value;
            }
        }

        [Category("Behavior"), UrlProperty("*.js"), DefaultValue(""), NotifyParentProperty(true), ResourceDescription("ScriptReference_Path")]
        public string Path
        {
            get
            {
                if (this._path != null)
                {
                    return this._path;
                }
                return string.Empty;
            }
            set
            {
                this._path = value;
            }
        }

        [DefaultValue((string) null), ResourceDescription("ScriptReference_ResourceUICultures"), NotifyParentProperty(true), Category("Behavior"), MergableProperty(false), TypeConverter(typeof(StringArrayConverter))]
        public string[] ResourceUICultures
        {
            get => 
                this._resourceUICultures;
            set
            {
                this._resourceUICultures = value;
            }
        }

        [ResourceDescription("ScriptReference_ScriptMode"), Category("Behavior"), DefaultValue(0), NotifyParentProperty(true)]
        public System.Web.UI.ScriptMode ScriptMode
        {
            get => 
                this._scriptMode;
            set
            {
                if ((value < System.Web.UI.ScriptMode.Auto) || (value > System.Web.UI.ScriptMode.Release))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._scriptMode = value;
            }
        }
    }
}

