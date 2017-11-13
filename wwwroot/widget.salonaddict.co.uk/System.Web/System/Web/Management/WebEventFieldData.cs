namespace System.Web.Management
{
    using System;

    internal class WebEventFieldData
    {
        private string _data;
        private string _name;
        private WebEventFieldType _type;

        public WebEventFieldData(string name, string data, WebEventFieldType type)
        {
            this._name = name;
            this._data = data;
            this._type = type;
        }

        public string Data =>
            this._data;

        public string Name =>
            this._name;

        public WebEventFieldType Type =>
            this._type;
    }
}

