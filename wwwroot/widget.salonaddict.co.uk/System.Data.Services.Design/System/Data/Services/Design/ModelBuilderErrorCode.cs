namespace System.Data.Services.Design
{
    using System;

    internal enum ModelBuilderErrorCode
    {
        AssociationMissingKeyColumn = 0x1777,
        BaseValue = 0x1770,
        CannotCreateEntityWithoutPrimaryKey = 0x177d,
        ClientAutoGenNotAllowed = 0x177a,
        CodeGenAdditionalEdmSchemaIsInvalid = 0x1780,
        CodeGenNamespaceCannotBeDetermined = 0x177e,
        CodeGenSourceFilePathIsInvalid = 0x177f,
        DirectoryNotFound = 0x1785,
        DuplicateClassName = 0x1792,
        EntityTypeAndSetAccessibilityConflict = 0x1794,
        ExcludedColumnWasAKeyColumn = 0x178f,
        FacetValueOutOfRange = 0x1776,
        FileNotFound = 0x1784,
        GeneratedFactoryMethodNameConflict = 0x178d,
        GeneratedNavigationPropertyNameConflict = 0x1781,
        GeneratedPropertyAccessibilityConflict = 0x1791,
        IncompatibleSettingForCaseSensitiveOption = 0x1787,
        InvalidAttributeSuppliedForProperty = 0x1782,
        InvalidAttributeSuppliedForType = 0x1788,
        InvalidGetStatementSuppliedForProperty = 0x178b,
        InvalidInterfaceSuppliedForType = 0x178a,
        InvalidKeyTypeFound = 0x1790,
        InvalidMemberSuppliedForType = 0x1789,
        InvalidSetStatementSuppliedForProperty = 0x178c,
        IOException = 0x1786,
        MissingEntity = 0x1774,
        NoPrimaryKeyDefined = 0x1772,
        OneToOneAssociationFound = 0x1779,
        ParameterDirectionNotValid = 0x177c,
        PrimaryKeyCannotBeForeignKey = 0x1778,
        RelationshipSpansSchemas = 0x1771,
        SecurityError = 0x1783,
        UnknownError = 0x1773,
        UnsupportedDbRelationship = 0x1793,
        UnsupportedForeinKeyPattern = 0x178e,
        UnsupportedModelGenerationConcept = 0x177b,
        UnsupportedType = 0x1775
    }
}

