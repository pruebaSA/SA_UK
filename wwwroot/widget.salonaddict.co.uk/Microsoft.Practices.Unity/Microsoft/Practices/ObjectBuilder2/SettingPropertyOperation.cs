namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Properties;
    using System;

    public class SettingPropertyOperation : PropertyOperation
    {
        public SettingPropertyOperation(Type typeBeingConstructed, string propertyName) : base(typeBeingConstructed, propertyName)
        {
        }

        protected override string GetDescriptionFormat() => 
            Resources.SettingPropertyOperation;
    }
}

