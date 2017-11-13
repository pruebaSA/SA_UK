namespace System.IO
{
    using System;

    internal sealed class MdaHelper
    {
        private string allocatedCallstack;
        private StreamWriter streamWriter;

        internal MdaHelper(StreamWriter sw, string cs)
        {
            this.streamWriter = sw;
            this.allocatedCallstack = cs;
        }

        ~MdaHelper()
        {
            if (((this.streamWriter.charPos != 0) && (this.streamWriter.stream != null)) && (this.streamWriter.stream != Stream.Null))
            {
                string str = (this.streamWriter.stream is FileStream) ? ((FileStream) this.streamWriter.stream).NameInternal : "<unknown>";
                string resourceString = Environment.GetResourceString("IO_StreamWriterBufferedDataLost", new object[] { this.streamWriter.stream.GetType().FullName, str });
                if (this.allocatedCallstack != null)
                {
                    string str3 = resourceString;
                    resourceString = str3 + Environment.NewLine + Environment.GetResourceString("AllocatedFrom") + Environment.NewLine + this.allocatedCallstack;
                }
                Mda.StreamWriterBufferedDataLost(resourceString);
            }
        }
    }
}

