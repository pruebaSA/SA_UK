namespace System.IO
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    [Serializable, ComVisible(true)]
    public class StringWriter : TextWriter
    {
        private bool _isOpen;
        private StringBuilder _sb;
        private static UnicodeEncoding m_encoding;

        public StringWriter() : this(new StringBuilder(), CultureInfo.CurrentCulture)
        {
        }

        public StringWriter(IFormatProvider formatProvider) : this(new StringBuilder(), formatProvider)
        {
        }

        public StringWriter(StringBuilder sb) : this(sb, CultureInfo.CurrentCulture)
        {
        }

        public StringWriter(StringBuilder sb, IFormatProvider formatProvider) : base(formatProvider)
        {
            if (sb == null)
            {
                throw new ArgumentNullException("sb", Environment.GetResourceString("ArgumentNull_Buffer"));
            }
            this._sb = sb;
            this._isOpen = true;
        }

        public override void Close()
        {
            this.Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            this._isOpen = false;
            base.Dispose(disposing);
        }

        public virtual StringBuilder GetStringBuilder() => 
            this._sb;

        public override string ToString() => 
            this._sb.ToString();

        public override void Write(char value)
        {
            if (!this._isOpen)
            {
                __Error.WriterClosed();
            }
            this._sb.Append(value);
        }

        public override void Write(string value)
        {
            if (!this._isOpen)
            {
                __Error.WriterClosed();
            }
            if (value != null)
            {
                this._sb.Append(value);
            }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            if (!this._isOpen)
            {
                __Error.WriterClosed();
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer", Environment.GetResourceString("ArgumentNull_Buffer"));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if ((buffer.Length - index) < count)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
            }
            this._sb.Append(buffer, index, count);
        }

        public override System.Text.Encoding Encoding
        {
            get
            {
                if (m_encoding == null)
                {
                    m_encoding = new UnicodeEncoding(false, false);
                }
                return m_encoding;
            }
        }
    }
}

