namespace System.Net.Configuration
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Net;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Threading;

    internal sealed class DefaultProxySectionInternal
    {
        private static object classSyncObject;
        private IWebProxy webProxy;

        [SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.ControlPrincipal)]
        internal DefaultProxySectionInternal(DefaultProxySection section)
        {
            System.Net.WebProxy proxy;
            if (!section.Enabled)
            {
                return;
            }
            if ((((section.Proxy.AutoDetect == ProxyElement.AutoDetectValues.Unspecified) && (section.Proxy.ScriptLocation == null)) && (string.IsNullOrEmpty(section.Module.Type) && (section.Proxy.UseSystemDefault != ProxyElement.UseSystemDefaultValues.True))) && (((section.Proxy.ProxyAddress == null) && (section.Proxy.BypassOnLocal == ProxyElement.BypassOnLocalValues.Unspecified)) && (section.BypassList.Count == 0)))
            {
                if (section.Proxy.UseSystemDefault == ProxyElement.UseSystemDefaultValues.False)
                {
                    this.webProxy = new EmptyWebProxy();
                    return;
                }
                try
                {
                    using (WindowsIdentity.Impersonate(IntPtr.Zero))
                    {
                        this.webProxy = new WebRequest.WebProxyWrapper(new System.Net.WebProxy(true));
                    }
                    goto Label_02E3;
                }
                catch
                {
                    throw;
                }
            }
            if (!string.IsNullOrEmpty(section.Module.Type))
            {
                Type c = Type.GetType(section.Module.Type, true, true);
                if ((c.Attributes & TypeAttributes.NestedFamORAssem) != TypeAttributes.Public)
                {
                    throw new ConfigurationErrorsException(System.SR.GetString("net_config_proxy_module_not_public"));
                }
                if (!typeof(IWebProxy).IsAssignableFrom(c))
                {
                    throw new InvalidCastException(System.SR.GetString("net_invalid_cast", new object[] { c.FullName, "IWebProxy" }));
                }
                this.webProxy = (IWebProxy) Activator.CreateInstance(c, BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[0], CultureInfo.InvariantCulture);
            }
            else
            {
                if (((section.Proxy.UseSystemDefault == ProxyElement.UseSystemDefaultValues.True) && (section.Proxy.AutoDetect == ProxyElement.AutoDetectValues.Unspecified)) && (section.Proxy.ScriptLocation == null))
                {
                    try
                    {
                        using (WindowsIdentity.Impersonate(IntPtr.Zero))
                        {
                            this.webProxy = new System.Net.WebProxy(false);
                        }
                        goto Label_01DE;
                    }
                    catch
                    {
                        throw;
                    }
                }
                this.webProxy = new System.Net.WebProxy();
            }
        Label_01DE:
            proxy = this.webProxy as System.Net.WebProxy;
            if (proxy != null)
            {
                if (section.Proxy.AutoDetect != ProxyElement.AutoDetectValues.Unspecified)
                {
                    proxy.AutoDetect = section.Proxy.AutoDetect == ProxyElement.AutoDetectValues.True;
                }
                if (section.Proxy.ScriptLocation != null)
                {
                    proxy.ScriptLocation = section.Proxy.ScriptLocation;
                }
                if (section.Proxy.BypassOnLocal != ProxyElement.BypassOnLocalValues.Unspecified)
                {
                    proxy.BypassProxyOnLocal = section.Proxy.BypassOnLocal == ProxyElement.BypassOnLocalValues.True;
                }
                if (section.Proxy.ProxyAddress != null)
                {
                    proxy.Address = section.Proxy.ProxyAddress;
                }
                int count = section.BypassList.Count;
                if (count > 0)
                {
                    string[] strArray = new string[section.BypassList.Count];
                    for (int i = 0; i < count; i++)
                    {
                        strArray[i] = section.BypassList[i].Address;
                    }
                    proxy.BypassList = strArray;
                }
                if (section.Module.Type == null)
                {
                    this.webProxy = new WebRequest.WebProxyWrapper(proxy);
                }
            }
        Label_02E3:
            if ((this.webProxy != null) && section.UseDefaultCredentials)
            {
                this.webProxy.Credentials = SystemNetworkCredential.defaultCredential;
            }
        }

        internal static DefaultProxySectionInternal GetSection()
        {
            DefaultProxySectionInternal internal2;
            lock (ClassSyncObject)
            {
                DefaultProxySection section = System.Configuration.PrivilegedConfigurationManager.GetSection(ConfigurationStrings.DefaultProxySectionPath) as DefaultProxySection;
                if (section == null)
                {
                    internal2 = null;
                }
                else
                {
                    try
                    {
                        internal2 = new DefaultProxySectionInternal(section);
                    }
                    catch (Exception exception)
                    {
                        if (NclUtilities.IsFatal(exception))
                        {
                            throw;
                        }
                        throw new ConfigurationErrorsException(System.SR.GetString("net_config_proxy"), exception);
                    }
                    catch
                    {
                        throw new ConfigurationErrorsException(System.SR.GetString("net_config_proxy"), new Exception(System.SR.GetString("net_nonClsCompliantException")));
                    }
                }
            }
            return internal2;
        }

        internal static object ClassSyncObject
        {
            get
            {
                if (classSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref classSyncObject, obj2, null);
                }
                return classSyncObject;
            }
        }

        internal IWebProxy WebProxy =>
            this.webProxy;
    }
}

