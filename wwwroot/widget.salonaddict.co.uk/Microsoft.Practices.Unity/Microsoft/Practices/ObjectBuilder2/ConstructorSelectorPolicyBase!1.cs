namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Properties;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public abstract class ConstructorSelectorPolicyBase<TInjectionConstructorMarkerAttribute> : IConstructorSelectorPolicy, IBuilderPolicy where TInjectionConstructorMarkerAttribute: Attribute
    {
        protected ConstructorSelectorPolicyBase()
        {
        }

        protected abstract IDependencyResolverPolicy CreateResolver(ParameterInfo parameter);
        private SelectedConstructor CreateSelectedConstructor(IBuilderContext context, IPolicyList resolverPolicyDestination, ConstructorInfo ctor)
        {
            SelectedConstructor constructor = new SelectedConstructor(ctor);
            foreach (ParameterInfo info in ctor.GetParameters())
            {
                string buildKey = Guid.NewGuid().ToString();
                IDependencyResolverPolicy policy = this.CreateResolver(info);
                resolverPolicyDestination.Set<IDependencyResolverPolicy>(policy, buildKey);
                DependencyResolverTrackerPolicy.TrackKey(resolverPolicyDestination, context.BuildKey, buildKey);
                constructor.AddParameterKey(buildKey);
            }
            return constructor;
        }

        private static ConstructorInfo FindInjectionConstructor(Type typeToConstruct)
        {
            ConstructorInfo[] infoArray = (from ctor in typeToConstruct.GetConstructors()
                where ctor.IsDefined(typeof(TInjectionConstructorMarkerAttribute), true)
                select ctor).ToArray<ConstructorInfo>();
            switch (infoArray.Length)
            {
                case 0:
                    return null;

                case 1:
                    return infoArray[0];
            }
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.MultipleInjectionConstructors, new object[] { typeToConstruct.Name }));
        }

        private static ConstructorInfo FindLongestConstructor(Type typeToConstruct)
        {
            ConstructorInfo[] constructors = typeToConstruct.GetConstructors();
            Array.Sort<ConstructorInfo>(constructors, new ConstructorLengthComparer<TInjectionConstructorMarkerAttribute>());
            switch (constructors.Length)
            {
                case 0:
                    return null;

                case 1:
                    return constructors[0];
            }
            int length = constructors[0].GetParameters().Length;
            if (constructors[1].GetParameters().Length == length)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.AmbiguousInjectionConstructor, new object[] { typeToConstruct.Name, length }));
            }
            return constructors[0];
        }

        public SelectedConstructor SelectConstructor(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            Type typeToConstruct = context.BuildKey.Type;
            ConstructorInfo ctor = ConstructorSelectorPolicyBase<TInjectionConstructorMarkerAttribute>.FindInjectionConstructor(typeToConstruct) ?? ConstructorSelectorPolicyBase<TInjectionConstructorMarkerAttribute>.FindLongestConstructor(typeToConstruct);
            if (ctor != null)
            {
                return this.CreateSelectedConstructor(context, resolverPolicyDestination, ctor);
            }
            return null;
        }

        private class ConstructorLengthComparer : IComparer<ConstructorInfo>
        {
            public int Compare(ConstructorInfo x, ConstructorInfo y) => 
                (y.GetParameters().Length - x.GetParameters().Length);
        }
    }
}

