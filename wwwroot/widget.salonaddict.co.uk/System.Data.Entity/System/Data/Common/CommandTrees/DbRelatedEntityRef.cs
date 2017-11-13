namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    internal sealed class DbRelatedEntityRef
    {
        private readonly RelationshipEndMember _sourceEnd;
        private readonly RelationshipEndMember _targetEnd;
        private ExpressionLink _targetEntityRef;

        internal DbRelatedEntityRef(DbCommandTree commandTree, RelationshipEndMember sourceEnd, RelationshipEndMember targetEnd, DbExpression targetEntityRef)
        {
            commandTree.TypeHelper.CheckMember(sourceEnd, "sourceEnd");
            commandTree.TypeHelper.CheckMember(targetEnd, "targetEnd");
            this._targetEntityRef = new ExpressionLink("TargetEntityReference", commandTree);
            this._targetEntityRef.InitializeValue(targetEntityRef);
            if (!object.ReferenceEquals(sourceEnd.DeclaringType, targetEnd.DeclaringType))
            {
                throw EntityUtil.Argument(Strings.Cqt_RelatedEntityRef_TargetEndFromDifferentRelationship, "targetEnd");
            }
            if (object.ReferenceEquals(sourceEnd, targetEnd))
            {
                throw EntityUtil.Argument(Strings.Cqt_RelatedEntityRef_TargetEndSameAsSourceEnd, "targetEnd");
            }
            if ((targetEnd.RelationshipMultiplicity != RelationshipMultiplicity.One) && (targetEnd.RelationshipMultiplicity != RelationshipMultiplicity.ZeroOrOne))
            {
                throw EntityUtil.Argument(Strings.Cqt_RelatedEntityRef_TargetEndMustBeAtMostOne, "targetEnd");
            }
            if (!TypeSemantics.IsReferenceType(targetEntityRef.ResultType))
            {
                throw EntityUtil.Argument(Strings.Cqt_RelatedEntityRef_TargetEntityNotRef, "targetEntityRef");
            }
            EntityTypeBase elementType = TypeHelpers.GetEdmType<RefType>(targetEnd.TypeUsage).ElementType;
            EntityTypeBase item = TypeHelpers.GetEdmType<RefType>(targetEntityRef.ResultType).ElementType;
            if (!elementType.EdmEquals(item) && !TypeSemantics.IsSubTypeOf(item, elementType))
            {
                throw EntityUtil.Argument(Strings.Cqt_RelatedEntityRef_TargetEntityNotCompatible, "targetEntityRef");
            }
            this._targetEnd = targetEnd;
            this._sourceEnd = sourceEnd;
        }

        internal RelationshipEndMember SourceEnd =>
            this._sourceEnd;

        internal RelationshipEndMember TargetEnd =>
            this._targetEnd;

        internal DbExpression TargetEntityReference
        {
            get => 
                this._targetEntityRef.Expression;
            set
            {
                this._targetEntityRef.Expression = value;
            }
        }
    }
}

