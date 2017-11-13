namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class DeferredResolveBuildPlanPolicy : IBuildPlanPolicy, IBuilderPolicy
    {
        public void BuildUp(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                Delegate delegate2;
                IUnityContainer currentContainer = context.NewBuildUp<IUnityContainer>();
                Type typeToBuild = GetTypeToBuild(context.BuildKey.Type);
                string name = context.BuildKey.Name;
                if (IsResolvingIEnumerable(typeToBuild))
                {
                    delegate2 = CreateResolveAllResolver(currentContainer, typeToBuild);
                }
                else
                {
                    delegate2 = CreateResolver(currentContainer, typeToBuild, name);
                }
                context.Existing = delegate2;
                DynamicMethodConstructorStrategy.SetPerBuildSingleton(context);
            }
        }

        private static Delegate CreateResolveAllResolver(IUnityContainer currentContainer, Type enumerableType)
        {
            Type typeToBuild = GetTypeToBuild(enumerableType);
            Type type = typeof(ResolveAllTrampoline).MakeGenericType(new Type[] { typeToBuild });
            Type type3 = typeof(Func<>).MakeGenericType(new Type[] { enumerableType });
            MethodInfo method = type.GetMethod("ResolveAll");
            object firstArgument = Activator.CreateInstance(type, new object[] { currentContainer });
            return Delegate.CreateDelegate(type3, firstArgument, method);
        }

        private static Delegate CreateResolver(IUnityContainer currentContainer, Type typeToBuild, string nameToBuild)
        {
            Type type = typeof(ResolveTrampoline).MakeGenericType(new Type[] { typeToBuild });
            Type type2 = typeof(Func<>).MakeGenericType(new Type[] { typeToBuild });
            MethodInfo method = type.GetMethod("Resolve");
            object firstArgument = Activator.CreateInstance(type, new object[] { currentContainer, nameToBuild });
            return Delegate.CreateDelegate(type2, firstArgument, method);
        }

        private static Type GetTypeToBuild(Type t) => 
            t.GetGenericArguments()[0];

        private static bool IsResolvingIEnumerable(Type typeToBuild) => 
            (typeToBuild.IsGenericType && (typeToBuild.GetGenericTypeDefinition() == typeof(IEnumerable<>)));

        private class ResolveAllTrampoline<TItem>
        {
            private readonly IUnityContainer container;

            public ResolveAllTrampoline(IUnityContainer container)
            {
                this.container = container;
            }

            public IEnumerable<TItem> ResolveAll() => 
                this.container.ResolveAll<TItem>(new ResolverOverride[0]);
        }

        private class ResolveTrampoline<TItem>
        {
            private readonly IUnityContainer container;
            private readonly string name;

            public ResolveTrampoline(IUnityContainer container, string name)
            {
                this.container = container;
                this.name = name;
            }

            public TItem Resolve() => 
                this.container.Resolve<TItem>(this.name, new ResolverOverride[0]);
        }
    }
}

