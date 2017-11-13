namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal sealed class DomainConstraintConversionContext<T_Variable, T_Element> : ConversionContext<DomainConstraint<T_Variable, T_Element>>
    {
        private readonly Dictionary<DomainVariable<T_Variable, T_Element>, int> _domainVariableToRobddVariableMap;
        private Dictionary<int, DomainVariable<T_Variable, T_Element>> _inverseMap;

        public DomainConstraintConversionContext()
        {
            this._domainVariableToRobddVariableMap = new Dictionary<DomainVariable<T_Variable, T_Element>, int>();
        }

        internal override IEnumerable<LiteralVertexPair<DomainConstraint<T_Variable, T_Element>>> GetSuccessors(Vertex vertex)
        {
            this.InitializeInverseMap();
            DomainVariable<T_Variable, T_Element> variable = this._inverseMap[vertex.Variable];
            T_Element[] iteratorVariable1 = variable.Domain.ToArray();
            Dictionary<Vertex, System.Data.Common.Utils.Set<T_Element>> iteratorVariable2 = new Dictionary<Vertex, System.Data.Common.Utils.Set<T_Element>>();
            for (int i = 0; i < vertex.Children.Length; i++)
            {
                System.Data.Common.Utils.Set<T_Element> set;
                Vertex key = vertex.Children[i];
                if (!iteratorVariable2.TryGetValue(key, out set))
                {
                    set = new System.Data.Common.Utils.Set<T_Element>(variable.Domain.Comparer);
                    iteratorVariable2.Add(key, set);
                }
                set.Add(iteratorVariable1[i]);
            }
            foreach (KeyValuePair<Vertex, System.Data.Common.Utils.Set<T_Element>> iteratorVariable3 in iteratorVariable2)
            {
                Vertex iteratorVariable4 = iteratorVariable3.Key;
                System.Data.Common.Utils.Set<T_Element> iteratorVariable5 = iteratorVariable3.Value;
                DomainConstraint<T_Variable, T_Element> identifier = new DomainConstraint<T_Variable, T_Element>(variable, iteratorVariable5.MakeReadOnly());
                Literal<DomainConstraint<T_Variable, T_Element>> literal = new Literal<DomainConstraint<T_Variable, T_Element>>(new TermExpr<DomainConstraint<T_Variable, T_Element>>(identifier), true);
                yield return new LiteralVertexPair<DomainConstraint<T_Variable, T_Element>>(iteratorVariable4, literal);
            }
        }

        private void InitializeInverseMap()
        {
            if (this._inverseMap == null)
            {
                this._inverseMap = this._domainVariableToRobddVariableMap.ToDictionary<KeyValuePair<DomainVariable<T_Variable, T_Element>, int>, int, DomainVariable<T_Variable, T_Element>>(kvp => kvp.Value, kvp => kvp.Key);
            }
        }

        internal override Vertex TranslateTermToVertex(TermExpr<DomainConstraint<T_Variable, T_Element>> term)
        {
            int num;
            System.Data.Common.Utils.Set<T_Element> range = term.Identifier.Range;
            DomainVariable<T_Variable, T_Element> key = term.Identifier.Variable;
            System.Data.Common.Utils.Set<T_Element> domain = key.Domain;
            if (range.All<T_Element>(element => !domain.Contains(element)))
            {
                return Vertex.Zero;
            }
            if (domain.All<T_Element>(element => range.Contains(element)))
            {
                return Vertex.One;
            }
            Vertex[] children = domain.Select<T_Element, Vertex>(delegate (T_Element element) {
                if (!range.Contains(element))
                {
                    return Vertex.Zero;
                }
                return Vertex.One;
            }).ToArray<Vertex>();
            if (!this._domainVariableToRobddVariableMap.TryGetValue(key, out num))
            {
                num = base.Solver.CreateVariable();
                this._domainVariableToRobddVariableMap[key] = num;
            }
            return base.Solver.CreateLeafVertex(num, children);
        }

        [CompilerGenerated]
        private sealed class <GetSuccessors>d__5 : IEnumerable<LiteralVertexPair<DomainConstraint<T_Variable, T_Element>>>, IEnumerable, IEnumerator<LiteralVertexPair<DomainConstraint<T_Variable, T_Element>>>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private LiteralVertexPair<DomainConstraint<T_Variable, T_Element>> <>2__current;
            public Vertex <>3__vertex;
            public DomainConstraintConversionContext<T_Variable, T_Element> <>4__this;
            public Dictionary<Vertex, System.Data.Common.Utils.Set<T_Element>>.Enumerator <>7__wrape;
            private int <>l__initialThreadId;
            public DomainConstraint<T_Variable, T_Element> <constraint>5__c;
            public T_Element[] <domain>5__7;
            public DomainVariable<T_Variable, T_Element> <domainVariable>5__6;
            public Literal<DomainConstraint<T_Variable, T_Element>> <literal>5__d;
            public System.Data.Common.Utils.Set<T_Element> <range>5__b;
            public Vertex <successorVertex>5__a;
            public KeyValuePair<Vertex, System.Data.Common.Utils.Set<T_Element>> <vertexRange>5__9;
            public Dictionary<Vertex, System.Data.Common.Utils.Set<T_Element>> <vertexToRange>5__8;
            public Vertex vertex;

            [DebuggerHidden]
            public <GetSuccessors>d__5(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finallyf()
            {
                this.<>1__state = -1;
                this.<>7__wrape.Dispose();
            }

            private bool MoveNext()
            {
                bool flag;
                try
                {
                    int num;
                    Vertex vertex;
                    System.Data.Common.Utils.Set<T_Element> set;
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            this.<>4__this.InitializeInverseMap();
                            this.<domainVariable>5__6 = this.<>4__this._inverseMap[this.vertex.Variable];
                            this.<domain>5__7 = this.<domainVariable>5__6.Domain.ToArray();
                            this.<vertexToRange>5__8 = new Dictionary<Vertex, System.Data.Common.Utils.Set<T_Element>>();
                            num = 0;
                            goto Label_00CF;

                        case 2:
                            goto Label_018C;

                        default:
                            goto Label_01A9;
                    }
                Label_0078:
                    vertex = this.vertex.Children[num];
                    if (!this.<vertexToRange>5__8.TryGetValue(vertex, out set))
                    {
                        set = new System.Data.Common.Utils.Set<T_Element>(this.<domainVariable>5__6.Domain.Comparer);
                        this.<vertexToRange>5__8.Add(vertex, set);
                    }
                    set.Add(this.<domain>5__7[num]);
                    num++;
                Label_00CF:
                    if (num < this.vertex.Children.Length)
                    {
                        goto Label_0078;
                    }
                    this.<>7__wrape = this.<vertexToRange>5__8.GetEnumerator();
                    this.<>1__state = 1;
                    while (this.<>7__wrape.MoveNext())
                    {
                        this.<vertexRange>5__9 = this.<>7__wrape.Current;
                        this.<successorVertex>5__a = this.<vertexRange>5__9.Key;
                        this.<range>5__b = this.<vertexRange>5__9.Value;
                        this.<constraint>5__c = new DomainConstraint<T_Variable, T_Element>(this.<domainVariable>5__6, this.<range>5__b.MakeReadOnly());
                        this.<literal>5__d = new Literal<DomainConstraint<T_Variable, T_Element>>(new TermExpr<DomainConstraint<T_Variable, T_Element>>(this.<constraint>5__c), true);
                        this.<>2__current = new LiteralVertexPair<DomainConstraint<T_Variable, T_Element>>(this.<successorVertex>5__a, this.<literal>5__d);
                        this.<>1__state = 2;
                        return true;
                    Label_018C:
                        this.<>1__state = 1;
                    }
                    this.<>m__Finallyf();
                Label_01A9:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            IEnumerator<LiteralVertexPair<DomainConstraint<T_Variable, T_Element>>> IEnumerable<LiteralVertexPair<DomainConstraint<T_Variable, T_Element>>>.GetEnumerator()
            {
                DomainConstraintConversionContext<T_Variable, T_Element>.<GetSuccessors>d__5 d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (DomainConstraintConversionContext<T_Variable, T_Element>.<GetSuccessors>d__5) this;
                }
                else
                {
                    d__ = new DomainConstraintConversionContext<T_Variable, T_Element>.<GetSuccessors>d__5(0) {
                        <>4__this = this.<>4__this
                    };
                }
                d__.vertex = this.<>3__vertex;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Data.Common.Utils.Boolean.LiteralVertexPair<System.Data.Common.Utils.Boolean.DomainConstraint<T_Variable,T_Element>>>.GetEnumerator();

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
                            this.<>m__Finallyf();
                        }
                        return;
                }
            }

            LiteralVertexPair<DomainConstraint<T_Variable, T_Element>> IEnumerator<LiteralVertexPair<DomainConstraint<T_Variable, T_Element>>>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

