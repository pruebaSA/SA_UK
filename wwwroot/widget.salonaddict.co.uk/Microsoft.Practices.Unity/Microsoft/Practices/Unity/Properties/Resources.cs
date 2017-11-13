namespace Microsoft.Practices.Unity.Properties
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
    internal class Resources
    {
        private static CultureInfo resourceCulture;
        private static System.Resources.ResourceManager resourceMan;

        internal Resources()
        {
        }

        internal static string AmbiguousInjectionConstructor =>
            ResourceManager.GetString("AmbiguousInjectionConstructor", resourceCulture);

        internal static string ArgumentMustNotBeEmpty =>
            ResourceManager.GetString("ArgumentMustNotBeEmpty", resourceCulture);

        internal static string BuildFailedException =>
            ResourceManager.GetString("BuildFailedException", resourceCulture);

        internal static string CannotConstructInterface =>
            ResourceManager.GetString("CannotConstructInterface", resourceCulture);

        internal static string CannotExtractTypeFromBuildKey =>
            ResourceManager.GetString("CannotExtractTypeFromBuildKey", resourceCulture);

        internal static string CannotInjectGenericMethod =>
            ResourceManager.GetString("CannotInjectGenericMethod", resourceCulture);

        internal static string CannotInjectIndexer =>
            ResourceManager.GetString("CannotInjectIndexer", resourceCulture);

        internal static string CannotInjectMethodWithOutParam =>
            ResourceManager.GetString("CannotInjectMethodWithOutParam", resourceCulture);

        internal static string CannotInjectMethodWithOutParams =>
            ResourceManager.GetString("CannotInjectMethodWithOutParams", resourceCulture);

        internal static string CannotInjectMethodWithRefParams =>
            ResourceManager.GetString("CannotInjectMethodWithRefParams", resourceCulture);

        internal static string CannotInjectOpenGenericMethod =>
            ResourceManager.GetString("CannotInjectOpenGenericMethod", resourceCulture);

        internal static string CannotInjectStaticMethod =>
            ResourceManager.GetString("CannotInjectStaticMethod", resourceCulture);

        internal static string CannotResolveOpenGenericType =>
            ResourceManager.GetString("CannotResolveOpenGenericType", resourceCulture);

        internal static string ConstructorArgumentResolveOperation =>
            ResourceManager.GetString("ConstructorArgumentResolveOperation", resourceCulture);

        internal static string ConstructorParameterResolutionFailed =>
            ResourceManager.GetString("ConstructorParameterResolutionFailed", resourceCulture);

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get => 
                resourceCulture;
            set
            {
                resourceCulture = value;
            }
        }

        internal static string ExceptionNullParameterValue =>
            ResourceManager.GetString("ExceptionNullParameterValue", resourceCulture);

        internal static string InvokingConstructorOperation =>
            ResourceManager.GetString("InvokingConstructorOperation", resourceCulture);

        internal static string InvokingMethodOperation =>
            ResourceManager.GetString("InvokingMethodOperation", resourceCulture);

        internal static string KeyAlreadyPresent =>
            ResourceManager.GetString("KeyAlreadyPresent", resourceCulture);

        internal static string LifetimeManagerInUse =>
            ResourceManager.GetString("LifetimeManagerInUse", resourceCulture);

        internal static string MarkerBuildPlanInvoked =>
            ResourceManager.GetString("MarkerBuildPlanInvoked", resourceCulture);

        internal static string MethodArgumentResolveOperation =>
            ResourceManager.GetString("MethodArgumentResolveOperation", resourceCulture);

        internal static string MethodParameterResolutionFailed =>
            ResourceManager.GetString("MethodParameterResolutionFailed", resourceCulture);

        internal static string MissingDependency =>
            ResourceManager.GetString("MissingDependency", resourceCulture);

        internal static string MultipleInjectionConstructors =>
            ResourceManager.GetString("MultipleInjectionConstructors", resourceCulture);

        internal static string MustHaveOpenGenericType =>
            ResourceManager.GetString("MustHaveOpenGenericType", resourceCulture);

        internal static string MustHaveSameNumberOfGenericArguments =>
            ResourceManager.GetString("MustHaveSameNumberOfGenericArguments", resourceCulture);

        internal static string NoConstructorFound =>
            ResourceManager.GetString("NoConstructorFound", resourceCulture);

        internal static string NoMatchingGenericArgument =>
            ResourceManager.GetString("NoMatchingGenericArgument", resourceCulture);

        internal static string NoOperationExceptionReason =>
            ResourceManager.GetString("NoOperationExceptionReason", resourceCulture);

        internal static string NoSuchConstructor =>
            ResourceManager.GetString("NoSuchConstructor", resourceCulture);

        internal static string NoSuchMethod =>
            ResourceManager.GetString("NoSuchMethod", resourceCulture);

        internal static string NoSuchProperty =>
            ResourceManager.GetString("NoSuchProperty", resourceCulture);

        internal static string NotAGenericType =>
            ResourceManager.GetString("NotAGenericType", resourceCulture);

        internal static string NotAnArrayTypeWithRankOne =>
            ResourceManager.GetString("NotAnArrayTypeWithRankOne", resourceCulture);

        internal static string OptionalDependenciesMustBeReferenceTypes =>
            ResourceManager.GetString("OptionalDependenciesMustBeReferenceTypes", resourceCulture);

        internal static string PropertyNotSettable =>
            ResourceManager.GetString("PropertyNotSettable", resourceCulture);

        internal static string PropertyTypeMismatch =>
            ResourceManager.GetString("PropertyTypeMismatch", resourceCulture);

        internal static string PropertyValueResolutionFailed =>
            ResourceManager.GetString("PropertyValueResolutionFailed", resourceCulture);

        internal static string ProvidedStringArgMustNotBeEmpty =>
            ResourceManager.GetString("ProvidedStringArgMustNotBeEmpty", resourceCulture);

        internal static string ResolutionFailed =>
            ResourceManager.GetString("ResolutionFailed", resourceCulture);

        internal static string ResolutionTraceDetail =>
            ResourceManager.GetString("ResolutionTraceDetail", resourceCulture);

        internal static string ResolutionWithMappingTraceDetail =>
            ResourceManager.GetString("ResolutionWithMappingTraceDetail", resourceCulture);

        internal static string ResolvingPropertyValueOperation =>
            ResourceManager.GetString("ResolvingPropertyValueOperation", resourceCulture);

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    System.Resources.ResourceManager manager = new System.Resources.ResourceManager("Microsoft.Practices.Unity.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = manager;
                }
                return resourceMan;
            }
        }

        internal static string SelectedConstructorHasRefParameters =>
            ResourceManager.GetString("SelectedConstructorHasRefParameters", resourceCulture);

        internal static string SettingPropertyOperation =>
            ResourceManager.GetString("SettingPropertyOperation", resourceCulture);

        internal static string TypeIsNotConstructable =>
            ResourceManager.GetString("TypeIsNotConstructable", resourceCulture);

        internal static string TypesAreNotAssignable =>
            ResourceManager.GetString("TypesAreNotAssignable", resourceCulture);

        internal static string UnknownType =>
            ResourceManager.GetString("UnknownType", resourceCulture);
    }
}

