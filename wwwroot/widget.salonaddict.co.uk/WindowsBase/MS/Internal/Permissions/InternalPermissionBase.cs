namespace MS.Internal.Permissions
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Windows;

    [FriendAccessAllowed]
    internal abstract class InternalPermissionBase : CodeAccessPermission, IUnrestrictedPermission
    {
        public override void FromXml(SecurityElement elem)
        {
        }

        public override IPermission Intersect(IPermission target)
        {
            if (target == null)
            {
                return null;
            }
            if (target.GetType() != base.GetType())
            {
                throw new ArgumentException(System.Windows.SR.Get("InvalidPermissionType"), base.GetType().FullName);
            }
            return this.Copy();
        }

        public override bool IsSubsetOf(IPermission target)
        {
            if (target == null)
            {
                return false;
            }
            if (target.GetType() != base.GetType())
            {
                throw new ArgumentException(System.Windows.SR.Get("InvalidPermissionType"), base.GetType().FullName);
            }
            return true;
        }

        public bool IsUnrestricted() => 
            true;

        public override SecurityElement ToXml()
        {
            SecurityElement element = new SecurityElement("IPermission");
            Type type = base.GetType();
            StringBuilder builder = new StringBuilder(type.Assembly.ToString());
            builder.Replace('"', '\'');
            element.AddAttribute("class", type.FullName + ", " + builder);
            element.AddAttribute("version", "1");
            return element;
        }
    }
}

