namespace System.Data.SqlClient
{
    using System;
    using System.Data.Common;
    using System.Globalization;

    internal sealed class ServerInfo
    {
        private string _extendedServerName;
        private string _resolvedServerName;
        private string _userProtocol;
        private string _userServerName;
        internal readonly string PreRoutingServerName;

        internal ServerInfo(string userProtocol, string userServerName)
        {
            this._userProtocol = userProtocol;
            this._userServerName = userServerName;
            this.PreRoutingServerName = null;
        }

        internal ServerInfo(SqlConnectionString userOptions, RoutingInfo routing, string preRoutingServerName)
        {
            if ((routing == null) || (routing.ServerName == null))
            {
                this.UserServerName = string.Empty;
            }
            else
            {
                this.UserServerName = string.Format(CultureInfo.InvariantCulture, "{0},{1}", new object[] { routing.ServerName, routing.Port });
            }
            this.PreRoutingServerName = preRoutingServerName;
            this.UserProtocol = "tcp";
            this.SetDerivedNames(this.UserProtocol, this.UserServerName);
        }

        internal void SetDerivedNames(string protocol, string serverName)
        {
            if (!ADP.IsEmpty(protocol))
            {
                this.ExtendedServerName = protocol + ":" + serverName;
            }
            else
            {
                this.ExtendedServerName = serverName;
            }
            this.ResolvedServerName = serverName;
        }

        internal string ExtendedServerName
        {
            get => 
                this._extendedServerName;
            set
            {
                this._extendedServerName = value;
            }
        }

        internal string ResolvedServerName
        {
            get => 
                this._resolvedServerName;
            set
            {
                this._resolvedServerName = value;
            }
        }

        internal string UserProtocol
        {
            get => 
                this._userProtocol;
            set
            {
                this._userProtocol = value;
            }
        }

        internal string UserServerName
        {
            get => 
                this._userServerName;
            set
            {
                this._userServerName = value;
            }
        }
    }
}

