namespace SA.Payments.Realex.RealVault
{
    using System;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    public class ReceiptInRequest : RequestBase
    {
        private string _amount;
        private bool _autoSettle;
        private string _billingCountry;
        private string _billingZipPostalCode;
        private string _cardRef;
        private string _comment1;
        private string _comment2;
        private string _currencyCode;
        private string _customerNo;
        private string _cvnNumber;
        private string _orderID;
        private string _payerRef;
        private string _productID;
        private string _shippingCountry;
        private string _shippingZipPostalCode;
        private XElement _supplementaryData;
        private string _varRef;

        public ReceiptInRequest(string orderID, string payerRef, string cardRef, string currencyCode, string amount) : this(orderID, payerRef, cardRef, currencyCode, amount, false)
        {
        }

        public ReceiptInRequest(string orderID, string payerRef, string cardRef, string currencyCode, string amount, bool autoSettle) : this(orderID, payerRef, cardRef, currencyCode, amount, autoSettle, new SettingsProviderConfig())
        {
        }

        public ReceiptInRequest(string orderID, string payerRef, string cardRef, string currencyCode, string amount, bool autoSettle, ISettingsProvider settings) : base(settings)
        {
            this.OrderID = orderID;
            this.PayerRef = payerRef;
            this.CardRef = cardRef;
            this.CurrencyCode = currencyCode;
            this.Amount = amount;
            this.AutoSettle = autoSettle;
        }

        public override XElement ConvertToXML()
        {
            ISettingsProvider settings = base.Settings;
            XElement element = new XElement("request");
            element.Add(new XAttribute("type", "receipt-in"));
            element.Add(new XAttribute("timestamp", base.TimeStamp));
            element.Add(new XElement("merchantid", settings.MerchantID));
            element.Add(new XElement("account", settings.Account));
            element.Add(new XElement("orderid", this._orderID));
            element.Add(new XElement("paymentdata", new XElement("cvn", new XElement("number", this._cvnNumber ?? string.Empty))));
            element.Add(new XElement("amount", new object[] { new XAttribute("currency", this._currencyCode), this._amount }));
            element.Add(new XElement("payerref", this._payerRef));
            element.Add(new XElement("paymentmethod", this._cardRef));
            element.Add(new XElement("autosettle", new XAttribute("flag", this._autoSettle ? "1" : "0")));
            string content = Helper.GenerateSignatureSHA1(settings.SharedSecret, new string[] { base.TimeStamp, settings.MerchantID, this._orderID, this._amount, this._currencyCode, this._payerRef });
            element.Add(new XElement("sha1hash", content));
            object[] objArray2 = new object[2];
            object[] objArray3 = new object[] { this._comment1 ?? string.Empty, new XAttribute("id", "1") };
            objArray2[0] = new XElement("comment", objArray3);
            object[] objArray4 = new object[] { this._comment2 ?? string.Empty, new XAttribute("id", "2") };
            objArray2[1] = new XElement("comment", objArray4);
            element.Add(new XElement("comments", objArray2));
            element.Add(new XElement("recurring"));
            object[] objArray5 = new object[5];
            object[] objArray6 = new object[] { new XAttribute("type", "billing"), new XElement("code", this._billingZipPostalCode ?? string.Empty), new XElement("country", this._billingCountry ?? string.Empty) };
            objArray5[0] = new XElement("address", objArray6);
            object[] objArray7 = new object[] { new XAttribute("type", "shipping"), new XElement("code", this._shippingZipPostalCode ?? string.Empty), new XElement("country", this._shippingCountry ?? string.Empty) };
            objArray5[1] = new XElement("address", objArray7);
            objArray5[2] = new XElement("custnum", this._customerNo ?? string.Empty);
            objArray5[3] = new XElement("varref", this._varRef ?? string.Empty);
            objArray5[4] = new XElement("prodid", this._productID ?? string.Empty);
            element.Add(new XElement("tssinfo", objArray5));
            XElement element2 = new XElement("supplementarydata");
            if (this._supplementaryData != null)
            {
                element2.Add(this._supplementaryData);
            }
            element.Add(element2);
            return element;
        }

        protected virtual void ValidateAmount(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new RealVaultException("Amount cannot be null or empty.");
            }
            if (value.Length < 2)
            {
                throw new RealVaultException("Amount must be at least 2 characters in length.");
            }
            if (value.Length > 11)
            {
                throw new RealVaultException("Amount must be less than or equal to 11 characters in length.");
            }
            if (!Regex.IsMatch(value, "^[0-9]*$"))
            {
                throw new RealVaultException("Amount can only contain the characters: '0-9'.");
            }
        }

        protected virtual void ValidateAutoSettle(bool value)
        {
        }

        protected virtual void ValidateBillingCountry(string value)
        {
        }

        protected virtual void ValidateBillingZipPostalCode(string value)
        {
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
                throw new RealVaultException("Currency code cannot be null or empty.");
            }
            if (value.Length != 3)
            {
                throw new RealVaultException("Currency code must be 3 characters in length.");
            }
            if (!Regex.IsMatch(value, "^[A-Z]*$"))
            {
                throw new RealVaultException("Currency code can only contain the characters: 'A-Z'.");
            }
        }

        protected virtual void ValidateCustomerNumber(string value)
        {
            if ((value != null) && (value.Length > 50))
            {
                throw new RealVaultException("Customer number must be less than 50 characters in length.");
            }
        }

        protected virtual void ValidateNumberCVN(string value)
        {
            if (value != null)
            {
                if (value.Length < 3)
                {
                    throw new RealVaultException("CVN must be greater than or equal to 3 characters in length.");
                }
                if (value.Length > 4)
                {
                    throw new RealVaultException("CVN must be less than or equal to 4 characters in length.");
                }
                if (!Regex.IsMatch(value, "^[0-9]*$"))
                {
                    throw new RealVaultException("CVN can only contain the characters: '0-9'.");
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

        protected virtual void ValidateProductID(string value)
        {
            if ((value != null) && (value.Length > 50))
            {
                throw new RealVaultException("Product identifier must be less than 50 characters in length.");
            }
        }

        protected virtual void ValidateShippingCountry(string value)
        {
        }

        protected virtual void ValidateShippingZipPostalCode(string value)
        {
        }

        protected virtual void ValidateSupplementaryData(XElement value)
        {
        }

        protected virtual void ValidateVarRef(string value)
        {
            if ((value != null) && (value.Length > 50))
            {
                throw new RealVaultException("Variable reference must be less than 50 characters in length.");
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

        public string BillingZipPostalCode
        {
            get => 
                this._billingZipPostalCode;
            set
            {
                this.ValidateBillingZipPostalCode(value);
                this._billingZipPostalCode = value;
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
                this._cvnNumber;
            set
            {
                this.ValidateNumberCVN(value);
                this._cvnNumber = value;
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

        public string ShippingZipPostalCode
        {
            get => 
                this._shippingZipPostalCode;
            set
            {
                this.ValidateShippingZipPostalCode(value);
                this._shippingZipPostalCode = value;
            }
        }

        public XElement SupplementaryData
        {
            get => 
                this._supplementaryData;
            set
            {
                this.ValidateSupplementaryData(value);
                this._supplementaryData = value;
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

