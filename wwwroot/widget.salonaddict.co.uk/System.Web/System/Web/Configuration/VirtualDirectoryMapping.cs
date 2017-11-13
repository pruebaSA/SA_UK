namespace System.Web.Configuration
{
    using System;
    using System.IO;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class VirtualDirectoryMapping
    {
        private string _configFileBaseName;
        private bool _isAppRoot;
        private string _physicalDirectory;
        private VirtualPath _virtualDirectory;
        private const string DEFAULT_BASE_NAME = "web.config";

        public VirtualDirectoryMapping(string physicalDirectory, bool isAppRoot) : this(null, physicalDirectory, isAppRoot, "web.config")
        {
        }

        public VirtualDirectoryMapping(string physicalDirectory, bool isAppRoot, string configFileBaseName) : this(null, physicalDirectory, isAppRoot, configFileBaseName)
        {
        }

        private VirtualDirectoryMapping(VirtualPath virtualDirectory, string physicalDirectory, bool isAppRoot, string configFileBaseName)
        {
            this._virtualDirectory = virtualDirectory;
            this._isAppRoot = isAppRoot;
            this.PhysicalDirectory = physicalDirectory;
            this.ConfigFileBaseName = configFileBaseName;
        }

        internal VirtualDirectoryMapping Clone() => 
            new VirtualDirectoryMapping(this._virtualDirectory, this._physicalDirectory, this._isAppRoot, this._configFileBaseName);

        internal void SetVirtualDirectory(VirtualPath virtualDirectory)
        {
            this._virtualDirectory = virtualDirectory;
        }

        internal void Validate()
        {
            if (this._physicalDirectory != null)
            {
                string path = Path.Combine(this._physicalDirectory, this._configFileBaseName);
                string fullPath = Path.GetFullPath(path);
                if (((Path.GetDirectoryName(fullPath) != this._physicalDirectory) || (Path.GetFileName(fullPath) != this._configFileBaseName)) || FileUtil.IsSuspiciousPhysicalPath(path))
                {
                    throw ExceptionUtil.ParameterInvalid("configFileBaseName");
                }
            }
        }

        public string ConfigFileBaseName
        {
            get => 
                this._configFileBaseName;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw ExceptionUtil.PropertyInvalid("ConfigFileBaseName");
                }
                this._configFileBaseName = value;
            }
        }

        public bool IsAppRoot
        {
            get => 
                this._isAppRoot;
            set
            {
                this._isAppRoot = value;
            }
        }

        public string PhysicalDirectory
        {
            get => 
                this._physicalDirectory;
            set
            {
                string str = value;
                if (string.IsNullOrEmpty(str))
                {
                    str = null;
                }
                else
                {
                    if (UrlPath.PathEndsWithExtraSlash(str))
                    {
                        str = str.Substring(0, str.Length - 1);
                    }
                    if (FileUtil.IsSuspiciousPhysicalPath(str))
                    {
                        throw ExceptionUtil.ParameterInvalid("PhysicalDirectory");
                    }
                }
                this._physicalDirectory = str;
            }
        }

        public string VirtualDirectory =>
            this._virtualDirectory?.VirtualPathString;

        internal VirtualPath VirtualDirectoryObject =>
            this._virtualDirectory;
    }
}

