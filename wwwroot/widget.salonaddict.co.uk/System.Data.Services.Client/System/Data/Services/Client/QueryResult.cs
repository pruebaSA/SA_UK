namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;

    internal class QueryResult : BaseAsyncResult
    {
        private Stream asyncResponseStream;
        private byte[] asyncStreamCopyBuffer;
        private long contentLength;
        private string contentType;
        private HttpWebResponse httpWebResponse;
        internal readonly HttpWebRequest Request;
        private Stream requestStream;
        private MemoryStream requestStreamContent;
        private Stream responseStream;
        private bool responseStreamOwner;
        private static byte[] reusableAsyncCopyBuffer;
        internal readonly DataServiceRequest ServiceRequest;
        private HttpStatusCode statusCode;
        private bool usingBuffer;

        internal QueryResult(object source, string method, DataServiceRequest serviceRequest, HttpWebRequest request, AsyncCallback callback, object state) : base(source, method, callback, state)
        {
            this.ServiceRequest = serviceRequest;
            this.Request = request;
            base.Abortable = request;
        }

        private static void AsyncEndGetResponse(IAsyncResult asyncResult)
        {
            QueryResult asyncState = asyncResult.AsyncState as QueryResult;
            try
            {
                CompleteCheck(asyncState, InternalError.InvalidEndGetResponseCompleted);
                asyncState.CompletedSynchronously &= asyncResult.CompletedSynchronously;
                HttpWebRequest request = Util.NullCheck<HttpWebRequest>(asyncState.Request, InternalError.InvalidEndGetResponseRequest);
                HttpWebResponse response = null;
                try
                {
                    response = (HttpWebResponse) request.EndGetResponse(asyncResult);
                }
                catch (WebException exception)
                {
                    response = (HttpWebResponse) exception.Response;
                    if (response == null)
                    {
                        throw;
                    }
                }
                asyncState.SetHttpWebResponse(Util.NullCheck<HttpWebResponse>(response, InternalError.InvalidEndGetResponseResponse));
                Stream responseStream = null;
                if (HttpStatusCode.NoContent != response.StatusCode)
                {
                    responseStream = response.GetResponseStream();
                    asyncState.asyncResponseStream = responseStream;
                }
                if ((responseStream != null) && responseStream.CanRead)
                {
                    if (asyncState.responseStream == null)
                    {
                        asyncState.responseStream = Util.NullCheck<Stream>(asyncState.GetAsyncResponseStreamCopy(), InternalError.InvalidAsyncResponseStreamCopy);
                    }
                    if (asyncState.asyncStreamCopyBuffer == null)
                    {
                        asyncState.asyncStreamCopyBuffer = Util.NullCheck<byte[]>(asyncState.GetAsyncResponseStreamCopyBuffer(), InternalError.InvalidAsyncResponseStreamCopyBuffer);
                    }
                    ReadResponseStream(asyncState);
                }
                else
                {
                    asyncState.SetCompleted();
                }
            }
            catch (Exception exception2)
            {
                if (asyncState.HandleFailure(exception2))
                {
                    throw;
                }
            }
            finally
            {
                asyncState.HandleCompleted();
            }
        }

        private static void AsyncEndRead(IAsyncResult asyncResult)
        {
            QueryResult asyncState = asyncResult.AsyncState as QueryResult;
            int count = 0;
            try
            {
                CompleteCheck(asyncState, InternalError.InvalidEndReadCompleted);
                asyncState.CompletedSynchronously &= asyncResult.CompletedSynchronously;
                Stream stream = Util.NullCheck<Stream>(asyncState.asyncResponseStream, InternalError.InvalidEndReadStream);
                Stream stream2 = Util.NullCheck<Stream>(asyncState.responseStream, InternalError.InvalidEndReadCopy);
                byte[] buffer = Util.NullCheck<byte[]>(asyncState.asyncStreamCopyBuffer, InternalError.InvalidEndReadBuffer);
                count = stream.EndRead(asyncResult);
                asyncState.usingBuffer = false;
                if (0 < count)
                {
                    stream2.Write(buffer, 0, count);
                }
                if (((0 < count) && (0 < buffer.Length)) && stream.CanRead)
                {
                    if (!asyncResult.CompletedSynchronously)
                    {
                        ReadResponseStream(asyncState);
                    }
                }
                else
                {
                    if (stream2.Position < stream2.Length)
                    {
                        ((MemoryStream) stream2).SetLength(stream2.Position);
                    }
                    asyncState.SetCompleted();
                }
            }
            catch (Exception exception)
            {
                if (asyncState.HandleFailure(exception))
                {
                    throw;
                }
            }
            finally
            {
                asyncState.HandleCompleted();
            }
        }

        internal void BeginExecute()
        {
            try
            {
                IAsyncResult result = BaseAsyncResult.InvokeAsync(new Func<AsyncCallback, object, IAsyncResult>(this.Request.BeginGetResponse), new AsyncCallback(QueryResult.AsyncEndGetResponse), this);
                base.CompletedSynchronously &= result.CompletedSynchronously;
            }
            catch (Exception exception)
            {
                base.HandleFailure(exception);
                throw;
            }
            finally
            {
                base.HandleCompleted();
            }
        }

        private static void CompleteCheck(QueryResult pereq, InternalError errorcode)
        {
            if ((pereq == null) || (pereq.IsCompletedInternally && !pereq.IsAborted))
            {
                Error.ThrowInternalError(errorcode);
            }
        }

        protected override void CompletedRequest()
        {
            Util.Dispose<Stream>(ref this.asyncResponseStream);
            Util.Dispose<Stream>(ref this.requestStream);
            Util.Dispose<MemoryStream>(ref this.requestStreamContent);
            byte[] asyncStreamCopyBuffer = this.asyncStreamCopyBuffer;
            this.asyncStreamCopyBuffer = null;
            if ((asyncStreamCopyBuffer != null) && !this.usingBuffer)
            {
                this.PutAsyncResponseStreamCopyBuffer(asyncStreamCopyBuffer);
            }
            if (this.responseStreamOwner && (this.responseStream != null))
            {
                this.responseStream.Position = 0L;
            }
            if (this.httpWebResponse != null)
            {
                this.httpWebResponse.Close();
                Exception e = DataServiceContext.HandleResponse(this.StatusCode, this.httpWebResponse.Headers["DataServiceVersion"], new Func<Stream>(this.GetResponseStream), false);
                if (e != null)
                {
                    base.HandleFailure(e);
                }
            }
        }

        internal static QueryResult EndExecute<TElement>(object source, IAsyncResult asyncResult)
        {
            QueryResult result = null;
            try
            {
                result = BaseAsyncResult.EndExecute<QueryResult>(source, "Execute", asyncResult);
            }
            catch (InvalidOperationException exception)
            {
                result = asyncResult as QueryResult;
                QueryOperationResponse response = result.GetResponse<TElement>(MaterializeAtom.EmptyResults);
                if (response != null)
                {
                    response.Error = exception;
                    throw new DataServiceQueryException(Strings.DataServiceException_GeneralError, exception, response);
                }
                throw;
            }
            return result;
        }

        internal void Execute()
        {
            try
            {
                HttpWebResponse response = null;
                try
                {
                    response = (HttpWebResponse) this.Request.GetResponse();
                }
                catch (WebException exception)
                {
                    response = (HttpWebResponse) exception.Response;
                    if (response == null)
                    {
                        throw;
                    }
                }
                this.SetHttpWebResponse(Util.NullCheck<HttpWebResponse>(response, InternalError.InvalidGetResponse));
                if (HttpStatusCode.NoContent != this.StatusCode)
                {
                    using (Stream stream = this.httpWebResponse.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            Stream asyncResponseStreamCopy = this.GetAsyncResponseStreamCopy();
                            this.responseStream = asyncResponseStreamCopy;
                            byte[] asyncResponseStreamCopyBuffer = this.GetAsyncResponseStreamCopyBuffer();
                            long num = WebUtil.CopyStream(stream, asyncResponseStreamCopy, ref asyncResponseStreamCopyBuffer);
                            if (this.responseStreamOwner)
                            {
                                if (0L == num)
                                {
                                    this.responseStream = null;
                                }
                                else if (asyncResponseStreamCopy.Position < asyncResponseStreamCopy.Length)
                                {
                                    ((MemoryStream) asyncResponseStreamCopy).SetLength(asyncResponseStreamCopy.Position);
                                }
                            }
                            this.PutAsyncResponseStreamCopyBuffer(asyncResponseStreamCopyBuffer);
                        }
                    }
                }
            }
            catch (Exception exception2)
            {
                base.HandleFailure(exception2);
                throw;
            }
            finally
            {
                base.SetCompleted();
                this.CompletedRequest();
            }
            if (base.Failure != null)
            {
                throw base.Failure;
            }
        }

        protected virtual Stream GetAsyncResponseStreamCopy()
        {
            this.responseStreamOwner = true;
            long contentLength = this.contentLength;
            if ((0L < contentLength) && (contentLength <= 0x7fffffffL))
            {
                return new MemoryStream((int) contentLength);
            }
            return new MemoryStream();
        }

        protected virtual byte[] GetAsyncResponseStreamCopyBuffer() => 
            (Interlocked.Exchange<byte[]>(ref reusableAsyncCopyBuffer, null) ?? new byte[0x1f40]);

        internal MaterializeAtom GetMaterializer(DataServiceContext context, ProjectionPlan plan)
        {
            if (HttpStatusCode.NoContent != this.StatusCode)
            {
                return DataServiceRequest.Materialize(context, this.ServiceRequest.QueryComponents, plan, this.ContentType, this.GetResponseStream());
            }
            return MaterializeAtom.EmptyResults;
        }

        internal QueryOperationResponse<TElement> GetResponse<TElement>(MaterializeAtom results)
        {
            if (this.httpWebResponse != null)
            {
                return new QueryOperationResponse<TElement>(WebUtil.WrapResponseHeaders(this.httpWebResponse), this.ServiceRequest, results) { StatusCode = (int) this.httpWebResponse.StatusCode };
            }
            return null;
        }

        internal Stream GetResponseStream() => 
            this.responseStream;

        internal QueryOperationResponse GetResponseWithType(MaterializeAtom results, Type elementType)
        {
            if (this.httpWebResponse != null)
            {
                Dictionary<string, string> headers = WebUtil.WrapResponseHeaders(this.httpWebResponse);
                QueryOperationResponse response = QueryOperationResponse.GetInstance(elementType, headers, this.ServiceRequest, results);
                response.StatusCode = (int) this.httpWebResponse.StatusCode;
                return response;
            }
            return null;
        }

        internal QueryOperationResponse<TElement> ProcessResult<TElement>(DataServiceContext context, ProjectionPlan plan)
        {
            MaterializeAtom results = DataServiceRequest.Materialize(context, this.ServiceRequest.QueryComponents, plan, this.ContentType, this.GetResponseStream());
            return this.GetResponse<TElement>(results);
        }

        protected virtual void PutAsyncResponseStreamCopyBuffer(byte[] buffer)
        {
            reusableAsyncCopyBuffer = buffer;
        }

        private static void ReadResponseStream(QueryResult queryResult)
        {
            IAsyncResult result;
            byte[] asyncStreamCopyBuffer = queryResult.asyncStreamCopyBuffer;
            Stream asyncResponseStream = queryResult.asyncResponseStream;
            do
            {
                int offset = 0;
                int length = asyncStreamCopyBuffer.Length;
                queryResult.usingBuffer = true;
                result = BaseAsyncResult.InvokeAsync(new BaseAsyncResult.Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(asyncResponseStream.BeginRead), asyncStreamCopyBuffer, offset, length, new AsyncCallback(QueryResult.AsyncEndRead), queryResult);
                queryResult.CompletedSynchronously &= result.CompletedSynchronously;
            }
            while ((result.CompletedSynchronously && !queryResult.IsCompletedInternally) && asyncResponseStream.CanRead);
        }

        protected virtual void SetHttpWebResponse(HttpWebResponse response)
        {
            this.httpWebResponse = response;
            this.statusCode = response.StatusCode;
            this.contentLength = response.ContentLength;
            this.contentType = response.ContentType;
        }

        internal long ContentLength =>
            this.contentLength;

        internal string ContentType =>
            this.contentType;

        internal HttpStatusCode StatusCode =>
            this.statusCode;
    }
}

