namespace System.Data.Common.CommandTrees.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.Text;

    internal class ExpressionPrinter : TreePrinter
    {
        private PrinterVisitor _visitor = new PrinterVisitor();

        internal ExpressionPrinter()
        {
        }

        private static TreeNode CreateParametersNode(DbCommandTree tree)
        {
            TreeNode node = new TreeNode("Parameters", new TreeNode[0]);
            foreach (KeyValuePair<string, TypeUsage> pair in tree.Parameters)
            {
                TreeNode node2 = new TreeNode(pair.Key, new TreeNode[0]);
                PrinterVisitor.AppendTypeSpecifier(node2, pair.Value);
                node.Children.Add(node2);
            }
            return node;
        }

        internal string Print(DbDeleteCommandTree tree)
        {
            TreeNode node;
            TreeNode node2;
            if (tree.Target != null)
            {
                node = this._visitor.VisitBinding("Target", tree.Target);
            }
            else
            {
                node = new TreeNode("Target", new TreeNode[0]);
            }
            if (tree.Predicate != null)
            {
                node2 = this._visitor.VisitExpression("Predicate", tree.Predicate);
            }
            else
            {
                node2 = new TreeNode("Predicate", new TreeNode[0]);
            }
            return this.Print(new TreeNode("DbDeleteCommandTree", new TreeNode[] { CreateParametersNode(tree), node, node2 }));
        }

        internal string Print(DbExpression expr) => 
            this.Print(this._visitor.VisitExpression(expr));

        internal string Print(DbFunctionCommandTree tree)
        {
            TreeNode node = new TreeNode("EdmFunction", new TreeNode[0]);
            if (tree.EdmFunction != null)
            {
                node.Children.Add(this._visitor.VisitFunction(tree.EdmFunction, null));
            }
            TreeNode node2 = new TreeNode("ResultType", new TreeNode[0]);
            if (tree.ResultType != null)
            {
                PrinterVisitor.AppendTypeSpecifier(node2, tree.ResultType);
            }
            return this.Print(new TreeNode("DbFunctionCommandTree", new TreeNode[] { CreateParametersNode(tree), node, node2 }));
        }

        internal string Print(DbInsertCommandTree tree)
        {
            TreeNode node = null;
            if (tree.Target != null)
            {
                node = this._visitor.VisitBinding("Target", tree.Target);
            }
            else
            {
                node = new TreeNode("Target", new TreeNode[0]);
            }
            TreeNode node2 = new TreeNode("SetClauses", new TreeNode[0]);
            foreach (DbModificationClause clause in tree.SetClauses)
            {
                if (clause != null)
                {
                    node2.Children.Add(clause.Print(this._visitor));
                }
            }
            TreeNode node3 = null;
            if (tree.Returning != null)
            {
                node3 = new TreeNode("Returning", new TreeNode[] { this._visitor.VisitExpression(tree.Returning) });
            }
            else
            {
                node3 = new TreeNode("Returning", new TreeNode[0]);
            }
            return this.Print(new TreeNode("DbInsertCommandTree", new TreeNode[] { CreateParametersNode(tree), node, node2, node3 }));
        }

        internal string Print(DbQueryCommandTree tree)
        {
            TreeNode node = new TreeNode("Query", new TreeNode[0]);
            if (tree.Query != null)
            {
                PrinterVisitor.AppendTypeSpecifier(node, tree.Query.ResultType);
                node.Children.Add(this._visitor.VisitExpression(tree.Query));
            }
            return this.Print(new TreeNode("DbQueryCommandTree", new TreeNode[] { CreateParametersNode(tree), node }));
        }

        internal string Print(DbUpdateCommandTree tree)
        {
            TreeNode node = null;
            TreeNode node3;
            TreeNode node4;
            if (tree.Target != null)
            {
                node = this._visitor.VisitBinding("Target", tree.Target);
            }
            else
            {
                node = new TreeNode("Target", new TreeNode[0]);
            }
            TreeNode node2 = new TreeNode("SetClauses", new TreeNode[0]);
            foreach (DbModificationClause clause in tree.SetClauses)
            {
                if (clause != null)
                {
                    node2.Children.Add(clause.Print(this._visitor));
                }
            }
            if (tree.Predicate != null)
            {
                node3 = new TreeNode("Predicate", new TreeNode[] { this._visitor.VisitExpression(tree.Predicate) });
            }
            else
            {
                node3 = new TreeNode("Predicate", new TreeNode[0]);
            }
            if (tree.Returning != null)
            {
                node4 = new TreeNode("Returning", new TreeNode[] { this._visitor.VisitExpression(tree.Returning) });
            }
            else
            {
                node4 = new TreeNode("Returning", new TreeNode[0]);
            }
            return this.Print(new TreeNode("DbUpdateCommandTree", new TreeNode[] { CreateParametersNode(tree), node, node2, node3, node4 }));
        }

        private class PrinterVisitor : DbExpressionVisitor<TreeNode>
        {
            private bool _infix = true;
            private int _maxStringLength = 80;
            private static Dictionary<DbExpressionKind, string> _opMap = InitializeOpMap();

            private void AppendArguments(TreeNode node, IList<FunctionParameter> paramInfos, IList<DbExpression> args)
            {
                if (paramInfos.Count > 0)
                {
                    node.Children.Add(new TreeNode("Arguments", this.VisitParams(paramInfos, args)));
                }
            }

            private static void AppendFullName(StringBuilder text, EdmType type)
            {
                if ((BuiltInTypeKind.RowType != type.BuiltInTypeKind) && !string.IsNullOrEmpty(type.NamespaceName))
                {
                    text.Append(type.NamespaceName);
                    text.Append(".");
                }
                text.Append(type.Name);
            }

            private static void AppendParameters(TreeNode node, IList<FunctionParameter> paramInfos)
            {
                node.Text.Append("(");
                for (int i = 0; i < paramInfos.Count; i++)
                {
                    FunctionParameter parameter = paramInfos[i];
                    AppendType(node, parameter.TypeUsage);
                    node.Text.Append(" ");
                    node.Text.Append(parameter.Name);
                    if (i < (paramInfos.Count - 1))
                    {
                        node.Text.Append(", ");
                    }
                }
                node.Text.Append(")");
            }

            internal static void AppendType(TreeNode node, TypeUsage type)
            {
                BuildTypeName(node.Text, type);
            }

            internal static void AppendTypeSpecifier(TreeNode node, TypeUsage type)
            {
                node.Text.Append(" : ");
                AppendType(node, type);
            }

            private static void BuildTypeName(StringBuilder text, TypeUsage type)
            {
                RowType edmType = type.EdmType as RowType;
                CollectionType type3 = type.EdmType as CollectionType;
                RefType type4 = type.EdmType as RefType;
                if (TypeSemantics.IsPrimitiveType(type))
                {
                    text.Append(type);
                }
                else if (type3 != null)
                {
                    text.Append("Collection{");
                    BuildTypeName(text, type3.TypeUsage);
                    text.Append("}");
                }
                else if (type4 != null)
                {
                    text.Append("Ref<");
                    AppendFullName(text, type4.ElementType);
                    text.Append(">");
                }
                else if (edmType != null)
                {
                    text.Append("Record[");
                    int num = 0;
                    foreach (EdmProperty property in edmType.Properties)
                    {
                        text.Append("'");
                        text.Append(property.Name);
                        text.Append("'");
                        text.Append("=");
                        BuildTypeName(text, property.TypeUsage);
                        num++;
                        if (num < edmType.Properties.Count)
                        {
                            text.Append(", ");
                        }
                    }
                    text.Append("]");
                }
                else
                {
                    if (!string.IsNullOrEmpty(type.EdmType.NamespaceName))
                    {
                        text.Append(type.EdmType.NamespaceName);
                        text.Append(".");
                    }
                    text.Append(type.EdmType.Name);
                }
            }

            private TreeNode CreateNavigationNode(RelationshipEndMember fromEnd, RelationshipEndMember toEnd)
            {
                TreeNode node = new TreeNode();
                node.Text.Append("Navigation : ");
                node.Text.Append(fromEnd.Name);
                node.Text.Append(" -> ");
                node.Text.Append(toEnd.Name);
                return node;
            }

            private TreeNode CreateRelationshipNode(RelationshipType relType)
            {
                TreeNode node = new TreeNode("Relationship", new TreeNode[0]);
                node.Text.Append(" : ");
                AppendFullName(node.Text, relType);
                return node;
            }

            private static Dictionary<DbExpressionKind, string> InitializeOpMap() => 
                new Dictionary<DbExpressionKind, string>(12) { 
                    [DbExpressionKind.Divide] = "/",
                    [DbExpressionKind.Modulo] = "%",
                    [DbExpressionKind.Multiply] = "*",
                    [DbExpressionKind.Plus] = "+",
                    [DbExpressionKind.Minus] = "-",
                    [DbExpressionKind.UnaryMinus] = "-",
                    [DbExpressionKind.Equals] = "=",
                    [DbExpressionKind.LessThan] = "<",
                    [DbExpressionKind.LessThanOrEquals] = "<=",
                    [DbExpressionKind.GreaterThan] = ">",
                    [DbExpressionKind.GreaterThanOrEquals] = ">=",
                    [DbExpressionKind.NotEquals] = "<>"
                };

            private static TreeNode NodeFromExpression(DbExpression expr) => 
                new TreeNode(Enum.GetName(typeof(DbExpressionKind), expr.ExpressionKind), new TreeNode[0]);

            public override TreeNode Visit(DbAndExpression e) => 
                this.VisitInfix(e, e.Left, "And", e.Right);

            public override TreeNode Visit(DbApplyExpression e)
            {
                TreeNode node = NodeFromExpression(e);
                node.Children.Add(this.VisitBinding("Input", e.Input));
                node.Children.Add(this.VisitBinding("Apply", e.Apply));
                return node;
            }

            public override TreeNode Visit(DbArithmeticExpression e)
            {
                if (DbExpressionKind.UnaryMinus == e.ExpressionKind)
                {
                    return this.Visit(_opMap[e.ExpressionKind], new DbExpression[] { e.Arguments[0] });
                }
                return this.VisitInfix(e, e.Arguments[0], _opMap[e.ExpressionKind], e.Arguments[1]);
            }

            public override TreeNode Visit(DbCaseExpression e)
            {
                TreeNode node = new TreeNode("Case", new TreeNode[0]);
                for (int i = 0; i < e.When.Count; i++)
                {
                    node.Children.Add(this.Visit("When", new DbExpression[] { e.When[i] }));
                    node.Children.Add(this.Visit("Then", new DbExpression[] { e.Then[i] }));
                }
                node.Children.Add(this.Visit("Else", new DbExpression[] { e.Else }));
                return node;
            }

            public override TreeNode Visit(DbCastExpression e) => 
                this.VisitCastOrTreat("Cast", e);

            public override TreeNode Visit(DbComparisonExpression e) => 
                this.VisitInfix(e, e.Left, _opMap[e.ExpressionKind], e.Right);

            public override TreeNode Visit(DbConstantExpression e)
            {
                TreeNode node = new TreeNode();
                string str = e.Value as string;
                if (str != null)
                {
                    str = str.Replace("\r\n", @"\r\n");
                    int length = str.Length;
                    if (this._maxStringLength > 0)
                    {
                        length = Math.Min(str.Length, this._maxStringLength);
                    }
                    node.Text.Append("'");
                    node.Text.Append(str, 0, length);
                    if (str.Length > length)
                    {
                        node.Text.Append("...");
                    }
                    node.Text.Append("'");
                    return node;
                }
                node.Text.Append(e.Value.ToString());
                return node;
            }

            public override TreeNode Visit(DbCrossJoinExpression e)
            {
                TreeNode node = NodeFromExpression(e);
                node.Children.Add(this.VisitBindingList("Inputs", e.Inputs));
                return node;
            }

            public override TreeNode Visit(DbDerefExpression e) => 
                this.VisitUnary(e);

            public override TreeNode Visit(DbDistinctExpression e) => 
                this.VisitUnary(e);

            public override TreeNode Visit(DbElementExpression e) => 
                this.VisitUnary(e, true);

            public override TreeNode Visit(DbEntityRefExpression e) => 
                this.VisitUnary(e, true);

            public override TreeNode Visit(DbExceptExpression e) => 
                this.VisitBinary(e);

            public override TreeNode Visit(DbExpression e)
            {
                throw EntityUtil.NotSupported(Strings.Cqt_General_UnsupportedExpression(e.GetType().FullName));
            }

            public override TreeNode Visit(DbFilterExpression e)
            {
                TreeNode node = NodeFromExpression(e);
                node.Children.Add(this.VisitBinding("Input", e.Input));
                node.Children.Add(this.Visit("Predicate", new DbExpression[] { e.Predicate }));
                return node;
            }

            public override TreeNode Visit(DbFunctionExpression e)
            {
                TreeNode parentNode = this.VisitFunction(e.Function, e.Arguments);
                if (e.LambdaBody != null)
                {
                    this.VisitLambdaBody(e.LambdaBody, parentNode);
                }
                return parentNode;
            }

            public override TreeNode Visit(DbGroupByExpression e)
            {
                List<TreeNode> children = new List<TreeNode>();
                List<TreeNode> list2 = new List<TreeNode>();
                RowType edmType = TypeHelpers.GetEdmType<RowType>(TypeHelpers.GetEdmType<CollectionType>(e.ResultType).TypeUsage);
                int num = 0;
                for (int i = 0; i < e.Keys.Count; i++)
                {
                    children.Add(this.VisitWithLabel("Key", edmType.Properties[i].Name, e.Keys[num]));
                    num++;
                }
                int num3 = 0;
                for (int j = e.Keys.Count; j < edmType.Properties.Count; j++)
                {
                    TreeNode item = new TreeNode("Aggregate : '", new TreeNode[0]);
                    item.Text.Append(edmType.Properties[j].Name);
                    item.Text.Append("'");
                    DbFunctionAggregate aggregate = e.Aggregates[num3] as DbFunctionAggregate;
                    TreeNode node2 = this.VisitFunction(aggregate.Function, aggregate.Arguments);
                    if (aggregate.Distinct)
                    {
                        node2 = new TreeNode("Distinct", new TreeNode[] { node2 });
                    }
                    item.Children.Add(node2);
                    list2.Add(item);
                    num3++;
                }
                TreeNode node3 = NodeFromExpression(e);
                node3.Children.Add(this.VisitGroupBinding(e.Input));
                if (children.Count > 0)
                {
                    node3.Children.Add(new TreeNode("Keys", children));
                }
                if (list2.Count > 0)
                {
                    node3.Children.Add(new TreeNode("Aggregates", list2));
                }
                return node3;
            }

            public override TreeNode Visit(DbIntersectExpression e) => 
                this.VisitBinary(e);

            public override TreeNode Visit(DbIsEmptyExpression e) => 
                this.VisitUnary(e);

            public override TreeNode Visit(DbIsNullExpression e) => 
                this.VisitUnary(e);

            public override TreeNode Visit(DbIsOfExpression e)
            {
                TreeNode node = new TreeNode();
                if (DbExpressionKind.IsOfOnly == e.ExpressionKind)
                {
                    node.Text.Append("IsOfOnly");
                }
                else
                {
                    node.Text.Append("IsOf");
                }
                AppendTypeSpecifier(node, e.OfType);
                node.Children.Add(this.VisitExpression(e.Argument));
                return node;
            }

            public override TreeNode Visit(DbJoinExpression e)
            {
                TreeNode node = NodeFromExpression(e);
                node.Children.Add(this.VisitBinding("Left", e.Left));
                node.Children.Add(this.VisitBinding("Right", e.Right));
                node.Children.Add(this.Visit("JoinCondition", new DbExpression[] { e.JoinCondition }));
                return node;
            }

            public override TreeNode Visit(DbLikeExpression e) => 
                this.Visit("Like", new DbExpression[] { e.Argument, e.Pattern, e.Escape });

            public override TreeNode Visit(DbLimitExpression e) => 
                this.Visit(e.WithTies ? "LimitWithTies" : "Limit", new DbExpression[] { e.Argument, e.Limit });

            public override TreeNode Visit(DbNewInstanceExpression e)
            {
                TreeNode node = NodeFromExpression(e);
                AppendTypeSpecifier(node, e.ResultType);
                if (BuiltInTypeKind.CollectionType == e.ResultType.EdmType.BuiltInTypeKind)
                {
                    foreach (DbExpression expression in e.Arguments)
                    {
                        node.Children.Add(this.VisitExpression(expression));
                    }
                    return node;
                }
                string label = (BuiltInTypeKind.RowType == e.ResultType.EdmType.BuiltInTypeKind) ? "Column" : "Property";
                IList<EdmProperty> properties = TypeHelpers.GetProperties(e.ResultType);
                for (int i = 0; i < properties.Count; i++)
                {
                    node.Children.Add(this.VisitWithLabel(label, properties[i].Name, e.Arguments[i]));
                }
                if ((BuiltInTypeKind.EntityType == e.ResultType.EdmType.BuiltInTypeKind) && e.HasRelatedEntityReferences)
                {
                    TreeNode item = new TreeNode("RelatedEntityReferences", new TreeNode[0]);
                    foreach (DbRelatedEntityRef ref2 in e.RelatedEntityReferences)
                    {
                        TreeNode node3 = this.CreateNavigationNode(ref2.SourceEnd, ref2.TargetEnd);
                        node3.Children.Add(this.CreateRelationshipNode((RelationshipType) ref2.SourceEnd.DeclaringType));
                        node3.Children.Add(this.VisitExpression(ref2.TargetEntityReference));
                        item.Children.Add(node3);
                    }
                    node.Children.Add(item);
                }
                return node;
            }

            public override TreeNode Visit(DbNotExpression e) => 
                this.VisitUnary(e);

            public override TreeNode Visit(DbNullExpression e) => 
                new TreeNode("null", new TreeNode[0]);

            public override TreeNode Visit(DbOfTypeExpression e)
            {
                TreeNode node = new TreeNode((e.ExpressionKind == DbExpressionKind.OfTypeOnly) ? "OfTypeOnly" : "OfType", new TreeNode[0]);
                AppendTypeSpecifier(node, e.OfType);
                node.Children.Add(this.VisitExpression(e.Argument));
                return node;
            }

            public override TreeNode Visit(DbOrExpression e) => 
                this.VisitInfix(e, e.Left, "Or", e.Right);

            public override TreeNode Visit(DbParameterReferenceExpression e)
            {
                TreeNode node = new TreeNode();
                node.Text.AppendFormat("@{0}", e.ParameterName);
                return node;
            }

            public override TreeNode Visit(DbProjectExpression e)
            {
                TreeNode node = NodeFromExpression(e);
                node.Children.Add(this.VisitBinding("Input", e.Input));
                node.Children.Add(this.Visit("Projection", new DbExpression[] { e.Projection }));
                return node;
            }

            public override TreeNode Visit(DbPropertyExpression e)
            {
                TreeNode node = null;
                if (e.Instance != null)
                {
                    node = this.VisitExpression(e.Instance);
                    if ((e.Instance.ExpressionKind == DbExpressionKind.VariableReference) || ((e.Instance.ExpressionKind == DbExpressionKind.Property) && (node.Children.Count == 0)))
                    {
                        node.Text.Append(".");
                        node.Text.Append(e.Property.Name);
                        return node;
                    }
                }
                TreeNode node2 = new TreeNode(".", new TreeNode[0]);
                EdmProperty property = e.Property as EdmProperty;
                if ((property != null) && !(property.DeclaringType is RowType))
                {
                    AppendFullName(node2.Text, property.DeclaringType);
                    node2.Text.Append(".");
                }
                node2.Text.Append(e.Property.Name);
                if (node != null)
                {
                    node2.Children.Add(new TreeNode("Instance", new TreeNode[] { node }));
                }
                return node2;
            }

            public override TreeNode Visit(DbQuantifierExpression e)
            {
                TreeNode node = NodeFromExpression(e);
                node.Children.Add(this.VisitBinding("Input", e.Input));
                node.Children.Add(this.Visit("Predicate", new DbExpression[] { e.Predicate }));
                return node;
            }

            public override TreeNode Visit(DbRefExpression e)
            {
                TreeNode node = new TreeNode("Ref", new TreeNode[0]);
                node.Text.Append("<");
                AppendFullName(node.Text, TypeHelpers.GetEdmType<RefType>(e.ResultType).ElementType);
                node.Text.Append(">");
                TreeNode item = new TreeNode("EntitySet : ", new TreeNode[0]);
                item.Text.Append(e.EntitySet.EntityContainer.Name);
                item.Text.Append(".");
                item.Text.Append(e.EntitySet.Name);
                node.Children.Add(item);
                node.Children.Add(this.Visit("Keys", new DbExpression[] { e.Argument }));
                return node;
            }

            public override TreeNode Visit(DbRefKeyExpression e) => 
                this.VisitUnary(e, true);

            public override TreeNode Visit(DbRelationshipNavigationExpression e)
            {
                TreeNode node = NodeFromExpression(e);
                node.Children.Add(this.CreateRelationshipNode(e.Relationship));
                node.Children.Add(this.CreateNavigationNode(e.NavigateFrom, e.NavigateTo));
                node.Children.Add(this.Visit("Source", new DbExpression[] { e.NavigationSource }));
                return node;
            }

            public override TreeNode Visit(DbScanExpression e)
            {
                TreeNode node = NodeFromExpression(e);
                node.Text.Append(" : ");
                node.Text.Append(e.Target.EntityContainer.Name);
                node.Text.Append(".");
                node.Text.Append(e.Target.Name);
                return node;
            }

            public override TreeNode Visit(DbSkipExpression e)
            {
                TreeNode node = NodeFromExpression(e);
                node.Children.Add(this.VisitBinding("Input", e.Input));
                node.Children.Add(this.VisitSortOrder(e.SortOrder));
                node.Children.Add(this.Visit("Count", new DbExpression[] { e.Count }));
                return node;
            }

            public override TreeNode Visit(DbSortExpression e)
            {
                TreeNode node = NodeFromExpression(e);
                node.Children.Add(this.VisitBinding("Input", e.Input));
                node.Children.Add(this.VisitSortOrder(e.SortOrder));
                return node;
            }

            public override TreeNode Visit(DbTreatExpression e) => 
                this.VisitCastOrTreat("Treat", e);

            public override TreeNode Visit(DbUnionAllExpression e) => 
                this.VisitBinary(e);

            public override TreeNode Visit(DbVariableReferenceExpression e)
            {
                TreeNode node = new TreeNode();
                node.Text.AppendFormat("Var({0})", e.VariableName);
                return node;
            }

            private TreeNode Visit(string name, params DbExpression[] exprs)
            {
                TreeNode node = new TreeNode(name, new TreeNode[0]);
                foreach (DbExpression expression in exprs)
                {
                    node.Children.Add(this.VisitExpression(expression));
                }
                return node;
            }

            private TreeNode VisitBinary(DbBinaryExpression expr)
            {
                TreeNode node = NodeFromExpression(expr);
                node.Children.Add(this.VisitExpression(expr.Left));
                node.Children.Add(this.VisitExpression(expr.Right));
                return node;
            }

            internal TreeNode VisitBinding(string propName, DbExpressionBinding binding) => 
                this.VisitWithLabel(propName, binding.VariableName, binding.Expression);

            private TreeNode VisitBindingList(string propName, IList<DbExpressionBinding> bindings)
            {
                List<TreeNode> children = new List<TreeNode>();
                for (int i = 0; i < bindings.Count; i++)
                {
                    children.Add(this.VisitBinding(CommandTreeUtils.FormatIndex(propName, i), bindings[i]));
                }
                return new TreeNode(propName, children);
            }

            private TreeNode VisitCastOrTreat(string op, DbUnaryExpression e)
            {
                TreeNode node = null;
                TreeNode node2 = this.VisitExpression(e.Argument);
                if (node2.Children.Count == 0)
                {
                    node2.Text.Insert(0, op);
                    node2.Text.Insert(op.Length, '(');
                    node2.Text.Append(" As ");
                    AppendType(node2, e.ResultType);
                    node2.Text.Append(")");
                    return node2;
                }
                node = new TreeNode(op, new TreeNode[0]);
                AppendTypeSpecifier(node, e.ResultType);
                node.Children.Add(node2);
                return node;
            }

            internal TreeNode VisitExpression(DbExpression expr) => 
                expr.Accept<TreeNode>(this);

            internal TreeNode VisitExpression(string name, DbExpression expr) => 
                new TreeNode(name, new TreeNode[] { expr.Accept<TreeNode>(this) });

            internal TreeNode VisitFunction(EdmFunction func, IList<DbExpression> args)
            {
                TreeNode node = new TreeNode();
                AppendFullName(node.Text, func);
                AppendParameters(node, func.Parameters);
                if (args != null)
                {
                    this.AppendArguments(node, func.Parameters, args);
                }
                return node;
            }

            private TreeNode VisitGroupBinding(DbGroupExpressionBinding groupBinding)
            {
                TreeNode node = this.VisitExpression(groupBinding.Expression);
                TreeNode node2 = new TreeNode {
                    Children = { node }
                };
                node2.Text.AppendFormat(CultureInfo.InvariantCulture, "Input : '{0}', '{1}'", new object[] { groupBinding.VariableName, groupBinding.GroupVariableName });
                return node2;
            }

            private TreeNode VisitInfix(DbExpression root, DbExpression left, string name, DbExpression right)
            {
                if (this._infix)
                {
                    return new TreeNode("", new TreeNode[0]) { Children = { 
                        this.VisitExpression(left),
                        new TreeNode(name, new TreeNode[0]),
                        this.VisitExpression(right)
                    } };
                }
                return this.Visit(name, new DbExpression[] { left, right });
            }

            private void VisitLambdaBody(DbExpression body, TreeNode parentNode)
            {
                parentNode.Children.Add(this.Visit("Body", new DbExpression[] { body }));
            }

            private List<TreeNode> VisitParams(IList<FunctionParameter> paramInfo, IList<DbExpression> args)
            {
                List<TreeNode> list = new List<TreeNode>();
                for (int i = 0; i < paramInfo.Count; i++)
                {
                    TreeNode item = new TreeNode(paramInfo[i].Name, new TreeNode[0]) {
                        Children = { this.VisitExpression(args[i]) }
                    };
                    list.Add(item);
                }
                return list;
            }

            private TreeNode VisitSortOrder(IList<DbSortClause> sortOrder)
            {
                TreeNode node = new TreeNode("SortOrder", new TreeNode[0]);
                foreach (DbSortClause clause in sortOrder)
                {
                    TreeNode item = this.Visit(clause.Ascending ? "Asc" : "Desc", new DbExpression[] { clause.Expression });
                    if (!string.IsNullOrEmpty(clause.Collation))
                    {
                        item.Text.Append(" : ");
                        item.Text.Append(clause.Collation);
                    }
                    node.Children.Add(item);
                }
                return node;
            }

            private TreeNode VisitUnary(DbUnaryExpression expr) => 
                this.VisitUnary(expr, false);

            private TreeNode VisitUnary(DbUnaryExpression expr, bool appendType)
            {
                TreeNode node = NodeFromExpression(expr);
                if (appendType)
                {
                    AppendTypeSpecifier(node, expr.ResultType);
                }
                node.Children.Add(this.VisitExpression(expr.Argument));
                return node;
            }

            private TreeNode VisitWithLabel(string label, string name, DbExpression def)
            {
                TreeNode node = new TreeNode(label, new TreeNode[0]);
                node.Text.Append(" : '");
                node.Text.Append(name);
                node.Text.Append("'");
                node.Children.Add(this.VisitExpression(def));
                return node;
            }
        }
    }
}

