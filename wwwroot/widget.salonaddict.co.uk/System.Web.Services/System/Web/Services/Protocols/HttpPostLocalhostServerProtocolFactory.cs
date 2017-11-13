namespace System.Web.Services.Protocols
{
    using System;
    using System.Web;

    internal class HttpPostLocalhostServerProtocolFactory : ServerProtocolFactory
    {
        protected override ServerProtocol CreateIfRequestCompatible(HttpRequest request)
        {
            if (request.PathInfo.Length < 2)
            {
                return null;
            }
            if (request.HttpMethod != "POST")
            {
                return new UnsupportedRequestProtocol(0x195);
            }
            string str = request.ServerVariables["LOCAL_ADDR"];
            string str2 = request.ServerVariables["REMOTE_ADDR"];
            if (!(request.Url.IsLoopback || (((str != null) && (str2 != null)) && (str == str2))))
            {
                return null;
            }
            return new HttpPostServerProtocol();
        }
    }
}

