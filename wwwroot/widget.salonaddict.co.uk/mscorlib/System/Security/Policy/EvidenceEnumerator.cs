namespace System.Security.Policy
{
    using System;
    using System.Collections;

    internal sealed class EvidenceEnumerator : IEnumerator
    {
        private IEnumerator m_enumerator;
        private Evidence m_evidence;
        private bool m_first;

        public EvidenceEnumerator(Evidence evidence)
        {
            this.m_evidence = evidence;
            this.Reset();
        }

        public bool MoveNext()
        {
            if (this.m_enumerator == null)
            {
                return false;
            }
            if (this.m_enumerator.MoveNext())
            {
                return true;
            }
            if (!this.m_first)
            {
                return false;
            }
            this.m_enumerator = this.m_evidence.GetAssemblyEnumerator();
            this.m_first = false;
            return ((this.m_enumerator != null) && this.m_enumerator.MoveNext());
        }

        public void Reset()
        {
            this.m_first = true;
            if (this.m_evidence != null)
            {
                this.m_enumerator = this.m_evidence.GetHostEnumerator();
            }
            else
            {
                this.m_enumerator = null;
            }
        }

        public object Current =>
            this.m_enumerator?.Current;
    }
}

