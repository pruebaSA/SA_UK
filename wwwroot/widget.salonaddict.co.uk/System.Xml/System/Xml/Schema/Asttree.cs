namespace System.Xml.Schema
{
    using MS.Internal.Xml.XPath;
    using System;
    using System.Collections;
    using System.Xml;
    using System.Xml.XPath;

    internal class Asttree
    {
        private ArrayList fAxisArray;
        private bool isField;
        private XmlNamespaceManager nsmgr;
        private string xpathexpr;

        public Asttree(string xPath, bool isField, XmlNamespaceManager nsmgr)
        {
            this.xpathexpr = xPath;
            this.isField = isField;
            this.nsmgr = nsmgr;
            this.CompileXPath(xPath, isField, nsmgr);
        }

        public void CompileXPath(string xPath, bool isField, XmlNamespaceManager nsmgr)
        {
            if ((xPath == null) || (xPath.Length == 0))
            {
                throw new XmlSchemaException("Sch_EmptyXPath", string.Empty);
            }
            string[] strArray = xPath.Split(new char[] { '|' });
            ArrayList list = new ArrayList(strArray.Length);
            this.fAxisArray = new ArrayList(strArray.Length);
            try
            {
                foreach (string str in strArray)
                {
                    Axis axis = (Axis) XPathParser.ParseXPathExpresion(str);
                    list.Add(axis);
                }
            }
            catch
            {
                throw new XmlSchemaException("Sch_ICXpathError", xPath);
            }
            foreach (Axis axis3 in list)
            {
                Axis ast = axis3;
                if (ast == null)
                {
                    throw new XmlSchemaException("Sch_ICXpathError", xPath);
                }
                Axis axis4 = ast;
                if (!IsAttribute(ast))
                {
                    goto Label_014D;
                }
                if (!isField)
                {
                    throw new XmlSchemaException("Sch_SelectorAttr", xPath);
                }
                this.SetURN(ast, nsmgr);
                try
                {
                    ast = (Axis) ast.Input;
                    goto Label_014D;
                }
                catch
                {
                    throw new XmlSchemaException("Sch_ICXpathError", xPath);
                }
            Label_00FB:
                if (IsSelf(ast) && (axis3 != ast))
                {
                    axis4.Input = ast.Input;
                }
                else
                {
                    axis4 = ast;
                    if (IsNameTest(ast))
                    {
                        this.SetURN(ast, nsmgr);
                    }
                }
                try
                {
                    ast = (Axis) ast.Input;
                }
                catch
                {
                    throw new XmlSchemaException("Sch_ICXpathError", xPath);
                }
            Label_014D:
                if ((ast != null) && (IsNameTest(ast) || IsSelf(ast)))
                {
                    goto Label_00FB;
                }
                axis4.Input = null;
                if (ast == null)
                {
                    if (IsSelf(axis3) && (axis3.Input != null))
                    {
                        this.fAxisArray.Add(new ForwardAxis(DoubleLinkAxis.ConvertTree((Axis) axis3.Input), false));
                    }
                    else
                    {
                        this.fAxisArray.Add(new ForwardAxis(DoubleLinkAxis.ConvertTree(axis3), false));
                    }
                }
                else
                {
                    if (!IsDescendantOrSelf(ast))
                    {
                        throw new XmlSchemaException("Sch_ICXpathError", xPath);
                    }
                    try
                    {
                        ast = (Axis) ast.Input;
                    }
                    catch
                    {
                        throw new XmlSchemaException("Sch_ICXpathError", xPath);
                    }
                    if (((ast == null) || !IsSelf(ast)) || (ast.Input != null))
                    {
                        throw new XmlSchemaException("Sch_ICXpathError", xPath);
                    }
                    if (IsSelf(axis3) && (axis3.Input != null))
                    {
                        this.fAxisArray.Add(new ForwardAxis(DoubleLinkAxis.ConvertTree((Axis) axis3.Input), true));
                    }
                    else
                    {
                        this.fAxisArray.Add(new ForwardAxis(DoubleLinkAxis.ConvertTree(axis3), true));
                    }
                }
            }
        }

        internal static bool IsAttribute(Axis ast) => 
            ((ast.TypeOfAxis == Axis.AxisType.Attribute) && (ast.NodeType == XPathNodeType.Attribute));

        private static bool IsDescendantOrSelf(Axis ast) => 
            (((ast.TypeOfAxis == Axis.AxisType.DescendantOrSelf) && (ast.NodeType == XPathNodeType.All)) && ast.AbbrAxis);

        private static bool IsNameTest(Axis ast) => 
            ((ast.TypeOfAxis == Axis.AxisType.Child) && (ast.NodeType == XPathNodeType.Element));

        internal static bool IsSelf(Axis ast) => 
            (((ast.TypeOfAxis == Axis.AxisType.Self) && (ast.NodeType == XPathNodeType.All)) && ast.AbbrAxis);

        private void SetURN(Axis axis, XmlNamespaceManager nsmgr)
        {
            if (axis.Prefix.Length != 0)
            {
                axis.Urn = nsmgr.LookupNamespace(axis.Prefix);
                if (axis.Urn == null)
                {
                    throw new XmlSchemaException("Sch_UnresolvedPrefix", axis.Prefix);
                }
            }
            else if (axis.Name.Length != 0)
            {
                axis.Urn = null;
            }
            else
            {
                axis.Urn = "";
            }
        }

        internal ArrayList SubtreeArray =>
            this.fAxisArray;
    }
}

