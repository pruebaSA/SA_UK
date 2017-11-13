namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Xsl.Qil;

    internal class Stylesheet
    {
        public Dictionary<QilName, List<QilFunction>> ApplyImportsFunctions = new Dictionary<QilName, List<QilFunction>>();
        public Dictionary<QilName, AttributeSet> AttributeSets = new Dictionary<QilName, AttributeSet>();
        private Compiler compiler;
        public List<XslNode> GlobalVarPars = new List<XslNode>();
        public List<Uri> ImportHrefs = new List<Uri>();
        private int importPrecedence;
        public Stylesheet[] Imports;
        private int orderNumber;
        public Dictionary<QilName, List<TemplateMatch>> TemplateMatches = new Dictionary<QilName, List<TemplateMatch>>();
        public List<Template> Templates = new List<Template>();
        public List<WhitespaceRule>[] WhitespaceRules = new List<WhitespaceRule>[3];

        public Stylesheet(Compiler compiler, int importPrecedence)
        {
            this.compiler = compiler;
            this.importPrecedence = importPrecedence;
            this.WhitespaceRules[0] = new List<WhitespaceRule>();
            this.WhitespaceRules[1] = new List<WhitespaceRule>();
            this.WhitespaceRules[2] = new List<WhitespaceRule>();
        }

        public bool AddTemplate(Template template)
        {
            template.ImportPrecedence = this.importPrecedence;
            template.OrderNumber = this.orderNumber++;
            this.compiler.AllTemplates.Add(template);
            if (template.Name != null)
            {
                Template template2;
                if (!this.compiler.NamedTemplates.TryGetValue(template.Name, out template2))
                {
                    this.compiler.NamedTemplates[template.Name] = template;
                }
                else if (template2.ImportPrecedence == template.ImportPrecedence)
                {
                    return false;
                }
            }
            if (template.Match != null)
            {
                this.Templates.Add(template);
            }
            return true;
        }

        public void AddTemplateMatch(Template template, QilLoop filter)
        {
            List<TemplateMatch> list;
            if (!this.TemplateMatches.TryGetValue(template.Mode, out list))
            {
                list = this.TemplateMatches[template.Mode] = new List<TemplateMatch>();
            }
            list.Add(new TemplateMatch(template, filter));
        }

        public bool AddVarPar(VarPar var)
        {
            foreach (XslNode node in this.GlobalVarPars)
            {
                if (node.Name.Equals(var.Name))
                {
                    return this.compiler.AllGlobalVarPars.ContainsKey(var.Name);
                }
            }
            this.GlobalVarPars.Add(var);
            return true;
        }

        public void AddWhitespaceRule(int index, WhitespaceRule rule)
        {
            this.WhitespaceRules[index].Add(rule);
        }

        public void SortTemplateMatches()
        {
            foreach (QilName name in this.TemplateMatches.Keys)
            {
                this.TemplateMatches[name].Sort(TemplateMatch.Comparer);
            }
        }

        public int ImportPrecedence =>
            this.importPrecedence;
    }
}

