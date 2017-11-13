namespace System.Xml.Xsl.XsltOld
{
    using System;

    internal class ValueOfAction : CompiledAction
    {
        private bool disableOutputEscaping;
        private const int ResultStored = 2;
        private static Action s_BuiltInRule = new BuiltInRuleTextAction();
        private int selectKey = -1;

        internal static Action BuiltInRule() => 
            s_BuiltInRule;

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            base.CheckRequiredAttribute(compiler, this.selectKey != -1, "select");
            base.CheckEmpty(compiler);
        }

        internal override bool CompileAttribute(Compiler compiler)
        {
            string localName = compiler.Input.LocalName;
            string xpathQuery = compiler.Input.Value;
            if (Keywords.Equals(localName, compiler.Atoms.Select))
            {
                this.selectKey = compiler.AddQuery(xpathQuery);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.DisableOutputEscaping))
            {
                this.disableOutputEscaping = compiler.GetYesNo(xpathQuery);
            }
            else
            {
                return false;
            }
            return true;
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                {
                    string text = processor.ValueOf(frame, this.selectKey);
                    if (!processor.TextEvent(text, this.disableOutputEscaping))
                    {
                        frame.StoredOutput = text;
                        frame.State = 2;
                        return;
                    }
                    frame.Finished();
                    return;
                }
                case 1:
                    break;

                case 2:
                    processor.TextEvent(frame.StoredOutput);
                    frame.Finished();
                    break;

                default:
                    return;
            }
        }
    }
}

