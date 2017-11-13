namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml;
    using System.Xml.XPath;

    internal class ProcessingInstructionAction : ContainerAction
    {
        private const char Charl = 'l';
        private const char CharL = 'L';
        private const char Charm = 'm';
        private const char CharM = 'M';
        private const char Charx = 'x';
        private const char CharX = 'X';
        private string name;
        private Avt nameAvt;
        private const int NameEvaluated = 2;
        private const int NameReady = 3;

        internal ProcessingInstructionAction()
        {
        }

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            base.CheckRequiredAttribute(compiler, this.nameAvt, "name");
            if (this.nameAvt.IsConstant)
            {
                this.name = this.nameAvt.Evaluate(null, null);
                this.nameAvt = null;
                if (!IsProcessingInstructionName(this.name))
                {
                    this.name = null;
                }
            }
            if (compiler.Recurse())
            {
                base.CompileTemplate(compiler);
                compiler.ToParent();
            }
        }

        internal override bool CompileAttribute(Compiler compiler)
        {
            string localName = compiler.Input.LocalName;
            string avtText = compiler.Input.Value;
            if (Keywords.Equals(localName, compiler.Atoms.Name))
            {
                this.nameAvt = Avt.CompileAvt(compiler, avtText);
                return true;
            }
            return false;
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                    if (this.nameAvt != null)
                    {
                        frame.StoredOutput = this.nameAvt.Evaluate(processor, frame);
                        if (!IsProcessingInstructionName(frame.StoredOutput))
                        {
                            frame.Finished();
                            return;
                        }
                        break;
                    }
                    frame.StoredOutput = this.name;
                    if (this.name != null)
                    {
                        break;
                    }
                    frame.Finished();
                    return;

                case 1:
                    if (processor.EndEvent(XPathNodeType.ProcessingInstruction))
                    {
                        frame.Finished();
                        return;
                    }
                    frame.State = 1;
                    return;

                case 3:
                    break;

                default:
                    frame.Finished();
                    return;
            }
            if (!processor.BeginEvent(XPathNodeType.ProcessingInstruction, string.Empty, frame.StoredOutput, string.Empty, false))
            {
                frame.State = 3;
            }
            else
            {
                processor.PushActionFrame(frame);
                frame.State = 1;
            }
        }

        internal static bool IsProcessingInstructionName(string name)
        {
            if (name != null)
            {
                int length = name.Length;
                int num2 = 0;
                XmlCharType instance = XmlCharType.Instance;
                while ((num2 < length) && instance.IsWhiteSpace(name[num2]))
                {
                    num2++;
                }
                if (num2 >= length)
                {
                    return false;
                }
                if ((num2 >= length) || instance.IsStartNCNameChar(name[num2]))
                {
                    while ((num2 < length) && instance.IsNCNameChar(name[num2]))
                    {
                        num2++;
                    }
                    while ((num2 < length) && instance.IsWhiteSpace(name[num2]))
                    {
                        num2++;
                    }
                    if (num2 < length)
                    {
                        return false;
                    }
                    if ((((length == 3) && ((name[0] == 'X') || (name[0] == 'x'))) && ((name[1] == 'M') || (name[1] == 'm'))) && ((name[2] == 'L') || (name[2] == 'l')))
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}

