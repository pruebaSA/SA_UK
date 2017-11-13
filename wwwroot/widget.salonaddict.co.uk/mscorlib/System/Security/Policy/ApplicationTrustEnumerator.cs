namespace System.Security.Policy
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public sealed class ApplicationTrustEnumerator : IEnumerator
    {
        private int m_current;
        private ApplicationTrustCollection m_trusts;

        private ApplicationTrustEnumerator()
        {
        }

        internal ApplicationTrustEnumerator(ApplicationTrustCollection trusts)
        {
            this.m_trusts = trusts;
            this.m_current = -1;
        }

        public bool MoveNext()
        {
            if (this.m_current == (this.m_trusts.Count - 1))
            {
                return false;
            }
            this.m_current++;
            return true;
        }

        public void Reset()
        {
            this.m_current = -1;
        }

        public ApplicationTrust Current =>
            this.m_trusts[this.m_current];

        object IEnumerator.Current =>
            this.m_trusts[this.m_current];
    }
}

