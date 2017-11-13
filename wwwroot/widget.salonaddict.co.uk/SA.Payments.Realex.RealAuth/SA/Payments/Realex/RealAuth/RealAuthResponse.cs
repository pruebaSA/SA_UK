namespace SA.Payments.Realex.RealAuth
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    public class RealAuthResponse : ResponseBase
    {
        private string _account;
        private string _authCode;
        private string _batchID;
        private Dictionary<string, string> _checks = new Dictionary<string, string>();
        private string _issuingBank;
        private string _issuingCountry;
        private string _issuingCountryCode;
        private string _issuingRegion;
        private string _merchantID;
        private string _message;
        private string _orderID;
        private string _pasRef;
        private string _realScore;
        private string _responseText;
        private string _result;
        private string _resultCVN;
        private string _signature;
        private string _timestamp;

        public override bool HasErrors() => 
            (this._result != "00");

        public override bool IsValid(ISettingsProvider settings)
        {
            if (settings == null)
            {
                throw new RealAuthException("Settings cannot be null", new ArgumentNullException("settings"));
            }
            return (this.HasErrors() || (Helper.GenerateSignatureSHA1(settings.SharedSecret, new string[] { this._timestamp, settings.MerchantID, this._orderID, this._result, this._message, this._pasRef, this._authCode }) == this.Signature));
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
            XmlNode node6 = node.SelectSingleNode("authcode");
            XmlNode node7 = node.SelectSingleNode("result");
            XmlNode node8 = node.SelectSingleNode("message");
            XmlNode node9 = node.SelectSingleNode("pasref");
            XmlNode node10 = node.SelectSingleNode("cvnresult");
            XmlNode node11 = node.SelectSingleNode("batchid");
            XmlNode node12 = node.SelectSingleNode("cardissuer");
            XmlNode node13 = node.SelectSingleNode("tss");
            XmlNode node14 = node.SelectSingleNode("sha1hash");
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
            if (node5 != null)
            {
                str4 = node5.InnerText.Trim();
            }
            string str5 = string.Empty;
            if (node6 != null)
            {
                str5 = node6.InnerText.Trim();
            }
            string str6 = string.Empty;
            if (node7 != null)
            {
                str6 = node7.InnerText.Trim();
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
            string str12 = string.Empty;
            string str13 = string.Empty;
            string str14 = string.Empty;
            if (node12 != null)
            {
                XmlNode node15 = node12.SelectSingleNode("bank");
                XmlNode node16 = node12.SelectSingleNode("country");
                XmlNode node17 = node12.SelectSingleNode("countrycode");
                XmlNode node18 = node12.SelectSingleNode("region");
                if (node15 != null)
                {
                    str11 = node15.InnerText.Trim();
                }
                if (node16 != null)
                {
                    str12 = node16.InnerText.Trim();
                }
                if (node17 != null)
                {
                    str13 = node17.InnerText.Trim();
                }
                if (node18 != null)
                {
                    str14 = node18.InnerText.Trim();
                }
            }
            string str15 = string.Empty;
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (node13 != null)
            {
                XmlNode node19 = node13.SelectSingleNode("result");
                if (node19 != null)
                {
                    str15 = node19.InnerText.Trim();
                }
                XmlNodeList list = node13.SelectNodes("check");
                if ((list != null) && (list.Count > 0))
                {
                    foreach (XmlNode node20 in list)
                    {
                        if (node20.Attributes["id"] != null)
                        {
                            dictionary.Add(node20.Attributes["id"].InnerText.Trim(), node20.InnerText.Trim());
                        }
                    }
                }
            }
            string str16 = string.Empty;
            if (node14 != null)
            {
                str16 = node14.InnerText.Trim();
            }
            this._account = str3;
            this._authCode = str5;
            this._batchID = str10;
            this._checks = dictionary;
            this._issuingBank = str11;
            this._issuingCountry = str12;
            this._issuingCountryCode = str13;
            this._issuingRegion = str14;
            this._merchantID = str2;
            this._message = str7;
            this._orderID = str4;
            this._pasRef = str8;
            this._realScore = str15;
            this._result = str6;
            this._resultCVN = str9;
            this._signature = str16;
            this._timestamp = str;
            this._responseText = value;
        }

        public override string ToString() => 
            (this._responseText ?? string.Empty);

        public virtual string Account =>
            this._account;

        public string AuthCode =>
            this._authCode;

        public string BatchID =>
            this._batchID;

        public Dictionary<string, string> Checks =>
            this._checks;

        public string IssuingBank =>
            this._issuingBank;

        public string IssuingCountry =>
            this._issuingCountry;

        public string IssuingCountryCode =>
            this._issuingCountryCode;

        public string IssuingRegion =>
            this._issuingRegion;

        public virtual string MerchantID =>
            this._merchantID;

        public string Message =>
            this._message;

        public string OrderID =>
            this._orderID;

        public string PasRef =>
            this._pasRef;

        public string RealScore =>
            this._realScore;

        public string Result =>
            this._result;

        public string ResultCVN =>
            this._resultCVN;

        internal virtual string Signature =>
            this._signature;

        public virtual string TimeStamp =>
            this._timestamp;
    }
}

