namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Properties;
    using System;
    using System.Globalization;

    public class ConstructorArgumentResolveOperation : BuildOperation
    {
        private readonly string constructorSignature;
        private readonly string parameterName;

        public ConstructorArgumentResolveOperation(Type typeBeingConstructed, string constructorSignature, string parameterName) : base(typeBeingConstructed)
        {
            this.constructorSignature = constructorSignature;
            this.parameterName = parameterName;
        }

        public override string ToString() => 
            string.Format(CultureInfo.CurrentCulture, Resources.ConstructorArgumentResolveOperation, new object[] { this.parameterName, this.constructorSignature });

        public string ConstructorSignature =>
            this.constructorSignature;

        public string ParameterName =>
            this.parameterName;
    }
}

