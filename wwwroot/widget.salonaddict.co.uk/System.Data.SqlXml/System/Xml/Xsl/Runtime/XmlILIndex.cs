namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.XPath;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class XmlILIndex
    {
        private Dictionary<string, XmlQueryNodeSequence> table = new Dictionary<string, XmlQueryNodeSequence>();

        internal XmlILIndex()
        {
        }

        public void Add(string key, XPathNavigator navigator)
        {
            XmlQueryNodeSequence sequence;
            if (!this.table.TryGetValue(key, out sequence))
            {
                sequence = new XmlQueryNodeSequence();
                sequence.AddClone(navigator);
                this.table.Add(key, sequence);
            }
            else if (!navigator.IsSamePosition(sequence[sequence.Count - 1]))
            {
                sequence.AddClone(navigator);
            }
        }

        public XmlQueryNodeSequence Lookup(string key)
        {
            XmlQueryNodeSequence sequence;
            if (!this.table.TryGetValue(key, out sequence))
            {
                sequence = new XmlQueryNodeSequence();
            }
            return sequence;
        }
    }
}

