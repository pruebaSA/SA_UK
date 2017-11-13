namespace SA.Payments.Realex.RealVault
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Xml;

    public class RealVaultResponse : ResponseBase
    {
        private string _account;
        private string _authCode;
        private string _batchID;
        private string _merchantID;
        private string _message;
        private string _orderID;
        private string _pasRef;
        private string _processingTimeTaken;
        private string _responseText;
        private string _result;
        private string _signature;
        private string _timestamp;
        private string _timetaken;

        protected virtual string GenerateSignature(string sharedSecret)
        {
            string s = $"{this._timestamp ?? string.Empty}.{this._merchantID ?? string.Empty}.{this._orderID ?? string.Empty}.{this._result ?? string.Empty}.{this._message ?? string.Empty}.{this._pasRef ?? string.Empty}.{this._authCode ?? string.Empty}";
            SHA1 sha = new SHA1Managed();
            string str2 = Helper.HexEncode(sha.ComputeHash(Encoding.UTF8.GetBytes(s)));
            return Helper.HexEncode(sha.ComputeHash(Encoding.UTF8.GetBytes(str2 + "." + sharedSecret)));
        }

        public override bool HasErrors() => 
            (this._result != "00");

        public override bool IsValid(ISettingsProvider settings)
        {
            if (settings == null)
            {
                throw new RealVaultException("Settings cannot be null", new ArgumentNullException("settings"));
            }
            return (this.HasErrors() || (this.GenerateSignature(settings.SharedSecret) == this.Signature));
        }

        public override void LoadXML(string value)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(value);
            XmlNode node = document.SelectSingleNode("/response");
            XmlNode node2 = node.SelectSingleNode("@timestamp");
            XmlNode node3 = node.SelectSingleNode("merchantid");
            XmlNode node4 = node.SelectSingleNode("account");
            XmlNode node5 = node.SelectSingleNode("orderid");
            XmlNode node6 = node.SelectSingleNode("result");
            XmlNode node7 = node.SelectSingleNode("message");
            XmlNode node8 = node.SelectSingleNode("pasref");
            XmlNode node9 = node.SelectSingleNode("authcode");
            XmlNode node10 = node.SelectSingleNode("batchid");
            XmlNode node11 = node.SelectSingleNode("timetaken");
            XmlNode node12 = node.SelectSingleNode("processingtimetaken");
            XmlNode node13 = node.SelectSingleNode("sha1hash");
            string str = string.Empty;
            if (node2 != null)
            {
                str = node2.InnerText.Trim();
            }
            string str2 = string.Empty;
            if (node3 != null)
            {
                str2 = node3.InnerText.Trim();
            }
            string str3 = string.Empty;
            if (node4 != null)
            {
                str3 = node4.InnerText.Trim();
            }
            string str4 = string.Empty;
            if (node6 != null)
            {
                str4 = node6.InnerText.Trim();
            }
            string str5 = string.Empty;
            if (node7 != null)
            {
                str5 = node7.InnerText.Trim();
            }
            string str6 = string.Empty;
            if (node5 != null)
            {
                str6 = node5.InnerText.Trim();
            }
            string str7 = string.Empty;
            if (node8 != null)
            {
                str7 = node8.InnerText.Trim();
            }
            string str8 = string.Empty;
            if (node9 != null)
            {
                str8 = node9.InnerText.Trim();
            }
            string str9 = string.Empty;
            if (node10 != null)
            {
                str9 = node10.InnerText.Trim();
            }
            string str10 = string.Empty;
            if (node11 != null)
            {
                str10 = node11.InnerText.Trim();
            }
            string str11 = string.Empty;
            if (node12 != null)
            {
                str11 = node12.InnerText.Trim();
            }
            string str12 = string.Empty;
            if (node13 != null)
            {
                str12 = node13.InnerText.Trim();
            }
            this._account = str3;
            this._authCode = str8;
            this._batchID = str9;
            this._merchantID = str2;
            this._message = str5;
            this._orderID = str6;
            this._pasRef = str7;
            this._processingTimeTaken = str11;
            this._result = str4;
            this._signature = str12;
            this._timestamp = str;
            this._timetaken = str10;
            this._responseText = value;
        }

        public override string ToString() => 
            this._responseText;

        public virtual string Account =>
            this._account;

        public string AuthCode =>
            this._authCode;

        public string BatchID =>
            this._batchID;

        public virtual string MerchantID =>
            this._merchantID;

        public string Message =>
            this._message;

        public string OrderID =>
            this._orderID;

        public string PasRef =>
            this._pasRef;

        public string ProcessingTimeTaken =>
            this._processingTimeTaken;

        public string Result =>
            this._result;

        internal virtual string Signature =>
            this._signature;

        public virtual string TimeStamp =>
            this._timestamp;

        public string TimeTaken =>
            this._timetaken;
    }
}

