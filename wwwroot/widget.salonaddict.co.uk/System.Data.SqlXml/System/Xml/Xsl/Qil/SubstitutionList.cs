namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Collections;

    internal sealed class SubstitutionList
    {
        private ArrayList s = new ArrayList(4);

        public void AddSubstitutionPair(QilNode find, QilNode replace)
        {
            this.s.Add(find);
            this.s.Add(replace);
        }

        public QilNode FindReplacement(QilNode n)
        {
            for (int i = this.s.Count - 2; i >= 0; i -= 2)
            {
                if (this.s[i] == n)
                {
                    return (QilNode) this.s[i + 1];
                }
            }
            return null;
        }

        public void RemoveLastNSubstitutionPairs(int n)
        {
            if (n > 0)
            {
                n *= 2;
                this.s.RemoveRange(this.s.Count - n, n);
            }
        }

        public void RemoveLastSubstitutionPair()
        {
            this.s.RemoveRange(this.s.Count - 2, 2);
        }
    }
}

