namespace SA.Payments.Realex.RealAuth
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    public class AuthRequest : RequestBase
    {
        private string _amount;
        private bool _autoSettle;
        private string _billingCountry;
        private string _billingZipPostalCode;
        private string _cardName;
        private string _cardNumber;
        private SA.Payments.Realex.RealAuth.CardType _cardType;
        private string _comment1;
        private string _comment2;
        private string _currencyCode;
        private string _customerIP;
        private string _customerNo;
        private string _cvnnum;
        private string _expiry;
        private string _issueno;
        private string _orderID;
        private CVNPresenceIndicator _presind;
        private string _productID;
        private string _shippingCountry;
        private string _shippingZipPostalCode;
        private string _varRef;

        public AuthRequest(string orderID, string amount, string currencyCode, SA.Payments.Realex.RealAuth.CardType cardType, string cardName, string cardNumber, string expiry) : this(orderID, amount, currencyCode, cardType, cardName, cardNumber, expiry, false)
        {
        }

        public AuthRequest(string orderID, string amount, string currencyCode, SA.Payments.Realex.RealAuth.CardType cardType, string cardName, string cardNumber, string expiry, bool autoSettle) : this(orderID, amount, currencyCode, cardType, cardName, cardNumber, expiry, autoSettle, new SettingsProviderConfig())
        {
        }

        public AuthRequest(string orderID, string amount, string currencyCode, SA.Payments.Realex.RealAuth.CardType cardType, string cardName, string cardNumber, string expiry, bool autoSettle, ISettingsProvider settings) : base(settings)
        {
            this.OrderID = orderID;
            this.Amount = amount;
            this.CurrencyCode = currencyCode;
            this.CardType = cardType;
            this.CardName = cardName;
            this.CardNumber = cardNumber;
            this.Expiry = expiry;
            this.AutoSettle = autoSettle;
            this.PresenceIndicator = CVNPresenceIndicator.NotRequested;
        }

        public override XElement ConvertToXML()
        {
            ISettingsProvider settings = base.Settings;
            XElement element = new XElement("request");
            element.Add(new XAttribute("timestamp", base.TimeStamp));
            element.Add(new XAttribute("type", "auth"));
            element.Add(new XElement("merchantid", settings.MerchantID));
            element.Add(new XElement("account", settings.Account));
            element.Add(new XElement("orderid", this._orderID));
            element.Add(new XElement("amount", new object[] { new XAttribute("currency", this._currencyCode), this._amount }));
            Func<SA.Payments.Realex.RealAuth.CardType, string> func = delegate (SA.Payments.Realex.RealAuth.CardType cardType) {
                switch (cardType)
                {
                    case SA.Payments.Realex.RealAuth.CardType.Visa:
                        return "VISA";

                    case SA.Payments.Realex.RealAuth.CardType.MasterCard:
                        return "MC";

                    case SA.Payments.Realex.RealAuth.CardType.Switch:
                        return "SWITCH";

                    case SA.Payments.Realex.RealAuth.CardType.Amex:
                        return "AMEX";

                    case SA.Payments.Realex.RealAuth.CardType.Laser:
                        return "LASER";

                    case SA.Payments.Realex.RealAuth.CardType.Diners:
                        return "DINERS";
                }
                throw new RealAuthException("Invalid card type.");
            };
            XElement content = new XElement("card");
            object[] objArray2 = new object[] { new XElement("number", this._cardNumber), new XElement("expdate", this._expiry), new XElement("chname", this._cardName), new XElement("type", func(this._cardType)), new XElement("issueno", this._issueno ?? string.Empty) };
            content.Add(objArray2);
            if (!string.IsNullOrEmpty(this._cvnnum))
            {
                object[] objArray3 = new object[] { new XElement("number", this._cvnnum ?? string.Empty), new XElement("presind", ((int) this._presind).ToString()) };
                content.Add(new XElement("cvn", objArray3));
            }
            element.Add(content);
            element.Add(new XElement("autosettle", new XAttribute("flag", this._autoSettle ? "1" : "0")));
            object[] objArray4 = new object[2];
            object[] objArray5 = new object[] { this._comment1 ?? string.Empty, new XAttribute("id", "1") };
            objArray4[0] = new XElement("comment", objArray5);
            object[] objArray6 = new object[] { this._comment2 ?? string.Empty, new XAttribute("id", "2") };
            objArray4[1] = new XElement("comment", objArray6);
            element.Add(new XElement("comments", objArray4));
            object[] objArray7 = new object[6];
            objArray7[0] = new XElement("custnum", this._customerNo ?? string.Empty);
            objArray7[1] = new XElement("prodid", this._productID ?? string.Empty);
            objArray7[2] = new XElement("varref", this._varRef ?? string.Empty);
            objArray7[3] = new XElement("custipaddress", this._customerIP ?? string.Empty);
            object[] objArray8 = new object[] { new XAttribute("type", "billing"), new XElement("code", this._billingZipPostalCode ?? string.Empty), new XElement("country", this._billingCountry ?? string.Empty) };
            objArray7[4] = new XElement("address", objArray8);
            object[] objArray9 = new object[] { new XAttribute("type", "shipping"), new XElement("code", this._shippingZipPostalCode ?? string.Empty), new XElement("country", this._shippingCountry ?? string.Empty) };
            objArray7[5] = new XElement("address", objArray9);
            element.Add(new XElement("tssinfo", objArray7));
            string str = Helper.GenerateSignatureSHA1(settings.SharedSecret, new string[] { base.TimeStamp, settings.MerchantID, this._orderID, this._amount, this._currencyCode, this._cardNumber });
            element.Add(new XElement("sha1hash", str));
            return element;
        }

        protected virtual void ValidateAmount(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new RealAuthException("Amount cannot be null or empty.");
            }
            if (value.Length < 2)
            {
                throw new RealAuthException("Amount must be at least 2 characters in length.");
            }
            if (value.Length > 11)
            {
                throw new RealAuthException("Amount must be less than or equal to 11 characters in length.");
            }
            if (!Regex.IsMatch(value, "^[0-9]*$"))
            {
                throw new RealAuthException("Amount can only contain the characters: '0-9'.");
            }
        }

        protected virtual void ValidateAutoSettle(bool value)
        {
        }

        protected virtual void ValidateBillingCountry(string value)
        {
        }

        protected virtual void ValidateBillingPostalCode(string value)
        {
        }

        protected virtual void ValidateCardName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new RealAuthException("Card name cannot be null or empty.");
            }
            if (value.Length > 100)
            {
                throw new RealAuthException("Card name must be less than 100 characters in length.");
            }
        }

        protected virtual void ValidateCardNumber(string value)
        {
            if (!Helper.IsValidCardNumber(value))
            {
                throw new RealAuthException("Invalid card number.");
            }
        }

        protected virtual void ValidateCardType(SA.Payments.Realex.RealAuth.CardType value)
        {
        }

        protected virtual void ValidateComment1(string value)
        {
        }

        protected virtual void ValidateComment2(string value)
        {
        }

        protected virtual void ValidateCurrencyCode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new RealAuthException("Currency code cannot be null or empty.");
            }
            if (value.Length != 3)
            {
                throw new RealAuthException("Currency code must be 3 characters in length.");
            }
            if (!Regex.IsMatch(value, "^[A-Z]*$"))
            {
                throw new RealAuthException("Currency code can only contain the characters: 'A-Z'.");
            }
        }

        protected virtual void ValidateCustomerIP(string value)
        {
        }

        protected virtual void ValidateCustomerNumber(string value)
        {
            if ((value != null) && (value.Length > 50))
            {
                throw new RealAuthException("Customer number must be less than 50 characters in length.");
            }
        }

        protected virtual void ValidateCVNPresenceIndicator(CVNPresenceIndicator value)
        {
        }

        protected virtual void ValidateExpiry(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new RealAuthException("Expiry cannot be null or empty.");
            }
            if (value.Length != 4)
            {
                throw new RealAuthException("Expiry must be in the format of MMyy");
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
                throw new RealAuthException("Expiry is invalid.");
            }
        }

        protected virtual void ValidateIssueNumber(string value)
        {
            if (value != null)
            {
                if (value.Length > 3)
                {
                    throw new RealAuthException("Issue number must be less than 3 characters in length.");
                }
                if (!Regex.IsMatch(value, "^[0-9]*$"))
                {
                    throw new RealAuthException("Issue number can only contain the characters: '0-9'.");
                }
            }
        }

        protected virtual void ValidateNumberCVN(string value)
        {
            if (value != null)
            {
                if (value.Length < 3)
                {
                    throw new RealAuthException("CVN must be greater than or equal to 3 characters in length.");
                }
                if (value.Length > 4)
                {
                    throw new RealAuthException("CVN must be less than or equal to 4 characters in length.");
                }
                if (!Regex.IsMatch(value, "^[0-9]*$"))
                {
                    throw new RealAuthException("CVN can only contain the characters: '0-9'.");
                }
            }
        }

        protected virtual void ValidateOrderID(string value)
        {
            if (value != null)
            {
                if (value.Length > 40)
                {
                    throw new RealAuthException("Order identifier must be less than 40 characters in length.");
                }
                if (!Regex.IsMatch(value, @"^[a-zA-Z0-9\-_]*$"))
                {
                    throw new RealAuthException("Order identifier can only contain the characters: 'a-z', 'A-Z', '0-9', '-', and '_'.");
                }
            }
        }

        protected virtual void ValidateProductID(string value)
        {
            if ((value != null) && (value.Length > 50))
            {
                throw new RealAuthException("Product identifier must be less than 50 characters in length.");
            }
        }

        protected virtual void ValidateShippingCountry(string value)
        {
        }

        protected virtual void ValidateShippingPostalCode(string value)
        {
        }

        protected virtual void ValidateVarRef(string value)
        {
            if ((value != null) && (value.Length > 50))
            {
                throw new RealAuthException("Variable reference must be less than 50 characters in length.");
            }
        }

        public string Amount
        {
            get => 
                this._amount;
            set
            {
                this.ValidateAmount(value);
                this._amount = value;
            }
        }

        public bool AutoSettle
        {
            get => 
                this._autoSettle;
            set
            {
                this.ValidateAutoSettle(value);
                this._autoSettle = value;
            }
        }

        public string BillingCountry
        {
            get => 
                this._billingCountry;
            set
            {
                this.ValidateBillingCountry(value);
                this._billingCountry = value;
            }
        }

        public string BillingPostalCode
        {
            get => 
                this._billingZipPostalCode;
            set
            {
                this.ValidateBillingPostalCode(value);
                this._billingZipPostalCode = value;
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
                this._cardName;
            set
            {
                this.ValidateCardNumber(value);
                this._cardNumber = value;
            }
        }

        public SA.Payments.Realex.RealAuth.CardType CardType
        {
            get => 
                this._cardType;
            set
            {
                this.ValidateCardType(value);
                this._cardType = value;
            }
        }

        public string Comment1
        {
            get => 
                this._comment1;
            set
            {
                this.ValidateComment1(value);
                this._comment1 = value;
            }
        }

        public string Comment2
        {
            get => 
                this._comment2;
            set
            {
                this.ValidateComment2(value);
                this._comment2 = value;
            }
        }

        public string CurrencyCode
        {
            get => 
                this._currencyCode;
            set
            {
                this.ValidateCurrencyCode(value);
                this._currencyCode = value;
            }
        }

        public string CustomerIP
        {
            get => 
                this._customerIP;
            set
            {
                this.ValidateCustomerIP(value);
                this._customerIP = value;
            }
        }

        public string CustomerNumber
        {
            get => 
                this._customerNo;
            set
            {
                this.ValidateCustomerNumber(value);
                this._customerNo = value;
            }
        }

        public string CVNNumber
        {
            get => 
                this._cvnnum;
            set
            {
                this.ValidateNumberCVN(value);
                this._cvnnum = value;
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

        public string IssueNumber
        {
            get => 
                this._issueno;
            set
            {
                this.ValidateIssueNumber(value);
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

        public CVNPresenceIndicator PresenceIndicator
        {
            get => 
                this._presind;
            set
            {
                this.ValidateCVNPresenceIndicator(value);
                this._presind = value;
            }
        }

        public string ProductID
        {
            get => 
                this._productID;
            set
            {
                this.ValidateProductID(value);
                this._productID = value;
            }
        }

        public string ShippingCountry
        {
            get => 
                this._shippingCountry;
            set
            {
                this.ValidateShippingCountry(value);
                this._shippingCountry = value;
            }
        }

        public string ShippingPostalCode
        {
            get => 
                this._shippingZipPostalCode;
            set
            {
                this.ValidateShippingPostalCode(value);
                this._shippingZipPostalCode = value;
            }
        }

        public string VarRef
        {
            get => 
                this._varRef;
            set
            {
                this.ValidateVarRef(value);
                this._varRef = value;
            }
        }
    }
}

