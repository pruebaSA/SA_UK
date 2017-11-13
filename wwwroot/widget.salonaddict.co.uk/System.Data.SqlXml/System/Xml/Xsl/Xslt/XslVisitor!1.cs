namespace System.Xml.Xsl.Xslt
{
    using System;

    internal abstract class XslVisitor<T>
    {
        protected XslVisitor()
        {
        }

        protected virtual T Visit(XslNode node)
        {
            switch (node.NodeType)
            {
                case XslNodeType.ApplyImports:
                    return this.VisitApplyImports(node);

                case XslNodeType.ApplyTemplates:
                    return this.VisitApplyTemplates(node);

                case XslNodeType.Attribute:
                    return this.VisitAttribute((NodeCtor) node);

                case XslNodeType.AttributeSet:
                    return this.VisitAttributeSet((AttributeSet) node);

                case XslNodeType.CallTemplate:
                    return this.VisitCallTemplate(node);

                case XslNodeType.Choose:
                    return this.VisitChoose(node);

                case XslNodeType.Comment:
                    return this.VisitComment(node);

                case XslNodeType.Copy:
                    return this.VisitCopy(node);

                case XslNodeType.CopyOf:
                    return this.VisitCopyOf(node);

                case XslNodeType.Element:
                    return this.VisitElement((NodeCtor) node);

                case XslNodeType.Error:
                    return this.VisitError(node);

                case XslNodeType.ForEach:
                    return this.VisitForEach(node);

                case XslNodeType.If:
                    return this.VisitIf(node);

                case XslNodeType.Key:
                    return this.VisitKey((Key) node);

                case XslNodeType.List:
                    return this.VisitList(node);

                case XslNodeType.LiteralAttribute:
                    return this.VisitLiteralAttribute(node);

                case XslNodeType.LiteralElement:
                    return this.VisitLiteralElement(node);

                case XslNodeType.Message:
                    return this.VisitMessage(node);

                case XslNodeType.Nop:
                    return this.VisitNop(node);

                case XslNodeType.Number:
                    return this.VisitNumber((System.Xml.Xsl.Xslt.Number) node);

                case XslNodeType.Otherwise:
                    return this.VisitOtherwise(node);

                case XslNodeType.Param:
                    return this.VisitParam((VarPar) node);

                case XslNodeType.PI:
                    return this.VisitPI(node);

                case XslNodeType.Sort:
                    return this.VisitSort((Sort) node);

                case XslNodeType.Template:
                    return this.VisitTemplate((Template) node);

                case XslNodeType.Text:
                    return this.VisitText((Text) node);

                case XslNodeType.UseAttributeSet:
                    return this.VisitUseAttributeSet(node);

                case XslNodeType.ValueOf:
                    return this.VisitValueOf(node);

                case XslNodeType.ValueOfDoe:
                    return this.VisitValueOfDoe(node);

                case XslNodeType.Variable:
                    return this.VisitVariable((VarPar) node);

                case XslNodeType.WithParam:
                    return this.VisitWithParam((VarPar) node);
            }
            return this.VisitUnknown(node);
        }

        protected virtual T VisitApplyImports(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitApplyTemplates(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitAttribute(NodeCtor node) => 
            this.VisitChildren(node);

        protected virtual T VisitAttributeSet(AttributeSet node) => 
            this.VisitChildren(node);

        protected virtual T VisitCallTemplate(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitChildren(XslNode node)
        {
            foreach (XslNode node2 in node.Content)
            {
                this.Visit(node2);
            }
            return default(T);
        }

        protected virtual T VisitChoose(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitComment(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitCopy(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitCopyOf(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitElement(NodeCtor node) => 
            this.VisitChildren(node);

        protected virtual T VisitError(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitForEach(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitIf(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitKey(Key node) => 
            this.VisitChildren(node);

        protected virtual T VisitList(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitLiteralAttribute(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitLiteralElement(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitMessage(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitNop(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitNumber(System.Xml.Xsl.Xslt.Number node) => 
            this.VisitChildren(node);

        protected virtual T VisitOtherwise(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitParam(VarPar node) => 
            this.VisitChildren(node);

        protected virtual T VisitPI(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitSort(Sort node) => 
            this.VisitChildren(node);

        protected virtual T VisitTemplate(Template node) => 
            this.VisitChildren(node);

        protected virtual T VisitText(Text node) => 
            this.VisitChildren(node);

        protected virtual T VisitUnknown(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitUseAttributeSet(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitValueOf(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitValueOfDoe(XslNode node) => 
            this.VisitChildren(node);

        protected virtual T VisitVariable(VarPar node) => 
            this.VisitChildren(node);

        protected virtual T VisitWithParam(VarPar node) => 
            this.VisitChildren(node);
    }
}

