namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal abstract class BasicVisitor<T_Identifier> : Visitor<T_Identifier, BoolExpr<T_Identifier>>
    {
        protected BasicVisitor()
        {
        }

        private IEnumerable<BoolExpr<T_Identifier>> AcceptChildren(IEnumerable<BoolExpr<T_Identifier>> children)
        {
            foreach (BoolExpr<T_Identifier> iteratorVariable0 in children)
            {
                yield return iteratorVariable0.Accept<BoolExpr<T_Identifier>>(this);
            }
        }

        internal override BoolExpr<T_Identifier> VisitAnd(AndExpr<T_Identifier> expression) => 
            new AndExpr<T_Identifier>(this.AcceptChildren(expression.Children));

        internal override BoolExpr<T_Identifier> VisitFalse(FalseExpr<T_Identifier> expression) => 
            expression;

        internal override BoolExpr<T_Identifier> VisitNot(NotExpr<T_Identifier> expression) => 
            new NotExpr<T_Identifier>(expression.Child.Accept<BoolExpr<T_Identifier>>(this));

        internal override BoolExpr<T_Identifier> VisitOr(OrExpr<T_Identifier> expression) => 
            new OrExpr<T_Identifier>(this.AcceptChildren(expression.Children));

        internal override BoolExpr<T_Identifier> VisitTerm(TermExpr<T_Identifier> expression) => 
            expression;

        internal override BoolExpr<T_Identifier> VisitTrue(TrueExpr<T_Identifier> expression) => 
            expression;

        [CompilerGenerated]
        private sealed class <AcceptChildren>d__0 : IEnumerable<BoolExpr<T_Identifier>>, IEnumerable, IEnumerator<BoolExpr<T_Identifier>>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private BoolExpr<T_Identifier> <>2__current;
            public IEnumerable<BoolExpr<T_Identifier>> <>3__children;
            public BasicVisitor<T_Identifier> <>4__this;
            public IEnumerator<BoolExpr<T_Identifier>> <>7__wrap2;
            private int <>l__initialThreadId;
            public BoolExpr<T_Identifier> <child>5__1;
            public IEnumerable<BoolExpr<T_Identifier>> children;

            [DebuggerHidden]
            public <AcceptChildren>d__0(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finally3()
            {
                this.<>1__state = -1;
                if (this.<>7__wrap2 != null)
                {
                    this.<>7__wrap2.Dispose();
                }
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
                            this.<>7__wrap2 = this.children.GetEnumerator();
                            this.<>1__state = 1;
                            goto Label_0076;

                        case 2:
                            this.<>1__state = 1;
                            goto Label_0076;

                        default:
                            goto Label_0089;
                    }
                Label_003C:
                    this.<child>5__1 = this.<>7__wrap2.Current;
                    this.<>2__current = this.<child>5__1.Accept<BoolExpr<T_Identifier>>(this.<>4__this);
                    this.<>1__state = 2;
                    return true;
                Label_0076:
                    if (this.<>7__wrap2.MoveNext())
                    {
                        goto Label_003C;
                    }
                    this.<>m__Finally3();
                Label_0089:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            IEnumerator<BoolExpr<T_Identifier>> IEnumerable<BoolExpr<T_Identifier>>.GetEnumerator()
            {
                BasicVisitor<T_Identifier>.<AcceptChildren>d__0 d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (BasicVisitor<T_Identifier>.<AcceptChildren>d__0) this;
                }
                else
                {
                    d__ = new BasicVisitor<T_Identifier>.<AcceptChildren>d__0(0) {
                        <>4__this = this.<>4__this
                    };
                }
                d__.children = this.<>3__children;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Data.Common.Utils.Boolean.BoolExpr<T_Identifier>>.GetEnumerator();

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

            BoolExpr<T_Identifier> IEnumerator<BoolExpr<T_Identifier>>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

