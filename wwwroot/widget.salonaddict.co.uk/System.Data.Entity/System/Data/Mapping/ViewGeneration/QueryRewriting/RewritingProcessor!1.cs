namespace System.Data.Mapping.ViewGeneration.QueryRewriting
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class RewritingProcessor<T_Tile> : TileProcessor<T_Tile> where T_Tile: class
    {
        private int m_numDifference;
        private int m_numErrors;
        private int m_numIntersection;
        private int m_numSATChecks;
        private int m_numUnion;
        private TileProcessor<T_Tile> m_tileProcessor;
        public int MAX_PERMUTATIONS;
        public int MIN_PERMUTATIONS;
        public double PERMUTE_FRACTION;
        public bool REORDER_VIEWS;
        private static Random rnd;

        static RewritingProcessor()
        {
            RewritingProcessor<T_Tile>.rnd = new Random(0x5e3);
        }

        public RewritingProcessor(TileProcessor<T_Tile> tileProcessor)
        {
            this.m_tileProcessor = tileProcessor;
        }

        public void AddError()
        {
            this.m_numErrors++;
        }

        public static IEnumerable<T> AllButOne<T>(IEnumerable<T> list, int toSkipPosition)
        {
            int iteratorVariable0 = 0;
            IEnumerator<T> enumerator = (IEnumerator<T>) this.list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                if (iteratorVariable0++ != this.toSkipPosition)
                {
                    yield return current;
                }
            }
            this.<>m__Finally4();
        }

        internal override T_Tile AntiSemiJoin(T_Tile a, T_Tile b)
        {
            this.m_numDifference++;
            return this.m_tileProcessor.AntiSemiJoin(a, b);
        }

        public static IEnumerable<T> Concat<T>(T value, IEnumerable<T> rest)
        {
            yield return this.value;
            IEnumerator<T> enumerator = (IEnumerator<T>) this.rest.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                yield return current;
            }
            this.<>m__Finallya();
        }

        public int CountOperators(T_Tile query)
        {
            int num = 0;
            if ((query != null) && (this.GetOpKind(query) != TileOpKind.Named))
            {
                num++;
                num += this.CountOperators(this.GetArg1(query));
                num += this.CountOperators(this.GetArg2(query));
            }
            return num;
        }

        public int CountViews(T_Tile query)
        {
            HashSet<T_Tile> views = new HashSet<T_Tile>();
            this.GatherViews(query, views);
            return views.Count;
        }

        public void GatherViews(T_Tile rewriting, HashSet<T_Tile> views)
        {
            if (rewriting != null)
            {
                if (this.GetOpKind(rewriting) == TileOpKind.Named)
                {
                    views.Add(rewriting);
                }
                else
                {
                    this.GatherViews(this.GetArg1(rewriting), views);
                    this.GatherViews(this.GetArg2(rewriting), views);
                }
            }
        }

        internal override T_Tile GetArg1(T_Tile tile) => 
            this.m_tileProcessor.GetArg1(tile);

        internal override T_Tile GetArg2(T_Tile tile) => 
            this.m_tileProcessor.GetArg2(tile);

        internal override TileOpKind GetOpKind(T_Tile tile) => 
            this.m_tileProcessor.GetOpKind(tile);

        public void GetStatistics(out int numSATChecks, out int numIntersection, out int numUnion, out int numDifference, out int numErrors)
        {
            numSATChecks = this.m_numSATChecks;
            numIntersection = this.m_numIntersection;
            numUnion = this.m_numUnion;
            numDifference = this.m_numDifference;
            numErrors = this.m_numErrors;
        }

        internal bool IsContainedIn(T_Tile a, T_Tile b)
        {
            T_Tile tile = this.AntiSemiJoin(a, b);
            return this.IsEmpty(tile);
        }

        public bool IsDisjointFrom(T_Tile a, T_Tile b) => 
            this.m_tileProcessor.IsEmpty(this.Join(a, b));

        internal override bool IsEmpty(T_Tile a)
        {
            this.m_numSATChecks++;
            return this.m_tileProcessor.IsEmpty(a);
        }

        internal bool IsEquivalentTo(T_Tile a, T_Tile b)
        {
            bool flag = this.IsContainedIn(a, b);
            bool flag2 = this.IsContainedIn(b, a);
            return (flag && flag2);
        }

        internal override T_Tile Join(T_Tile a, T_Tile b)
        {
            if (a == null)
            {
                return b;
            }
            this.m_numIntersection++;
            return this.m_tileProcessor.Join(a, b);
        }

        public static IEnumerable<IEnumerable<T>> Permute<T>(IEnumerable<T> list)
        {
            IEnumerable<T> iteratorVariable0 = null;
            int iteratorVariable1 = 0;
            IEnumerator<T> enumerator = (IEnumerator<T>) this.list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                iteratorVariable0 = (IEnumerable<T>) RewritingProcessor<T_Tile>.AllButOne<T>(this.list, iteratorVariable1++);
                IEnumerator<IEnumerable<T>> iteratorVariable5 = RewritingProcessor<T_Tile>.Permute<T>((IEnumerable<T>) iteratorVariable0).GetEnumerator();
                while (iteratorVariable5.MoveNext())
                {
                    IEnumerable<T> iteratorVariable3 = iteratorVariable5.Current;
                    yield return RewritingProcessor<T_Tile>.Concat<T>(current, (IEnumerable<T>) iteratorVariable3);
                }
                this.<>m__Finally15();
            }
            this.<>m__Finally13();
            if (iteratorVariable0 == null)
            {
                yield return this.list;
            }
        }

        public void PrintStatistics()
        {
            Console.WriteLine("{0} containment checks, {4} set operations ({1} intersections + {2} unions + {3} differences)", new object[] { this.m_numSATChecks, this.m_numIntersection, this.m_numUnion, this.m_numDifference, (this.m_numIntersection + this.m_numUnion) + this.m_numDifference });
            Console.WriteLine("{0} errors", this.m_numErrors);
        }

        public static List<T> RandomPermutation<T>(IEnumerable<T> input)
        {
            List<T> list = new List<T>(input);
            for (int i = 0; i < list.Count; i++)
            {
                int num2 = RewritingProcessor<T_Tile>.rnd.Next(list.Count);
                T local = list[i];
                list[i] = list[num2];
                list[num2] = local;
            }
            return list;
        }

        public static IEnumerable<T> Reverse<T>(IEnumerable<T> input, HashSet<T> filter)
        {
            List<T> iteratorVariable0 = new List<T>(this.input);
            iteratorVariable0.Reverse();
            List<T>.Enumerator enumerator = (List<T>.Enumerator) iteratorVariable0.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                if (this.filter.Contains(current))
                {
                    yield return current;
                }
            }
            this.<>m__Finally1c();
        }

        public bool RewriteQuery(T_Tile toFill, T_Tile toAvoid, IEnumerable<T_Tile> views, out T_Tile rewriting)
        {
            if (!this.RewriteQueryOnce(toFill, toAvoid, views, out rewriting))
            {
                return false;
            }
            HashSet<T_Tile> set = new HashSet<T_Tile>();
            this.GatherViews(rewriting, set);
            int num = this.CountOperators(rewriting);
            int num2 = 0;
            int num3 = Math.Min(this.MAX_PERMUTATIONS, Math.Max(this.MIN_PERMUTATIONS, (int) (set.Count * this.PERMUTE_FRACTION)));
            while (num2++ < num3)
            {
                T_Tile local;
                IEnumerable<T_Tile> enumerable;
                if (num2 == 1)
                {
                    enumerable = RewritingProcessor<T_Tile>.Reverse<T_Tile>(views, set);
                }
                else
                {
                    enumerable = RewritingProcessor<T_Tile>.RandomPermutation<T_Tile>(set);
                }
                this.RewriteQueryOnce(toFill, toAvoid, enumerable, out local);
                int num4 = this.CountOperators(local);
                if (num4 < num)
                {
                    num = num4;
                    rewriting = local;
                }
                HashSet<T_Tile> set2 = new HashSet<T_Tile>();
                this.GatherViews(local, set2);
                set = set2;
            }
            return true;
        }

        public bool RewriteQueryOnce(T_Tile toFill, T_Tile toAvoid, IEnumerable<T_Tile> views, out T_Tile rewriting)
        {
            List<T_Tile> list = new List<T_Tile>(views);
            return RewritingPass<T_Tile>.RewriteQuery(toFill, toAvoid, out rewriting, list, (RewritingProcessor<T_Tile>) this);
        }

        internal override T_Tile Union(T_Tile a, T_Tile b)
        {
            this.m_numUnion++;
            return this.m_tileProcessor.Union(a, b);
        }

        internal TileProcessor<T_Tile> TileProcessor =>
            this.m_tileProcessor;

        [CompilerGenerated]
        private sealed class <AllButOne>d__0<T> : IEnumerable<T>, IEnumerable, IEnumerator<T>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private T <>2__current;
            public IEnumerable<T> <>3__list;
            public int <>3__toSkipPosition;
            public IEnumerator<T> <>7__wrap3;
            private int <>l__initialThreadId;
            public T <value>5__2;
            public int <valuePosition>5__1;
            public IEnumerable<T> list;
            public int toSkipPosition;

            [DebuggerHidden]
            public <AllButOne>d__0(int <>1__state)
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

            private bool MoveNext()
            {
                bool flag;
                try
                {
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            this.<valuePosition>5__1 = 0;
                            this.<>7__wrap3 = this.list.GetEnumerator();
                            this.<>1__state = 1;
                            goto Label_008E;

                        case 2:
                            this.<>1__state = 1;
                            goto Label_008E;

                        default:
                            goto Label_00A1;
                    }
                Label_0046:
                    this.<value>5__2 = this.<>7__wrap3.Current;
                    if (this.<valuePosition>5__1++ != this.toSkipPosition)
                    {
                        this.<>2__current = this.<value>5__2;
                        this.<>1__state = 2;
                        return true;
                    }
                Label_008E:
                    if (this.<>7__wrap3.MoveNext())
                    {
                        goto Label_0046;
                    }
                    this.<>m__Finally4();
                Label_00A1:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                RewritingProcessor<T_Tile>.<AllButOne>d__0<T> d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (RewritingProcessor<T_Tile>.<AllButOne>d__0<T>) this;
                }
                else
                {
                    d__ = new RewritingProcessor<T_Tile>.<AllButOne>d__0<T>(0);
                }
                d__.list = this.<>3__list;
                d__.toSkipPosition = this.<>3__toSkipPosition;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();

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
                            this.<>m__Finally4();
                        }
                        return;
                }
            }

            T IEnumerator<T>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }

        [CompilerGenerated]
        private sealed class <Concat>d__7<T> : IEnumerable<T>, IEnumerable, IEnumerator<T>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private T <>2__current;
            public IEnumerable<T> <>3__rest;
            public T <>3__value;
            public IEnumerator<T> <>7__wrap9;
            private int <>l__initialThreadId;
            public T <restValue>5__8;
            public IEnumerable<T> rest;
            public T value;

            [DebuggerHidden]
            public <Concat>d__7(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finallya()
            {
                this.<>1__state = -1;
                if (this.<>7__wrap9 != null)
                {
                    this.<>7__wrap9.Dispose();
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
                            this.<>2__current = this.value;
                            this.<>1__state = 1;
                            return true;

                        case 1:
                            this.<>1__state = -1;
                            this.<>7__wrap9 = this.rest.GetEnumerator();
                            this.<>1__state = 2;
                            goto Label_0090;

                        case 3:
                            this.<>1__state = 2;
                            goto Label_0090;

                        default:
                            goto Label_00A3;
                    }
                Label_0061:
                    this.<restValue>5__8 = this.<>7__wrap9.Current;
                    this.<>2__current = this.<restValue>5__8;
                    this.<>1__state = 3;
                    return true;
                Label_0090:
                    if (this.<>7__wrap9.MoveNext())
                    {
                        goto Label_0061;
                    }
                    this.<>m__Finallya();
                Label_00A3:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                RewritingProcessor<T_Tile>.<Concat>d__7<T> d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (RewritingProcessor<T_Tile>.<Concat>d__7<T>) this;
                }
                else
                {
                    d__ = new RewritingProcessor<T_Tile>.<Concat>d__7<T>(0);
                }
                d__.value = this.<>3__value;
                d__.rest = this.<>3__rest;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();

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
                            this.<>m__Finallya();
                        }
                        return;
                }
            }

            T IEnumerator<T>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }

        [CompilerGenerated]
        private sealed class <Permute>d__d<T> : IEnumerable<IEnumerable<T>>, IEnumerable, IEnumerator<IEnumerable<T>>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private IEnumerable<T> <>2__current;
            public IEnumerable<T> <>3__list;
            public IEnumerator<T> <>7__wrap12;
            public IEnumerator<IEnumerable<T>> <>7__wrap14;
            private int <>l__initialThreadId;
            public IEnumerable<T> <rest>5__e;
            public IEnumerable<T> <restPermutation>5__11;
            public T <value>5__10;
            public int <valuePosition>5__f;
            public IEnumerable<T> list;

            [DebuggerHidden]
            public <Permute>d__d(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finally13()
            {
                this.<>1__state = -1;
                if (this.<>7__wrap12 != null)
                {
                    this.<>7__wrap12.Dispose();
                }
            }

            private void <>m__Finally15()
            {
                this.<>1__state = 1;
                if (this.<>7__wrap14 != null)
                {
                    this.<>7__wrap14.Dispose();
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
                            this.<rest>5__e = null;
                            this.<valuePosition>5__f = 0;
                            this.<>7__wrap12 = this.list.GetEnumerator();
                            this.<>1__state = 1;
                            goto Label_00F7;

                        case 3:
                            goto Label_00DD;

                        case 4:
                            this.<>1__state = -1;
                            goto Label_0133;

                        default:
                            goto Label_0133;
                    }
                Label_0058:
                    this.<value>5__10 = this.<>7__wrap12.Current;
                    this.<rest>5__e = RewritingProcessor<T_Tile>.AllButOne<T>(this.list, this.<valuePosition>5__f++);
                    this.<>7__wrap14 = RewritingProcessor<T_Tile>.Permute<T>(this.<rest>5__e).GetEnumerator();
                    this.<>1__state = 2;
                    while (this.<>7__wrap14.MoveNext())
                    {
                        this.<restPermutation>5__11 = this.<>7__wrap14.Current;
                        this.<>2__current = RewritingProcessor<T_Tile>.Concat<T>(this.<value>5__10, this.<restPermutation>5__11);
                        this.<>1__state = 3;
                        return true;
                    Label_00DD:
                        this.<>1__state = 2;
                    }
                    this.<>m__Finally15();
                Label_00F7:
                    if (this.<>7__wrap12.MoveNext())
                    {
                        goto Label_0058;
                    }
                    this.<>m__Finally13();
                    if (this.<rest>5__e == null)
                    {
                        this.<>2__current = this.list;
                        this.<>1__state = 4;
                        return true;
                    }
                Label_0133:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            IEnumerator<IEnumerable<T>> IEnumerable<IEnumerable<T>>.GetEnumerator()
            {
                RewritingProcessor<T_Tile>.<Permute>d__d<T> _d;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    _d = (RewritingProcessor<T_Tile>.<Permute>d__d<T>) this;
                }
                else
                {
                    _d = new RewritingProcessor<T_Tile>.<Permute>d__d<T>(0);
                }
                _d.list = this.<>3__list;
                return _d;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<T>>.GetEnumerator();

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
                    case 3:
                        try
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
                                        this.<>m__Finally15();
                                    }
                                    return;
                            }
                        }
                        finally
                        {
                            this.<>m__Finally13();
                        }
                        break;

                    default:
                        return;
                }
            }

            IEnumerable<T> IEnumerator<IEnumerable<T>>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }

        [CompilerGenerated]
        private sealed class <Reverse>d__18<T> : IEnumerable<T>, IEnumerable, IEnumerator<T>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private T <>2__current;
            public HashSet<T> <>3__filter;
            public IEnumerable<T> <>3__input;
            public List<T>.Enumerator <>7__wrap1b;
            private int <>l__initialThreadId;
            public List<T> <output>5__19;
            public T <t>5__1a;
            public HashSet<T> filter;
            public IEnumerable<T> input;

            [DebuggerHidden]
            public <Reverse>d__18(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finally1c()
            {
                this.<>1__state = -1;
                this.<>7__wrap1b.Dispose();
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
                            this.<output>5__19 = new List<T>(this.input);
                            this.<output>5__19.Reverse();
                            this.<>7__wrap1b = this.<output>5__19.GetEnumerator();
                            this.<>1__state = 1;
                            goto Label_009D;

                        case 2:
                            this.<>1__state = 1;
                            goto Label_009D;

                        default:
                            goto Label_00B0;
                    }
                Label_005B:
                    this.<t>5__1a = this.<>7__wrap1b.Current;
                    if (this.filter.Contains(this.<t>5__1a))
                    {
                        this.<>2__current = this.<t>5__1a;
                        this.<>1__state = 2;
                        return true;
                    }
                Label_009D:
                    if (this.<>7__wrap1b.MoveNext())
                    {
                        goto Label_005B;
                    }
                    this.<>m__Finally1c();
                Label_00B0:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                RewritingProcessor<T_Tile>.<Reverse>d__18<T> d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (RewritingProcessor<T_Tile>.<Reverse>d__18<T>) this;
                }
                else
                {
                    d__ = new RewritingProcessor<T_Tile>.<Reverse>d__18<T>(0);
                }
                d__.input = this.<>3__input;
                d__.filter = this.<>3__filter;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();

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
                            this.<>m__Finally1c();
                        }
                        return;
                }
            }

            T IEnumerator<T>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

