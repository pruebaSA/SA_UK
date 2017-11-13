namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class VariableAction : ContainerAction, IXsltContextVariable
    {
        protected string baseUri;
        public static object BeingComputedMark = new object();
        protected XmlQualifiedName name;
        protected string nameStr;
        protected int selectKey = -1;
        protected int stylesheetid;
        private const int ValueCalculated = 2;
        private int varKey;
        protected VariableType varType;

        internal VariableAction(VariableType type)
        {
            this.varType = type;
        }

        internal override void Compile(Compiler compiler)
        {
            this.stylesheetid = compiler.Stylesheetid;
            this.baseUri = compiler.Input.BaseURI;
            base.CompileAttributes(compiler);
            base.CheckRequiredAttribute(compiler, this.name, "name");
            if (compiler.Recurse())
            {
                base.CompileTemplate(compiler);
                compiler.ToParent();
                if ((this.selectKey != -1) && (base.containedActions != null))
                {
                    throw XsltException.Create("Xslt_VariableCntSel2", new string[] { this.nameStr });
                }
            }
            if (base.containedActions != null)
            {
                this.baseUri = this.baseUri + '#' + compiler.GetUnicRtfId();
            }
            else
            {
                this.baseUri = null;
            }
            this.varKey = compiler.InsertVariable(this);
        }

        internal override bool CompileAttribute(Compiler compiler)
        {
            string localName = compiler.Input.LocalName;
            string xpathQuery = compiler.Input.Value;
            if (Keywords.Equals(localName, compiler.Atoms.Name))
            {
                this.nameStr = xpathQuery;
                this.name = compiler.CreateXPathQName(this.nameStr);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.Select))
            {
                this.selectKey = compiler.AddQuery(xpathQuery);
            }
            else
            {
                return false;
            }
            return true;
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            object navigator = null;
            switch (frame.State)
            {
                case 0:
                    if (!this.IsGlobal)
                    {
                        break;
                    }
                    if (frame.GetVariable(this.varKey) == null)
                    {
                        frame.SetVariable(this.varKey, BeingComputedMark);
                        break;
                    }
                    frame.Finished();
                    return;

                case 1:
                    navigator = ((NavigatorOutput) processor.PopOutput()).Navigator;
                    goto Label_00D9;

                case 2:
                    goto Label_00D9;

                default:
                    return;
            }
            if (this.varType == VariableType.GlobalParameter)
            {
                navigator = processor.GetGlobalParameter(this.name);
            }
            else if (this.varType == VariableType.LocalParameter)
            {
                navigator = processor.GetParameter(this.name);
            }
            if (navigator == null)
            {
                if (this.selectKey == -1)
                {
                    if (base.containedActions != null)
                    {
                        NavigatorOutput output = new NavigatorOutput(this.baseUri);
                        processor.PushOutput(output);
                        processor.PushActionFrame(frame);
                        frame.State = 1;
                        return;
                    }
                    navigator = string.Empty;
                }
                else
                {
                    navigator = processor.RunQuery(frame, this.selectKey);
                }
            }
        Label_00D9:
            frame.SetVariable(this.varKey, navigator);
            frame.Finished();
        }

        object IXsltContextVariable.Evaluate(XsltContext xsltContext) => 
            ((XsltCompileContext) xsltContext).EvaluateVariable(this);

        internal bool IsGlobal
        {
            get
            {
                if (this.varType != VariableType.GlobalVariable)
                {
                    return (this.varType == VariableType.GlobalParameter);
                }
                return true;
            }
        }

        internal XmlQualifiedName Name =>
            this.name;

        internal string NameStr =>
            this.nameStr;

        internal int Stylesheetid =>
            this.stylesheetid;

        bool IXsltContextVariable.IsLocal
        {
            get
            {
                if (this.varType != VariableType.LocalVariable)
                {
                    return (this.varType == VariableType.LocalParameter);
                }
                return true;
            }
        }

        bool IXsltContextVariable.IsParam
        {
            get
            {
                if (this.varType != VariableType.LocalParameter)
                {
                    return (this.varType == VariableType.GlobalParameter);
                }
                return true;
            }
        }

        XPathResultType IXsltContextVariable.VariableType =>
            XPathResultType.Any;

        internal int VarKey =>
            this.varKey;

        internal VariableType VarType =>
            this.varType;
    }
}

