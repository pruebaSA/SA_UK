namespace System.IO.IsolatedStorage
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Permissions;
    using System.Security.Policy;
    using System.Text;
    using System.Threading;

    [ComVisible(true)]
    public abstract class IsolatedStorage : MarshalByRefObject
    {
        internal const IsolatedStorageScope c_AppMachine = (IsolatedStorageScope.Application | IsolatedStorageScope.Machine);
        internal const IsolatedStorageScope c_AppUser = (IsolatedStorageScope.Application | IsolatedStorageScope.User);
        internal const IsolatedStorageScope c_AppUserRoaming = (IsolatedStorageScope.Application | IsolatedStorageScope.Roaming | IsolatedStorageScope.User);
        internal const IsolatedStorageScope c_Assembly = (IsolatedStorageScope.Assembly | IsolatedStorageScope.User);
        internal const IsolatedStorageScope c_AssemblyRoaming = (IsolatedStorageScope.Roaming | IsolatedStorageScope.Assembly | IsolatedStorageScope.User);
        internal const IsolatedStorageScope c_Domain = (IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain | IsolatedStorageScope.User);
        internal const IsolatedStorageScope c_DomainRoaming = (IsolatedStorageScope.Roaming | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain | IsolatedStorageScope.User);
        internal const IsolatedStorageScope c_MachineAssembly = (IsolatedStorageScope.Machine | IsolatedStorageScope.Assembly);
        internal const IsolatedStorageScope c_MachineDomain = (IsolatedStorageScope.Machine | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain);
        private object m_AppIdentity;
        private string m_AppName;
        private object m_AssemIdentity;
        private string m_AssemName;
        private object m_DomainIdentity;
        private string m_DomainName;
        private ulong m_Quota;
        private IsolatedStorageScope m_Scope;
        private bool m_ValidQuota;
        private static char[] s_Base32Char = new char[] { 
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
            'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5'
        };
        private static IsolatedStorageFilePermission s_PermAppMachine;
        private static IsolatedStorageFilePermission s_PermAppUser;
        private static IsolatedStorageFilePermission s_PermAppUserRoaming;
        private static IsolatedStorageFilePermission s_PermAssem;
        private static IsolatedStorageFilePermission s_PermAssemRoaming;
        private static SecurityPermission s_PermControlEvidence;
        private static IsolatedStorageFilePermission s_PermDomain;
        private static IsolatedStorageFilePermission s_PermDomainRoaming;
        private static PermissionSet s_PermExecution;
        private static IsolatedStorageFilePermission s_PermMachineAssem;
        private static IsolatedStorageFilePermission s_PermMachineDomain;
        private static PermissionSet s_PermReflection;
        private static PermissionSet s_PermUnrestricted;
        private const string s_Publisher = "Publisher";
        private const string s_Site = "Site";
        private const string s_StrongName = "StrongName";
        private const string s_Url = "Url";
        private const string s_Zone = "Zone";

        protected IsolatedStorage()
        {
        }

        private static object _GetAccountingInfo(Evidence evidence, Type evidenceType, IsolatedStorageScope fAssmDomApp, out object oNormalized)
        {
            object obj2 = null;
            IEnumerator hostEnumerator = evidence.GetHostEnumerator();
            if (evidenceType != null)
            {
                while (hostEnumerator.MoveNext())
                {
                    object current = hostEnumerator.Current;
                    if (current.GetType().Equals(evidenceType))
                    {
                        obj2 = current;
                        break;
                    }
                }
            }
            else
            {
                Publisher publisher = null;
                StrongName name = null;
                Url url = null;
                Site site = null;
                Zone zone = null;
                while (hostEnumerator.MoveNext())
                {
                    obj2 = hostEnumerator.Current;
                    if (obj2 is Publisher)
                    {
                        publisher = (Publisher) obj2;
                        break;
                    }
                    if (obj2 is StrongName)
                    {
                        name = (StrongName) obj2;
                    }
                    else
                    {
                        if (obj2 is Url)
                        {
                            url = (Url) obj2;
                            continue;
                        }
                        if (obj2 is Site)
                        {
                            site = (Site) obj2;
                            continue;
                        }
                        if (obj2 is Zone)
                        {
                            zone = (Zone) obj2;
                        }
                    }
                }
                if (publisher == null)
                {
                    if (name == null)
                    {
                        if (url == null)
                        {
                            if (site == null)
                            {
                                if (zone == null)
                                {
                                    if (fAssmDomApp == IsolatedStorageScope.Domain)
                                    {
                                        throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DomainNoEvidence"));
                                    }
                                    if (fAssmDomApp == IsolatedStorageScope.Application)
                                    {
                                        throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_ApplicationNoEvidence"));
                                    }
                                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_AssemblyNoEvidence"));
                                }
                                obj2 = zone;
                            }
                            else
                            {
                                obj2 = site;
                            }
                        }
                        else
                        {
                            obj2 = url;
                        }
                    }
                    else
                    {
                        obj2 = name;
                    }
                }
                else
                {
                    obj2 = publisher;
                }
                goto Label_0154;
            }
            if (obj2 == null)
            {
                if (fAssmDomApp == IsolatedStorageScope.Domain)
                {
                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DomainNoEvidence"));
                }
                if (fAssmDomApp == IsolatedStorageScope.Application)
                {
                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_ApplicationNoEvidence"));
                }
                throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_AssemblyNoEvidence"));
            }
        Label_0154:
            if (obj2 is INormalizeForIsolatedStorage)
            {
                oNormalized = ((INormalizeForIsolatedStorage) obj2).Normalize();
                return obj2;
            }
            if (obj2 is Publisher)
            {
                oNormalized = ((Publisher) obj2).Normalize();
                return obj2;
            }
            if (obj2 is StrongName)
            {
                oNormalized = ((StrongName) obj2).Normalize();
                return obj2;
            }
            if (obj2 is Url)
            {
                oNormalized = ((Url) obj2).Normalize();
                return obj2;
            }
            if (obj2 is Site)
            {
                oNormalized = ((Site) obj2).Normalize();
                return obj2;
            }
            if (obj2 is Zone)
            {
                oNormalized = ((Zone) obj2).Normalize();
                return obj2;
            }
            oNormalized = null;
            return obj2;
        }

        private void _InitStore(IsolatedStorageScope scope, Evidence domainEv, Type domainEvidenceType, Evidence assemEv, Type assemblyEvidenceType, Evidence appEv, Type appEvidenceType)
        {
            VerifyScope(scope);
            if (IsApp(scope))
            {
                if (appEv == null)
                {
                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_ApplicationMissingIdentity"));
                }
            }
            else
            {
                if (assemEv == null)
                {
                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_AssemblyMissingIdentity"));
                }
                if (IsDomain(scope) && (domainEv == null))
                {
                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DomainMissingIdentity"));
                }
            }
            DemandPermission(scope);
            string typeName = null;
            string instanceName = null;
            if (IsApp(scope))
            {
                this.m_AppIdentity = GetAccountingInfo(appEv, appEvidenceType, IsolatedStorageScope.Application, out typeName, out instanceName);
                this.m_AppName = this.GetNameFromID(typeName, instanceName);
            }
            else
            {
                this.m_AssemIdentity = GetAccountingInfo(assemEv, assemblyEvidenceType, IsolatedStorageScope.Assembly, out typeName, out instanceName);
                this.m_AssemName = this.GetNameFromID(typeName, instanceName);
                if (IsDomain(scope))
                {
                    this.m_DomainIdentity = GetAccountingInfo(domainEv, domainEvidenceType, IsolatedStorageScope.Domain, out typeName, out instanceName);
                    this.m_DomainName = this.GetNameFromID(typeName, instanceName);
                }
            }
            this.m_Scope = scope;
        }

        private static void DemandPermission(IsolatedStorageScope scope)
        {
            IsolatedStorageFilePermission permission = null;
            switch (scope)
            {
                case (IsolatedStorageScope.Assembly | IsolatedStorageScope.User):
                    if (s_PermAssem == null)
                    {
                        s_PermAssem = new IsolatedStorageFilePermission(IsolatedStorageContainment.AssemblyIsolationByUser, 0L, false);
                    }
                    permission = s_PermAssem;
                    break;

                case (IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain | IsolatedStorageScope.User):
                    if (s_PermDomain == null)
                    {
                        s_PermDomain = new IsolatedStorageFilePermission(IsolatedStorageContainment.DomainIsolationByUser, 0L, false);
                    }
                    permission = s_PermDomain;
                    break;

                case (IsolatedStorageScope.Roaming | IsolatedStorageScope.Assembly | IsolatedStorageScope.User):
                    if (s_PermAssemRoaming == null)
                    {
                        s_PermAssemRoaming = new IsolatedStorageFilePermission(IsolatedStorageContainment.AssemblyIsolationByRoamingUser, 0L, false);
                    }
                    permission = s_PermAssemRoaming;
                    break;

                case (IsolatedStorageScope.Roaming | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain | IsolatedStorageScope.User):
                    if (s_PermDomainRoaming == null)
                    {
                        s_PermDomainRoaming = new IsolatedStorageFilePermission(IsolatedStorageContainment.DomainIsolationByRoamingUser, 0L, false);
                    }
                    permission = s_PermDomainRoaming;
                    break;

                case (IsolatedStorageScope.Machine | IsolatedStorageScope.Assembly):
                    if (s_PermMachineAssem == null)
                    {
                        s_PermMachineAssem = new IsolatedStorageFilePermission(IsolatedStorageContainment.AssemblyIsolationByMachine, 0L, false);
                    }
                    permission = s_PermMachineAssem;
                    break;

                case (IsolatedStorageScope.Machine | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain):
                    if (s_PermMachineDomain == null)
                    {
                        s_PermMachineDomain = new IsolatedStorageFilePermission(IsolatedStorageContainment.DomainIsolationByMachine, 0L, false);
                    }
                    permission = s_PermMachineDomain;
                    break;

                case (IsolatedStorageScope.Application | IsolatedStorageScope.User):
                    if (s_PermAppUser == null)
                    {
                        s_PermAppUser = new IsolatedStorageFilePermission(IsolatedStorageContainment.ApplicationIsolationByUser, 0L, false);
                    }
                    permission = s_PermAppUser;
                    break;

                case (IsolatedStorageScope.Application | IsolatedStorageScope.Roaming | IsolatedStorageScope.User):
                    if (s_PermAppUserRoaming == null)
                    {
                        s_PermAppUserRoaming = new IsolatedStorageFilePermission(IsolatedStorageContainment.ApplicationIsolationByRoamingUser, 0L, false);
                    }
                    permission = s_PermAppUserRoaming;
                    break;

                case (IsolatedStorageScope.Application | IsolatedStorageScope.Machine):
                    if (s_PermAppMachine == null)
                    {
                        s_PermAppMachine = new IsolatedStorageFilePermission(IsolatedStorageContainment.ApplicationIsolationByMachine, 0L, false);
                    }
                    permission = s_PermAppMachine;
                    break;
            }
            permission.Demand();
        }

        private static object GetAccountingInfo(Evidence evidence, Type evidenceType, IsolatedStorageScope fAssmDomApp, out string typeName, out string instanceName)
        {
            object oNormalized = null;
            MemoryStream stream;
            object o = _GetAccountingInfo(evidence, evidenceType, fAssmDomApp, out oNormalized);
            typeName = GetPredefinedTypeName(o);
            if (typeName == null)
            {
                GetReflectionPermission().Assert();
                stream = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, o.GetType());
                stream.Position = 0L;
                typeName = GetHash(stream);
            }
            instanceName = null;
            if (oNormalized != null)
            {
                if (oNormalized is Stream)
                {
                    instanceName = GetHash((Stream) oNormalized);
                }
                else if (oNormalized is string)
                {
                    if (IsValidName((string) oNormalized))
                    {
                        instanceName = (string) oNormalized;
                    }
                    else
                    {
                        stream = new MemoryStream();
                        new BinaryWriter(stream).Write((string) oNormalized);
                        stream.Position = 0L;
                        instanceName = GetHash(stream);
                    }
                }
            }
            else
            {
                oNormalized = o;
            }
            if (instanceName == null)
            {
                GetReflectionPermission().Assert();
                stream = new MemoryStream();
                new BinaryFormatter().Serialize(stream, oNormalized);
                stream.Position = 0L;
                instanceName = GetHash(stream);
            }
            return o;
        }

        private static SecurityPermission GetControlEvidencePermission()
        {
            if (s_PermControlEvidence == null)
            {
                s_PermControlEvidence = new SecurityPermission(SecurityPermissionFlag.ControlEvidence);
            }
            return s_PermControlEvidence;
        }

        private static PermissionSet GetExecutionPermission()
        {
            if (s_PermExecution == null)
            {
                s_PermExecution = new PermissionSet(PermissionState.None);
                s_PermExecution.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            }
            return s_PermExecution;
        }

        internal static string GetHash(Stream s)
        {
            using (SHA1 sha = new SHA1CryptoServiceProvider())
            {
                return ToBase32StringSuitableForDirName(sha.ComputeHash(s));
            }
        }

        internal MemoryStream GetIdentityStream(IsolatedStorageScope scope)
        {
            object appIdentity;
            GetReflectionPermission().Assert();
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream serializationStream = new MemoryStream();
            if (IsApp(scope))
            {
                appIdentity = this.m_AppIdentity;
            }
            else if (IsDomain(scope))
            {
                appIdentity = this.m_DomainIdentity;
            }
            else
            {
                appIdentity = this.m_AssemIdentity;
            }
            if (appIdentity != null)
            {
                formatter.Serialize(serializationStream, appIdentity);
            }
            serializationStream.Position = 0L;
            return serializationStream;
        }

        private string GetNameFromID(string typeID, string instanceID)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(typeID);
            builder.Append(this.SeparatorInternal);
            builder.Append(instanceID);
            return builder.ToString();
        }

        protected abstract IsolatedStoragePermission GetPermission(PermissionSet ps);
        private static string GetPredefinedTypeName(object o)
        {
            if (o is Publisher)
            {
                return "Publisher";
            }
            if (o is StrongName)
            {
                return "StrongName";
            }
            if (o is Url)
            {
                return "Url";
            }
            if (o is Site)
            {
                return "Site";
            }
            if (o is Zone)
            {
                return "Zone";
            }
            return null;
        }

        private static PermissionSet GetReflectionPermission()
        {
            if (s_PermReflection == null)
            {
                s_PermReflection = new PermissionSet(PermissionState.Unrestricted);
            }
            return s_PermReflection;
        }

        private static PermissionSet GetUnrestricted()
        {
            if (s_PermUnrestricted == null)
            {
                s_PermUnrestricted = new PermissionSet(PermissionState.Unrestricted);
            }
            return s_PermUnrestricted;
        }

        protected void InitStore(IsolatedStorageScope scope, Type appEvidenceType)
        {
            PermissionSet granted = null;
            PermissionSet denied = null;
            nGetCaller();
            GetControlEvidencePermission().Assert();
            if (IsApp(scope))
            {
                AppDomain domain = Thread.GetDomain();
                if (!IsRoaming(scope))
                {
                    domain.nGetGrantSet(out granted, out denied);
                    if (granted == null)
                    {
                        throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DomainGrantSet"));
                    }
                }
                ActivationContext activationContext = AppDomain.CurrentDomain.ActivationContext;
                if (activationContext == null)
                {
                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_ApplicationMissingIdentity"));
                }
                ApplicationSecurityInfo info = new ApplicationSecurityInfo(activationContext);
                this._InitStore(scope, null, null, null, null, info.ApplicationEvidence, appEvidenceType);
            }
            this.SetQuota(granted, denied);
        }

        protected void InitStore(IsolatedStorageScope scope, Type domainEvidenceType, Type assemblyEvidenceType)
        {
            PermissionSet granted = null;
            PermissionSet denied = null;
            Assembly assembly = nGetCaller();
            GetControlEvidencePermission().Assert();
            if (IsDomain(scope))
            {
                AppDomain domain = Thread.GetDomain();
                if (!IsRoaming(scope))
                {
                    domain.nGetGrantSet(out granted, out denied);
                    if (granted == null)
                    {
                        throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DomainGrantSet"));
                    }
                }
                this._InitStore(scope, domain.Evidence, domainEvidenceType, assembly.Evidence, assemblyEvidenceType, null, null);
            }
            else
            {
                if (!IsRoaming(scope))
                {
                    assembly.nGetGrantSet(out granted, out denied);
                    if (granted == null)
                    {
                        throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_AssemblyGrantSet"));
                    }
                }
                this._InitStore(scope, null, null, assembly.Evidence, assemblyEvidenceType, null, null);
            }
            this.SetQuota(granted, denied);
        }

        internal void InitStore(IsolatedStorageScope scope, object domain, object assem, object app)
        {
            PermissionSet newGrant = null;
            PermissionSet newDenied = null;
            Evidence domainEv = null;
            Evidence assemEv = null;
            Evidence appEv = null;
            if (IsApp(scope))
            {
                appEv = new Evidence();
                appEv.AddHost(app);
            }
            else
            {
                assemEv = new Evidence();
                assemEv.AddHost(assem);
                if (IsDomain(scope))
                {
                    domainEv = new Evidence();
                    domainEv.AddHost(domain);
                }
            }
            this._InitStore(scope, domainEv, null, assemEv, null, appEv, null);
            if (!IsRoaming(scope))
            {
                Assembly assembly = nGetCaller();
                GetControlEvidencePermission().Assert();
                assembly.nGetGrantSet(out newGrant, out newDenied);
                if (newGrant == null)
                {
                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_AssemblyGrantSet"));
                }
            }
            this.SetQuota(newGrant, newDenied);
        }

        internal bool InitStore(IsolatedStorageScope scope, Stream domain, Stream assem, Stream app, string domainName, string assemName, string appName)
        {
            try
            {
                GetReflectionPermission().Assert();
                BinaryFormatter formatter = new BinaryFormatter();
                if (IsApp(scope))
                {
                    this.m_AppIdentity = formatter.Deserialize(app);
                    this.m_AppName = appName;
                }
                else
                {
                    this.m_AssemIdentity = formatter.Deserialize(assem);
                    this.m_AssemName = assemName;
                    if (IsDomain(scope))
                    {
                        this.m_DomainIdentity = formatter.Deserialize(domain);
                        this.m_DomainName = domainName;
                    }
                }
            }
            catch
            {
                return false;
            }
            this.m_Scope = scope;
            return true;
        }

        internal void InitStore(IsolatedStorageScope scope, Evidence domainEv, Type domainEvidenceType, Evidence assemEv, Type assemEvidenceType, Evidence appEv, Type appEvidenceType)
        {
            PermissionSet psAllowed = null;
            PermissionSet denied = null;
            if (!IsRoaming(scope))
            {
                if (IsApp(scope))
                {
                    psAllowed = SecurityManager.ResolvePolicy(appEv, GetExecutionPermission(), GetUnrestricted(), null, out denied);
                    if (psAllowed == null)
                    {
                        throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_ApplicationGrantSet"));
                    }
                }
                else if (IsDomain(scope))
                {
                    psAllowed = SecurityManager.ResolvePolicy(domainEv, GetExecutionPermission(), GetUnrestricted(), null, out denied);
                    if (psAllowed == null)
                    {
                        throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DomainGrantSet"));
                    }
                }
                else
                {
                    psAllowed = SecurityManager.ResolvePolicy(assemEv, GetExecutionPermission(), GetUnrestricted(), null, out denied);
                    if (psAllowed == null)
                    {
                        throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_AssemblyGrantSet"));
                    }
                }
            }
            this._InitStore(scope, domainEv, domainEvidenceType, assemEv, assemEvidenceType, appEv, appEvidenceType);
            this.SetQuota(psAllowed, denied);
        }

        internal bool IsApp() => 
            ((this.m_Scope & IsolatedStorageScope.Application) != IsolatedStorageScope.None);

        internal static bool IsApp(IsolatedStorageScope scope) => 
            ((scope & IsolatedStorageScope.Application) != IsolatedStorageScope.None);

        internal bool IsAssembly() => 
            ((this.m_Scope & IsolatedStorageScope.Assembly) != IsolatedStorageScope.None);

        internal bool IsDomain() => 
            ((this.m_Scope & IsolatedStorageScope.Domain) != IsolatedStorageScope.None);

        internal static bool IsDomain(IsolatedStorageScope scope) => 
            ((scope & IsolatedStorageScope.Domain) != IsolatedStorageScope.None);

        internal static bool IsMachine(IsolatedStorageScope scope) => 
            ((scope & IsolatedStorageScope.Machine) != IsolatedStorageScope.None);

        internal bool IsRoaming() => 
            ((this.m_Scope & IsolatedStorageScope.Roaming) != IsolatedStorageScope.None);

        internal static bool IsRoaming(IsolatedStorageScope scope) => 
            ((scope & IsolatedStorageScope.Roaming) != IsolatedStorageScope.None);

        private static bool IsValidName(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsLetter(s[i]) && !char.IsDigit(s[i]))
                {
                    return false;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Assembly nGetCaller();
        public abstract void Remove();
        internal void SetQuota(PermissionSet psAllowed, PermissionSet psDenied)
        {
            IsolatedStoragePermission permission = this.GetPermission(psAllowed);
            this.m_Quota = 0L;
            if (permission != null)
            {
                if (permission.IsUnrestricted())
                {
                    this.m_Quota = 0x7fffffffffffffffL;
                }
                else
                {
                    this.m_Quota = (ulong) permission.UserQuota;
                }
            }
            if (psDenied != null)
            {
                IsolatedStoragePermission permission2 = this.GetPermission(psDenied);
                if (permission2 != null)
                {
                    if (permission2.IsUnrestricted())
                    {
                        this.m_Quota = 0L;
                    }
                    else
                    {
                        ulong userQuota = (ulong) permission2.UserQuota;
                        if (userQuota > this.m_Quota)
                        {
                            this.m_Quota = 0L;
                        }
                        else
                        {
                            this.m_Quota -= userQuota;
                        }
                    }
                }
            }
            this.m_ValidQuota = true;
        }

        internal static string ToBase32StringSuitableForDirName(byte[] buff)
        {
            StringBuilder builder = new StringBuilder();
            int length = buff.Length;
            int num7 = 0;
            do
            {
                byte num = (num7 < length) ? buff[num7++] : ((byte) 0);
                byte num2 = (num7 < length) ? buff[num7++] : ((byte) 0);
                byte index = (num7 < length) ? buff[num7++] : ((byte) 0);
                byte num4 = (num7 < length) ? buff[num7++] : ((byte) 0);
                byte num5 = (num7 < length) ? buff[num7++] : ((byte) 0);
                builder.Append(s_Base32Char[num & 0x1f]);
                builder.Append(s_Base32Char[num2 & 0x1f]);
                builder.Append(s_Base32Char[index & 0x1f]);
                builder.Append(s_Base32Char[num4 & 0x1f]);
                builder.Append(s_Base32Char[num5 & 0x1f]);
                builder.Append(s_Base32Char[((num & 0xe0) >> 5) | ((num4 & 0x60) >> 2)]);
                builder.Append(s_Base32Char[((num2 & 0xe0) >> 5) | ((num5 & 0x60) >> 2)]);
                index = (byte) (index >> 5);
                if ((num4 & 0x80) != 0)
                {
                    index = (byte) (index | 8);
                }
                if ((num5 & 0x80) != 0)
                {
                    index = (byte) (index | 0x10);
                }
                builder.Append(s_Base32Char[index]);
            }
            while (num7 < length);
            return builder.ToString();
        }

        internal static void VerifyScope(IsolatedStorageScope scope)
        {
            if ((((scope != (IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain | IsolatedStorageScope.User)) && (scope != (IsolatedStorageScope.Assembly | IsolatedStorageScope.User))) && ((scope != (IsolatedStorageScope.Roaming | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain | IsolatedStorageScope.User)) && (scope != (IsolatedStorageScope.Roaming | IsolatedStorageScope.Assembly | IsolatedStorageScope.User)))) && (((scope != (IsolatedStorageScope.Machine | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain)) && (scope != (IsolatedStorageScope.Machine | IsolatedStorageScope.Assembly))) && (((scope != (IsolatedStorageScope.Application | IsolatedStorageScope.User)) && (scope != (IsolatedStorageScope.Application | IsolatedStorageScope.Machine))) && (scope != (IsolatedStorageScope.Application | IsolatedStorageScope.Roaming | IsolatedStorageScope.User)))))
            {
                throw new ArgumentException(Environment.GetResourceString("IsolatedStorage_Scope_Invalid"));
            }
        }

        [ComVisible(false)]
        public object ApplicationIdentity
        {
            [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPolicy)]
            get
            {
                if (!this.IsApp())
                {
                    throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_ApplicationUndefined"));
                }
                return this.m_AppIdentity;
            }
        }

        internal string AppName
        {
            get
            {
                if (!this.IsApp())
                {
                    throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_ApplicationUndefined"));
                }
                return this.m_AppName;
            }
        }

        public object AssemblyIdentity
        {
            [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPolicy)]
            get
            {
                if (!this.IsAssembly())
                {
                    throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_AssemblyUndefined"));
                }
                return this.m_AssemIdentity;
            }
        }

        internal string AssemName
        {
            get
            {
                if (!this.IsAssembly())
                {
                    throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_AssemblyUndefined"));
                }
                return this.m_AssemName;
            }
        }

        [CLSCompliant(false)]
        public virtual ulong CurrentSize
        {
            get
            {
                throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_CurrentSizeUndefined"));
            }
        }

        public object DomainIdentity
        {
            [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPolicy)]
            get
            {
                if (!this.IsDomain())
                {
                    throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_DomainUndefined"));
                }
                return this.m_DomainIdentity;
            }
        }

        internal string DomainName
        {
            get
            {
                if (!this.IsDomain())
                {
                    throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_DomainUndefined"));
                }
                return this.m_DomainName;
            }
        }

        [CLSCompliant(false)]
        public virtual ulong MaximumSize
        {
            get
            {
                if (!this.m_ValidQuota)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_QuotaIsUndefined"));
                }
                return this.m_Quota;
            }
        }

        public IsolatedStorageScope Scope =>
            this.m_Scope;

        protected virtual char SeparatorExternal =>
            '\\';

        protected virtual char SeparatorInternal =>
            '.';
    }
}

