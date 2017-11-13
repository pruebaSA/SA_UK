namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Properties;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    public class DynamicMethodConstructorStrategy : BuilderStrategy
    {
        private static readonly MethodInfo setCurrentOperationToInvokingConstructor = StaticReflection.GetMethodInfo(Expression.Lambda<Action>(Expression.Call(null, (MethodInfo) methodof(DynamicMethodConstructorStrategy.SetCurrentOperationToInvokingConstructor), new Expression[] { Expression.Constant(null, typeof(string)), Expression.Constant(null, typeof(IBuilderContext)) }), new ParameterExpression[0]));
        private static readonly MethodInfo setCurrentOperationToResolvingParameter = StaticReflection.GetMethodInfo(Expression.Lambda<Action>(Expression.Call(null, (MethodInfo) methodof(DynamicMethodConstructorStrategy.SetCurrentOperationToResolvingParameter), new Expression[] { Expression.Constant(null, typeof(string)), Expression.Constant(null, typeof(string)), Expression.Constant(null, typeof(IBuilderContext)) }), new ParameterExpression[0]));
        private static readonly MethodInfo setExistingInContext = StaticReflection.GetPropertySetMethodInfo<IBuilderContext, object>(ctx => ctx.Existing);
        private static readonly MethodInfo setPerBuildSingleton = StaticReflection.GetMethodInfo(Expression.Lambda<Action>(Expression.Call(null, (MethodInfo) methodof(DynamicMethodConstructorStrategy.SetPerBuildSingleton), new Expression[] { Expression.Constant(null, typeof(IBuilderContext)) }), new ParameterExpression[0]));
        private static readonly MethodInfo throwForAttemptingToConstructInterface = StaticReflection.GetMethodInfo(Expression.Lambda<Action>(Expression.Call(null, (MethodInfo) methodof(DynamicMethodConstructorStrategy.ThrowForAttemptingToConstructInterface), new Expression[] { Expression.Constant(null, typeof(IBuilderContext)) }), new ParameterExpression[0]));
        private static readonly MethodInfo throwForNullExistingObject = StaticReflection.GetMethodInfo(Expression.Lambda<Action>(Expression.Call(null, (MethodInfo) methodof(DynamicMethodConstructorStrategy.ThrowForNullExistingObject), new Expression[] { Expression.Constant(null, typeof(IBuilderContext)) }), new ParameterExpression[0]));
        private static readonly MethodInfo throwForNullExistingObjectWithInvalidConstructor = StaticReflection.GetMethodInfo(Expression.Lambda<Action>(Expression.Call(null, (MethodInfo) methodof(DynamicMethodConstructorStrategy.ThrowForNullExistingObjectWithInvalidConstructor), new Expression[] { Expression.Constant(null, typeof(IBuilderContext)), Expression.Constant(null, typeof(string)) }), new ParameterExpression[0]));

        private static string CreateSignatureString(ConstructorInfo ctor)
        {
            string fullName = ctor.DeclaringType.FullName;
            ParameterInfo[] parameters = ctor.GetParameters();
            string[] strArray = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                strArray[i] = string.Format(CultureInfo.CurrentCulture, "{0} {1}", new object[] { parameters[i].ParameterType.FullName, parameters[i].Name });
            }
            return string.Format(CultureInfo.CurrentCulture, "{0}({1})", new object[] { fullName, string.Join(", ", strArray) });
        }

        private static void GuardTypeIsNonPrimitive(IBuilderContext context, SelectedConstructor selectedConstructor)
        {
            Type type = context.BuildKey.Type;
            if (!type.IsInterface && ((type == typeof(string)) || (selectedConstructor == null)))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.TypeIsNotConstructable, new object[] { type.Name }));
            }
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            IPolicyList list;
            Guard.ArgumentNotNull(context, "context");
            DynamicBuildPlanGenerationContext existing = (DynamicBuildPlanGenerationContext) context.Existing;
            SelectedConstructor selectedConstructor = context.Policies.Get<IConstructorSelectorPolicy>(context.BuildKey, out list).SelectConstructor(context, list);
            GuardTypeIsNonPrimitive(context, selectedConstructor);
            Label label = existing.IL.DefineLabel();
            if (!existing.TypeToBuild.IsValueType)
            {
                existing.EmitLoadExisting();
                existing.IL.Emit(OpCodes.Brtrue, label);
            }
            existing.EmitLoadContext();
            existing.IL.EmitCall(OpCodes.Call, throwForAttemptingToConstructInterface, null);
            if (selectedConstructor != null)
            {
                string str = CreateSignatureString(selectedConstructor.Constructor);
                ParameterInfo[] parameters = selectedConstructor.Constructor.GetParameters();
                if (!parameters.Any<ParameterInfo>(pi => pi.ParameterType.IsByRef))
                {
                    int index = 0;
                    foreach (string str2 in selectedConstructor.GetParameterKeys())
                    {
                        existing.IL.Emit(OpCodes.Ldstr, parameters[index].Name);
                        existing.IL.Emit(OpCodes.Ldstr, str);
                        existing.EmitLoadContext();
                        existing.IL.EmitCall(OpCodes.Call, setCurrentOperationToResolvingParameter, null);
                        existing.EmitResolveDependency(parameters[index].ParameterType, str2);
                        index++;
                    }
                    existing.IL.Emit(OpCodes.Ldstr, str);
                    existing.EmitLoadContext();
                    existing.IL.EmitCall(OpCodes.Call, setCurrentOperationToInvokingConstructor, null);
                    existing.IL.Emit(OpCodes.Newobj, selectedConstructor.Constructor);
                    existing.EmitStoreExisting();
                    existing.EmitClearCurrentOperation();
                    existing.EmitLoadContext();
                    existing.EmitLoadExisting();
                    if (existing.TypeToBuild.IsValueType)
                    {
                        existing.IL.Emit(OpCodes.Box, existing.TypeToBuild);
                    }
                    existing.IL.EmitCall(OpCodes.Callvirt, setExistingInContext, null);
                    existing.EmitLoadContext();
                    existing.IL.EmitCall(OpCodes.Call, setPerBuildSingleton, null);
                }
                else
                {
                    existing.EmitLoadContext();
                    existing.IL.Emit(OpCodes.Ldstr, str);
                    existing.IL.EmitCall(OpCodes.Call, throwForNullExistingObjectWithInvalidConstructor, null);
                }
            }
            else
            {
                existing.EmitLoadContext();
                existing.IL.EmitCall(OpCodes.Call, throwForNullExistingObject, null);
            }
            existing.IL.MarkLabel(label);
        }

        public static void SetCurrentOperationToInvokingConstructor(string constructorSignature, IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            context.CurrentOperation = new InvokingConstructorOperation(context.BuildKey.Type, constructorSignature);
        }

        public static void SetCurrentOperationToResolvingParameter(string parameterName, string constructorSignature, IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            context.CurrentOperation = new ConstructorArgumentResolveOperation(context.BuildKey.Type, constructorSignature, parameterName);
        }

        public static void SetPerBuildSingleton(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            if (context.Policies.Get<ILifetimePolicy>(context.BuildKey) is PerResolveLifetimeManager)
            {
                PerResolveLifetimeManager policy = new PerResolveLifetimeManager(context.Existing);
                context.Policies.Set<ILifetimePolicy>(policy, context.BuildKey);
            }
        }

        public static void ThrowForAttemptingToConstructInterface(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            if (context.BuildKey.Type.IsInterface)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CannotConstructInterface, new object[] { context.BuildKey.Type, context.BuildKey }));
            }
        }

        public static void ThrowForNullExistingObject(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.NoConstructorFound, new object[] { context.BuildKey.Type.Name }));
        }

        public static void ThrowForNullExistingObjectWithInvalidConstructor(IBuilderContext context, string signature)
        {
            Guard.ArgumentNotNull(context, "context");
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.SelectedConstructorHasRefParameters, new object[] { context.BuildKey.Type.Name, signature }));
        }
    }
}

