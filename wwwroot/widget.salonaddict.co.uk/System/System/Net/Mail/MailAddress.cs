﻿namespace System.Net.Mail
{
    using System;
    using System.Net.Mime;
    using System.Text;

    public class MailAddress
    {
        private string address;
        private string displayName;
        private Encoding displayNameEncoding;
        private string encodedDisplayName;
        private string fullAddress;
        private string host;
        private string userName;

        public MailAddress(string address) : this(address, null, (Encoding) null)
        {
        }

        public MailAddress(string address, string displayName) : this(address, displayName, (Encoding) null)
        {
        }

        public MailAddress(string address, string displayName, Encoding displayNameEncoding)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (address == string.Empty)
            {
                throw new ArgumentException(SR.GetString("net_emptystringcall", new object[] { "address" }), "address");
            }
            this.displayNameEncoding = displayNameEncoding;
            this.displayName = displayName;
            this.ParseValue(address);
            if ((this.displayName != null) && (this.displayName != string.Empty))
            {
                if ((this.displayName[0] == '"') && (this.displayName[this.displayName.Length - 1] == '"'))
                {
                    this.displayName = this.displayName.Substring(1, this.displayName.Length - 2);
                }
                this.displayName = this.displayName.Trim();
            }
            if ((this.displayName != null) && (this.displayName.Length > 0))
            {
                if (!MimeBasePart.IsAscii(this.displayName, false) || (this.displayNameEncoding != null))
                {
                    if (this.displayNameEncoding == null)
                    {
                        this.displayNameEncoding = Encoding.GetEncoding("utf-8");
                    }
                    this.encodedDisplayName = MimeBasePart.EncodeHeaderValue(this.displayName, this.displayNameEncoding, MimeBasePart.ShouldUseBase64Encoding(displayNameEncoding));
                    StringBuilder builder = new StringBuilder();
                    int offset = 0;
                    MailBnfHelper.ReadUnQuotedString(this.encodedDisplayName, ref offset, builder);
                    this.encodedDisplayName = builder.ToString();
                }
                else
                {
                    this.encodedDisplayName = this.displayName;
                }
            }
        }

        internal MailAddress(string address, string encodedDisplayName, uint bogusParam)
        {
            this.encodedDisplayName = encodedDisplayName;
            this.GetParts(address);
        }

        private void CombineParts()
        {
            if ((this.userName != null) && (this.host != null))
            {
                StringBuilder builder = new StringBuilder();
                MailBnfHelper.GetDotAtomOrQuotedString(this.User, builder);
                builder.Append('@');
                MailBnfHelper.GetDotAtomOrDomainLiteral(this.Host, builder);
                this.address = builder.ToString();
            }
        }

        public override bool Equals(object value)
        {
            if (value == null)
            {
                return false;
            }
            return this.ToString().Equals(value.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode() => 
            this.ToString().GetHashCode();

        private void GetParts(string address)
        {
            if (address != null)
            {
                int index = address.IndexOf('@');
                if (index < 0)
                {
                    throw new FormatException(SR.GetString("MailAddressInvalidFormat"));
                }
                this.userName = address.Substring(0, index);
                this.host = address.Substring(index + 1);
            }
        }

        private void ParseValue(string address)
        {
            string str = null;
            int offset = 0;
            MailBnfHelper.SkipFWS(address, ref offset);
            int index = address.IndexOf('"', offset);
            if (index == offset)
            {
                index = address.IndexOf('"', index + 1);
                if (index > offset)
                {
                    int num3 = index + 1;
                    MailBnfHelper.SkipFWS(address, ref num3);
                    if ((address.Length > num3) && (address[num3] != '@'))
                    {
                        str = address.Substring(offset, (index + 1) - offset);
                        address = address.Substring(num3);
                    }
                }
            }
            if (str == null)
            {
                int startIndex = address.IndexOf('<', offset);
                if (startIndex >= offset)
                {
                    str = address.Substring(offset, startIndex - offset);
                    address = address.Substring(startIndex);
                }
            }
            if (str == null)
            {
                index = address.IndexOf('"', offset);
                if (index > offset)
                {
                    str = address.Substring(offset, index - offset);
                    address = address.Substring(index);
                }
            }
            if (this.displayName == null)
            {
                this.displayName = str;
            }
            int num5 = 0;
            address = MailBnfHelper.ReadMailAddress(address, ref num5, out this.encodedDisplayName);
            this.GetParts(address);
        }

        internal string ToEncodedString()
        {
            if (this.fullAddress == null)
            {
                if ((this.encodedDisplayName != null) && (this.encodedDisplayName != string.Empty))
                {
                    StringBuilder builder = new StringBuilder();
                    MailBnfHelper.GetDotAtomOrQuotedString(this.encodedDisplayName, builder);
                    builder.Append(" <");
                    builder.Append(this.Address);
                    builder.Append('>');
                    this.fullAddress = builder.ToString();
                }
                else
                {
                    this.fullAddress = this.Address;
                }
            }
            return this.fullAddress;
        }

        public override string ToString()
        {
            if (this.fullAddress == null)
            {
                if ((this.encodedDisplayName != null) && (this.encodedDisplayName != string.Empty))
                {
                    StringBuilder builder = new StringBuilder();
                    if (this.DisplayName.StartsWith("\"") && this.DisplayName.EndsWith("\""))
                    {
                        builder.Append(this.DisplayName);
                    }
                    else
                    {
                        builder.Append('"');
                        builder.Append(this.DisplayName);
                        builder.Append('"');
                    }
                    builder.Append(" <");
                    builder.Append(this.Address);
                    builder.Append('>');
                    this.fullAddress = builder.ToString();
                }
                else
                {
                    this.fullAddress = this.Address;
                }
            }
            return this.fullAddress;
        }

        public string Address
        {
            get
            {
                if (this.address == null)
                {
                    this.CombineParts();
                }
                return this.address;
            }
        }

        public string DisplayName
        {
            get
            {
                if (this.displayName == null)
                {
                    if ((this.encodedDisplayName != null) && (this.encodedDisplayName.Length > 0))
                    {
                        this.displayName = MimeBasePart.DecodeHeaderValue(this.encodedDisplayName);
                    }
                    else
                    {
                        this.displayName = string.Empty;
                    }
                }
                return this.displayName;
            }
        }

        public string Host =>
            this.host;

        internal string SmtpAddress
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append('<');
                builder.Append(this.Address);
                builder.Append('>');
                return builder.ToString();
            }
        }

        public string User =>
            this.userName;
    }
}

