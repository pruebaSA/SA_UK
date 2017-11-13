namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class Stylesheet
    {
        private Hashtable attributeSetTable;
        private ArrayList imports = new ArrayList();
        private Hashtable modeManagers;
        private Hashtable queryKeyTable;
        private Hashtable scriptObjectTypes = new Hashtable();
        private int templateCount;
        private Hashtable templateNameTable = new Hashtable();
        private TemplateManager templates;
        private bool whitespace;
        private ArrayList whitespaceList;

        internal void AddAttributeSet(AttributeSetAction attributeSet)
        {
            if (this.attributeSetTable == null)
            {
                this.attributeSetTable = new Hashtable();
            }
            if (!this.attributeSetTable.ContainsKey(attributeSet.Name))
            {
                this.attributeSetTable[attributeSet.Name] = attributeSet;
            }
            else
            {
                ((AttributeSetAction) this.attributeSetTable[attributeSet.Name]).Merge(attributeSet);
            }
        }

        internal void AddSpace(Compiler compiler, string query, double Priority, bool PreserveSpace)
        {
            if (this.queryKeyTable != null)
            {
                if (this.queryKeyTable.Contains(query))
                {
                    ((WhitespaceElement) this.queryKeyTable[query]).ReplaceValue(PreserveSpace);
                    return;
                }
            }
            else
            {
                this.queryKeyTable = new Hashtable();
                this.whitespaceList = new ArrayList();
            }
            WhitespaceElement element = new WhitespaceElement(compiler.AddQuery(query), Priority, PreserveSpace);
            this.queryKeyTable[query] = element;
            this.whitespaceList.Add(element);
        }

        internal void AddTemplate(TemplateAction template)
        {
            XmlQualifiedName mode = template.Mode;
            if (template.Name != null)
            {
                if (this.templateNameTable.ContainsKey(template.Name))
                {
                    throw XsltException.Create("Xslt_DupTemplateName", new string[] { template.Name.ToString() });
                }
                this.templateNameTable[template.Name] = template;
            }
            if (template.MatchKey != -1)
            {
                if (this.modeManagers == null)
                {
                    this.modeManagers = new Hashtable();
                }
                if (mode == null)
                {
                    mode = XmlQualifiedName.Empty;
                }
                TemplateManager manager = (TemplateManager) this.modeManagers[mode];
                if (manager == null)
                {
                    manager = new TemplateManager(this, mode);
                    this.modeManagers[mode] = manager;
                    if (mode.IsEmpty)
                    {
                        this.templates = manager;
                    }
                }
                template.TemplateId = ++this.templateCount;
                manager.AddTemplate(template);
            }
        }

        internal TemplateAction FindTemplate(XmlQualifiedName name)
        {
            TemplateAction action = null;
            if (this.templateNameTable != null)
            {
                action = (TemplateAction) this.templateNameTable[name];
            }
            if ((action == null) && (this.imports != null))
            {
                for (int i = this.imports.Count - 1; i >= 0; i--)
                {
                    action = ((Stylesheet) this.imports[i]).FindTemplate(name);
                    if (action != null)
                    {
                        return action;
                    }
                }
            }
            return action;
        }

        internal TemplateAction FindTemplate(Processor processor, XPathNavigator navigator)
        {
            TemplateAction action = null;
            if (this.templates != null)
            {
                action = this.templates.FindTemplate(processor, navigator);
            }
            if (action == null)
            {
                action = this.FindTemplateImports(processor, navigator);
            }
            return action;
        }

        internal TemplateAction FindTemplate(Processor processor, XPathNavigator navigator, XmlQualifiedName mode)
        {
            TemplateAction action = null;
            if (this.modeManagers != null)
            {
                TemplateManager manager = (TemplateManager) this.modeManagers[mode];
                if (manager != null)
                {
                    action = manager.FindTemplate(processor, navigator);
                }
            }
            if (action == null)
            {
                action = this.FindTemplateImports(processor, navigator, mode);
            }
            return action;
        }

        internal TemplateAction FindTemplateImports(Processor processor, XPathNavigator navigator)
        {
            TemplateAction action = null;
            if (this.imports != null)
            {
                for (int i = this.imports.Count - 1; i >= 0; i--)
                {
                    action = ((Stylesheet) this.imports[i]).FindTemplate(processor, navigator);
                    if (action != null)
                    {
                        return action;
                    }
                }
            }
            return action;
        }

        internal TemplateAction FindTemplateImports(Processor processor, XPathNavigator navigator, XmlQualifiedName mode)
        {
            TemplateAction action = null;
            if (this.imports != null)
            {
                for (int i = this.imports.Count - 1; i >= 0; i--)
                {
                    action = ((Stylesheet) this.imports[i]).FindTemplate(processor, navigator, mode);
                    if (action != null)
                    {
                        return action;
                    }
                }
            }
            return action;
        }

        internal bool PreserveWhiteSpace(Processor proc, XPathNavigator node)
        {
            if (this.whitespaceList != null)
            {
                for (int i = this.whitespaceList.Count - 1; 0 <= i; i--)
                {
                    WhitespaceElement element = (WhitespaceElement) this.whitespaceList[i];
                    if (proc.Matches(node, element.Key))
                    {
                        return element.PreserveSpace;
                    }
                }
            }
            if (this.imports != null)
            {
                for (int j = this.imports.Count - 1; j >= 0; j--)
                {
                    Stylesheet stylesheet = (Stylesheet) this.imports[j];
                    if (!stylesheet.PreserveWhiteSpace(proc, node))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal void ProcessTemplates()
        {
            if (this.modeManagers != null)
            {
                IDictionaryEnumerator enumerator = this.modeManagers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ((TemplateManager) enumerator.Value).ProcessTemplates();
                }
            }
            if (this.imports != null)
            {
                for (int i = this.imports.Count - 1; i >= 0; i--)
                {
                    ((Stylesheet) this.imports[i]).ProcessTemplates();
                }
            }
        }

        internal void ReplaceNamespaceAlias(Compiler compiler)
        {
            if (this.modeManagers != null)
            {
                IDictionaryEnumerator enumerator = this.modeManagers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TemplateManager manager = (TemplateManager) enumerator.Value;
                    if (manager.templates != null)
                    {
                        for (int i = 0; i < manager.templates.Count; i++)
                        {
                            ((TemplateAction) manager.templates[i]).ReplaceNamespaceAlias(compiler);
                        }
                    }
                }
            }
            if (this.templateNameTable != null)
            {
                IDictionaryEnumerator enumerator2 = this.templateNameTable.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    ((TemplateAction) enumerator2.Value).ReplaceNamespaceAlias(compiler);
                }
            }
            if (this.imports != null)
            {
                for (int j = this.imports.Count - 1; j >= 0; j--)
                {
                    ((Stylesheet) this.imports[j]).ReplaceNamespaceAlias(compiler);
                }
            }
        }

        internal void SortWhiteSpace()
        {
            if (this.queryKeyTable != null)
            {
                for (int i = 0; i < this.whitespaceList.Count; i++)
                {
                    for (int j = this.whitespaceList.Count - 1; j > i; j--)
                    {
                        WhitespaceElement element = (WhitespaceElement) this.whitespaceList[j - 1];
                        WhitespaceElement element2 = (WhitespaceElement) this.whitespaceList[j];
                        if (element2.Priority < element.Priority)
                        {
                            this.whitespaceList[j - 1] = element2;
                            this.whitespaceList[j] = element;
                        }
                    }
                }
                this.whitespace = true;
            }
            if (this.imports != null)
            {
                for (int k = this.imports.Count - 1; k >= 0; k--)
                {
                    Stylesheet stylesheet = (Stylesheet) this.imports[k];
                    if (stylesheet.Whitespace)
                    {
                        stylesheet.SortWhiteSpace();
                        this.whitespace = true;
                    }
                }
            }
        }

        internal Hashtable AttributeSetTable =>
            this.attributeSetTable;

        internal ArrayList Imports =>
            this.imports;

        internal Hashtable ScriptObjectTypes =>
            this.scriptObjectTypes;

        internal bool Whitespace =>
            this.whitespace;

        private class WhitespaceElement
        {
            private int key;
            private bool preserveSpace;
            private double priority;

            internal WhitespaceElement(int Key, double priority, bool PreserveSpace)
            {
                this.key = Key;
                this.priority = priority;
                this.preserveSpace = PreserveSpace;
            }

            internal void ReplaceValue(bool PreserveSpace)
            {
                this.preserveSpace = PreserveSpace;
            }

            internal int Key =>
                this.key;

            internal bool PreserveSpace =>
                this.preserveSpace;

            internal double Priority =>
                this.priority;
        }
    }
}

