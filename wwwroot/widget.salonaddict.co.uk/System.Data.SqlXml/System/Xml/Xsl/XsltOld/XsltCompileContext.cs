namespace System.Xml.Xsl.XsltOld
{
    using MS.Internal.Xml.XPath;
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Runtime;

    internal class XsltCompileContext : XsltContext
    {
        private const BindingFlags bindingFlags = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        private const string f_NodeSet = "node-set";
        private InputScopeManager manager;
        private Processor processor;
        private static IXsltContextFunction s_FuncNodeSet = new FuncNodeSet();
        private static Hashtable s_FunctionTable = CreateFunctionTable();

        internal XsltCompileContext() : base(false)
        {
        }

        internal XsltCompileContext(InputScopeManager manager, Processor processor) : base(false)
        {
            this.manager = manager;
            this.processor = processor;
        }

        private static void AddKeyValue(Hashtable keyTable, string key, XPathNavigator value, bool checkDuplicates)
        {
            ArrayList list = (ArrayList) keyTable[key];
            if (list == null)
            {
                list = new ArrayList();
                keyTable.Add(key, list);
            }
            else if (checkDuplicates && (value.ComparePosition((XPathNavigator) list[list.Count - 1]) == XmlNodeOrder.Same))
            {
                return;
            }
            list.Add(value.Clone());
        }

        private Hashtable BuildKeyTable(Key key, XPathNavigator root)
        {
            Hashtable keyTable = new Hashtable();
            string queryExpression = this.processor.GetQueryExpression(key.MatchKey);
            Query compiledQuery = this.processor.GetCompiledQuery(key.MatchKey);
            Query useExpr = this.processor.GetCompiledQuery(key.UseKey);
            XPathNodeIterator iterator = root.SelectDescendants(XPathNodeType.All, false);
            while (iterator.MoveNext())
            {
                XPathNavigator current = iterator.Current;
                EvaluateKey(current, compiledQuery, queryExpression, useExpr, keyTable);
                if (current.MoveToFirstAttribute())
                {
                    do
                    {
                        EvaluateKey(current, compiledQuery, queryExpression, useExpr, keyTable);
                    }
                    while (current.MoveToNextAttribute());
                    current.MoveToParent();
                }
            }
            return keyTable;
        }

        public override int CompareDocument(string baseUri, string nextbaseUri) => 
            string.Compare(baseUri, nextbaseUri, StringComparison.Ordinal);

        private Uri ComposeUri(string thisUri, string baseUri)
        {
            XmlResolver resolver = this.processor.Resolver;
            Uri uri = null;
            if (baseUri.Length != 0)
            {
                uri = resolver.ResolveUri(null, baseUri);
            }
            return resolver.ResolveUri(uri, thisUri);
        }

        private static Hashtable CreateFunctionTable() => 
            new Hashtable(10) { 
                ["current"] = new FuncCurrent(),
                ["unparsed-entity-uri"] = new FuncUnEntityUri(),
                ["generate-id"] = new FuncGenerateId(),
                ["system-property"] = new FuncSystemProp(),
                ["element-available"] = new FuncElementAvailable(),
                ["function-available"] = new FuncFunctionAvailable(),
                ["document"] = new FuncDocument(),
                ["key"] = new FuncKey(),
                ["format-number"] = new FuncFormatNumber()
            };

        private XPathNodeIterator Current()
        {
            XPathNavigator current = this.processor.Current;
            if (current != null)
            {
                return new XPathSingletonIterator(current.Clone());
            }
            return XPathEmptyIterator.Instance;
        }

        private XPathNodeIterator Document(object arg0, string baseUri)
        {
            if (this.processor.permissions != null)
            {
                this.processor.permissions.PermitOnly();
            }
            XPathNodeIterator iterator = arg0 as XPathNodeIterator;
            if (iterator != null)
            {
                ArrayList list = new ArrayList();
                Hashtable hashtable = new Hashtable();
                while (iterator.MoveNext())
                {
                    Uri key = this.ComposeUri(iterator.Current.Value, baseUri ?? iterator.Current.BaseURI);
                    if (!hashtable.ContainsKey(key))
                    {
                        hashtable.Add(key, null);
                        list.Add(this.processor.GetNavigator(key));
                    }
                }
                return new XPathArrayIterator(list);
            }
            return new XPathSingletonIterator(this.processor.GetNavigator(this.ComposeUri(XmlConvert.ToXPathString(arg0), baseUri ?? this.manager.Navigator.BaseURI)));
        }

        private bool ElementAvailable(string qname)
        {
            string str;
            string str2;
            PrefixQName.ParseQualifiedName(qname, out str2, out str);
            if (this.manager.ResolveXmlNamespace(str2) != "http://www.w3.org/1999/XSL/Transform")
            {
                return false;
            }
            if ((((((str != "apply-imports") && (str != "apply-templates")) && ((str != "attribute") && (str != "call-template"))) && (((str != "choose") && (str != "comment")) && ((str != "copy") && (str != "copy-of")))) && ((((str != "element") && (str != "fallback")) && ((str != "for-each") && (str != "if"))) && (((str != "message") && (str != "number")) && ((str != "processing-instruction") && (str != "text"))))) && (str != "value-of"))
            {
                return (str == "variable");
            }
            return true;
        }

        private static void EvaluateKey(XPathNavigator node, Query matchExpr, string matchStr, Query useExpr, Hashtable keyTable)
        {
            try
            {
                if (matchExpr.MatchNode(node) == null)
                {
                    return;
                }
            }
            catch (XPathException)
            {
                throw XsltException.Create("Xslt_InvalidPattern", new string[] { matchStr });
            }
            object obj2 = useExpr.Evaluate(new XPathSingletonIterator(node, true));
            XPathNodeIterator iterator = obj2 as XPathNodeIterator;
            if (iterator != null)
            {
                for (bool flag = false; iterator.MoveNext(); flag = true)
                {
                    AddKeyValue(keyTable, iterator.Current.Value, node, flag);
                }
            }
            else
            {
                string key = XmlConvert.ToXPathString(obj2);
                AddKeyValue(keyTable, key, node, false);
            }
        }

        internal object EvaluateVariable(VariableAction variable)
        {
            object variableValue = this.processor.GetVariableValue(variable);
            if ((variableValue == null) && !variable.IsGlobal)
            {
                VariableAction action = this.manager.VariableScope.ResolveGlobalVariable(variable.Name);
                if (action != null)
                {
                    variableValue = this.processor.GetVariableValue(action);
                }
            }
            if (variableValue == null)
            {
                throw XsltException.Create("Xslt_InvalidVariable", new string[] { variable.Name.ToString() });
            }
            return variableValue;
        }

        private MethodInfo FindBestMethod(MethodInfo[] methods, bool ignoreCase, bool publicOnly, string name, XPathResultType[] argTypes)
        {
            int length = methods.Length;
            int num2 = 0;
            for (int i = 0; i < length; i++)
            {
                if ((string.Compare(name, methods[i].Name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0) && (!publicOnly || methods[i].GetBaseDefinition().IsPublic))
                {
                    methods[num2++] = methods[i];
                }
            }
            length = num2;
            if (length == 0)
            {
                return null;
            }
            if (argTypes != null)
            {
                num2 = 0;
                for (int j = 0; j < length; j++)
                {
                    if (methods[j].GetParameters().Length == argTypes.Length)
                    {
                        methods[num2++] = methods[j];
                    }
                }
                length = num2;
                if (length <= 1)
                {
                    return methods[0];
                }
                num2 = 0;
                for (int k = 0; k < length; k++)
                {
                    bool flag = true;
                    ParameterInfo[] parameters = methods[k].GetParameters();
                    for (int m = 0; m < parameters.Length; m++)
                    {
                        XPathResultType type = argTypes[m];
                        if (type != XPathResultType.Any)
                        {
                            XPathResultType xPathType = GetXPathType(parameters[m].ParameterType);
                            if ((xPathType != type) && (xPathType != XPathResultType.Any))
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        methods[num2++] = methods[k];
                    }
                }
                length = num2;
            }
            return methods[0];
        }

        private bool FunctionAvailable(string qname)
        {
            string str;
            string str2;
            PrefixQName.ParseQualifiedName(qname, out str2, out str);
            string ns = this.LookupNamespace(str2);
            if (ns == "urn:schemas-microsoft-com:xslt")
            {
                return (str == "node-set");
            }
            if (ns.Length != 0)
            {
                object obj2;
                return (this.GetExtentionMethod(ns, str, null, out obj2) != null);
            }
            return (((((((str == "last") || (str == "position")) || ((str == "name") || (str == "namespace-uri"))) || (((str == "local-name") || (str == "count")) || ((str == "id") || (str == "string")))) || ((((str == "concat") || (str == "starts-with")) || ((str == "contains") || (str == "substring-before"))) || (((str == "substring-after") || (str == "substring")) || ((str == "string-length") || (str == "normalize-space"))))) || (((((str == "translate") || (str == "boolean")) || ((str == "not") || (str == "true"))) || (((str == "false") || (str == "lang")) || ((str == "number") || (str == "sum")))) || (((str == "floor") || (str == "ceiling")) || (str == "round")))) || ((s_FunctionTable[str] != null) && (str != "unparsed-entity-uri")));
        }

        private IXsltContextFunction GetExtentionMethod(string ns, string name, XPathResultType[] argTypes, out object extension)
        {
            FuncExtension extension2 = null;
            extension = this.processor.GetScriptObject(ns);
            if (extension != null)
            {
                MethodInfo info = this.FindBestMethod(extension.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance), true, false, name, argTypes);
                if (info != null)
                {
                    extension2 = new FuncExtension(extension, info, null);
                }
                return extension2;
            }
            extension = this.processor.GetExtensionObject(ns);
            if (extension == null)
            {
                return null;
            }
            MethodInfo method = this.FindBestMethod(extension.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance), false, true, name, argTypes);
            if (method != null)
            {
                extension2 = new FuncExtension(extension, method, this.processor.permissions);
            }
            return extension2;
        }

        public static XPathResultType GetXPathType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                    if (!typeof(XPathNavigator).IsAssignableFrom(type) && !typeof(IXPathNavigable).IsAssignableFrom(type))
                    {
                        if (typeof(XPathNodeIterator).IsAssignableFrom(type))
                        {
                            return XPathResultType.NodeSet;
                        }
                        return XPathResultType.Any;
                    }
                    return XPathResultType.String;

                case TypeCode.Boolean:
                    return XPathResultType.Boolean;

                case TypeCode.DateTime:
                    return XPathResultType.Error;

                case TypeCode.String:
                    return XPathResultType.String;
            }
            return XPathResultType.Number;
        }

        public override string LookupNamespace(string prefix) => 
            this.manager.ResolveXPathNamespace(prefix);

        public override bool PreserveWhitespace(XPathNavigator node)
        {
            node = node.Clone();
            node.MoveToParent();
            return this.processor.Stylesheet.PreserveWhiteSpace(this.processor, node);
        }

        internal void Recycle()
        {
            this.manager = null;
            this.processor = null;
        }

        internal void Reinitialize(InputScopeManager manager, Processor processor)
        {
            this.manager = manager;
            this.processor = processor;
        }

        private DecimalFormat ResolveFormatName(string formatName)
        {
            string ns = string.Empty;
            string local = string.Empty;
            if (formatName != null)
            {
                string str3;
                PrefixQName.ParseQualifiedName(formatName, out str3, out local);
                ns = this.LookupNamespace(str3);
            }
            DecimalFormat decimalFormat = this.processor.RootAction.GetDecimalFormat(new XmlQualifiedName(local, ns));
            if (decimalFormat != null)
            {
                return decimalFormat;
            }
            if (formatName != null)
            {
                throw XsltException.Create("Xslt_NoDecimalFormat", new string[] { formatName });
            }
            return new DecimalFormat(new NumberFormatInfo(), '#', '0', ';');
        }

        public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
        {
            IXsltContextFunction function = null;
            if (prefix.Length == 0)
            {
                function = s_FunctionTable[name] as IXsltContextFunction;
            }
            else
            {
                string ns = this.LookupNamespace(prefix);
                if ((ns == "urn:schemas-microsoft-com:xslt") && (name == "node-set"))
                {
                    function = s_FuncNodeSet;
                }
                else
                {
                    object obj2;
                    function = this.GetExtentionMethod(ns, name, argTypes, out obj2);
                    if (obj2 == null)
                    {
                        throw XsltException.Create("Xslt_ScriptInvalidPrefix", new string[] { prefix });
                    }
                }
            }
            if (function == null)
            {
                throw XsltException.Create("Xslt_UnknownXsltFunction", new string[] { name });
            }
            if ((argTypes.Length >= function.Minargs) && (function.Maxargs >= argTypes.Length))
            {
                return function;
            }
            string[] args = new string[] { name, argTypes.Length.ToString(CultureInfo.InvariantCulture) };
            throw XsltException.Create("Xslt_WrongNumberArgs", args);
        }

        public override IXsltContextVariable ResolveVariable(string prefix, string name)
        {
            string ns = this.LookupNamespace(prefix);
            XmlQualifiedName qname = new XmlQualifiedName(name, ns);
            IXsltContextVariable variable = this.manager.VariableScope.ResolveVariable(qname);
            if (variable == null)
            {
                throw XsltException.Create("Xslt_InvalidVariable", new string[] { qname.ToString() });
            }
            return variable;
        }

        private string SystemProperty(string qname)
        {
            string str2;
            string str3;
            string str = string.Empty;
            PrefixQName.ParseQualifiedName(qname, out str2, out str3);
            string str4 = this.LookupNamespace(str2);
            if (str4 == "http://www.w3.org/1999/XSL/Transform")
            {
                switch (str3)
                {
                    case "version":
                        return "1";

                    case "vendor":
                        return "Microsoft";

                    case "vendor-url":
                        str = "http://www.microsoft.com";
                        break;
                }
                return str;
            }
            if ((str4 == null) && (str2 != null))
            {
                throw XsltException.Create("Xslt_InvalidPrefix", new string[] { str2 });
            }
            return string.Empty;
        }

        public override string DefaultNamespace =>
            string.Empty;

        public override bool Whitespace =>
            this.processor.Stylesheet.Whitespace;

        private class FuncCurrent : XsltCompileContext.XsltFunctionImpl
        {
            public FuncCurrent() : base(0, 0, XPathResultType.NodeSet, new XPathResultType[0])
            {
            }

            public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext) => 
                ((XsltCompileContext) xsltContext).Current();
        }

        private class FuncDocument : XsltCompileContext.XsltFunctionImpl
        {
            public FuncDocument() : base(1, 2, XPathResultType.NodeSet, new XPathResultType[] { XPathResultType.Any, XPathResultType.NodeSet })
            {
            }

            public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
            {
                string baseUri = null;
                if (args.Length == 2)
                {
                    XPathNodeIterator iterator = XsltCompileContext.XsltFunctionImpl.ToIterator(args[1]);
                    if (iterator.MoveNext())
                    {
                        baseUri = iterator.Current.BaseURI;
                    }
                    else
                    {
                        baseUri = string.Empty;
                    }
                }
                try
                {
                    return ((XsltCompileContext) xsltContext).Document(args[0], baseUri);
                }
                catch (Exception exception)
                {
                    if (!XmlException.IsCatchableException(exception))
                    {
                        throw;
                    }
                    return XPathEmptyIterator.Instance;
                }
            }
        }

        private class FuncElementAvailable : XsltCompileContext.XsltFunctionImpl
        {
            public FuncElementAvailable() : base(1, 1, XPathResultType.Boolean, new XPathResultType[] { XPathResultType.String })
            {
            }

            public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext) => 
                ((XsltCompileContext) xsltContext).ElementAvailable(XsltCompileContext.XsltFunctionImpl.ToString(args[0]));
        }

        private class FuncExtension : XsltCompileContext.XsltFunctionImpl
        {
            private object extension;
            private MethodInfo method;
            private PermissionSet permissions;
            private TypeCode[] typeCodes;

            public FuncExtension(object extension, MethodInfo method, PermissionSet permissions)
            {
                this.extension = extension;
                this.method = method;
                this.permissions = permissions;
                XPathResultType xPathType = XsltCompileContext.GetXPathType(method.ReturnType);
                ParameterInfo[] parameters = method.GetParameters();
                int length = parameters.Length;
                int maxArgs = parameters.Length;
                this.typeCodes = new TypeCode[parameters.Length];
                XPathResultType[] argTypes = new XPathResultType[parameters.Length];
                bool flag = true;
                for (int i = parameters.Length - 1; 0 <= i; i--)
                {
                    this.typeCodes[i] = Type.GetTypeCode(parameters[i].ParameterType);
                    argTypes[i] = XsltCompileContext.GetXPathType(parameters[i].ParameterType);
                    if (flag)
                    {
                        if (parameters[i].IsOptional)
                        {
                            length--;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                }
                base.Init(length, maxArgs, xPathType, argTypes);
            }

            public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
            {
                for (int i = args.Length - 1; 0 <= i; i--)
                {
                    args[i] = XsltCompileContext.XsltFunctionImpl.ConvertToXPathType(args[i], base.ArgTypes[i], this.typeCodes[i]);
                }
                if (this.permissions != null)
                {
                    this.permissions.PermitOnly();
                }
                return this.method.Invoke(this.extension, args);
            }
        }

        private class FuncFormatNumber : XsltCompileContext.XsltFunctionImpl
        {
            public FuncFormatNumber() : base(2, 3, XPathResultType.String, typeArray)
            {
                XPathResultType[] typeArray = new XPathResultType[3];
                typeArray[1] = XPathResultType.String;
                typeArray[2] = XPathResultType.String;
            }

            public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
            {
                DecimalFormat decimalFormat = ((XsltCompileContext) xsltContext).ResolveFormatName((args.Length == 3) ? XsltCompileContext.XsltFunctionImpl.ToString(args[2]) : null);
                return DecimalFormatter.Format(XsltCompileContext.XsltFunctionImpl.ToNumber(args[0]), XsltCompileContext.XsltFunctionImpl.ToString(args[1]), decimalFormat);
            }
        }

        private class FuncFunctionAvailable : XsltCompileContext.XsltFunctionImpl
        {
            public FuncFunctionAvailable() : base(1, 1, XPathResultType.Boolean, new XPathResultType[] { XPathResultType.String })
            {
            }

            public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext) => 
                ((XsltCompileContext) xsltContext).FunctionAvailable(XsltCompileContext.XsltFunctionImpl.ToString(args[0]));
        }

        private class FuncGenerateId : XsltCompileContext.XsltFunctionImpl
        {
            public FuncGenerateId() : base(0, 1, XPathResultType.String, new XPathResultType[] { XPathResultType.NodeSet })
            {
            }

            public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
            {
                if (args.Length <= 0)
                {
                    return docContext.UniqueId;
                }
                XPathNodeIterator iterator = XsltCompileContext.XsltFunctionImpl.ToIterator(args[0]);
                if (iterator.MoveNext())
                {
                    return iterator.Current.UniqueId;
                }
                return string.Empty;
            }
        }

        private class FuncKey : XsltCompileContext.XsltFunctionImpl
        {
            public FuncKey() : base(2, 2, XPathResultType.NodeSet, new XPathResultType[] { XPathResultType.String, XPathResultType.Any })
            {
            }

            private static ArrayList AddToList(ArrayList resultCollection, ArrayList newList)
            {
                if (newList != null)
                {
                    if (resultCollection == null)
                    {
                        return newList;
                    }
                    if (!(resultCollection[0] is ArrayList))
                    {
                        ArrayList list = resultCollection;
                        resultCollection = new ArrayList();
                        resultCollection.Add(list);
                    }
                    resultCollection.Add(newList);
                }
                return resultCollection;
            }

            public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
            {
                string str;
                string str2;
                XsltCompileContext context = (XsltCompileContext) xsltContext;
                PrefixQName.ParseQualifiedName(XsltCompileContext.XsltFunctionImpl.ToString(args[0]), out str2, out str);
                string ns = xsltContext.LookupNamespace(str2);
                XmlQualifiedName name = new XmlQualifiedName(str, ns);
                XPathNavigator root = docContext.Clone();
                root.MoveToRoot();
                ArrayList resultCollection = null;
                foreach (Key key in context.processor.KeyList)
                {
                    if (key.Name == name)
                    {
                        Hashtable keys = key.GetKeys(root);
                        if (keys == null)
                        {
                            keys = context.BuildKeyTable(key, root);
                            key.AddKey(root, keys);
                        }
                        XPathNodeIterator iterator = args[1] as XPathNodeIterator;
                        if (iterator != null)
                        {
                            iterator = iterator.Clone();
                            while (iterator.MoveNext())
                            {
                                resultCollection = AddToList(resultCollection, (ArrayList) keys[iterator.Current.Value]);
                            }
                        }
                        else
                        {
                            resultCollection = AddToList(resultCollection, (ArrayList) keys[XsltCompileContext.XsltFunctionImpl.ToString(args[1])]);
                        }
                    }
                }
                if (resultCollection == null)
                {
                    return XPathEmptyIterator.Instance;
                }
                if (resultCollection[0] is XPathNavigator)
                {
                    return new XPathArrayIterator(resultCollection);
                }
                return new XPathMultyIterator(resultCollection);
            }
        }

        private class FuncNodeSet : XsltCompileContext.XsltFunctionImpl
        {
            public FuncNodeSet() : base(1, 1, XPathResultType.NodeSet, new XPathResultType[] { XPathResultType.String })
            {
            }

            public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext) => 
                new XPathSingletonIterator(XsltCompileContext.XsltFunctionImpl.ToNavigator(args[0]));
        }

        private class FuncSystemProp : XsltCompileContext.XsltFunctionImpl
        {
            public FuncSystemProp() : base(1, 1, XPathResultType.String, new XPathResultType[] { XPathResultType.String })
            {
            }

            public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext) => 
                ((XsltCompileContext) xsltContext).SystemProperty(XsltCompileContext.XsltFunctionImpl.ToString(args[0]));
        }

        private class FuncUnEntityUri : XsltCompileContext.XsltFunctionImpl
        {
            public FuncUnEntityUri() : base(1, 1, XPathResultType.String, new XPathResultType[] { XPathResultType.String })
            {
            }

            public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
            {
                throw XsltException.Create("Xslt_UnsuppFunction", new string[] { "unparsed-entity-uri" });
            }
        }

        private abstract class XsltFunctionImpl : IXsltContextFunction
        {
            private XPathResultType[] argTypes;
            private int maxargs;
            private int minargs;
            private XPathResultType returnType;

            public XsltFunctionImpl()
            {
            }

            public XsltFunctionImpl(int minArgs, int maxArgs, XPathResultType returnType, XPathResultType[] argTypes)
            {
                this.Init(minArgs, maxArgs, returnType, argTypes);
            }

            public static object ConvertToXPathType(object val, XPathResultType xt, TypeCode typeCode)
            {
                switch (xt)
                {
                    case XPathResultType.Number:
                        return ToNumeric(val, typeCode);

                    case XPathResultType.String:
                        if (typeCode != TypeCode.String)
                        {
                            return ToNavigator(val);
                        }
                        return ToString(val);

                    case XPathResultType.Boolean:
                        return ToBoolean(val);

                    case XPathResultType.NodeSet:
                        return ToIterator(val);

                    case ((XPathResultType) 4):
                        return val;

                    case XPathResultType.Any:
                    case XPathResultType.Error:
                        return val;
                }
                return val;
            }

            protected void Init(int minArgs, int maxArgs, XPathResultType returnType, XPathResultType[] argTypes)
            {
                this.minargs = minArgs;
                this.maxargs = maxArgs;
                this.returnType = returnType;
                this.argTypes = argTypes;
            }

            public abstract object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext);
            private static string IteratorToString(XPathNodeIterator it)
            {
                if (it.MoveNext())
                {
                    return it.Current.Value;
                }
                return string.Empty;
            }

            public static bool ToBoolean(object argument)
            {
                XPathNodeIterator it = argument as XPathNodeIterator;
                if (it != null)
                {
                    return Convert.ToBoolean(IteratorToString(it), CultureInfo.InvariantCulture);
                }
                XPathNavigator navigator = argument as XPathNavigator;
                if (navigator != null)
                {
                    return Convert.ToBoolean(navigator.ToString(), CultureInfo.InvariantCulture);
                }
                return Convert.ToBoolean(argument, CultureInfo.InvariantCulture);
            }

            public static XPathNodeIterator ToIterator(object argument)
            {
                XPathNodeIterator iterator = argument as XPathNodeIterator;
                if (iterator == null)
                {
                    throw XsltException.Create("Xslt_NoNodeSetConversion", new string[0]);
                }
                return iterator;
            }

            public static XPathNavigator ToNavigator(object argument)
            {
                XPathNavigator navigator = argument as XPathNavigator;
                if (navigator == null)
                {
                    throw XsltException.Create("Xslt_NoNavigatorConversion", new string[0]);
                }
                return navigator;
            }

            public static double ToNumber(object argument)
            {
                XPathNodeIterator it = argument as XPathNodeIterator;
                if (it != null)
                {
                    return XmlConvert.ToXPathDouble(IteratorToString(it));
                }
                XPathNavigator navigator = argument as XPathNavigator;
                if (navigator != null)
                {
                    return XmlConvert.ToXPathDouble(navigator.ToString());
                }
                return XmlConvert.ToXPathDouble(argument);
            }

            private static object ToNumeric(object argument, TypeCode typeCode) => 
                Convert.ChangeType(ToNumber(argument), typeCode, CultureInfo.InvariantCulture);

            public static string ToString(object argument)
            {
                XPathNodeIterator it = argument as XPathNodeIterator;
                if (it != null)
                {
                    return IteratorToString(it);
                }
                return XmlConvert.ToXPathString(argument);
            }

            public XPathResultType[] ArgTypes =>
                this.argTypes;

            public int Maxargs =>
                this.maxargs;

            public int Minargs =>
                this.minargs;

            public XPathResultType ReturnType =>
                this.returnType;
        }
    }
}

