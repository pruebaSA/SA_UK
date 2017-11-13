namespace System.Security.RightsManagement
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Security;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    public class LocalizedNameDescriptionPair
    {
        private string _description;
        private string _name;

        public LocalizedNameDescriptionPair(string name, string description)
        {
            SecurityHelper.DemandRightsManagementPermission();
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }
            this._name = name;
            this._description = description;
        }

        public override bool Equals(object obj)
        {
            SecurityHelper.DemandRightsManagementPermission();
            if ((obj == null) || (obj.GetType() != base.GetType()))
            {
                return false;
            }
            LocalizedNameDescriptionPair pair = obj as LocalizedNameDescriptionPair;
            return ((string.CompareOrdinal(pair.Name, this.Name) == 0) && (string.CompareOrdinal(pair.Description, this.Description) == 0));
        }

        public override int GetHashCode()
        {
            SecurityHelper.DemandRightsManagementPermission();
            return (this.Name.GetHashCode() ^ this.Description.GetHashCode());
        }

        public string Description
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._description;
            }
        }

        public string Name
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._name;
            }
        }
    }
}

