namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class ITreeGenerator : DbExpressionVisitor<Node>
    {
        private readonly DbProjectExpression _discriminatedViewTopProject;
        private readonly DiscriminatorMap _discriminatorMap;
        private readonly Command _iqtCommand;
        private readonly Dictionary<Node, Var> _varMap = new Dictionary<Node, Var>();
        private readonly Stack<CqtVariableScope> _varScopes = new Stack<CqtVariableScope>();
        private static Dictionary<DbExpressionKind, OpType> s_opMap = InitializeExpressionKindToOpTypeMap();

        private ITreeGenerator(DbQueryCommandTree ctree, DiscriminatorMap discriminatorMap)
        {
            this._iqtCommand = new Command(ctree.MetadataWorkspace, ctree.DataSpace);
            if (discriminatorMap != null)
            {
                this._discriminatorMap = discriminatorMap;
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(ctree.Query.ExpressionKind == DbExpressionKind.Project, "top level QMV expression must be project to match discriminator pattern");
                this._discriminatedViewTopProject = (DbProjectExpression) ctree.Query;
            }
            foreach (KeyValuePair<string, TypeUsage> pair in ctree.Parameters)
            {
                this._iqtCommand.CreateParameterVar(pair.Key, pair.Value);
            }
            this._iqtCommand.Root = this.VisitExpr(ctree.Query);
            if (!this._iqtCommand.Root.Op.IsRelOp)
            {
                Var var;
                Node definingExpr = this.ConvertToScalarOpTree(this._iqtCommand.Root, ctree.Query);
                Node node2 = this._iqtCommand.CreateNode(this._iqtCommand.CreateSingleRowTableOp());
                Node node3 = this._iqtCommand.CreateVarDefListNode(definingExpr, out var);
                ProjectOp op = this._iqtCommand.CreateProjectOp(var);
                Node node4 = this._iqtCommand.CreateNode(op, node2, node3);
                if (TypeSemantics.IsCollectionType(this._iqtCommand.Root.Op.Type))
                {
                    UnnestOp op2 = this._iqtCommand.CreateUnnestOp(var);
                    node4 = this._iqtCommand.CreateNode(op2, node3.Child0);
                    var = op2.Table.Columns[0];
                }
                this._iqtCommand.Root = node4;
                this._varMap[this._iqtCommand.Root] = var;
            }
            this._iqtCommand.Root = this.CapWithPhysicalProject(this._iqtCommand.Root);
        }

        private Node BuildEntityRef(Node arg, TypeUsage entityType)
        {
            TypeUsage type = TypeHelpers.CreateReferenceTypeUsage((EntityType) entityType.EdmType);
            return this._iqtCommand.CreateNode(this._iqtCommand.CreateGetEntityRefOp(type), arg);
        }

        private Node BuildSoftCast(Node node, EdmType targetType) => 
            this.BuildSoftCast(node, TypeUsage.Create(targetType));

        private Node BuildSoftCast(Node node, TypeUsage targetType)
        {
            if (node.Op.IsRelOp)
            {
                Var var2;
                targetType = TypeHelpers.GetEdmType<CollectionType>(targetType).TypeUsage;
                Var v = this._varMap[node];
                if (Command.EqualTypes(targetType, v.Type))
                {
                    return node;
                }
                Node node2 = this._iqtCommand.CreateNode(this._iqtCommand.CreateVarRefOp(v));
                Node definingExpr = this._iqtCommand.CreateNode(this._iqtCommand.CreateSoftCastOp(targetType), node2);
                Node node4 = this._iqtCommand.CreateVarDefListNode(definingExpr, out var2);
                ProjectOp op = this._iqtCommand.CreateProjectOp(var2);
                Node node5 = this._iqtCommand.CreateNode(op, node, node4);
                this._varMap[node5] = var2;
                return node5;
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(node.Op.IsScalarOp, "I want a scalar op");
            if (Command.EqualTypes(node.Op.Type, targetType))
            {
                return node;
            }
            SoftCastOp op2 = this._iqtCommand.CreateSoftCastOp(targetType);
            return this._iqtCommand.CreateNode(op2, node);
        }

        private Node CapWithPhysicalProject(Node input)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(input.Op.IsRelOp, "unexpected non-RelOp?");
            Var outputVar = this._varMap[input];
            PhysicalProjectOp op = this._iqtCommand.CreatePhysicalProjectOp(outputVar);
            return this._iqtCommand.CreateNode(op, input);
        }

        private Node CapWithProject(Node input)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(input.Op.IsRelOp, "unexpected non-RelOp?");
            if (input.Op.OpType == OpType.Project)
            {
                return input;
            }
            Var v = this._varMap[input];
            ProjectOp op = this._iqtCommand.CreateProjectOp(v);
            Node node = this._iqtCommand.CreateNode(op, input, this._iqtCommand.CreateNode(this._iqtCommand.CreateVarDefListOp()));
            this._varMap[node] = v;
            return node;
        }

        private Node ConvertToScalarOpTree(Node node, DbExpression expr)
        {
            if (node.Op.IsRelOp)
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsCollectionType(expr.ResultType), "RelOp with non-Collection result type");
                CollectOp op = this._iqtCommand.CreateCollectOp(expr.ResultType);
                Node node2 = this.CapWithPhysicalProject(node);
                node = this._iqtCommand.CreateNode(op, node2);
                return node;
            }
            if (this.IsPredicate(expr))
            {
                CaseOp op2 = this._iqtCommand.CreateCaseOp(this._iqtCommand.BooleanType);
                List<Node> args = new List<Node>((expr.ExpressionKind == DbExpressionKind.IsNull) ? 3 : 5) {
                    node,
                    this._iqtCommand.CreateNode(this._iqtCommand.CreateInternalConstantOp(this._iqtCommand.BooleanType, true))
                };
                if (expr.ExpressionKind != DbExpressionKind.IsNull)
                {
                    Node node3 = this.VisitExpr(expr);
                    args.Add(this._iqtCommand.CreateNode(this._iqtCommand.CreateConditionalOp(OpType.Not), node3));
                }
                args.Add(this._iqtCommand.CreateNode(this._iqtCommand.CreateInternalConstantOp(this._iqtCommand.BooleanType, false)));
                if (expr.ExpressionKind != DbExpressionKind.IsNull)
                {
                    args.Add(this._iqtCommand.CreateNode(this._iqtCommand.CreateNullOp(this._iqtCommand.BooleanType)));
                }
                node = this._iqtCommand.CreateNode(op2, args);
            }
            return node;
        }

        private Node CreateLimitNode(Node inputNode, Node limitNode, bool withTies)
        {
            if ((OpType.ConstrainedSort == inputNode.Op.OpType) && (OpType.Null == inputNode.Child2.Op.OpType))
            {
                inputNode.Child2 = limitNode;
                if (withTies)
                {
                    ((ConstrainedSortOp) inputNode.Op).WithTies = true;
                }
                return inputNode;
            }
            if (OpType.Sort == inputNode.Op.OpType)
            {
                return this._iqtCommand.CreateNode(this._iqtCommand.CreateConstrainedSortOp(((SortOp) inputNode.Op).Keys, withTies), inputNode.Child0, this._iqtCommand.CreateNode(this._iqtCommand.CreateNullOp(this._iqtCommand.IntegerType)), limitNode);
            }
            return this._iqtCommand.CreateNode(this._iqtCommand.CreateConstrainedSortOp(new List<SortKey>(), withTies), inputNode, this._iqtCommand.CreateNode(this._iqtCommand.CreateNullOp(this._iqtCommand.IntegerType)), limitNode);
        }

        private Node CreateNewInstanceArgument(EdmMember property, DbExpression value) => 
            this.BuildSoftCast(this.VisitExprAsScalar(value), Helper.GetModelTypeUsage(property));

        private Node EnsureRelOp(Node inputNode)
        {
            Var var;
            Var var2;
            Op op = inputNode.Op;
            if (op.IsRelOp)
            {
                return inputNode;
            }
            ScalarOp op2 = op as ScalarOp;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(op2 != null, "An expression in a CQT produced a non-ScalarOp and non-RelOp output Op");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsCollectionType(op2.Type), "An expression used as a RelOp argument was neither a RelOp or a collection");
            if (op is CollectOp)
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(inputNode.HasChild0, "CollectOp without argument");
                if (inputNode.Child0.Op is PhysicalProjectOp)
                {
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(inputNode.Child0.HasChild0, "PhysicalProjectOp without argument");
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(inputNode.Child0.Child0.Op.IsRelOp, "PhysicalProjectOp applied to non-RelOp input");
                    return inputNode.Child0.Child0;
                }
            }
            Node node = this._iqtCommand.CreateVarDefNode(inputNode, out var);
            UnnestOp op4 = this._iqtCommand.CreateUnnestOp(var);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(op4.Table.Columns.Count == 1, "Unnest of collection ScalarOp produced unexpected number of columns (1 expected)");
            Node node2 = this._iqtCommand.CreateNode(op4, node);
            this._varMap[node2] = op4.Table.Columns[0];
            Node definingExpr = this._iqtCommand.CreateNode(this._iqtCommand.CreateVarRefOp(op4.Table.Columns[0]));
            Node node4 = this._iqtCommand.CreateVarDefListNode(definingExpr, out var2);
            ProjectOp op5 = this._iqtCommand.CreateProjectOp(var2);
            Node node5 = this._iqtCommand.CreateNode(op5, node2, node4);
            this._varMap[node5] = var2;
            return node5;
        }

        private Node EnterExpressionBinding(DbExpressionBinding binding) => 
            this.PushBindingScope(binding.Expression, binding.VariableName);

        private Node EnterGroupExpressionBinding(DbGroupExpressionBinding binding) => 
            this.PushBindingScope(binding.Expression, binding.VariableName);

        private void EnterLambdaFunction(EdmFunction function, List<Node> argumentValues)
        {
            IList<FunctionParameter> parameters = function.Parameters;
            if (parameters.Count > 0)
            {
                Dictionary<string, Node> args = new Dictionary<string, Node>();
                int num = 0;
                foreach (Node node in argumentValues)
                {
                    args.Add(parameters[num].Name, node);
                    num++;
                }
                this._varScopes.Push(new LambdaScope(this, this._iqtCommand, args));
            }
        }

        private ExpressionBindingScope ExitExpressionBinding()
        {
            ExpressionBindingScope scope = this._varScopes.Pop() as ExpressionBindingScope;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(scope != null, "ExitExpressionBinding called without ExpressionBindingScope on top of scope stack");
            return scope;
        }

        private void ExitGroupExpressionBinding()
        {
            ExpressionBindingScope scope = this._varScopes.Pop() as ExpressionBindingScope;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(scope != null, "ExitGroupExpressionBinding called without ExpressionBindingScope on top of scope stack");
        }

        private LambdaScope ExitLambdaFunction()
        {
            LambdaScope scope = this._varScopes.Pop() as LambdaScope;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(scope != null, "ExitLambdaFunction called without LambdaScope on top of scope stack");
            return scope;
        }

        private static RowType ExtractElementRowType(TypeUsage typeUsage) => 
            TypeHelpers.GetEdmType<RowType>(TypeHelpers.GetEdmType<CollectionType>(typeUsage).TypeUsage);

        public static Command Generate(DbQueryCommandTree ctree) => 
            Generate(ctree, null);

        internal static Command Generate(DbQueryCommandTree ctree, DiscriminatorMap discriminatorMap)
        {
            ITreeGenerator generator = new ITreeGenerator(ctree, discriminatorMap);
            return generator._iqtCommand;
        }

        private Node GenerateDiscriminatedProject(DbProjectExpression e)
        {
            Var var;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(null != this._discriminatedViewTopProject, "if a project matches the pattern, there must be a corresponding discriminator map");
            Node node = this.EnterExpressionBinding(e.Input);
            List<RelProperty> relProperties = new List<RelProperty>();
            List<Node> collection = new List<Node>();
            foreach (KeyValuePair<RelProperty, DbExpression> pair in this._discriminatorMap.RelPropertyMap)
            {
                relProperties.Add(pair.Key);
                collection.Add(this.VisitExprAsScalar(pair.Value));
            }
            DiscriminatedNewEntityOp op = this._iqtCommand.CreateDiscriminatedNewEntityOp(e.Projection.ResultType, new ExplicitDiscriminatorMap(this._discriminatorMap), this._discriminatorMap.EntitySet, relProperties);
            List<Node> args = new List<Node>(this._discriminatorMap.PropertyMap.Count + 1) {
                this.CreateNewInstanceArgument(this._discriminatorMap.Discriminator.Property, this._discriminatorMap.Discriminator)
            };
            foreach (KeyValuePair<EdmProperty, DbExpression> pair2 in this._discriminatorMap.PropertyMap)
            {
                DbExpression expression = pair2.Value;
                EdmProperty key = pair2.Key;
                Node item = this.CreateNewInstanceArgument(key, expression);
                args.Add(item);
            }
            args.AddRange(collection);
            Node definingExpr = this._iqtCommand.CreateNode(op, args);
            this.ExitExpressionBinding();
            Node node4 = this._iqtCommand.CreateVarDefListNode(definingExpr, out var);
            ProjectOp op2 = this._iqtCommand.CreateProjectOp(var);
            Node node5 = this._iqtCommand.CreateNode(op2, node, node4);
            this._varMap[node5] = var;
            return node5;
        }

        private Node GenerateStandardProject(DbProjectExpression e)
        {
            Var var;
            Node node = this.EnterExpressionBinding(e.Input);
            Node definingExpr = this.VisitExprAsScalar(e.Projection);
            this.ExitExpressionBinding();
            Node node3 = this._iqtCommand.CreateVarDefListNode(definingExpr, out var);
            ProjectOp op = this._iqtCommand.CreateProjectOp(var);
            Node node4 = this._iqtCommand.CreateNode(op, node, node3);
            this._varMap[node4] = var;
            return node4;
        }

        private static Dictionary<DbExpressionKind, OpType> InitializeExpressionKindToOpTypeMap() => 
            new Dictionary<DbExpressionKind, OpType>(12) { 
                [DbExpressionKind.Plus] = OpType.Plus,
                [DbExpressionKind.Minus] = OpType.Minus,
                [DbExpressionKind.Multiply] = OpType.Multiply,
                [DbExpressionKind.Divide] = OpType.Divide,
                [DbExpressionKind.Modulo] = OpType.Modulo,
                [DbExpressionKind.UnaryMinus] = OpType.UnaryMinus,
                [DbExpressionKind.Equals] = OpType.EQ,
                [DbExpressionKind.NotEquals] = OpType.NE,
                [DbExpressionKind.LessThan] = OpType.LT,
                [DbExpressionKind.GreaterThan] = OpType.GT,
                [DbExpressionKind.LessThanOrEquals] = OpType.LE,
                [DbExpressionKind.GreaterThanOrEquals] = OpType.GE
            };

        private bool IsPredicate(DbExpression expr)
        {
            if (TypeSemantics.IsPrimitiveType(expr.ResultType, PrimitiveTypeKind.Boolean))
            {
                switch (expr.ExpressionKind)
                {
                    case DbExpressionKind.All:
                    case DbExpressionKind.And:
                    case DbExpressionKind.Any:
                    case DbExpressionKind.Equals:
                    case DbExpressionKind.GreaterThan:
                    case DbExpressionKind.GreaterThanOrEquals:
                    case DbExpressionKind.IsEmpty:
                    case DbExpressionKind.IsNull:
                    case DbExpressionKind.IsOf:
                    case DbExpressionKind.IsOfOnly:
                    case DbExpressionKind.LessThan:
                    case DbExpressionKind.LessThanOrEquals:
                    case DbExpressionKind.Like:
                    case DbExpressionKind.Not:
                    case DbExpressionKind.NotEquals:
                    case DbExpressionKind.Or:
                        return true;

                    case DbExpressionKind.Function:
                    {
                        DbFunctionExpression expression = expr as DbFunctionExpression;
                        return (((expression != null) && expression.IsLambda) && this.IsPredicate(expression.LambdaBody));
                    }
                }
            }
            return false;
        }

        private Node ProjectNewRecord(Node inputNode, RowType recType, IEnumerable<Var> colVars)
        {
            Var var2;
            List<Node> args = new List<Node>();
            foreach (Var var in colVars)
            {
                args.Add(this._iqtCommand.CreateNode(this._iqtCommand.CreateVarRefOp(var)));
            }
            Node definingExpr = this._iqtCommand.CreateNode(this._iqtCommand.CreateNewRecordOp(recType), args);
            Node node2 = this._iqtCommand.CreateVarDefListNode(definingExpr, out var2);
            ProjectOp op = this._iqtCommand.CreateProjectOp(var2);
            Node node3 = this._iqtCommand.CreateNode(op, inputNode, node2);
            this._varMap[node3] = var2;
            return node3;
        }

        private Node PushBindingScope(DbExpression boundExpression, string bindingName)
        {
            Node inputNode = this.VisitExpr(boundExpression);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(inputNode != null, "DbExpressionBinding.Expression produced null conversion");
            inputNode = this.EnsureRelOp(inputNode);
            Var iqtVar = this._varMap[inputNode];
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(iqtVar != null, "No Var found for Input Op");
            this._varScopes.Push(new ExpressionBindingScope(this._iqtCommand, bindingName, iqtVar));
            return inputNode;
        }

        public override Node Visit(DbAndExpression e)
        {
            Op op = this._iqtCommand.CreateConditionalOp(OpType.And);
            return this.VisitBinary(e, op, new VisitExprDelegate(this.VisitExprAsPredicate));
        }

        public override Node Visit(DbApplyExpression e)
        {
            Node node = this.EnterExpressionBinding(e.Input);
            Node node2 = this.EnterExpressionBinding(e.Apply);
            this.ExitExpressionBinding();
            this.ExitExpressionBinding();
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((DbExpressionKind.CrossApply == e.ExpressionKind) || (DbExpressionKind.OuterApply == e.ExpressionKind), "Unrecognized DbExpressionKind specified in DbApplyExpression");
            ApplyBaseOp op = null;
            if (DbExpressionKind.CrossApply == e.ExpressionKind)
            {
                op = this._iqtCommand.CreateCrossApplyOp();
            }
            else
            {
                op = this._iqtCommand.CreateOuterApplyOp();
            }
            Node inputNode = this._iqtCommand.CreateNode(op, node, node2);
            return this.ProjectNewRecord(inputNode, ExtractElementRowType(e.ResultType), new Var[] { this._varMap[node], this._varMap[node2] });
        }

        public override Node Visit(DbArithmeticExpression e)
        {
            Op op = this._iqtCommand.CreateArithmeticOp(s_opMap[e.ExpressionKind], e.ResultType);
            List<Node> args = new List<Node>();
            foreach (DbExpression expression in e.Arguments)
            {
                Node node = this.VisitExprAsScalar(expression);
                args.Add(this.BuildSoftCast(node, e.ResultType));
            }
            return this._iqtCommand.CreateNode(op, args);
        }

        public override Node Visit(DbCaseExpression e)
        {
            List<Node> args = new List<Node>();
            for (int i = 0; i < e.When.Count; i++)
            {
                args.Add(this.VisitExprAsPredicate(e.When[i]));
                args.Add(this.BuildSoftCast(this.VisitExprAsScalar(e.Then[i]), e.ResultType));
            }
            args.Add(this.BuildSoftCast(this.VisitExprAsScalar(e.Else), e.ResultType));
            return this._iqtCommand.CreateNode(this._iqtCommand.CreateCaseOp(e.ResultType), args);
        }

        public override Node Visit(DbCastExpression e)
        {
            Op op = this._iqtCommand.CreateCastOp(e.ResultType);
            return this.VisitUnary(e, op, new VisitExprDelegate(this.VisitExprAsScalar));
        }

        public override Node Visit(DbComparisonExpression e)
        {
            Op op = this._iqtCommand.CreateComparisonOp(s_opMap[e.ExpressionKind]);
            Node node = this.VisitExprAsScalar(e.Left);
            Node node2 = this.VisitExprAsScalar(e.Right);
            TypeUsage commonTypeUsage = TypeHelpers.GetCommonTypeUsage(e.Left.ResultType, e.Right.ResultType);
            if (!Command.EqualTypes(e.Left.ResultType, e.Right.ResultType))
            {
                node = this.BuildSoftCast(node, commonTypeUsage);
                node2 = this.BuildSoftCast(node2, commonTypeUsage);
            }
            if (TypeSemantics.IsEntityType(commonTypeUsage) && ((e.ExpressionKind == DbExpressionKind.Equals) || (e.ExpressionKind == DbExpressionKind.NotEquals)))
            {
                node = this.BuildEntityRef(node, commonTypeUsage);
                node2 = this.BuildEntityRef(node2, commonTypeUsage);
            }
            return this._iqtCommand.CreateNode(op, node, node2);
        }

        public override Node Visit(DbConstantExpression e)
        {
            ConstantBaseOp op = this._iqtCommand.CreateConstantOp(e.ResultType, e.Value);
            return this._iqtCommand.CreateNode(op);
        }

        public override Node Visit(DbCrossJoinExpression e) => 
            this.VisitJoin(e, e.Inputs, null);

        public override Node Visit(DbDerefExpression e)
        {
            Op op = this._iqtCommand.CreateDerefOp(e.ResultType);
            return this.VisitUnary(e, op, new VisitExprDelegate(this.VisitExprAsScalar));
        }

        public override Node Visit(DbDistinctExpression e)
        {
            Node node = this.EnsureRelOp(this.VisitExpr(e.Argument));
            Var keyVar = this._varMap[node];
            Op op = this._iqtCommand.CreateDistinctOp(keyVar);
            Node node2 = this._iqtCommand.CreateNode(op, node);
            this._varMap[node2] = keyVar;
            return node2;
        }

        public override Node Visit(DbElementExpression e)
        {
            Op op = this._iqtCommand.CreateElementOp(e.ResultType);
            Node node = this.EnsureRelOp(this.VisitExpr(e.Argument));
            Var var = this._varMap[node];
            node = this._iqtCommand.CreateNode(this._iqtCommand.CreateSingleRowOp(), node);
            this._varMap[node] = var;
            node = this.CapWithProject(node);
            return this._iqtCommand.CreateNode(op, node);
        }

        public override Node Visit(DbEntityRefExpression e)
        {
            Op op = this._iqtCommand.CreateGetEntityRefOp(e.ResultType);
            return this.VisitUnary(e, op, new VisitExprDelegate(this.VisitExprAsScalar));
        }

        public override Node Visit(DbExceptExpression e) => 
            this.VisitSetOpExpression(e);

        public override Node Visit(DbExpression e)
        {
            throw EntityUtil.NotSupported(Strings.Cqt_General_UnsupportedExpression(e.GetType().FullName));
        }

        public override Node Visit(DbFilterExpression e)
        {
            Node node = this.EnterExpressionBinding(e.Input);
            Node node2 = this.VisitExprAsPredicate(e.Predicate);
            this.ExitExpressionBinding();
            Op op = this._iqtCommand.CreateFilterOp();
            Node node3 = this._iqtCommand.CreateNode(op, node, node2);
            this._varMap[node3] = this._varMap[node];
            return node3;
        }

        public override Node Visit(DbFunctionExpression e)
        {
            Node node = null;
            List<Node> argumentValues = new List<Node>(e.Arguments.Count);
            int num = 0;
            foreach (DbExpression expression in e.Arguments)
            {
                if (e.IsLambda)
                {
                    argumentValues.Add(this.VisitExpr(expression));
                }
                else
                {
                    argumentValues.Add(this.BuildSoftCast(this.VisitExprAsScalar(expression), e.Function.Parameters[num].TypeUsage));
                }
                num++;
            }
            if (e.LambdaBody != null)
            {
                this.EnterLambdaFunction(e.Function, argumentValues);
                node = this.VisitExpr(e.LambdaBody);
                this.ExitLambdaFunction();
                return node;
            }
            return this._iqtCommand.CreateNode(this._iqtCommand.CreateFunctionOp(e.Function), argumentValues);
        }

        public override Node Visit(DbGroupByExpression e)
        {
            VarVec gbyKeys = this._iqtCommand.CreateVarVec();
            VarVec outputs = this._iqtCommand.CreateVarVec();
            Node node = this.EnterGroupExpressionBinding(e.Input);
            List<Node> args = new List<Node>();
            for (int i = 0; i < e.Keys.Count; i++)
            {
                Var var;
                DbExpression expr = e.Keys[i];
                Node definingExpr = this.VisitExprAsScalar(expr);
                ScalarOp op = definingExpr.Op as ScalarOp;
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(op != null, "GroupBy Key is not a ScalarOp");
                args.Add(this._iqtCommand.CreateVarDefNode(definingExpr, out var));
                outputs.Set(var);
                gbyKeys.Set(var);
            }
            ExpressionBindingScope item = this.ExitExpressionBinding();
            item = new ExpressionBindingScope(this._iqtCommand, e.Input.GroupVariableName, item.ScopeVar);
            this._varScopes.Push(item);
            List<Node> list2 = new List<Node>();
            for (int j = 0; j < e.Aggregates.Count; j++)
            {
                Var var2;
                DbAggregate aggregate = e.Aggregates[j];
                IList<Node> list3 = this.VisitExprAsScalar(aggregate.Arguments);
                Node node3 = null;
                DbFunctionAggregate aggregate2 = aggregate as DbFunctionAggregate;
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(aggregate2 != null, "Unrecognized DbAggregate used in DbGroupByExpression");
                TypeUsage typeUsage = aggregate2.Function.ReturnParameter.TypeUsage;
                node3 = this._iqtCommand.CreateNode(this._iqtCommand.CreateAggregateOp(aggregate2.Function, aggregate2.Distinct), list3);
                list2.Add(this._iqtCommand.CreateVarDefNode(node3, out var2));
                outputs.Set(var2);
            }
            this.ExitGroupExpressionBinding();
            Node inputNode = this._iqtCommand.CreateNode(this._iqtCommand.CreateGroupByOp(gbyKeys, outputs), node, this._iqtCommand.CreateNode(this._iqtCommand.CreateVarDefListOp(), args), this._iqtCommand.CreateNode(this._iqtCommand.CreateVarDefListOp(), list2));
            return this.ProjectNewRecord(inputNode, ExtractElementRowType(e.ResultType), outputs);
        }

        public override Node Visit(DbIntersectExpression e) => 
            this.VisitSetOpExpression(e);

        public override Node Visit(DbIsEmptyExpression e)
        {
            Op op = this._iqtCommand.CreateExistsOp();
            Node node = this.EnsureRelOp(this.VisitExpr(e.Argument));
            return this._iqtCommand.CreateNode(this._iqtCommand.CreateConditionalOp(OpType.Not), this._iqtCommand.CreateNode(op, node));
        }

        public override Node Visit(DbIsNullExpression e)
        {
            bool flag = false;
            if (e.Argument.ExpressionKind == DbExpressionKind.IsNull)
            {
                flag = true;
            }
            else if (e.Argument.ExpressionKind == DbExpressionKind.Not)
            {
                DbNotExpression argument = (DbNotExpression) e.Argument;
                if (argument.Argument.ExpressionKind == DbExpressionKind.IsNull)
                {
                    flag = true;
                }
            }
            Op op = this._iqtCommand.CreateConditionalOp(OpType.IsNull);
            if (flag)
            {
                return this._iqtCommand.CreateNode(op, this._iqtCommand.CreateNode(this._iqtCommand.CreateInternalConstantOp(this._iqtCommand.BooleanType, true)));
            }
            Node arg = this.VisitExprAsScalar(e.Argument);
            if (TypeSemantics.IsEntityType(e.Argument.ResultType))
            {
                arg = this.BuildEntityRef(arg, e.Argument.ResultType);
            }
            return this._iqtCommand.CreateNode(op, arg);
        }

        public override Node Visit(DbIsOfExpression e)
        {
            Op op = null;
            if (DbExpressionKind.IsOfOnly == e.ExpressionKind)
            {
                op = this._iqtCommand.CreateIsOfOnlyOp(e.OfType);
            }
            else
            {
                op = this._iqtCommand.CreateIsOfOp(e.OfType);
            }
            return this.VisitUnary(e, op, new VisitExprDelegate(this.VisitExprAsScalar));
        }

        public override Node Visit(DbJoinExpression e)
        {
            List<DbExpressionBinding> inputs = new List<DbExpressionBinding> {
                e.Left,
                e.Right
            };
            return this.VisitJoin(e, inputs, e.JoinCondition);
        }

        public override Node Visit(DbLikeExpression e) => 
            this._iqtCommand.CreateNode(this._iqtCommand.CreateLikeOp(), this.VisitExpr(e.Argument), this.VisitExpr(e.Pattern), this.VisitExpr(e.Escape));

        public override Node Visit(DbLimitExpression expression)
        {
            Node node3;
            Node inputNode = this.EnsureRelOp(this.VisitExpr(expression.Argument));
            Var var = this._varMap[inputNode];
            Node limitNode = this.VisitExprAsScalar(expression.Limit);
            if (OpType.Project == inputNode.Op.OpType)
            {
                inputNode.Child0 = this.CreateLimitNode(inputNode.Child0, limitNode, expression.WithTies);
                node3 = inputNode;
            }
            else
            {
                node3 = this.CreateLimitNode(inputNode, limitNode, expression.WithTies);
            }
            if (!object.ReferenceEquals(node3, inputNode))
            {
                this._varMap[node3] = var;
            }
            return node3;
        }

        public override Node Visit(DbNewInstanceExpression e)
        {
            Op op = null;
            List<Node> collection = null;
            if (TypeSemantics.IsCollectionType(e.ResultType))
            {
                op = this._iqtCommand.CreateNewMultisetOp(e.ResultType);
            }
            else if (TypeSemantics.IsRowType(e.ResultType))
            {
                op = this._iqtCommand.CreateNewRecordOp(e.ResultType);
            }
            else if (TypeSemantics.IsEntityType(e.ResultType))
            {
                List<RelProperty> relProperties = new List<RelProperty>();
                collection = new List<Node>();
                if (e.HasRelatedEntityReferences)
                {
                    foreach (DbRelatedEntityRef ref2 in e.RelatedEntityReferences)
                    {
                        RelProperty item = new RelProperty((RelationshipType) ref2.TargetEnd.DeclaringType, ref2.SourceEnd, ref2.TargetEnd);
                        relProperties.Add(item);
                        Node node = this.VisitExprAsScalar(ref2.TargetEntityReference);
                        collection.Add(node);
                    }
                }
                op = this._iqtCommand.CreateNewEntityOp(e.ResultType, relProperties);
            }
            else
            {
                op = this._iqtCommand.CreateNewInstanceOp(e.ResultType);
            }
            List<Node> args = new List<Node>();
            if (TypeSemantics.IsStructuralType(e.ResultType))
            {
                StructuralType edmType = TypeHelpers.GetEdmType<StructuralType>(e.ResultType);
                int num = 0;
                foreach (EdmMember member in TypeHelpers.GetAllStructuralMembers(edmType))
                {
                    Node node2 = this.BuildSoftCast(this.VisitExprAsScalar(e.Arguments[num]), Helper.GetModelTypeUsage(member));
                    args.Add(node2);
                    num++;
                }
            }
            else
            {
                TypeUsage typeUsage = TypeHelpers.GetEdmType<CollectionType>(e.ResultType).TypeUsage;
                foreach (DbExpression expression in e.Arguments)
                {
                    Node node3 = this.BuildSoftCast(this.VisitExprAsScalar(expression), typeUsage);
                    args.Add(node3);
                }
            }
            if (collection != null)
            {
                args.AddRange(collection);
            }
            return this._iqtCommand.CreateNode(op, args);
        }

        public override Node Visit(DbNotExpression e)
        {
            Op op = this._iqtCommand.CreateConditionalOp(OpType.Not);
            return this.VisitUnary(e, op, new VisitExprDelegate(this.VisitExprAsPredicate));
        }

        public override Node Visit(DbNullExpression e)
        {
            NullOp op = this._iqtCommand.CreateNullOp(e.ResultType);
            return this._iqtCommand.CreateNode(op);
        }

        public override Node Visit(DbOfTypeExpression e)
        {
            Node node2;
            Var var2;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsCollectionType(e.Argument.ResultType), "Non-Collection Type Argument in DbOfTypeExpression");
            Node inputNode = this.EnsureRelOp(this.VisitExpr(e.Argument));
            Var inputVar = this._varMap[inputNode];
            bool flag = DbExpressionKind.OfTypeOnly == e.ExpressionKind;
            this._iqtCommand.BuildOfTypeTree(inputNode, inputVar, e.OfType, !flag, out node2, out var2);
            this._varMap[node2] = var2;
            return node2;
        }

        public override Node Visit(DbOrExpression e)
        {
            Op op = this._iqtCommand.CreateConditionalOp(OpType.Or);
            return this.VisitBinary(e, op, new VisitExprDelegate(this.VisitExprAsPredicate));
        }

        public override Node Visit(DbParameterReferenceExpression e)
        {
            Op op = this._iqtCommand.CreateVarRefOp(this._iqtCommand.GetParameter(e.ParameterName));
            return this._iqtCommand.CreateNode(op);
        }

        public override Node Visit(DbProjectExpression e)
        {
            if (e == this._discriminatedViewTopProject)
            {
                return this.GenerateDiscriminatedProject(e);
            }
            return this.GenerateStandardProject(e);
        }

        public override Node Visit(DbPropertyExpression e)
        {
            if (((BuiltInTypeKind.EdmProperty != e.Property.BuiltInTypeKind) && (e.Property.BuiltInTypeKind != BuiltInTypeKind.AssociationEndMember)) && (BuiltInTypeKind.NavigationProperty != e.Property.BuiltInTypeKind))
            {
                throw EntityUtil.NotSupported();
            }
            Node node = null;
            Op op = this._iqtCommand.CreatePropertyOp(e.Property);
            if (e.Instance == null)
            {
                return this._iqtCommand.CreateNode(op);
            }
            Node node2 = this.VisitExpr(e.Instance);
            if ((e.Instance.ExpressionKind != DbExpressionKind.NewInstance) || !Helper.IsStructuralType(e.Instance.ResultType.EdmType))
            {
                node2 = this.BuildSoftCast(node2, e.Property.DeclaringType);
                return this._iqtCommand.CreateNode(op, node2);
            }
            IList allStructuralMembers = Helper.GetAllStructuralMembers(e.Instance.ResultType.EdmType);
            int num = -1;
            for (int i = 0; i < allStructuralMembers.Count; i++)
            {
                if (string.Equals(e.Property.Name, ((EdmMember) allStructuralMembers[i]).Name, StringComparison.Ordinal))
                {
                    num = i;
                    break;
                }
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(num > -1, "The specified property was not found");
            node = node2.Children[num];
            return this.BuildSoftCast(node, e.ResultType);
        }

        public override Node Visit(DbQuantifierExpression e)
        {
            Node node = null;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((DbExpressionKind.Any == e.ExpressionKind) || (DbExpressionKind.All == e.ExpressionKind), "Invalid DbExpressionKind in DbQuantifierExpression");
            Node node2 = this.EnterExpressionBinding(e.Input);
            Node node3 = this.VisitExprAsPredicate(e.Predicate);
            if (e.ExpressionKind == DbExpressionKind.All)
            {
                node3 = this._iqtCommand.CreateNode(this._iqtCommand.CreateConditionalOp(OpType.Not), node3);
                Node node4 = this.VisitExprAsScalar(e.Predicate);
                node4 = this._iqtCommand.CreateNode(this._iqtCommand.CreateConditionalOp(OpType.IsNull), node4);
                node3 = this._iqtCommand.CreateNode(this._iqtCommand.CreateConditionalOp(OpType.Or), node3, node4);
            }
            this.ExitExpressionBinding();
            Var var = this._varMap[node2];
            node2 = this._iqtCommand.CreateNode(this._iqtCommand.CreateFilterOp(), node2, node3);
            this._varMap[node2] = var;
            node = this._iqtCommand.CreateNode(this._iqtCommand.CreateExistsOp(), node2);
            if (e.ExpressionKind == DbExpressionKind.All)
            {
                node = this._iqtCommand.CreateNode(this._iqtCommand.CreateConditionalOp(OpType.Not), node);
            }
            return node;
        }

        public override Node Visit(DbRefExpression e)
        {
            Op op = this._iqtCommand.CreateRefOp(e.EntitySet, e.ResultType);
            Node node = this.BuildSoftCast(this.VisitExprAsScalar(e.Argument), TypeHelpers.CreateKeyRowType(e.EntitySet.ElementType, this._iqtCommand.MetadataWorkspace));
            return this._iqtCommand.CreateNode(op, node);
        }

        public override Node Visit(DbRefKeyExpression e)
        {
            Op op = this._iqtCommand.CreateGetRefKeyOp(e.ResultType);
            return this.VisitUnary(e, op, new VisitExprDelegate(this.VisitExprAsScalar));
        }

        public override Node Visit(DbRelationshipNavigationExpression e)
        {
            RelProperty relProperty = new RelProperty(e.Relationship, e.NavigateFrom, e.NavigateTo);
            Op op = this._iqtCommand.CreateNavigateOp(e.ResultType, relProperty);
            Node node = this.VisitExprAsScalar(e.NavigationSource);
            return this._iqtCommand.CreateNode(op, node);
        }

        public override Node Visit(DbScanExpression e)
        {
            TableMD tableMetadata = Command.CreateTableDefinition(e.Target);
            ScanTableOp op = this._iqtCommand.CreateScanTableOp(tableMetadata);
            Node node = this._iqtCommand.CreateNode(op);
            Var var = op.Table.Columns[0];
            this._varMap[node] = var;
            return node;
        }

        public override Node Visit(DbSkipExpression expression)
        {
            Var var;
            List<SortKey> sortKeys = new List<SortKey>();
            Node node = this.VisitSortArguments(expression.Input, expression.SortOrder, sortKeys, out var);
            Node node2 = this.VisitExprAsScalar(expression.Count);
            Node node3 = this._iqtCommand.CreateNode(this._iqtCommand.CreateConstrainedSortOp(sortKeys), node, node2, this._iqtCommand.CreateNode(this._iqtCommand.CreateNullOp(this._iqtCommand.IntegerType)));
            this._varMap[node3] = var;
            return node3;
        }

        public override Node Visit(DbSortExpression e)
        {
            Var var;
            List<SortKey> sortKeys = new List<SortKey>();
            Node node = this.VisitSortArguments(e.Input, e.SortOrder, sortKeys, out var);
            SortOp op = this._iqtCommand.CreateSortOp(sortKeys);
            Node node2 = this._iqtCommand.CreateNode(op, node);
            this._varMap[node2] = var;
            return node2;
        }

        public override Node Visit(DbTreatExpression e)
        {
            Op op = this._iqtCommand.CreateTreatOp(e.ResultType);
            return this.VisitUnary(e, op, new VisitExprDelegate(this.VisitExprAsScalar));
        }

        public override Node Visit(DbUnionAllExpression e) => 
            this.VisitSetOpExpression(e);

        public override Node Visit(DbVariableReferenceExpression e)
        {
            Node node = null;
            foreach (CqtVariableScope scope in this._varScopes)
            {
                if (scope.Contains(e.VariableName))
                {
                    node = scope[e.VariableName];
                    break;
                }
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(node != null, "CQT VarRef could not be resolved in the variable scope stack");
            return node;
        }

        private Node VisitBinary(DbBinaryExpression e, Op op, VisitExprDelegate exprDelegate) => 
            this._iqtCommand.CreateNode(op, exprDelegate(e.Left), exprDelegate(e.Right));

        private Node VisitExpr(DbExpression e) => 
            e?.Accept<Node>(this);

        private static IList<Node> VisitExpr(IList<DbExpression> exprs, VisitExprDelegate exprDelegate)
        {
            List<Node> list = new List<Node>();
            for (int i = 0; i < exprs.Count; i++)
            {
                list.Add(exprDelegate(exprs[i]));
            }
            return list;
        }

        private Node VisitExprAsPredicate(DbExpression expr)
        {
            if (expr == null)
            {
                return null;
            }
            Node node = this.VisitExpr(expr);
            if (!this.IsPredicate(expr))
            {
                ComparisonOp op = this._iqtCommand.CreateComparisonOp(OpType.EQ);
                Node node2 = this._iqtCommand.CreateNode(this._iqtCommand.CreateInternalConstantOp(this._iqtCommand.BooleanType, true));
                return this._iqtCommand.CreateNode(op, node, node2);
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(!node.Op.IsRelOp, "unexpected relOp as predicate?");
            return node;
        }

        private IList<Node> VisitExprAsScalar(IList<DbExpression> exprs) => 
            VisitExpr(exprs, new VisitExprDelegate(this.VisitExprAsScalar));

        private Node VisitExprAsScalar(DbExpression expr)
        {
            if (expr == null)
            {
                return null;
            }
            Node node = this.VisitExpr(expr);
            return this.ConvertToScalarOpTree(node, expr);
        }

        private Node VisitJoin(DbExpression e, IList<DbExpressionBinding> inputs, DbExpression joinCond)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((((DbExpressionKind.CrossJoin == e.ExpressionKind) || (DbExpressionKind.InnerJoin == e.ExpressionKind)) || (DbExpressionKind.LeftOuterJoin == e.ExpressionKind)) || (DbExpressionKind.FullOuterJoin == e.ExpressionKind), "Unrecognized JoinType specified in DbJoinExpression");
            List<Node> args = new List<Node>();
            List<Var> colVars = new List<Var>();
            for (int i = 0; i < inputs.Count; i++)
            {
                Node node = this.EnterExpressionBinding(inputs[i]);
                args.Add(node);
                colVars.Add(this._varMap[node]);
            }
            Node item = this.VisitExprAsPredicate(joinCond);
            for (int j = 0; j < args.Count; j++)
            {
                this.ExitExpressionBinding();
            }
            JoinBaseOp op = null;
            switch (e.ExpressionKind)
            {
                case DbExpressionKind.InnerJoin:
                    op = this._iqtCommand.CreateInnerJoinOp();
                    break;

                case DbExpressionKind.LeftOuterJoin:
                    op = this._iqtCommand.CreateLeftOuterJoinOp();
                    break;

                case DbExpressionKind.CrossJoin:
                    op = this._iqtCommand.CreateCrossJoinOp();
                    break;

                case DbExpressionKind.FullOuterJoin:
                    op = this._iqtCommand.CreateFullOuterJoinOp();
                    break;
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(op != null, "Unrecognized JoinOp specified in DbJoinExpression, no JoinOp was produced");
            if (e.ExpressionKind != DbExpressionKind.CrossJoin)
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(item != null, "Non CrossJoinOps must specify a join condition");
                args.Add(item);
            }
            return this.ProjectNewRecord(this._iqtCommand.CreateNode(op, args), ExtractElementRowType(e.ResultType), colVars);
        }

        private Node VisitSetOpExpression(DbBinaryExpression expression)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(((DbExpressionKind.Except == expression.ExpressionKind) || (DbExpressionKind.Intersect == expression.ExpressionKind)) || (DbExpressionKind.UnionAll == expression.ExpressionKind), "Non-SetOp DbExpression used as argument to VisitSetOpExpression");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsCollectionType(expression.ResultType), "SetOp DbExpression does not have collection result type?");
            Node node = this.EnsureRelOp(this.VisitExpr(expression.Left));
            Node node2 = this.EnsureRelOp(this.VisitExpr(expression.Right));
            node = this.BuildSoftCast(node, expression.ResultType);
            node2 = this.BuildSoftCast(node2, expression.ResultType);
            Var key = this._iqtCommand.CreateSetOpVar(TypeHelpers.GetEdmType<CollectionType>(expression.ResultType).TypeUsage);
            System.Data.Query.InternalTrees.VarMap leftMap = new System.Data.Query.InternalTrees.VarMap();
            leftMap.Add(key, this._varMap[node]);
            System.Data.Query.InternalTrees.VarMap rightMap = new System.Data.Query.InternalTrees.VarMap();
            rightMap.Add(key, this._varMap[node2]);
            Op op = null;
            switch (expression.ExpressionKind)
            {
                case DbExpressionKind.Except:
                    op = this._iqtCommand.CreateExceptOp(leftMap, rightMap);
                    break;

                case DbExpressionKind.Intersect:
                    op = this._iqtCommand.CreateIntersectOp(leftMap, rightMap);
                    break;

                case DbExpressionKind.UnionAll:
                    op = this._iqtCommand.CreateUnionAllOp(leftMap, rightMap);
                    break;
            }
            Node node3 = this._iqtCommand.CreateNode(op, node, node2);
            this._varMap[node3] = key;
            return node3;
        }

        private Node VisitSortArguments(DbExpressionBinding input, IList<DbSortClause> sortOrder, List<SortKey> sortKeys, out Var inputVar)
        {
            Node node = this.EnterExpressionBinding(input);
            inputVar = this._varMap[node];
            VarVec vars = this._iqtCommand.CreateVarVec();
            vars.Set(inputVar);
            List<Node> args = new List<Node>();
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(sortKeys.Count == 0, "Non-empty SortKey list before adding converted SortClauses");
            for (int i = 0; i < sortOrder.Count; i++)
            {
                Var var;
                DbSortClause clause = sortOrder[i];
                Node definingExpr = this.VisitExprAsScalar(clause.Expression);
                ScalarOp op = definingExpr.Op as ScalarOp;
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(op != null, "DbSortClause Expression converted to non-ScalarOp");
                args.Add(this._iqtCommand.CreateVarDefNode(definingExpr, out var));
                vars.Set(var);
                SortKey item = null;
                if (string.IsNullOrEmpty(clause.Collation))
                {
                    item = Command.CreateSortKey(var, clause.Ascending);
                }
                else
                {
                    item = Command.CreateSortKey(var, clause.Ascending, clause.Collation);
                }
                sortKeys.Add(item);
            }
            this.ExitExpressionBinding();
            return this._iqtCommand.CreateNode(this._iqtCommand.CreateProjectOp(vars), node, this._iqtCommand.CreateNode(this._iqtCommand.CreateVarDefListOp(), args));
        }

        private Node VisitUnary(DbUnaryExpression e, Op op, VisitExprDelegate exprDelegate) => 
            this._iqtCommand.CreateNode(op, exprDelegate(e.Argument));

        internal Dictionary<Node, Var> VarMap =>
            this._varMap;

        private abstract class CqtVariableScope
        {
            protected CqtVariableScope()
            {
            }

            internal abstract bool Contains(string varName);

            internal abstract Node this[string varName] { get; }
        }

        private class ExpressionBindingScope : ITreeGenerator.CqtVariableScope
        {
            private Command _tree;
            private Var _var;
            private string _varName;

            internal ExpressionBindingScope(Command iqtTree, string name, Var iqtVar)
            {
                this._tree = iqtTree;
                this._varName = name;
                this._var = iqtVar;
            }

            internal override bool Contains(string name) => 
                (this._varName == name);

            internal override Node this[string name]
            {
                get
                {
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(name == this._varName, "huh?");
                    return this._tree.CreateNode(this._tree.CreateVarRefOp(this._var));
                }
            }

            internal Var ScopeVar =>
                this._var;
        }

        private class LambdaScope : ITreeGenerator.CqtVariableScope
        {
            private Dictionary<string, Node> _arguments;
            private Command _command;
            private Dictionary<Node, bool> _referencedArgs;
            private ITreeGenerator _treeGen;

            internal LambdaScope(ITreeGenerator treeGen, Command command, Dictionary<string, Node> args)
            {
                this._treeGen = treeGen;
                this._command = command;
                this._arguments = args;
                this._referencedArgs = new Dictionary<Node, bool>(this._arguments.Count);
            }

            internal override bool Contains(string name) => 
                this._arguments.ContainsKey(name);

            private void MapCopiedNodeVars(IList<Node> sources, IList<Node> copies, Dictionary<Var, Var> varMappings)
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(sources.Count == copies.Count, "Source/Copy Node count mismatch");
                for (int i = 0; i < sources.Count; i++)
                {
                    Node key = sources[i];
                    Node node2 = copies[i];
                    if (key.Children.Count > 0)
                    {
                        this.MapCopiedNodeVars(key.Children, node2.Children, varMappings);
                    }
                    Var var = null;
                    if (this._treeGen.VarMap.TryGetValue(key, out var))
                    {
                        System.Data.Query.PlanCompiler.PlanCompiler.Assert(varMappings.ContainsKey(var), "No mapping found for Var in Var to Var map from OpCopier");
                        this._treeGen.VarMap[node2] = varMappings[var];
                    }
                }
            }

            internal override Node this[string name]
            {
                get
                {
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(this._arguments.ContainsKey(name), "LambdaScope indexer called for invalid Var");
                    Node key = this._arguments[name];
                    if (this._referencedArgs.ContainsKey(key))
                    {
                        VarMap varMap = null;
                        Node node2 = OpCopier.Copy(this._command, key, out varMap);
                        if (varMap.Count > 0)
                        {
                            List<Node> sources = new List<Node>(1) {
                                key
                            };
                            List<Node> copies = new List<Node>(1) {
                                node2
                            };
                            this.MapCopiedNodeVars(sources, copies, varMap);
                        }
                        return node2;
                    }
                    this._referencedArgs[key] = true;
                    return key;
                }
            }
        }

        private delegate Node VisitExprDelegate(DbExpression e);
    }
}

