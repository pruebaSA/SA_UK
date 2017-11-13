namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.Configuration;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Medium), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Medium)]
    public abstract class PageParserFilter
    {
        private int _currentControlCount;
        private int _currentDependenciesCount;
        private int _currentDirectDependenciesCount;
        private int _dependenciesAllowed;
        private int _directDependenciesAllowed;
        private int _numberOfControlsAllowed;
        private TemplateParser _parser;
        private System.Web.VirtualPath _virtualPath;

        protected PageParserFilter()
        {
        }

        protected void AddControl(Type type, IDictionary attributes)
        {
            this._parser.AddControl(type, attributes);
        }

        public virtual bool AllowBaseType(Type baseType) => 
            false;

        public virtual bool AllowControl(Type controlType, ControlBuilder builder) => 
            false;

        internal bool AllowControlInternal(Type controlType, ControlBuilder builder)
        {
            this.OnControlAdded();
            return this.AllowControl(controlType, builder);
        }

        public virtual bool AllowServerSideInclude(string includeVirtualPath) => 
            false;

        public virtual bool AllowVirtualReference(string referenceVirtualPath, VirtualReferenceType referenceType) => 
            false;

        internal bool AllowVirtualReference(CompilationSection compConfig, System.Web.VirtualPath referenceVirtualPath)
        {
            VirtualReferenceType page;
            string extension = referenceVirtualPath.Extension;
            Type type = CompilationUtil.GetBuildProviderTypeFromExtension(compConfig, extension, BuildProviderAppliesTo.Web, false);
            if (type == null)
            {
                return false;
            }
            if (type == typeof(PageBuildProvider))
            {
                page = VirtualReferenceType.Page;
            }
            else if (type == typeof(UserControlBuildProvider))
            {
                page = VirtualReferenceType.UserControl;
            }
            else if (type == typeof(MasterPageBuildProvider))
            {
                page = VirtualReferenceType.Master;
            }
            else if (type == typeof(SourceFileBuildProvider))
            {
                page = VirtualReferenceType.SourceFile;
            }
            else
            {
                page = VirtualReferenceType.Other;
            }
            return this.AllowVirtualReference(referenceVirtualPath.VirtualPathString, page);
        }

        internal static PageParserFilter Create(PagesSection pagesConfig, System.Web.VirtualPath virtualPath, TemplateParser parser)
        {
            PageParserFilter filter = pagesConfig.CreateControlTypeFilter();
            if (filter != null)
            {
                filter.InitializeInternal(virtualPath, parser);
            }
            return filter;
        }

        public virtual CompilationMode GetCompilationMode(CompilationMode current) => 
            current;

        public virtual Type GetNoCompileUserControlType() => 
            null;

        protected virtual void Initialize()
        {
        }

        internal void InitializeInternal(System.Web.VirtualPath virtualPath, TemplateParser parser)
        {
            this._parser = parser;
            this._virtualPath = virtualPath;
            this.Initialize();
            this._numberOfControlsAllowed = this.NumberOfControlsAllowed;
            this._dependenciesAllowed = this.TotalNumberOfDependenciesAllowed + 1;
            this._directDependenciesAllowed = this.NumberOfDirectDependenciesAllowed + 1;
        }

        private void OnControlAdded()
        {
            if (this._numberOfControlsAllowed >= 0)
            {
                this._currentControlCount++;
                if (this._currentControlCount > this._numberOfControlsAllowed)
                {
                    throw new HttpException(System.Web.SR.GetString("Too_many_controls", new object[] { this._numberOfControlsAllowed.ToString(CultureInfo.CurrentCulture) }));
                }
            }
        }

        internal void OnDependencyAdded()
        {
            if (this._dependenciesAllowed > 0)
            {
                this._currentDependenciesCount++;
                if (this._currentDependenciesCount > this._dependenciesAllowed)
                {
                    throw new HttpException(System.Web.SR.GetString("Too_many_dependencies", new object[] { this.VirtualPath, this._dependenciesAllowed.ToString(CultureInfo.CurrentCulture) }));
                }
            }
        }

        internal void OnDirectDependencyAdded()
        {
            if (this._directDependenciesAllowed > 0)
            {
                this._currentDirectDependenciesCount++;
                if (this._currentDirectDependenciesCount > this._directDependenciesAllowed)
                {
                    throw new HttpException(System.Web.SR.GetString("Too_many_direct_dependencies", new object[] { this.VirtualPath, this._directDependenciesAllowed.ToString(CultureInfo.CurrentCulture) }));
                }
            }
        }

        public virtual void ParseComplete(ControlBuilder rootBuilder)
        {
        }

        public virtual void PreprocessDirective(string directiveName, IDictionary attributes)
        {
        }

        public virtual bool ProcessCodeConstruct(CodeConstructType codeType, string code) => 
            false;

        public virtual bool ProcessDataBindingAttribute(string controlId, string name, string value) => 
            false;

        public virtual bool ProcessEventHookup(string controlId, string eventName, string handlerName) => 
            false;

        protected void SetPageProperty(string filter, string name, string value)
        {
            if (filter == null)
            {
                filter = string.Empty;
            }
            this._parser.RootBuilder.PreprocessAttribute(filter, name, value, true);
        }

        public virtual bool AllowCode =>
            false;

        protected int Line =>
            this._parser._lineNumber;

        public virtual int NumberOfControlsAllowed =>
            0;

        public virtual int NumberOfDirectDependenciesAllowed =>
            0;

        public virtual int TotalNumberOfDependenciesAllowed =>
            0;

        protected string VirtualPath =>
            this._virtualPath.VirtualPathString;
    }
}

