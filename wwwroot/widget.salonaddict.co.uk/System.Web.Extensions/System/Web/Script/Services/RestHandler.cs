namespace System.Web.Script.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Security;
    using System.Text;
    using System.Web;
    using System.Web.Resources;
    using System.Web.Script.Serialization;

    internal class RestHandler : IHttpHandler
    {
        private WebServiceMethodData _webServiceMethodData;

        internal static IHttpHandler CreateHandler(HttpContext context)
        {
            if ((context.Request.PathInfo.Length < 2) || (context.Request.PathInfo[0] != '/'))
            {
                throw new InvalidOperationException(AtlasWeb.WebService_InvalidWebServiceCall);
            }
            WebServiceData webServiceData = WebServiceData.GetWebServiceData(context, context.Request.FilePath);
            string methodName = context.Request.PathInfo.Substring(1);
            return CreateHandler(webServiceData, methodName);
        }

        private static IHttpHandler CreateHandler(WebServiceData webServiceData, string methodName)
        {
            RestHandler handler;
            WebServiceMethodData methodData = webServiceData.GetMethodData(methodName);
            if (methodData.RequiresSession)
            {
                handler = new RestHandlerWithSession();
            }
            else
            {
                handler = new RestHandler();
            }
            handler._webServiceMethodData = methodData;
            return handler;
        }

        internal static void ExecuteWebServiceCall(HttpContext context, WebServiceMethodData methodData)
        {
            try
            {
                NamedPermissionSet namedPermissionSet = HttpRuntime.NamedPermissionSet;
                if (namedPermissionSet != null)
                {
                    namedPermissionSet.PermitOnly();
                }
                IDictionary<string, object> rawParams = GetRawParams(methodData, context);
                InvokeMethod(context, methodData, rawParams);
            }
            catch (Exception exception)
            {
                WriteExceptionJsonString(context, exception);
            }
        }

        private static IDictionary<string, object> GetRawParams(WebServiceMethodData methodData, HttpContext context)
        {
            if (methodData.UseGet)
            {
                if (context.Request.HttpMethod != "GET")
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.WebService_InvalidVerbRequest, new object[] { methodData.MethodName, "POST" }));
                }
                return GetRawParamsFromGetRequest(context, methodData.Owner.Serializer, methodData);
            }
            if (context.Request.HttpMethod != "POST")
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.WebService_InvalidVerbRequest, new object[] { methodData.MethodName, "GET" }));
            }
            return GetRawParamsFromPostRequest(context, methodData.Owner.Serializer);
        }

        private static IDictionary<string, object> GetRawParamsFromGetRequest(HttpContext context, JavaScriptSerializer serializer, WebServiceMethodData methodData)
        {
            NameValueCollection queryString = context.Request.QueryString;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (WebServiceParameterData data in methodData.ParameterDatas)
            {
                string name = data.ParameterInfo.Name;
                string input = queryString[name];
                if (input != null)
                {
                    dictionary.Add(name, serializer.DeserializeObject(input));
                }
            }
            return dictionary;
        }

        private static IDictionary<string, object> GetRawParamsFromPostRequest(HttpContext context, JavaScriptSerializer serializer)
        {
            string str = new StreamReader(context.Request.InputStream).ReadToEnd();
            if (string.IsNullOrEmpty(str))
            {
                return new Dictionary<string, object>();
            }
            return serializer.Deserialize<IDictionary<string, object>>(str);
        }

        private static void InitializeCachePolicy(WebServiceMethodData methodData, HttpContext context)
        {
            int cacheDuration = methodData.CacheDuration;
            if (cacheDuration > 0)
            {
                context.Response.Cache.SetCacheability(HttpCacheability.Server);
                context.Response.Cache.SetExpires(DateTime.Now.AddSeconds((double) cacheDuration));
                context.Response.Cache.SetSlidingExpiration(false);
                context.Response.Cache.SetValidUntilExpires(true);
                if (methodData.ParameterDatas.Count > 0)
                {
                    context.Response.Cache.VaryByParams["*"] = true;
                }
                else
                {
                    context.Response.Cache.VaryByParams.IgnoreParams = true;
                }
            }
            else
            {
                context.Response.Cache.SetNoServerCaching();
                context.Response.Cache.SetMaxAge(TimeSpan.Zero);
            }
        }

        private static void InvokeMethod(HttpContext context, WebServiceMethodData methodData, IDictionary<string, object> rawParams)
        {
            string str;
            InitializeCachePolicy(methodData, context);
            object target = null;
            if (!methodData.IsStatic)
            {
                target = Activator.CreateInstance(methodData.Owner.TypeData.Type);
            }
            object obj3 = methodData.CallMethodFromRawParams(target, rawParams);
            string s = null;
            if (methodData.UseXmlResponse)
            {
                s = obj3 as string;
                if ((s == null) || methodData.XmlSerializeString)
                {
                    try
                    {
                        s = ServicesUtilities.XmlSerializeObjectToString(obj3);
                    }
                    catch (Exception exception)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.WebService_InvalidXmlReturnType, new object[] { methodData.MethodName, obj3.GetType().FullName, exception.Message }));
                    }
                }
                str = "text/xml";
            }
            else
            {
                s = "{\"d\":" + methodData.Owner.Serializer.Serialize(obj3) + "}";
                str = "application/json";
            }
            context.Response.ContentType = str;
            if (s != null)
            {
                context.Response.Write(s);
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            ExecuteWebServiceCall(context, this._webServiceMethodData);
        }

        internal static void WriteExceptionJsonString(HttpContext context, Exception ex)
        {
            WriteExceptionJsonString(context, ex, 500);
        }

        internal static void WriteExceptionJsonString(HttpContext context, Exception ex, int statusCode)
        {
            string charset = context.Response.Charset;
            context.Response.ClearHeaders();
            context.Response.ClearContent();
            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(statusCode);
            context.Response.ContentType = "application/json";
            context.Response.AddHeader("jsonerror", "true");
            context.Response.Charset = charset;
            context.Response.TrySkipIisCustomErrors = true;
            using (StreamWriter writer = new StreamWriter(context.Response.OutputStream, new UTF8Encoding(false)))
            {
                if (ex is TargetInvocationException)
                {
                    ex = ex.InnerException;
                }
                if (context.IsCustomErrorEnabled)
                {
                    writer.Write(JavaScriptSerializer.SerializeInternal(new WebServiceError(AtlasWeb.WebService_Error, string.Empty, string.Empty)));
                }
                else
                {
                    writer.Write(JavaScriptSerializer.SerializeInternal(new WebServiceError(ex.Message, ex.StackTrace, ex.GetType().FullName)));
                }
                writer.Flush();
            }
        }

        public bool IsReusable =>
            false;

        internal class WebServiceError
        {
            public string ExceptionType;
            public string Message;
            public string StackTrace;

            public WebServiceError(string msg, string stack, string type)
            {
                this.Message = msg;
                this.StackTrace = stack;
                this.ExceptionType = type;
            }
        }
    }
}

