namespace System.Security
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable, ComVisible(true)]
    public sealed class NamedPermissionSet : PermissionSet
    {
        private string m_description;
        [OptionalField(VersionAdded=2)]
        internal string m_descrResource;
        private string m_name;

        internal NamedPermissionSet()
        {
        }

        public NamedPermissionSet(NamedPermissionSet permSet) : base(permSet)
        {
            this.m_name = permSet.m_name;
            this.m_description = permSet.Description;
        }

        public NamedPermissionSet(string name)
        {
            CheckName(name);
            this.m_name = name;
        }

        public NamedPermissionSet(string name, PermissionState state) : base(state)
        {
            CheckName(name);
            this.m_name = name;
        }

        public NamedPermissionSet(string name, PermissionSet permSet) : base(permSet)
        {
            CheckName(name);
            this.m_name = name;
        }

        private static void CheckName(string name)
        {
            if ((name == null) || name.Equals(""))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_NPMSInvalidName"));
            }
        }

        public override PermissionSet Copy() => 
            new NamedPermissionSet(this);

        public NamedPermissionSet Copy(string name) => 
            new NamedPermissionSet(this) { Name = name };

        [ComVisible(false)]
        public override bool Equals(object obj) => 
            base.Equals(obj);

        public override void FromXml(SecurityElement et)
        {
            this.FromXml(et, false, false);
        }

        internal override void FromXml(SecurityElement et, bool allowInternalOnly, bool ignoreTypeLoadFailures)
        {
            if (et == null)
            {
                throw new ArgumentNullException("et");
            }
            string str = et.Attribute("Name");
            this.m_name = (str == null) ? null : str;
            str = et.Attribute("Description");
            this.m_description = (str == null) ? "" : str;
            this.m_descrResource = null;
            base.FromXml(et, allowInternalOnly, ignoreTypeLoadFailures);
        }

        internal void FromXmlNameOnly(SecurityElement et)
        {
            string str = et.Attribute("Name");
            this.m_name = (str == null) ? null : str;
        }

        [ComVisible(false)]
        public override int GetHashCode() => 
            base.GetHashCode();

        public override SecurityElement ToXml()
        {
            SecurityElement element = base.ToXml("System.Security.NamedPermissionSet");
            if ((this.m_name != null) && !this.m_name.Equals(""))
            {
                element.AddAttribute("Name", SecurityElement.Escape(this.m_name));
            }
            if ((this.Description != null) && !this.Description.Equals(""))
            {
                element.AddAttribute("Description", SecurityElement.Escape(this.Description));
            }
            return element;
        }

        public string Description
        {
            get
            {
                if (this.m_descrResource != null)
                {
                    this.m_description = Environment.GetResourceString(this.m_descrResource);
                    this.m_descrResource = null;
                }
                return this.m_description;
            }
            set
            {
                this.m_description = value;
                this.m_descrResource = null;
            }
        }

        public string Name
        {
            get => 
                this.m_name;
            set
            {
                CheckName(value);
                this.m_name = value;
            }
        }
    }
}

