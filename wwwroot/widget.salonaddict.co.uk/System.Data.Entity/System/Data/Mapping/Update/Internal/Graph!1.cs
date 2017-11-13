namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal class Graph<TVertex>
    {
        private IEqualityComparer<TVertex> m_comparer;
        private EdgeMap<TVertex> m_incomingEdgeMap;
        private EdgeMap<TVertex> m_outgoingEdgeMap;
        private HashSet<TVertex> m_remainder;
        private HashSet<TVertex> m_vertices;

        internal Graph(IEqualityComparer<TVertex> comparer)
        {
            EntityUtil.CheckArgumentNull<IEqualityComparer<TVertex>>(comparer, "comparer");
            this.m_comparer = comparer;
            this.m_outgoingEdgeMap = new EdgeMap<TVertex>(comparer);
            this.m_incomingEdgeMap = new EdgeMap<TVertex>(comparer);
            this.m_vertices = new HashSet<TVertex>(comparer);
        }

        internal void AddEdge(TVertex from, TVertex to)
        {
            if (this.m_vertices.Contains(from) && this.m_vertices.Contains(to))
            {
                this.m_outgoingEdgeMap.Add(from, to);
                this.m_incomingEdgeMap.Add(to, from);
            }
        }

        internal void AddVertex(TVertex vertex)
        {
            this.m_vertices.Add(vertex);
        }

        private void RemoveEdge(TVertex from, TVertex to)
        {
            this.m_outgoingEdgeMap.Remove(from, to);
            this.m_incomingEdgeMap.Remove(to, from);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<TVertex, Set<TVertex>> pair in this.m_outgoingEdgeMap)
            {
                bool flag = true;
                builder.AppendFormat(CultureInfo.InvariantCulture, "[{0}] --> ", new object[] { pair.Key });
                foreach (TVertex local in pair.Value)
                {
                    if (flag)
                    {
                        flag = false;
                    }
                    else
                    {
                        builder.Append(", ");
                    }
                    builder.AppendFormat(CultureInfo.InvariantCulture, "[{0}]", new object[] { local });
                }
                builder.Append("; ");
            }
            return builder.ToString();
        }

        internal IEnumerable<TVertex[]> TryStagedTopologicalSort()
        {
            Stack<TVertex> iteratorVariable0 = new Stack<TVertex>();
            foreach (TVertex local in this.m_vertices)
            {
                if (!this.m_incomingEdgeMap.ContainsKey(local))
                {
                    iteratorVariable0.Push(local);
                }
            }
        Label_PostSwitchInIterator:;
            if (0 < iteratorVariable0.Count)
            {
                TVertex[] iteratorVariable1 = new TVertex[iteratorVariable0.Count];
                for (int i = 0; 0 < iteratorVariable0.Count; i++)
                {
                    iteratorVariable1[i] = iteratorVariable0.Pop();
                }
                yield return iteratorVariable1;
                foreach (TVertex local2 in iteratorVariable1)
                {
                    Set<TVertex> set;
                    this.m_vertices.Remove(local2);
                    if (this.m_outgoingEdgeMap.TryGetValue(local2, out set))
                    {
                        List<TVertex> list = new List<TVertex>(set);
                        foreach (TVertex local3 in list)
                        {
                            this.RemoveEdge(local2, local3);
                            if (!this.m_incomingEdgeMap.ContainsKey(local3))
                            {
                                iteratorVariable0.Push(local3);
                            }
                        }
                    }
                }
                goto Label_PostSwitchInIterator;
            }
            this.m_remainder = this.m_vertices;
        }

        internal IEnumerable<KeyValuePair<TVertex, TVertex>> Edges
        {
            get
            {
                foreach (KeyValuePair<TVertex, Set<TVertex>> iteratorVariable0 in this.m_outgoingEdgeMap)
                {
                    foreach (TVertex iteratorVariable1 in iteratorVariable0.Value)
                    {
                        yield return new KeyValuePair<TVertex, TVertex>(iteratorVariable0.Key, iteratorVariable1);
                    }
                }
            }
        }

        internal HashSet<TVertex> Remainder =>
            this.m_remainder;

        internal HashSet<TVertex> Vertices =>
            this.m_vertices;

        [CompilerGenerated]
        private sealed class <get_Edges>d__0 : IEnumerable<KeyValuePair<TVertex, TVertex>>, IEnumerable, IEnumerator<KeyValuePair<TVertex, TVertex>>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private KeyValuePair<TVertex, TVertex> <>2__current;
            public Graph<TVertex> <>4__this;
            public Dictionary<TVertex, Set<TVertex>>.Enumerator <>7__wrap3;
            public HashSet<TVertex>.Enumerator <>7__wrap5;
            private int <>l__initialThreadId;
            public KeyValuePair<TVertex, Set<TVertex>> <outgoingEdge>5__1;
            public TVertex <vertex>5__2;

            [DebuggerHidden]
            public <get_Edges>d__0(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finally4()
            {
                this.<>1__state = -1;
                this.<>7__wrap3.Dispose();
            }

            private void <>m__Finally6()
            {
                this.<>1__state = 1;
                this.<>7__wrap5.Dispose();
            }

            private bool MoveNext()
            {
                try
                {
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            this.<>7__wrap3 = this.<>4__this.m_outgoingEdgeMap.GetEnumerator();
                            this.<>1__state = 1;
                            while (this.<>7__wrap3.MoveNext())
                            {
                                this.<outgoingEdge>5__1 = this.<>7__wrap3.Current;
                                this.<>7__wrap5 = this.<outgoingEdge>5__1.Value.GetEnumerator();
                                this.<>1__state = 2;
                                while (this.<>7__wrap5.MoveNext())
                                {
                                    this.<vertex>5__2 = this.<>7__wrap5.Current;
                                    this.<>2__current = new KeyValuePair<TVertex, TVertex>(this.<outgoingEdge>5__1.Key, this.<vertex>5__2);
                                    this.<>1__state = 3;
                                    return true;
                                Label_00AE:
                                    this.<>1__state = 2;
                                }
                                this.<>m__Finally6();
                            }
                            this.<>m__Finally4();
                            break;

                        case 3:
                            goto Label_00AE;
                    }
                    return false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
            }

            [DebuggerHidden]
            IEnumerator<KeyValuePair<TVertex, TVertex>> IEnumerable<KeyValuePair<TVertex, TVertex>>.GetEnumerator()
            {
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    return (Graph<TVertex>.<get_Edges>d__0) this;
                }
                return new Graph<TVertex>.<get_Edges>d__0(0) { <>4__this = this.<>4__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TVertex,TVertex>>.GetEnumerator();

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
                                        this.<>m__Finally6();
                                    }
                                    return;
                            }
                        }
                        finally
                        {
                            this.<>m__Finally4();
                        }
                        break;

                    default:
                        return;
                }
            }

            KeyValuePair<TVertex, TVertex> IEnumerator<KeyValuePair<TVertex, TVertex>>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }

        [CompilerGenerated]
        private sealed class <TryStagedTopologicalSort>d__9 : IEnumerable<TVertex[]>, IEnumerable, IEnumerator<TVertex[]>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private TVertex[] <>2__current;
            public Graph<TVertex> <>4__this;
            private int <>l__initialThreadId;
            public TVertex[] <froms>5__b;
            public int <index>5__c;
            public Stack<TVertex> <roots>5__a;

            [DebuggerHidden]
            public <TryStagedTopologicalSort>d__9(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        this.<roots>5__a = new Stack<TVertex>();
                        foreach (TVertex local in this.<>4__this.m_vertices)
                        {
                            if (!this.<>4__this.m_incomingEdgeMap.ContainsKey(local))
                            {
                                this.<roots>5__a.Push(local);
                            }
                        }
                        break;

                    case 2:
                        this.<>1__state = -1;
                        foreach (TVertex local2 in this.<froms>5__b)
                        {
                            Set<TVertex> set;
                            this.<>4__this.m_vertices.Remove(local2);
                            if (this.<>4__this.m_outgoingEdgeMap.TryGetValue(local2, out set))
                            {
                                List<TVertex> list = new List<TVertex>(set);
                                foreach (TVertex local3 in list)
                                {
                                    this.<>4__this.RemoveEdge(local2, local3);
                                    if (!this.<>4__this.m_incomingEdgeMap.ContainsKey(local3))
                                    {
                                        this.<roots>5__a.Push(local3);
                                    }
                                }
                            }
                        }
                        break;

                    default:
                        goto Label_01D7;
                }
                if (0 < this.<roots>5__a.Count)
                {
                    this.<froms>5__b = new TVertex[this.<roots>5__a.Count];
                    this.<index>5__c = 0;
                    while (0 < this.<roots>5__a.Count)
                    {
                        this.<froms>5__b[this.<index>5__c] = this.<roots>5__a.Pop();
                        this.<index>5__c++;
                    }
                    this.<>2__current = this.<froms>5__b;
                    this.<>1__state = 2;
                    return true;
                }
                this.<>4__this.m_remainder = this.<>4__this.m_vertices;
            Label_01D7:
                return false;
            }

            [DebuggerHidden]
            IEnumerator<TVertex[]> IEnumerable<TVertex[]>.GetEnumerator()
            {
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    return (Graph<TVertex>.<TryStagedTopologicalSort>d__9) this;
                }
                return new Graph<TVertex>.<TryStagedTopologicalSort>d__9(0) { <>4__this = this.<>4__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<TVertex[]>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
            }

            TVertex[] IEnumerator<TVertex[]>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }

        private class EdgeMap : Dictionary<TVertex, Set<TVertex>>
        {
            private IEqualityComparer<TVertex> m_comparer;

            internal EdgeMap(IEqualityComparer<TVertex> comparer) : base(comparer)
            {
                this.m_comparer = comparer;
            }

            internal void Add(TVertex key, TVertex valueElement)
            {
                Set<TVertex> set;
                if (!base.TryGetValue(key, out set))
                {
                    set = new Set<TVertex>(this.m_comparer);
                    base.Add(key, set);
                }
                if (!set.Contains(valueElement))
                {
                    set.Add(valueElement);
                }
            }

            internal void Remove(TVertex key, TVertex valueElement)
            {
                Set<TVertex> set = base[key];
                set.Remove(valueElement);
                if (set.Count == 0)
                {
                    base.Remove(key);
                }
            }
        }
    }
}

