namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    internal static class ViewValidator
    {
        internal static IEnumerable<EdmSchemaError> ValidateQueryView(DbQueryCommandTree view, StoreItemCollection storeItemCollection, StorageSetMapping setMapping, EntityTypeBase elementType, bool includeSubtypes)
        {
            ViewExpressionValidator validator = new ViewExpressionValidator(storeItemCollection, setMapping, elementType, includeSubtypes);
            validator.VisitExpression(view.Query);
            return validator.Errors;
        }

        private sealed class ViewExpressionValidator : BasicExpressionVisitor
        {
            private readonly EntityTypeBase _elementType;
            private readonly List<EdmSchemaError> _errors;
            private readonly bool _includeSubtypes;
            private readonly StorageSetMapping _setMapping;
            private readonly StoreItemCollection _storeItemCollection;

            internal ViewExpressionValidator(StoreItemCollection storeItemCollection, StorageSetMapping setMapping, EntityTypeBase elementType, bool includeSubtypes)
            {
                this._setMapping = setMapping;
                this._storeItemCollection = storeItemCollection;
                this._errors = new List<EdmSchemaError>();
                this._elementType = elementType;
                this._includeSubtypes = includeSubtypes;
            }

            private void ValidateExpressionKind(DbExpressionKind expressionKind)
            {
                switch (expressionKind)
                {
                    case DbExpressionKind.And:
                    case DbExpressionKind.Case:
                    case DbExpressionKind.Cast:
                    case DbExpressionKind.Constant:
                    case DbExpressionKind.EntityRef:
                    case DbExpressionKind.Equals:
                    case DbExpressionKind.Filter:
                    case DbExpressionKind.FullOuterJoin:
                    case DbExpressionKind.GreaterThan:
                    case DbExpressionKind.GreaterThanOrEquals:
                    case DbExpressionKind.InnerJoin:
                    case DbExpressionKind.IsNull:
                    case DbExpressionKind.LeftOuterJoin:
                    case DbExpressionKind.LessThan:
                    case DbExpressionKind.LessThanOrEquals:
                    case DbExpressionKind.NewInstance:
                    case DbExpressionKind.Not:
                    case DbExpressionKind.NotEquals:
                    case DbExpressionKind.Null:
                    case DbExpressionKind.Or:
                    case DbExpressionKind.Project:
                    case DbExpressionKind.Property:
                    case DbExpressionKind.Ref:
                    case DbExpressionKind.Scan:
                    case DbExpressionKind.UnionAll:
                    case DbExpressionKind.VariableReference:
                        return;
                }
                string str = this._includeSubtypes ? ("IsTypeOf(" + this._elementType.ToString() + ")") : this._elementType.ToString();
                this._errors.Add(new EdmSchemaError(Strings.Mapping_UnsupportedExpressionKind_QueryView_2(this._setMapping.Set.Name, str, expressionKind), 0x817, EdmSchemaErrorSeverity.Error, this._setMapping.EntityContainerMapping.SourceLocation, this._setMapping.StartLineNumber, this._setMapping.StartLinePosition));
            }

            public override void Visit(DbNewInstanceExpression expression)
            {
                base.Visit(expression);
                EdmType edmType = expression.ResultType.EdmType;
                if ((edmType.BuiltInTypeKind != BuiltInTypeKind.RowType) && !this._setMapping.Set.ElementType.IsAssignableFrom(edmType))
                {
                    this._errors.Add(new EdmSchemaError(Strings.Mapping_UnsupportedInitialization_QueryView_2(this._setMapping.Set.Name, edmType.FullName), 0x81a, EdmSchemaErrorSeverity.Error, this._setMapping.EntityContainerMapping.SourceLocation, this._setMapping.StartLineNumber, this._setMapping.StartLinePosition));
                }
            }

            public override void Visit(DbPropertyExpression expression)
            {
                base.Visit(expression);
                if (expression.Property.BuiltInTypeKind != BuiltInTypeKind.EdmProperty)
                {
                    this._errors.Add(new EdmSchemaError(Strings.Mapping_UnsupportedPropertyKind_QueryView_3(this._setMapping.Set.Name, expression.Property.Name, expression.Property.BuiltInTypeKind), 0x819, EdmSchemaErrorSeverity.Error, this._setMapping.EntityContainerMapping.SourceLocation, this._setMapping.StartLineNumber, this._setMapping.StartLinePosition));
                }
            }

            public override void Visit(DbScanExpression expression)
            {
                base.Visit(expression);
                EntitySetBase target = expression.Target;
                if (target.EntityContainer.DataSpace != DataSpace.SSpace)
                {
                    this._errors.Add(new EdmSchemaError(Strings.Mapping_UnsupportedScanTarget_QueryView_2(this._setMapping.Set.Name, target.Name), 0x818, EdmSchemaErrorSeverity.Error, this._setMapping.EntityContainerMapping.SourceLocation, this._setMapping.StartLineNumber, this._setMapping.StartLinePosition));
                }
            }

            public override void VisitExpression(DbExpression expression)
            {
                if (expression != null)
                {
                    this.ValidateExpressionKind(expression.ExpressionKind);
                }
                base.VisitExpression(expression);
            }

            internal IEnumerable<EdmSchemaError> Errors =>
                this._errors;
        }
    }
}

