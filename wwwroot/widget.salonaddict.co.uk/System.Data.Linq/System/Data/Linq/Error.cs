namespace System.Data.Linq
{
    using System;

    internal static class Error
    {
        internal static Exception ArgumentNull(string paramName) => 
            new ArgumentNullException(paramName);

        internal static Exception ArgumentOutOfRange(string paramName) => 
            new ArgumentOutOfRangeException(paramName);

        internal static Exception ArgumentTypeHasNoIdentityKey(object p0) => 
            new ArgumentException(Strings.ArgumentTypeHasNoIdentityKey(p0));

        internal static Exception CannotAddChangeConflicts() => 
            new NotSupportedException(Strings.CannotAddChangeConflicts);

        internal static Exception CannotAttachAddNonNewEntities() => 
            new NotSupportedException(Strings.CannotAttachAddNonNewEntities);

        internal static Exception CannotAttachAlreadyExistingEntity() => 
            new InvalidOperationException(Strings.CannotAttachAlreadyExistingEntity);

        internal static Exception CannotAttachAsModifiedWithoutOriginalState() => 
            new InvalidOperationException(Strings.CannotAttachAsModifiedWithoutOriginalState);

        internal static Exception CannotChangeInheritanceType(object p0, object p1, object p2, object p3) => 
            new InvalidOperationException(Strings.CannotChangeInheritanceType(p0, p1, p2, p3));

        internal static Exception CannotPerformCUDOnReadOnlyTable(object p0) => 
            new InvalidOperationException(Strings.CannotPerformCUDOnReadOnlyTable(p0));

        internal static Exception CannotPerformOperationDuringSubmitChanges() => 
            new InvalidOperationException(Strings.CannotPerformOperationDuringSubmitChanges);

        internal static Exception CannotPerformOperationForUntrackedObject() => 
            new InvalidOperationException(Strings.CannotPerformOperationForUntrackedObject);

        internal static Exception CannotPerformOperationOutsideSubmitChanges() => 
            new InvalidOperationException(Strings.CannotPerformOperationOutsideSubmitChanges);

        internal static Exception CannotRemoveChangeConflicts() => 
            new NotSupportedException(Strings.CannotRemoveChangeConflicts);

        internal static Exception CannotRemoveUnattachedEntity() => 
            new InvalidOperationException(Strings.CannotRemoveUnattachedEntity);

        internal static Exception CantAddAlreadyExistingItem() => 
            new InvalidOperationException(Strings.CantAddAlreadyExistingItem);

        internal static Exception ColumnMappedMoreThanOnce(object p0) => 
            new InvalidOperationException(Strings.ColumnMappedMoreThanOnce(p0));

        internal static Exception CouldNotAttach() => 
            new InvalidOperationException(Strings.CouldNotAttach);

        internal static Exception CouldNotConvert(object p0, object p1) => 
            new InvalidCastException(Strings.CouldNotConvert(p0, p1));

        internal static Exception CouldNotGetTableForSubtype(object p0, object p1) => 
            new InvalidOperationException(Strings.CouldNotGetTableForSubtype(p0, p1));

        internal static Exception CouldNotRemoveRelationshipBecauseOneSideCannotBeNull(object p0, object p1, object p2) => 
            new InvalidOperationException(Strings.CouldNotRemoveRelationshipBecauseOneSideCannotBeNull(p0, p1, p2));

        internal static Exception CycleDetected() => 
            new InvalidOperationException(Strings.CycleDetected);

        internal static Exception DataContextCannotBeUsedAfterDispose() => 
            new ObjectDisposedException(Strings.DataContextCannotBeUsedAfterDispose);

        internal static Exception DbGeneratedChangeNotAllowed(object p0, object p1) => 
            new InvalidOperationException(Strings.DbGeneratedChangeNotAllowed(p0, p1));

        internal static Exception DeferredLoadingRequiresObjectTracking() => 
            new InvalidOperationException(Strings.DeferredLoadingRequiresObjectTracking);

        internal static Exception EntityIsTheWrongType() => 
            new InvalidOperationException(Strings.EntityIsTheWrongType);

        internal static Exception EntitySetAlreadyLoaded() => 
            new InvalidOperationException(Strings.EntitySetAlreadyLoaded);

        internal static Exception EntitySetDataBindingWithAbstractBaseClass(object p0) => 
            new InvalidOperationException(Strings.EntitySetDataBindingWithAbstractBaseClass(p0));

        internal static Exception EntitySetDataBindingWithNonPublicDefaultConstructor(object p0) => 
            new InvalidOperationException(Strings.EntitySetDataBindingWithNonPublicDefaultConstructor(p0));

        internal static Exception EntitySetModifiedDuringEnumeration() => 
            new InvalidOperationException(Strings.EntitySetModifiedDuringEnumeration);

        internal static Exception ExpectedQueryableArgument(object p0, object p1) => 
            new ArgumentException(Strings.ExpectedQueryableArgument(p0, p1));

        internal static Exception ExpectedUpdateDeleteOrChange() => 
            new InvalidOperationException(Strings.ExpectedUpdateDeleteOrChange);

        internal static Exception IdentityChangeNotAllowed(object p0, object p1) => 
            new InvalidOperationException(Strings.IdentityChangeNotAllowed(p0, p1));

        internal static Exception IncludeCycleNotAllowed() => 
            new InvalidOperationException(Strings.IncludeCycleNotAllowed);

        internal static Exception IncludeNotAllowedAfterFreeze() => 
            new InvalidOperationException(Strings.IncludeNotAllowedAfterFreeze);

        internal static Exception InconsistentAssociationAndKeyChange(object p0, object p1) => 
            new InvalidOperationException(Strings.InconsistentAssociationAndKeyChange(p0, p1));

        internal static Exception InsertAutoSyncFailure() => 
            new InvalidOperationException(Strings.InsertAutoSyncFailure);

        internal static Exception InvalidLoadOptionsLoadMemberSpecification() => 
            new InvalidOperationException(Strings.InvalidLoadOptionsLoadMemberSpecification);

        internal static Exception KeyIsWrongSize(object p0, object p1) => 
            new InvalidOperationException(Strings.KeyIsWrongSize(p0, p1));

        internal static Exception KeyValueIsWrongType(object p0, object p1) => 
            new InvalidOperationException(Strings.KeyValueIsWrongType(p0, p1));

        internal static Exception LoadOptionsChangeNotAllowedAfterQuery() => 
            new InvalidOperationException(Strings.LoadOptionsChangeNotAllowedAfterQuery);

        internal static Exception ModifyDuringAddOrRemove() => 
            new ArgumentException(Strings.ModifyDuringAddOrRemove);

        internal static Exception NonEntityAssociationMapping(object p0, object p1, object p2) => 
            new InvalidOperationException(Strings.NonEntityAssociationMapping(p0, p1, p2));

        internal static Exception NotImplemented() => 
            new NotImplementedException();

        internal static Exception NotSupported() => 
            new NotSupportedException();

        internal static Exception ObjectTrackingRequired() => 
            new InvalidOperationException(Strings.ObjectTrackingRequired);

        internal static Exception OptionsCannotBeModifiedAfterQuery() => 
            new InvalidOperationException(Strings.OptionsCannotBeModifiedAfterQuery);

        internal static Exception OriginalEntityIsWrongType() => 
            new InvalidOperationException(Strings.OriginalEntityIsWrongType);

        internal static Exception ProviderDoesNotImplementRequiredInterface(object p0, object p1) => 
            new InvalidOperationException(Strings.ProviderDoesNotImplementRequiredInterface(p0, p1));

        internal static Exception ProviderTypeNull() => 
            new InvalidOperationException(Strings.ProviderTypeNull);

        internal static Exception RefreshOfDeletedObject() => 
            new InvalidOperationException(Strings.RefreshOfDeletedObject);

        internal static Exception SubqueryDoesNotSupportOperator(object p0) => 
            new NotSupportedException(Strings.SubqueryDoesNotSupportOperator(p0));

        internal static Exception SubqueryMustBeSequence() => 
            new InvalidOperationException(Strings.SubqueryMustBeSequence);

        internal static Exception SubqueryNotAllowedAfterFreeze() => 
            new InvalidOperationException(Strings.SubqueryNotAllowedAfterFreeze);

        internal static Exception SubqueryNotSupportedOn(object p0) => 
            new NotSupportedException(Strings.SubqueryNotSupportedOn(p0));

        internal static Exception SubqueryNotSupportedOnType(object p0, object p1) => 
            new NotSupportedException(Strings.SubqueryNotSupportedOnType(p0, p1));

        internal static Exception TypeCouldNotBeAdded(object p0) => 
            new InvalidOperationException(Strings.TypeCouldNotBeAdded(p0));

        internal static Exception TypeCouldNotBeRemoved(object p0) => 
            new InvalidOperationException(Strings.TypeCouldNotBeRemoved(p0));

        internal static Exception TypeCouldNotBeTracked(object p0) => 
            new InvalidOperationException(Strings.TypeCouldNotBeTracked(p0));

        internal static Exception TypeIsNotEntity(object p0) => 
            new InvalidOperationException(Strings.TypeIsNotEntity(p0));

        internal static Exception TypeIsNotMarkedAsTable(object p0) => 
            new InvalidOperationException(Strings.TypeIsNotMarkedAsTable(p0));

        internal static Exception UnableToDetermineDataContext() => 
            new InvalidOperationException(Strings.UnableToDetermineDataContext);

        internal static Exception UnhandledBindingType(object p0) => 
            new ArgumentException(Strings.UnhandledBindingType(p0));

        internal static Exception UnhandledExpressionType(object p0) => 
            new ArgumentException(Strings.UnhandledExpressionType(p0));

        internal static Exception UnrecognizedRefreshObject() => 
            new ArgumentException(Strings.UnrecognizedRefreshObject);
    }
}

