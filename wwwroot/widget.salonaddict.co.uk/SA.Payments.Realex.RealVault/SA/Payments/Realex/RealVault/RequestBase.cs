namespace SA.Payments.Realex.RealVault
{
    using System;
    using System.IO;
    using System.Net;
    using System.Xml.Linq;

    public abstract class RequestBase
    {
        private ISettingsProvider _settings;
        private readonly string _timestamp;

        protected RequestBase(ISettingsProvider settings)
        {
            this._settings = settings;
            this._timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public abstract XElement ConvertToXML();
        public TResult GetResponse<TResult>() where TResult: ResponseBase, new()
        {
            TResult local2;
            try
            {
                string str = this.ConvertToXML().ToString();
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create("https://epage.payandshop.com/epage-remote-plugins.cgi");
                request.ContentType = "text/xml";
                request.Timeout = 0xafc8;
                request.AllowAutoRedirect = false;
                request.ContentLength = str.Length;
                request.Method = "POST";
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(str);
                    writer.Flush();
                    writer.Close();
                }
                string str2 = string.Empty;
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    str2 = reader.ReadToEnd();
                }
                TResult local = Activator.CreateInstance<TResult>();
                local.LoadXML(str2);
                local2 = local;
            }
            catch (Exception exception)
            {
                throw new RealVaultException(exception.Message, exception);
            }
            return local2;
        }

        public ISettingsProvider Settings =>
            this._settings;

        public virtual string TimeStamp =>
            this._timestamp;
    }
}

