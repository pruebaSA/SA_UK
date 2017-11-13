namespace System.Runtime.Remoting.MetadataServices
{
    using System;

    public class ServiceType
    {
        private Type _type;
        private string _url;

        public ServiceType(Type type)
        {
            this._type = type;
            this._url = null;
        }

        public ServiceType(Type type, string url)
        {
            this._type = type;
            this._url = url;
        }

        public Type ObjectType =>
            this._type;

        public string Url =>
            this._url;
    }
}

