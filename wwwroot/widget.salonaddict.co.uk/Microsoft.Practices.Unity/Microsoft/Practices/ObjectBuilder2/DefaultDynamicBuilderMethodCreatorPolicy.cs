namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Reflection.Emit;

    public class DefaultDynamicBuilderMethodCreatorPolicy : IDynamicBuilderMethodCreatorPolicy, IBuilderPolicy
    {
        public DynamicMethod CreateBuilderMethod(Type typeToBuild, string methodName)
        {
            Guard.ArgumentNotNull(typeToBuild, "typeToBuild");
            Guard.ArgumentNotNullOrEmpty(methodName, "methodName");
            return new DynamicMethod(methodName, typeof(void), new Type[] { typeof(IBuilderContext) }, true);
        }
    }
}

