namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.XPath;

    [StructLayout(LayoutKind.Sequential)]
    internal struct FunctionFocus : IFocus
    {
        private bool isSet;
        private QilParameter current;
        private QilParameter position;
        private QilParameter last;
        public void StartFocus(IList<QilNode> args, XslFlags flags)
        {
            int num = 0;
            if ((flags & XslFlags.Current) != XslFlags.None)
            {
                this.current = (QilParameter) args[num++];
            }
            if ((flags & XslFlags.Position) != XslFlags.None)
            {
                this.position = (QilParameter) args[num++];
            }
            if ((flags & XslFlags.Last) != XslFlags.None)
            {
                this.last = (QilParameter) args[num++];
            }
            this.isSet = true;
        }

        public void StopFocus()
        {
            this.isSet = false;
            this.current = this.position = (QilParameter) (this.last = null);
        }

        public bool IsFocusSet =>
            this.isSet;
        public QilNode GetCurrent() => 
            this.current;

        public QilNode GetPosition() => 
            this.position;

        public QilNode GetLast() => 
            this.last;
    }
}

