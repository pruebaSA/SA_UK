﻿namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class RoleGroup
    {
        private ITemplate _contentTemplate;
        private string[] _roles;

        public bool ContainsUser(IPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (this._roles != null)
            {
                foreach (string str in this._roles)
                {
                    if (user.IsInRole(str))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            StringArrayConverter converter = new StringArrayConverter();
            return converter.ConvertToString(this.Roles);
        }

        [TemplateContainer(typeof(LoginView)), Browsable(false), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate ContentTemplate
        {
            get => 
                this._contentTemplate;
            set
            {
                this._contentTemplate = value;
            }
        }

        [TypeConverter(typeof(StringArrayConverter))]
        public string[] Roles
        {
            get
            {
                if (this._roles == null)
                {
                    return new string[0];
                }
                return (string[]) this._roles.Clone();
            }
            set
            {
                if (value == null)
                {
                    this._roles = value;
                }
                else
                {
                    this._roles = (string[]) value.Clone();
                }
            }
        }
    }
}

