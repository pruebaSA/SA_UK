namespace System.Data.Mapping.ViewGeneration.Utils
{
    using System;

    internal enum ViewGenErrorCode
    {
        AmbiguousMultiConstants = 0xbbd,
        AttributesUnrecoverable = 0xbbc,
        ConcurrencyDerivedClass = 0xbc0,
        ConcurrencyTokenHasCondition = 0xbc1,
        DisjointConstraintViolation = 0xbcc,
        DomainConstraintViolation = 0xbc4,
        DuplicateCPropertiesMapped = 0xbcd,
        ErrorPatternConditionError = 0xbd8,
        ErrorPatternInvalidPartitionError = 0xbda,
        ErrorPatternMissingMappingError = 0xbdb,
        ErrorPatternSplittingError = 0xbd9,
        ForeignKeyColumnOrderIncorrect = 0xbcb,
        ForeignKeyLowerBoundMustBeOne = 0xbc9,
        ForeignKeyMissingRelationshipMapping = 0xbc7,
        ForeignKeyMissingTableMapping = 0xbc5,
        ForeignKeyNotGuaranteedInCSpace = 0xbc6,
        ForeignKeyParentTableNotMappedToEnd = 0xbca,
        ForeignKeyUpperBoundMustBeOne = 0xbc8,
        ImpopssibleCondition = 0xbd6,
        InvalidCondition = 0xbb9,
        KeyConstraintUpdateViolation = 0xbbb,
        KeyConstraintViolation = 0xbba,
        KeyNotMappedForCSideExtent = 0xbd0,
        KeyNotMappedForTable = 0xbd1,
        MissingExtentMapping = 0xbd3,
        NoDefaultValue = 0xbcf,
        NonKeyProjectedWithOverlappingPartitions = 0xbbf,
        NotNullNoProjectedSlot = 0xbce,
        NullableMappingForNonNullableColumn = 0xbd7,
        PartitionConstraintViolation = 0xbd2,
        Value = 0xbb8
    }
}

