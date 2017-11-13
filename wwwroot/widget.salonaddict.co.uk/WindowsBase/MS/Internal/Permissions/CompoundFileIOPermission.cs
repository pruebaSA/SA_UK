namespace MS.Internal.Permissions
{
    using MS.Internal.WindowsBase;
    using System.Security;

    [FriendAccessAllowed]
    internal class CompoundFileIOPermission : InternalPermissionBase
    {
        public override IPermission Copy() => 
            new CompoundFileIOPermission();
    }
}

