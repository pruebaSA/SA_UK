namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.RegularExpressions;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class BaseParser
    {
        private VirtualPath _baseVirtualDir;
        private VirtualPath _currentVirtualPath;
        private static Regex _directiveRegex;
        private static Regex _tagRegex;
        internal static readonly Regex aspCodeRegex = new AspCodeRegex();
        internal static readonly Regex aspExprRegex = new AspExprRegex();
        internal static readonly Regex commentRegex = new CommentRegex();
        internal static readonly Regex databindExprRegex = new DatabindExprRegex();
        internal static readonly Regex endtagRegex = new EndTagRegex();
        internal static readonly Regex gtRegex = new GTRegex();
        internal static readonly Regex includeRegex = new IncludeRegex();
        internal static readonly Regex ltRegex = new LTRegex();
        internal static readonly Regex runatServerRegex = new RunatServerRegex();
        internal static readonly Regex serverTagsRegex = new ServerTagsRegex();
        internal static readonly Regex textRegex = new TextRegex();

        internal static object CreateInstanceNoException(string typeName)
        {
            object obj2 = null;
            try
            {
                Type type = typeof(System.Web.RegularExpressions.TagRegex).Assembly.GetType(typeName);
                if (type != null)
                {
                    obj2 = Activator.CreateInstance(type);
                }
            }
            catch
            {
            }
            return obj2;
        }

        internal VirtualPath ResolveVirtualPath(VirtualPath virtualPath) => 
            VirtualPathProvider.CombineVirtualPathsInternal(this.CurrentVirtualPath, virtualPath);

        internal VirtualPath BaseVirtualDir =>
            this._baseVirtualDir;

        internal VirtualPath CurrentVirtualPath
        {
            get => 
                this._currentVirtualPath;
            set
            {
                this._currentVirtualPath = value;
                if (value != null)
                {
                    this._baseVirtualDir = value.Parent;
                }
            }
        }

        internal string CurrentVirtualPathString =>
            VirtualPath.GetVirtualPathString(this.CurrentVirtualPath);

        internal static Regex DirectiveRegex
        {
            get
            {
                if (_directiveRegex == null)
                {
                    if (AppSettings.UseStrictParserRegex)
                    {
                        _directiveRegex = (Regex) CreateInstanceNoException("System.Web.RegularExpressions.DirectiveRegex40");
                    }
                    if (_directiveRegex == null)
                    {
                        _directiveRegex = new System.Web.RegularExpressions.DirectiveRegex();
                    }
                }
                return _directiveRegex;
            }
        }

        internal static Regex TagRegex
        {
            get
            {
                if (_tagRegex == null)
                {
                    if (AppSettings.UseStrictParserRegex)
                    {
                        _tagRegex = (Regex) CreateInstanceNoException("System.Web.RegularExpressions.TagRegex40");
                    }
                    if (_tagRegex == null)
                    {
                        _tagRegex = new System.Web.RegularExpressions.TagRegex();
                    }
                }
                return _tagRegex;
            }
        }
    }
}

