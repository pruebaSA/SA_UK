namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [StructLayout(LayoutKind.Sequential), FriendAccessAllowed]
    internal struct SecurityCriticalData<T>
    {
        [SecurityCritical]
        private T _value;
        [SecurityCritical, SecurityTreatAsSafe]
        internal SecurityCriticalData(T value)
        {
            this._value = value;
        }

        internal T Value =>
            this._value;
    }
}

