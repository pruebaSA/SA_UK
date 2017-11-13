namespace MigraDoc.Rendering
{
    using PdfSharp.Drawing;
    using System;
    using System.Collections;

    internal class ParagraphFormatInfo : FormatInfo
    {
        internal Hashtable imageRenderInfos;
        internal bool isEnding;
        internal bool isStarting;
        private ArrayList lineInfos = new ArrayList();
        internal XFont listFont;
        internal string listSymbol;
        internal bool widowControl;

        internal ParagraphFormatInfo()
        {
        }

        internal void AddLineInfo(LineInfo lineInfo)
        {
            this.lineInfos.Add(lineInfo);
        }

        internal void Append(FormatInfo mergeInfo)
        {
            ParagraphFormatInfo info = (ParagraphFormatInfo) mergeInfo;
            this.lineInfos.AddRange(info.lineInfos);
        }

        internal LineInfo GetFirstLineInfo() => 
            ((LineInfo) this.lineInfos[0]);

        internal LineInfo GetLastLineInfo() => 
            ((LineInfo) this.lineInfos[this.LineCount - 1]);

        internal LineInfo GetLineInfo(int lineIdx) => 
            ((LineInfo) this.lineInfos[lineIdx]);

        internal void RemoveEnding()
        {
            if (!this.IsEmpty)
            {
                if ((this.widowControl && this.isEnding) && (this.LineCount >= 2))
                {
                    this.lineInfos.RemoveAt(this.LineCount - 2);
                }
                if (this.LineCount > 0)
                {
                    this.lineInfos.RemoveAt(this.LineCount - 1);
                }
                this.isEnding = false;
            }
        }

        internal override bool EndingIsComplete
        {
            get
            {
                if (!this.widowControl)
                {
                    return this.isEnding;
                }
                return (this.IsComplete || (this.isEnding && (this.lineInfos.Count >= 2)));
            }
        }

        internal override bool IsComplete =>
            (this.isStarting && this.isEnding);

        internal override bool IsEmpty =>
            (this.lineInfos.Count == 0);

        internal override bool IsEnding =>
            this.isEnding;

        internal override bool IsStarting =>
            this.isStarting;

        internal int LineCount =>
            this.lineInfos.Count;

        internal override bool StartingIsComplete
        {
            get
            {
                if (!this.widowControl)
                {
                    return this.isStarting;
                }
                return (this.IsComplete || (this.isStarting && (this.lineInfos.Count >= 2)));
            }
        }
    }
}

