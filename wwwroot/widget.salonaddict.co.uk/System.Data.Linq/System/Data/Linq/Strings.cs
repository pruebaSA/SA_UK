namespace System.Data.Linq
{
    using System;

    internal static class Strings
    {
        internal static string ArgumentTypeHasNoIdentityKey(object p0) => 
            System.Data.Linq.SR.GetString("ArgumentTypeHasNoIdentityKey", new object[] { p0 });

        internal static string CannotChangeInheritanceType(object p0, object p1, object p2, object p3) => 
            System.Data.Linq.SR.GetString("CannotChangeInheritanceType", new object[] { p0, p1, p2, p3 });

        internal static string CannotPerformCUDOnReadOnlyTable(object p0) => 
            System.Data.Linq.SR.GetString("CannotPerformCUDOnReadOnlyTable", new object[] { p0 });

        internal static string ColumnMappedMoreThanOnce(object p0) => 
            System.Data.Linq.SR.GetString("ColumnMappedMoreThanOnce", new object[] { p0 });

        internal static string CouldNotConvert(object p0, object p1) => 
            System.Data.Linq.SR.GetString("CouldNotConvert", new object[] { p0, p1 });

        internal static string CouldNotGetTableForSubtype(object p0, object p1) => 
            System.Data.Linq.SR.GetString("CouldNotGetTableForSubtype", new object[] { p0, p1 });

        internal static string CouldNotRemoveRelationshipBecauseOneSideCannotBeNull(object p0, object p1, object p2) => 
            System.Data.Linq.SR.GetString("CouldNotRemoveRelationshipBecauseOneSideCannotBeNull", new object[] { p0, p1, p2 });

        internal static string DbGeneratedChangeNotAllowed(object p0, object p1) => 
            System.Data.Linq.SR.GetString("DbGeneratedChangeNotAllowed", new object[] { p0, p1 });

        internal static string EntitySetDataBindingWithAbstractBaseClass(object p0) => 
            System.Data.Linq.SR.GetString("EntitySetDataBindingWithAbstractBaseClass", new object[] { p0 });

        internal static string EntitySetDataBindingWithNonPublicDefaultConstructor(object p0) => 
            System.Data.Linq.SR.GetString("EntitySetDataBindingWithNonPublicDefaultConstructor", new object[] { p0 });

        internal static string ExpectedQueryableArgument(object p0, object p1) => 
            System.Data.Linq.SR.GetString("ExpectedQueryableArgument", new object[] { p0, p1 });

        internal static string IdentityChangeNotAllowed(object p0, object p1) => 
            System.Data.Linq.SR.GetString("IdentityChangeNotAllowed", new object[] { p0, p1 });

        internal static string InconsistentAssociationAndKeyChange(object p0, object p1) => 
            System.Data.Linq.SR.GetString("InconsistentAssociationAndKeyChange", new object[] { p0, p1 });

        internal static string KeyIsWrongSize(object p0, object p1) => 
            System.Data.Linq.SR.GetString("KeyIsWrongSize", new object[] { p0, p1 });

        internal static string KeyValueIsWrongType(object p0, object p1) => 
            System.Data.Linq.SR.GetString("KeyValueIsWrongType", new object[] { p0, p1 });

        internal static string NonEntityAssociationMapping(object p0, object p1, object p2) => 
            System.Data.Linq.SR.GetString("NonEntityAssociationMapping", new object[] { p0, p1, p2 });

        internal static string ProviderDoesNotImplementRequiredInterface(object p0, object p1) => 
            System.Data.Linq.SR.GetString("ProviderDoesNotImplementRequiredInterface", new object[] { p0, p1 });

        internal static string SubqueryDoesNotSupportOperator(object p0) => 
            System.Data.Linq.SR.GetString("SubqueryDoesNotSupportOperator", new object[] { p0 });

        internal static string SubqueryNotSupportedOn(object p0) => 
            System.Data.Linq.SR.GetString("SubqueryNotSupportedOn", new object[] { p0 });

        internal static string SubqueryNotSupportedOnType(object p0, object p1) => 
            System.Data.Linq.SR.GetString("SubqueryNotSupportedOnType", new object[] { p0, p1 });

        internal static string TypeCouldNotBeAdded(object p0) => 
            System.Data.Linq.SR.GetString("TypeCouldNotBeAdded", new object[] { p0 });

        internal static string TypeCouldNotBeRemoved(object p0) => 
            System.Data.Linq.SR.GetString("TypeCouldNotBeRemoved", new object[] { p0 });

        internal static string TypeCouldNotBeTracked(object p0) => 
            System.Data.Linq.SR.GetString("TypeCouldNotBeTracked", new object[] { p0 });

        internal static string TypeIsNotEntity(object p0) => 
            System.Data.Linq.SR.GetString("TypeIsNotEntity", new object[] { p0 });

        internal static string TypeIsNotMarkedAsTable(object p0) => 
            System.Data.Linq.SR.GetString("TypeIsNotMarkedAsTable", new object[] { p0 });

        internal static string UnhandledBindingType(object p0) => 
            System.Data.Linq.SR.GetString("UnhandledBindingType", new object[] { p0 });

        internal static string UnhandledExpressionType(object p0) => 
            System.Data.Linq.SR.GetString("UnhandledExpressionType", new object[] { p0 });

        internal static string UpdatesFailedMessage(object p0, object p1) => 
            System.Data.Linq.SR.GetString("UpdatesFailedMessage", new object[] { p0, p1 });

        internal static string CannotAddChangeConflicts =>
            System.Data.Linq.SR.GetString("CannotAddChangeConflicts");

        internal static string CannotAttachAddNonNewEntities =>
            System.Data.Linq.SR.GetString("CannotAttachAddNonNewEntities");

        internal static string CannotAttachAlreadyExistingEntity =>
            System.Data.Linq.SR.GetString("CannotAttachAlreadyExistingEntity");

        internal static string CannotAttachAsModifiedWithoutOriginalState =>
            System.Data.Linq.SR.GetString("CannotAttachAsModifiedWithoutOriginalState");

        internal static string CannotPerformOperationDuringSubmitChanges =>
            System.Data.Linq.SR.GetString("CannotPerformOperationDuringSubmitChanges");

        internal static string CannotPerformOperationForUntrackedObject =>
            System.Data.Linq.SR.GetString("CannotPerformOperationForUntrackedObject");

        internal static string CannotPerformOperationOutsideSubmitChanges =>
            System.Data.Linq.SR.GetString("CannotPerformOperationOutsideSubmitChanges");

        internal static string CannotRemoveChangeConflicts =>
            System.Data.Linq.SR.GetString("CannotRemoveChangeConflicts");

        internal static string CannotRemoveUnattachedEntity =>
            System.Data.Linq.SR.GetString("CannotRemoveUnattachedEntity");

        internal static string CantAddAlreadyExistingItem =>
            System.Data.Linq.SR.GetString("CantAddAlreadyExistingItem");

        internal static string CantAddAlreadyExistingKey =>
            System.Data.Linq.SR.GetString("CantAddAlreadyExistingKey");

        internal static string CouldNotAttach =>
            System.Data.Linq.SR.GetString("CouldNotAttach");

        internal static string CycleDetected =>
            System.Data.Linq.SR.GetString("CycleDetected");

        internal static string DatabaseGeneratedAlreadyExistingKey =>
            System.Data.Linq.SR.GetString("DatabaseGeneratedAlreadyExistingKey");

        internal static string DataContextCannotBeUsedAfterDispose =>
            System.Data.Linq.SR.GetString("DataContextCannotBeUsedAfterDispose");

        internal static string DeferredLoadingRequiresObjectTracking =>
            System.Data.Linq.SR.GetString("DeferredLoadingRequiresObjectTracking");

        internal static string DeleteCallbackComment =>
            System.Data.Linq.SR.GetString("DeleteCallbackComment");

        internal static string EntityIsTheWrongType =>
            System.Data.Linq.SR.GetString("EntityIsTheWrongType");

        internal static string EntitySetAlreadyLoaded =>
            System.Data.Linq.SR.GetString("EntitySetAlreadyLoaded");

        internal static string EntitySetModifiedDuringEnumeration =>
            System.Data.Linq.SR.GetString("EntitySetModifiedDuringEnumeration");

        internal static string ExpectedUpdateDeleteOrChange =>
            System.Data.Linq.SR.GetString("ExpectedUpdateDeleteOrChange");

        internal static string IncludeCycleNotAllowed =>
            System.Data.Linq.SR.GetString("IncludeCycleNotAllowed");

        internal static string IncludeNotAllowedAfterFreeze =>
            System.Data.Linq.SR.GetString("IncludeNotAllowedAfterFreeze");

        internal static string InsertAutoSyncFailure =>
            System.Data.Linq.SR.GetString("InsertAutoSyncFailure");

        internal static string InsertCallbackComment =>
            System.Data.Linq.SR.GetString("InsertCallbackComment");

        internal static string InvalidLoadOptionsLoadMemberSpecification =>
            System.Data.Linq.SR.GetString("InvalidLoadOptionsLoadMemberSpecification");

        internal static string LoadOptionsChangeNotAllowedAfterQuery =>
            System.Data.Linq.SR.GetString("LoadOptionsChangeNotAllowedAfterQuery");

        internal static string ModifyDuringAddOrRemove =>
            System.Data.Linq.SR.GetString("ModifyDuringAddOrRemove");

        internal static string ObjectTrackingRequired =>
            System.Data.Linq.SR.GetString("ObjectTrackingRequired");

        internal static string OptionsCannotBeModifiedAfterQuery =>
            System.Data.Linq.SR.GetString("OptionsCannotBeModifiedAfterQuery");

        internal static string OriginalEntityIsWrongType =>
            System.Data.Linq.SR.GetString("OriginalEntityIsWrongType");

        internal static string OwningTeam =>
            System.Data.Linq.SR.GetString("OwningTeam");

        internal static string ProviderTypeNull =>
            System.Data.Linq.SR.GetString("ProviderTypeNull");

        internal static string RefreshOfDeletedObject =>
            System.Data.Linq.SR.GetString("RefreshOfDeletedObject");

        internal static string RowNotFoundOrChanged =>
            System.Data.Linq.SR.GetString("RowNotFoundOrChanged");

        internal static string SubqueryMustBeSequence =>
            System.Data.Linq.SR.GetString("SubqueryMustBeSequence");

        internal static string SubqueryNotAllowedAfterFreeze =>
            System.Data.Linq.SR.GetString("SubqueryNotAllowedAfterFreeze");

        internal static string UnableToDetermineDataContext =>
            System.Data.Linq.SR.GetString("UnableToDetermineDataContext");

        internal static string UnrecognizedRefreshObject =>
            System.Data.Linq.SR.GetString("UnrecognizedRefreshObject");

        internal static string UpdateCallbackComment =>
            System.Data.Linq.SR.GetString("UpdateCallbackComment");
    }
}

