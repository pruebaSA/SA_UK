namespace MS.Internal.Security.RightsManagement
{
    using System;
    using System.Security;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal class RightNameExpirationInfoPair
    {
        private string _rightName;
        private DateTime _validFrom;
        private DateTime _validUntil;

        internal RightNameExpirationInfoPair(string rightName, DateTime validFrom, DateTime validUntil)
        {
            this._rightName = rightName;
            this._validFrom = validFrom;
            this._validUntil = validUntil;
        }

        internal string RightName =>
            this._rightName;

        internal DateTime ValidFrom =>
            this._validFrom;

        internal DateTime ValidUntil =>
            this._validUntil;
    }
}

