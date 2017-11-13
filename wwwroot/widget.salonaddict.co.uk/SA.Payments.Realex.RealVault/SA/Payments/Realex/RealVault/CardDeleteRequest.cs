namespace SA.Payments.Realex.RealVault
{
    using System;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    public class CardDeleteRequest : RequestBase
    {
        private string _cardRef;
        private string _payerRef;

        public CardDeleteRequest(string payerRef, string cardRef) : this(payerRef, cardRef, new SettingsProviderConfig())
        {
        }

        public CardDeleteRequest(string payerRef, string cardRef, ISettingsProvider settings) : base(settings)
        {
            this.CardRef = cardRef;
            this.PayerRef = payerRef;
        }

        public override XElement ConvertToXML()
        {
            ISettingsProvider settings = base.Settings;
            XElement element = new XElement("request");
            element.Add(new XAttribute("timestamp", base.TimeStamp));
            element.Add(new XAttribute("type", "card-cancel-card"));
            element.Add(new XElement("merchantid", settings.MerchantID));
            element.Add(new XElement("card", new object[] { new XElement("ref", this._cardRef), new XElement("payerref", this._payerRef) }));
            string content = Helper.GenerateSignatureSHA1(settings.SharedSecret, new string[] { base.TimeStamp, settings.MerchantID, this._payerRef, this._cardRef });
            element.Add(new XElement("sha1hash", content));
            return element;
        }

        protected virtual void ValidateCardRef(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new RealVaultException("Card reference cannot be null or empty.");
            }
            if (value.Length > 30)
            {
                throw new RealVaultException("Card reference must be less than 40 characters in length.");
            }
            if (!Regex.IsMatch(value, "^[a-zA-Z0-9_]*$"))
            {
                throw new RealVaultException("Card reference can only contain the characters: 'a-z', 'A-Z', '0-9', and '_'.");
            }
        }

        protected virtual void ValidatePayerRef(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new RealVaultException("Payer reference cannot be null or empty.");
            }
            if (value.Length > 50)
            {
                throw new RealVaultException("Payer reference must be less than 40 characters in length.");
            }
            if (!Regex.IsMatch(value, "^[a-zA-Z0-9_]*$"))
            {
                throw new RealVaultException("Payer reference can only contain the characters: 'a-z', 'A-Z', '0-9', and '_'.");
            }
        }

        public string CardRef
        {
            get => 
                this._cardRef;
            set
            {
                this.ValidateCardRef(value);
                this._cardRef = value;
            }
        }

        public string PayerRef
        {
            get => 
                this._payerRef;
            set
            {
                this.ValidatePayerRef(value);
                this._payerRef = value;
            }
        }
    }
}

