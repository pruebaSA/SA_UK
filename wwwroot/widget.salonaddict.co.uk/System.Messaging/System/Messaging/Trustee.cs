namespace System.Messaging
{
    using System;
    using System.ComponentModel;

    public class Trustee
    {
        private string name;
        private string systemName;
        private System.Messaging.TrusteeType trusteeType;

        public Trustee()
        {
        }

        public Trustee(string name) : this(name, null)
        {
        }

        public Trustee(string name, string systemName) : this(name, systemName, System.Messaging.TrusteeType.Unknown)
        {
        }

        public Trustee(string name, string systemName, System.Messaging.TrusteeType trusteeType)
        {
            this.Name = name;
            this.SystemName = systemName;
            this.TrusteeType = trusteeType;
        }

        public string Name
        {
            get => 
                this.name;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.name = value;
            }
        }

        public string SystemName
        {
            get => 
                this.systemName;
            set
            {
                this.systemName = value;
            }
        }

        public System.Messaging.TrusteeType TrusteeType
        {
            get => 
                this.trusteeType;
            set
            {
                if (!ValidationUtility.ValidateTrusteeType(value))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(System.Messaging.TrusteeType));
                }
                this.trusteeType = value;
            }
        }
    }
}

