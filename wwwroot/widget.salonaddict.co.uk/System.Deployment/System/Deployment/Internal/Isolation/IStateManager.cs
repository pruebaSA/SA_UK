﻿namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("07662534-750b-4ed5-9cfb-1c5bc5acfd07"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IStateManager
    {
        void PrepareApplicationState([In] UIntPtr Inputs, ref UIntPtr Outputs);
        void SetApplicationRunningState([In] uint Flags, [In] System.Deployment.Internal.Isolation.IActContext Context, [In] uint RunningState, out uint Disposition);
        void GetApplicationStateFilesystemLocation([In] uint Flags, [In] System.Deployment.Internal.Isolation.IDefinitionAppId Appidentity, [In] System.Deployment.Internal.Isolation.IDefinitionIdentity ComponentIdentity, [In] UIntPtr Coordinates, [MarshalAs(UnmanagedType.LPWStr)] out string Path);
        void Scavenge([In] uint Flags, out uint Disposition);
    }
}

