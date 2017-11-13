namespace System.ServiceModel
{
    using System;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public sealed class XmlSerializerFormatAttribute : Attribute
    {
        private bool isStyleSet;
        private OperationFormatStyle style;
        private bool supportFaults;
        private OperationFormatUse use;

        internal static void ValidateOperationFormatStyle(OperationFormatStyle value)
        {
            if (!OperationFormatStyleHelper.IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
            }
        }

        internal static void ValidateOperationFormatUse(OperationFormatUse value)
        {
            if (!OperationFormatUseHelper.IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
            }
        }

        internal bool IsEncoded
        {
            get => 
                (this.use == OperationFormatUse.Encoded);
            set
            {
                this.use = value ? OperationFormatUse.Encoded : OperationFormatUse.Literal;
            }
        }

        public OperationFormatStyle Style
        {
            get => 
                this.style;
            set
            {
                ValidateOperationFormatStyle(value);
                this.style = value;
                this.isStyleSet = true;
            }
        }

        public bool SupportFaults
        {
            get => 
                this.supportFaults;
            set
            {
                this.supportFaults = value;
            }
        }

        public OperationFormatUse Use
        {
            get => 
                this.use;
            set
            {
                ValidateOperationFormatUse(value);
                this.use = value;
                if (!this.isStyleSet && this.IsEncoded)
                {
                    this.Style = OperationFormatStyle.Rpc;
                }
            }
        }
    }
}

