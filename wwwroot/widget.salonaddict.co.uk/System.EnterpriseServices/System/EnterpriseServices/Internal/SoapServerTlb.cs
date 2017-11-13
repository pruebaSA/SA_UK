namespace System.EnterpriseServices.Internal
{
    using System;
    using System.EnterpriseServices;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;

    [Guid("F6B6768F-F99E-4152-8ED2-0412F78517FB")]
    public sealed class SoapServerTlb : ISoapServerTlb
    {
        public void AddServerTlb(string progId, string classId, string interfaceId, string srcTlbPath, string rootWebServer, string inBaseUrl, string inVirtualRoot, string clientActivated, string wellKnown, string discoFile, string operation, out string strAssemblyName, out string typeName)
        {
            strAssemblyName = "";
            typeName = "";
            bool flag = false;
            bool inDefault = false;
            bool flag3 = false;
            bool flag4 = true;
            try
            {
                try
                {
                    new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                }
                catch (SecurityException)
                {
                    ComSoapPublishError.Report(Resource.FormatString("Soap_SecurityFailure"));
                    throw;
                }
                Platform.Assert(Platform.Whistler, "SoapServerTlb.AddServerTlb");
                if ((operation != null) && (operation.ToLower(CultureInfo.InvariantCulture) == "delete"))
                {
                    flag = true;
                }
                if (srcTlbPath.Length > 0)
                {
                    inDefault = SoapServerInfo.BoolFromString(discoFile, inDefault);
                    flag3 = SoapServerInfo.BoolFromString(wellKnown, flag3);
                    flag4 = SoapServerInfo.BoolFromString(clientActivated, flag4);
                    string str2 = SoapServerInfo.ServerPhysicalPath(rootWebServer, inBaseUrl, inVirtualRoot, !flag);
                    string str3 = srcTlbPath.ToLower(CultureInfo.InvariantCulture);
                    if (str3.EndsWith("mscoree.dll", StringComparison.Ordinal))
                    {
                        Type typeFromProgID = Type.GetTypeFromProgID(progId);
                        typeName = typeFromProgID.FullName;
                        strAssemblyName = typeFromProgID.Assembly.GetName().Name;
                    }
                    else if (str3.EndsWith("scrobj.dll", StringComparison.Ordinal))
                    {
                        if (!flag)
                        {
                            throw new ServicedComponentException(Resource.FormatString("ServicedComponentException_WSCNotSupported"));
                        }
                    }
                    else
                    {
                        string error = "";
                        GenerateMetadata metadata = new GenerateMetadata();
                        if (flag)
                        {
                            strAssemblyName = metadata.GetAssemblyName(srcTlbPath, str2 + @"\bin\");
                        }
                        else
                        {
                            strAssemblyName = metadata.GenerateSigned(srcTlbPath, str2 + @"\bin\", false, out error);
                        }
                        if (strAssemblyName.Length > 0)
                        {
                            try
                            {
                                typeName = this.GetTypeName(str2 + @"\bin\" + strAssemblyName + ".dll", progId, classId);
                            }
                            catch (DirectoryNotFoundException)
                            {
                                if (!flag)
                                {
                                    throw;
                                }
                            }
                            catch (FileNotFoundException)
                            {
                                if (!flag)
                                {
                                    throw;
                                }
                            }
                        }
                    }
                    if (((progId.Length > 0) && (strAssemblyName.Length > 0)) && (typeName.Length > 0))
                    {
                        DiscoFile file = new DiscoFile();
                        string assemblyFile = str2 + @"\bin\" + strAssemblyName + ".dll";
                        if (flag)
                        {
                            SoapServerConfig.DeleteComponent(str2 + @"\Web.Config", strAssemblyName, typeName, progId, assemblyFile);
                            file.DeleteElement(str2 + @"\Default.disco", progId + ".soap?WSDL");
                        }
                        else
                        {
                            SoapServerConfig.AddComponent(str2 + @"\Web.Config", strAssemblyName, typeName, progId, assemblyFile, "SingleCall", flag3, flag4);
                            if (inDefault)
                            {
                                file.AddElement(str2 + @"\Default.disco", progId + ".soap?WSDL");
                            }
                        }
                    }
                }
            }
            catch (ServicedComponentException exception)
            {
                this.ThrowHelper("Soap_PublishServerTlbFailure", exception);
            }
            catch (RegistrationException exception2)
            {
                this.ThrowHelper("Soap_PublishServerTlbFailure", exception2);
            }
            catch
            {
                this.ThrowHelper("Soap_PublishServerTlbFailure", null);
            }
        }

        public void DeleteServerTlb(string progId, string classId, string interfaceId, string srcTlbPath, string rootWebServer, string baseUrl, string virtualRoot, string operation, string assemblyName, string typeName)
        {
            try
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
            }
            catch (SecurityException)
            {
                ComSoapPublishError.Report(Resource.FormatString("Soap_SecurityFailure"));
                throw;
            }
            Platform.Assert(Platform.Whistler, "SoapServerTlb.DeleteServerTlb");
            string name = assemblyName;
            if ((((progId.Length > 0) || (classId.Length > 0)) || ((assemblyName.Length > 0) || (typeName.Length > 0))) && ((baseUrl.Length > 0) || (virtualRoot.Length > 0)))
            {
                string str3 = SoapServerInfo.ServerPhysicalPath(rootWebServer, baseUrl, virtualRoot, false);
                string str4 = srcTlbPath.ToLower(CultureInfo.InvariantCulture);
                if (!str4.EndsWith("scrobj.dll", StringComparison.Ordinal))
                {
                    if (str4.EndsWith("mscoree.dll", StringComparison.Ordinal))
                    {
                        Type typeFromProgID = Type.GetTypeFromProgID(progId);
                        typeName = typeFromProgID.FullName;
                        name = typeFromProgID.Assembly.GetName().Name;
                    }
                    else
                    {
                        name = new GenerateMetadata().GetAssemblyName(srcTlbPath, str3 + @"\bin\");
                        if (name.Length > 0)
                        {
                            try
                            {
                                typeName = this.GetTypeName(str3 + @"\bin\" + name + ".dll", progId, classId);
                            }
                            catch (DirectoryNotFoundException)
                            {
                            }
                            catch (FileNotFoundException)
                            {
                            }
                        }
                    }
                    if (((progId.Length > 0) && (name.Length > 0)) && (typeName.Length > 0))
                    {
                        DiscoFile file = new DiscoFile();
                        string assemblyFile = str3 + @"\bin\" + name + ".dll";
                        SoapServerConfig.DeleteComponent(str3 + @"\Web.Config", name, typeName, progId, assemblyFile);
                        file.DeleteElement(str3 + @"\Default.disco", progId + ".soap?WSDL");
                    }
                }
            }
        }

        internal string GetTypeName(string assemblyPath, string progId, string classId)
        {
            string typeNameFromProgId = "";
            AssemblyManager manager = null;
            AppDomain domain = AppDomain.CreateDomain("SoapDomain");
            if (domain != null)
            {
                try
                {
                    AssemblyName id = typeof(AssemblyManager).Assembly.GetName();
                    Evidence securityAttributes = new Evidence(AppDomain.CurrentDomain.Evidence);
                    securityAttributes.AddAssembly(id);
                    ObjectHandle handle = domain.CreateInstance(id.FullName, typeof(AssemblyManager).FullName, false, BindingFlags.Default, null, null, null, null, securityAttributes);
                    if (handle == null)
                    {
                        return typeNameFromProgId;
                    }
                    manager = (AssemblyManager) handle.Unwrap();
                    if (classId.Length > 0)
                    {
                        return manager.InternalGetTypeNameFromClassId(assemblyPath, classId);
                    }
                    typeNameFromProgId = manager.InternalGetTypeNameFromProgId(assemblyPath, progId);
                }
                finally
                {
                    AppDomain.Unload(domain);
                }
            }
            return typeNameFromProgId;
        }

        private void ThrowHelper(string messageId, Exception e)
        {
            ComSoapPublishError.Report(Resource.FormatString(messageId));
            if (e != null)
            {
                throw e;
            }
        }
    }
}

