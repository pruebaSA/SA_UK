namespace PdfSharp.Pdf
{
    using PdfSharp;
    using PdfSharp.Pdf.IO;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("({Value})")]
    public sealed class PdfName : PdfItem
    {
        public static readonly PdfName Empty = new PdfName("/");
        private string value;

        public PdfName()
        {
            this.value = "/";
        }

        public PdfName(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if ((value.Length == 0) || (value[0] != '/'))
            {
                throw new ArgumentException(PSSR.NameMustStartWithSlash);
            }
            this.value = value;
        }

        public override bool Equals(object obj) => 
            this.value.Equals(obj);

        public override int GetHashCode() => 
            this.value.GetHashCode();

        public static bool operator ==(PdfName name, string str) => 
            (name.value == str);

        public static bool operator !=(PdfName name, string str) => 
            (name.value != str);

        public override string ToString() => 
            this.value;

        internal override void WriteObject(PdfWriter writer)
        {
            writer.Write(this);
        }

        public static PdfXNameComparer Comparer =>
            new PdfXNameComparer();

        public string Value =>
            this.value;

        public class PdfXNameComparer : IComparer<PdfName>
        {
            public int Compare(PdfName x, PdfName y)
            {
                PdfName name = x;
                PdfName name2 = y;
                if (name != null)
                {
                    if (name2 != null)
                    {
                        return name.value.CompareTo(name2.value);
                    }
                    return -1;
                }
                if (name2 != null)
                {
                    return 1;
                }
                return 0;
            }
        }
    }
}

