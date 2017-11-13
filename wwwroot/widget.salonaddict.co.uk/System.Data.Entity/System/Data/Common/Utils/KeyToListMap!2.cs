namespace System.Data.Common.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    internal class KeyToListMap<TKey, TValue> : InternalBase
    {
        private Dictionary<TKey, List<TValue>> m_map;

        internal KeyToListMap(IEqualityComparer<TKey> comparer)
        {
            this.m_map = new Dictionary<TKey, List<TValue>>(comparer);
        }

        internal void Add(TKey key, TValue value)
        {
            List<TValue> list;
            if (!this.m_map.TryGetValue(key, out list))
            {
                list = new List<TValue>();
                this.m_map[key] = list;
            }
            list.Add(value);
        }

        internal void AddRange(TKey key, IEnumerable<TValue> values)
        {
            foreach (TValue local in values)
            {
                this.Add(key, local);
            }
        }

        internal bool ContainsKey(TKey key) => 
            this.m_map.ContainsKey(key);

        internal IEnumerable<TValue> EnumerateValues(TKey key)
        {
            List<TValue> iteratorVariable0;
            if (this.m_map.TryGetValue(key, out iteratorVariable0))
            {
                foreach (TValue iteratorVariable1 in iteratorVariable0)
                {
                    yield return iteratorVariable1;
                }
            }
        }

        internal ReadOnlyCollection<TValue> ListForKey(TKey key) => 
            new ReadOnlyCollection<TValue>(this.m_map[key]);

        internal bool RemoveKey(TKey key) => 
            this.m_map.Remove(key);

        internal override void ToCompactString(StringBuilder builder)
        {
            foreach (TKey local in this.Keys)
            {
                StringUtil.FormatStringBuilder(builder, "{0}", new object[] { local });
                builder.Append(": ");
                IEnumerable<TValue> list = this.ListForKey(local);
                StringUtil.ToSeparatedString(builder, list, ",", "null");
                builder.Append("; ");
            }
        }

        internal bool TryGetListForKey(TKey key, out ReadOnlyCollection<TValue> valueCollection)
        {
            List<TValue> list;
            valueCollection = null;
            if (this.m_map.TryGetValue(key, out list))
            {
                valueCollection = new ReadOnlyCollection<TValue>(list);
                return true;
            }
            return false;
        }

        internal IEnumerable<TValue> AllValues
        {
            get
            {
                foreach (TKey iteratorVariable0 in this.Keys)
                {
                    foreach (TValue iteratorVariable1 in this.ListForKey(iteratorVariable0))
                    {
                        yield return iteratorVariable1;
                    }
                }
            }
        }

        internal IEnumerable<TKey> Keys =>
            this.m_map.Keys;

        internal IEnumerable<KeyValuePair<TKey, List<TValue>>> KeyValuePairs =>
            this.m_map;

        [CompilerGenerated]
        private sealed class <EnumerateValues>d__9 : IEnumerable<TValue>, IEnumerable, IEnumerator<TValue>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private TValue <>2__current;
            public TKey <>3__key;
            public KeyToListMap<TKey, TValue> <>4__this;
            public List<TValue>.Enumerator <>7__wrapc;
            private int <>l__initialThreadId;
            public TValue <value>5__b;
            public List<TValue> <values>5__a;
            public TKey key;

            [DebuggerHidden]
            public <EnumerateValues>d__9(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finallyd()
            {
                this.<>1__state = -1;
                this.<>7__wrapc.Dispose();
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
                            if (!this.<>4__this.m_map.TryGetValue(this.key, out this.<values>5__a))
                            {
                                goto Label_009F;
                            }
                            this.<>7__wrapc = this.<values>5__a.GetEnumerator();
                            this.<>1__state = 1;
                            goto Label_008C;

                        case 2:
                            this.<>1__state = 1;
                            goto Label_008C;

                        default:
                            goto Label_009F;
                    }
                Label_005D:
                    this.<value>5__b = this.<>7__wrapc.Current;
                    this.<>2__current = this.<value>5__b;
                    this.<>1__state = 2;
                    return true;
                Label_008C:
                    if (this.<>7__wrapc.MoveNext())
                    {
                        goto Label_005D;
                    }
                    this.<>m__Finallyd();
                Label_009F:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                KeyToListMap<TKey, TValue>.<EnumerateValues>d__9 d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (KeyToListMap<TKey, TValue>.<EnumerateValues>d__9) this;
                }
                else
                {
                    d__ = new KeyToListMap<TKey, TValue>.<EnumerateValues>d__9(0) {
                        <>4__this = this.<>4__this
                    };
                }
                d__.key = this.<>3__key;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<TValue>.GetEnumerator();

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
                            this.<>m__Finallyd();
                        }
                        return;
                }
            }

            TValue IEnumerator<TValue>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }

        [CompilerGenerated]
        private sealed class <get_AllValues>d__0 : IEnumerable<TValue>, IEnumerable, IEnumerator<TValue>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private TValue <>2__current;
            public KeyToListMap<TKey, TValue> <>4__this;
            public IEnumerator<TKey> <>7__wrap3;
            public IEnumerator<TValue> <>7__wrap5;
            private int <>l__initialThreadId;
            public TKey <key>5__1;
            public TValue <value>5__2;

            [DebuggerHidden]
            public <get_AllValues>d__0(int <>1__state)
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
                this.<>1__state = 1;
                if (this.<>7__wrap5 != null)
                {
                    this.<>7__wrap5.Dispose();
                }
            }

            private bool MoveNext()
            {
                try
                {
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            this.<>7__wrap3 = this.<>4__this.Keys.GetEnumerator();
                            this.<>1__state = 1;
                            while (this.<>7__wrap3.MoveNext())
                            {
                                this.<key>5__1 = this.<>7__wrap3.Current;
                                this.<>7__wrap5 = this.<>4__this.ListForKey(this.<key>5__1).GetEnumerator();
                                this.<>1__state = 2;
                                while (this.<>7__wrap5.MoveNext())
                                {
                                    this.<value>5__2 = this.<>7__wrap5.Current;
                                    this.<>2__current = this.<value>5__2;
                                    this.<>1__state = 3;
                                    return true;
                                Label_009B:
                                    this.<>1__state = 2;
                                }
                                this.<>m__Finally6();
                            }
                            this.<>m__Finally4();
                            break;

                        case 3:
                            goto Label_009B;
                    }
                    return false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
            }

            [DebuggerHidden]
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    return (KeyToListMap<TKey, TValue>.<get_AllValues>d__0) this;
                }
                return new KeyToListMap<TKey, TValue>.<get_AllValues>d__0(0) { <>4__this = this.<>4__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<TValue>.GetEnumerator();

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

            TValue IEnumerator<TValue>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

