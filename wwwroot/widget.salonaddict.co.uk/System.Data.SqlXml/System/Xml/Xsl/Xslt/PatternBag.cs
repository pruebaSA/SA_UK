namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Xsl.Qil;

    internal class PatternBag
    {
        public Dictionary<QilName, List<Pattern>> FixedNamePatterns = new Dictionary<QilName, List<Pattern>>();
        public List<QilName> FixedNamePatternsNames = new List<QilName>();
        public List<Pattern> NonFixedNamePatterns = new List<Pattern>();

        public void Add(Pattern pattern)
        {
            List<Pattern> nonFixedNamePatterns;
            QilName qName = pattern.Match.QName;
            if (qName == null)
            {
                nonFixedNamePatterns = this.NonFixedNamePatterns;
            }
            else if (!this.FixedNamePatterns.TryGetValue(qName, out nonFixedNamePatterns))
            {
                this.FixedNamePatternsNames.Add(qName);
                nonFixedNamePatterns = this.FixedNamePatterns[qName] = new List<Pattern>();
            }
            nonFixedNamePatterns.Add(pattern);
        }

        public void Clear()
        {
            this.FixedNamePatterns.Clear();
            this.FixedNamePatternsNames.Clear();
            this.NonFixedNamePatterns.Clear();
        }
    }
}

