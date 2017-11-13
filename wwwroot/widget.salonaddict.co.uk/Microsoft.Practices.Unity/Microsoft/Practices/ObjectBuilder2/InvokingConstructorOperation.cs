namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Properties;
    using System;
    using System.Globalization;

    public class InvokingConstructorOperation : BuildOperation
    {
        private readonly string constructorSignature;

        public InvokingConstructorOperation(Type typeBeingConstructed, string constructorSignature) : base(typeBeingConstructed)
        {
            this.constructorSignature = constructorSignature;
        }

        public override string ToString() => 
            string.Format(CultureInfo.CurrentCulture, Resources.InvokingConstructorOperation, new object[] { this.constructorSignature });

        public string ConstructorSignature =>
            this.constructorSignature;
    }
}

