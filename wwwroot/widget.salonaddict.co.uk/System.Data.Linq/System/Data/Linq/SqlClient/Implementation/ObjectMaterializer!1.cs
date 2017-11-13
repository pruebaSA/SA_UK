namespace System.Data.Linq.SqlClient.Implementation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Linq;
    using System.Data.Linq.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public abstract class ObjectMaterializer<TDataReader> where TDataReader: DbDataReader
    {
        public object[] Arguments;
        public DbDataReader BufferReader;
        public TDataReader DataReader;
        public object[] Globals;
        public object[] Locals;
        public int[] Ordinals;

        public ObjectMaterializer()
        {
            this.DataReader = default(TDataReader);
        }

        public static IEnumerable<TOutput> Convert<TOutput>(IEnumerable source)
        {
            IEnumerator enumerator = this.source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                yield return DBConvert.ChangeType<TOutput>(current);
            }
            this.<>m__Finally4();
        }

        public static IGrouping<TKey, TElement> CreateGroup<TKey, TElement>(TKey key, IEnumerable<TElement> items) => 
            new ObjectReaderCompiler.Group<TKey, TElement>(key, items);

        public static IOrderedEnumerable<TElement> CreateOrderedEnumerable<TElement>(IEnumerable<TElement> items) => 
            new ObjectReaderCompiler.OrderedResults<TElement>(items);

        public static Exception ErrorAssignmentToNull(Type type) => 
            System.Data.Linq.SqlClient.Error.CannotAssignNull(type);

        public abstract IEnumerable ExecuteSubQuery(int iSubQuery, object[] args);
        public abstract IEnumerable<T> GetLinkSource<T>(int globalLink, int localFactory, object[] keyValues);
        public abstract IEnumerable<T> GetNestedLinkSource<T>(int globalLink, int localFactory, object instance);
        public abstract object InsertLookup(int globalMetaType, object instance);
        public abstract bool Read();
        public abstract void SendEntityMaterialized(int globalMetaType, object instance);

        public abstract bool CanDeferLoad { get; }

        [CompilerGenerated]
        private sealed class <Convert>d__0<TOutput> : IEnumerable<TOutput>, IEnumerable, IEnumerator<TOutput>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private TOutput <>2__current;
            public IEnumerable <>3__source;
            public IEnumerator <>7__wrap2;
            public IDisposable <>7__wrap3;
            private int <>l__initialThreadId;
            public object <value>5__1;
            public IEnumerable source;

            [DebuggerHidden]
            public <Convert>d__0(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finally4()
            {
                this.<>1__state = -1;
                this.<>7__wrap3 = this.<>7__wrap2 as IDisposable;
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
                            this.<>7__wrap2 = this.source.GetEnumerator();
                            this.<>1__state = 1;
                            goto Label_0070;

                        case 2:
                            this.<>1__state = 1;
                            goto Label_0070;

                        default:
                            goto Label_0083;
                    }
                Label_003C:
                    this.<value>5__1 = this.<>7__wrap2.Current;
                    this.<>2__current = DBConvert.ChangeType<TOutput>(this.<value>5__1);
                    this.<>1__state = 2;
                    return true;
                Label_0070:
                    if (this.<>7__wrap2.MoveNext())
                    {
                        goto Label_003C;
                    }
                    this.<>m__Finally4();
                Label_0083:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            IEnumerator<TOutput> IEnumerable<TOutput>.GetEnumerator()
            {
                ObjectMaterializer<TDataReader>.<Convert>d__0<TOutput> d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (ObjectMaterializer<TDataReader>.<Convert>d__0<TOutput>) this;
                }
                else
                {
                    d__ = new ObjectMaterializer<TDataReader>.<Convert>d__0<TOutput>(0);
                }
                d__.source = this.<>3__source;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<TOutput>.GetEnumerator();

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

            TOutput IEnumerator<TOutput>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

