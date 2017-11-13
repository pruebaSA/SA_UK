namespace System.Data.Mapping.ViewGeneration.QueryRewriting
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal abstract class Tile<T_Query> where T_Query: ITileQuery
    {
        private readonly TileOpKind m_opKind;
        private readonly T_Query m_query;

        protected Tile(TileOpKind opKind, T_Query query)
        {
            this.m_opKind = opKind;
            this.m_query = query;
        }

        public IEnumerable<T_Query> GetNamedQueries() => 
            Tile<T_Query>.GetNamedQueries((Tile<T_Query>) this);

        private static IEnumerable<T_Query> GetNamedQueries(Tile<T_Query> rewriting)
        {
            if (rewriting != null)
            {
                if (rewriting.OpKind == TileOpKind.Named)
                {
                    yield return ((TileNamed<T_Query>) rewriting).NamedQuery;
                }
                else
                {
                    foreach (T_Query iteratorVariable0 in Tile<T_Query>.GetNamedQueries(rewriting.Arg1))
                    {
                        yield return iteratorVariable0;
                    }
                    foreach (T_Query iteratorVariable1 in Tile<T_Query>.GetNamedQueries(rewriting.Arg2))
                    {
                        yield return iteratorVariable1;
                    }
                }
            }
        }

        internal abstract Tile<T_Query> Replace(Tile<T_Query> oldTile, Tile<T_Query> newTile);
        public override string ToString()
        {
            if (this.Description != null)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}: [{1}]", new object[] { this.Description, this.Query });
            }
            return string.Format(CultureInfo.InvariantCulture, "[{0}]", new object[] { this.Query });
        }

        public abstract Tile<T_Query> Arg1 { get; }

        public abstract Tile<T_Query> Arg2 { get; }

        public abstract string Description { get; }

        public TileOpKind OpKind =>
            this.m_opKind;

        public T_Query Query =>
            this.m_query;

        [CompilerGenerated]
        private sealed class <GetNamedQueries>d__0 : IEnumerable<T_Query>, IEnumerable, IEnumerator<T_Query>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private T_Query <>2__current;
            public Tile<T_Query> <>3__rewriting;
            public IEnumerator<T_Query> <>7__wrap3;
            public IEnumerator<T_Query> <>7__wrap5;
            private int <>l__initialThreadId;
            public T_Query <query>5__1;
            public T_Query <query>5__2;
            public Tile<T_Query> rewriting;

            [DebuggerHidden]
            public <GetNamedQueries>d__0(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finally4()
            {
                this.<>1__state = -1;
                if (this.<>7__wrap3 != null)
                {
                    this.<>7__wrap3.Dispose();
                }
            }

            private void <>m__Finally6()
            {
                this.<>1__state = -1;
                if (this.<>7__wrap5 != null)
                {
                    this.<>7__wrap5.Dispose();
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
                            if (this.rewriting == null)
                            {
                                goto Label_0149;
                            }
                            if (this.rewriting.OpKind != TileOpKind.Named)
                            {
                                break;
                            }
                            this.<>2__current = ((TileNamed<T_Query>) this.rewriting).NamedQuery;
                            this.<>1__state = 1;
                            return true;

                        case 1:
                            this.<>1__state = -1;
                            goto Label_0149;

                        case 3:
                            goto Label_00C9;

                        case 5:
                            goto Label_012F;

                        default:
                            goto Label_0149;
                    }
                    this.<>7__wrap3 = Tile<T_Query>.GetNamedQueries(this.rewriting.Arg1).GetEnumerator();
                    this.<>1__state = 2;
                    while (this.<>7__wrap3.MoveNext())
                    {
                        this.<query>5__1 = this.<>7__wrap3.Current;
                        this.<>2__current = this.<query>5__1;
                        this.<>1__state = 3;
                        return true;
                    Label_00C9:
                        this.<>1__state = 2;
                    }
                    this.<>m__Finally4();
                    this.<>7__wrap5 = Tile<T_Query>.GetNamedQueries(this.rewriting.Arg2).GetEnumerator();
                    this.<>1__state = 4;
                    while (this.<>7__wrap5.MoveNext())
                    {
                        this.<query>5__2 = this.<>7__wrap5.Current;
                        this.<>2__current = this.<query>5__2;
                        this.<>1__state = 5;
                        return true;
                    Label_012F:
                        this.<>1__state = 4;
                    }
                    this.<>m__Finally6();
                Label_0149:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            IEnumerator<T_Query> IEnumerable<T_Query>.GetEnumerator()
            {
                Tile<T_Query>.<GetNamedQueries>d__0 d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (Tile<T_Query>.<GetNamedQueries>d__0) this;
                }
                else
                {
                    d__ = new Tile<T_Query>.<GetNamedQueries>d__0(0);
                }
                d__.rewriting = this.<>3__rewriting;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<T_Query>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
                switch (this.<>1__state)
                {
                    case 2:
                    case 3:
                        try
                        {
                        }
                        finally
                        {
                            this.<>m__Finally4();
                        }
                        break;

                    case 4:
                    case 5:
                        try
                        {
                        }
                        finally
                        {
                            this.<>m__Finally6();
                        }
                        return;
                }
            }

            T_Query IEnumerator<T_Query>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

