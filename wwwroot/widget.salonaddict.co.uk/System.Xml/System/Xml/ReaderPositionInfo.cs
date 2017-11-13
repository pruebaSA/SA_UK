namespace System.Xml
{
    using System;

    internal class ReaderPositionInfo : PositionInfo
    {
        private IXmlLineInfo lineInfo;

        public ReaderPositionInfo(IXmlLineInfo lineInfo)
        {
            this.lineInfo = lineInfo;
        }

        public override bool HasLineInfo() => 
            this.lineInfo.HasLineInfo();

        public override int LineNumber =>
            this.lineInfo.LineNumber;

        public override int LinePosition =>
            this.lineInfo.LinePosition;
    }
}

