namespace MS.Internal.Security.RightsManagement
{
    using System;
    using System.Security;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal class RevocationPoint
    {
        private SystemTime _frequency;
        private string _id;
        private string _idType;
        private string _name;
        private string _publicKey;
        private Uri _url;

        internal SystemTime Frequency
        {
            get => 
                this._frequency;
            set
            {
                this._frequency = value;
            }
        }

        internal string Id
        {
            get => 
                this._id;
            set
            {
                this._id = value;
            }
        }

        internal string IdType
        {
            get => 
                this._idType;
            set
            {
                this._idType = value;
            }
        }

        internal string Name
        {
            get => 
                this._name;
            set
            {
                this._name = value;
            }
        }

        internal string PublicKey
        {
            get => 
                this._publicKey;
            set
            {
                this._publicKey = value;
            }
        }

        internal Uri Url
        {
            get => 
                this._url;
            set
            {
                this._url = value;
            }
        }
    }
}

