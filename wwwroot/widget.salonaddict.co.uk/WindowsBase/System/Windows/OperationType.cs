namespace System.Windows
{
    using MS.Internal.WindowsBase;
    using System;

    [FriendAccessAllowed]
    internal enum OperationType : byte
    {
        AddChild = 1,
        ChangeMutableDefaultValue = 4,
        Inherit = 3,
        RemoveChild = 2,
        Unknown = 0
    }
}

