namespace System.Threading
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;

    [Serializable]
    internal sealed class DomainCompressedStack
    {
        private bool m_bHaltConstruction;
        private PermissionListSet m_pls;

        private static DomainCompressedStack CreateManagedObject(IntPtr unmanagedDCS)
        {
            DomainCompressedStack stack = new DomainCompressedStack();
            stack.m_pls = PermissionListSet.CreateCompressedState(unmanagedDCS, out stack.m_bHaltConstruction);
            return stack;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int GetDescCount(IntPtr dcs);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool GetDescriptorInfo(IntPtr dcs, int index, out PermissionSet granted, out PermissionSet refused, out Assembly assembly, out FrameSecurityDescriptor fsd);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void GetDomainPermissionSets(IntPtr dcs, out PermissionSet granted, out PermissionSet refused);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool IgnoreDomain(IntPtr dcs);

        internal bool ConstructionHalted =>
            this.m_bHaltConstruction;

        internal PermissionListSet PLS =>
            this.m_pls;
    }
}

