namespace System.Security
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Security.Policy;
    using System.Security.Util;
    using System.Text;
    using System.Threading;

    internal class PolicyManager
    {
        private object m_policyLevels;

        internal PolicyManager()
        {
        }

        internal void AddLevel(PolicyLevel level)
        {
            this.PolicyLevels.Add(level);
        }

        internal static bool CanUseQuickCache(CodeGroup group)
        {
            ArrayList list = new ArrayList {
                group
            };
            for (int i = 0; i < list.Count; i++)
            {
                group = (CodeGroup) list[i];
                if (group is IUnionSemanticCodeGroup)
                {
                    if (!TestPolicyStatement(group.PolicyStatement))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                IMembershipCondition membershipCondition = group.MembershipCondition;
                if ((membershipCondition != null) && !(membershipCondition is IConstantMembershipCondition))
                {
                    return false;
                }
                IList children = group.Children;
                if ((children != null) && (children.Count > 0))
                {
                    IEnumerator enumerator = children.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        list.Add(enumerator.Current);
                    }
                }
            }
            return true;
        }

        internal static bool CheckMembershipCondition(IMembershipCondition membershipCondition, Evidence evidence, out object usedEvidence)
        {
            IReportMatchMembershipCondition condition = membershipCondition as IReportMatchMembershipCondition;
            if (condition != null)
            {
                return condition.Check(evidence, out usedEvidence);
            }
            usedEvidence = null;
            evidence.MarkAllEvidenceAsUsed();
            return membershipCondition.Check(evidence);
        }

        internal PermissionSet CodeGroupResolve(Evidence evidence, bool systemPolicy)
        {
            PermissionSet permissionSet = null;
            PolicyLevel current = null;
            IEnumerator enumerator = this.PolicyLevels.GetEnumerator();
            char[] serializedEvidence = MakeEvidenceArray(evidence, false);
            int count = evidence.Count;
            bool flag = AppDomain.CurrentDomain.GetData("IgnoreSystemPolicy") != null;
            bool flag2 = false;
            while (enumerator.MoveNext())
            {
                PolicyStatement statement;
                current = (PolicyLevel) enumerator.Current;
                if (systemPolicy)
                {
                    if (current.Type != PolicyLevelType.AppDomain)
                    {
                        goto Label_0064;
                    }
                    continue;
                }
                if (flag && (current.Type != PolicyLevelType.AppDomain))
                {
                    continue;
                }
            Label_0064:
                statement = current.Resolve(evidence, count, serializedEvidence);
                if (permissionSet == null)
                {
                    permissionSet = statement.PermissionSet;
                }
                else
                {
                    permissionSet.InplaceIntersect(statement.GetPermissionSetNoCopy());
                }
                if ((permissionSet == null) || permissionSet.FastIsEmpty())
                {
                    break;
                }
                if ((statement.Attributes & PolicyStatementAttribute.LevelFinal) == PolicyStatementAttribute.LevelFinal)
                {
                    if (current.Type != PolicyLevelType.AppDomain)
                    {
                        flag2 = true;
                    }
                    break;
                }
            }
            if ((permissionSet != null) && flag2)
            {
                PolicyLevel level2 = null;
                for (int i = this.PolicyLevels.Count - 1; i >= 0; i--)
                {
                    current = (PolicyLevel) this.PolicyLevels[i];
                    if (current.Type == PolicyLevelType.AppDomain)
                    {
                        level2 = current;
                        break;
                    }
                }
                if (level2 != null)
                {
                    permissionSet.InplaceIntersect(level2.Resolve(evidence, count, serializedEvidence).GetPermissionSetNoCopy());
                }
            }
            if (permissionSet == null)
            {
                permissionSet = new PermissionSet(PermissionState.None);
            }
            if (!CodeAccessSecurityEngine.DoesFullTrustMeanFullTrust() || !permissionSet.IsUnrestricted())
            {
                IEnumerator hostEnumerator = evidence.GetHostEnumerator();
                while (hostEnumerator.MoveNext())
                {
                    object obj2 = hostEnumerator.Current;
                    IIdentityPermissionFactory factory = obj2 as IIdentityPermissionFactory;
                    if (factory != null)
                    {
                        IPermission perm = factory.CreateIdentityPermission(evidence);
                        if (perm != null)
                        {
                            permissionSet.AddPermission(perm);
                        }
                    }
                }
            }
            permissionSet.IgnoreTypeLoadFailures = true;
            return permissionSet;
        }

        internal static void EncodeLevel(PolicyLevel level)
        {
            SecurityElement element = new SecurityElement("configuration");
            SecurityElement child = new SecurityElement("mscorlib");
            SecurityElement element3 = new SecurityElement("security");
            SecurityElement element4 = new SecurityElement("policy");
            element.AddChild(child);
            child.AddChild(element3);
            element3.AddChild(element4);
            element4.AddChild(level.ToXml());
            try
            {
                StringBuilder builder = new StringBuilder();
                Encoding encoding = Encoding.UTF8;
                SecurityElement element5 = new SecurityElement("xml") {
                    m_type = SecurityElementType.Format
                };
                element5.AddAttribute("version", "1.0");
                element5.AddAttribute("encoding", encoding.WebName);
                builder.Append(element5.ToString());
                builder.Append(element.ToString());
                byte[] bytes = encoding.GetBytes(builder.ToString());
                if ((level.Path == null) || !Config.SaveDataByte(level.Path, bytes, 0, bytes.Length))
                {
                    throw new PolicyException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Policy_UnableToSave"), new object[] { level.Label }));
                }
            }
            catch (Exception exception)
            {
                if (exception is PolicyException)
                {
                    throw exception;
                }
                throw new PolicyException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Policy_UnableToSave"), new object[] { level.Label }), exception);
            }
            catch
            {
                throw new PolicyException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Policy_UnableToSave"), new object[] { level.Label }));
            }
            Config.ResetCacheData(level.ConfigId);
            if (CanUseQuickCache(level.RootCodeGroup))
            {
                Config.SetQuickCache(level.ConfigId, GenerateQuickCache(level));
            }
        }

        private void EncodeLevel(string label)
        {
            for (int i = 0; i < this.PolicyLevels.Count; i++)
            {
                PolicyLevel level = (PolicyLevel) this.PolicyLevels[i];
                if (level.Label.Equals(label))
                {
                    EncodeLevel(level);
                    return;
                }
            }
        }

        private static QuickCacheEntryType GenerateQuickCache(PolicyLevel level)
        {
            QuickCacheEntryType[] typeArray = new QuickCacheEntryType[] { QuickCacheEntryType.FullTrustZoneMyComputer, QuickCacheEntryType.FullTrustZoneIntranet, QuickCacheEntryType.FullTrustZoneInternet, QuickCacheEntryType.FullTrustZoneTrusted, QuickCacheEntryType.FullTrustZoneUntrusted };
            QuickCacheEntryType type = 0;
            Evidence evidence = new Evidence();
            try
            {
                if (level.Resolve(evidence).PermissionSet.IsUnrestricted())
                {
                    type |= QuickCacheEntryType.FullTrustAll;
                }
            }
            catch (PolicyException)
            {
            }
            Array values = Enum.GetValues(typeof(SecurityZone));
            for (int i = 0; i < values.Length; i++)
            {
                if (((SecurityZone) values.GetValue(i)) != SecurityZone.NoZone)
                {
                    Evidence evidence2 = new Evidence();
                    evidence2.AddHost(new Zone((SecurityZone) values.GetValue(i)));
                    try
                    {
                        if (level.Resolve(evidence2).PermissionSet.IsUnrestricted())
                        {
                            type |= typeArray[i];
                        }
                    }
                    catch (PolicyException)
                    {
                    }
                }
            }
            return type;
        }

        private static bool IsFullTrust(Evidence evidence, ApplicationTrust appTrust)
        {
            if (appTrust != null)
            {
                StrongName[] fullTrustAssemblies = appTrust.FullTrustAssemblies;
                if (fullTrustAssemblies != null)
                {
                    for (int i = 0; i < fullTrustAssemblies.Length; i++)
                    {
                        if (fullTrustAssemblies[i] != null)
                        {
                            StrongNameMembershipCondition condition = new StrongNameMembershipCondition(fullTrustAssemblies[i].PublicKey, fullTrustAssemblies[i].Name, fullTrustAssemblies[i].Version);
                            object usedEvidence = null;
                            if (((IReportMatchMembershipCondition) condition).Check(evidence, out usedEvidence))
                            {
                                IDelayEvaluatedEvidence evidence2 = usedEvidence as IDelayEvaluatedEvidence;
                                if (usedEvidence != null)
                                {
                                    evidence2.MarkUsed();
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal static bool IsGacAssembly(Evidence evidence) => 
            new GacMembershipCondition().Check(evidence);

        internal static char[] MakeEvidenceArray(Evidence evidence, bool verbose)
        {
            IEnumerator enumerator = evidence.GetEnumerator();
            int num = 0;
            while (enumerator.MoveNext())
            {
                IBuiltInEvidence current = enumerator.Current as IBuiltInEvidence;
                if (current == null)
                {
                    return null;
                }
                num += current.GetRequiredSize(verbose);
            }
            enumerator.Reset();
            char[] buffer = new char[num];
            for (int i = 0; enumerator.MoveNext(); i = ((IBuiltInEvidence) enumerator.Current).OutputToBuffer(buffer, i, verbose))
            {
            }
            return buffer;
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPolicy)]
        internal IEnumerator PolicyHierarchy() => 
            this.PolicyLevels.GetEnumerator();

        internal PermissionSet Resolve(Evidence evidence)
        {
            if (!IsGacAssembly(evidence))
            {
                HostSecurityManager hostSecurityManager = AppDomain.CurrentDomain.HostSecurityManager;
                if ((hostSecurityManager.Flags & HostSecurityManagerOptions.HostResolvePolicy) == HostSecurityManagerOptions.HostResolvePolicy)
                {
                    return hostSecurityManager.ResolvePolicy(evidence);
                }
            }
            return this.ResolveHelper(evidence);
        }

        internal static PolicyStatement ResolveCodeGroup(CodeGroup codeGroup, Evidence evidence)
        {
            if (codeGroup.GetType().Assembly != typeof(UnionCodeGroup).Assembly)
            {
                evidence.MarkAllEvidenceAsUsed();
            }
            return codeGroup.Resolve(evidence);
        }

        internal IEnumerator ResolveCodeGroups(Evidence evidence)
        {
            ArrayList list = new ArrayList();
            IEnumerator enumerator = this.PolicyLevels.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CodeGroup group = ((PolicyLevel) enumerator.Current).ResolveMatchingCodeGroups(evidence);
                if (group != null)
                {
                    list.Add(group);
                }
            }
            return list.GetEnumerator(0, list.Count);
        }

        internal PermissionSet ResolveHelper(Evidence evidence)
        {
            if (IsGacAssembly(evidence))
            {
                return new PermissionSet(PermissionState.Unrestricted);
            }
            ApplicationTrust applicationTrust = AppDomain.CurrentDomain.ApplicationTrust;
            if (applicationTrust != null)
            {
                if (IsFullTrust(evidence, applicationTrust))
                {
                    return new PermissionSet(PermissionState.Unrestricted);
                }
                return applicationTrust.DefaultGrantSet.PermissionSet;
            }
            return this.CodeGroupResolve(evidence, false);
        }

        internal void Save()
        {
            this.EncodeLevel(Environment.GetResourceString("Policy_PL_Enterprise"));
            this.EncodeLevel(Environment.GetResourceString("Policy_PL_Machine"));
            this.EncodeLevel(Environment.GetResourceString("Policy_PL_User"));
        }

        private static bool TestPolicyStatement(PolicyStatement policy) => 
            ((policy == null) || ((policy.Attributes & PolicyStatementAttribute.Exclusive) == PolicyStatementAttribute.Nothing));

        private IList PolicyLevels
        {
            get
            {
                if (this.m_policyLevels == null)
                {
                    ArrayList list = new ArrayList();
                    string locationFromType = PolicyLevel.GetLocationFromType(PolicyLevelType.Enterprise);
                    list.Add(new PolicyLevel(PolicyLevelType.Enterprise, locationFromType, ConfigId.EnterprisePolicyLevel));
                    string path = PolicyLevel.GetLocationFromType(PolicyLevelType.Machine);
                    list.Add(new PolicyLevel(PolicyLevelType.Machine, path, ConfigId.MachinePolicyLevel));
                    if (Config.UserDirectory != null)
                    {
                        string str3 = PolicyLevel.GetLocationFromType(PolicyLevelType.User);
                        list.Add(new PolicyLevel(PolicyLevelType.User, str3, ConfigId.UserPolicyLevel));
                    }
                    Interlocked.CompareExchange(ref this.m_policyLevels, list, null);
                }
                return (this.m_policyLevels as ArrayList);
            }
        }
    }
}

