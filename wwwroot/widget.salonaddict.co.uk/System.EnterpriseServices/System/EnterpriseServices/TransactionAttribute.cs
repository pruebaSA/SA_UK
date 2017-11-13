namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.Runtime.InteropServices;

    [ComVisible(false), AttributeUsage(AttributeTargets.Class, Inherited=true)]
    public sealed class TransactionAttribute : Attribute, IConfigurationAttribute
    {
        private TransactionIsolationLevel _isolation;
        private int _timeout;
        private TransactionOption _value;

        public TransactionAttribute() : this(TransactionOption.Required)
        {
        }

        public TransactionAttribute(TransactionOption val)
        {
            this._value = val;
            this._isolation = TransactionIsolationLevel.Serializable;
            this._timeout = -1;
        }

        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info) => 
            false;

        bool IConfigurationAttribute.Apply(Hashtable info)
        {
            object obj2 = this._value;
            Platform.Assert(Platform.MTS, "TransactionAttribute");
            if (Platform.IsLessThan(Platform.W2K))
            {
                switch (this._value)
                {
                    case TransactionOption.Disabled:
                        obj2 = "NotSupported";
                        break;

                    case TransactionOption.NotSupported:
                        obj2 = "NotSupported";
                        break;

                    case TransactionOption.Supported:
                        obj2 = "Supported";
                        break;

                    case TransactionOption.Required:
                        obj2 = "Required";
                        break;

                    case TransactionOption.RequiresNew:
                        obj2 = "Requires New";
                        break;
                }
            }
            ICatalogObject obj3 = (ICatalogObject) info["Component"];
            obj3.SetValue("Transaction", obj2);
            if (this._isolation != TransactionIsolationLevel.Serializable)
            {
                Platform.Assert(Platform.Whistler, "TransactionAttribute.Isolation");
                obj3.SetValue("TxIsolationLevel", this._isolation);
            }
            if (this._timeout != -1)
            {
                Platform.Assert(Platform.W2K, "TransactionAttribute.Timeout");
                obj3.SetValue("ComponentTransactionTimeout", this._timeout);
                obj3.SetValue("ComponentTransactionTimeoutEnabled", true);
            }
            return true;
        }

        bool IConfigurationAttribute.IsValidTarget(string s) => 
            (s == "Component");

        public TransactionIsolationLevel Isolation
        {
            get => 
                this._isolation;
            set
            {
                this._isolation = value;
            }
        }

        public int Timeout
        {
            get => 
                this._timeout;
            set
            {
                this._timeout = value;
            }
        }

        public TransactionOption Value =>
            this._value;
    }
}

