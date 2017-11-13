namespace System.Data.Common.CommandTrees.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.CommandTrees;
    using System.Data.Metadata.Edm;

    internal abstract class ExpressionDumper : DbExpressionVisitor
    {
        internal ExpressionDumper()
        {
        }

        private void Begin(DbExpression expr)
        {
            this.Begin(expr, new Dictionary<string, object>());
        }

        internal void Begin(string name)
        {
            this.Begin(name, (Dictionary<string, object>) null);
        }

        private void Begin(DbExpression expr, Dictionary<string, object> attrs)
        {
            attrs.Add("DbExpressionKind", Enum.GetName(typeof(DbExpressionKind), expr.ExpressionKind));
            this.Begin(expr.GetType().Name, attrs);
            this.Dump(expr.ResultType, "ResultType");
        }

        internal abstract void Begin(string name, Dictionary<string, object> attrs);
        private void Begin(string name, params object[] attrs)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            for (int i = 0; i < attrs.Length; i++)
            {
                string key = (string) attrs[i++];
                dictionary.Add(key, attrs[i]);
            }
            this.Begin(name, dictionary);
        }

        private void Begin(DbExpression expr, string attributeName, object attributeValue)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object> {
                { 
                    attributeName,
                    attributeValue
                }
            };
            this.Begin(expr, attrs);
        }

        private void BeginBinary(DbBinaryExpression e)
        {
            this.Begin(e);
            this.Begin("Left");
            this.Dump(e.Left);
            this.End("Left");
            this.Begin("Right");
            this.Dump(e.Right);
            this.End("Right");
        }

        private void BeginUnary(DbUnaryExpression e)
        {
            this.Begin(e);
            this.Begin("Argument");
            this.Dump(e.Argument);
            this.End("Argument");
        }

        internal void Dump(IEnumerable<FunctionParameter> paramList)
        {
            this.Begin("Parameters");
            foreach (FunctionParameter parameter in paramList)
            {
                this.Begin("Parameter", new object[] { "Name", parameter.Name });
                this.Dump(parameter.TypeUsage, "ParameterType");
                this.End("Parameter");
            }
            this.End("Parameters");
        }

        protected virtual void Dump(IList<DbSortClause> sortOrder)
        {
            this.Begin("SortOrder");
            foreach (DbSortClause clause in sortOrder)
            {
                string collation = clause.Collation;
                if (collation == null)
                {
                    collation = "";
                }
                this.Begin("DbSortClause", new object[] { "Ascending", clause.Ascending, "Collation", collation });
                this.Dump(clause.Expression, "Expression");
                this.End("DbSortClause");
            }
            this.End("SortOrder");
        }

        internal void Dump(DbExpression target)
        {
            target.Accept(this);
        }

        internal void Dump(DbExpressionBinding binding)
        {
            this.Begin("DbExpressionBinding", new object[] { "VariableName", binding.VariableName });
            this.Begin("Expression");
            this.Dump(binding.Expression);
            this.End("Expression");
            this.End("DbExpressionBinding");
        }

        internal void Dump(DbGroupExpressionBinding binding)
        {
            this.Begin("DbGroupExpressionBinding", new object[] { "VariableName", binding.VariableName, "GroupVariableName", binding.GroupVariableName });
            this.Begin("Expression");
            this.Dump(binding.Expression);
            this.End("Expression");
            this.End("DbGroupExpressionBinding");
        }

        internal void Dump(EdmFunction function)
        {
            this.Begin("Function", new object[] { "Name", function.Name, "Namespace", function.NamespaceName });
            this.Dump(function.Parameters);
            this.Dump(function.ReturnParameter.TypeUsage, "ReturnType");
            this.End("Function");
        }

        internal void Dump(EdmProperty prop)
        {
            this.Begin("Property", new object[] { "Name", prop.Name, "Nullable", prop.Nullable });
            this.Dump(prop.DeclaringType, "DeclaringType");
            this.Dump(prop.TypeUsage, "PropertyType");
            this.End("Property");
        }

        internal void Dump(EdmType type)
        {
            this.Begin("EdmType", new object[] { "BuiltInTypeKind", Enum.GetName(typeof(BuiltInTypeKind), type.BuiltInTypeKind), "Namespace", type.NamespaceName, "Name", type.Name });
            this.End("EdmType");
        }

        internal void Dump(RelationshipType type)
        {
            this.Begin("RelationshipType", new object[] { "Namespace", type.NamespaceName, "Name", type.Name });
            this.End("RelationshipType");
        }

        internal void Dump(TypeUsage type)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object>();
            foreach (Facet facet in type.Facets)
            {
                attrs.Add(facet.Name, facet.Value);
            }
            this.Begin("TypeUsage", attrs);
            this.Dump(type.EdmType);
            this.End("TypeUsage");
        }

        internal void Dump(DbExpression e, string name)
        {
            this.Begin(name);
            this.Dump(e);
            this.End(name);
        }

        internal void Dump(DbExpressionBinding binding, string name)
        {
            this.Begin(name);
            this.Dump(binding);
            this.End(name);
        }

        internal void Dump(DbGroupExpressionBinding binding, string name)
        {
            this.Begin(name);
            this.Dump(binding);
            this.End(name);
        }

        internal void Dump(EdmType type, string name)
        {
            this.Begin(name);
            this.Dump(type);
            this.End(name);
        }

        internal void Dump(NavigationProperty navProp, string name)
        {
            this.Begin(name);
            this.Begin("NavigationProperty", new object[] { "Name", navProp.Name, "RelationshipTypeName", navProp.RelationshipType.FullName, "ToEndMemberName", navProp.ToEndMember.Name });
            this.Dump(navProp.DeclaringType, "DeclaringType");
            this.Dump(navProp.TypeUsage, "PropertyType");
            this.End("NavigationProperty");
            this.End(name);
        }

        internal void Dump(RelationshipEndMember end, string name)
        {
            this.Begin(name);
            this.Begin("RelationshipEndMember", new object[] { "Name", end.Name, "RelationshipMultiplicity", Enum.GetName(typeof(RelationshipMultiplicity), end.RelationshipMultiplicity) });
            this.Dump(end.DeclaringType, "DeclaringRelation");
            this.Dump(end.TypeUsage, "EndType");
            this.End("RelationshipEndMember");
            this.End(name);
        }

        internal void Dump(RelationshipType type, string name)
        {
            this.Begin(name);
            this.Dump(type);
            this.End(name);
        }

        internal void Dump(TypeUsage type, string name)
        {
            this.Begin(name);
            this.Dump(type);
            this.End(name);
        }

        internal void Dump(IEnumerable<DbExpression> exprs, string pluralName, string singularName)
        {
            this.Begin(pluralName);
            foreach (DbExpression expression in exprs)
            {
                this.Begin(singularName);
                this.Dump(expression);
                this.End(singularName);
            }
            this.End(pluralName);
        }

        private void End(DbExpression expr)
        {
            this.End(expr.GetType().Name);
        }

        internal abstract void End(string name);
        public override void Visit(DbAndExpression e)
        {
            this.BeginBinary(e);
            this.End(e);
        }

        public override void Visit(DbApplyExpression e)
        {
            this.Begin(e);
            this.Dump(e.Input, "Input");
            this.Dump(e.Apply, "Apply");
            this.End(e);
        }

        public override void Visit(DbArithmeticExpression e)
        {
            this.Begin(e);
            this.Dump(e.Arguments, "Arguments", "Argument");
            this.End(e);
        }

        public override void Visit(DbCaseExpression e)
        {
            this.Begin(e);
            this.Dump(e.When, "Whens", "When");
            this.Dump(e.Then, "Thens", "Then");
            this.Dump(e.Else, "Else");
        }

        public override void Visit(DbCastExpression e)
        {
            this.BeginUnary(e);
            this.End(e);
        }

        public override void Visit(DbComparisonExpression e)
        {
            this.BeginBinary(e);
            this.End(e);
        }

        public override void Visit(DbConstantExpression e)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object> {
                { 
                    "Value",
                    e.Value
                }
            };
            this.Begin(e, attrs);
            this.End(e);
        }

        public override void Visit(DbCrossJoinExpression e)
        {
            this.Begin(e);
            this.Begin("Inputs");
            foreach (DbExpressionBinding binding in e.Inputs)
            {
                this.Dump(binding, "Input");
            }
            this.End("Inputs");
            this.End(e);
        }

        public override void Visit(DbDerefExpression e)
        {
            this.BeginUnary(e);
            this.End(e);
        }

        public override void Visit(DbDistinctExpression e)
        {
            this.BeginUnary(e);
            this.End(e);
        }

        public override void Visit(DbElementExpression e)
        {
            this.BeginUnary(e);
            this.End(e);
        }

        public override void Visit(DbEntityRefExpression e)
        {
            this.BeginUnary(e);
            this.End(e);
        }

        public override void Visit(DbExceptExpression e)
        {
            this.BeginBinary(e);
            this.End(e);
        }

        public override void Visit(DbExpression e)
        {
            this.Begin(e);
            this.End(e);
        }

        public override void Visit(DbFilterExpression e)
        {
            this.Begin(e);
            this.Dump(e.Input, "Input");
            this.Dump(e.Predicate, "Predicate");
            this.End(e);
        }

        public override void Visit(DbFunctionExpression e)
        {
            this.Begin(e);
            this.Dump(e.Function);
            this.Dump(e.Arguments, "Arguments", "Argument");
            if (e.LambdaBody != null)
            {
                this.Dump(e.LambdaBody, "LambdaBody");
            }
            this.End(e);
        }

        public override void Visit(DbGroupByExpression e)
        {
            this.Begin(e);
            this.Dump(e.Input, "Input");
            this.Dump(e.Keys, "Keys", "Key");
            this.Begin("Aggregates");
            foreach (DbAggregate aggregate in e.Aggregates)
            {
                DbFunctionAggregate aggregate2 = aggregate as DbFunctionAggregate;
                this.Begin("DbFunctionAggregate");
                this.Dump(aggregate2.Function);
                this.Dump(aggregate2.Arguments, "Arguments", "Argument");
                this.End("DbFunctionAggregate");
            }
            this.End("Aggregates");
            this.End(e);
        }

        public override void Visit(DbIntersectExpression e)
        {
            this.BeginBinary(e);
            this.End(e);
        }

        public override void Visit(DbIsEmptyExpression e)
        {
            this.BeginUnary(e);
            this.End(e);
        }

        public override void Visit(DbIsNullExpression e)
        {
            this.BeginUnary(e);
            this.End(e);
        }

        public override void Visit(DbIsOfExpression e)
        {
            this.BeginUnary(e);
            this.Dump(e.OfType, "OfType");
            this.End(e);
        }

        public override void Visit(DbJoinExpression e)
        {
            this.Begin(e);
            this.Dump(e.Left, "Left");
            this.Dump(e.Right, "Right");
            this.Dump(e.JoinCondition, "JoinCondition");
            this.End(e);
        }

        public override void Visit(DbLikeExpression e)
        {
            this.Begin(e);
            this.Dump(e.Argument, "Argument");
            this.Dump(e.Pattern, "Pattern");
            this.Dump(e.Escape, "Escape");
            this.End(e);
        }

        public override void Visit(DbLimitExpression e)
        {
            this.Begin(e, "WithTies", e.WithTies);
            this.Dump(e.Argument, "Argument");
            this.Dump(e.Limit, "Limit");
            this.End(e);
        }

        public override void Visit(DbNewInstanceExpression e)
        {
            this.Begin(e);
            this.Dump(e.Arguments, "Arguments", "Argument");
            if (e.HasRelatedEntityReferences)
            {
                this.Begin("RelatedEntityReferences");
                foreach (DbRelatedEntityRef ref2 in e.RelatedEntityReferences)
                {
                    this.Begin("DbRelatedEntityRef");
                    this.Dump(ref2.SourceEnd, "SourceEnd");
                    this.Dump(ref2.TargetEnd, "TargetEnd");
                    this.Dump(ref2.TargetEntityReference, "TargetEntityReference");
                    this.End("DbRelatedEntityRef");
                }
                this.End("RelatedEntityReferences");
            }
            this.End(e);
        }

        public override void Visit(DbNotExpression e)
        {
            this.BeginUnary(e);
            this.End(e);
        }

        public override void Visit(DbNullExpression e)
        {
            this.Begin(e);
            this.End(e);
        }

        public override void Visit(DbOfTypeExpression e)
        {
            this.BeginUnary(e);
            this.Dump(e.OfType, "OfType");
            this.End(e);
        }

        public override void Visit(DbOrExpression e)
        {
            this.BeginBinary(e);
            this.End(e);
        }

        public override void Visit(DbParameterReferenceExpression e)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object> {
                { 
                    "ParameterName",
                    e.ParameterName
                }
            };
            this.Begin(e, attrs);
            this.End(e);
        }

        public override void Visit(DbProjectExpression e)
        {
            this.Begin(e);
            this.Dump(e.Input, "Input");
            this.Dump(e.Projection, "Projection");
            this.End(e);
        }

        public override void Visit(DbPropertyExpression e)
        {
            this.Begin(e);
            RelationshipEndMember property = e.Property as RelationshipEndMember;
            if (property != null)
            {
                this.Dump(property, "Property");
            }
            else if (Helper.IsNavigationProperty(e.Property))
            {
                this.Dump((NavigationProperty) e.Property, "Property");
            }
            else
            {
                this.Dump((EdmProperty) e.Property);
            }
            if (e.Instance != null)
            {
                this.Dump(e.Instance, "Instance");
            }
            this.End(e);
        }

        public override void Visit(DbQuantifierExpression e)
        {
            this.Begin(e);
            this.Dump(e.Input, "Input");
            this.Dump(e.Predicate, "Predicate");
            this.End(e);
        }

        public override void Visit(DbRefExpression e)
        {
            this.BeginUnary(e);
            this.End(e);
        }

        public override void Visit(DbRefKeyExpression e)
        {
            this.BeginUnary(e);
            this.End(e);
        }

        public override void Visit(DbRelationshipNavigationExpression e)
        {
            this.Begin(e);
            this.Dump(e.NavigateFrom, "NavigateFrom");
            this.Dump(e.NavigateTo, "NavigateTo");
            this.Dump(e.Relationship, "Relationship");
            this.Dump(e.NavigationSource, "NavigationSource");
            this.End(e);
        }

        public override void Visit(DbScanExpression e)
        {
            this.Begin(e);
            this.Begin("Target", new object[] { "Name", e.Target.Name, "Container", e.Target.EntityContainer.Name });
            this.Dump(e.Target.ElementType, "TargetElementType");
            this.End("Target");
            this.End(e);
        }

        public override void Visit(DbSkipExpression e)
        {
            this.Begin(e);
            this.Dump(e.Input, "Input");
            this.Dump(e.SortOrder);
            this.Dump(e.Count, "Count");
            this.End(e);
        }

        public override void Visit(DbSortExpression e)
        {
            this.Begin(e);
            this.Dump(e.Input, "Input");
            this.Dump(e.SortOrder);
            this.End(e);
        }

        public override void Visit(DbTreatExpression e)
        {
            this.BeginUnary(e);
            this.End(e);
        }

        public override void Visit(DbUnionAllExpression e)
        {
            this.BeginBinary(e);
            this.End(e);
        }

        public override void Visit(DbVariableReferenceExpression e)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object> {
                { 
                    "VariableName",
                    e.VariableName
                }
            };
            this.Begin(e, attrs);
            this.End(e);
        }
    }
}

