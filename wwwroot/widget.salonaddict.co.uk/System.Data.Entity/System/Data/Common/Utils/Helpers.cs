namespace System.Data.Common.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal static class Helpers
    {
        internal static IEnumerable<SuperType> AsSuperTypeList<SubType, SuperType>(IEnumerable<SubType> values) where SubType: SuperType
        {
            foreach (SubType iteratorVariable0 in values)
            {
                yield return (SuperType) iteratorVariable0;
            }
        }

        internal static TNode BuildBalancedTreeInPlace<TNode>(IList<TNode> nodes, System.Func<TNode, TNode, TNode> combinator)
        {
            EntityUtil.CheckArgumentNull<IList<TNode>>(nodes, "nodes");
            EntityUtil.CheckArgumentNull<System.Func<TNode, TNode, TNode>>(combinator, "combinator");
            if (nodes.Count != 1)
            {
                if (nodes.Count == 2)
                {
                    return combinator(nodes[0], nodes[1]);
                }
                for (int i = nodes.Count; i != 1; i /= 2)
                {
                    bool flag = (i & 1) == 1;
                    if (flag)
                    {
                        i--;
                    }
                    int num2 = 0;
                    for (int j = 0; j < i; j += 2)
                    {
                        nodes[num2++] = combinator(nodes[j], nodes[j + 1]);
                    }
                    if (flag)
                    {
                        int num4 = num2 - 1;
                        nodes[num4] = combinator(nodes[num4], nodes[i]);
                    }
                }
            }
            return nodes[0];
        }

        internal static void FormatTraceLine(string format, params object[] args)
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        internal static bool IsSetEqual<Type>(IEnumerable<Type> list1, IEnumerable<Type> list2, IEqualityComparer<Type> comparer)
        {
            Set<Type> set = new Set<Type>(list1, comparer);
            Set<Type> other = new Set<Type>(list2, comparer);
            return set.SetEquals(other);
        }

        internal static void StringTrace(string arg)
        {
            Trace.Write(arg);
        }

        internal static void StringTraceLine(string arg)
        {
            Trace.WriteLine(arg);
        }

        [CompilerGenerated]
        private sealed class <AsSuperTypeList>d__0<SubType, SuperType> : IEnumerable<SuperType>, IEnumerable, IEnumerator<SuperType>, IEnumerator, IDisposable where SubType: SuperType
        {
            private int <>1__state;
            private SuperType <>2__current;
            public IEnumerable<SubType> <>3__values;
            public IEnumerator<SubType> <>7__wrap2;
            private int <>l__initialThreadId;
            public SubType <value>5__1;
            public IEnumerable<SubType> values;

            [DebuggerHidden]
            public <AsSuperTypeList>d__0(int <>1__state)
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
                            this.<>7__wrap2 = this.values.GetEnumerator();
                            this.<>1__state = 1;
                            goto Label_0075;

                        case 2:
                            this.<>1__state = 1;
                            goto Label_0075;

                        default:
                            goto Label_0088;
                    }
                Label_003C:
                    this.<value>5__1 = this.<>7__wrap2.Current;
                    this.<>2__current = (SuperType) this.<value>5__1;
                    this.<>1__state = 2;
                    return true;
                Label_0075:
                    if (this.<>7__wrap2.MoveNext())
                    {
                        goto Label_003C;
                    }
                    this.<>m__Finally3();
                Label_0088:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            IEnumerator<SuperType> IEnumerable<SuperType>.GetEnumerator()
            {
                Helpers.<AsSuperTypeList>d__0<SubType, SuperType> d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (Helpers.<AsSuperTypeList>d__0<SubType, SuperType>) this;
                }
                else
                {
                    d__ = new Helpers.<AsSuperTypeList>d__0<SubType, SuperType>(0);
                }
                d__.values = this.<>3__values;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<SuperType>.GetEnumerator();

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

            SuperType IEnumerator<SuperType>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

