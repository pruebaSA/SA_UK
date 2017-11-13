namespace System.Xml
{
    using System;

    internal class PositionInfo : IXmlLineInfo
    {
        public static PositionInfo GetPositionInfo(object o)
        {
            IXmlLineInfo lineInfo = o as IXmlLineInfo;
            if (lineInfo != null)
            {
                return new ReaderPositionInfo(lineInfo);
            }
            return new PositionInfo();
        }

        public virtual bool HasLineInfo() => 
            false;

        public virtual int LineNumber =>
            0;

        public virtual int LinePosition =>
            0;
    }
}

