namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class ChooseAction : ContainerAction
    {
        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            if (compiler.Recurse())
            {
                this.CompileConditions(compiler);
                compiler.ToParent();
            }
        }

        private void CompileConditions(Compiler compiler)
        {
            IfAction action;
            NavigatorInput input = compiler.Input;
            bool flag = false;
            bool flag2 = false;
        Label_000B:
            switch (input.NodeType)
            {
                case XPathNodeType.Element:
                {
                    compiler.PushNamespaceScope();
                    string namespaceURI = input.NamespaceURI;
                    string localName = input.LocalName;
                    if (!Keywords.Equals(namespaceURI, input.Atoms.XsltNamespace))
                    {
                        throw compiler.UnexpectedKeyword();
                    }
                    action = null;
                    if (!Keywords.Equals(localName, input.Atoms.When))
                    {
                        if (!Keywords.Equals(localName, input.Atoms.Otherwise))
                        {
                            throw compiler.UnexpectedKeyword();
                        }
                        if (flag2)
                        {
                            throw XsltException.Create("Xslt_DupOtherwise", new string[0]);
                        }
                        action = compiler.CreateIfAction(IfAction.ConditionType.ConditionOtherwise);
                        flag2 = true;
                        break;
                    }
                    if (flag2)
                    {
                        throw XsltException.Create("Xslt_WhenAfterOtherwise", new string[0]);
                    }
                    action = compiler.CreateIfAction(IfAction.ConditionType.ConditionWhen);
                    flag = true;
                    break;
                }
                case XPathNodeType.SignificantWhitespace:
                case XPathNodeType.Whitespace:
                case XPathNodeType.ProcessingInstruction:
                case XPathNodeType.Comment:
                    goto Label_0114;

                default:
                    throw XsltException.Create("Xslt_InvalidContents", new string[] { "choose" });
            }
            base.AddAction(action);
            compiler.PopScope();
        Label_0114:
            if (compiler.Advance())
            {
                goto Label_000B;
            }
            if (!flag)
            {
                throw XsltException.Create("Xslt_NoWhen", new string[0]);
            }
        }
    }
}

