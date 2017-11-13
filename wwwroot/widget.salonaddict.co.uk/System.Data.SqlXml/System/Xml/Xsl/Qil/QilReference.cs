namespace System.Xml.Xsl.Qil
{
    using System;

    internal class QilReference : QilNode
    {
        private string debugName;
        private const int MaxDebugNameLength = 0x3ff;

        public QilReference(QilNodeType nodeType) : base(nodeType)
        {
        }

        public string DebugName
        {
            get => 
                this.debugName;
            set
            {
                if (value.Length > 0x3ff)
                {
                    value = value.Substring(0, 0x3ff);
                }
                this.debugName = value;
            }
        }
    }
}

