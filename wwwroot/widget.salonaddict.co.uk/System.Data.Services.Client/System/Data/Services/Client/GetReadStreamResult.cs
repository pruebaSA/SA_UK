namespace System.Data.Services.Client
{
    using System;
    using System.Net;

    internal class GetReadStreamResult : BaseAsyncResult
    {
        private readonly HttpWebRequest request;
        private HttpWebResponse response;

        internal GetReadStreamResult(object source, string method, HttpWebRequest request, AsyncCallback callback, object state) : base(source, method, callback, state)
        {
            this.request = request;
            base.Abortable = request;
        }

        private static void AsyncEndGetResponse(IAsyncResult asyncResult)
        {
            GetReadStreamResult asyncState = asyncResult.AsyncState as GetReadStreamResult;
            try
            {
                asyncState.CompletedSynchronously &= asyncResult.CompletedSynchronously;
                HttpWebRequest request = Util.NullCheck<HttpWebRequest>(asyncState.request, InternalError.InvalidEndGetResponseRequest);
                HttpWebResponse webResponse = null;
                try
                {
                    webResponse = (HttpWebResponse) request.EndGetResponse(asyncResult);
                }
                catch (WebException exception)
                {
                    webResponse = (HttpWebResponse) exception.Response;
                    if (webResponse == null)
                    {
                        throw;
                    }
                }
                asyncState.SetHttpWebResponse(webResponse);
                asyncState.SetCompleted();
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

        internal void Begin()
        {
            try
            {
                IAsyncResult result = BaseAsyncResult.InvokeAsync(new Func<AsyncCallback, object, IAsyncResult>(this.request.BeginGetResponse), new AsyncCallback(GetReadStreamResult.AsyncEndGetResponse), this);
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

        protected override void CompletedRequest()
        {
            if (this.response != null)
            {
                InvalidOperationException e = null;
                if (!WebUtil.SuccessStatusCode(this.response.StatusCode))
                {
                    e = DataServiceContext.GetResponseText(new Func<Stream>(this.response.GetResponseStream), this.response.StatusCode);
                }
                if (e != null)
                {
                    this.response.Close();
                    base.HandleFailure(e);
                }
            }
        }

        internal DataServiceStreamResponse End()
        {
            if (this.response != null)
            {
                return new DataServiceStreamResponse(this.response);
            }
            return null;
        }

        internal DataServiceStreamResponse Execute()
        {
            try
            {
                HttpWebResponse webResponse = null;
                try
                {
                    webResponse = (HttpWebResponse) this.request.GetResponse();
                }
                catch (WebException exception)
                {
                    webResponse = (HttpWebResponse) exception.Response;
                    if (webResponse == null)
                    {
                        throw;
                    }
                }
                this.SetHttpWebResponse(webResponse);
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
            return this.End();
        }

        private void SetHttpWebResponse(HttpWebResponse webResponse)
        {
            this.response = webResponse;
        }
    }
}

