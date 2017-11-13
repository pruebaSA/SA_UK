namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public class DynamicBuildPlanGenerationContext
    {
        private readonly DynamicMethod buildMethod;
        private static readonly MethodInfo ClearCurrentOperation = StaticReflection.GetMethodInfo(Expression.Lambda<Action>(Expression.Call(null, (MethodInfo) methodof(DynamicBuildPlanGenerationContext.DoClearCurrentOperation), new Expression[] { Expression.Constant(null, typeof(IBuilderContext)) }), new ParameterExpression[0]));
        private LocalBuilder existingObjectLocal;
        private static readonly MethodInfo GetBuildKey = StaticReflection.GetPropertyGetMethodInfo<IBuilderContext, NamedTypeBuildKey>(ctx => ctx.BuildKey);
        private static readonly MethodInfo GetExisting = StaticReflection.GetPropertyGetMethodInfo<IBuilderContext, object>(ctx => ctx.Existing);
        private static readonly MethodInfo GetResolverMethod = StaticReflection.GetMethodInfo(Expression.Lambda<Action>(Expression.Call(null, (MethodInfo) methodof(DynamicBuildPlanGenerationContext.GetResolver), new Expression[] { Expression.Constant(null, typeof(IBuilderContext)), Expression.Constant(null, typeof(Type)), Expression.Constant(null, typeof(string)) }), new ParameterExpression[0]));
        private static readonly MethodInfo GetTypeFromHandle = StaticReflection.GetMethodInfo(Expression.Lambda<Action>(Expression.Call(null, (MethodInfo) methodof(Type.GetTypeFromHandle), new Expression[] { Expression.Property(Expression.Constant(typeof(Type), typeof(Type)), (MethodInfo) methodof(Type.get_TypeHandle)) }), new ParameterExpression[0]));
        private readonly ILGenerator il;
        private static readonly MethodInfo ResolveDependency = StaticReflection.GetMethodInfo<IDependencyResolverPolicy>((Expression<Action<IDependencyResolverPolicy>>) (r => r.Resolve(null)));
        private static readonly MethodInfo SetExisting = StaticReflection.GetPropertySetMethodInfo<IBuilderContext, object>(ctx => ctx.Existing);
        private readonly Type typeToBuild;

        public DynamicBuildPlanGenerationContext(Type typeToBuild, IDynamicBuilderMethodCreatorPolicy builderMethodCreator)
        {
            Guard.ArgumentNotNull(typeToBuild, "typeToBuild");
            Guard.ArgumentNotNull(builderMethodCreator, "builderMethodCreator");
            this.typeToBuild = typeToBuild;
            this.buildMethod = builderMethodCreator.CreateBuilderMethod(typeToBuild, this.BuildMethodName());
            this.il = this.buildMethod.GetILGenerator();
            this.CreatePreamble();
        }

        private string BuildMethodName() => 
            ("BuildUp_" + this.typeToBuild.FullName);

        private void CreatePostamble()
        {
            this.EmitLoadContext();
            this.il.Emit(OpCodes.Ldloc, this.existingObjectLocal);
            if (this.typeToBuild.IsValueType)
            {
                this.il.Emit(OpCodes.Box, this.typeToBuild);
            }
            this.il.EmitCall(OpCodes.Callvirt, SetExisting, null);
            this.il.Emit(OpCodes.Ret);
        }

        private void CreatePreamble()
        {
            this.existingObjectLocal = this.il.DeclareLocal(this.typeToBuild);
            this.EmitLoadContext();
            this.il.EmitCall(OpCodes.Callvirt, GetExisting, null);
            if (!this.typeToBuild.IsValueType)
            {
                this.il.Emit(OpCodes.Castclass, this.typeToBuild);
                this.EmitStoreExisting();
            }
            else
            {
                Label label = this.il.DefineLabel();
                Label label2 = this.il.DefineLabel();
                this.il.Emit(OpCodes.Dup);
                this.il.Emit(OpCodes.Ldnull);
                this.il.Emit(OpCodes.Beq_S, label);
                this.il.Emit(OpCodes.Unbox_Any, this.typeToBuild);
                this.EmitStoreExisting();
                this.il.Emit(OpCodes.Br_S, label2);
                this.il.MarkLabel(label);
                this.il.Emit(OpCodes.Pop);
                this.il.MarkLabel(label2);
            }
        }

        public static void DoClearCurrentOperation(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            context.CurrentOperation = null;
        }

        public void EmitCastOrUnbox(Type targetType)
        {
            Guard.ArgumentNotNull(targetType, "targetType");
            if (targetType.IsValueType)
            {
                this.IL.Emit(OpCodes.Unbox_Any, targetType);
            }
            else
            {
                this.IL.Emit(OpCodes.Castclass, targetType);
            }
        }

        public void EmitClearCurrentOperation()
        {
            this.EmitLoadContext();
            this.IL.EmitCall(OpCodes.Call, ClearCurrentOperation, null);
        }

        public void EmitLoadBuildKey()
        {
            this.EmitLoadContext();
            this.IL.EmitCall(OpCodes.Callvirt, GetBuildKey, null);
        }

        public void EmitLoadContext()
        {
            this.IL.Emit(OpCodes.Ldarg_0);
        }

        public void EmitLoadExisting()
        {
            this.IL.Emit(OpCodes.Ldloc, this.existingObjectLocal);
        }

        public void EmitLoadTypeOnStack(Type t)
        {
            this.IL.Emit(OpCodes.Ldtoken, t);
            this.IL.EmitCall(OpCodes.Call, GetTypeFromHandle, null);
        }

        public void EmitResolveDependency(Type dependencyType, string key)
        {
            Guard.ArgumentNotNull(dependencyType, "dependencyType");
            this.EmitLoadContext();
            this.EmitLoadTypeOnStack(dependencyType);
            this.IL.Emit(OpCodes.Ldstr, key);
            this.IL.EmitCall(OpCodes.Call, GetResolverMethod, null);
            this.EmitLoadContext();
            this.IL.EmitCall(OpCodes.Callvirt, ResolveDependency, null);
            this.EmitCastOrUnbox(dependencyType);
        }

        public void EmitStoreExisting()
        {
            this.IL.Emit(OpCodes.Stloc, this.existingObjectLocal);
        }

        internal DynamicBuildPlanMethod GetBuildMethod()
        {
            this.CreatePostamble();
            return (DynamicBuildPlanMethod) this.buildMethod.CreateDelegate(typeof(DynamicBuildPlanMethod));
        }

        public MethodInfo GetMethodInfo<TImplementer>(string name, params Type[] argumentTypes) => 
            typeof(TImplementer).GetMethod(name, argumentTypes);

        public MethodInfo GetPropertyGetter<TImplementer, TProperty>(string name) => 
            typeof(TImplementer).GetProperty(name, typeof(TProperty)).GetGetMethod();

        public static IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType, string resolverKey) => 
            (context.GetOverriddenResolver(dependencyType) ?? context.Policies.Get<IDependencyResolverPolicy>(resolverKey));

        public ILGenerator IL =>
            this.il;

        public Type TypeToBuild =>
            this.typeToBuild;
    }
}

