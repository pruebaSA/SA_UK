namespace System.Data.Objects.DataClasses
{
    internal interface IRelationshipFixer
    {
        RelatedEnd CreateSourceEnd(RelationshipNavigation navigation, RelationshipManager relationshipManager);
    }
}

