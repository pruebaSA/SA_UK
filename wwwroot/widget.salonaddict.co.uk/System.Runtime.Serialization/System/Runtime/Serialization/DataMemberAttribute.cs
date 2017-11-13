namespace System.Runtime.Serialization
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited=false, AllowMultiple=false)]
    public sealed class DataMemberAttribute : Attribute
    {
        private bool emitDefaultValue = true;
        private bool isNameSetExplicit;
        private bool isRequired;
        private string name;
        private int order = -1;

        public bool EmitDefaultValue
        {
            get => 
                this.emitDefaultValue;
            set
            {
                this.emitDefaultValue = value;
            }
        }

        internal bool IsNameSetExplicit =>
            this.isNameSetExplicit;

        public bool IsRequired
        {
            get => 
                this.isRequired;
            set
            {
                this.isRequired = value;
            }
        }

        public string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
                this.isNameSetExplicit = true;
            }
        }

        public int Order
        {
            get => 
                this.order;
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("OrderCannotBeNegative")));
                }
                this.order = value;
            }
        }
    }
}

