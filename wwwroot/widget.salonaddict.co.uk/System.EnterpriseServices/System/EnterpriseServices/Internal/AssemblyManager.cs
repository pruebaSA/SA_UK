namespace System.EnterpriseServices.Internal
{
    using Microsoft.Win32;
    using System;
    using System.EnterpriseServices;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;

    internal class AssemblyManager : MarshalByRefObject
    {
        internal bool CompareToCache(string AssemblyPath, string srcTypeLib)
        {
            bool flag = true;
            try
            {
                string cacheName = CacheInfo.GetCacheName(AssemblyPath, srcTypeLib);
                if (!File.Exists(AssemblyPath))
                {
                    return false;
                }
                if (!File.Exists(cacheName))
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                flag = false;
                ComSoapPublishError.Report(exception.ToString());
            }
            catch
            {
                flag = false;
                ComSoapPublishError.Report(Resource.FormatString("Err_NonClsException", "AssemblyManager.CompareToCache"));
            }
            return flag;
        }

        [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        internal static extern bool CopyFile(string source, string dest, bool failifexists);
        internal bool CopyToCache(string AssemblyPath, string srcTypeLib)
        {
            try
            {
                string cacheName = CacheInfo.GetCacheName(AssemblyPath, srcTypeLib);
                return (File.Exists(cacheName) || CopyFile(AssemblyPath, cacheName, false));
            }
            catch (Exception exception)
            {
                ComSoapPublishError.Report(exception.ToString());
            }
            catch
            {
                ComSoapPublishError.Report(Resource.FormatString("Err_NonClsException", "AssemblyManager.CopyToCache"));
            }
            return false;
        }

        internal bool GetFromCache(string AssemblyPath, string srcTypeLib)
        {
            try
            {
                string cacheName = CacheInfo.GetCacheName(AssemblyPath, srcTypeLib);
                return (File.Exists(cacheName) && CopyFile(cacheName, AssemblyPath, true));
            }
            catch (Exception exception)
            {
                ComSoapPublishError.Report(exception.ToString());
            }
            catch
            {
                ComSoapPublishError.Report(Resource.FormatString("Err_NonClsException", "AssemblyManager.GetFromCache"));
            }
            return false;
        }

        public string GetFullName(string fName, string strAssemblyName)
        {
            string fullName = "";
            AppDomainSetup info = new AppDomainSetup();
            AppDomain domain = AppDomain.CreateDomain("SoapDomain", null, info);
            if (domain != null)
            {
                try
                {
                    ObjectHandle handle = domain.CreateInstance(typeof(AssemblyManager).Assembly.FullName, typeof(AssemblyManager).FullName);
                    if (handle != null)
                    {
                        fullName = ((AssemblyManager) handle.Unwrap()).InternalGetFullName(fName, strAssemblyName);
                    }
                }
                finally
                {
                    AppDomain.Unload(domain);
                }
            }
            return fullName;
        }

        public string GetGacName(string fName)
        {
            string gacName = "";
            AppDomainSetup info = new AppDomainSetup();
            AppDomain domain = AppDomain.CreateDomain("SoapDomain", null, info);
            if (domain != null)
            {
                try
                {
                    ObjectHandle handle = domain.CreateInstance(typeof(AssemblyManager).Assembly.FullName, typeof(AssemblyManager).FullName);
                    if (handle != null)
                    {
                        gacName = ((AssemblyManager) handle.Unwrap()).InternalGetGacName(fName);
                    }
                }
                finally
                {
                    AppDomain.Unload(domain);
                }
            }
            return gacName;
        }

        internal string InternalGetFullName(string fName, string strAssemblyName)
        {
            string fullName = "";
            try
            {
                if (File.Exists(fName))
                {
                    return AssemblyName.GetAssemblyName(fName).FullName;
                }
                try
                {
                    fullName = Assembly.LoadWithPartialName(strAssemblyName, null).FullName;
                }
                catch
                {
                    throw new RegistrationException(Resource.FormatString("ServicedComponentException_AssemblyNotInGAC"));
                }
            }
            catch (RegistrationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                ComSoapPublishError.Report(exception.ToString());
            }
            catch
            {
                ComSoapPublishError.Report(Resource.FormatString("Err_NonClsException", "AssemblyManager.InternalGetFullName"));
            }
            return fullName;
        }

        internal string InternalGetGacName(string fName)
        {
            string str = "";
            try
            {
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(fName);
                str = assemblyName.Name + ",Version=" + assemblyName.Version.ToString();
            }
            catch (Exception exception)
            {
                ComSoapPublishError.Report(exception.ToString());
            }
            catch
            {
                ComSoapPublishError.Report(Resource.FormatString("Err_NonClsException", "AssemblyManager.InternalGetGacName"));
            }
            return str;
        }

        internal string InternalGetTypeNameFromClassId(string assemblyPath, string classId)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            Guid guid = new Guid(classId);
            foreach (Type type in assembly.GetTypes())
            {
                if (guid.Equals(type.GUID))
                {
                    return type.FullName;
                }
            }
            return "";
        }

        internal string InternalGetTypeNameFromProgId(string AssemblyPath, string ProgId)
        {
            string str = "";
            Assembly assembly = Assembly.LoadFrom(AssemblyPath);
            try
            {
                string g = (string) Registry.ClassesRoot.OpenSubKey(ProgId + @"\CLSID").GetValue("");
                Guid guid = new Guid(g);
                foreach (Type type in assembly.GetTypes())
                {
                    if (guid.Equals(type.GUID))
                    {
                        return type.FullName;
                    }
                }
            }
            catch
            {
                str = string.Empty;
                throw;
            }
            return str;
        }
    }
}

