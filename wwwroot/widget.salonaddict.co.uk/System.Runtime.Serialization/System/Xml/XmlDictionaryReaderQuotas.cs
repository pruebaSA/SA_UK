﻿namespace System.Xml
{
    using System;
    using System.Runtime.Serialization;

    public sealed class XmlDictionaryReaderQuotas
    {
        private const int DefaultMaxArrayLength = 0x4000;
        private const int DefaultMaxBytesPerRead = 0x1000;
        private const int DefaultMaxDepth = 0x20;
        private const int DefaultMaxNameTableCharCount = 0x4000;
        private const int DefaultMaxStringContentLength = 0x2000;
        private static XmlDictionaryReaderQuotas defaultQuota = new XmlDictionaryReaderQuotas(0x20, 0x2000, 0x4000, 0x1000, 0x4000);
        private int maxArrayLength;
        private int maxBytesPerRead;
        private int maxDepth;
        private int maxNameTableCharCount;
        private static XmlDictionaryReaderQuotas maxQuota = new XmlDictionaryReaderQuotas(0x7fffffff, 0x7fffffff, 0x7fffffff, 0x7fffffff, 0x7fffffff);
        private int maxStringContentLength;
        private bool readOnly;

        public XmlDictionaryReaderQuotas()
        {
            defaultQuota.CopyTo(this);
        }

        private XmlDictionaryReaderQuotas(int maxDepth, int maxStringContentLength, int maxArrayLength, int maxBytesPerRead, int maxNameTableCharCount)
        {
            this.maxDepth = maxDepth;
            this.maxStringContentLength = maxStringContentLength;
            this.maxArrayLength = maxArrayLength;
            this.maxBytesPerRead = maxBytesPerRead;
            this.maxNameTableCharCount = maxNameTableCharCount;
            this.MakeReadOnly();
        }

        public void CopyTo(XmlDictionaryReaderQuotas quotas)
        {
            if (quotas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("quotas"));
            }
            if (quotas.readOnly)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.Runtime.Serialization.SR.GetString("QuotaCopyReadOnly")));
            }
            this.InternalCopyTo(quotas);
        }

        internal void InternalCopyTo(XmlDictionaryReaderQuotas quotas)
        {
            quotas.maxStringContentLength = this.maxStringContentLength;
            quotas.maxArrayLength = this.maxArrayLength;
            quotas.maxDepth = this.MaxDepth;
            quotas.maxNameTableCharCount = this.maxNameTableCharCount;
            quotas.maxBytesPerRead = this.maxBytesPerRead;
        }

        internal void MakeReadOnly()
        {
            this.readOnly = true;
        }

        public static XmlDictionaryReaderQuotas Max =>
            maxQuota;

        public int MaxArrayLength
        {
            get => 
                this.maxArrayLength;
            set
            {
                if (this.readOnly)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.Runtime.Serialization.SR.GetString("QuotaIsReadOnly", new object[] { "MaxArrayLength" })));
                }
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("QuotaMustBePositive"), "value"));
                }
                this.maxArrayLength = value;
            }
        }

        public int MaxBytesPerRead
        {
            get => 
                this.maxBytesPerRead;
            set
            {
                if (this.readOnly)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.Runtime.Serialization.SR.GetString("QuotaIsReadOnly", new object[] { "MaxBytesPerRead" })));
                }
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("QuotaMustBePositive"), "value"));
                }
                this.maxBytesPerRead = value;
            }
        }

        public int MaxDepth
        {
            get => 
                this.maxDepth;
            set
            {
                if (this.readOnly)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.Runtime.Serialization.SR.GetString("QuotaIsReadOnly", new object[] { "MaxDepth" })));
                }
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("QuotaMustBePositive"), "value"));
                }
                this.maxDepth = value;
            }
        }

        public int MaxNameTableCharCount
        {
            get => 
                this.maxNameTableCharCount;
            set
            {
                if (this.readOnly)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.Runtime.Serialization.SR.GetString("QuotaIsReadOnly", new object[] { "MaxNameTableCharCount" })));
                }
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("QuotaMustBePositive"), "value"));
                }
                this.maxNameTableCharCount = value;
            }
        }

        public int MaxStringContentLength
        {
            get => 
                this.maxStringContentLength;
            set
            {
                if (this.readOnly)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.Runtime.Serialization.SR.GetString("QuotaIsReadOnly", new object[] { "MaxStringContentLength" })));
                }
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("QuotaMustBePositive"), "value"));
                }
                this.maxStringContentLength = value;
            }
        }
    }
}

