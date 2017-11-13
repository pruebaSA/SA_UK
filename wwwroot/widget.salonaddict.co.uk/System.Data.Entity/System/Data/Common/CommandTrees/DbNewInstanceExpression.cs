namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbNewInstanceExpression : DbExpression
    {
        private IList<DbExpression> _elements;
        private System.Collections.ObjectModel.ReadOnlyCollection<DbRelatedEntityRef> _relatedEntityRefs;

        internal DbNewInstanceExpression(DbCommandTree cmdTree, TypeUsage type, IList<DbExpression> args) : base(cmdTree, DbExpressionKind.NewInstance)
        {
            cmdTree.TypeHelper.CheckType(type);
            CollectionType type2 = null;
            if (TypeHelpers.TryGetEdmType<CollectionType>(type, out type2) && (type2 != null))
            {
                TypeUsage typeUsage = type2.TypeUsage;
                if ((args == null) || (args.Count == 0))
                {
                    this._elements = new ExpressionList("Arguments", cmdTree, 0);
                }
                else
                {
                    this._elements = new ExpressionList("Arguments", cmdTree, typeUsage, args);
                }
            }
            else
            {
                EntityUtil.CheckArgumentNull<IList<DbExpression>>(args, "args");
                this._elements = CreateStructuralArgumentList(cmdTree, type, type.EdmType as StructuralType, args);
            }
            base.ResultType = type;
        }

        internal DbNewInstanceExpression(DbCommandTree cmdTree, EntityType entityType, IList<DbExpression> attributeValues, IList<DbRelatedEntityRef> relationships) : base(cmdTree, DbExpressionKind.NewInstance)
        {
            EntityUtil.CheckArgumentNull<EntityType>(entityType, "entityType");
            EntityUtil.CheckArgumentNull<IList<DbExpression>>(attributeValues, "attributeValues");
            EntityUtil.CheckArgumentNull<IList<DbRelatedEntityRef>>(relationships, "relationships");
            TypeUsage type = CommandTreeTypeHelper.CreateResultType(entityType);
            cmdTree.TypeHelper.CheckType(type, "entityType");
            this._elements = CreateStructuralArgumentList(cmdTree, type, entityType, attributeValues);
            if (relationships.Count > 0)
            {
                List<DbRelatedEntityRef> list = new List<DbRelatedEntityRef>(relationships.Count);
                for (int i = 0; i < relationships.Count; i++)
                {
                    DbRelatedEntityRef ref2 = relationships[i];
                    EntityUtil.CheckArgumentNull<DbRelatedEntityRef>(ref2, CommandTreeUtils.FormatIndex("relationships", i));
                    if (!object.ReferenceEquals(ref2.TargetEntityReference.CommandTree, cmdTree))
                    {
                        throw EntityUtil.Argument(Strings.Cqt_NewInstance_IncompatibleRelatedEntity_TargetEntityNotValid, CommandTreeUtils.FormatIndex("relationships", i));
                    }
                    EntityTypeBase elementType = TypeHelpers.GetEdmType<RefType>(ref2.SourceEnd.TypeUsage).ElementType;
                    if (!entityType.EdmEquals(elementType) && !entityType.IsSubtypeOf(elementType))
                    {
                        throw EntityUtil.Argument(Strings.Cqt_NewInstance_IncompatibleRelatedEntity_SourceTypeNotValid, CommandTreeUtils.FormatIndex("relationships", i));
                    }
                    list.Add(ref2);
                }
                this._relatedEntityRefs = list.AsReadOnly();
            }
            base.ResultType = type;
        }

        public override void Accept(DbExpressionVisitor visitor)
        {
            if (visitor == null)
            {
                throw EntityUtil.ArgumentNull("visitor");
            }
            visitor.Visit(this);
        }

        public override TResultType Accept<TResultType>(DbExpressionVisitor<TResultType> visitor) => 
            visitor?.Visit(this);

        private static IList<DbExpression> CreateStructuralArgumentList(DbCommandTree cmdTree, TypeUsage type, StructuralType structType, IList<DbExpression> args)
        {
            if (structType == null)
            {
                throw EntityUtil.Argument(Strings.Cqt_NewInstance_StructuralTypeRequired, "type");
            }
            if (structType.Members.Count < 1)
            {
                throw EntityUtil.Argument(Strings.Cqt_NewInstance_CannotInstantiateMemberlessType(TypeHelpers.GetFullName(type)), "type");
            }
            if (structType.Abstract)
            {
                throw EntityUtil.Argument(Strings.Cqt_NewInstance_CannotInstantiateAbstractType(TypeHelpers.GetFullName(type)), "type");
            }
            return new ExpressionList("Arguments", cmdTree, TypeHelpers.GetAllStructuralMembers(structType), args);
        }

        public IList<DbExpression> Arguments =>
            this._elements;

        internal bool HasRelatedEntityReferences =>
            (this._relatedEntityRefs != null);

        internal System.Collections.ObjectModel.ReadOnlyCollection<DbRelatedEntityRef> RelatedEntityReferences =>
            this._relatedEntityRefs;
    }
}

