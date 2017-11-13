namespace System.Text
{
    using System;

    [Serializable]
    public sealed class EncodingInfo
    {
        private int iCodePage;
        private string strDisplayName;
        private string strEncodingName;

        internal EncodingInfo(int codePage, string name, string displayName)
        {
            this.iCodePage = codePage;
            this.strEncodingName = name;
            this.strDisplayName = displayName;
        }

        public override bool Equals(object value)
        {
            EncodingInfo info = value as EncodingInfo;
            return ((info != null) && (this.CodePage == info.CodePage));
        }

        public Encoding GetEncoding() => 
            Encoding.GetEncoding(this.iCodePage);

        public override int GetHashCode() => 
            this.CodePage;

        public int CodePage =>
            this.iCodePage;

        public string DisplayName =>
            this.strDisplayName;

        public string Name =>
            this.strEncodingName;
    }
}

