namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class BatchStream : Stream
    {
        private string batchBoundary;
        private System.Text.Encoding batchEncoding;
        private int batchLength;
        private readonly bool batchRequest;
        private BatchStreamState batchState;
        private readonly byte[] byteBuffer;
        private int byteLength;
        private int bytePosition;
        private string changesetBoundary;
        private System.Text.Encoding changesetEncoding;
        private bool checkPreamble;
        private Dictionary<string, string> contentHeaders;
        private Stream contentStream;
        private const int DefaultBufferSize = 0x1f40;
        private bool disposeWithContentStreamDispose;
        private Stream reader;
        private string statusCode;
        private int totalCount;
        private MemoryStream writer;

        internal BatchStream(Stream stream, string boundary, System.Text.Encoding batchEncoding, bool requestStream)
        {
            this.reader = stream;
            this.byteBuffer = new byte[0x1f40];
            this.batchBoundary = VerifyBoundary(boundary);
            this.batchState = BatchStreamState.StartBatch;
            this.batchEncoding = batchEncoding;
            this.checkPreamble = null != batchEncoding;
            this.batchRequest = requestStream;
        }

        private void Append(ref byte[] buffer, int count)
        {
            int num = (buffer != null) ? buffer.Length : 0;
            byte[] dst = new byte[num + count];
            if (0 < num)
            {
                Buffer.BlockCopy(buffer, 0, dst, 0, num);
            }
            Buffer.BlockCopy(this.byteBuffer, this.bytePosition, dst, num, count);
            buffer = dst;
            this.totalCount += count;
            this.bytePosition += count;
            this.byteLength -= count;
            this.batchLength -= count;
        }

        private void AssertOpen()
        {
            if (this.reader == null)
            {
                Error.ThrowObjectDisposed(base.GetType());
            }
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw Error.NotSupported();
        }

        private void ClearPreviousOperationInformation()
        {
            this.contentHeaders = null;
            this.contentStream = null;
            this.statusCode = null;
        }

        private System.Text.Encoding DetectEncoding()
        {
            if (this.byteLength >= 2)
            {
                if ((this.byteBuffer[0] == 0xfe) && (this.byteBuffer[1] == 0xff))
                {
                    this.bytePosition = 2;
                    this.byteLength -= 2;
                    return new UnicodeEncoding(true, true);
                }
                if ((this.byteBuffer[0] == 0xff) && (this.byteBuffer[1] == 0xfe))
                {
                    if (((this.byteLength >= 4) && (this.byteBuffer[2] == 0)) && (this.byteBuffer[3] == 0))
                    {
                        this.bytePosition = 4;
                        this.byteLength -= 4;
                        return new UTF32Encoding(false, true);
                    }
                    this.bytePosition = 2;
                    this.byteLength -= 2;
                    return new UnicodeEncoding(false, true);
                }
                if (((this.byteLength >= 3) && (this.byteBuffer[0] == 0xef)) && ((this.byteBuffer[1] == 0xbb) && (this.byteBuffer[2] == 0xbf)))
                {
                    this.bytePosition = 3;
                    this.byteLength -= 3;
                    return System.Text.Encoding.UTF8;
                }
                if ((((this.byteLength >= 4) && (this.byteBuffer[0] == 0)) && ((this.byteBuffer[1] == 0) && (this.byteBuffer[2] == 0xfe))) && (this.byteBuffer[3] == 0xff))
                {
                    this.bytePosition = 4;
                    this.byteLength -= 4;
                    return new UTF32Encoding(true, true);
                }
            }
            return System.Text.Encoding.ASCII;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.contentStream != null)
                {
                    this.disposeWithContentStreamDispose = true;
                }
                else
                {
                    this.byteLength = 0;
                    if (this.reader != null)
                    {
                        this.reader.Dispose();
                        this.reader = null;
                    }
                    this.contentHeaders = null;
                    if (this.contentStream != null)
                    {
                        this.contentStream.Dispose();
                    }
                    if (this.writer != null)
                    {
                        this.writer.Dispose();
                    }
                }
            }
            base.Dispose(disposing);
        }

        public override void Flush()
        {
            this.reader.Flush();
        }

        internal static bool GetBoundaryAndEncodingFromMultipartMixedContentType(string contentType, out string boundary, out System.Text.Encoding encoding)
        {
            string str;
            boundary = null;
            encoding = null;
            KeyValuePair<string, string>[] pairArray = HttpProcessUtility.ReadContentType(contentType, out str, out encoding);
            if (string.Equals("multipart/mixed", str, StringComparison.OrdinalIgnoreCase))
            {
                if (pairArray != null)
                {
                    foreach (KeyValuePair<string, string> pair in pairArray)
                    {
                        if (string.Equals(pair.Key, "boundary", StringComparison.OrdinalIgnoreCase))
                        {
                            if (boundary != null)
                            {
                                boundary = null;
                                break;
                            }
                            boundary = pair.Value;
                        }
                    }
                }
                if (string.IsNullOrEmpty(boundary))
                {
                    throw Error.BatchStreamMissingBoundary();
                }
            }
            return (null != boundary);
        }

        internal Stream GetContentStream() => 
            this.contentStream;

        internal string GetResponseVersion()
        {
            string str;
            this.ContentHeaders.TryGetValue("DataServiceVersion", out str);
            return str;
        }

        private static BatchStreamState GetStateBasedOnHttpMethodName(string methodName)
        {
            if ("GET".Equals(methodName, StringComparison.Ordinal))
            {
                return BatchStreamState.Get;
            }
            if ("DELETE".Equals(methodName, StringComparison.Ordinal))
            {
                return BatchStreamState.Delete;
            }
            if ("POST".Equals(methodName, StringComparison.Ordinal))
            {
                return BatchStreamState.Post;
            }
            if ("PUT".Equals(methodName, StringComparison.Ordinal))
            {
                return BatchStreamState.Put;
            }
            if (!"MERGE".Equals(methodName, StringComparison.Ordinal))
            {
                throw Error.BatchStreamInvalidHttpMethodName(methodName);
            }
            return BatchStreamState.Merge;
        }

        internal HttpStatusCode GetStatusCode()
        {
            if (this.statusCode == null)
            {
                return HttpStatusCode.InternalServerError;
            }
            return (HttpStatusCode) int.Parse(this.statusCode, CultureInfo.InvariantCulture);
        }

        internal bool MoveNext()
        {
            string str;
            string str2;
            if ((this.reader == null) || this.disposeWithContentStreamDispose)
            {
                return false;
            }
            if (this.contentStream != null)
            {
                this.contentStream.Dispose();
            }
            switch (this.batchState)
            {
                case BatchStreamState.EndBatch:
                    throw Error.BatchStreamInvalidBatchFormat();

                case BatchStreamState.StartBatch:
                case BatchStreamState.EndChangeSet:
                    break;

                case BatchStreamState.BeginChangeSet:
                    this.contentHeaders = null;
                    this.changesetEncoding = null;
                    this.batchState = BatchStreamState.EndChangeSet;
                    goto Label_00B7;

                case BatchStreamState.Post:
                case BatchStreamState.Put:
                case BatchStreamState.Merge:
                    this.batchState = BatchStreamState.EndChangeSet;
                    goto Label_00B7;

                case BatchStreamState.Delete:
                case BatchStreamState.ChangeResponse:
                    this.ClearPreviousOperationInformation();
                    this.batchState = BatchStreamState.EndChangeSet;
                    goto Label_00B7;

                case BatchStreamState.Get:
                case BatchStreamState.GetResponse:
                    this.ClearPreviousOperationInformation();
                    break;

                default:
                    throw Error.BatchStreamInvalidBatchFormat();
            }
            this.batchState = BatchStreamState.EndBatch;
            this.batchLength = 0x7fffffff;
        Label_00B7:
            str = this.ReadLine();
            if (string.IsNullOrEmpty(str))
            {
                str = this.ReadLine();
            }
            if (string.IsNullOrEmpty(str))
            {
                throw Error.BatchStreamInvalidBatchFormat();
            }
            if (str.EndsWith("--", StringComparison.Ordinal))
            {
                str = str.Substring(0, str.Length - 2);
                if ((this.changesetBoundary != null) && (str == this.changesetBoundary))
                {
                    this.changesetBoundary = null;
                    return true;
                }
                if (str != this.batchBoundary)
                {
                    throw Error.BatchStreamInvalidDelimiter(str);
                }
                if (BatchStreamState.EndChangeSet == this.batchState)
                {
                    throw Error.BatchStreamMissingEndChangesetDelimiter();
                }
                this.changesetBoundary = null;
                this.batchBoundary = null;
                if (this.byteLength != 0)
                {
                    throw Error.BatchStreamMoreDataAfterEndOfBatch();
                }
                return false;
            }
            if ((this.changesetBoundary == null) || (str != this.changesetBoundary))
            {
                if (str != this.batchBoundary)
                {
                    throw Error.BatchStreamInvalidDelimiter(str);
                }
                if (this.batchState != BatchStreamState.EndBatch)
                {
                    if (this.batchState == BatchStreamState.EndChangeSet)
                    {
                        throw Error.BatchStreamMissingEndChangesetDelimiter();
                    }
                    throw Error.BatchStreamInvalidBatchFormat();
                }
            }
            this.ReadContentHeaders();
            bool flag = false;
            if (!this.contentHeaders.TryGetValue("Content-Type", out str2))
            {
                throw Error.BatchStreamMissingContentTypeHeader("Content-Type");
            }
            if (string.Equals(str2, "application/http", StringComparison.OrdinalIgnoreCase))
            {
                string str3;
                if (this.contentHeaders.Count != 2)
                {
                    throw Error.BatchStreamInvalidNumberOfHeadersAtOperationStart("Content-Type", "Content-Transfer-Encoding");
                }
                if (!this.contentHeaders.TryGetValue("Content-Transfer-Encoding", out str3) || ("binary" != str3))
                {
                    throw Error.BatchStreamMissingOrInvalidContentEncodingHeader("Content-Transfer-Encoding", "binary");
                }
                flag = true;
            }
            else
            {
                string str4;
                System.Text.Encoding encoding;
                if (this.batchState != BatchStreamState.EndBatch)
                {
                    throw Error.BatchStreamInvalidContentTypeSpecified("Content-Type", str2, "application/http", "multipart/mixed");
                }
                if (!GetBoundaryAndEncodingFromMultipartMixedContentType(str2, out str4, out encoding))
                {
                    throw Error.BatchStreamInvalidContentTypeSpecified("Content-Type", str2, "application/http", "multipart/mixed");
                }
                this.changesetBoundary = VerifyBoundary(str4);
                this.changesetEncoding = encoding;
                this.batchState = BatchStreamState.BeginChangeSet;
                if ((this.contentHeaders.Count > 2) || ((this.contentHeaders.Count == 2) && !this.contentHeaders.ContainsKey("Content-Length")))
                {
                    throw Error.BatchStreamInvalidNumberOfHeadersAtChangeSetStart("Content-Type", "Content-Length");
                }
            }
            if (flag)
            {
                this.ReadHttpHeaders();
                this.contentHeaders.TryGetValue("Content-Type", out str2);
            }
            string str5 = null;
            int contentLength = -1;
            if (this.contentHeaders.TryGetValue("Content-Length", out str5))
            {
                contentLength = int.Parse(str5, CultureInfo.InvariantCulture);
                if (contentLength < 0)
                {
                    throw Error.BatchStreamInvalidContentLengthSpecified(str5);
                }
                if (this.batchState == BatchStreamState.BeginChangeSet)
                {
                    this.batchLength = contentLength;
                }
                else if (contentLength != 0)
                {
                    this.contentStream = new StreamWithLength(this, contentLength);
                }
            }
            else
            {
                if (this.batchState == BatchStreamState.EndBatch)
                {
                    this.batchLength = 0x7fffffff;
                }
                if (this.batchState != BatchStreamState.BeginChangeSet)
                {
                    this.contentStream = new StreamWithDelimiter(this);
                }
            }
            if (this.contentStream == null)
            {
                switch (this.batchState)
                {
                    case BatchStreamState.BeginChangeSet:
                    case BatchStreamState.Delete:
                    case BatchStreamState.Get:
                    case BatchStreamState.GetResponse:
                    case BatchStreamState.ChangeResponse:
                        goto Label_03B5;
                }
                throw Error.BatchStreamContentExpected(this.batchState);
            }
        Label_03B5:
            if (!string.IsNullOrEmpty(str2))
            {
                switch (this.batchState)
                {
                    case BatchStreamState.BeginChangeSet:
                    case BatchStreamState.Post:
                    case BatchStreamState.Put:
                    case BatchStreamState.Merge:
                    case BatchStreamState.GetResponse:
                    case BatchStreamState.ChangeResponse:
                        goto Label_03FE;
                }
                throw Error.BatchStreamContentUnexpected(this.batchState);
            }
        Label_03FE:
            return true;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw Error.NotSupported();
        }

        private bool ReadBuffer()
        {
            this.AssertOpen();
            if (this.byteLength != 0)
            {
                return true;
            }
            this.bytePosition = 0;
            this.byteLength = this.reader.Read(this.byteBuffer, this.bytePosition, this.byteBuffer.Length);
            if (this.writer != null)
            {
                this.writer.Write(this.byteBuffer, this.bytePosition, this.byteLength);
            }
            if (this.batchEncoding == null)
            {
                this.batchEncoding = this.DetectEncoding();
            }
            else if (this.changesetEncoding != null)
            {
                this.changesetEncoding = this.DetectEncoding();
            }
            else if (this.checkPreamble)
            {
                bool flag = true;
                byte[] preamble = this.batchEncoding.GetPreamble();
                if (preamble.Length <= this.byteLength)
                {
                    for (int i = 0; i < preamble.Length; i++)
                    {
                        if (preamble[i] != this.byteBuffer[i])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        this.byteLength -= preamble.Length;
                        this.bytePosition += preamble.Length;
                    }
                }
                this.checkPreamble = false;
            }
            return (0 < this.byteLength);
        }

        private void ReadContentHeaders()
        {
            this.contentHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            while (true)
            {
                string headerValue = this.ReadLine();
                if (0 >= headerValue.Length)
                {
                    return;
                }
                int index = headerValue.IndexOf(':');
                if (index <= 0)
                {
                    throw Error.BatchStreamInvalidHeaderValueSpecified(headerValue);
                }
                string key = headerValue.Substring(0, index).Trim();
                string str3 = headerValue.Substring(index + 1).Trim();
                this.contentHeaders.Add(key, str3);
            }
        }

        private int ReadDelimiter(byte[] buffer, int offset, int count)
        {
            int num = 0;
            string str = null;
            string batchBoundary = this.batchBoundary;
            string changesetBoundary = this.changesetBoundary;
            while (((0 < count) && (0 < this.batchLength)) && this.ReadBuffer())
            {
                int num2 = 0;
                int num3 = 0;
                int num4 = 0;
                int num5 = Math.Min(Math.Min(count, this.byteLength), this.batchLength) + this.bytePosition;
                byte[] byteBuffer = this.byteBuffer;
                for (int i = this.bytePosition; i < num5; i++)
                {
                    byte num7 = byteBuffer[i];
                    buffer[offset++] = num7;
                    if (num7 == batchBoundary[num3])
                    {
                        if (batchBoundary.Length != ++num3)
                        {
                            goto Label_0092;
                        }
                        num5 = (1 + i) - num3;
                        offset -= num3;
                        break;
                    }
                    num3 = 0;
                Label_0092:
                    if ((changesetBoundary != null) && (num7 == changesetBoundary[num4]))
                    {
                        if (changesetBoundary.Length != ++num4)
                        {
                            continue;
                        }
                        num5 = (1 + i) - num4;
                        offset -= num4;
                        break;
                    }
                    num4 = 0;
                }
                num5 -= this.bytePosition;
                if (num3 < num4)
                {
                    num2 = num4;
                    str = changesetBoundary;
                }
                else
                {
                    num2 = num3;
                    str = batchBoundary;
                }
                if (num5 == this.batchLength)
                {
                    num2 = 0;
                }
                if ((0 < num2) && (str.Length != num2))
                {
                    if (((num5 + num) == num2) && (num2 < this.byteLength))
                    {
                        throw Error.BatchStreamInternalBufferRequestTooSmall();
                    }
                    num5 -= num2;
                    offset -= num2;
                }
                this.totalCount += num5;
                this.bytePosition += num5;
                this.byteLength -= num5;
                this.batchLength -= num5;
                count -= num5;
                num += num5;
                if (((num2 > 0) && (num >= 2)) && ((buffer[num - 2] == 13) && (buffer[num - 1] == 10)))
                {
                    num -= 2;
                }
                if (str.Length == num2)
                {
                    return num;
                }
                if (0 < num2)
                {
                    if (num2 != this.byteLength)
                    {
                        return num;
                    }
                    if (0 < this.bytePosition)
                    {
                        Buffer.BlockCopy(byteBuffer, this.bytePosition, byteBuffer, 0, this.byteLength);
                        this.bytePosition = 0;
                    }
                    int num8 = this.reader.Read(this.byteBuffer, this.byteLength, this.byteBuffer.Length - this.byteLength);
                    if (this.writer != null)
                    {
                        this.writer.Write(this.byteBuffer, this.byteLength, num8);
                    }
                    if (num8 == 0)
                    {
                        this.totalCount += num2;
                        this.bytePosition += num2;
                        this.byteLength -= num2;
                        this.batchLength -= num2;
                        offset += num2;
                        count -= num2;
                        return (num + num2);
                    }
                    this.byteLength += num8;
                }
            }
            return num;
        }

        private void ReadHttpHeaders()
        {
            BatchStreamState stateBasedOnHttpMethodName;
            string header = this.ReadLine();
            int index = header.IndexOf(' ');
            if ((index <= 0) || ((header.Length - 3) <= index))
            {
                throw Error.BatchStreamInvalidMethodHeaderSpecified(header);
            }
            int num2 = this.batchRequest ? header.LastIndexOf(' ') : header.IndexOf(' ', index + 1);
            if (((num2 < 0) || (((num2 - index) - 1) <= 0)) || ((header.Length - 1) <= num2))
            {
                throw Error.BatchStreamInvalidMethodHeaderSpecified(header);
            }
            string methodName = header.Substring(0, index);
            string str3 = header.Substring(index + 1, (num2 - index) - 1);
            string str4 = header.Substring(num2 + 1);
            string actualVersion = this.batchRequest ? str4 : methodName;
            if (actualVersion != "HTTP/1.1")
            {
                throw Error.BatchStreamInvalidHttpVersionSpecified(actualVersion, "HTTP/1.1");
            }
            this.ReadContentHeaders();
            if (this.batchRequest)
            {
                stateBasedOnHttpMethodName = GetStateBasedOnHttpMethodName(methodName);
            }
            else
            {
                stateBasedOnHttpMethodName = (this.batchState == BatchStreamState.EndBatch) ? BatchStreamState.GetResponse : BatchStreamState.ChangeResponse;
                this.statusCode = str3;
            }
            if (this.batchState == BatchStreamState.EndBatch)
            {
                if ((!this.batchRequest || (stateBasedOnHttpMethodName != BatchStreamState.Get)) && (this.batchRequest || (stateBasedOnHttpMethodName != BatchStreamState.GetResponse)))
                {
                    throw Error.BatchStreamOnlyGETOperationsCanBeSpecifiedInBatch();
                }
                this.batchState = stateBasedOnHttpMethodName;
            }
            else
            {
                if (this.batchState != BatchStreamState.EndChangeSet)
                {
                    throw Error.BatchStreamInvalidOperationHeaderSpecified();
                }
                if ((this.batchRequest && (((BatchStreamState.Post == stateBasedOnHttpMethodName) || (BatchStreamState.Put == stateBasedOnHttpMethodName)) || ((BatchStreamState.Delete == stateBasedOnHttpMethodName) || (BatchStreamState.Merge == stateBasedOnHttpMethodName)))) || (!this.batchRequest && (stateBasedOnHttpMethodName == BatchStreamState.ChangeResponse)))
                {
                    this.batchState = stateBasedOnHttpMethodName;
                }
                else
                {
                    this.batchState = BatchStreamState.Post;
                    throw Error.BatchStreamGetMethodNotSupportInChangeset();
                }
            }
        }

        private int ReadLength(byte[] buffer, int offset, int count)
        {
            int num = 0;
            if (0 < this.byteLength)
            {
                int num2 = Math.Min(Math.Min(count, this.byteLength), this.batchLength);
                Buffer.BlockCopy(this.byteBuffer, this.bytePosition, buffer, offset, num2);
                this.totalCount += num2;
                this.bytePosition += num2;
                this.byteLength -= num2;
                this.batchLength -= num2;
                offset += num2;
                count -= num2;
                num = num2;
            }
            if ((0 >= count) || (this.batchLength <= 0))
            {
                return num;
            }
            int num3 = this.reader.Read(buffer, offset, Math.Min(count, this.batchLength));
            if (this.writer != null)
            {
                this.writer.Write(buffer, offset, num3);
            }
            this.totalCount += num3;
            this.batchLength -= num3;
            return (num + num3);
        }

        private string ReadLine()
        {
            if ((this.batchLength == 0) || !this.ReadBuffer())
            {
                return null;
            }
            byte[] buffer = null;
            do
            {
                int bytePosition = this.bytePosition;
                int num2 = bytePosition + Math.Min(this.byteLength, this.batchLength);
                do
                {
                    char ch = (char) this.byteBuffer[bytePosition];
                    if (('\r' == ch) || ('\n' == ch))
                    {
                        string str;
                        bytePosition -= this.bytePosition;
                        if (buffer != null)
                        {
                            this.Append(ref buffer, bytePosition);
                            str = this.Encoding.GetString(buffer, 0, buffer.Length);
                        }
                        else
                        {
                            str = this.Encoding.GetString(this.byteBuffer, this.bytePosition, bytePosition);
                            this.totalCount += bytePosition;
                            this.bytePosition += bytePosition;
                            this.byteLength -= bytePosition;
                            this.batchLength -= bytePosition;
                        }
                        this.totalCount++;
                        this.bytePosition++;
                        this.byteLength--;
                        this.batchLength--;
                        if ((('\r' == ch) && ((0 < this.byteLength) || this.ReadBuffer())) && (0 < this.batchLength))
                        {
                            ch = (char) this.byteBuffer[this.bytePosition];
                            if ('\n' == ch)
                            {
                                this.totalCount++;
                                this.bytePosition++;
                                this.byteLength--;
                                this.batchLength--;
                            }
                        }
                        return str;
                    }
                    bytePosition++;
                }
                while (bytePosition < num2);
                bytePosition -= this.bytePosition;
                this.Append(ref buffer, bytePosition);
            }
            while (this.ReadBuffer() && (0 < this.batchLength));
            return this.Encoding.GetString(buffer, 0, buffer.Length);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.AssertOpen();
            if (offset < 0L)
            {
                throw Error.ArgumentOutOfRange("offset");
            }
            if (SeekOrigin.Current != origin)
            {
                throw Error.ArgumentOutOfRange("origin");
            }
            if (0x7fffffffL == offset)
            {
                byte[] buffer = new byte[0x100];
                while (0 < this.ReadDelimiter(buffer, 0, buffer.Length))
                {
                }
            }
            else if (0L < offset)
            {
                do
                {
                    int num = Math.Min((int) offset, Math.Min(this.byteLength, this.batchLength));
                    this.totalCount += num;
                    this.bytePosition += num;
                    this.byteLength -= num;
                    this.batchLength -= num;
                    offset -= num;
                }
                while (((0L < offset) && (this.batchLength != 0)) && this.ReadBuffer());
            }
            return 0L;
        }

        public override void SetLength(long value)
        {
            throw Error.NotSupported();
        }

        private static string VerifyBoundary(string boundary)
        {
            if ((boundary == null) || (70 < boundary.Length))
            {
                throw Error.BatchStreamInvalidDelimiter(boundary);
            }
            foreach (char ch in boundary)
            {
                if ((('\x007f' < ch) || char.IsWhiteSpace(ch)) || char.IsControl(ch))
                {
                    throw Error.BatchStreamInvalidDelimiter(boundary);
                }
            }
            return ("--" + boundary);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw Error.NotSupported();
        }

        public override bool CanRead =>
            ((this.reader != null) && this.reader.CanRead);

        public override bool CanSeek =>
            false;

        public override bool CanWrite =>
            false;

        public Dictionary<string, string> ContentHeaders =>
            this.contentHeaders;

        public System.Text.Encoding Encoding =>
            (this.changesetEncoding ?? this.batchEncoding);

        public override long Length
        {
            get
            {
                throw Error.NotSupported();
            }
        }

        public override long Position
        {
            get
            {
                throw Error.NotSupported();
            }
            set
            {
                throw Error.NotSupported();
            }
        }

        public BatchStreamState State =>
            this.batchState;

        private sealed class StreamWithDelimiter : BatchStream.StreamWithLength
        {
            internal StreamWithDelimiter(BatchStream stream) : base(stream, 0x7fffffff)
            {
            }

            public override int Read(byte[] buffer, int offset, int count) => 
                base.Target?.ReadDelimiter(buffer, offset, count);
        }

        private class StreamWithLength : Stream
        {
            private int length;
            private BatchStream target;

            internal StreamWithLength(BatchStream stream, int contentLength)
            {
                this.target = stream;
                this.length = contentLength;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing && (this.target != null))
                {
                    if (this.target.disposeWithContentStreamDispose)
                    {
                        this.target.contentStream = null;
                        this.target.Dispose();
                    }
                    else if (0 < this.length)
                    {
                        if (this.target.reader != null)
                        {
                            this.target.Seek((long) this.length, SeekOrigin.Current);
                        }
                        this.length = 0;
                    }
                    this.target.ClearPreviousOperationInformation();
                }
                this.target = null;
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (this.target == null)
                {
                    Error.ThrowObjectDisposed(base.GetType());
                }
                int num = this.target.ReadLength(buffer, offset, Math.Min(count, this.length));
                this.length -= num;
                return num;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw Error.NotSupported();
            }

            public override void SetLength(long value)
            {
                throw Error.NotSupported();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw Error.NotSupported();
            }

            public override bool CanRead =>
                ((this.target != null) && this.target.CanRead);

            public override bool CanSeek =>
                false;

            public override bool CanWrite =>
                false;

            public override long Length
            {
                get
                {
                    throw Error.NotSupported();
                }
            }

            public override long Position
            {
                get
                {
                    throw Error.NotSupported();
                }
                set
                {
                    throw Error.NotSupported();
                }
            }

            internal BatchStream Target =>
                this.target;
        }
    }
}

