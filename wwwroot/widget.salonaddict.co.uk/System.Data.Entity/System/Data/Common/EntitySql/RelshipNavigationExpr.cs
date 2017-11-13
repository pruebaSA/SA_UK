namespace System.Data.Common.EntitySql
{
    using System;
    using System.Data;
    using System.Data.Entity;

    internal sealed class RelshipNavigationExpr : Expr
    {
        private Identifier _fromEndIdentifier;
        private Expr _fromEntity;
        private DottedIdentifier _relationTypeName;
        private Identifier _toEndIdentifier;

        internal RelshipNavigationExpr(Expr fromEntity, DotExpr relationTypeName)
        {
            this._fromEntity = fromEntity;
            if (!relationTypeName.IsDottedIdentifier)
            {
                throw EntityUtil.EntitySqlError(relationTypeName.ErrCtx, Strings.InvalidRelationTypeName);
            }
            this._relationTypeName = new DottedIdentifier(relationTypeName);
        }

        internal RelshipNavigationExpr(Expr fromEntity, Identifier relationTypeName)
        {
            this._fromEntity = fromEntity;
            this._relationTypeName = new DottedIdentifier(relationTypeName);
        }

        internal RelshipNavigationExpr(Expr fromEntity, DotExpr relationTypeName, Identifier toEndIdentifier) : this(fromEntity, relationTypeName)
        {
            this._toEndIdentifier = toEndIdentifier;
        }

        internal RelshipNavigationExpr(Expr fromEntity, Identifier relationTypeName, Identifier toEndIdentifier) : this(fromEntity, relationTypeName)
        {
            this._toEndIdentifier = toEndIdentifier;
        }

        internal RelshipNavigationExpr(Expr fromEntity, DotExpr relationTypeName, Identifier toEndIdentifier, Identifier fromEndIdentifier) : this(fromEntity, relationTypeName, toEndIdentifier)
        {
            this._fromEndIdentifier = fromEndIdentifier;
        }

        internal RelshipNavigationExpr(Expr fromEntity, Identifier relationTypeName, Identifier toEndIdentifier, Identifier fromEndIdentifier) : this(fromEntity, relationTypeName, toEndIdentifier)
        {
            this._fromEndIdentifier = fromEndIdentifier;
        }

        internal Identifier FromEndIdentifier =>
            this._fromEndIdentifier;

        internal string FromEndIdentifierName
        {
            get
            {
                if (this.FromEndIdentifier != null)
                {
                    return this.FromEndIdentifier.Name;
                }
                return string.Empty;
            }
        }

        internal Expr RelationshipSource =>
            this._fromEntity;

        internal string RelationTypeFullName =>
            this.RelationTypeNameIdentifier.FullName;

        internal DottedIdentifier RelationTypeNameIdentifier =>
            this._relationTypeName;

        internal string[] RelationTypeNames =>
            this.RelationTypeNameIdentifier.Names;

        internal Identifier ToEndIdentifier =>
            this._toEndIdentifier;

        internal string ToEndIdentifierName
        {
            get
            {
                if (this.ToEndIdentifier != null)
                {
                    return this.ToEndIdentifier.Name;
                }
                return string.Empty;
            }
        }
    }
}

