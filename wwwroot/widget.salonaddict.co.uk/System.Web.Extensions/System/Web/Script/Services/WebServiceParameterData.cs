namespace System.Web.Script.Services
{
    using System;
    using System.Reflection;

    internal class WebServiceParameterData
    {
        private int _index;
        private System.Reflection.ParameterInfo _param;
        private string _paramName;
        private Type _paramType;

        internal WebServiceParameterData(System.Reflection.ParameterInfo param, int index)
        {
            this._param = param;
            this._index = index;
        }

        internal WebServiceParameterData(string paramName, Type paramType, int index)
        {
            this._paramName = paramName;
            this._paramType = paramType;
            this._index = index;
        }

        internal int Index =>
            this._index;

        internal System.Reflection.ParameterInfo ParameterInfo =>
            this._param;

        internal string ParameterName
        {
            get
            {
                if (this._param != null)
                {
                    return this._param.Name;
                }
                return this._paramName;
            }
        }

        internal Type ParameterType
        {
            get
            {
                if (this._param != null)
                {
                    return this._param.ParameterType;
                }
                return this._paramType;
            }
        }
    }
}

