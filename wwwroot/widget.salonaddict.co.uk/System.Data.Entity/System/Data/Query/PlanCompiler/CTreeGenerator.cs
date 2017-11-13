namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal class CTreeGenerator : BasicOpVisitorOfT<DbExpression>
    {
        private List<ParameterVar> _addedParams = new List<ParameterVar>();
        private AliasGenerator _applyAliases = new AliasGenerator("Apply");
        private List<ParameterVar> _availableParams = new List<ParameterVar>();
        private Stack<RelOpInfo> _bindingScopes = new Stack<RelOpInfo>();
        private DbCommandTree _commandTree;
        private AliasGenerator _distinctAliases = new AliasGenerator("Distinct");
        private AliasGenerator _elementAliases = new AliasGenerator("Element");
        private AliasGenerator _exceptAliases = new AliasGenerator("Except");
        private AliasGenerator _extentAliases = new AliasGenerator("Extent");
        private AliasGenerator _filterAliases = new AliasGenerator("Filter");
        private AliasGenerator _groupByAliases = new AliasGenerator("GroupBy");
        private AliasGenerator _intersectAliases = new AliasGenerator("Intersect");
        private Command _iqtCommand;
        private AliasGenerator _joinAliases = new AliasGenerator("Join");
        private AliasGenerator _limitAliases = new AliasGenerator("Limit");
        private AliasGenerator _projectAliases = new AliasGenerator("Project");
        private Dictionary<DbExpression, RelOpInfo> _relOpState = new Dictionary<DbExpression, RelOpInfo>();
        private AliasGenerator _singleRowTableAliases = new AliasGenerator("SingleRowTable");
        private AliasGenerator _skipAliases = new AliasGenerator("Skip");
        private AliasGenerator _sortAliases = new AliasGenerator("Sort");
        private AliasGenerator _unionAllAliases = new AliasGenerator("UnionAll");
        private Stack<VarDefScope> _varScopes = new Stack<VarDefScope>();

        private CTreeGenerator(Command itree, Node toConvert)
        {
            this._iqtCommand = itree;
            this._iqtCommand.GetParameters(this._availableParams);
            DbQueryCommandTree tree = new DbQueryCommandTree(itree.MetadataWorkspace, itree.DataSpace);
            this._commandTree = tree;
            tree.Query = base.VisitNode(toConvert);
        }

        private static void AssertBinary(Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(2 == n.Children.Count, string.Format(CultureInfo.InvariantCulture, "Non-Binary {0} encountered", new object[] { n.Op.GetType().Name }));
        }

        private void AssertRelOp(DbExpression expr)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(this._relOpState.ContainsKey(expr), "not a relOp expression?");
        }

        private RelOpInfo BuildEmptyProjection(Node relOpNode)
        {
            if (relOpNode.Op.OpType == OpType.Project)
            {
                relOpNode = relOpNode.Child0;
            }
            RelOpInfo scope = this.EnterExpressionBindingScope(relOpNode);
            DbExpression expression = this._commandTree.CreateTrueExpression();
            List<KeyValuePair<string, DbExpression>> recordColumns = new List<KeyValuePair<string, DbExpression>> {
                new KeyValuePair<string, DbExpression>("C0", expression)
            };
            DbExpression expr = this._commandTree.CreateProjectExpression(scope.CreateBinding(), this._commandTree.CreateNewRowExpression(recordColumns));
            this.PublishRelOp(this._projectAliases.Next(), expr, new VarInfoList());
            this.ExitExpressionBindingScope(scope);
            return this.ConsumeRelOp(expr);
        }

        private RelOpInfo BuildProjection(Node relOpNode, IEnumerable<Var> projectionVars)
        {
            DbExpression expr = null;
            ProjectOp op = relOpNode.Op as ProjectOp;
            if (op != null)
            {
                expr = this.VisitProject(op, relOpNode, projectionVars);
            }
            else
            {
                RelOpInfo sourceInfo = this.EnterExpressionBindingScope(relOpNode);
                expr = this.CreateProject(sourceInfo, projectionVars);
                this.ExitExpressionBindingScope(sourceInfo);
            }
            return this.ConsumeRelOp(expr);
        }

        private RelOpInfo ConsumeRelOp(DbExpression expr)
        {
            this.AssertRelOp(expr);
            RelOpInfo info = this._relOpState[expr];
            this._relOpState.Remove(expr);
            return info;
        }

        private DbExpression CreateLimitExpression(DbExpression argument, DbExpression limit, bool withTies)
        {
            if (withTies)
            {
                return this._commandTree.CreateLimitWithTiesExpression(argument, limit);
            }
            return this._commandTree.CreateLimitExpression(argument, limit);
        }

        private DbExpression CreateProject(RelOpInfo sourceInfo, IEnumerable<Var> outputVars)
        {
            VarInfoList publishedVars = new VarInfoList();
            List<KeyValuePair<string, DbExpression>> recordColumns = new List<KeyValuePair<string, DbExpression>>();
            AliasGenerator defaultAliasGenerator = new AliasGenerator("C");
            Dictionary<string, AliasGenerator> aliasMap = new Dictionary<string, AliasGenerator>(StringComparer.InvariantCultureIgnoreCase);
            Dictionary<string, string> alreadyUsedNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (Var var in outputVars)
            {
                string key = GenerateNameForVar(var, aliasMap, defaultAliasGenerator, alreadyUsedNames);
                DbExpression expression = this.ResolveVar(var);
                recordColumns.Add(new KeyValuePair<string, DbExpression>(key, expression));
                VarInfo item = new VarInfo(var);
                item.PrependProperty(key);
                publishedVars.Add(item);
            }
            DbExpression expr = this._commandTree.CreateProjectExpression(sourceInfo.CreateBinding(), this._commandTree.CreateNewRowExpression(recordColumns));
            this.PublishRelOp(this._projectAliases.Next(), expr, publishedVars);
            return expr;
        }

        private RelOpInfo EnterExpressionBindingScope(Node inputNode) => 
            this.EnterExpressionBindingScope(inputNode, true);

        private RelOpInfo EnterExpressionBindingScope(Node inputNode, bool pushScope)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(inputNode.Op is RelOp, "Non-RelOp used as DbExpressionBinding Input");
            DbExpression expr = base.VisitNode(inputNode);
            RelOpInfo inputState = this.ConsumeRelOp(expr);
            if (pushScope)
            {
                this.PushExpressionBindingScope(inputState);
            }
            return inputState;
        }

        private void EnterVarDefListScope(Node varDefListNode)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(varDefListNode.Op is VarDefListOp, "EnterVarDefListScope called with non-VarDefListOp");
            this.EnterVarDefScope(varDefListNode.Children);
        }

        private void EnterVarDefScope(List<Node> varDefNodes)
        {
            Dictionary<Var, DbExpression> definedVars = new Dictionary<Var, DbExpression>();
            foreach (Node node in varDefNodes)
            {
                VarDefOp op = node.Op as VarDefOp;
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(op != null, "VarDefListOp contained non-VarDefOp child node");
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(op.Var is ComputedVar, "VarDefOp defined non-Computed Var");
                definedVars.Add(op.Var, base.VisitNode(node.Child0));
            }
            this._varScopes.Push(new VarDefScope(definedVars));
        }

        private void ExitExpressionBindingScope(RelOpInfo scope)
        {
            this.ExitExpressionBindingScope(scope, true);
        }

        private void ExitExpressionBindingScope(RelOpInfo scope, bool wasPushed)
        {
            if (wasPushed)
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(this._bindingScopes.Count > 0, "ExitExpressionBindingScope called on empty ExpressionBindingScope stack");
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(this._bindingScopes.Pop() == scope, "ExitExpressionBindingScope called on incorrect expression");
            }
        }

        private void ExitVarDefScope()
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(this._varScopes.Count > 0, "ExitVarDefScope called on empty VarDefScope stack");
            this._varScopes.Pop();
        }

        internal static DbCommandTree Generate(Command itree, Node toConvert)
        {
            CTreeGenerator generator = new CTreeGenerator(itree, toConvert);
            return generator._commandTree;
        }

        private static string GenerateNameForVar(Var projectedVar, Dictionary<string, AliasGenerator> aliasMap, AliasGenerator defaultAliasGenerator, Dictionary<string, string> alreadyUsedNames)
        {
            string str;
            AliasGenerator generator;
            if (projectedVar.TryGetName(out str))
            {
                if (!aliasMap.TryGetValue(str, out generator))
                {
                    generator = new AliasGenerator(str);
                    aliasMap[str] = generator;
                }
                else
                {
                    str = generator.Next();
                }
            }
            else
            {
                generator = defaultAliasGenerator;
                str = generator.Next();
            }
            while (alreadyUsedNames.ContainsKey(str))
            {
                str = generator.Next();
            }
            alreadyUsedNames[str] = str;
            return str;
        }

        private static VarInfoList GetTableVars(Table targetTable)
        {
            VarInfoList list = new VarInfoList();
            if (targetTable.TableMetadata.Flattened)
            {
                for (int i = 0; i < targetTable.Columns.Count; i++)
                {
                    VarInfo item = new VarInfo(targetTable.Columns[i]);
                    item.PrependProperty(targetTable.TableMetadata.Columns[i].Name);
                    list.Add(item);
                }
                return list;
            }
            list.Add(new VarInfo(targetTable.Columns[0]));
            return list;
        }

        private RelOpInfo PublishRelOp(string name, DbExpression expr, VarInfoList publishedVars)
        {
            RelOpInfo info = new RelOpInfo(name, expr, publishedVars);
            this._relOpState.Add(expr, info);
            return info;
        }

        private void PushExpressionBindingScope(RelOpInfo inputState)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(((inputState != null) && (inputState.PublisherName != null)) && (inputState.PublishedVars != null), "Invalid RelOpInfo produced by DbExpressionBinding Input");
            this._bindingScopes.Push(inputState);
        }

        private DbExpression ResolveVar(Var referencedVar)
        {
            DbExpression resultExpr = null;
            ParameterVar item = referencedVar as ParameterVar;
            if (item != null)
            {
                if (!this._addedParams.Contains(item) && this._availableParams.Contains(item))
                {
                    this._commandTree.AddParameter(item.ParameterName, item.Type);
                    this._addedParams.Add(item);
                }
                resultExpr = this._commandTree.CreateParameterReferenceExpression(item.ParameterName);
            }
            else
            {
                ComputedVar targetVar = referencedVar as ComputedVar;
                if (((targetVar != null) && (this._varScopes.Count > 0)) && !this._varScopes.Peek().TryResolveVar(targetVar, out resultExpr))
                {
                    resultExpr = null;
                }
                if (resultExpr == null)
                {
                    DbExpression expression2 = null;
                    foreach (RelOpInfo info in this._bindingScopes)
                    {
                        if (info.TryResolveVar(referencedVar, out expression2))
                        {
                            resultExpr = expression2;
                            break;
                        }
                    }
                }
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(resultExpr != null, string.Format(CultureInfo.InvariantCulture, "Unresolvable Var used in Command: VarType={0}, Id={1}", new object[] { Enum.GetName(typeof(VarType), referencedVar.VarType), referencedVar.Id }));
            return resultExpr;
        }

        public override DbExpression Visit(AggregateOp op, Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "AggregateOp encountered outside of GroupByOp");
            throw EntityUtil.NotSupported(Strings.Iqt_CTGen_UnexpectedAggregate);
        }

        public override DbExpression Visit(ArithmeticOp op, Node n)
        {
            DbExpression expression = null;
            if (OpType.UnaryMinus == op.OpType)
            {
                expression = this._commandTree.CreateUnaryMinusExpression(this.VisitChild(n, 0));
            }
            else
            {
                DbExpression left = this.VisitChild(n, 0);
                DbExpression right = this.VisitChild(n, 1);
                switch (op.OpType)
                {
                    case OpType.Plus:
                        expression = this._commandTree.CreatePlusExpression(left, right);
                        goto Label_00AF;

                    case OpType.Minus:
                        expression = this._commandTree.CreateMinusExpression(left, right);
                        goto Label_00AF;

                    case OpType.Multiply:
                        expression = this._commandTree.CreateMultiplyExpression(left, right);
                        goto Label_00AF;

                    case OpType.Divide:
                        expression = this._commandTree.CreateDivideExpression(left, right);
                        goto Label_00AF;

                    case OpType.Modulo:
                        expression = this._commandTree.CreateModuloExpression(left, right);
                        goto Label_00AF;
                }
                expression = null;
            }
        Label_00AF:;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(expression != null, string.Format(CultureInfo.InvariantCulture, "ArithmeticOp OpType not recognized: {0}", new object[] { Enum.GetName(typeof(OpType), op.OpType) }));
            return expression;
        }

        public override DbExpression Visit(CaseOp op, Node n)
        {
            int count = n.Children.Count;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(count > 1, "Invalid CaseOp: At least 2 child Nodes (1 When/Then pair) must be present");
            List<DbExpression> whenExpressions = new List<DbExpression>();
            List<DbExpression> thenExpressions = new List<DbExpression>();
            DbExpression elseExpression = null;
            if ((n.Children.Count % 2) == 0)
            {
                elseExpression = this._commandTree.CreateNullExpression(op.Type);
            }
            else
            {
                count--;
                elseExpression = this.VisitChild(n, n.Children.Count - 1);
            }
            for (int i = 0; i < count; i += 2)
            {
                whenExpressions.Add(this.VisitChild(n, i));
                thenExpressions.Add(this.VisitChild(n, i + 1));
            }
            return this._commandTree.CreateCaseExpression(whenExpressions, thenExpressions, elseExpression);
        }

        public override DbExpression Visit(CastOp op, Node n) => 
            this._commandTree.CreateCastExpression(this.VisitChild(n, 0), op.Type);

        public override DbExpression Visit(CollectOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(ComparisonOp op, Node n)
        {
            AssertBinary(n);
            DbExpression left = this.VisitChild(n, 0);
            DbExpression right = this.VisitChild(n, 1);
            DbExpression expression3 = null;
            switch (op.OpType)
            {
                case OpType.GT:
                    expression3 = this._commandTree.CreateGreaterThanExpression(left, right);
                    break;

                case OpType.GE:
                    expression3 = this._commandTree.CreateGreaterThanOrEqualsExpression(left, right);
                    break;

                case OpType.LE:
                    expression3 = this._commandTree.CreateLessThanOrEqualsExpression(left, right);
                    break;

                case OpType.LT:
                    expression3 = this._commandTree.CreateLessThanExpression(left, right);
                    break;

                case OpType.EQ:
                    expression3 = this._commandTree.CreateEqualsExpression(left, right);
                    break;

                case OpType.NE:
                    expression3 = this._commandTree.CreateNotEqualsExpression(left, right);
                    break;

                default:
                    expression3 = null;
                    break;
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(expression3 != null, string.Format(CultureInfo.InvariantCulture, "ComparisonOp OpType not recognized: {0}", new object[] { Enum.GetName(typeof(OpType), op.OpType) }));
            return expression3;
        }

        public override DbExpression Visit(ConditionalOp op, Node n)
        {
            DbExpression left = this.VisitChild(n, 0);
            DbExpression argument = null;
            switch (op.OpType)
            {
                case OpType.And:
                    argument = this._commandTree.CreateAndExpression(left, this.VisitChild(n, 1));
                    break;

                case OpType.Or:
                    argument = this._commandTree.CreateOrExpression(left, this.VisitChild(n, 1));
                    break;

                case OpType.Not:
                {
                    DbNotExpression expression3 = left as DbNotExpression;
                    if (expression3 == null)
                    {
                        argument = this._commandTree.CreateNotExpression(left);
                        break;
                    }
                    argument = expression3.Argument;
                    break;
                }
                case OpType.IsNull:
                    argument = this._commandTree.CreateIsNullExpression(left);
                    break;

                default:
                    argument = null;
                    break;
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(argument != null, string.Format(CultureInfo.InvariantCulture, "ConditionalOp OpType not recognized: {0}", new object[] { Enum.GetName(typeof(OpType), op.OpType) }));
            return argument;
        }

        public override DbExpression Visit(ConstantOp op, Node n) => 
            this.VisitConstantOp(op, n);

        public override DbExpression Visit(ConstantPredicateOp op, Node n) => 
            this._commandTree.CreateEqualsExpression(this._commandTree.CreateTrueExpression(), op.IsTrue ? this._commandTree.CreateTrueExpression() : this._commandTree.CreateFalseExpression());

        public override DbExpression Visit(ConstrainedSortOp op, Node n)
        {
            DbExpression expr = null;
            RelOpInfo scope = null;
            string name = null;
            bool condition = OpType.Null == n.Child1.Op.OpType;
            bool flag2 = OpType.Null == n.Child2.Op.OpType;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(!condition || !flag2, "ConstrainedSortOp with no Skip Count and no Limit?");
            if (op.Keys.Count == 0)
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(condition, "ConstrainedSortOp without SortKeys cannot have Skip Count");
                DbExpression expression2 = base.VisitNode(n.Child0);
                scope = this.ConsumeRelOp(expression2);
                expr = this.CreateLimitExpression(expression2, base.VisitNode(n.Child2), op.WithTies);
                name = this._limitAliases.Next();
            }
            else
            {
                scope = this.EnterExpressionBindingScope(n.Child0);
                List<DbSortClause> sortOrder = this.VisitSortKeys(op.Keys);
                this.ExitExpressionBindingScope(scope);
                if (!condition && !flag2)
                {
                    expr = this.CreateLimitExpression(this._commandTree.CreateSkipExpression(scope.CreateBinding(), sortOrder, this.VisitChild(n, 1)), this.VisitChild(n, 2), op.WithTies);
                    name = this._limitAliases.Next();
                }
                else if (!condition && flag2)
                {
                    expr = this._commandTree.CreateSkipExpression(scope.CreateBinding(), sortOrder, this.VisitChild(n, 1));
                    name = this._skipAliases.Next();
                }
                else if (condition && !flag2)
                {
                    expr = this.CreateLimitExpression(this._commandTree.CreateSortExpression(scope.CreateBinding(), sortOrder), this.VisitChild(n, 2), op.WithTies);
                    name = this._limitAliases.Next();
                }
            }
            this.PublishRelOp(name, expr, scope.PublishedVars);
            return expr;
        }

        public override DbExpression Visit(CrossApplyOp op, Node n) => 
            this.VisitApply(n, DbExpressionKind.CrossApply);

        public override DbExpression Visit(CrossJoinOp op, Node n)
        {
            List<DbExpressionBinding> inputs = new List<DbExpressionBinding>();
            VarInfoList publishedVars = new VarInfoList();
            foreach (Node node in n.Children)
            {
                RelOpInfo scope = this.VisitJoinInput(node);
                inputs.Add(scope.CreateBinding());
                this.ExitExpressionBindingScope(scope, false);
                scope.PublishedVars.PrependProperty(scope.PublisherName);
                publishedVars.AddRange(scope.PublishedVars);
            }
            DbExpression expr = this._commandTree.CreateCrossJoinExpression(inputs);
            this.PublishRelOp(this._joinAliases.Next(), expr, publishedVars);
            return expr;
        }

        public override DbExpression Visit(DerefOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(DiscriminatedNewEntityOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(DistinctOp op, Node n)
        {
            RelOpInfo info = this.BuildProjection(n.Child0, op.Keys);
            DbExpression expr = this._commandTree.CreateDistinctExpression(info.Publisher);
            this.PublishRelOp(this._distinctAliases.Next(), expr, info.PublishedVars);
            return expr;
        }

        public override DbExpression Visit(ElementOp op, Node n)
        {
            DbExpression expr = base.VisitNode(n.Child0);
            this.AssertRelOp(expr);
            this.ConsumeRelOp(expr);
            return this._commandTree.CreateElementExpressionUnwrapSingleProperty(expr);
        }

        public override DbExpression Visit(ExceptOp op, Node n)
        {
            VarInfoList publishedVars = new VarInfoList();
            DbExpression[] expressionArray = this.VisitSetOp(op, n, publishedVars);
            DbExpression expr = this._commandTree.CreateExceptExpression(expressionArray[0], expressionArray[1]);
            this.PublishRelOp(this._exceptAliases.Next(), expr, publishedVars);
            return expr;
        }

        public override DbExpression Visit(ExistsOp op, Node n)
        {
            DbExpression expr = base.VisitNode(n.Child0);
            this.ConsumeRelOp(expr);
            return this._commandTree.CreateNotExpression(this._commandTree.CreateIsEmptyExpression(expr));
        }

        public override DbExpression Visit(FilterOp op, Node n)
        {
            RelOpInfo scope = this.EnterExpressionBindingScope(n.Child0);
            DbExpression predicate = base.VisitNode(n.Child1);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsPrimitiveType(predicate.ResultType, PrimitiveTypeKind.Boolean), "Invalid FilterOp Predicate (non-ScalarOp or non-Boolean result)");
            DbExpression expr = this._commandTree.CreateFilterExpression(scope.CreateBinding(), predicate);
            this.ExitExpressionBindingScope(scope);
            this.PublishRelOp(this._filterAliases.Next(), expr, scope.PublishedVars);
            return expr;
        }

        public override DbExpression Visit(FullOuterJoinOp op, Node n) => 
            this.VisitBinaryJoin(n, DbExpressionKind.FullOuterJoin);

        public override DbExpression Visit(FunctionOp op, Node n) => 
            this._commandTree.CreateFunctionExpression(op.Function, this.VisitChildren(n));

        public override DbExpression Visit(GetEntityRefOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(GetRefKeyOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(GroupByOp op, Node n)
        {
            VarInfoList publishedVars = new VarInfoList();
            RelOpInfo scope = this.EnterExpressionBindingScope(n.Child0);
            this.EnterVarDefListScope(n.Child1);
            AliasGenerator generator = new AliasGenerator("K");
            List<KeyValuePair<string, DbExpression>> keys = new List<KeyValuePair<string, DbExpression>>();
            List<Var> list3 = new List<Var>(op.Outputs);
            foreach (Var var in op.Keys)
            {
                string key = generator.Next();
                keys.Add(new KeyValuePair<string, DbExpression>(key, this.ResolveVar(var)));
                VarInfo item = new VarInfo(var);
                item.PrependProperty(key);
                publishedVars.Add(item);
                list3.Remove(var);
            }
            this.ExitVarDefScope();
            string publisherName = scope.PublisherName;
            string groupVarName = string.Format(CultureInfo.InvariantCulture, "{0}Group", new object[] { publisherName });
            scope.PublisherName = groupVarName;
            Dictionary<Var, DbAggregate> dictionary = new Dictionary<Var, DbAggregate>();
            Node node = n.Child2;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(node.Op is VarDefListOp, "Invalid Aggregates VarDefListOp Node encountered in GroupByOp");
            foreach (Node node2 in node.Children)
            {
                DbFunctionAggregate aggregate;
                VarDefOp op2 = node2.Op as VarDefOp;
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(op2 != null, "Non-VarDefOp Node encountered as child of Aggregates VarDefListOp Node");
                Var var2 = op2.Var;
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(var2 is ComputedVar, "Non-ComputedVar encountered in Aggregate VarDefOp");
                Node node3 = node2.Child0;
                DbExpression argument = base.VisitNode(node3.Child0);
                AggregateOp op3 = node3.Op as AggregateOp;
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(op3 != null, "Non-Aggregate Node encountered as child of Aggregate VarDefOp Node");
                if (op3.IsDistinctAggregate)
                {
                    aggregate = this._commandTree.CreateDistinctFunctionAggregate(op3.AggFunc, argument);
                }
                else
                {
                    aggregate = this._commandTree.CreateFunctionAggregate(op3.AggFunc, argument);
                }
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(list3.Contains(var2), "Defined aggregate Var not in Output Aggregate Vars list?");
                dictionary.Add(var2, aggregate);
            }
            this.ExitExpressionBindingScope(scope);
            AliasGenerator generator2 = new AliasGenerator("A");
            List<KeyValuePair<string, DbAggregate>> aggregates = new List<KeyValuePair<string, DbAggregate>>();
            foreach (Var var3 in list3)
            {
                string str4 = generator2.Next();
                aggregates.Add(new KeyValuePair<string, DbAggregate>(str4, dictionary[var3]));
                VarInfo info3 = new VarInfo(var3);
                info3.PrependProperty(str4);
                publishedVars.Add(info3);
            }
            DbExpression expr = this._commandTree.CreateGroupByExpression(this._commandTree.CreateGroupExpressionBinding(scope.Publisher, publisherName, groupVarName), keys, aggregates);
            this.PublishRelOp(this._groupByAliases.Next(), expr, publishedVars);
            return expr;
        }

        public override DbExpression Visit(InnerJoinOp op, Node n) => 
            this.VisitBinaryJoin(n, DbExpressionKind.InnerJoin);

        public override DbExpression Visit(InternalConstantOp op, Node n) => 
            this.VisitConstantOp(op, n);

        public override DbExpression Visit(IntersectOp op, Node n)
        {
            VarInfoList publishedVars = new VarInfoList();
            DbExpression[] expressionArray = this.VisitSetOp(op, n, publishedVars);
            DbExpression expr = this._commandTree.CreateIntersectExpression(expressionArray[0], expressionArray[1]);
            this.PublishRelOp(this._intersectAliases.Next(), expr, publishedVars);
            return expr;
        }

        public override DbExpression Visit(IsOfOp op, Node n)
        {
            if (op.IsOfOnly)
            {
                return this._commandTree.CreateIsOfOnlyExpression(this.VisitChild(n, 0), op.IsOfType);
            }
            return this._commandTree.CreateIsOfExpression(this.VisitChild(n, 0), op.IsOfType);
        }

        public override DbExpression Visit(LeftOuterJoinOp op, Node n) => 
            this.VisitBinaryJoin(n, DbExpressionKind.LeftOuterJoin);

        public override DbExpression Visit(LikeOp op, Node n) => 
            this._commandTree.CreateLikeExpression(this.VisitChild(n, 0), this.VisitChild(n, 1), this.VisitChild(n, 2));

        public override DbExpression Visit(MultiStreamNestOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(NavigateOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(NewEntityOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(NewInstanceOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(NewMultisetOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(NewRecordOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(NullOp op, Node n) => 
            this._commandTree.CreateNullExpression(op.Type);

        public override DbExpression Visit(OuterApplyOp op, Node n) => 
            this.VisitApply(n, DbExpressionKind.OuterApply);

        public override DbExpression Visit(PhysicalProjectOp op, Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(n.Children.Count == 1, "more than one input to physicalProjectOp?");
            VarList collection = new VarList();
            foreach (Var var in op.Outputs)
            {
                if (!collection.Contains(var))
                {
                    collection.Add(var);
                }
            }
            op.Outputs.Clear();
            op.Outputs.AddRange(collection);
            return this.BuildProjection(n.Child0, op.Outputs).Publisher;
        }

        public override DbExpression Visit(ProjectOp op, Node n) => 
            this.VisitProject(op, n, op.Outputs);

        public override DbExpression Visit(PropertyOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(RefOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(RelPropertyOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(ScanTableOp op, Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(op.Table.TableMetadata.Extent != null, "Invalid TableMetadata used in ScanTableOp - no Extent specified");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(!n.HasChild0, "views are not expected here");
            VarInfoList tableVars = GetTableVars(op.Table);
            DbExpression expr = this._commandTree.CreateScanExpression(op.Table.TableMetadata.Extent);
            this.PublishRelOp(this._extentAliases.Next(), expr, tableVars);
            return expr;
        }

        public override DbExpression Visit(ScanViewOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(SingleRowOp op, Node n)
        {
            RelOpInfo info;
            DbExpression publisher;
            if (n.Child0.Op.OpType != OpType.Project)
            {
                ExtendedNodeInfo extendedNodeInfo = this._iqtCommand.GetExtendedNodeInfo(n.Child0);
                if (extendedNodeInfo.Definitions.IsEmpty)
                {
                    info = this.BuildEmptyProjection(n.Child0);
                }
                else
                {
                    info = this.BuildProjection(n.Child0, extendedNodeInfo.Definitions);
                }
                publisher = info.Publisher;
            }
            else
            {
                publisher = base.VisitNode(n.Child0);
                this.AssertRelOp(publisher);
                info = this.ConsumeRelOp(publisher);
            }
            DbElementExpression expression2 = this._commandTree.CreateElementExpression(publisher);
            List<DbExpression> collectionElements = new List<DbExpression> {
                expression2
            };
            DbNewInstanceExpression expr = this._commandTree.CreateNewCollectionExpression(collectionElements);
            this.PublishRelOp(this._elementAliases.Next(), expr, info.PublishedVars);
            return expr;
        }

        public override DbExpression Visit(SingleRowTableOp op, Node n)
        {
            List<DbExpression> collectionElements = new List<DbExpression> {
                this._commandTree.CreateConstantExpression(true)
            };
            DbNewInstanceExpression expr = this._commandTree.CreateNewCollectionExpression(collectionElements);
            this.PublishRelOp(this._singleRowTableAliases.Next(), expr, new VarInfoList());
            return expr;
        }

        public override DbExpression Visit(SingleStreamNestOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(SoftCastOp op, Node n) => 
            this.VisitChild(n, 0);

        public override DbExpression Visit(SortOp op, Node n)
        {
            RelOpInfo scope = this.EnterExpressionBindingScope(n.Child0);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(!n.HasChild1, "SortOp can have only one child");
            DbExpression expr = this._commandTree.CreateSortExpression(scope.CreateBinding(), this.VisitSortKeys(op.Keys));
            this.ExitExpressionBindingScope(scope);
            this.PublishRelOp(this._sortAliases.Next(), expr, scope.PublishedVars);
            return expr;
        }

        public override DbExpression Visit(TreatOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override DbExpression Visit(UnionAllOp op, Node n)
        {
            VarInfoList publishedVars = new VarInfoList();
            DbExpression[] expressionArray = this.VisitSetOp(op, n, publishedVars);
            DbExpression expr = this._commandTree.CreateUnionAllExpression(expressionArray[0], expressionArray[1]);
            this.PublishRelOp(this._unionAllAliases.Next(), expr, publishedVars);
            return expr;
        }

        public override DbExpression Visit(UnnestOp op, Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(n.Child0.Op.OpType == OpType.VarDef, "an unnest's child must be a VarDef");
            Node node = n.Child0.Child0;
            DbExpression expr = node.Op.Accept<DbExpression>(this, node);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(expr.ResultType.EdmType.BuiltInTypeKind == BuiltInTypeKind.CollectionType, "the input to unnest must yield a collection after plan compilation");
            VarInfoList tableVars = GetTableVars(op.Table);
            this.PublishRelOp(this._extentAliases.Next(), expr, tableVars);
            return expr;
        }

        public override DbExpression Visit(VarDefListOp op, Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "Unexpected VarDefListOp");
            throw EntityUtil.NotSupported(Strings.Iqt_CTGen_UnexpectedVarDefList);
        }

        public override DbExpression Visit(VarDefOp op, Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "Unexpected VarDefOp");
            throw EntityUtil.NotSupported(Strings.Iqt_CTGen_UnexpectedVarDef);
        }

        public override DbExpression Visit(VarRefOp op, Node n) => 
            this.ResolveVar(op.Var);

        private DbExpression VisitApply(Node applyNode, DbExpressionKind applyKind)
        {
            RelOpInfo scope = this.EnterExpressionBindingScope(applyNode.Child0);
            RelOpInfo info2 = this.EnterExpressionBindingScope(applyNode.Child1, false);
            DbExpression expr = this._commandTree.CreateApplyExpressionByKind(applyKind, scope.CreateBinding(), info2.CreateBinding());
            this.ExitExpressionBindingScope(info2, false);
            this.ExitExpressionBindingScope(scope);
            scope.PublishedVars.PrependProperty(scope.PublisherName);
            info2.PublishedVars.PrependProperty(info2.PublisherName);
            VarInfoList publishedVars = new VarInfoList();
            publishedVars.AddRange(scope.PublishedVars);
            publishedVars.AddRange(info2.PublishedVars);
            this.PublishRelOp(this._applyAliases.Next(), expr, publishedVars);
            return expr;
        }

        private DbExpression VisitBinaryJoin(Node joinNode, DbExpressionKind joinKind)
        {
            RelOpInfo inputState = this.VisitJoinInput(joinNode.Child0);
            RelOpInfo info2 = this.VisitJoinInput(joinNode.Child1);
            bool wasPushed = false;
            DbExpression joinCondition = null;
            if (joinNode.Children.Count > 2)
            {
                wasPushed = true;
                this.PushExpressionBindingScope(inputState);
                this.PushExpressionBindingScope(info2);
                joinCondition = base.VisitNode(joinNode.Child2);
            }
            else
            {
                joinCondition = this._commandTree.CreateTrueExpression();
            }
            DbExpression expr = this._commandTree.CreateJoinExpressionByKind(joinKind, joinCondition, inputState.CreateBinding(), info2.CreateBinding());
            VarInfoList publishedVars = new VarInfoList();
            this.ExitExpressionBindingScope(info2, wasPushed);
            info2.PublishedVars.PrependProperty(info2.PublisherName);
            publishedVars.AddRange(info2.PublishedVars);
            this.ExitExpressionBindingScope(inputState, wasPushed);
            inputState.PublishedVars.PrependProperty(inputState.PublisherName);
            publishedVars.AddRange(inputState.PublishedVars);
            this.PublishRelOp(this._joinAliases.Next(), expr, publishedVars);
            return expr;
        }

        private DbExpression VisitChild(Node n, int index)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(n.Children.Count > index, "VisitChild called with invalid index");
            return base.VisitNode(n.Children[index]);
        }

        private List<DbExpression> VisitChildren(Node n)
        {
            List<DbExpression> list = new List<DbExpression>();
            foreach (Node node in n.Children)
            {
                list.Add(base.VisitNode(node));
            }
            return list;
        }

        protected override DbExpression VisitConstantOp(ConstantBaseOp op, Node n) => 
            this._commandTree.CreateConstantExpression(op.Value, op.Type);

        private RelOpInfo VisitJoinInput(Node joinInputNode)
        {
            if ((joinInputNode.Op.OpType == OpType.Filter) && (joinInputNode.Child0.Op.OpType == OpType.ScanTable))
            {
                ScanTableOp op = (ScanTableOp) joinInputNode.Child0.Op;
                if (op.Table.ReferencedColumns.IsEmpty)
                {
                    return this.BuildEmptyProjection(joinInputNode);
                }
                return this.BuildProjection(joinInputNode, op.Table.ReferencedColumns);
            }
            return this.EnterExpressionBindingScope(joinInputNode, false);
        }

        private DbExpression VisitProject(ProjectOp op, Node n, IEnumerable<Var> varList)
        {
            RelOpInfo sourceInfo = this.EnterExpressionBindingScope(n.Child0);
            if (n.Children.Count > 1)
            {
                this.EnterVarDefListScope(n.Child1);
            }
            DbExpression expression = this.CreateProject(sourceInfo, varList);
            if (n.Children.Count > 1)
            {
                this.ExitVarDefScope();
            }
            this.ExitExpressionBindingScope(sourceInfo);
            return expression;
        }

        private DbExpression[] VisitSetOp(SetOp op, Node n, VarInfoList publishedVars)
        {
            AssertBinary(n);
            DbExpression expression = this.VisitSetOpArgument(n.Child0, op.Outputs, op.VarMap[0]);
            DbExpression expression2 = this.VisitSetOpArgument(n.Child1, op.Outputs, op.VarMap[1]);
            CollectionType edmType = TypeHelpers.GetEdmType<CollectionType>(TypeHelpers.GetCommonTypeUsage(expression.ResultType, expression2.ResultType));
            IEnumerator<EdmProperty> enumerator = null;
            RowType type = null;
            if (TypeHelpers.TryGetEdmType<RowType>(edmType.TypeUsage, out type))
            {
                enumerator = type.Properties.GetEnumerator();
            }
            foreach (Var var in op.Outputs)
            {
                VarInfo item = new VarInfo(var);
                if (type != null)
                {
                    if (!enumerator.MoveNext())
                    {
                        System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "Record columns don't match output vars");
                    }
                    item.PrependProperty(enumerator.Current.Name);
                }
                publishedVars.Add(item);
            }
            return new DbExpression[] { expression, expression2 };
        }

        private DbExpression VisitSetOpArgument(Node argNode, VarVec outputVars, VarMap argVars)
        {
            RelOpInfo info;
            List<Var> projectionVars = new List<Var>();
            if (outputVars.IsEmpty)
            {
                info = this.BuildEmptyProjection(argNode);
            }
            else
            {
                foreach (Var var in outputVars)
                {
                    projectionVars.Add(argVars[var]);
                }
                info = this.BuildProjection(argNode, projectionVars);
            }
            return info.Publisher;
        }

        private List<DbSortClause> VisitSortKeys(IList<System.Data.Query.InternalTrees.SortKey> sortKeys)
        {
            VarVec vec = this._iqtCommand.CreateVarVec();
            List<DbSortClause> list = new List<DbSortClause>();
            foreach (System.Data.Query.InternalTrees.SortKey key in sortKeys)
            {
                if (!vec.IsSet(key.Var))
                {
                    vec.Set(key.Var);
                    DbSortClause item = null;
                    if (!string.IsNullOrEmpty(key.Collation))
                    {
                        item = this._commandTree.CreateSortClause(this.ResolveVar(key.Var), key.AscendingSort, key.Collation);
                    }
                    else
                    {
                        item = this._commandTree.CreateSortClause(this.ResolveVar(key.Var), key.AscendingSort);
                    }
                    list.Add(item);
                }
            }
            return list;
        }

        private abstract class IqtVarScope
        {
            protected IqtVarScope()
            {
            }

            internal abstract bool TryResolveVar(Var targetVar, out DbExpression resultExpr);
        }

        private class RelOpInfo : CTreeGenerator.IqtVarScope
        {
            private string _bindingName;
            private CTreeGenerator.VarInfoList _definedVars;
            private DbExpression _publishingExpression;

            internal RelOpInfo(string bindingName, DbExpression publisher, IEnumerable<CTreeGenerator.VarInfo> publishedVars)
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsCollectionType(publisher.ResultType), "non-collection type used as RelOpInfo publisher");
                this._bindingName = bindingName;
                this._publishingExpression = publisher;
                this._definedVars = new CTreeGenerator.VarInfoList(publishedVars);
            }

            internal DbExpressionBinding CreateBinding() => 
                this._publishingExpression.CommandTree.CreateExpressionBinding(this._publishingExpression, this._bindingName);

            internal override bool TryResolveVar(Var targetVar, out DbExpression resultExpr)
            {
                resultExpr = null;
                CTreeGenerator.VarInfo varInfo = null;
                if (!this._definedVars.TryGetInfo(targetVar, out varInfo))
                {
                    return false;
                }
                resultExpr = this._publishingExpression.CommandTree.CreateVariableReferenceExpression(this._bindingName, TypeHelpers.GetEdmType<CollectionType>(this._publishingExpression.ResultType).TypeUsage);
                foreach (string str in varInfo.PropertyPath)
                {
                    resultExpr = this._publishingExpression.CommandTree.CreatePropertyExpression(str, resultExpr);
                }
                return true;
            }

            internal CTreeGenerator.VarInfoList PublishedVars =>
                this._definedVars;

            internal DbExpression Publisher =>
                this._publishingExpression;

            internal string PublisherName
            {
                get => 
                    this._bindingName;
                set
                {
                    this._bindingName = value;
                }
            }
        }

        private class VarDefScope : CTreeGenerator.IqtVarScope
        {
            private Dictionary<Var, DbExpression> _definedVars;

            internal VarDefScope(Dictionary<Var, DbExpression> definedVars)
            {
                this._definedVars = definedVars;
            }

            internal override bool TryResolveVar(Var targetVar, out DbExpression resultExpr)
            {
                resultExpr = null;
                DbExpression expression = null;
                if (this._definedVars.TryGetValue(targetVar, out expression))
                {
                    resultExpr = expression;
                    return true;
                }
                return false;
            }
        }

        private class VarInfo
        {
            private List<string> _propertyChain = new List<string>();
            private System.Data.Query.InternalTrees.Var _var;

            internal VarInfo(System.Data.Query.InternalTrees.Var target)
            {
                this._var = target;
            }

            internal void PrependProperty(string propName)
            {
                this._propertyChain.Insert(0, propName);
            }

            internal List<string> PropertyPath =>
                this._propertyChain;

            internal System.Data.Query.InternalTrees.Var Var =>
                this._var;
        }

        private class VarInfoList : List<CTreeGenerator.VarInfo>
        {
            internal VarInfoList()
            {
            }

            internal VarInfoList(IEnumerable<CTreeGenerator.VarInfo> elements) : base(elements)
            {
            }

            internal void PrependProperty(string propName)
            {
                foreach (CTreeGenerator.VarInfo info in this)
                {
                    info.PropertyPath.Insert(0, propName);
                }
            }

            internal bool TryGetInfo(Var targetVar, out CTreeGenerator.VarInfo varInfo)
            {
                varInfo = null;
                foreach (CTreeGenerator.VarInfo info in this)
                {
                    if (info.Var == targetVar)
                    {
                        varInfo = info;
                        return true;
                    }
                }
                return false;
            }
        }
    }
}

