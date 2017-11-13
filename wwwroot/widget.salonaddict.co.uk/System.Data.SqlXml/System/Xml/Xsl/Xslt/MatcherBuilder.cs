namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.XPath;

    internal class MatcherBuilder
    {
        private List<List<TemplateMatch>> allMatches = new List<List<TemplateMatch>>();
        private PatternBag attributePatterns = new PatternBag();
        private List<Pattern> commentPatterns = new List<Pattern>();
        private List<Pattern> documentPatterns = new List<Pattern>();
        private PatternBag elementPatterns = new PatternBag();
        private XPathQilFactory f;
        private List<Pattern> heterogenousPatterns = new List<Pattern>();
        private InvokeGenerator invkGen;
        private const int NoMatch = -1;
        private PatternBag piPatterns = new PatternBag();
        private int priority = -1;
        private ReferenceReplacer refReplacer;
        private List<Pattern> textPatterns = new List<Pattern>();

        public MatcherBuilder(XPathQilFactory f, ReferenceReplacer refReplacer, InvokeGenerator invkGen)
        {
            this.f = f;
            this.refReplacer = refReplacer;
            this.invkGen = invkGen;
        }

        private void AddPatterns(List<TemplateMatch> matches)
        {
            foreach (TemplateMatch match in matches)
            {
                Pattern item = new Pattern(match, ++this.priority);
                switch (match.NodeKind)
                {
                    case XmlNodeKindFlags.Comment:
                    {
                        this.commentPatterns.Add(item);
                        continue;
                    }
                    case XmlNodeKindFlags.PI:
                    {
                        this.piPatterns.Add(item);
                        continue;
                    }
                    case XmlNodeKindFlags.Document:
                    {
                        this.documentPatterns.Add(item);
                        continue;
                    }
                    case XmlNodeKindFlags.Element:
                    {
                        this.elementPatterns.Add(item);
                        continue;
                    }
                    case XmlNodeKindFlags.Attribute:
                    {
                        this.attributePatterns.Add(item);
                        continue;
                    }
                    case XmlNodeKindFlags.Text:
                    {
                        this.textPatterns.Add(item);
                        continue;
                    }
                }
                this.heterogenousPatterns.Add(item);
            }
        }

        public QilNode BuildMatcher(QilIterator it, IList<XslNode> actualArgs, QilNode otherwise)
        {
            QilNode node = this.f.Int32(-1);
            node = this.MatchPatterns(it, XmlQueryTypeFactory.PI, this.piPatterns, node);
            node = this.MatchPatterns(it, XmlQueryTypeFactory.Comment, this.commentPatterns, node);
            node = this.MatchPatterns(it, XmlQueryTypeFactory.Document, this.documentPatterns, node);
            node = this.MatchPatterns(it, XmlQueryTypeFactory.Text, this.textPatterns, node);
            node = this.MatchPatterns(it, XmlQueryTypeFactory.Attribute, this.attributePatterns, node);
            node = this.MatchPatterns(it, XmlQueryTypeFactory.Element, this.elementPatterns, node);
            node = this.MatchPatternsWhosePriorityGreater(it, this.heterogenousPatterns, node);
            if (this.IsNoMatch(node))
            {
                return otherwise;
            }
            QilNode[] args = new QilNode[this.priority + 2];
            int num = -1;
            foreach (List<TemplateMatch> list in this.allMatches)
            {
                foreach (TemplateMatch match in list)
                {
                    args[++num] = this.invkGen.GenerateInvoke(match.TemplateFunction, actualArgs);
                }
            }
            args[++num] = otherwise;
            return this.f.Choice(node, this.f.BranchList(args));
        }

        private void Clear()
        {
            this.priority = -1;
            this.elementPatterns.Clear();
            this.attributePatterns.Clear();
            this.textPatterns.Clear();
            this.documentPatterns.Clear();
            this.commentPatterns.Clear();
            this.piPatterns.Clear();
            this.heterogenousPatterns.Clear();
            this.allMatches.Clear();
        }

        private void CollectPatterns(Stylesheet sheet, QilName mode)
        {
            List<TemplateMatch> list;
            foreach (Stylesheet stylesheet in sheet.Imports)
            {
                this.CollectPatterns(stylesheet, mode);
            }
            if (sheet.TemplateMatches.TryGetValue(mode, out list))
            {
                this.AddPatterns(list);
                this.allMatches.Add(list);
            }
        }

        public void CollectPatterns(Stylesheet sheet, QilName mode, bool applyImports)
        {
            this.Clear();
            if (applyImports)
            {
                foreach (Stylesheet stylesheet in sheet.Imports)
                {
                    this.CollectPatterns(stylesheet, mode);
                }
            }
            else
            {
                this.CollectPatterns(sheet, mode);
            }
        }

        private bool IsNoMatch(QilNode matcher) => 
            (matcher.NodeType == QilNodeType.LiteralInt32);

        private QilNode MatchPattern(QilIterator it, TemplateMatch match)
        {
            QilNode condition = match.Condition;
            if (condition == null)
            {
                return this.f.True();
            }
            condition = condition.DeepClone(this.f.BaseFactory);
            return this.refReplacer.Replace(condition, match.Iterator, it);
        }

        private QilNode MatchPatterns(QilIterator it, List<Pattern> patternList)
        {
            QilNode falseBranch = this.f.Int32(-1);
            foreach (Pattern pattern in patternList)
            {
                falseBranch = this.f.Conditional(this.MatchPattern(it, pattern.Match), this.f.Int32(pattern.Priority), falseBranch);
            }
            return falseBranch;
        }

        private QilNode MatchPatterns(QilIterator it, XmlQueryType xt, List<Pattern> patternList, QilNode otherwise)
        {
            if (patternList.Count == 0)
            {
                return otherwise;
            }
            return this.f.Conditional(this.f.IsType(it, xt), this.MatchPatterns(it, patternList), otherwise);
        }

        private QilNode MatchPatterns(QilIterator it, XmlQueryType xt, PatternBag patternBag, QilNode otherwise)
        {
            if (patternBag.FixedNamePatternsNames.Count == 0)
            {
                return this.MatchPatterns(it, xt, patternBag.NonFixedNamePatterns, otherwise);
            }
            QilNode falseBranch = this.f.Int32(-1);
            foreach (QilName name in patternBag.FixedNamePatternsNames)
            {
                falseBranch = this.f.Conditional(this.f.Eq(this.f.NameOf(it), name.ShallowClone(this.f.BaseFactory)), this.MatchPatterns(it, patternBag.FixedNamePatterns[name]), falseBranch);
            }
            falseBranch = this.MatchPatternsWhosePriorityGreater(it, patternBag.NonFixedNamePatterns, falseBranch);
            return this.f.Conditional(this.f.IsType(it, xt), falseBranch, otherwise);
        }

        private QilNode MatchPatternsWhosePriorityGreater(QilIterator it, List<Pattern> patternList, QilNode matcher)
        {
            if (patternList.Count == 0)
            {
                return matcher;
            }
            if (this.IsNoMatch(matcher))
            {
                return this.MatchPatterns(it, patternList);
            }
            QilIterator trueBranch = this.f.Let(matcher);
            QilNode falseBranch = this.f.Int32(-1);
            int val = -1;
            foreach (Pattern pattern in patternList)
            {
                if (pattern.Priority > (val + 1))
                {
                    falseBranch = this.f.Conditional(this.f.Gt(trueBranch, this.f.Int32(val)), trueBranch, falseBranch);
                }
                falseBranch = this.f.Conditional(this.MatchPattern(it, pattern.Match), this.f.Int32(pattern.Priority), falseBranch);
                val = pattern.Priority;
            }
            if (val != this.priority)
            {
                falseBranch = this.f.Conditional(this.f.Gt(trueBranch, this.f.Int32(val)), trueBranch, falseBranch);
            }
            return this.f.Loop(trueBranch, falseBranch);
        }
    }
}

