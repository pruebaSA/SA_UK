namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Properties;
    using System;
    using System.Globalization;

    public class InvokingMethodOperation : BuildOperation
    {
        private readonly string methodSignature;

        public InvokingMethodOperation(Type typeBeingConstructed, string methodSignature) : base(typeBeingConstructed)
        {
            this.methodSignature = methodSignature;
        }

        public override string ToString() => 
            string.Format(CultureInfo.CurrentCulture, Resources.InvokingMethodOperation, new object[] { base.TypeBeingConstructed.Name, this.methodSignature });

        public string MethodSignature =>
            this.methodSignature;
    }
}

