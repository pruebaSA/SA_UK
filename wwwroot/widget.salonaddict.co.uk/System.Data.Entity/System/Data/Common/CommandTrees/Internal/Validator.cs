namespace System.Data.Common.CommandTrees.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    internal class Validator : BasicExpressionVisitor
    {
        private Stack<int> _cycleStack = new Stack<int>();
        private Stack<Dictionary<string, TypeUsage>> _scopes = new Stack<Dictionary<string, TypeUsage>>();

        private void AddToScope(IEnumerable<KeyValuePair<string, TypeUsage>> scopeElements)
        {
            Dictionary<string, TypeUsage> dictionary = this._scopes.Peek();
            foreach (KeyValuePair<string, TypeUsage> pair in scopeElements)
            {
                dictionary.Add(pair.Key, pair.Value);
            }
        }

        private void AddToScope(string strName, TypeUsage t)
        {
            this._scopes.Peek().Add(strName, t);
        }

        private TypeUsage FindInScopes(string name)
        {
            TypeUsage usage = null;
            foreach (Dictionary<string, TypeUsage> dictionary in this._scopes)
            {
                if (dictionary.TryGetValue(name, out usage))
                {
                    return usage;
                }
            }
            return null;
        }

        private static void Invalid(DbCommandTree tree, string message)
        {
            InvalidCommandTreeException e = new InvalidCommandTreeException(message);
            EntityUtil.TraceExceptionAsReturnValue(e);
            throw e;
        }

        private void PopScope()
        {
            this._scopes.Pop();
        }

        private void PushScope()
        {
            this._scopes.Push(new Dictionary<string, TypeUsage>());
        }

        private void Reset()
        {
            this._scopes.Clear();
            this._cycleStack.Clear();
        }

        private void Validate(DbExpression expression)
        {
            this.Reset();
            this.VisitExpression(expression);
        }

        internal void Validate(DbQueryCommandTree cmd)
        {
            EntityUtil.CheckArgumentNull<DbQueryCommandTree>(cmd, "cmd");
            using (new EntityBid.ScopeAuto("<cqti.Validator.Validate(DbQueryCommandTree)|API> cmd=%d#", cmd.ObjectId))
            {
                if (cmd.Query == null)
                {
                    Invalid(cmd, Strings.Cqt_QueryTree_NullQueryInvalid);
                }
                this.Validate(cmd.Query);
            }
        }

        public override void Visit(DbCrossJoinExpression e)
        {
            for (int i = 0; i < e.Inputs.Count; i++)
            {
                this.VisitExpression(e.Inputs[i].Expression);
            }
        }

        public override void Visit(DbJoinExpression e)
        {
            List<KeyValuePair<string, TypeUsage>> scopeElements = new List<KeyValuePair<string, TypeUsage>>(2);
            this.VisitExpression(e.Left.Expression);
            scopeElements.Add(new KeyValuePair<string, TypeUsage>(e.Left.VariableName, e.Left.VariableType));
            this.VisitExpression(e.Right.Expression);
            scopeElements.Add(new KeyValuePair<string, TypeUsage>(e.Right.VariableName, e.Right.VariableType));
            this.PushScope();
            this.AddToScope(scopeElements);
            this.VisitExpression(e.JoinCondition);
            this.PopScope();
        }

        public override void Visit(DbParameterReferenceExpression e)
        {
            if (!e.CommandTree.HasParameter(e.ParameterName, e.ResultType))
            {
                Invalid(e.CommandTree, Strings.Cqt_CommandTree_NoParameterExists);
            }
        }

        public override void Visit(DbVariableReferenceExpression e)
        {
            TypeUsage usage = this.FindInScopes(e.VariableName);
            if (usage == null)
            {
                Invalid(e.CommandTree, Strings.Cqt_Validator_VarRefInvalid(e.VariableName));
            }
            if (!TypeSemantics.IsStructurallyEqualTo(e.ResultType, usage))
            {
                Invalid(e.CommandTree, Strings.Cqt_Validator_VarRefTypeMismatch(e.VariableName));
            }
        }

        public override void VisitExpression(DbExpression expression)
        {
            foreach (int num in this._cycleStack)
            {
                if (num == expression.ObjectId)
                {
                    Invalid(expression.CommandTree, Strings.Cqt_Validator_CycleDetected);
                }
            }
            this._cycleStack.Push(expression.ObjectId);
            base.VisitExpression(expression);
            this._cycleStack.Pop();
        }

        protected override void VisitExpressionBindingPost(DbExpressionBinding b)
        {
            base.VisitExpressionBindingPost(b);
            this.PopScope();
        }

        protected override void VisitExpressionBindingPre(DbExpressionBinding b)
        {
            base.VisitExpressionBindingPre(b);
            this.PushScope();
            this.AddToScope(b.VariableName, b.VariableType);
        }

        protected override void VisitGroupExpressionBindingMid(DbGroupExpressionBinding gb)
        {
            base.VisitGroupExpressionBindingMid(gb);
            this.PopScope();
            this.PushScope();
            this.AddToScope(gb.GroupVariableName, gb.GroupVariableType);
        }

        protected override void VisitGroupExpressionBindingPost(DbGroupExpressionBinding gb)
        {
            base.VisitGroupExpressionBindingPost(gb);
            this.PopScope();
        }

        protected override void VisitGroupExpressionBindingPre(DbGroupExpressionBinding gb)
        {
            base.VisitGroupExpressionBindingPre(gb);
            this.PushScope();
            this.AddToScope(gb.VariableName, gb.VariableType);
        }

        protected override void VisitLambdaFunctionPost(EdmFunction function, DbExpression body)
        {
            base.VisitLambdaFunctionPost(function, body);
            this.PopScope();
        }

        protected override void VisitLambdaFunctionPre(EdmFunction function, DbExpression body)
        {
            base.VisitLambdaFunctionPre(function, body);
            this.PushScope();
            foreach (FunctionParameter parameter in function.Parameters)
            {
                this.AddToScope(parameter.Name, parameter.TypeUsage);
            }
        }
    }
}

