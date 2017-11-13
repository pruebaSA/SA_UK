namespace System.DirectoryServices
{
    using System;

    internal sealed class ActiveDirectoryRightsTranslator
    {
        internal static int AccessMaskFromRights(ActiveDirectoryRights adRights) => 
            ((int) adRights);

        internal static ActiveDirectoryRights RightsFromAccessMask(int accessMask) => 
            ((ActiveDirectoryRights) accessMask);
    }
}

