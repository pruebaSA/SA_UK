namespace System.Data.SqlClient.SqlGen
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Metadata.Edm;
    using System.Globalization;

    internal class Sql8ExpressionRewriter : ExpressionCopier
    {
        private Sql8ExpressionRewriter(DbQueryCommandTree newTree) : base(newTree, ExpressionCopier.MetadataMapper.IdentityMapper)
        {
        }

        private static bool AreMatching(DbPropertyExpression expr1, DbPropertyExpression expr2, string expr1BindingVariableName, string expr2BindingVariableName)
        {
            if (expr1.Property.Name != expr2.Property.Name)
            {
                return false;
            }
            if (expr1.Instance.ExpressionKind != expr2.Instance.ExpressionKind)
            {
                return false;
            }
            if (expr1.Instance.ExpressionKind == DbExpressionKind.Property)
            {
                return AreMatching((DbPropertyExpression) expr1.Instance, (DbPropertyExpression) expr2.Instance, expr1BindingVariableName, expr2BindingVariableName);
            }
            DbVariableReferenceExpression instance = (DbVariableReferenceExpression) expr1.Instance;
            DbVariableReferenceExpression expression2 = (DbVariableReferenceExpression) expr2.Instance;
            return (string.Equals(instance.VariableName, expr1BindingVariableName, StringComparison.Ordinal) && string.Equals(expression2.VariableName, expr2BindingVariableName, StringComparison.Ordinal));
        }

        private DbExpressionBinding CapWithProject(DbExpressionBinding inputBinding, IList<DbPropertyExpression> flattenedProperties)
        {
            List<KeyValuePair<string, DbExpression>> recordColumns = new List<KeyValuePair<string, DbExpression>>(flattenedProperties.Count);
            Dictionary<string, int> dictionary = new Dictionary<string, int>(flattenedProperties.Count);
            foreach (DbPropertyExpression expression in flattenedProperties)
            {
                int num;
                string name = expression.Property.Name;
                if (dictionary.TryGetValue(name, out num))
                {
                    string str2;
                    do
                    {
                        num++;
                        str2 = name + num.ToString(CultureInfo.InvariantCulture);
                    }
                    while (dictionary.ContainsKey(str2));
                    dictionary[name] = num;
                    name = str2;
                }
                dictionary[name] = 0;
                recordColumns.Add(new KeyValuePair<string, DbExpression>(name, expression));
            }
            DbExpression projection = base.CommandTree.CreateNewRowExpression(recordColumns);
            DbProjectExpression input = base.CommandTree.CreateProjectExpression(inputBinding, projection);
            DbExpressionBinding binding = base.CommandTree.CreateExpressionBinding(input);
            flattenedProperties.Clear();
            RowType edmType = (RowType) projection.ResultType.EdmType;
            foreach (KeyValuePair<string, DbExpression> pair in recordColumns)
            {
                EdmProperty propertyInfo = edmType.Properties[pair.Key];
                flattenedProperties.Add(base.CommandTree.CreatePropertyExpression(propertyInfo, base.CommandTree.CreateVariableReferenceExpression(binding.VariableName, binding.VariableType)));
            }
            return binding;
        }

        private void FlattenProperties(DbExpression input, IList<DbPropertyExpression> flattenedProperties)
        {
            IList<EdmProperty> properties = TypeHelpers.GetProperties(input.ResultType);
            for (int i = 0; i < properties.Count; i++)
            {
                DbExpression instance = (i == 0) ? input : input.Clone();
                DbPropertyExpression item = base.CommandTree.CreatePropertyExpression(properties[i], instance);
                if (TypeSemantics.IsPrimitiveType(properties[i].TypeUsage))
                {
                    flattenedProperties.Add(item);
                }
                else
                {
                    this.FlattenProperties(item, flattenedProperties);
                }
            }
        }

        private static bool HasMatchInList(DbPropertyExpression expr, IList<DbPropertyExpression> list, string exprBindingVariableName, string listExpressionsBindingVariableName)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (AreMatching(expr, list[i], exprBindingVariableName, listExpressionsBindingVariableName))
                {
                    list.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        private static bool RemoveNonSortProperties(IList<DbPropertyExpression> list1, IList<DbPropertyExpression> list2, IList<DbPropertyExpression> sortList, string list1BindingVariableName, string sortExpressionsBindingVariableName)
        {
            bool flag = false;
            for (int i = list1.Count - 1; i >= 0; i--)
            {
                if (!HasMatchInList(list1[i], sortList, list1BindingVariableName, sortExpressionsBindingVariableName))
                {
                    list1.RemoveAt(i);
                    list2.RemoveAt(i);
                    flag = true;
                }
            }
            return flag;
        }

        internal static DbQueryCommandTree Rewrite(DbQueryCommandTree originalTree)
        {
            DbQueryCommandTree tree = new DbQueryCommandTree(originalTree.MetadataWorkspace, originalTree.DataSpace);
            originalTree.CopyParametersTo(tree);
            tree.Query = new Sql8ExpressionRewriter(tree).VisitExpr(originalTree.Query);
            return tree;
        }

        private DbExpression TransformIntersectOrExcept(DbExpression left, DbExpression right, DbExpressionKind expressionKind) => 
            this.TransformIntersectOrExcept(left, right, expressionKind, null, null);

        private DbExpression TransformIntersectOrExcept(DbExpression left, DbExpression right, DbExpressionKind expressionKind, IList<DbPropertyExpression> sortExpressionsOverLeft, string sortExpressionsBindingVariableName)
        {
            DbExpression expression8;
            bool flag = (expressionKind == DbExpressionKind.Except) || (expressionKind == DbExpressionKind.Skip);
            bool flag2 = (expressionKind == DbExpressionKind.Except) || (expressionKind == DbExpressionKind.Intersect);
            DbExpressionBinding input = base.CommandTree.CreateExpressionBinding(left);
            DbExpressionBinding inputBinding = base.CommandTree.CreateExpressionBinding(right);
            IList<DbPropertyExpression> flattenedProperties = new List<DbPropertyExpression>();
            IList<DbPropertyExpression> list2 = new List<DbPropertyExpression>();
            this.FlattenProperties(input.Variable, flattenedProperties);
            this.FlattenProperties(inputBinding.Variable, list2);
            if ((expressionKind == DbExpressionKind.Skip) && RemoveNonSortProperties(flattenedProperties, list2, sortExpressionsOverLeft, input.VariableName, sortExpressionsBindingVariableName))
            {
                inputBinding = this.CapWithProject(inputBinding, list2);
            }
            DbExpression expression = null;
            for (int i = 0; i < flattenedProperties.Count; i++)
            {
                DbExpression expression2 = base.CommandTree.CreateEqualsExpression(flattenedProperties[i], list2[i]);
                DbExpression expression3 = base.CommandTree.CreateIsNullExpression(flattenedProperties[i].Clone());
                DbExpression expression4 = base.CommandTree.CreateIsNullExpression(list2[i].Clone());
                DbExpression expression5 = base.CommandTree.CreateAndExpression(expression3, expression4);
                DbExpression expression6 = base.CommandTree.CreateOrExpression(expression2, expression5);
                if (i == 0)
                {
                    expression = expression6;
                }
                else
                {
                    expression = base.CommandTree.CreateAndExpression(expression, expression6);
                }
            }
            DbExpression argument = base.CommandTree.CreateAnyExpression(inputBinding, expression);
            if (flag)
            {
                expression8 = base.CommandTree.CreateNotExpression(argument);
            }
            else
            {
                expression8 = argument;
            }
            DbExpression expression9 = base.CommandTree.CreateFilterExpression(input, expression8);
            if (flag2)
            {
                expression9 = base.CommandTree.CreateDistinctExpression(expression9);
            }
            return expression9;
        }

        public override DbExpression Visit(DbExceptExpression e) => 
            this.TransformIntersectOrExcept(base.VisitExpr(e.Left), base.VisitExpr(e.Right), DbExpressionKind.Except);

        public override DbExpression Visit(DbIntersectExpression e) => 
            this.TransformIntersectOrExcept(base.VisitExpr(e.Left), base.VisitExpr(e.Right), DbExpressionKind.Intersect);

        public override DbExpression Visit(DbSkipExpression e)
        {
            DbExpression right = base.CommandTree.CreateLimitExpression(base.CommandTree.CreateSortExpression(base.VisitBinding(e.Input), base.VisitSortOrder(e.SortOrder)), base.VisitExpr(e.Count));
            DbExpression left = base.VisitExpr(e.Input.Expression);
            List<DbSortClause> sortOrder = base.VisitSortOrder(e.SortOrder);
            IList<DbPropertyExpression> sortExpressionsOverLeft = new List<DbPropertyExpression>(e.SortOrder.Count);
            foreach (DbSortClause clause in sortOrder)
            {
                if (clause.Expression.ExpressionKind == DbExpressionKind.Property)
                {
                    sortExpressionsOverLeft.Add((DbPropertyExpression) clause.Expression);
                }
            }
            DbExpression input = this.TransformIntersectOrExcept(left, right, DbExpressionKind.Skip, sortExpressionsOverLeft, e.Input.VariableName);
            return base.CommandTree.CreateSortExpression(base.CommandTree.CreateExpressionBinding(input, e.Input.VariableName), sortOrder);
        }
    }
}

