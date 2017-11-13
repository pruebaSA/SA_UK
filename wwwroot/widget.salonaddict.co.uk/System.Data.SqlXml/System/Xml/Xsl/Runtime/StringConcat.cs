namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct StringConcat
    {
        private string s1;
        private string s2;
        private string s3;
        private string s4;
        private string delimiter;
        private List<string> strList;
        private int idxStr;
        public void Clear()
        {
            this.idxStr = 0;
            this.delimiter = null;
        }

        public string Delimiter
        {
            get => 
                this.delimiter;
            set
            {
                this.delimiter = value;
            }
        }
        internal int Count =>
            this.idxStr;
        public void Concat(string value)
        {
            if ((this.delimiter != null) && (this.idxStr != 0))
            {
                this.ConcatNoDelimiter(this.delimiter);
            }
            this.ConcatNoDelimiter(value);
        }

        public string GetResult()
        {
            switch (this.idxStr)
            {
                case 0:
                    return string.Empty;

                case 1:
                    return this.s1;

                case 2:
                    return (this.s1 + this.s2);

                case 3:
                    return (this.s1 + this.s2 + this.s3);

                case 4:
                    return (this.s1 + this.s2 + this.s3 + this.s4);
            }
            return string.Concat(this.strList.ToArray());
        }

        internal void ConcatNoDelimiter(string s)
        {
            switch (this.idxStr)
            {
                case 0:
                    this.s1 = s;
                    goto Label_00AA;

                case 1:
                    this.s2 = s;
                    goto Label_00AA;

                case 2:
                    this.s3 = s;
                    goto Label_00AA;

                case 3:
                    this.s4 = s;
                    goto Label_00AA;

                case 4:
                {
                    int capacity = (this.strList == null) ? 8 : this.strList.Count;
                    List<string> list = this.strList = new List<string>(capacity);
                    list.Add(this.s1);
                    list.Add(this.s2);
                    list.Add(this.s3);
                    list.Add(this.s4);
                    break;
                }
            }
            this.strList.Add(s);
        Label_00AA:
            this.idxStr++;
        }
    }
}

