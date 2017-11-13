namespace System.Net
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class ListenerPrefixEnumerator : IEnumerator<string>, IDisposable, IEnumerator
    {
        private IEnumerator enumerator;

        internal ListenerPrefixEnumerator(IEnumerator enumerator)
        {
            this.enumerator = enumerator;
        }

        public void Dispose()
        {
        }

        public bool MoveNext() => 
            this.enumerator.MoveNext();

        void IEnumerator.Reset()
        {
            this.enumerator.Reset();
        }

        public string Current =>
            ((string) this.enumerator.Current);

        object IEnumerator.Current =>
            this.enumerator.Current;
    }
}

