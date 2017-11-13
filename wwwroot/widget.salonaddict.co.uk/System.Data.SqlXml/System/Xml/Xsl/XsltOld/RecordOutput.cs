namespace System.Xml.Xsl.XsltOld
{
    using System;

    internal interface RecordOutput
    {
        Processor.OutputResult RecordDone(RecordBuilder record);
        void TheEnd();
    }
}

