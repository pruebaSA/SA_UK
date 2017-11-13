namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;

    internal class ForEachAction : ContainerAction
    {
        private const int ContentsProcessed = 5;
        private const int PositionAdvanced = 4;
        private const int ProcessedSort = 2;
        private const int ProcessNextNode = 3;
        private int selectKey = -1;
        private ContainerAction sortContainer;

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            base.CheckRequiredAttribute(compiler, this.selectKey != -1, "select");
            compiler.CanHaveApplyImports = false;
            if (compiler.Recurse())
            {
                this.CompileSortElements(compiler);
                base.CompileTemplate(compiler);
                compiler.ToParent();
            }
        }

        internal override bool CompileAttribute(Compiler compiler)
        {
            string localName = compiler.Input.LocalName;
            string xpathQuery = compiler.Input.Value;
            if (Keywords.Equals(localName, compiler.Atoms.Select))
            {
                this.selectKey = compiler.AddQuery(xpathQuery);
                return true;
            }
            return false;
        }

        protected void CompileSortElements(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
        Label_0007:
            switch (input.NodeType)
            {
                case XPathNodeType.Element:
                    if (!Keywords.Equals(input.NamespaceURI, input.Atoms.XsltNamespace) || !Keywords.Equals(input.LocalName, input.Atoms.Sort))
                    {
                        break;
                    }
                    if (this.sortContainer == null)
                    {
                        this.sortContainer = new ContainerAction();
                    }
                    this.sortContainer.AddAction(compiler.CreateSortAction());
                    goto Label_008F;

                case XPathNodeType.Text:
                    break;

                case XPathNodeType.SignificantWhitespace:
                    base.AddEvent(compiler.CreateTextEvent());
                    goto Label_008F;

                default:
                    goto Label_008F;
            }
            return;
        Label_008F:
            if (input.Advance())
            {
                goto Label_0007;
            }
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                    if (this.sortContainer == null)
                    {
                        break;
                    }
                    processor.InitSortArray();
                    processor.PushActionFrame(this.sortContainer, frame.NodeSet);
                    frame.State = 2;
                    return;

                case 1:
                    return;

                case 2:
                    break;

                case 3:
                    goto Label_0082;

                case 4:
                    goto Label_009B;

                case 5:
                    frame.State = 3;
                    goto Label_0082;

                default:
                    return;
            }
            frame.InitNewNodeSet(processor.StartQuery(frame.NodeSet, this.selectKey));
            if (this.sortContainer != null)
            {
                frame.SortNewNodeSet(processor, processor.SortArray);
            }
            frame.State = 3;
        Label_0082:
            if (frame.NewNextNode(processor))
            {
                frame.State = 4;
            }
            else
            {
                frame.Finished();
                return;
            }
        Label_009B:
            processor.PushActionFrame(frame, frame.NewNodeSet);
            frame.State = 5;
        }
    }
}

