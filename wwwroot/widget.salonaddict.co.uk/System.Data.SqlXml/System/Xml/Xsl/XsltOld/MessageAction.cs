namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Xml.Xsl;

    internal class MessageAction : ContainerAction
    {
        private bool _Terminate;

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            if (compiler.Recurse())
            {
                base.CompileTemplate(compiler);
                compiler.ToParent();
            }
        }

        internal override bool CompileAttribute(Compiler compiler)
        {
            string localName = compiler.Input.LocalName;
            string str2 = compiler.Input.Value;
            if (Keywords.Equals(localName, compiler.Atoms.Terminate))
            {
                this._Terminate = compiler.GetYesNo(str2);
                return true;
            }
            return false;
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                {
                    TextOnlyOutput output = new TextOnlyOutput(processor, new StringWriter(CultureInfo.InvariantCulture));
                    processor.PushOutput(output);
                    processor.PushActionFrame(frame);
                    frame.State = 1;
                    return;
                }
                case 1:
                {
                    TextOnlyOutput output2 = processor.PopOutput() as TextOnlyOutput;
                    Console.WriteLine(output2.Writer.ToString());
                    if (this._Terminate)
                    {
                        throw XsltException.Create("Xslt_Terminate", new string[] { output2.Writer.ToString() });
                    }
                    frame.Finished();
                    return;
                }
            }
        }
    }
}

