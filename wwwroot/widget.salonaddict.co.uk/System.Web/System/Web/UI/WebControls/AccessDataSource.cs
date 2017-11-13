namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Caching;
    using System.Web.UI;
    using System.Web.Util;

    [WebSysDisplayName("AccessDataSource_DisplayName"), Designer("System.Web.UI.Design.WebControls.AccessDataSourceDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ToolboxBitmap(typeof(AccessDataSource)), WebSysDescription("AccessDataSource_Description"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class AccessDataSource : SqlDataSource
    {
        private System.Web.UI.FileDataSourceCache _cache;
        private string _connectionString;
        private string _dataFile;
        private string _physicalDataFile;
        private const string OleDbProviderName = "System.Data.OleDb";

        public AccessDataSource()
        {
        }

        public AccessDataSource(string dataFile, string selectCommand)
        {
            if (string.IsNullOrEmpty(dataFile))
            {
                throw new ArgumentNullException("dataFile");
            }
            this.DataFile = dataFile;
            base.SelectCommand = selectCommand;
        }

        private void AddCacheFileDependency()
        {
            this.FileDataSourceCache.FileDependencies.Clear();
            string physicalDataFile = this.PhysicalDataFile;
            if (physicalDataFile.Length > 0)
            {
                this.FileDataSourceCache.FileDependencies.Add(physicalDataFile);
            }
        }

        private string CreateConnectionString() => 
            ("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + this.PhysicalDataFile);

        protected override SqlDataSourceView CreateDataSourceView(string viewName) => 
            new AccessDataSourceView(this, viewName, this.Context);

        protected override DbProviderFactory GetDbProviderFactory() => 
            OleDbFactory.Instance;

        private string GetPhysicalDataFilePath()
        {
            string dataFile = this.DataFile;
            if (dataFile.Length == 0)
            {
                return null;
            }
            if (!UrlPath.IsAbsolutePhysicalPath(dataFile))
            {
                if (base.DesignMode)
                {
                    throw new NotSupportedException(System.Web.SR.GetString("AccessDataSource_DesignTimeRelativePathsNotSupported", new object[] { this.ID }));
                }
                dataFile = this.Context.Request.MapPath(dataFile, base.AppRelativeTemplateSourceDirectory, true);
            }
            HttpRuntime.CheckFilePermission(dataFile, true);
            if (!HttpRuntime.HasPathDiscoveryPermission(dataFile))
            {
                throw new HttpException(System.Web.SR.GetString("AccessDataSource_NoPathDiscoveryPermission", new object[] { HttpRuntime.GetSafePath(dataFile), this.ID }));
            }
            return dataFile;
        }

        internal override void SaveDataToCache(int startRowIndex, int maximumRows, object data, CacheDependency dependency)
        {
            this.AddCacheFileDependency();
            base.SaveDataToCache(startRowIndex, maximumRows, data, dependency);
        }

        internal override DataSourceCache Cache
        {
            get
            {
                if (this._cache == null)
                {
                    this._cache = new System.Web.UI.FileDataSourceCache();
                }
                return this._cache;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public override string ConnectionString
        {
            get
            {
                if (this._connectionString == null)
                {
                    this._connectionString = this.CreateConnectionString();
                }
                return this._connectionString;
            }
            set
            {
                throw new InvalidOperationException(System.Web.SR.GetString("AccessDataSource_CannotSetConnectionString"));
            }
        }

        [WebCategory("Data"), DefaultValue(""), WebSysDescription("AccessDataSource_DataFile"), Editor("System.Web.UI.Design.MdbDataFileEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty]
        public string DataFile
        {
            get
            {
                if (this._dataFile != null)
                {
                    return this._dataFile;
                }
                return string.Empty;
            }
            set
            {
                if (this.DataFile != value)
                {
                    this._dataFile = value;
                    this._connectionString = null;
                    this._physicalDataFile = null;
                    this.RaiseDataSourceChangedEvent(EventArgs.Empty);
                }
            }
        }

        private System.Web.UI.FileDataSourceCache FileDataSourceCache =>
            (this.Cache as System.Web.UI.FileDataSourceCache);

        private string PhysicalDataFile
        {
            get
            {
                if (this._physicalDataFile == null)
                {
                    this._physicalDataFile = this.GetPhysicalDataFilePath();
                }
                return this._physicalDataFile;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string ProviderName
        {
            get => 
                "System.Data.OleDb";
            set
            {
                throw new InvalidOperationException(System.Web.SR.GetString("AccessDataSource_CannotSetProvider", new object[] { this.ID }));
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public override string SqlCacheDependency
        {
            get
            {
                throw new NotSupportedException(System.Web.SR.GetString("AccessDataSource_SqlCacheDependencyNotSupported", new object[] { this.ID }));
            }
            set
            {
                throw new NotSupportedException(System.Web.SR.GetString("AccessDataSource_SqlCacheDependencyNotSupported", new object[] { this.ID }));
            }
        }
    }
}

