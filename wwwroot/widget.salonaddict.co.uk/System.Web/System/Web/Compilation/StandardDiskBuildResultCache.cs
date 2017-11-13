namespace System.Web.Compilation
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.UI;
    using System.Web.Util;

    internal class StandardDiskBuildResultCache : DiskBuildResultCache
    {
        private static ArrayList _satelliteDirectories;
        private const string fusionCacheDirectoryName = "assembly";
        private const string webHashDirectoryName = "hash";

        internal StandardDiskBuildResultCache(string cacheDir) : base(cacheDir)
        {
            base.EnsureDiskCacheDirectoryCreated();
            this.FindSatelliteDirectories();
        }

        internal void DeleteFilesInDirectory(string path)
        {
            foreach (FileData data in (IEnumerable) FileEnumerator.Create(path))
            {
                if (data.IsDirectory)
                {
                    Directory.Delete(data.FullName, true);
                }
                else
                {
                    Util.RemoveOrRenameFile(data.FullName);
                }
            }
        }

        private void FindSatelliteDirectories()
        {
            foreach (string str in Directory.GetDirectories(base._cacheDir))
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(str);
                if (((fileNameWithoutExtension != "assembly") && (fileNameWithoutExtension != "hash")) && Util.IsCultureName(fileNameWithoutExtension))
                {
                    if (_satelliteDirectories == null)
                    {
                        _satelliteDirectories = new ArrayList();
                    }
                    _satelliteDirectories.Add(Path.Combine(base._cacheDir, str));
                }
            }
        }

        internal long GetPreservedSpecialFilesCombinedHash()
        {
            string specialFilesCombinedHashFileName = this.GetSpecialFilesCombinedHashFileName();
            if (!FileUtil.FileExists(specialFilesCombinedHashFileName))
            {
                return 0L;
            }
            try
            {
                return long.Parse(Util.StringFromFile(specialFilesCombinedHashFileName), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0L;
            }
        }

        private string GetSpecialFilesCombinedHashFileName() => 
            BuildManager.WebHashFilePath;

        internal void RemoveAllCodegenFiles()
        {
            foreach (FileData data in (IEnumerable) FileEnumerator.Create(base._cacheDir))
            {
                if (data.IsDirectory)
                {
                    if (((data.Name != "assembly") && (data.Name != "hash")) && !StringUtil.StringStartsWith(data.Name, "Sources_"))
                    {
                        try
                        {
                            this.DeleteFilesInDirectory(data.FullName);
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    DiskBuildResultCache.TryDeleteFile(data.FullName);
                }
            }
            AppDomainSetup setupInformation = Thread.GetDomain().SetupInformation;
            UnsafeNativeMethods.DeleteShadowCache(setupInformation.CachePath, setupInformation.ApplicationName);
        }

        internal void RemoveOldTempFiles()
        {
            string path = base._cacheDir + @"\";
            foreach (FileData data in (IEnumerable) FileEnumerator.Create(path))
            {
                if (!data.IsDirectory)
                {
                    string extension = Path.GetExtension(data.Name);
                    switch (extension)
                    {
                        case ".dll":
                        case ".pdb":
                        case ".web":
                        case ".ccu":
                        case ".compiled":
                        {
                            continue;
                        }
                    }
                    if (extension != ".delete")
                    {
                        int length = data.Name.LastIndexOf('.');
                        if (length <= 0)
                        {
                            goto Label_0123;
                        }
                        string str3 = data.Name.Substring(0, length);
                        int num2 = str3.LastIndexOf('.');
                        if (num2 > 0)
                        {
                            str3 = str3.Substring(0, num2);
                        }
                        if (!FileUtil.FileExists(path + str3 + ".dll") && !FileUtil.FileExists(path + "App_Web_" + str3 + ".dll"))
                        {
                            goto Label_0123;
                        }
                    }
                    else
                    {
                        DiskBuildResultCache.CheckAndRemoveDotDeleteFile(new FileInfo(data.FullName));
                    }
                }
                continue;
            Label_0123:
                Util.DeleteFileNoException(data.FullName);
            }
        }

        internal static void RemoveSatelliteAssemblies(string baseAssemblyName)
        {
            if (_satelliteDirectories != null)
            {
                string str = baseAssemblyName + ".resources";
                foreach (string str2 in _satelliteDirectories)
                {
                    string str3 = Path.Combine(str2, str);
                    Util.DeleteFileIfExistsNoException(str3 + ".dll");
                    Util.DeleteFileIfExistsNoException(str3 + ".pdb");
                }
            }
        }

        internal void SavePreservedSpecialFilesCombinedHash(long hash)
        {
            StreamWriter writer = null;
            try
            {
                string specialFilesCombinedHashFileName = this.GetSpecialFilesCombinedHashFileName();
                string directoryName = Path.GetDirectoryName(specialFilesCombinedHashFileName);
                if (!FileUtil.DirectoryExists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                writer = new StreamWriter(specialFilesCombinedHashFileName, false, Encoding.UTF8);
                writer.Write(hash.ToString("x", CultureInfo.InvariantCulture));
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }
    }
}

