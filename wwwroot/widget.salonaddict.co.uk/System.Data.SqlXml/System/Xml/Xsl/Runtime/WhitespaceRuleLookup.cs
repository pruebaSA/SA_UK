namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Xsl.Qil;

    internal class WhitespaceRuleLookup
    {
        private XmlNameTable nameTable;
        private Hashtable qnames;
        private InternalWhitespaceRule ruleTemp;
        private ArrayList wildcards;

        public WhitespaceRuleLookup()
        {
            this.qnames = new Hashtable();
            this.wildcards = new ArrayList();
        }

        public WhitespaceRuleLookup(IList<WhitespaceRule> rules) : this()
        {
            for (int i = rules.Count - 1; i >= 0; i--)
            {
                WhitespaceRule rule = rules[i];
                InternalWhitespaceRule rule2 = new InternalWhitespaceRule(rule.LocalName, rule.NamespaceName, rule.PreserveSpace, -i);
                if ((rule.LocalName == null) || (rule.NamespaceName == null))
                {
                    this.wildcards.Add(rule2);
                }
                else
                {
                    this.qnames[rule2] = rule2;
                }
            }
            this.ruleTemp = new InternalWhitespaceRule();
        }

        public void Atomize(XmlNameTable nameTable)
        {
            if (nameTable != this.nameTable)
            {
                this.nameTable = nameTable;
                foreach (InternalWhitespaceRule rule in this.qnames.Values)
                {
                    rule.Atomize(nameTable);
                }
                foreach (InternalWhitespaceRule rule2 in this.wildcards)
                {
                    rule2.Atomize(nameTable);
                }
            }
        }

        public bool ShouldStripSpace(string localName, string namespaceName)
        {
            this.ruleTemp.Init(localName, namespaceName, false, 0);
            InternalWhitespaceRule rule = this.qnames[this.ruleTemp] as InternalWhitespaceRule;
            int count = this.wildcards.Count;
            while (count-- != 0)
            {
                InternalWhitespaceRule rule2 = this.wildcards[count] as InternalWhitespaceRule;
                if (rule != null)
                {
                    if (rule.Priority > rule2.Priority)
                    {
                        return !rule.PreserveSpace;
                    }
                    if (rule.PreserveSpace == rule2.PreserveSpace)
                    {
                        continue;
                    }
                }
                if (((rule2.LocalName == null) || (rule2.LocalName == localName)) && ((rule2.NamespaceName == null) || (rule2.NamespaceName == namespaceName)))
                {
                    return !rule2.PreserveSpace;
                }
            }
            return ((rule != null) && !rule.PreserveSpace);
        }

        private class InternalWhitespaceRule : WhitespaceRule
        {
            private int hashCode;
            private int priority;

            public InternalWhitespaceRule()
            {
            }

            public InternalWhitespaceRule(string localName, string namespaceName, bool preserveSpace, int priority)
            {
                this.Init(localName, namespaceName, preserveSpace, priority);
            }

            public void Atomize(XmlNameTable nameTable)
            {
                if (base.LocalName != null)
                {
                    base.LocalName = nameTable.Add(base.LocalName);
                }
                if (base.NamespaceName != null)
                {
                    base.NamespaceName = nameTable.Add(base.NamespaceName);
                }
            }

            public override bool Equals(object obj)
            {
                WhitespaceRuleLookup.InternalWhitespaceRule rule = obj as WhitespaceRuleLookup.InternalWhitespaceRule;
                return ((base.LocalName == base.LocalName) && (base.NamespaceName == rule.NamespaceName));
            }

            public override int GetHashCode() => 
                this.hashCode;

            public void Init(string localName, string namespaceName, bool preserveSpace, int priority)
            {
                base.Init(localName, namespaceName, preserveSpace);
                this.priority = priority;
                if ((localName != null) && (namespaceName != null))
                {
                    this.hashCode = localName.GetHashCode();
                }
            }

            public int Priority =>
                this.priority;
        }
    }
}

