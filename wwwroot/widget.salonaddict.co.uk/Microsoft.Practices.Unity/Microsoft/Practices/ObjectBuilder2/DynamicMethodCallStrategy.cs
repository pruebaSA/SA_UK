namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Properties;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    public class DynamicMethodCallStrategy : BuilderStrategy
    {
        private static readonly MethodInfo setCurrentOperationToInvokingMethod = StaticReflection.GetMethodInfo(Expression.Lambda<Action>(Expression.Call(null, (MethodInfo) methodof(DynamicMethodCallStrategy.SetCurrentOperationToInvokingMethod), new Expression[] { Expression.Constant(null, typeof(string)), Expression.Constant(null, typeof(IBuilderContext)) }), new ParameterExpression[0]));
        private static readonly MethodInfo setCurrentOperationToResolvingParameter = StaticReflection.GetMethodInfo(Expression.Lambda<Action>(Expression.Call(null, (MethodInfo) methodof(DynamicMethodCallStrategy.SetCurrentOperationToResolvingParameter), new Expression[] { Expression.Constant(null, typeof(string)), Expression.Constant(null, typeof(string)), Expression.Constant(null, typeof(IBuilderContext)) }), new ParameterExpression[0]));

        private static string GetMethodSignature(MethodBase method)
        {
            string name = method.Name;
            ParameterInfo[] parameters = method.GetParameters();
            string[] strArray = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                strArray[i] = parameters[i].ParameterType.FullName + " " + parameters[i].Name;
            }
            return string.Format(CultureInfo.CurrentCulture, "{0}({1})", new object[] { name, string.Join(", ", strArray) });
        }

        private static void GuardMethodHasNoOutParams(MethodInfo method)
        {
            if (method.GetParameters().Any<ParameterInfo>(param => param.IsOut))
            {
                ThrowIllegalInjectionMethod(Resources.CannotInjectMethodWithOutParam, method);
            }
        }

        private static void GuardMethodHasNoRefParams(MethodInfo method)
        {
            if (method.GetParameters().Any<ParameterInfo>(param => param.ParameterType.IsByRef))
            {
                ThrowIllegalInjectionMethod(Resources.CannotInjectMethodWithOutParam, method);
            }
        }

        private static void GuardMethodIsNotOpenGeneric(MethodInfo method)
        {
            if (method.IsGenericMethodDefinition)
            {
                ThrowIllegalInjectionMethod(Resources.CannotInjectOpenGenericMethod, method);
            }
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            IPolicyList list;
            DynamicBuildPlanGenerationContext existing = (DynamicBuildPlanGenerationContext) context.Existing;
            IMethodSelectorPolicy policy = context.Policies.Get<IMethodSelectorPolicy>(context.BuildKey, out list);
            bool flag = false;
            foreach (SelectedMethod method in policy.SelectMethods(context, list))
            {
                flag = true;
                string methodSignature = GetMethodSignature(method.Method);
                GuardMethodIsNotOpenGeneric(method.Method);
                GuardMethodHasNoOutParams(method.Method);
                GuardMethodHasNoRefParams(method.Method);
                ParameterInfo[] parameters = method.Method.GetParameters();
                existing.EmitLoadExisting();
                int index = 0;
                foreach (string str2 in method.GetParameterKeys())
                {
                    existing.IL.Emit(OpCodes.Ldstr, parameters[index].Name);
                    existing.IL.Emit(OpCodes.Ldstr, methodSignature);
                    existing.EmitLoadContext();
                    existing.IL.EmitCall(OpCodes.Call, setCurrentOperationToResolvingParameter, null);
                    existing.EmitResolveDependency(parameters[index].ParameterType, str2);
                    index++;
                }
                existing.IL.Emit(OpCodes.Ldstr, methodSignature);
                existing.EmitLoadContext();
                existing.IL.EmitCall(OpCodes.Call, setCurrentOperationToInvokingMethod, null);
                existing.IL.EmitCall(OpCodes.Callvirt, method.Method, null);
                if (method.Method.ReturnType != typeof(void))
                {
                    existing.IL.Emit(OpCodes.Pop);
                }
            }
            if (flag)
            {
                existing.EmitClearCurrentOperation();
            }
        }

        public static void SetCurrentOperationToInvokingMethod(string methodSignature, IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            context.CurrentOperation = new InvokingMethodOperation(context.BuildKey.Type, methodSignature);
        }

        public static void SetCurrentOperationToResolvingParameter(string parameterName, string methodSignature, IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            context.CurrentOperation = new MethodArgumentResolveOperation(context.BuildKey.Type, methodSignature, parameterName);
        }

        private static void ThrowIllegalInjectionMethod(string format, MethodInfo method)
        {
            throw new IllegalInjectionMethodException(string.Format(CultureInfo.CurrentCulture, format, new object[] { method.DeclaringType.Name, method.Name }));
        }
    }
}

