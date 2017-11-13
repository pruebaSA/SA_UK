namespace System.Web.Script.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;

    internal abstract class ClientProxyGenerator
    {
        protected StringBuilder _builder;
        protected bool _debugMode;
        private Dictionary<string, string> _docCommentCache;
        private Hashtable _registeredNamespaces = new Hashtable();
        private static string DebugXmlComments = "/// <param name=\"succeededCallback\" type=\"Function\" optional=\"true\" mayBeNull=\"true\"></param>\r\n/// <param name=\"failedCallback\" type=\"Function\" optional=\"true\" mayBeNull=\"true\"></param>\r\n/// <param name=\"userContext\" optional=\"true\" mayBeNull=\"true\"></param>\r\n";

        protected ClientProxyGenerator()
        {
        }

        private void AppendClientTypeDeclaration(string ns, string typeName, bool genClass, bool ensureNS)
        {
            if (!string.IsNullOrEmpty(ns))
            {
                if (ensureNS)
                {
                    this.EnsureNamespace(ns);
                }
            }
            else if (!genClass)
            {
                this._builder.Append("var ");
            }
            this._builder.Append(this.GetClientTypeNamespace(ServicesUtilities.GetClientTypeName(typeName)));
            if (genClass)
            {
                this._builder.Append(".prototype");
            }
            this._builder.Append('=');
        }

        private void BuildArgsDictionary(WebServiceMethodData methodData, StringBuilder args, StringBuilder argsDict, StringBuilder docComments)
        {
            argsDict.Append('{');
            foreach (WebServiceParameterData data in methodData.ParameterDatas)
            {
                string parameterName = data.ParameterName;
                if (docComments != null)
                {
                    docComments.Append("/// <param name=\"").Append(parameterName).Append("\"");
                    Type type = ServicesUtilities.UnwrapNullableType(data.ParameterType);
                    string clientTypeNamespace = this.GetClientTypeNamespace(ServicesUtilities.GetClientTypeFromServerType(methodData.Owner, type));
                    if (!string.IsNullOrEmpty(clientTypeNamespace))
                    {
                        docComments.Append(" type=\"").Append(clientTypeNamespace).Append("\"");
                    }
                    docComments.Append(">").Append(type.FullName).Append("</param>\r\n");
                }
                if (args.Length > 0)
                {
                    args.Append(',');
                    argsDict.Append(',');
                }
                args.Append(parameterName);
                argsDict.Append(parameterName).Append(':').Append(parameterName);
            }
            if (docComments != null)
            {
                docComments.Append(DebugXmlComments);
            }
            argsDict.Append("}");
            if (args.Length > 0)
            {
                args.Append(',');
            }
            args.Append("succeededCallback, failedCallback, userContext");
        }

        private void EnsureNamespace(string ns)
        {
            ns = this.GetClientTypeNamespace(ns);
            if (!string.IsNullOrEmpty(ns) && !this._registeredNamespaces.Contains(ns))
            {
                this._builder.Append("Type.registerNamespace('").Append(ns).Append("');\r\n");
                this._registeredNamespaces[ns] = null;
            }
        }

        private void GenerateClientTypeProxies(WebServiceData data)
        {
            bool flag = true;
            foreach (WebServiceTypeData data2 in data.ClientTypes)
            {
                if (flag)
                {
                    this._builder.Append("var gtc = Sys.Net.WebServiceProxy._generateTypedConstructor;\r\n");
                    flag = false;
                }
                string typeStringRepresentation = data.GetTypeStringRepresentation(data2);
                string clientTypeNamespace = this.GetClientTypeNamespace(data2.TypeName);
                string clientTypeName = ServicesUtilities.GetClientTypeName(clientTypeNamespace);
                string ns = this.GetClientTypeNamespace(data2.TypeNamespace);
                this.EnsureNamespace(data2.TypeNamespace);
                this._builder.Append("if (typeof(").Append(clientTypeName).Append(") === 'undefined') {\r\n");
                this.AppendClientTypeDeclaration(ns, clientTypeNamespace, false, false);
                this._builder.Append("gtc(\"");
                this._builder.Append(typeStringRepresentation);
                this._builder.Append("\");\r\n");
                this._builder.Append(clientTypeName).Append(".registerClass('").Append(clientTypeName).Append("');\r\n}\r\n");
            }
        }

        protected virtual void GenerateConstructor(WebServiceData webServiceData)
        {
            this.GenerateTypeDeclaration(webServiceData, false);
            this._builder.Append("function() {\r\n");
            this._builder.Append(this.GetProxyTypeName(webServiceData)).Append(".initializeBase(this);\r\n");
            this.GenerateFields();
            this._builder.Append("}\r\n");
        }

        private void GenerateEnumTypeProxies(IEnumerable<WebServiceEnumData> enumTypes)
        {
            foreach (WebServiceEnumData data in enumTypes)
            {
                this.EnsureNamespace(data.TypeNamespace);
                string clientTypeName = ServicesUtilities.GetClientTypeName(this.GetClientTypeNamespace(data.TypeName));
                string[] names = data.Names;
                long[] values = data.Values;
                this._builder.Append("if (typeof(").Append(clientTypeName).Append(") === 'undefined') {\r\n");
                this._builder.Append(clientTypeName).Append(" = function() { throw Error.invalidOperation(); }\r\n");
                this._builder.Append(clientTypeName).Append(".prototype = {");
                for (int i = 0; i < names.Length; i++)
                {
                    if (i > 0)
                    {
                        this._builder.Append(',');
                    }
                    this._builder.Append(names[i]);
                    this._builder.Append(": ");
                    if (data.IsULong)
                    {
                        this._builder.Append((ulong) values[i]);
                    }
                    else
                    {
                        this._builder.Append(values[i]);
                    }
                }
                this._builder.Append("}\r\n");
                this._builder.Append(clientTypeName).Append(".registerEnum('").Append(clientTypeName).Append('\'');
                this._builder.Append(", true);\r\n}\r\n");
            }
        }

        protected void GenerateFields()
        {
            this._builder.Append("this._timeout = 0;\r\n");
            this._builder.Append("this._userContext = null;\r\n");
            this._builder.Append("this._succeeded = null;\r\n");
            this._builder.Append("this._failed = null;\r\n");
        }

        protected virtual void GenerateMethods()
        {
        }

        protected virtual void GeneratePrototype(WebServiceData webServiceData)
        {
            this.GenerateTypeDeclaration(webServiceData, true);
            this._builder.Append("{\r\n");
            this._builder.Append("_get_path:function() {\r\n var p = this.get_path();\r\n if (p) return p;\r\n else return ");
            this._builder.Append(this.GetProxyTypeName(webServiceData)).Append("._staticInstance.get_path();},\r\n");
            bool flag = true;
            foreach (WebServiceMethodData data in webServiceData.MethodDatas)
            {
                if (!flag)
                {
                    this._builder.Append(",\r\n");
                }
                flag = false;
                this.GenerateWebMethodProxy(data);
            }
            this._builder.Append("}\r\n");
        }

        protected void GenerateRegisterClass(WebServiceData webServiceData)
        {
            string proxyTypeName = this.GetProxyTypeName(webServiceData);
            this._builder.Append(proxyTypeName).Append(".registerClass('").Append(proxyTypeName).Append("',Sys.Net.WebServiceProxy);\r\n");
        }

        protected void GenerateStaticInstance(WebServiceData data)
        {
            string proxyTypeName = this.GetProxyTypeName(data);
            this._builder.Append(proxyTypeName).Append("._staticInstance = new ").Append(proxyTypeName).Append("();\r\n");
            if (this._debugMode)
            {
                this._builder.Append(proxyTypeName).Append(".set_path = function(value) {\r\n");
                this._builder.Append(proxyTypeName).Append("._staticInstance.set_path(value); }\r\n");
                this._builder.Append(proxyTypeName).Append(".get_path = function() { \r\n/// <value type=\"String\" mayBeNull=\"true\">The service url.</value>\r\nreturn ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.get_path();}\r\n");
                this._builder.Append(proxyTypeName).Append(".set_timeout = function(value) {\r\n");
                this._builder.Append(proxyTypeName).Append("._staticInstance.set_timeout(value); }\r\n");
                this._builder.Append(proxyTypeName).Append(".get_timeout = function() { \r\n/// <value type=\"Number\">The service timeout.</value>\r\nreturn ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.get_timeout(); }\r\n");
                this._builder.Append(proxyTypeName).Append(".set_defaultUserContext = function(value) { \r\n");
                this._builder.Append(proxyTypeName).Append("._staticInstance.set_defaultUserContext(value); }\r\n");
                this._builder.Append(proxyTypeName).Append(".get_defaultUserContext = function() { \r\n/// <value mayBeNull=\"true\">The service default user context.</value>\r\nreturn ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.get_defaultUserContext(); }\r\n");
                this._builder.Append(proxyTypeName).Append(".set_defaultSucceededCallback = function(value) { \r\n ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.set_defaultSucceededCallback(value); }\r\n");
                this._builder.Append(proxyTypeName).Append(".get_defaultSucceededCallback = function() { \r\n/// <value type=\"Function\" mayBeNull=\"true\">The service default succeeded callback.</value>\r\nreturn ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.get_defaultSucceededCallback(); }\r\n");
                this._builder.Append(proxyTypeName).Append(".set_defaultFailedCallback = function(value) { \r\n");
                this._builder.Append(proxyTypeName).Append("._staticInstance.set_defaultFailedCallback(value); }\r\n");
                this._builder.Append(proxyTypeName).Append(".get_defaultFailedCallback = function() { \r\n/// <value type=\"Function\" mayBeNull=\"true\">The service default failed callback.</value>\r\nreturn ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.get_defaultFailedCallback(); }\r\n");
            }
            else
            {
                this._builder.Append(proxyTypeName).Append(".set_path = function(value) { ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.set_path(value); }\r\n");
                this._builder.Append(proxyTypeName).Append(".get_path = function() { return ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.get_path(); }\r\n");
                this._builder.Append(proxyTypeName).Append(".set_timeout = function(value) { ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.set_timeout(value); }\r\n");
                this._builder.Append(proxyTypeName).Append(".get_timeout = function() { return ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.get_timeout(); }\r\n");
                this._builder.Append(proxyTypeName).Append(".set_defaultUserContext = function(value) { ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.set_defaultUserContext(value); }\r\n");
                this._builder.Append(proxyTypeName).Append(".get_defaultUserContext = function() { return ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.get_defaultUserContext(); }\r\n");
                this._builder.Append(proxyTypeName).Append(".set_defaultSucceededCallback = function(value) { ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.set_defaultSucceededCallback(value); }\r\n");
                this._builder.Append(proxyTypeName).Append(".get_defaultSucceededCallback = function() { return ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.get_defaultSucceededCallback(); }\r\n");
                this._builder.Append(proxyTypeName).Append(".set_defaultFailedCallback = function(value) { ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.set_defaultFailedCallback(value); }\r\n");
                this._builder.Append(proxyTypeName).Append(".get_defaultFailedCallback = function() { return ");
                this._builder.Append(proxyTypeName).Append("._staticInstance.get_defaultFailedCallback(); }\r\n");
            }
            this._builder.Append(proxyTypeName).Append(".set_path(\"").Append(HttpUtility.UrlPathEncode(this.GetProxyPath())).Append("\");\r\n");
        }

        protected void GenerateStaticMethods(WebServiceData webServiceData)
        {
            string proxyTypeName = this.GetProxyTypeName(webServiceData);
            foreach (WebServiceMethodData data in webServiceData.MethodDatas)
            {
                string methodName = data.MethodName;
                this._builder.Append(proxyTypeName).Append('.').Append(methodName).Append("= function(");
                StringBuilder builder = new StringBuilder();
                bool flag = true;
                foreach (WebServiceParameterData data2 in data.ParameterDatas)
                {
                    if (!flag)
                    {
                        builder.Append(',');
                    }
                    else
                    {
                        flag = false;
                    }
                    builder.Append(data2.ParameterName);
                }
                if (!flag)
                {
                    builder.Append(',');
                }
                builder.Append("onSuccess,onFailed,userContext");
                this._builder.Append(builder.ToString()).Append(") {");
                if (this._debugMode)
                {
                    this._builder.Append("\r\n");
                    this._builder.Append(this._docCommentCache[methodName]);
                }
                this._builder.Append(proxyTypeName).Append("._staticInstance.").Append(methodName).Append('(');
                this._builder.Append(builder.ToString()).Append("); }\r\n");
            }
        }

        protected virtual void GenerateTypeDeclaration(WebServiceData webServiceData, bool genClass)
        {
            this.AppendClientTypeDeclaration(webServiceData.TypeData.TypeNamespace, webServiceData.TypeData.TypeName, genClass, true);
        }

        private void GenerateWebMethodProxy(WebServiceMethodData methodData)
        {
            string methodName = methodData.MethodName;
            this.GetProxyTypeName(methodData.Owner);
            string str2 = methodData.UseGet ? "true" : "false";
            this._builder.Append(methodName).Append(':');
            StringBuilder args = new StringBuilder();
            StringBuilder argsDict = new StringBuilder();
            StringBuilder docComments = null;
            string str3 = null;
            if (this._debugMode)
            {
                docComments = new StringBuilder();
            }
            this.BuildArgsDictionary(methodData, args, argsDict, docComments);
            if (this._debugMode)
            {
                str3 = docComments.ToString();
                this._docCommentCache[methodName] = str3;
            }
            this._builder.Append("function(").Append(args.ToString()).Append(") {\r\n");
            if (this._debugMode)
            {
                this._builder.Append(str3);
            }
            this._builder.Append("return this._invoke(this._get_path(), ");
            this._builder.Append("'").Append(methodName).Append("',");
            this._builder.Append(str2).Append(',');
            this._builder.Append(argsDict.ToString()).Append(",succeededCallback,failedCallback,userContext); }");
        }

        internal string GetClientProxyScript(WebServiceData webServiceData)
        {
            if (webServiceData.MethodDatas.Count == 0)
            {
                return null;
            }
            this._builder = new StringBuilder();
            if (this._debugMode)
            {
                this._docCommentCache = new Dictionary<string, string>();
            }
            this.GenerateConstructor(webServiceData);
            this.GeneratePrototype(webServiceData);
            this.GenerateRegisterClass(webServiceData);
            this.GenerateStaticInstance(webServiceData);
            this.GenerateStaticMethods(webServiceData);
            this.GenerateClientTypeProxies(webServiceData);
            this.GenerateEnumTypeProxies(webServiceData.EnumTypes);
            return this._builder.ToString();
        }

        protected virtual string GetClientTypeNamespace(string ns) => 
            ns;

        protected abstract string GetProxyPath();
        protected virtual string GetProxyTypeName(WebServiceData data) => 
            ServicesUtilities.GetClientTypeName(data.TypeData.TypeName);
    }
}

