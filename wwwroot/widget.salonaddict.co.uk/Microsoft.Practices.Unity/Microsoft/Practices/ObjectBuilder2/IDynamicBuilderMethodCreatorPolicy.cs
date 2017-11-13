namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Reflection.Emit;

    public interface IDynamicBuilderMethodCreatorPolicy : IBuilderPolicy
    {
        DynamicMethod CreateBuilderMethod(Type typeToBuild, string methodName);
    }
}

