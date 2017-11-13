namespace System.Security.Permissions
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security;

    [Serializable, ComVisible(true)]
    public sealed class GacIdentityPermission : CodeAccessPermission, IBuiltInPermission
    {
        public GacIdentityPermission()
        {
        }

        public GacIdentityPermission(PermissionState state)
        {
            if (state == PermissionState.Unrestricted)
            {
                if (!CodeAccessSecurityEngine.DoesFullTrustMeanFullTrust())
                {
                    throw new ArgumentException(Environment.GetResourceString("Argument_UnrestrictedIdentityPermission"));
                }
            }
            else if (state != PermissionState.None)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidPermissionState"));
            }
        }

        public override IPermission Copy() => 
            new GacIdentityPermission();

        public override void FromXml(SecurityElement securityElement)
        {
            CodeAccessPermission.ValidateElement(securityElement, this);
        }

        internal static int GetTokenIndex() => 
            15;

        public override IPermission Intersect(IPermission target)
        {
            if (target == null)
            {
                return null;
            }
            if (!(target is GacIdentityPermission))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_WrongType"), new object[] { base.GetType().FullName }));
            }
            return this.Copy();
        }

        public override bool IsSubsetOf(IPermission target)
        {
            if (target == null)
            {
                return false;
            }
            if (!(target is GacIdentityPermission))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_WrongType"), new object[] { base.GetType().FullName }));
            }
            return true;
        }

        int IBuiltInPermission.GetTokenIndex() => 
            GetTokenIndex();

        public override SecurityElement ToXml() => 
            CodeAccessPermission.CreatePermissionElement(this, "System.Security.Permissions.GacIdentityPermission");

        public override IPermission Union(IPermission target)
        {
            if ((target != null) && !(target is GacIdentityPermission))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_WrongType"), new object[] { base.GetType().FullName }));
            }
            return this.Copy();
        }
    }
}

