namespace System.Security.Principal
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(false)]
    public sealed class NTAccount : IdentityReference
    {
        private readonly string _Name;
        internal const int MaximumAccountNameLength = 0x100;
        internal const int MaximumDomainNameLength = 0xff;

        public NTAccount(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_StringZeroLength"), "name");
            }
            if (name.Length > 0x200)
            {
                throw new ArgumentException(Environment.GetResourceString("IdentityReference_AccountNameTooLong"), "name");
            }
            this._Name = name;
        }

        public NTAccount(string domainName, string accountName)
        {
            if (accountName == null)
            {
                throw new ArgumentNullException("accountName");
            }
            if (accountName.Length == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_StringZeroLength"), "accountName");
            }
            if (accountName.Length > 0x100)
            {
                throw new ArgumentException(Environment.GetResourceString("IdentityReference_AccountNameTooLong"), "accountName");
            }
            if ((domainName != null) && (domainName.Length > 0xff))
            {
                throw new ArgumentException(Environment.GetResourceString("IdentityReference_DomainNameTooLong"), "domainName");
            }
            if ((domainName == null) || (domainName.Length == 0))
            {
                this._Name = accountName;
            }
            else
            {
                this._Name = domainName + @"\" + accountName;
            }
        }

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }
            NTAccount account = o as NTAccount;
            if (account == null)
            {
                return false;
            }
            return (this == account);
        }

        public override int GetHashCode() => 
            StringComparer.InvariantCultureIgnoreCase.GetHashCode(this._Name);

        public override bool IsValidTargetType(Type targetType) => 
            ((targetType == typeof(SecurityIdentifier)) || (targetType == typeof(NTAccount)));

        public static bool operator ==(NTAccount left, NTAccount right)
        {
            object obj2 = left;
            object obj3 = right;
            return (((obj2 == null) && (obj3 == null)) || (((obj2 != null) && (obj3 != null)) && left.ToString().Equals(right.ToString(), StringComparison.OrdinalIgnoreCase)));
        }

        public static bool operator !=(NTAccount left, NTAccount right) => 
            !(left == right);

        public override string ToString() => 
            this._Name;

        public override IdentityReference Translate(Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }
            if (targetType == typeof(NTAccount))
            {
                return this;
            }
            if (targetType != typeof(SecurityIdentifier))
            {
                throw new ArgumentException(Environment.GetResourceString("IdentityReference_MustBeIdentityReference"), "targetType");
            }
            IdentityReferenceCollection sourceAccounts = new IdentityReferenceCollection(1) {
                this
            };
            return Translate(sourceAccounts, targetType, 1)[0];
        }

        internal static IdentityReferenceCollection Translate(IdentityReferenceCollection sourceAccounts, Type targetType, bool forceSuccess)
        {
            bool someFailed = false;
            IdentityReferenceCollection references = Translate(sourceAccounts, targetType, out someFailed);
            if (!forceSuccess || !someFailed)
            {
                return references;
            }
            IdentityReferenceCollection unmappedIdentities = new IdentityReferenceCollection();
            foreach (IdentityReference reference in references)
            {
                if (reference.GetType() != targetType)
                {
                    unmappedIdentities.Add(reference);
                }
            }
            throw new IdentityNotMappedException(Environment.GetResourceString("IdentityReference_IdentityNotMapped"), unmappedIdentities);
        }

        internal static IdentityReferenceCollection Translate(IdentityReferenceCollection sourceAccounts, Type targetType, out bool someFailed)
        {
            if (sourceAccounts == null)
            {
                throw new ArgumentNullException("sourceAccounts");
            }
            if (targetType != typeof(SecurityIdentifier))
            {
                throw new ArgumentException(Environment.GetResourceString("IdentityReference_MustBeIdentityReference"), "targetType");
            }
            return TranslateToSids(sourceAccounts, out someFailed);
        }

        private static IdentityReferenceCollection TranslateToSids(IdentityReferenceCollection sourceAccounts, out bool someFailed)
        {
            IdentityReferenceCollection references2;
            if (!Win32.LsaApisSupported)
            {
                throw new PlatformNotSupportedException(Environment.GetResourceString("PlatformNotSupported_Win9x"));
            }
            SafeLsaPolicyHandle invalidHandle = SafeLsaPolicyHandle.InvalidHandle;
            SafeLsaMemoryHandle referencedDomains = SafeLsaMemoryHandle.InvalidHandle;
            SafeLsaMemoryHandle sids = SafeLsaMemoryHandle.InvalidHandle;
            int index = 0;
            if (sourceAccounts == null)
            {
                throw new ArgumentNullException("sourceAccounts");
            }
            if (sourceAccounts.Count == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_EmptyCollection"), "sourceAccounts");
            }
            try
            {
                uint num2;
                Win32Native.UNICODE_STRING[] names = new Win32Native.UNICODE_STRING[sourceAccounts.Count];
                foreach (IdentityReference reference in sourceAccounts)
                {
                    NTAccount account = reference as NTAccount;
                    if (account == null)
                    {
                        throw new ArgumentException(Environment.GetResourceString("Argument_ImproperType"), "sourceAccounts");
                    }
                    names[index].Buffer = account.ToString();
                    if (((names[index].Buffer.Length * 2) + 2) > 0xffff)
                    {
                        throw new SystemException();
                    }
                    names[index].Length = (ushort) (names[index].Buffer.Length * 2);
                    names[index].MaximumLength = (ushort) (names[index].Length + 2);
                    index++;
                }
                invalidHandle = Win32.LsaOpenPolicy(null, PolicyRights.POLICY_LOOKUP_NAMES);
                someFailed = false;
                if (Win32.LsaLookupNames2Supported)
                {
                    num2 = Win32Native.LsaLookupNames2(invalidHandle, 0, sourceAccounts.Count, names, ref referencedDomains, ref sids);
                }
                else
                {
                    num2 = Win32Native.LsaLookupNames(invalidHandle, sourceAccounts.Count, names, ref referencedDomains, ref sids);
                }
                if ((num2 == 0xc0000017) || (num2 == 0xc000009a))
                {
                    throw new OutOfMemoryException();
                }
                if (num2 == 0xc0000022)
                {
                    throw new UnauthorizedAccessException();
                }
                if ((num2 == 0xc0000073) || (num2 == 0x107))
                {
                    someFailed = true;
                }
                else if (num2 != 0)
                {
                    throw new SystemException(Win32Native.GetMessage(Win32Native.LsaNtStatusToWinError((int) num2)));
                }
                IdentityReferenceCollection references = new IdentityReferenceCollection(sourceAccounts.Count);
                switch (num2)
                {
                    case 0:
                    case 0x107:
                        if (Win32.LsaLookupNames2Supported)
                        {
                            for (index = 0; index < sourceAccounts.Count; index++)
                            {
                                Win32Native.LSA_TRANSLATED_SID2 lsa_translated_sid = (Win32Native.LSA_TRANSLATED_SID2) Marshal.PtrToStructure(new IntPtr(((long) sids.DangerousGetHandle()) + (index * Marshal.SizeOf(typeof(Win32Native.LSA_TRANSLATED_SID2)))), typeof(Win32Native.LSA_TRANSLATED_SID2));
                                switch (lsa_translated_sid.Use)
                                {
                                    case 1:
                                    case 2:
                                    case 4:
                                    case 5:
                                    case 9:
                                    {
                                        references.Add(new SecurityIdentifier(lsa_translated_sid.Sid, true));
                                        continue;
                                    }
                                }
                                someFailed = true;
                                references.Add(sourceAccounts[index]);
                            }
                        }
                        else
                        {
                            Win32Native.LSA_REFERENCED_DOMAIN_LIST lsa_referenced_domain_list = (Win32Native.LSA_REFERENCED_DOMAIN_LIST) Marshal.PtrToStructure(referencedDomains.DangerousGetHandle(), typeof(Win32Native.LSA_REFERENCED_DOMAIN_LIST));
                            SecurityIdentifier[] identifierArray = new SecurityIdentifier[lsa_referenced_domain_list.Entries];
                            for (index = 0; index < lsa_referenced_domain_list.Entries; index++)
                            {
                                Win32Native.LSA_TRUST_INFORMATION lsa_trust_information = (Win32Native.LSA_TRUST_INFORMATION) Marshal.PtrToStructure(new IntPtr(((long) lsa_referenced_domain_list.Domains) + (index * Marshal.SizeOf(typeof(Win32Native.LSA_TRUST_INFORMATION)))), typeof(Win32Native.LSA_TRUST_INFORMATION));
                                identifierArray[index] = new SecurityIdentifier(lsa_trust_information.Sid, true);
                            }
                            for (index = 0; index < sourceAccounts.Count; index++)
                            {
                                Win32Native.LSA_TRANSLATED_SID lsa_translated_sid2 = (Win32Native.LSA_TRANSLATED_SID) Marshal.PtrToStructure(new IntPtr(((long) sids.DangerousGetHandle()) + (index * Marshal.SizeOf(typeof(Win32Native.LSA_TRANSLATED_SID)))), typeof(Win32Native.LSA_TRANSLATED_SID));
                                switch (lsa_translated_sid2.Use)
                                {
                                    case 1:
                                    case 2:
                                    case 4:
                                    case 5:
                                    case 9:
                                    {
                                        references.Add(new SecurityIdentifier(identifierArray[lsa_translated_sid2.DomainIndex], lsa_translated_sid2.Rid));
                                        continue;
                                    }
                                }
                                someFailed = true;
                                references.Add(sourceAccounts[index]);
                            }
                        }
                        break;

                    default:
                        for (index = 0; index < sourceAccounts.Count; index++)
                        {
                            references.Add(sourceAccounts[index]);
                        }
                        break;
                }
                references2 = references;
            }
            finally
            {
                invalidHandle.Dispose();
                referencedDomains.Dispose();
                sids.Dispose();
            }
            return references2;
        }

        public override string Value =>
            this.ToString();
    }
}

