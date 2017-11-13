namespace System.Xml.Xsl.XsltOld
{
    using System;

    internal class IfAction : ContainerAction
    {
        private int testKey = -1;
        private ConditionType type;

        internal IfAction(ConditionType type)
        {
            this.type = type;
        }

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            if (this.type != ConditionType.ConditionOtherwise)
            {
                base.CheckRequiredAttribute(compiler, this.testKey != -1, "test");
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
            string xpathQuery = compiler.Input.Value;
            if (!Keywords.Equals(localName, compiler.Atoms.Test))
            {
                return false;
            }
            if (this.type == ConditionType.ConditionOtherwise)
            {
                return false;
            }
            this.testKey = compiler.AddBooleanQuery(xpathQuery);
            return true;
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                    if (((this.type == ConditionType.ConditionIf) || (this.type == ConditionType.ConditionWhen)) && !processor.EvaluateBoolean(frame, this.testKey))
                    {
                        frame.Finished();
                        return;
                    }
                    processor.PushActionFrame(frame);
                    frame.State = 1;
                    return;

                case 1:
                    if ((this.type == ConditionType.ConditionWhen) || (this.type == ConditionType.ConditionOtherwise))
                    {
                        frame.Exit();
                    }
                    frame.Finished();
                    return;
            }
        }

        internal enum ConditionType
        {
            ConditionIf,
            ConditionWhen,
            ConditionOtherwise
        }
    }
}

