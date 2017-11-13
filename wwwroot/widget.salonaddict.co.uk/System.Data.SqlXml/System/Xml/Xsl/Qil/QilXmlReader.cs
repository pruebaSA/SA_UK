namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Xsl;

    internal sealed class QilXmlReader
    {
        private QilFactory f;
        private Dictionary<string, QilNode> fwdDecls;
        private bool inFwdDecls;
        private static Regex lineInfoRegex = new Regex(@"\[(\d+),(\d+) -- (\d+),(\d+)\]");
        private static Dictionary<string, MethodInfo> nameToFactoryMethod = new Dictionary<string, MethodInfo>();
        private XmlReader r;
        private Dictionary<string, QilNode> scope;
        private Stack<QilList> stk;
        private static Regex typeInfoRegex = new Regex(@"(\w+);([\w|\|]+);(\w+)");

        static QilXmlReader()
        {
            foreach (MethodInfo info in typeof(QilFactory).GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] parameters = info.GetParameters();
                int index = 0;
                while (index < parameters.Length)
                {
                    if (parameters[index].ParameterType != typeof(QilNode))
                    {
                        break;
                    }
                    index++;
                }
                if ((index == parameters.Length) && (!nameToFactoryMethod.ContainsKey(info.Name) || (nameToFactoryMethod[info.Name].GetParameters().Length < parameters.Length)))
                {
                    nameToFactoryMethod[info.Name] = info;
                }
            }
        }

        public QilXmlReader(XmlReader r)
        {
            this.r = r;
            this.f = new QilFactory();
        }

        private void EndElement()
        {
            MethodInfo info = null;
            QilNode node;
            QilExpression expression;
            int num;
            string id;
            QilName name;
            QilList values = this.stk.Pop();
            ReaderAnnotation annotation = (ReaderAnnotation) values.Annotation;
            string localName = this.r.LocalName;
            switch (this.r.LocalName)
            {
                case "QilExpression":
                    expression = this.f.QilExpression(values[values.Count - 1]);
                    num = 0;
                    goto Label_0237;

                case "ForwardDecls":
                    this.inFwdDecls = false;
                    return;

                case "Parameter":
                case "Let":
                case "For":
                case "Function":
                    id = annotation.Id;
                    name = annotation.Name;
                    switch (this.r.LocalName)
                    {
                        case "Parameter":
                            if (this.inFwdDecls || (values.Count == 0))
                            {
                                node = this.f.Parameter(null, name, annotation.XmlType);
                            }
                            else
                            {
                                node = this.f.Parameter(values[0], name, annotation.XmlType);
                            }
                            goto Label_03C1;

                        case "Let":
                            if (this.inFwdDecls)
                            {
                                node = this.f.Let(this.f.Unknown(annotation.XmlType));
                            }
                            else
                            {
                                node = this.f.Let(values[0]);
                            }
                            goto Label_03C1;

                        case "For":
                            node = this.f.For(values[0]);
                            goto Label_03C1;
                    }
                    if (this.inFwdDecls)
                    {
                        node = this.f.Function(values[0], values[1], annotation.XmlType);
                    }
                    else
                    {
                        node = this.f.Function(values[0], values[1], values[2], (annotation.XmlType != null) ? annotation.XmlType : values[1].XmlType);
                    }
                    goto Label_03C1;

                case "RefTo":
                {
                    string str2 = annotation.Id;
                    this.stk.Peek().Add(this.scope[str2]);
                    return;
                }
                case "Sequence":
                    node = this.f.Sequence(values);
                    goto Label_0605;

                case "FunctionList":
                    node = this.f.FunctionList(values);
                    goto Label_0605;

                case "GlobalVariableList":
                    node = this.f.GlobalVariableList(values);
                    goto Label_0605;

                case "GlobalParameterList":
                    node = this.f.GlobalParameterList(values);
                    goto Label_0605;

                case "ActualParameterList":
                    node = this.f.ActualParameterList(values);
                    goto Label_0605;

                case "FormalParameterList":
                    node = this.f.FormalParameterList(values);
                    goto Label_0605;

                case "SortKeyList":
                    node = this.f.SortKeyList(values);
                    goto Label_0605;

                case "BranchList":
                    node = this.f.BranchList(values);
                    goto Label_0605;

                case "XsltInvokeEarlyBound":
                {
                    MethodInfo method = null;
                    QilName name2 = (QilName) values[0];
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        Type type = assembly.GetType(annotation.ClrNamespace);
                        if (type != null)
                        {
                            method = type.GetMethod(name2.LocalName);
                            break;
                        }
                    }
                    node = this.f.XsltInvokeEarlyBound(name2, this.f.LiteralObject(method), values[1], annotation.XmlType);
                    goto Label_0605;
                }
                default:
                {
                    info = nameToFactoryMethod[this.r.LocalName];
                    object[] parameters = new object[values.Count];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        parameters[i] = values[i];
                    }
                    node = (QilNode) info.Invoke(this.f, parameters);
                    goto Label_0605;
                }
            }
        Label_0231:
            num++;
        Label_0237:
            if (num < (values.Count - 1))
            {
                switch (values[num].NodeType)
                {
                    case QilNodeType.FunctionList:
                        expression.FunctionList = (QilList) values[num];
                        break;

                    case QilNodeType.GlobalVariableList:
                        expression.GlobalVariableList = (QilList) values[num];
                        break;

                    case QilNodeType.GlobalParameterList:
                        expression.GlobalParameterList = (QilList) values[num];
                        break;

                    case QilNodeType.True:
                    case QilNodeType.False:
                        expression.IsDebug = values[num].NodeType == QilNodeType.True;
                        break;
                }
                goto Label_0231;
            }
            node = expression;
            goto Label_0605;
        Label_03C1:
            if (name != null)
            {
                ((QilReference) node).DebugName = name.ToString();
            }
            if (this.inFwdDecls)
            {
                this.fwdDecls[id] = node;
                this.scope[id] = node;
            }
            else if (this.fwdDecls.ContainsKey(id))
            {
                node = this.fwdDecls[id];
                this.fwdDecls.Remove(id);
                if (values.Count > 0)
                {
                    node[0] = values[0];
                }
                if (values.Count > 1)
                {
                    node[1] = values[1];
                }
            }
            else
            {
                this.scope[id] = node;
            }
            node.Annotation = annotation;
        Label_0605:
            node.SourceLine = values.SourceLine;
            this.stk.Peek().Add(node);
        }

        private ISourceLineInfo ParseLineInfo(string s)
        {
            if ((s != null) && (s.Length > 0))
            {
                Match match = lineInfoRegex.Match(s);
                return new SourceLineInfo("", int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture), int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture), int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture));
            }
            return null;
        }

        private QilName ParseName(string name)
        {
            string str;
            string str2;
            string str3;
            if ((name == null) || (name.Length <= 0))
            {
                return null;
            }
            int num = name.LastIndexOf('}');
            if ((num != -1) && (name[0] == '{'))
            {
                str3 = name.Substring(1, num - 1);
                name = name.Substring(num + 1);
            }
            else
            {
                str3 = string.Empty;
            }
            ValidateNames.ParseQNameThrow(name, out str, out str2);
            return this.f.LiteralQName(str2, str3, str);
        }

        private XmlQueryType ParseType(string s)
        {
            if ((s == null) || (s.Length <= 0))
            {
                return null;
            }
            Match match = typeInfoRegex.Match(s);
            XmlQueryCardinality c = new XmlQueryCardinality(match.Groups[1].Value);
            bool isStrict = bool.Parse(match.Groups[3].Value);
            string[] strArray = match.Groups[2].Value.Split(new char[] { '|' });
            XmlQueryType[] types = new XmlQueryType[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                types[i] = XmlQueryTypeFactory.Type((XmlTypeCode) Enum.Parse(typeof(XmlTypeCode), strArray[i]), isStrict);
            }
            return XmlQueryTypeFactory.Product(XmlQueryTypeFactory.Choice(types), c);
        }

        public QilExpression Read()
        {
            this.stk = new Stack<QilList>();
            this.inFwdDecls = false;
            this.scope = new Dictionary<string, QilNode>();
            this.fwdDecls = new Dictionary<string, QilNode>();
            this.stk.Push(this.f.Sequence());
            while (this.r.Read())
            {
                switch (this.r.NodeType)
                {
                    case XmlNodeType.EndElement:
                        this.EndElement();
                        break;

                    case XmlNodeType.Element:
                    {
                        bool isEmptyElement = this.r.IsEmptyElement;
                        if (this.StartElement() && isEmptyElement)
                        {
                            this.EndElement();
                        }
                        break;
                    }
                }
            }
            return (QilExpression) this.stk.Peek()[0];
        }

        private string ReadText()
        {
            string str = string.Empty;
            if (!this.r.IsEmptyElement)
            {
                while (this.r.Read())
                {
                    switch (this.r.NodeType)
                    {
                        case XmlNodeType.Whitespace:
                        case XmlNodeType.SignificantWhitespace:
                        case XmlNodeType.Text:
                            break;

                        default:
                            return str;
                    }
                    str = str + this.r.Value;
                }
            }
            return str;
        }

        private bool StartElement()
        {
            QilNode node;
            ReaderAnnotation annotation = new ReaderAnnotation();
            string localName = this.r.LocalName;
            switch (this.r.LocalName)
            {
                case "LiteralString":
                    node = this.f.LiteralString(this.ReadText());
                    goto Label_026F;

                case "LiteralInt32":
                    node = this.f.LiteralInt32(int.Parse(this.ReadText(), CultureInfo.InvariantCulture));
                    goto Label_026F;

                case "LiteralInt64":
                    node = this.f.LiteralInt64(long.Parse(this.ReadText(), CultureInfo.InvariantCulture));
                    goto Label_026F;

                case "LiteralDouble":
                    node = this.f.LiteralDouble(double.Parse(this.ReadText(), CultureInfo.InvariantCulture));
                    goto Label_026F;

                case "LiteralDecimal":
                    node = this.f.LiteralDecimal(decimal.Parse(this.ReadText(), CultureInfo.InvariantCulture));
                    goto Label_026F;

                case "LiteralType":
                    node = this.f.LiteralType(this.ParseType(this.ReadText()));
                    goto Label_026F;

                case "LiteralQName":
                    node = this.ParseName(this.r.GetAttribute("name"));
                    goto Label_026F;

                case "For":
                case "Let":
                case "Parameter":
                case "Function":
                case "RefTo":
                    annotation.Id = this.r.GetAttribute("id");
                    annotation.Name = this.ParseName(this.r.GetAttribute("name"));
                    break;

                case "XsltInvokeEarlyBound":
                    annotation.ClrNamespace = this.r.GetAttribute("clrNamespace");
                    break;

                case "ForwardDecls":
                    this.inFwdDecls = true;
                    break;
            }
            node = this.f.Sequence();
        Label_026F:
            annotation.XmlType = this.ParseType(this.r.GetAttribute("xmlType"));
            node.SourceLine = this.ParseLineInfo(this.r.GetAttribute("lineInfo"));
            node.Annotation = annotation;
            if (node is QilList)
            {
                this.stk.Push((QilList) node);
                return true;
            }
            this.stk.Peek().Add(node);
            return false;
        }

        private class ReaderAnnotation
        {
            public string ClrNamespace;
            public string Id;
            public QilName Name;
            public XmlQueryType XmlType;
        }
    }
}

