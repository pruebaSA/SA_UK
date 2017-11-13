namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Security;

    [FriendAccessAllowed]
    internal class SecurityCriticalDataClass<T>
    {
        [SecurityCritical]
        private T _value;

        [SecurityCritical, SecurityTreatAsSafe]
        internal SecurityCriticalDataClass(T value)
        {
            this._value = value;
        }

        internal T Value =>
            this._value;
    }
}

