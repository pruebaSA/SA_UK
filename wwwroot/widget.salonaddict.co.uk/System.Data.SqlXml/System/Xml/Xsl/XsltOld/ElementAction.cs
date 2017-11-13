namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class ElementAction : ContainerAction
    {
        private bool empty;
        private InputScopeManager manager;
        private string name;
        private Avt nameAvt;
        private const int NameDone = 2;
        private Avt nsAvt;
        private string nsUri;
        private PrefixQName qname;

        internal ElementAction()
        {
        }

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            base.CheckRequiredAttribute(compiler, this.nameAvt, "name");
            this.name = CompiledAction.PrecalculateAvt(ref this.nameAvt);
            this.nsUri = CompiledAction.PrecalculateAvt(ref this.nsAvt);
            if ((this.nameAvt == null) && (this.nsAvt == null))
            {
                if (this.name != "xmlns")
                {
                    this.qname = CreateElementQName(this.name, this.nsUri, compiler.CloneScopeManager());
                }
            }
            else
            {
                this.manager = compiler.CloneScopeManager();
            }
            if (compiler.Recurse())
            {
                base.CompileTemplate(compiler);
                compiler.ToParent();
            }
            this.empty = base.containedActions == null;
        }

        internal override bool CompileAttribute(Compiler compiler)
        {
            string localName = compiler.Input.LocalName;
            string avtText = compiler.Input.Value;
            if (Keywords.Equals(localName, compiler.Atoms.Name))
            {
                this.nameAvt = Avt.CompileAvt(compiler, avtText);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.Namespace))
            {
                this.nsAvt = Avt.CompileAvt(compiler, avtText);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.UseAttributeSets))
            {
                base.AddAction(compiler.CreateUseAttributeSetsAction());
            }
            else
            {
                return false;
            }
            return true;
        }

        private static PrefixQName CreateElementQName(string name, string nsUri, InputScopeManager manager)
        {
            if (nsUri == "http://www.w3.org/2000/xmlns/")
            {
                throw XsltException.Create("Xslt_ReservedNS", new string[] { nsUri });
            }
            PrefixQName name2 = new PrefixQName();
            name2.SetQName(name);
            if (nsUri == null)
            {
                name2.Namespace = manager.ResolveXmlNamespace(name2.Prefix);
                return name2;
            }
            name2.Namespace = nsUri;
            return name2;
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                    if (this.qname == null)
                    {
                        frame.CalulatedName = CreateElementQName((this.nameAvt == null) ? this.name : this.nameAvt.Evaluate(processor, frame), (this.nsAvt == null) ? this.nsUri : this.nsAvt.Evaluate(processor, frame), this.manager);
                        break;
                    }
                    frame.CalulatedName = this.qname;
                    break;

                case 1:
                    goto Label_00C2;

                case 2:
                    break;

                default:
                    return;
            }
            PrefixQName calulatedName = frame.CalulatedName;
            if (!processor.BeginEvent(XPathNodeType.Element, calulatedName.Prefix, calulatedName.Name, calulatedName.Namespace, this.empty))
            {
                frame.State = 2;
                return;
            }
            if (!this.empty)
            {
                processor.PushActionFrame(frame);
                frame.State = 1;
                return;
            }
        Label_00C2:
            if (!processor.EndEvent(XPathNodeType.Element))
            {
                frame.State = 1;
            }
            else
            {
                frame.Finished();
            }
        }
    }
}

