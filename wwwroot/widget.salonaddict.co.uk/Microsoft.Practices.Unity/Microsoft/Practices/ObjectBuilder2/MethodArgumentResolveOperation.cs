namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Properties;
    using System;
    using System.Globalization;

    public class MethodArgumentResolveOperation : BuildOperation
    {
        private readonly string methodSignature;
        private readonly string parameterName;

        public MethodArgumentResolveOperation(Type typeBeingConstructed, string methodSignature, string parameterName) : base(typeBeingConstructed)
        {
            this.methodSignature = methodSignature;
            this.parameterName = parameterName;
        }

        public override string ToString() => 
            string.Format(CultureInfo.CurrentCulture, Resources.MethodArgumentResolveOperation, new object[] { this.parameterName, base.TypeBeingConstructed.Name, this.methodSignature });

        public string MethodSignature =>
            this.methodSignature;

        public string ParameterName =>
            this.parameterName;
    }
}

