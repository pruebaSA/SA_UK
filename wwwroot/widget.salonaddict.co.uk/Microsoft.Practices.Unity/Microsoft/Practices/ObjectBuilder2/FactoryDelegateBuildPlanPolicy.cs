namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity;
    using System;

    internal class FactoryDelegateBuildPlanPolicy : IBuildPlanPolicy, IBuilderPolicy
    {
        private readonly Func<IUnityContainer, Type, string, object> factory;

        public FactoryDelegateBuildPlanPolicy(Func<IUnityContainer, Type, string, object> factory)
        {
            this.factory = factory;
        }

        public void BuildUp(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                IUnityContainer container = context.NewBuildUp<IUnityContainer>();
                context.Existing = this.factory(container, context.BuildKey.Type, context.BuildKey.Name);
                DynamicMethodConstructorStrategy.SetPerBuildSingleton(context);
            }
        }
    }
}

