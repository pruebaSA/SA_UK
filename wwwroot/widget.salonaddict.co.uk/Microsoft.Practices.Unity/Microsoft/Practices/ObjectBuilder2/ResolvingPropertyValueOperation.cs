namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Properties;
    using System;

    public class ResolvingPropertyValueOperation : PropertyOperation
    {
        public ResolvingPropertyValueOperation(Type typeBeingConstructed, string propertyName) : base(typeBeingConstructed, propertyName)
        {
        }

        protected override string GetDescriptionFormat() => 
            Resources.ResolvingPropertyValueOperation;
    }
}

