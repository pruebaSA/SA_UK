namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Globalization;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class SortAction : CompiledAction
    {
        private XmlCaseOrder caseOrder;
        private Avt caseOrderAvt;
        private XmlDataType dataType = XmlDataType.Text;
        private Avt dataTypeAvt;
        private bool forwardCompatibility;
        private string lang;
        private Avt langAvt;
        private InputScopeManager manager;
        private XmlSortOrder order = XmlSortOrder.Ascending;
        private Avt orderAvt;
        private int selectKey = -1;
        private Sort sort;

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            base.CheckEmpty(compiler);
            if (this.selectKey == -1)
            {
                this.selectKey = compiler.AddQuery(".");
            }
            this.forwardCompatibility = compiler.ForwardCompatibility;
            this.manager = compiler.CloneScopeManager();
            this.lang = this.ParseLang(CompiledAction.PrecalculateAvt(ref this.langAvt));
            this.dataType = this.ParseDataType(CompiledAction.PrecalculateAvt(ref this.dataTypeAvt), this.manager);
            this.order = this.ParseOrder(CompiledAction.PrecalculateAvt(ref this.orderAvt));
            this.caseOrder = this.ParseCaseOrder(CompiledAction.PrecalculateAvt(ref this.caseOrderAvt));
            if (((this.langAvt == null) && (this.dataTypeAvt == null)) && ((this.orderAvt == null) && (this.caseOrderAvt == null)))
            {
                this.sort = new Sort(this.selectKey, this.lang, this.dataType, this.order, this.caseOrder);
            }
        }

        internal override bool CompileAttribute(Compiler compiler)
        {
            string localName = compiler.Input.LocalName;
            string xpathQuery = compiler.Input.Value;
            if (Keywords.Equals(localName, compiler.Atoms.Select))
            {
                this.selectKey = compiler.AddQuery(xpathQuery);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.Lang))
            {
                this.langAvt = Avt.CompileAvt(compiler, xpathQuery);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.DataType))
            {
                this.dataTypeAvt = Avt.CompileAvt(compiler, xpathQuery);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.Order))
            {
                this.orderAvt = Avt.CompileAvt(compiler, xpathQuery);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.CaseOrder))
            {
                this.caseOrderAvt = Avt.CompileAvt(compiler, xpathQuery);
            }
            else
            {
                return false;
            }
            return true;
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            processor.AddSort((this.sort != null) ? this.sort : new Sort(this.selectKey, (this.langAvt == null) ? this.lang : this.ParseLang(this.langAvt.Evaluate(processor, frame)), (this.dataTypeAvt == null) ? this.dataType : this.ParseDataType(this.dataTypeAvt.Evaluate(processor, frame), this.manager), (this.orderAvt == null) ? this.order : this.ParseOrder(this.orderAvt.Evaluate(processor, frame)), (this.caseOrderAvt == null) ? this.caseOrder : this.ParseCaseOrder(this.caseOrderAvt.Evaluate(processor, frame))));
            frame.Finished();
        }

        private XmlCaseOrder ParseCaseOrder(string value)
        {
            if (value != null)
            {
                if (value == "upper-first")
                {
                    return XmlCaseOrder.UpperFirst;
                }
                if (value == "lower-first")
                {
                    return XmlCaseOrder.LowerFirst;
                }
                if (!this.forwardCompatibility)
                {
                    throw XsltException.Create("Xslt_InvalidAttrValue", new string[] { "case-order", value });
                }
            }
            return XmlCaseOrder.None;
        }

        private XmlDataType ParseDataType(string value, InputScopeManager manager)
        {
            if (value != null)
            {
                string str;
                string str2;
                if (value == "text")
                {
                    return XmlDataType.Text;
                }
                if (value == "number")
                {
                    return XmlDataType.Number;
                }
                PrefixQName.ParseQualifiedName(value, out str, out str2);
                manager.ResolveXmlNamespace(str);
                if ((str.Length == 0) && !this.forwardCompatibility)
                {
                    throw XsltException.Create("Xslt_InvalidAttrValue", new string[] { "data-type", value });
                }
            }
            return XmlDataType.Text;
        }

        private string ParseLang(string value)
        {
            if (value != null)
            {
                if (XmlComplianceUtil.IsValidLanguageID(value.ToCharArray(), 0, value.Length) || ((value.Length != 0) && (CultureInfo.GetCultureInfo(value) != null)))
                {
                    return value;
                }
                if (!this.forwardCompatibility)
                {
                    throw XsltException.Create("Xslt_InvalidAttrValue", new string[] { "lang", value });
                }
            }
            return null;
        }

        private XmlSortOrder ParseOrder(string value)
        {
            if (value != null)
            {
                if (value == "ascending")
                {
                    return XmlSortOrder.Ascending;
                }
                if (value == "descending")
                {
                    return XmlSortOrder.Descending;
                }
                if (!this.forwardCompatibility)
                {
                    throw XsltException.Create("Xslt_InvalidAttrValue", new string[] { "order", value });
                }
            }
            return XmlSortOrder.Ascending;
        }
    }
}

