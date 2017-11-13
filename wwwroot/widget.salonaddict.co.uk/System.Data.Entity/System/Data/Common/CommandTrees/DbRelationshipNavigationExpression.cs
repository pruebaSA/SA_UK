namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbRelationshipNavigationExpression : DbExpression
    {
        private ExpressionLink _from;
        private RelationshipEndMember _fromRole;
        private RelationshipType _relation;
        private RelationshipEndMember _toRole;

        internal DbRelationshipNavigationExpression(DbCommandTree cmdTree, RelationshipEndMember fromEnd, RelationshipEndMember toEnd, DbExpression from) : base(cmdTree, DbExpressionKind.RelationshipNavigation)
        {
            this.CheckEnds(fromEnd, toEnd);
            RelationshipType declaringType = fromEnd.DeclaringType as RelationshipType;
            CommandTreeTypeHelper.CheckType(declaringType);
            if (!declaringType.Equals(toEnd.DeclaringType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Factory_IncompatibleRelationEnds, "toEnd");
            }
            this._relation = declaringType;
            this._fromRole = fromEnd;
            this._toRole = toEnd;
            this._from = new ExpressionLink("NavigationSource", cmdTree, GetExpectedInstanceType(fromEnd, from), from);
            base.ResultType = GetResultType(toEnd);
        }

        internal DbRelationshipNavigationExpression(DbCommandTree cmdTree, RelationshipType type, string fromEndName, string toEndName, DbExpression from) : base(cmdTree, DbExpressionKind.RelationshipNavigation)
        {
            EntityUtil.CheckArgumentNull<string>(fromEndName, "fromEndName");
            EntityUtil.CheckArgumentNull<string>(toEndName, "toEndName");
            CommandTreeTypeHelper.CheckType(type);
            RelationshipEndMember fromEnd = FindEnd(type.Members, fromEndName, "fromEndName");
            RelationshipEndMember toEnd = FindEnd(type.Members, toEndName, "toEndName");
            this.CheckEnds(fromEnd, toEnd);
            this._relation = type;
            this._fromRole = fromEnd;
            this._toRole = toEnd;
            this._from = new ExpressionLink("NavigationSource", cmdTree, GetExpectedInstanceType(fromEnd, from), from);
            base.ResultType = GetResultType(toEnd);
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

        private void CheckEnds(RelationshipEndMember fromEnd, RelationshipEndMember toEnd)
        {
            base.CommandTree.TypeHelper.CheckMember(fromEnd, "fromEnd");
            base.CommandTree.TypeHelper.CheckMember(toEnd, "toEnd");
        }

        internal static RelationshipEndMember FindEnd(IEnumerable<EdmMember> members, string endName, string varName)
        {
            foreach (EdmMember member in members)
            {
                RelationshipEndMember member2 = member as RelationshipEndMember;
                if ((member2 != null) && member2.Name.Equals(endName, StringComparison.Ordinal))
                {
                    return member2;
                }
            }
            throw EntityUtil.ArgumentOutOfRange(Strings.Cqt_Factory_NoSuchRelationEnd, varName);
        }

        private static TypeUsage GetExpectedInstanceType(RelationshipEndMember end, DbExpression from)
        {
            TypeUsage typeUsage = end.TypeUsage;
            if (!TypeSemantics.IsReferenceType(typeUsage))
            {
                typeUsage = TypeHelpers.CreateReferenceTypeUsage(TypeHelpers.GetEdmType<EntityType>(typeUsage));
            }
            TypeUsage commonTypeUsage = TypeHelpers.GetCommonTypeUsage(typeUsage, from.ResultType);
            if (commonTypeUsage == null)
            {
                throw EntityUtil.Argument(Strings.Cqt_RelNav_WrongSourceType(TypeHelpers.GetFullName(typeUsage)), "from");
            }
            return commonTypeUsage;
        }

        private static TypeUsage GetResultType(RelationshipEndMember end)
        {
            TypeUsage typeUsage = end.TypeUsage;
            if (!TypeSemantics.IsReferenceType(typeUsage))
            {
                typeUsage = TypeHelpers.CreateReferenceTypeUsage(TypeHelpers.GetEdmType<EntityType>(typeUsage));
            }
            if (RelationshipMultiplicity.Many == end.RelationshipMultiplicity)
            {
                typeUsage = TypeHelpers.CreateCollectionTypeUsage(typeUsage);
            }
            return typeUsage;
        }

        public RelationshipEndMember NavigateFrom =>
            this._fromRole;

        public RelationshipEndMember NavigateTo =>
            this._toRole;

        public DbExpression NavigationSource
        {
            get => 
                this._from.Expression;
            internal set
            {
                this._from.Expression = value;
            }
        }

        public RelationshipType Relationship =>
            this._relation;
    }
}

