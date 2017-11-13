namespace System.Data.Common.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal class ModifiableIteratorCollection<TElement> : InternalBase
    {
        private int m_currentIteratorIndex;
        private List<TElement> m_elements;

        internal ModifiableIteratorCollection(IEnumerable<TElement> elements)
        {
            this.m_elements = new List<TElement>(elements);
            this.m_currentIteratorIndex = -1;
        }

        internal IEnumerable<TElement> Elements()
        {
            this.m_currentIteratorIndex = 0;
            while (true)
            {
                if (this.m_currentIteratorIndex >= this.m_elements.Count)
                {
                    yield break;
                }
                yield return this.m_elements[this.m_currentIteratorIndex];
                this.m_currentIteratorIndex++;
            }
        }

        private TElement Remove(int index)
        {
            TElement local = this.m_elements[index];
            int num = this.m_elements.Count - 1;
            this.m_elements[index] = this.m_elements[num];
            this.m_elements.RemoveAt(num);
            return local;
        }

        internal void RemoveCurrentOfIterator()
        {
            this.Remove(this.m_currentIteratorIndex);
            this.m_currentIteratorIndex--;
        }

        internal TElement RemoveOneElement() => 
            this.Remove(this.m_elements.Count - 1);

        internal void ResetIterator()
        {
            this.m_currentIteratorIndex = -1;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            StringUtil.ToCommaSeparatedString(builder, this.m_elements);
        }

        internal bool IsEmpty =>
            (this.m_elements.Count == 0);

        [CompilerGenerated]
        private sealed class <Elements>d__0 : IEnumerable<TElement>, IEnumerable, IEnumerator<TElement>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private TElement <>2__current;
            public ModifiableIteratorCollection<TElement> <>4__this;
            private int <>l__initialThreadId;

            [DebuggerHidden]
            public <Elements>d__0(int <>1__state)
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
                        this.<>4__this.m_currentIteratorIndex = 0;
                        break;

                    case 1:
                        this.<>1__state = -1;
                        this.<>4__this.m_currentIteratorIndex++;
                        break;

                    default:
                        goto Label_008D;
                }
                if (this.<>4__this.m_currentIteratorIndex < this.<>4__this.m_elements.Count)
                {
                    this.<>2__current = this.<>4__this.m_elements[this.<>4__this.m_currentIteratorIndex];
                    this.<>1__state = 1;
                    return true;
                }
            Label_008D:
                return false;
            }

            [DebuggerHidden]
            IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
            {
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    return (ModifiableIteratorCollection<TElement>.<Elements>d__0) this;
                }
                return new ModifiableIteratorCollection<TElement>.<Elements>d__0(0) { <>4__this = this.<>4__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<TElement>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
            }

            TElement IEnumerator<TElement>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

