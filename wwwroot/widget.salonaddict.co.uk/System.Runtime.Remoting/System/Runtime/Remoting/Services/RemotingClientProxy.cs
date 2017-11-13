namespace System.Runtime.Remoting.Services
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Http;

    [ComVisible(true)]
    public abstract class RemotingClientProxy : Component
    {
        protected object _tp;
        protected Type _type;
        protected string _url;

        protected RemotingClientProxy()
        {
        }

        protected void ConfigureProxy(Type type, string url)
        {
            lock (this)
            {
                this._type = type;
                this.Url = url;
            }
        }

        protected void ConnectProxy()
        {
            lock (this)
            {
                this._tp = null;
                this._tp = Activator.GetObject(this._type, this._url);
            }
        }

        public bool AllowAutoRedirect
        {
            get => 
                ((bool) ChannelServices.GetChannelSinkProperties(this._tp)["allowautoredirect"]);
            set
            {
                ChannelServices.GetChannelSinkProperties(this._tp)["allowautoredirect"] = value;
            }
        }

        public object Cookies =>
            null;

        public string Domain
        {
            get => 
                ((string) ChannelServices.GetChannelSinkProperties(this._tp)["domain"]);
            set
            {
                ChannelServices.GetChannelSinkProperties(this._tp)["domain"] = value;
            }
        }

        public bool EnableCookies
        {
            get => 
                false;
            set
            {
                throw new NotSupportedException();
            }
        }

        public string Password
        {
            get => 
                ((string) ChannelServices.GetChannelSinkProperties(this._tp)["password"]);
            set
            {
                ChannelServices.GetChannelSinkProperties(this._tp)["password"] = value;
            }
        }

        public string Path
        {
            get => 
                this.Url;
            set
            {
                this.Url = value;
            }
        }

        public bool PreAuthenticate
        {
            get => 
                ((bool) ChannelServices.GetChannelSinkProperties(this._tp)["preauthenticate"]);
            set
            {
                ChannelServices.GetChannelSinkProperties(this._tp)["preauthenticate"] = value;
            }
        }

        public string ProxyName
        {
            get => 
                ((string) ChannelServices.GetChannelSinkProperties(this._tp)["proxyname"]);
            set
            {
                ChannelServices.GetChannelSinkProperties(this._tp)["Proxyname"] = value;
            }
        }

        public int ProxyPort
        {
            get => 
                ((int) ChannelServices.GetChannelSinkProperties(this._tp)["proxyport"]);
            set
            {
                ChannelServices.GetChannelSinkProperties(this._tp)["proxyport"] = value;
            }
        }

        public int Timeout
        {
            get => 
                ((int) ChannelServices.GetChannelSinkProperties(this._tp)["timeout"]);
            set
            {
                ChannelServices.GetChannelSinkProperties(this._tp)["timeout"] = value;
            }
        }

        public string Url
        {
            get => 
                this._url;
            set
            {
                lock (this)
                {
                    this._url = value;
                }
                this.ConnectProxy();
                ChannelServices.GetChannelSinkProperties(this._tp)["url"] = value;
            }
        }

        public string UserAgent
        {
            get => 
                HttpClientTransportSink.UserAgent;
            set
            {
                throw new NotSupportedException();
            }
        }

        public string Username
        {
            get => 
                ((string) ChannelServices.GetChannelSinkProperties(this._tp)["username"]);
            set
            {
                ChannelServices.GetChannelSinkProperties(this._tp)["username"] = value;
            }
        }
    }
}

