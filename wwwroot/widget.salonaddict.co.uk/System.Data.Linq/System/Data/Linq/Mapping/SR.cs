namespace System.Data.Linq.Mapping
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Threading;

    internal sealed class SR
    {
        internal const string AbstractClassAssignInheritanceDiscriminator = "AbstractClassAssignInheritanceDiscriminator";
        internal const string BadFunctionTypeInMethodMapping = "BadFunctionTypeInMethodMapping";
        internal const string BadKeyMember = "BadKeyMember";
        internal const string BadStorageProperty = "BadStorageProperty";
        internal const string CannotGetInheritanceDefaultFromNonInheritanceClass = "CannotGetInheritanceDefaultFromNonInheritanceClass";
        internal const string CouldNotCreateAccessorToProperty = "CouldNotCreateAccessorToProperty";
        internal const string CouldNotFindElementTypeInModel = "CouldNotFindElementTypeInModel";
        internal const string CouldNotFindRequiredAttribute = "CouldNotFindRequiredAttribute";
        internal const string CouldNotFindRuntimeTypeForMapping = "CouldNotFindRuntimeTypeForMapping";
        internal const string CouldNotFindTypeFromMapping = "CouldNotFindTypeFromMapping";
        internal const string DatabaseNodeNotFound = "DatabaseNodeNotFound";
        internal const string DiscriminatorClrTypeNotSupported = "DiscriminatorClrTypeNotSupported";
        internal const string EntityRefAlreadyLoaded = "EntityRefAlreadyLoaded";
        internal const string ExpectedEmptyElement = "ExpectedEmptyElement";
        internal const string IdentityClrTypeNotSupported = "IdentityClrTypeNotSupported";
        internal const string IncorrectAutoSyncSpecification = "IncorrectAutoSyncSpecification";
        internal const string IncorrectNumberOfParametersMappedForMethod = "IncorrectNumberOfParametersMappedForMethod";
        internal const string InheritanceCodeMayNotBeNull = "InheritanceCodeMayNotBeNull";
        internal const string InheritanceCodeUsedForMultipleTypes = "InheritanceCodeUsedForMultipleTypes";
        internal const string InheritanceHierarchyDoesNotDefineDefault = "InheritanceHierarchyDoesNotDefineDefault";
        internal const string InheritanceSubTypeIsAlsoRoot = "InheritanceSubTypeIsAlsoRoot";
        internal const string InheritanceTypeDoesNotDeriveFromRoot = "InheritanceTypeDoesNotDeriveFromRoot";
        internal const string InheritanceTypeHasMultipleDefaults = "InheritanceTypeHasMultipleDefaults";
        internal const string InheritanceTypeHasMultipleDiscriminators = "InheritanceTypeHasMultipleDiscriminators";
        internal const string InvalidDeleteOnNullSpecification = "InvalidDeleteOnNullSpecification";
        internal const string InvalidFieldInfo = "InvalidFieldInfo";
        internal const string InvalidUseOfGenericMethodAsMappedFunction = "InvalidUseOfGenericMethodAsMappedFunction";
        internal const string LinkAlreadyLoaded = "LinkAlreadyLoaded";
        private static System.Data.Linq.Mapping.SR loader;
        internal const string MappedMemberHadNoCorrespondingMemberInType = "MappedMemberHadNoCorrespondingMemberInType";
        internal const string MappingForTableUndefined = "MappingForTableUndefined";
        internal const string MappingOfInterfacesMemberIsNotSupported = "MappingOfInterfacesMemberIsNotSupported";
        internal const string MemberMappedMoreThanOnce = "MemberMappedMoreThanOnce";
        internal const string MethodCannotBeFound = "MethodCannotBeFound";
        internal const string MismatchedThisKeyOtherKey = "MismatchedThisKeyOtherKey";
        internal const string NoDiscriminatorFound = "NoDiscriminatorFound";
        internal const string NonInheritanceClassHasDiscriminator = "NonInheritanceClassHasDiscriminator";
        internal const string NoResultTypesDeclaredForFunction = "NoResultTypesDeclaredForFunction";
        internal const string OwningTeam = "OwningTeam";
        internal const string PrimaryKeyInSubTypeNotSupported = "PrimaryKeyInSubTypeNotSupported";
        internal const string ProviderTypeNotFound = "ProviderTypeNotFound";
        private ResourceManager resources;
        internal const string TooManyResultTypesDeclaredForFunction = "TooManyResultTypesDeclaredForFunction";
        internal const string TwoMembersMarkedAsInheritanceDiscriminator = "TwoMembersMarkedAsInheritanceDiscriminator";
        internal const string TwoMembersMarkedAsPrimaryKeyAndDBGenerated = "TwoMembersMarkedAsPrimaryKeyAndDBGenerated";
        internal const string TwoMembersMarkedAsRowVersion = "TwoMembersMarkedAsRowVersion";
        internal const string UnableToAssignValueToReadonlyProperty = "UnableToAssignValueToReadonlyProperty";
        internal const string UnableToResolveRootForType = "UnableToResolveRootForType";
        internal const string UnexpectedElement = "UnexpectedElement";
        internal const string UnexpectedNull = "UnexpectedNull";
        internal const string UnhandledDeferredStorageType = "UnhandledDeferredStorageType";
        internal const string UnmappedClassMember = "UnmappedClassMember";
        internal const string UnrecognizedAttribute = "UnrecognizedAttribute";
        internal const string UnrecognizedElement = "UnrecognizedElement";

        internal SR()
        {
            this.resources = new ResourceManager("System.Data.Linq.Mapping", base.GetType().Assembly);
        }

        private static System.Data.Linq.Mapping.SR GetLoader()
        {
            if (loader == null)
            {
                System.Data.Linq.Mapping.SR sr = new System.Data.Linq.Mapping.SR();
                Interlocked.CompareExchange<System.Data.Linq.Mapping.SR>(ref loader, sr, null);
            }
            return loader;
        }

        public static object GetObject(string name)
        {
            System.Data.Linq.Mapping.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetObject(name, Culture);
        }

        public static string GetString(string name)
        {
            System.Data.Linq.Mapping.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetString(name, Culture);
        }

        public static string GetString(string name, params object[] args)
        {
            System.Data.Linq.Mapping.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            string format = loader.resources.GetString(name, Culture);
            if ((args == null) || (args.Length <= 0))
            {
                return format;
            }
            for (int i = 0; i < args.Length; i++)
            {
                string str2 = args[i] as string;
                if ((str2 != null) && (str2.Length > 0x400))
                {
                    args[i] = str2.Substring(0, 0x3fd) + "...";
                }
            }
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        private static CultureInfo Culture =>
            null;

        public static ResourceManager Resources =>
            GetLoader().resources;
    }
}

