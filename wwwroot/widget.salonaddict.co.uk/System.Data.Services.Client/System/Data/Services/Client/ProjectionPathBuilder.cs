namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    internal class ProjectionPathBuilder
    {
        private readonly Stack<bool> entityInScope = new Stack<bool>();
        private readonly Stack<Expression> parameterEntries = new Stack<Expression>();
        private readonly Stack<ParameterExpression> parameterExpressions = new Stack<ParameterExpression>();
        private readonly Stack<Expression> parameterExpressionTypes = new Stack<Expression>();
        private readonly Stack<Type> parameterProjectionTypes = new Stack<Type>();
        private readonly List<MemberInitRewrite> rewrites = new List<MemberInitRewrite>();

        internal ProjectionPathBuilder()
        {
        }

        internal void EnterLambdaScope(LambdaExpression lambda, Expression entry, Expression expectedType)
        {
            ParameterExpression item = lambda.Parameters[0];
            Type t = lambda.Body.Type;
            bool flag = ClientType.CheckElementTypeIsEntity(t);
            this.entityInScope.Push(flag);
            this.parameterExpressions.Push(item);
            this.parameterExpressionTypes.Push(expectedType);
            this.parameterEntries.Push(entry);
            this.parameterProjectionTypes.Push(t);
        }

        internal void EnterMemberInit(MemberInitExpression init)
        {
            bool item = ClientType.CheckElementTypeIsEntity(init.Type);
            this.entityInScope.Push(item);
        }

        internal Expression GetRewrite(Expression expression)
        {
            List<string> list = new List<string>();
            while (expression.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression expression2 = (MemberExpression) expression;
                list.Add(expression2.Member.Name);
                expression = expression2.Expression;
            }
            foreach (MemberInitRewrite rewrite in this.rewrites)
            {
                if ((rewrite.Root != expression) || (list.Count != rewrite.MemberNames.Length))
                {
                    continue;
                }
                bool flag = true;
                for (int i = 0; (i < list.Count) && (i < rewrite.MemberNames.Length); i++)
                {
                    if (list[(list.Count - i) - 1] != rewrite.MemberNames[i])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    return rewrite.RewriteExpression;
                }
            }
            return null;
        }

        internal void LeaveLambdaScope()
        {
            this.entityInScope.Pop();
            this.parameterExpressions.Pop();
            this.parameterExpressionTypes.Pop();
            this.parameterEntries.Pop();
            this.parameterProjectionTypes.Pop();
        }

        internal void LeaveMemberInit()
        {
            this.entityInScope.Pop();
        }

        internal void RegisterRewrite(Expression root, string[] names, Expression rewriteExpression)
        {
            MemberInitRewrite item = new MemberInitRewrite {
                Root = root,
                MemberNames = names,
                RewriteExpression = rewriteExpression
            };
            this.rewrites.Add(item);
            this.parameterEntries.Push(rewriteExpression);
        }

        internal void RevokeRewrite(Expression root, string[] names)
        {
            for (int i = 0; i < this.rewrites.Count; i++)
            {
                if ((this.rewrites[i].Root != root) || (names.Length != this.rewrites[i].MemberNames.Length))
                {
                    continue;
                }
                bool flag = true;
                for (int j = 0; j < names.Length; j++)
                {
                    if (names[j] != this.rewrites[i].MemberNames[j])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    this.rewrites.RemoveAt(i);
                    this.parameterEntries.Pop();
                    return;
                }
            }
        }

        public override string ToString()
        {
            string str = "ProjectionPathBuilder: ";
            if (this.parameterExpressions.Count == 0)
            {
                return (str + "(empty)");
            }
            object obj2 = str;
            return string.Concat(new object[] { obj2, "entity:", this.CurrentIsEntity, " param:", this.ParameterEntryInScope });
        }

        internal bool CurrentIsEntity =>
            this.entityInScope.Peek();

        internal Expression ExpectedParamTypeInScope =>
            this.parameterExpressionTypes.Peek();

        internal bool HasRewrites =>
            (this.rewrites.Count > 0);

        internal Expression LambdaParameterInScope =>
            this.parameterExpressions.Peek();

        internal Expression ParameterEntryInScope =>
            this.parameterEntries.Peek();

        internal class MemberInitRewrite
        {
            internal string[] MemberNames { get; set; }

            internal Expression RewriteExpression { get; set; }

            internal Expression Root { get; set; }
        }
    }
}

