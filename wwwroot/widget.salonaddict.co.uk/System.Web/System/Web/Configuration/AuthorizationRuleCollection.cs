namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Web;

    [ConfigurationCollection(typeof(AuthorizationRule), AddItemName="allow,deny", CollectionType=ConfigurationElementCollectionType.BasicMapAlternate), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class AuthorizationRuleCollection : ConfigurationElementCollection
    {
        private bool _fCheckForCommonCasesDone;
        private int _iAllUsersAllowed;
        private int _iAnonymousAllowed;
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public void Add(AuthorizationRule rule)
        {
            this.BaseAdd(-1, rule);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new AuthorizationRule();

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            AuthorizationRule rule = new AuthorizationRule();
            string str = elementName.ToLower(CultureInfo.InvariantCulture);
            if (str != null)
            {
                if (str != "allow")
                {
                    if (str == "deny")
                    {
                        rule.Action = AuthorizationRuleAction.Deny;
                    }
                    return rule;
                }
                rule.Action = AuthorizationRuleAction.Allow;
            }
            return rule;
        }

        private void DoCheckForCommonCases()
        {
            bool flag = true;
            bool flag2 = false;
            bool flag3 = false;
            foreach (AuthorizationRule rule in this)
            {
                if (rule.Everyone)
                {
                    if (!flag2 && (rule.Action == AuthorizationRuleAction.Deny))
                    {
                        this._iAllUsersAllowed = -1;
                    }
                    if (!flag3 && (rule.Action == AuthorizationRuleAction.Allow))
                    {
                        this._iAllUsersAllowed = 1;
                    }
                    break;
                }
                if (flag && rule.IncludesAnonymous)
                {
                    if (!flag2 && (rule.Action == AuthorizationRuleAction.Deny))
                    {
                        this._iAnonymousAllowed = -1;
                    }
                    if (!flag3 && (rule.Action == AuthorizationRuleAction.Allow))
                    {
                        this._iAnonymousAllowed = 1;
                    }
                    flag = false;
                }
                if (!flag2 && (rule.Action == AuthorizationRuleAction.Allow))
                {
                    flag2 = true;
                }
                if (!flag3 && (rule.Action == AuthorizationRuleAction.Deny))
                {
                    flag3 = true;
                }
                if ((!flag && flag2) && flag3)
                {
                    break;
                }
            }
        }

        public AuthorizationRule Get(int index) => 
            ((AuthorizationRule) base.BaseGet(index));

        protected override object GetElementKey(ConfigurationElement element)
        {
            AuthorizationRule rule = (AuthorizationRule) element;
            return rule._ActionString;
        }

        public int IndexOf(AuthorizationRule rule)
        {
            for (int i = 0; i < base.Count; i++)
            {
                if (object.Equals(this.Get(i), rule))
                {
                    return i;
                }
            }
            return -1;
        }

        protected override bool IsElementName(string elementname)
        {
            string str;
            bool flag = false;
            if (((str = elementname.ToLower(CultureInfo.InvariantCulture)) == null) || ((str != "allow") && (str != "deny")))
            {
                return flag;
            }
            return true;
        }

        internal bool IsUserAllowed(IPrincipal user, string verb)
        {
            if (user != null)
            {
                if (!this._fCheckForCommonCasesDone)
                {
                    this.DoCheckForCommonCases();
                    this._fCheckForCommonCasesDone = true;
                }
                if (!user.Identity.IsAuthenticated && (this._iAnonymousAllowed != 0))
                {
                    return (this._iAnonymousAllowed > 0);
                }
                if (this._iAllUsersAllowed != 0)
                {
                    return (this._iAllUsersAllowed > 0);
                }
                foreach (AuthorizationRule rule in this)
                {
                    int num = rule.IsUserAllowed(user, verb);
                    if (num != 0)
                    {
                        return (num > 0);
                    }
                }
            }
            return false;
        }

        public void Remove(AuthorizationRule rule)
        {
            int index = this.IndexOf(rule);
            if (index >= 0)
            {
                base.BaseRemoveAt(index);
            }
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public void Set(int index, AuthorizationRule rule)
        {
            this.BaseAdd(index, rule);
        }

        public override ConfigurationElementCollectionType CollectionType =>
            ConfigurationElementCollectionType.BasicMapAlternate;

        protected override string ElementName =>
            string.Empty;

        public AuthorizationRule this[int index]
        {
            get => 
                ((AuthorizationRule) base.BaseGet(index));
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

