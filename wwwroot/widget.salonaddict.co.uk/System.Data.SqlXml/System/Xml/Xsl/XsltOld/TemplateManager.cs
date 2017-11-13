namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;
    using System.Xml;
    using System.Xml.XPath;

    internal class TemplateManager
    {
        private XmlQualifiedName mode;
        private static TemplateComparer s_TemplateComparer = new TemplateComparer();
        private Stylesheet stylesheet;
        internal ArrayList templates;

        internal TemplateManager(Stylesheet stylesheet, XmlQualifiedName mode)
        {
            this.mode = mode;
            this.stylesheet = stylesheet;
        }

        internal void AddTemplate(TemplateAction template)
        {
            if (this.templates == null)
            {
                this.templates = new ArrayList();
            }
            this.templates.Add(template);
        }

        internal TemplateAction FindTemplate(Processor processor, XPathNavigator navigator)
        {
            if (this.templates != null)
            {
                for (int i = this.templates.Count - 1; i >= 0; i--)
                {
                    TemplateAction action = (TemplateAction) this.templates[i];
                    int matchKey = action.MatchKey;
                    if ((matchKey != -1) && processor.Matches(navigator, matchKey))
                    {
                        return action;
                    }
                }
            }
            return null;
        }

        internal void ProcessTemplates()
        {
            if (this.templates != null)
            {
                this.templates.Sort(s_TemplateComparer);
            }
        }

        internal XmlQualifiedName Mode =>
            this.mode;

        private class TemplateComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                TemplateAction action = (TemplateAction) x;
                TemplateAction action2 = (TemplateAction) y;
                if (action.Priority == action2.Priority)
                {
                    return (action.TemplateId - action2.TemplateId);
                }
                if (action.Priority <= action2.Priority)
                {
                    return -1;
                }
                return 1;
            }
        }
    }
}

