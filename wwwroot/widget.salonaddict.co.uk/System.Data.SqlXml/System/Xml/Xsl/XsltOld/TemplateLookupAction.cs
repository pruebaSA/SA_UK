namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml;
    using System.Xml.XPath;

    internal class TemplateLookupAction : Action
    {
        protected Stylesheet importsOf;
        protected XmlQualifiedName mode;

        internal Action BuiltInTemplate(XPathNavigator node)
        {
            Action action = null;
            switch (node.NodeType)
            {
                case XPathNodeType.Root:
                case XPathNodeType.Element:
                    return ApplyTemplatesAction.BuiltInRule(this.mode);

                case XPathNodeType.Attribute:
                case XPathNodeType.Text:
                case XPathNodeType.SignificantWhitespace:
                case XPathNodeType.Whitespace:
                    return ValueOfAction.BuiltInRule();

                case XPathNodeType.Namespace:
                case XPathNodeType.ProcessingInstruction:
                case XPathNodeType.Comment:
                case XPathNodeType.All:
                    return action;
            }
            return action;
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            Action action = null;
            if (this.mode != null)
            {
                action = (this.importsOf == null) ? processor.Stylesheet.FindTemplate(processor, frame.Node, this.mode) : this.importsOf.FindTemplateImports(processor, frame.Node, this.mode);
            }
            else
            {
                action = (this.importsOf == null) ? processor.Stylesheet.FindTemplate(processor, frame.Node) : this.importsOf.FindTemplateImports(processor, frame.Node);
            }
            if (action == null)
            {
                action = this.BuiltInTemplate(frame.Node);
            }
            if (action != null)
            {
                frame.SetAction(action);
            }
            else
            {
                frame.Finished();
            }
        }

        internal void Initialize(XmlQualifiedName mode, Stylesheet importsOf)
        {
            this.mode = mode;
            this.importsOf = importsOf;
        }
    }
}

