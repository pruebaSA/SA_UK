namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.Xsl;

    internal class WithParamAction : VariableAction
    {
        internal WithParamAction() : base(VariableType.WithParameter)
        {
        }

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            base.CheckRequiredAttribute(compiler, base.name, "name");
            if (compiler.Recurse())
            {
                base.CompileTemplate(compiler);
                compiler.ToParent();
                if ((base.selectKey != -1) && (base.containedActions != null))
                {
                    throw XsltException.Create("Xslt_VariableCntSel2", new string[] { base.nameStr });
                }
            }
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                {
                    if (base.selectKey == -1)
                    {
                        if (base.containedActions == null)
                        {
                            processor.SetParameter(base.name, string.Empty);
                            frame.Finished();
                        }
                        else
                        {
                            NavigatorOutput output = new NavigatorOutput(base.baseUri);
                            processor.PushOutput(output);
                            processor.PushActionFrame(frame);
                            frame.State = 1;
                        }
                        return;
                    }
                    object obj2 = processor.RunQuery(frame, base.selectKey);
                    processor.SetParameter(base.name, obj2);
                    frame.Finished();
                    return;
                }
                case 1:
                {
                    RecordOutput output2 = processor.PopOutput();
                    processor.SetParameter(base.name, ((NavigatorOutput) output2).Navigator);
                    frame.Finished();
                    return;
                }
            }
        }
    }
}

