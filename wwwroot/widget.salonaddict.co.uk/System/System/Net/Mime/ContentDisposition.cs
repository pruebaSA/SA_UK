namespace System.Net.Mime
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Net;
    using System.Net.Mail;
    using System.Text;

    public class ContentDisposition
    {
        private string disposition;
        private string dispositionType;
        private bool isChanged;
        private bool isPersisted;
        private TrackingStringDictionary parameters;

        public ContentDisposition()
        {
            this.isChanged = true;
            this.disposition = "attachment";
            this.ParseValue();
        }

        public ContentDisposition(string disposition)
        {
            if (disposition == null)
            {
                throw new ArgumentNullException("disposition");
            }
            this.isChanged = true;
            this.disposition = disposition;
            this.ParseValue();
        }

        public override bool Equals(object rparam)
        {
            if (rparam == null)
            {
                return false;
            }
            return (string.Compare(this.ToString(), rparam.ToString(), StringComparison.OrdinalIgnoreCase) == 0);
        }

        public override int GetHashCode() => 
            this.ToString().GetHashCode();

        private void ParseValue()
        {
            int offset = 0;
            this.parameters = new TrackingStringDictionary();
            Exception exception = null;
            try
            {
                this.dispositionType = MailBnfHelper.ReadToken(this.disposition, ref offset, null);
                if ((this.dispositionType == null) || (this.dispositionType.Length == 0))
                {
                    exception = new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                }
                if (exception == null)
                {
                    while (MailBnfHelper.SkipCFWS(this.disposition, ref offset))
                    {
                        string str2;
                        if (this.disposition[offset++] != ';')
                        {
                            exception = new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                        }
                        if (!MailBnfHelper.SkipCFWS(this.disposition, ref offset))
                        {
                            goto Label_0198;
                        }
                        string strA = MailBnfHelper.ReadParameterAttribute(this.disposition, ref offset, null);
                        if (this.disposition[offset++] != '=')
                        {
                            exception = new FormatException(SR.GetString("MailHeaderFieldMalformedHeader"));
                            goto Label_0198;
                        }
                        if (!MailBnfHelper.SkipCFWS(this.disposition, ref offset))
                        {
                            str2 = string.Empty;
                        }
                        else if (this.disposition[offset] == '"')
                        {
                            str2 = MailBnfHelper.ReadQuotedString(this.disposition, ref offset, null);
                        }
                        else
                        {
                            str2 = MailBnfHelper.ReadToken(this.disposition, ref offset, null);
                        }
                        if (((strA == null) || (str2 == null)) || ((strA.Length == 0) || (str2.Length == 0)))
                        {
                            exception = new FormatException(SR.GetString("ContentDispositionInvalid"));
                            goto Label_0198;
                        }
                        if (((string.Compare(strA, "creation-date", StringComparison.OrdinalIgnoreCase) == 0) || (string.Compare(strA, "modification-date", StringComparison.OrdinalIgnoreCase) == 0)) || (string.Compare(strA, "read-date", StringComparison.OrdinalIgnoreCase) == 0))
                        {
                            int num2 = 0;
                            MailBnfHelper.ReadDateTime(str2, ref num2);
                        }
                        this.parameters.Add(strA, str2);
                    }
                }
            }
            catch (FormatException)
            {
                throw new FormatException(SR.GetString("ContentDispositionInvalid"));
            }
        Label_0198:
            if (exception != null)
            {
                throw exception;
            }
            this.parameters.IsChanged = false;
        }

        internal void PersistIfNeeded(HeaderCollection headers, bool forcePersist)
        {
            if ((this.IsChanged || !this.isPersisted) || forcePersist)
            {
                headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentDisposition), this.ToString());
                this.isPersisted = true;
            }
        }

        internal void Set(string contentDisposition, HeaderCollection headers)
        {
            this.disposition = contentDisposition;
            this.ParseValue();
            headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentDisposition), this.ToString());
            this.isPersisted = true;
        }

        public override string ToString()
        {
            if (((this.disposition == null) || this.isChanged) || ((this.parameters != null) && this.parameters.IsChanged))
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(this.dispositionType);
                foreach (string str in this.Parameters.Keys)
                {
                    builder.Append("; ");
                    builder.Append(str);
                    builder.Append('=');
                    MailBnfHelper.GetTokenOrQuotedString(this.parameters[str], builder);
                }
                this.disposition = builder.ToString();
                this.isChanged = false;
                this.parameters.IsChanged = false;
                this.isPersisted = false;
            }
            return this.disposition;
        }

        public DateTime CreationDate
        {
            get
            {
                string data = this.Parameters["creation-date"];
                if (data == null)
                {
                    return DateTime.MinValue;
                }
                int offset = 0;
                return MailBnfHelper.ReadDateTime(data, ref offset);
            }
            set
            {
                this.Parameters["creation-date"] = MailBnfHelper.GetDateTimeString(value, null);
            }
        }

        public string DispositionType
        {
            get => 
                this.dispositionType;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (value == string.Empty)
                {
                    throw new ArgumentException(SR.GetString("net_emptystringset"), "value");
                }
                this.isChanged = true;
                this.dispositionType = value;
            }
        }

        public string FileName
        {
            get => 
                this.Parameters["filename"];
            set
            {
                if ((value == null) || (value == string.Empty))
                {
                    this.Parameters.Remove("filename");
                }
                else
                {
                    this.Parameters["filename"] = value;
                }
            }
        }

        public bool Inline
        {
            get => 
                (this.dispositionType == "inline");
            set
            {
                this.isChanged = true;
                if (value)
                {
                    this.dispositionType = "inline";
                }
                else
                {
                    this.dispositionType = "attachment";
                }
            }
        }

        internal bool IsChanged =>
            (this.isChanged || ((this.parameters != null) && this.parameters.IsChanged));

        public DateTime ModificationDate
        {
            get
            {
                string data = this.Parameters["modification-date"];
                if (data == null)
                {
                    return DateTime.MinValue;
                }
                int offset = 0;
                return MailBnfHelper.ReadDateTime(data, ref offset);
            }
            set
            {
                this.Parameters["modification-date"] = MailBnfHelper.GetDateTimeString(value, null);
            }
        }

        public StringDictionary Parameters
        {
            get
            {
                if (this.parameters == null)
                {
                    this.parameters = new TrackingStringDictionary();
                }
                return this.parameters;
            }
        }

        public DateTime ReadDate
        {
            get
            {
                string data = this.Parameters["read-date"];
                if (data == null)
                {
                    return DateTime.MinValue;
                }
                int offset = 0;
                return MailBnfHelper.ReadDateTime(data, ref offset);
            }
            set
            {
                this.Parameters["read-date"] = MailBnfHelper.GetDateTimeString(value, null);
            }
        }

        public long Size
        {
            get
            {
                string s = this.Parameters["size"];
                if (s == null)
                {
                    return -1L;
                }
                return long.Parse(s, CultureInfo.InvariantCulture);
            }
            set
            {
                this.Parameters["size"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}

