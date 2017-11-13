namespace SA.Payments.Realex.RealVault
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    public class CardCreateRequest : RequestBase
    {
        private string _cardName;
        private string _cardNumber;
        private string _cardRef;
        private SA.Payments.Realex.RealVault.CardType _cardType;
        private string _expiry;
        private string _issueno;
        private string _orderID;
        private string _payerRef;

        public CardCreateRequest(string payerRef, string cardRef, SA.Payments.Realex.RealVault.CardType cardType, string cardName, string cardNumber, string expiry) : this(payerRef, cardRef, cardType, cardName, cardNumber, expiry, new SettingsProviderConfig())
        {
        }

        public CardCreateRequest(string payerRef, string cardRef, SA.Payments.Realex.RealVault.CardType cardType, string cardName, string cardNumber, string expiry, ISettingsProvider settings) : base(settings)
        {
            this.PayerRef = payerRef;
            this.CardRef = cardRef;
            this.CardType = cardType;
            this.CardName = cardName;
            this.CardNumber = cardNumber;
            this.Expiry = expiry;
        }

        public override XElement ConvertToXML()
        {
            ISettingsProvider settings = base.Settings;
            XElement element = new XElement("request");
            element.Add(new XAttribute("type", "card-new"));
            element.Add(new XAttribute("timestamp", base.TimeStamp));
            element.Add(new XElement("merchantid", settings.MerchantID));
            element.Add(new XElement("orderid", this._orderID ?? string.Empty));
            Func<SA.Payments.Realex.RealVault.CardType, string> func = delegate (SA.Payments.Realex.RealVault.CardType cardType) {
                switch (cardType)
                {
                    case SA.Payments.Realex.RealVault.CardType.Visa:
                        return "VISA";

                    case SA.Payments.Realex.RealVault.CardType.MasterCard:
                        return "MC";

                    case SA.Payments.Realex.RealVault.CardType.Switch:
                        return "SWITCH";

                    case SA.Payments.Realex.RealVault.CardType.Amex:
                        return "AMEX";

                    case SA.Payments.Realex.RealVault.CardType.Laser:
                        return "LASER";

                    case SA.Payments.Realex.RealVault.CardType.Diners:
                        return "DINERS";
                }
                throw new RealVaultException("Invalid card type.");
            };
            object[] content = new object[] { new XElement("ref", this._cardRef), new XElement("payerref", this._payerRef), new XElement("number", this._cardNumber), new XElement("expdate", this._expiry), new XElement("chname", this._cardName), new XElement("type", func(this._cardType)), new XElement("issueno", this._issueno ?? string.Empty) };
            element.Add(new XElement("card", content));
            string[] strArray = new string[] { base.TimeStamp, settings.MerchantID, this._orderID ?? string.Empty, string.Empty, string.Empty, this._payerRef, this._cardName, this._cardNumber };
            string str = Helper.GenerateSignatureSHA1(settings.SharedSecret, strArray);
            element.Add(new XElement("sha1hash", str));
            return element;
        }

        protected virtual void ValidateCardName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new RealVaultException("Card name cannot be null or empty.");
            }
        }

        protected virtual void ValidateCardNumber(string value)
        {
            if (!Helper.IsValidCardNumber(value))
            {
                throw new RealVaultException("Invalid card number.");
            }
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

        protected virtual void ValidateCardType(SA.Payments.Realex.RealVault.CardType value)
        {
        }

        protected virtual void ValidateExpiry(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new RealVaultException("Expiry cannot be null or empty.");
            }
            if (value.Length != 4)
            {
                throw new RealVaultException("Expiry must be in the format of MMyy");
            }
            bool flag = false;
            try
            {
                DateTime.ParseExact(value, "MMyy", CultureInfo.InvariantCulture);
                flag = true;
            }
            catch
            {
            }
            if (!flag)
            {
                throw new RealVaultException("Expiry is invalid.");
            }
        }

        protected virtual void ValidateIssueNo(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (!Regex.IsMatch(value, "^[0-9]*$"))
                {
                    throw new RealVaultException("Issue number can only contain numeric characters.");
                }
                if (value.Length > 3)
                {
                    throw new RealVaultException("Issue number cannot be greater than 3 characters in length.");
                }
            }
        }

        protected virtual void ValidateOrderID(string value)
        {
            if (value != null)
            {
                if (value.Length > 40)
                {
                    throw new RealVaultException("Order identifier must be less than 40 characters in length.");
                }
                if (!Regex.IsMatch(value, @"^[a-zA-Z0-9\-_]*$"))
                {
                    throw new RealVaultException("Order identifier can only contain the characters: 'a-z', 'A-Z', '0-9', '-', and '_'.");
                }
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

        public string CardName
        {
            get => 
                this._cardName;
            set
            {
                this.ValidateCardName(value);
                this._cardName = value;
            }
        }

        public string CardNumber
        {
            get => 
                this._cardNumber;
            set
            {
                this.ValidateCardNumber(value);
                this._cardNumber = value;
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

        public SA.Payments.Realex.RealVault.CardType CardType
        {
            get => 
                this._cardType;
            set
            {
                this.ValidateCardType(value);
                this._cardType = value;
            }
        }

        public string Expiry
        {
            get => 
                this._expiry;
            set
            {
                this.ValidateExpiry(value);
                this._expiry = value;
            }
        }

        public string IssueNo
        {
            get => 
                this._issueno;
            set
            {
                this.ValidateIssueNo(value);
                this._issueno = value;
            }
        }

        public string OrderID
        {
            get => 
                this._orderID;
            set
            {
                this.ValidateOrderID(value);
                this._orderID = value;
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

