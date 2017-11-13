namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal class BooleanExpressionTermRewriter<T_From, T_To> : Visitor<T_From, BoolExpr<T_To>>
    {
        private readonly Func<TermExpr<T_From>, BoolExpr<T_To>> _translator;

        internal BooleanExpressionTermRewriter(Func<TermExpr<T_From>, BoolExpr<T_To>> translator)
        {
            this._translator = translator;
        }

        internal override BoolExpr<T_To> VisitAnd(AndExpr<T_From> expression) => 
            ((BoolExpr<T_To>) new AndExpr<T_To>(this.VisitChildren(expression)));

        private IEnumerable<BoolExpr<T_To>> VisitChildren(TreeExpr<T_From> expression)
        {
            foreach (BoolExpr<T_From> iteratorVariable0 in expression.Children)
            {
                yield return iteratorVariable0.Accept<BoolExpr<T_To>>(this);
            }
        }

        internal override BoolExpr<T_To> VisitFalse(FalseExpr<T_From> expression) => 
            ((BoolExpr<T_To>) FalseExpr<T_To>.Value);

        internal override BoolExpr<T_To> VisitNot(NotExpr<T_From> expression) => 
            ((BoolExpr<T_To>) new NotExpr<T_To>(expression.Child.Accept<BoolExpr<T_To>>(this)));

        internal override BoolExpr<T_To> VisitOr(OrExpr<T_From> expression) => 
            ((BoolExpr<T_To>) new OrExpr<T_To>(this.VisitChildren(expression)));

        internal override BoolExpr<T_To> VisitTerm(TermExpr<T_From> expression) => 
            this._translator(expression);

        internal override BoolExpr<T_To> VisitTrue(TrueExpr<T_From> expression) => 
            ((BoolExpr<T_To>) TrueExpr<T_To>.Value);

        [CompilerGenerated]
        private sealed class <VisitChildren>d__0 : IEnumerable<BoolExpr<T_To>>, IEnumerable, IEnumerator<BoolExpr<T_To>>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private BoolExpr<T_To> <>2__current;
            public TreeExpr<T_From> <>3__expression;
            public BooleanExpressionTermRewriter<T_From, T_To> <>4__this;
            public HashSet<BoolExpr<T_From>>.Enumerator <>7__wrap2;
            private int <>l__initialThreadId;
            public BoolExpr<T_From> <child>5__1;
            public TreeExpr<T_From> expression;

            [DebuggerHidden]
            public <VisitChildren>d__0(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finally3()
            {
                this.<>1__state = -1;
                this.<>7__wrap2.Dispose();
            }

            private bool MoveNext()
            {
                bool flag;
                try
                {
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            this.<>7__wrap2 = this.expression.Children.GetEnumerator();
                            this.<>1__state = 1;
                            goto Label_007B;

                        case 2:
                            this.<>1__state = 1;
                            goto Label_007B;

                        default:
                            goto Label_008E;
                    }
                Label_0041:
                    this.<child>5__1 = this.<>7__wrap2.Current;
                    this.<>2__current = this.<child>5__1.Accept<BoolExpr<T_To>>(this.<>4__this);
                    this.<>1__state = 2;
                    return true;
                Label_007B:
                    if (this.<>7__wrap2.MoveNext())
                    {
                        goto Label_0041;
                    }
                    this.<>m__Finally3();
                Label_008E:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            IEnumerator<BoolExpr<T_To>> IEnumerable<BoolExpr<T_To>>.GetEnumerator()
            {
                BooleanExpressionTermRewriter<T_From, T_To>.<VisitChildren>d__0 d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (BooleanExpressionTermRewriter<T_From, T_To>.<VisitChildren>d__0) this;
                }
                else
                {
                    d__ = new BooleanExpressionTermRewriter<T_From, T_To>.<VisitChildren>d__0(0) {
                        <>4__this = this.<>4__this
                    };
                }
                d__.expression = this.<>3__expression;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Data.Common.Utils.Boolean.BoolExpr<T_To>>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
                switch (this.<>1__state)
                {
                    case 1:
                    case 2:
                        try
                        {
                        }
                        finally
                        {
                            this.<>m__Finally3();
                        }
                        return;
                }
            }

            BoolExpr<T_To> IEnumerator<BoolExpr<T_To>>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

