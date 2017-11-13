namespace System.Web.Script.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Web.Resources;
    using System.Web.Script.Serialization;
    using System.Web.Services;

    internal class WebServiceMethodData
    {
        private System.Reflection.MethodInfo _methodInfo;
        private string _methodName;
        private WebServiceData _owner;
        private Dictionary<string, WebServiceParameterData> _parameterData;
        private ScriptMethodAttribute _scriptMethodAttribute;
        private bool? _useHttpGet;
        private WebMethodAttribute _webMethodAttribute;

        internal WebServiceMethodData(WebServiceData owner, System.Reflection.MethodInfo methodInfo, WebMethodAttribute webMethodAttribute, ScriptMethodAttribute scriptMethodAttribute)
        {
            this._owner = owner;
            this._methodInfo = methodInfo;
            this._webMethodAttribute = webMethodAttribute;
            this._methodName = this._webMethodAttribute.MessageName;
            this._scriptMethodAttribute = scriptMethodAttribute;
            if (string.IsNullOrEmpty(this._methodName))
            {
                this._methodName = methodInfo.Name;
            }
        }

        internal WebServiceMethodData(WebServiceData owner, string methodName, Dictionary<string, WebServiceParameterData> parameterData, bool useHttpGet)
        {
            this._owner = owner;
            this._methodName = methodName;
            this._parameterData = parameterData;
            this._useHttpGet = new bool?(useHttpGet);
        }

        private object CallMethod(object target, IDictionary<string, object> parameters)
        {
            this.EnsureParameters();
            object[] objArray = new object[this._parameterData.Count];
            foreach (WebServiceParameterData data in this._parameterData.Values)
            {
                object obj2;
                if (!parameters.TryGetValue(data.ParameterInfo.Name, out obj2))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.WebService_MissingArg, new object[] { data.ParameterInfo.Name }));
                }
                objArray[data.Index] = obj2;
            }
            return this._methodInfo.Invoke(target, objArray);
        }

        internal object CallMethodFromRawParams(object target, IDictionary<string, object> parameters)
        {
            parameters = this.StrongTypeParameters(parameters);
            return this.CallMethod(target, parameters);
        }

        private void EnsureParameters()
        {
            if (this._parameterData == null)
            {
                lock (this)
                {
                    Dictionary<string, WebServiceParameterData> dictionary = new Dictionary<string, WebServiceParameterData>();
                    int index = 0;
                    foreach (ParameterInfo info in this._methodInfo.GetParameters())
                    {
                        dictionary[info.Name] = new WebServiceParameterData(info, index);
                        index++;
                    }
                    this._parameterData = dictionary;
                }
            }
        }

        private IDictionary<string, object> StrongTypeParameters(IDictionary<string, object> rawParams)
        {
            IDictionary<string, WebServiceParameterData> parameterDataDictionary = this.ParameterDataDictionary;
            IDictionary<string, object> dictionary2 = new Dictionary<string, object>(rawParams.Count);
            foreach (KeyValuePair<string, object> pair in rawParams)
            {
                string key = pair.Key;
                if (parameterDataDictionary.ContainsKey(key))
                {
                    Type parameterType = parameterDataDictionary[key].ParameterInfo.ParameterType;
                    dictionary2[key] = ObjectConverter.ConvertObjectToType(pair.Value, parameterType, this.Owner.Serializer);
                }
            }
            return dictionary2;
        }

        internal int CacheDuration =>
            this._webMethodAttribute.CacheDuration;

        internal bool IsStatic =>
            this._methodInfo.IsStatic;

        internal System.Reflection.MethodInfo MethodInfo =>
            this._methodInfo;

        internal string MethodName =>
            this._methodName;

        internal WebServiceData Owner =>
            this._owner;

        internal IDictionary<string, WebServiceParameterData> ParameterDataDictionary
        {
            get
            {
                this.EnsureParameters();
                return this._parameterData;
            }
        }

        internal ICollection<WebServiceParameterData> ParameterDatas =>
            this.ParameterDataDictionary.Values;

        internal bool RequiresSession =>
            this._webMethodAttribute.EnableSession;

        internal Type ReturnType
        {
            get
            {
                if (this._methodInfo != null)
                {
                    return this._methodInfo.ReturnType;
                }
                return null;
            }
        }

        internal bool UseGet
        {
            get
            {
                if (this._useHttpGet.HasValue)
                {
                    return this._useHttpGet.Value;
                }
                return ((this._scriptMethodAttribute != null) && this._scriptMethodAttribute.UseHttpGet);
            }
        }

        internal bool UseXmlResponse =>
            ((this._scriptMethodAttribute != null) && (this._scriptMethodAttribute.ResponseFormat == ResponseFormat.Xml));

        internal bool XmlSerializeString =>
            ((this._scriptMethodAttribute != null) && this._scriptMethodAttribute.XmlSerializeString);
    }
}

