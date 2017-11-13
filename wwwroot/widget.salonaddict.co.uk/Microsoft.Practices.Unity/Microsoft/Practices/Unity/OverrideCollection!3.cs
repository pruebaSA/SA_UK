namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public abstract class OverrideCollection<TOverride, TKey, TValue> : ResolverOverride, IEnumerable<TOverride>, IEnumerable where TOverride: ResolverOverride
    {
        private readonly CompositeResolverOverride overrides;

        protected OverrideCollection()
        {
            this.overrides = new CompositeResolverOverride();
        }

        public void Add(TKey key, TValue value)
        {
            this.overrides.Add(this.MakeOverride(key, value));
        }

        public IEnumerator<TOverride> GetEnumerator()
        {
            foreach (ResolverOverride iteratorVariable0 in this.overrides)
            {
                yield return (TOverride) iteratorVariable0;
            }
        }

        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType) => 
            this.overrides.GetResolver(context, dependencyType);

        protected abstract TOverride MakeOverride(TKey key, TValue value);
        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        [CompilerGenerated]
        private sealed class <GetEnumerator>d__0 : IEnumerator<TOverride>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private TOverride <>2__current;
            public OverrideCollection<TOverride, TKey, TValue> <>4__this;
            public IEnumerator<ResolverOverride> <>7__wrap2;
            public ResolverOverride <o>5__1;

            [DebuggerHidden]
            public <GetEnumerator>d__0(int <>1__state)
            {
                this.<>1__state = <>1__state;
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
                try
                {
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            this.<>7__wrap2 = this.<>4__this.overrides.GetEnumerator();
                            this.<>1__state = 1;
                            while (this.<>7__wrap2.MoveNext())
                            {
                                this.<o>5__1 = this.<>7__wrap2.Current;
                                this.<>2__current = (TOverride) this.<o>5__1;
                                this.<>1__state = 2;
                                return true;
                            Label_0077:
                                this.<>1__state = 1;
                            }
                            this.<>m__Finally3();
                            break;

                        case 2:
                            goto Label_0077;
                    }
                    return false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
            }

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
                        break;
                }
            }

            TOverride IEnumerator<TOverride>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

