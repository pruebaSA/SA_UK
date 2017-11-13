namespace System.Web.Resources
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [DebuggerNonUserCode, GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0"), CompilerGenerated]
    internal class WCFModelStrings
    {
        private static CultureInfo resourceCulture;
        private static System.Resources.ResourceManager resourceMan;

        internal WCFModelStrings()
        {
        }

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

        internal static string ReferenceGroup_AppendLinePosition =>
            ResourceManager.GetString("ReferenceGroup_AppendLinePosition", resourceCulture);

        internal static string ReferenceGroup_DataContractExcludedAndIncluded =>
            ResourceManager.GetString("ReferenceGroup_DataContractExcludedAndIncluded", resourceCulture);

        internal static string ReferenceGroup_DuplicatedSchemaItems =>
            ResourceManager.GetString("ReferenceGroup_DuplicatedSchemaItems", resourceCulture);

        internal static string ReferenceGroup_DuplicatedSchemaItemsIgnored =>
            ResourceManager.GetString("ReferenceGroup_DuplicatedSchemaItemsIgnored", resourceCulture);

        internal static string ReferenceGroup_EmptyAddress =>
            ResourceManager.GetString("ReferenceGroup_EmptyAddress", resourceCulture);

        internal static string ReferenceGroup_EmptyProtocol =>
            ResourceManager.GetString("ReferenceGroup_EmptyProtocol", resourceCulture);

        internal static string ReferenceGroup_FailedToGenerateCode =>
            ResourceManager.GetString("ReferenceGroup_FailedToGenerateCode", resourceCulture);

        internal static string ReferenceGroup_FailedToLoadAssembly =>
            ResourceManager.GetString("ReferenceGroup_FailedToLoadAssembly", resourceCulture);

        internal static string ReferenceGroup_FieldDefinedDifferentlyInDuplicatedMessage =>
            ResourceManager.GetString("ReferenceGroup_FieldDefinedDifferentlyInDuplicatedMessage", resourceCulture);

        internal static string ReferenceGroup_FieldDefinedInOneOfDuplicatedMessage =>
            ResourceManager.GetString("ReferenceGroup_FieldDefinedInOneOfDuplicatedMessage", resourceCulture);

        internal static string ReferenceGroup_InvalidFileName =>
            ResourceManager.GetString("ReferenceGroup_InvalidFileName", resourceCulture);

        internal static string ReferenceGroup_InvalidSourceId =>
            ResourceManager.GetString("ReferenceGroup_InvalidSourceId", resourceCulture);

        internal static string ReferenceGroup_OperationDefinedDifferently =>
            ResourceManager.GetString("ReferenceGroup_OperationDefinedDifferently", resourceCulture);

        internal static string ReferenceGroup_OperationDefinedInOneOfDuplicatedServiceContract =>
            ResourceManager.GetString("ReferenceGroup_OperationDefinedInOneOfDuplicatedServiceContract", resourceCulture);

        internal static string ReferenceGroup_ServiceContractMappingMissMatch =>
            ResourceManager.GetString("ReferenceGroup_ServiceContractMappingMissMatch", resourceCulture);

        internal static string ReferenceGroup_SharedTypeMustBePublic =>
            ResourceManager.GetString("ReferenceGroup_SharedTypeMustBePublic", resourceCulture);

        internal static string ReferenceGroup_TwoExternalFilesWithSameName =>
            ResourceManager.GetString("ReferenceGroup_TwoExternalFilesWithSameName", resourceCulture);

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    System.Resources.ResourceManager manager = new System.Resources.ResourceManager("System.Web.Resources.WCFModelStrings", typeof(WCFModelStrings).Assembly);
                    resourceMan = manager;
                }
                return resourceMan;
            }
        }
    }
}

