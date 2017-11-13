namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class AttributeAction : ContainerAction
    {
        private InputScopeManager manager;
        private string name;
        private Avt nameAvt;
        private const int NameDone = 2;
        private Avt nsAvt;
        private string nsUri;
        private PrefixQName qname;

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
                    this.qname = CreateAttributeQName(this.name, this.nsUri, compiler.CloneScopeManager());
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
            else
            {
                return false;
            }
            return true;
        }

        private static PrefixQName CreateAttributeQName(string name, string nsUri, InputScopeManager manager)
        {
            if (name == "xmlns")
            {
                return null;
            }
            if (nsUri == "http://www.w3.org/2000/xmlns/")
            {
                throw XsltException.Create("Xslt_ReservedNS", new string[] { nsUri });
            }
            PrefixQName name2 = new PrefixQName();
            name2.SetQName(name);
            name2.Namespace = (nsUri != null) ? nsUri : manager.ResolveXPathNamespace(name2.Prefix);
            if (name2.Prefix.StartsWith("xml", StringComparison.Ordinal))
            {
                if (name2.Prefix.Length == 3)
                {
                    if ((name2.Namespace != "http://www.w3.org/XML/1998/namespace") || ((name2.Name != "lang") && (name2.Name != "space")))
                    {
                        name2.ClearPrefix();
                    }
                    return name2;
                }
                if (name2.Prefix != "xmlns")
                {
                    return name2;
                }
                if (name2.Namespace == "http://www.w3.org/2000/xmlns/")
                {
                    throw XsltException.Create("Xslt_InvalidPrefix", new string[] { name2.Prefix });
                }
                name2.ClearPrefix();
            }
            return name2;
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                    if (this.qname == null)
                    {
                        frame.CalulatedName = CreateAttributeQName((this.nameAvt == null) ? this.name : this.nameAvt.Evaluate(processor, frame), (this.nsAvt == null) ? this.nsUri : this.nsAvt.Evaluate(processor, frame), this.manager);
                        if (frame.CalulatedName == null)
                        {
                            frame.Finished();
                            return;
                        }
                        break;
                    }
                    frame.CalulatedName = this.qname;
                    break;

                case 1:
                    if (processor.EndEvent(XPathNodeType.Attribute))
                    {
                        frame.Finished();
                        return;
                    }
                    frame.State = 1;
                    return;

                case 2:
                    break;

                default:
                    return;
            }
            PrefixQName calulatedName = frame.CalulatedName;
            if (!processor.BeginEvent(XPathNodeType.Attribute, calulatedName.Prefix, calulatedName.Name, calulatedName.Namespace, false))
            {
                frame.State = 2;
            }
            else
            {
                processor.PushActionFrame(frame);
                frame.State = 1;
            }
        }
    }
}

