namespace System.Data.Objects.DataClasses
{
    using System;
    using System.Data.Metadata.Edm;

    [Serializable]
    internal class RelationshipFixer<TSourceEntity, TTargetEntity> : IRelationshipFixer where TSourceEntity: class, IEntityWithRelationships where TTargetEntity: class, IEntityWithRelationships
    {
        private RelationshipMultiplicity _sourceRoleMultiplicity;
        private RelationshipMultiplicity _targetRoleMultiplicity;

        internal RelationshipFixer(RelationshipMultiplicity sourceRoleMultiplicity, RelationshipMultiplicity targetRoleMultiplicity)
        {
            this._sourceRoleMultiplicity = sourceRoleMultiplicity;
            this._targetRoleMultiplicity = targetRoleMultiplicity;
        }

        RelatedEnd IRelationshipFixer.CreateSourceEnd(RelationshipNavigation navigation, RelationshipManager relationshipManager) => 
            relationshipManager.CreateRelatedEnd<TTargetEntity, TSourceEntity>(navigation, this._targetRoleMultiplicity, this._sourceRoleMultiplicity, null);
    }
}

