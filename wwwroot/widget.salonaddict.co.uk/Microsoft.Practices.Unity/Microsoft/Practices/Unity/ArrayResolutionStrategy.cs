namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class ArrayResolutionStrategy : BuilderStrategy
    {
        private static readonly MethodInfo genericResolveArrayMethod = typeof(ArrayResolutionStrategy).GetMethod("ResolveArray", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override void PreBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            Type type = context.BuildKey.Type;
            if (type.IsArray && (type.GetArrayRank() == 1))
            {
                Type elementType = type.GetElementType();
                MethodInfo method = genericResolveArrayMethod.MakeGenericMethod(new Type[] { elementType });
                ArrayResolver resolver = (ArrayResolver) Delegate.CreateDelegate(typeof(ArrayResolver), method);
                context.Existing = resolver(context);
                context.BuildComplete = true;
            }
        }

        private static object ResolveArray<T>(IBuilderContext context)
        {
            List<T> list = new List<T>(context.NewBuildUp<IUnityContainer>().ResolveAll<T>(new ResolverOverride[0]));
            return list.ToArray();
        }

        private delegate object ArrayResolver(IBuilderContext context);
    }
}

