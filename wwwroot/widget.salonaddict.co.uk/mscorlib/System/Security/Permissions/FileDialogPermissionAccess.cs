namespace System.Security.Permissions
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), Flags]
    public enum FileDialogPermissionAccess
    {
        None,
        Open,
        Save,
        OpenSave
    }
}

