namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public static class Sequence
    {
        public static T[] Collect<T>(params T[] arguments) => 
            arguments;

        public static IEnumerable<Pair<TFirstSequenceElement, TSecondSequenceElement>> Zip<TFirstSequenceElement, TSecondSequenceElement>(IEnumerable<TFirstSequenceElement> sequence1, IEnumerable<TSecondSequenceElement> sequence2)
        {
            IEnumerator<TFirstSequenceElement> enumerator = sequence1.GetEnumerator();
            IEnumerator<TSecondSequenceElement> iteratorVariable1 = sequence2.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!iteratorVariable1.MoveNext())
                {
                    break;
                }
                yield return new Pair<TFirstSequenceElement, TSecondSequenceElement>(enumerator.Current, iteratorVariable1.Current);
            }
        }

        [CompilerGenerated]
        private sealed class <Zip>d__0<TFirstSequenceElement, TSecondSequenceElement> : IEnumerable<Pair<TFirstSequenceElement, TSecondSequenceElement>>, IEnumerable, IEnumerator<Pair<TFirstSequenceElement, TSecondSequenceElement>>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private Pair<TFirstSequenceElement, TSecondSequenceElement> <>2__current;
            public IEnumerable<TFirstSequenceElement> <>3__sequence1;
            public IEnumerable<TSecondSequenceElement> <>3__sequence2;
            private int <>l__initialThreadId;
            public IEnumerator<TFirstSequenceElement> <enum1>5__1;
            public IEnumerator<TSecondSequenceElement> <enum2>5__2;
            public IEnumerable<TFirstSequenceElement> sequence1;
            public IEnumerable<TSecondSequenceElement> sequence2;

            [DebuggerHidden]
            public <Zip>d__0(int <>1__state)
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
                        this.<enum1>5__1 = this.sequence1.GetEnumerator();
                        this.<enum2>5__2 = this.sequence2.GetEnumerator();
                        while (this.<enum1>5__1.MoveNext())
                        {
                            if (!this.<enum2>5__2.MoveNext())
                            {
                                break;
                            }
                            this.<>2__current = new Pair<TFirstSequenceElement, TSecondSequenceElement>(this.<enum1>5__1.Current, this.<enum2>5__2.Current);
                            this.<>1__state = 1;
                            return true;
                        Label_008C:
                            this.<>1__state = -1;
                        }
                        break;

                    case 1:
                        goto Label_008C;
                }
                return false;
            }

            [DebuggerHidden]
            IEnumerator<Pair<TFirstSequenceElement, TSecondSequenceElement>> IEnumerable<Pair<TFirstSequenceElement, TSecondSequenceElement>>.GetEnumerator()
            {
                Sequence.<Zip>d__0<TFirstSequenceElement, TSecondSequenceElement> d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (Sequence.<Zip>d__0<TFirstSequenceElement, TSecondSequenceElement>) this;
                }
                else
                {
                    d__ = new Sequence.<Zip>d__0<TFirstSequenceElement, TSecondSequenceElement>(0);
                }
                d__.sequence1 = this.<>3__sequence1;
                d__.sequence2 = this.<>3__sequence2;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Microsoft.Practices.Unity.Utility.Pair<TFirstSequenceElement,TSecondSequenceElement>>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
            }

            Pair<TFirstSequenceElement, TSecondSequenceElement> IEnumerator<Pair<TFirstSequenceElement, TSecondSequenceElement>>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

