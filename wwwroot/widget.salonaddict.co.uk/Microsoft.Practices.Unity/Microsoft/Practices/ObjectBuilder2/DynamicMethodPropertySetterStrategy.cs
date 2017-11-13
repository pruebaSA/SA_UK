namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Properties;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Reflection.Emit;

    public class DynamicMethodPropertySetterStrategy : BuilderStrategy
    {
        private static readonly MethodInfo setCurrentOperationToResolvingPropertyValue = StaticReflection.GetMethodInfo(Expression.Lambda<Action>(Expression.Call(null, (MethodInfo) methodof(DynamicMethodPropertySetterStrategy.SetCurrentOperationToResolvingPropertyValue), new Expression[] { Expression.Constant(null, typeof(string)), Expression.Constant(null, typeof(IBuilderContext)) }), new ParameterExpression[0]));
        private static readonly MethodInfo setCurrentOperationToSettingProperty = StaticReflection.GetMethodInfo(Expression.Lambda<Action>(Expression.Call(null, (MethodInfo) methodof(DynamicMethodPropertySetterStrategy.SetCurrentOperationToSettingProperty), new Expression[] { Expression.Constant(null, typeof(string)), Expression.Constant(null, typeof(IBuilderContext)) }), new ParameterExpression[0]));

        private static MethodInfo GetValidatedPropertySetter(PropertyInfo property)
        {
            MethodInfo setMethod = property.GetSetMethod();
            if (setMethod == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.PropertyNotSettable, new object[] { property.Name, property.DeclaringType.FullName }));
            }
            return setMethod;
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            IPolicyList list;
            Guard.ArgumentNotNull(context, "context");
            DynamicBuildPlanGenerationContext existing = (DynamicBuildPlanGenerationContext) context.Existing;
            IPropertySelectorPolicy policy = context.Policies.Get<IPropertySelectorPolicy>(context.BuildKey, out list);
            bool flag = false;
            foreach (SelectedProperty property in policy.SelectProperties(context, list))
            {
                flag = true;
                existing.IL.Emit(OpCodes.Ldstr, property.Property.Name);
                existing.EmitLoadContext();
                existing.IL.EmitCall(OpCodes.Call, setCurrentOperationToResolvingPropertyValue, null);
                existing.EmitLoadExisting();
                existing.EmitResolveDependency(property.Property.PropertyType, property.Key);
                existing.IL.Emit(OpCodes.Ldstr, property.Property.Name);
                existing.EmitLoadContext();
                existing.IL.EmitCall(OpCodes.Call, setCurrentOperationToSettingProperty, null);
                existing.IL.EmitCall(OpCodes.Callvirt, GetValidatedPropertySetter(property.Property), null);
            }
            if (flag)
            {
                existing.EmitClearCurrentOperation();
            }
        }

        public static void SetCurrentOperationToResolvingPropertyValue(string propertyName, IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            context.CurrentOperation = new ResolvingPropertyValueOperation(context.BuildKey.Type, propertyName);
        }

        public static void SetCurrentOperationToSettingProperty(string propertyName, IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            context.CurrentOperation = new SettingPropertyOperation(context.BuildKey.Type, propertyName);
        }
    }
}

