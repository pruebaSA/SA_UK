namespace SA.Payments.Realex.RealVault
{
    using System;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    public class PayerCreateRequest : RequestBase
    {
        private string _addressLine1;
        private string _addressLine2;
        private string _addressLine3;
        private string _cityTown;
        private string _comment1;
        private string _comment2;
        private string _company;
        private string _country;
        private string _countryCode;
        private string _county;
        private string _email;
        private string _fax;
        private string _firstName;
        private string _homeNumber;
        private string _lastName;
        private string _mobile;
        private string _orderID;
        private string _payerComment1;
        private string _payerComment2;
        private string _payerRef;
        private string _payerType;
        private string _title;
        private string _workNumber;
        private string _zipPostalCode;

        public PayerCreateRequest(string payerType, string payerRef, string firstName, string lastName) : this(payerType, payerRef, firstName, lastName, new SettingsProviderConfig())
        {
        }

        public PayerCreateRequest(string payerType, string payerRef, string firstName, string lastName, ISettingsProvider settings) : base(settings)
        {
            this.PayerType = payerType;
            this.PayerRef = payerRef;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public override XElement ConvertToXML()
        {
            ISettingsProvider settings = base.Settings;
            XElement element = new XElement("request");
            element.Add(new XAttribute("type", "payer-new"));
            element.Add(new XAttribute("timestamp", base.TimeStamp));
            element.Add(new XElement("merchantid", settings.MerchantID));
            element.Add(new XElement("orderid", this._orderID ?? string.Empty));
            object[] content = new object[10];
            content[0] = new XAttribute("type", this._payerType);
            content[1] = new XAttribute("ref", this._payerRef);
            content[2] = new XElement("title", this._title ?? string.Empty);
            content[3] = new XElement("firstname", this._firstName ?? string.Empty);
            content[4] = new XElement("surname", this._lastName ?? string.Empty);
            content[5] = new XElement("company", this._company ?? string.Empty);
            object[] objArray2 = new object[7];
            objArray2[0] = new XElement("line1", this._addressLine1 ?? string.Empty);
            objArray2[1] = new XElement("line2", this._addressLine2 ?? string.Empty);
            objArray2[2] = new XElement("line3", this._addressLine3 ?? string.Empty);
            objArray2[3] = new XElement("city", this._cityTown ?? string.Empty);
            objArray2[4] = new XElement("county", this._county ?? string.Empty);
            objArray2[5] = new XElement("postcode", this._zipPostalCode ?? string.Empty);
            object[] objArray3 = new object[] { this._country ?? string.Empty, new XAttribute("code", this._countryCode ?? string.Empty) };
            objArray2[6] = new XElement("country", objArray3);
            content[6] = new XElement("address", objArray2);
            object[] objArray4 = new object[] { new XElement("home", this._homeNumber ?? string.Empty), new XElement("work", this._workNumber ?? string.Empty), new XElement("fax", this._fax ?? string.Empty), new XElement("mobile", this._mobile ?? string.Empty) };
            content[7] = new XElement("phonenumbers", objArray4);
            content[8] = new XElement("email", this._email ?? string.Empty);
            object[] objArray5 = new object[2];
            object[] objArray6 = new object[] { this._comment1 ?? string.Empty, new XAttribute("id", "1") };
            objArray5[0] = new XElement("comment", objArray6);
            object[] objArray7 = new object[] { this._comment2 ?? string.Empty, new XAttribute("id", "2") };
            objArray5[1] = new XElement("comment", objArray7);
            content[9] = new XElement("comments", objArray5);
            element.Add(new XElement("payer", content));
            string str = Helper.GenerateSignatureSHA1(settings.SharedSecret, new string[] { base.TimeStamp, settings.MerchantID, this._orderID, string.Empty, string.Empty, this._payerRef });
            element.Add(new XElement("sha1hash", str));
            object[] objArray8 = new object[2];
            object[] objArray9 = new object[] { this._payerComment1 ?? string.Empty, new XAttribute("id", "1") };
            objArray8[0] = new XElement("comment", objArray9);
            object[] objArray10 = new object[] { this._payerComment2 ?? string.Empty, new XAttribute("id", "2") };
            objArray8[1] = new XElement("comment", objArray10);
            element.Add(new XElement("comments", objArray8));
            return element;
        }

        protected virtual void ValidateAddressLine1(string value)
        {
            if (!string.IsNullOrEmpty(value) && (value.Length > 50))
            {
                throw new RealVaultException("Address line 1 must be less than or equal to 50 characters in length.");
            }
        }

        protected virtual void ValidateAddressLine2(string value)
        {
            if (!string.IsNullOrEmpty(value) && (value.Length > 50))
            {
                throw new RealVaultException("Address line 2 must be less than or equal to 50 characters in length.");
            }
        }

        protected virtual void ValidateAddressLine3(string value)
        {
            if (!string.IsNullOrEmpty(value) && (value.Length > 50))
            {
                throw new RealVaultException("Address line 3 must be less than or equal to 50 characters in length.");
            }
        }

        protected virtual void ValidateCityTown(string value)
        {
            if (!string.IsNullOrEmpty(value) && (value.Length > 20))
            {
                throw new RealVaultException("City must be less than or equal to 20 characters in length.");
            }
        }

        protected virtual void ValidateComment1(string value)
        {
        }

        protected virtual void ValidateComment2(string value)
        {
        }

        protected virtual void ValidateCompany(string value)
        {
            if (!string.IsNullOrEmpty(value) && (value.Length > 50))
            {
                throw new RealVaultException("Company must be less than or equal to 50 characters in length.");
            }
        }

        protected virtual void ValidateCountry(string value)
        {
        }

        protected virtual void ValidateCountryCode(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length != 2)
                {
                    throw new RealVaultException("Country code must be 2 characters in length.");
                }
                if (!Regex.IsMatch(value, "^[A-Z]*$"))
                {
                    throw new RealVaultException("Country code can only contain the characters: 'A-Z'.");
                }
            }
        }

        protected virtual void ValidateCounty(string value)
        {
            if (!string.IsNullOrEmpty(value) && (value.Length > 20))
            {
                throw new RealVaultException("County must be less than or equal to 20 characters in length.");
            }
        }

        protected virtual void ValidateEmail(string value)
        {
        }

        protected virtual void ValidateFax(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length > 20)
                {
                    throw new RealVaultException("Fax must less than or equal to 20 characters in length.");
                }
                if (!Regex.IsMatch(value, @"^[0-9\+\s\-]*$"))
                {
                    throw new RealVaultException("Fax can only contain the characters: '0-9', '+', '-' and 'space'.");
                }
            }
        }

        protected virtual void ValidateFirstName(string value)
        {
            if ((value != null) && (value.Length > 30))
            {
                throw new RealVaultException("First name must be less than or equal to 30 characters in length.");
            }
        }

        protected virtual void ValidateHomeNumber(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length > 20)
                {
                    throw new RealVaultException("Home number must less than or equal to 20 characters in length.");
                }
                if (!Regex.IsMatch(value, @"^[0-9\+\s\-]*$"))
                {
                    throw new RealVaultException("Home number can only contain the characters: '0-9', '+', '-' and 'space'.");
                }
            }
        }

        protected virtual void ValidateLastName(string value)
        {
            if ((value != null) && (value.Length > 30))
            {
                throw new RealVaultException("Last name must be less than or equal to 30 characters in length.");
            }
        }

        protected virtual void ValidateMobile(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length > 20)
                {
                    throw new RealVaultException("Mobile must less than or equal to 20 characters in length.");
                }
                if (!Regex.IsMatch(value, @"^[0-9\+\s\-]*$"))
                {
                    throw new RealVaultException("Mobile can only contain the characters: '0-9', '+', '-' and 'space'.");
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

        protected virtual void ValidatePayerComment1(string value)
        {
        }

        protected virtual void ValidatePayerComment2(string value)
        {
        }

        protected virtual void ValidatePayerRef(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new RealVaultException("Payer reference cannot be null or empty.");
            }
            if (value.Length > 50)
            {
                throw new RealVaultException("Payer reference must be less than or equal to 50 characters in length.");
            }
            if (!Regex.IsMatch(value, "^[a-zA-Z0-9_]*$"))
            {
                throw new RealVaultException("Payer reference can only contain the characters: 'a-z', 'A-Z', '0-9', and '_'.");
            }
        }

        protected virtual void ValidatePayerType(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new RealVaultException("Payer type cannot be null or empty.");
            }
            if (value.Length > 20)
            {
                throw new RealVaultException("Payer type must be less than or equal to 20 characters in length.");
            }
            if (!Regex.IsMatch(value, "^[a-zA-Z]*$"))
            {
                throw new RealVaultException("Payer type can only contain the characters: 'a-z' and 'A-Z'.");
            }
        }

        protected virtual void ValidateTitle(string value)
        {
            if (value != null)
            {
                if (value.Length > 10)
                {
                    throw new RealVaultException("Title must be less than or equal to 10 characters in length.");
                }
                if (!Regex.IsMatch(value, "^[a-zA-Z]*$"))
                {
                    throw new RealVaultException("Title can only contain the characters: 'a-z' and 'A-Z'.");
                }
            }
        }

        protected virtual void ValidateWorkNumber(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length > 20)
                {
                    throw new RealVaultException("Work number must less than or equal to 20 characters in length.");
                }
                if (!Regex.IsMatch(value, @"^[0-9\+\s\-]*$"))
                {
                    throw new RealVaultException("Work number can only contain the characters: '0-9', '+', '-' and 'space'.");
                }
            }
        }

        protected virtual void ValidateZipPostalCode(string value)
        {
            string.IsNullOrEmpty(value);
        }

        public string AddressLine1
        {
            get => 
                this._addressLine1;
            set
            {
                this.ValidateAddressLine1(value);
                this._addressLine1 = value;
            }
        }

        public string AddressLine2
        {
            get => 
                this._addressLine2;
            set
            {
                this.ValidateAddressLine2(value);
                this._addressLine2 = value;
            }
        }

        public string AddressLine3
        {
            get => 
                this._addressLine3;
            set
            {
                this.ValidateAddressLine3(value);
                this._addressLine3 = value;
            }
        }

        public string CityTown
        {
            get => 
                this._cityTown;
            set
            {
                this.ValidateCityTown(value);
                this._cityTown = value;
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

        public string Company
        {
            get => 
                this._company;
            set
            {
                this.ValidateCompany(value);
                this._company = value;
            }
        }

        public string Country
        {
            get => 
                this._country;
            set
            {
                this.ValidateCountry(value);
                this._country = value;
            }
        }

        public string CountryCode
        {
            get => 
                this._countryCode;
            set
            {
                this.ValidateCountryCode(value);
                this._countryCode = value;
            }
        }

        public string County
        {
            get => 
                this._county;
            set
            {
                this.ValidateCounty(value);
                this._county = value;
            }
        }

        public string Email
        {
            get => 
                this._email;
            set
            {
                this.ValidateEmail(value);
                this._email = value;
            }
        }

        public string Fax
        {
            get => 
                this._fax;
            set
            {
                this.ValidateFax(value);
                this._fax = value;
            }
        }

        public string FirstName
        {
            get => 
                this._firstName;
            set
            {
                this.ValidateFirstName(value);
                this._firstName = value;
            }
        }

        public string HomeNumber
        {
            get => 
                this._homeNumber;
            set
            {
                this.ValidateHomeNumber(value);
                this._homeNumber = value;
            }
        }

        public string LastName
        {
            get => 
                this._lastName;
            set
            {
                this.ValidateLastName(value);
                this._lastName = value;
            }
        }

        public string Mobile
        {
            get => 
                this._mobile;
            set
            {
                this.ValidateMobile(value);
                this._mobile = value;
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

        public string PayerComment1
        {
            get => 
                this._payerComment1;
            set
            {
                this.ValidatePayerComment1(value);
                this._payerComment1 = value;
            }
        }

        public string PayerComment2
        {
            get => 
                this._payerComment2;
            set
            {
                this.ValidatePayerComment2(value);
                this._payerComment2 = value;
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

        public string PayerType
        {
            get => 
                this._payerType;
            set
            {
                this.ValidatePayerType(value);
                this._payerType = value;
            }
        }

        public string Title
        {
            get => 
                this._title;
            set
            {
                this.ValidateTitle(value);
                this._title = value;
            }
        }

        public string WorkNumber
        {
            get => 
                this._workNumber;
            set
            {
                this.ValidateWorkNumber(value);
                this._workNumber = value;
            }
        }

        public string ZipPostalCode
        {
            get => 
                this._zipPostalCode;
            set
            {
                this.ValidateZipPostalCode(value);
                this._zipPostalCode = value;
            }
        }
    }
}

