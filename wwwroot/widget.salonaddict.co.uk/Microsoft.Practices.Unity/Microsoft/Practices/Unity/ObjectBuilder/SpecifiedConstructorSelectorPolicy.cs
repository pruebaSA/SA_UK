namespace Microsoft.Practices.Unity.ObjectBuilder
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Reflection;

    public class SpecifiedConstructorSelectorPolicy : IConstructorSelectorPolicy, IBuilderPolicy
    {
        private readonly ConstructorInfo ctor;
        private readonly MethodReflectionHelper ctorReflector;
        private readonly InjectionParameterValue[] parameterValues;

        public SpecifiedConstructorSelectorPolicy(ConstructorInfo ctor, InjectionParameterValue[] parameterValues)
        {
            this.ctor = ctor;
            this.ctorReflector = new MethodReflectionHelper(ctor);
            this.parameterValues = parameterValues;
        }

        public SelectedConstructor SelectConstructor(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            SelectedConstructor constructor;
            Type typeToBuild = context.BuildKey.Type;
            ReflectionHelper helper = new ReflectionHelper(this.ctor.DeclaringType);
            if (!(this.ctorReflector.MethodHasOpenGenericParameters || helper.IsOpenGeneric))
            {
                constructor = new SelectedConstructor(this.ctor);
            }
            else
            {
                Type[] closedParameterTypes = this.ctorReflector.GetClosedParameterTypes(typeToBuild.GetGenericArguments());
                constructor = new SelectedConstructor(typeToBuild.GetConstructor(closedParameterTypes));
            }
            SpecifiedMemberSelectorHelper.AddParameterResolvers(typeToBuild, resolverPolicyDestination, this.parameterValues, constructor);
            return constructor;
        }
    }
}

