namespace System.Data.Linq
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ModifiedMemberInfo
    {
        private MemberInfo member;
        private object current;
        private object original;
        internal ModifiedMemberInfo(MemberInfo member, object current, object original)
        {
            this.member = member;
            this.current = current;
            this.original = original;
        }

        public MemberInfo Member =>
            this.member;
        public object CurrentValue =>
            this.current;
        public object OriginalValue =>
            this.original;
    }
}

