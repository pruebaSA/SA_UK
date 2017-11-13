namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml;

    public abstract class DataServiceRequest
    {
        internal DataServiceRequest()
        {
        }

        internal IAsyncResult BeginExecute(object source, DataServiceContext context, AsyncCallback callback, object state)
        {
            QueryResult result = this.CreateResult(source, context, callback, state);
            result.BeginExecute();
            return result;
        }

        private QueryResult CreateResult(object source, DataServiceContext context, AsyncCallback callback, object state) => 
            new QueryResult(source, "Execute", this, context.CreateRequest(this.QueryComponents.Uri, "GET", false, null, this.QueryComponents.Version, false), callback, state);

        internal static IEnumerable<TElement> EndExecute<TElement>(object source, DataServiceContext context, IAsyncResult asyncResult)
        {
            QueryResult result = null;
            try
            {
                result = QueryResult.EndExecute<TElement>(source, asyncResult);
                return result.ProcessResult<TElement>(context, result.ServiceRequest.Plan);
            }
            catch (DataServiceQueryException exception)
            {
                Exception innerException = exception;
                while (innerException.InnerException != null)
                {
                    innerException = innerException.InnerException;
                }
                DataServiceClientException exception3 = innerException as DataServiceClientException;
                if ((!context.IgnoreResourceNotFoundException || (exception3 == null)) || (exception3.StatusCode != 0x194))
                {
                    throw;
                }
                QueryOperationResponse response = new QueryOperationResponse<TElement>(new Dictionary<string, string>(exception.Response.Headers), exception.Response.Query, MaterializeAtom.EmptyResults) {
                    StatusCode = 0x194
                };
                return (IEnumerable<TElement>) response;
            }
        }

        internal QueryOperationResponse<TElement> Execute<TElement>(DataServiceContext context, System.Data.Services.Client.QueryComponents queryComponents)
        {
            QueryResult result = null;
            QueryOperationResponse<TElement> response2;
            try
            {
                result = new DataServiceRequest<TElement>(queryComponents, this.Plan).CreateResult(this, context, null, null);
                result.Execute();
                response2 = result.ProcessResult<TElement>(context, this.Plan);
            }
            catch (InvalidOperationException exception)
            {
                QueryOperationResponse response = result.GetResponse<TElement>(MaterializeAtom.EmptyResults);
                if (response == null)
                {
                    throw;
                }
                if (context.IgnoreResourceNotFoundException)
                {
                    DataServiceClientException exception2 = exception as DataServiceClientException;
                    if ((exception2 != null) && (exception2.StatusCode == 0x194))
                    {
                        return (QueryOperationResponse<TElement>) response;
                    }
                }
                response.Error = exception;
                throw new DataServiceQueryException(Strings.DataServiceException_GeneralError, exception, response);
            }
            return response2;
        }

        internal static DataServiceRequest GetInstance(Type elementType, Uri requestUri) => 
            ((DataServiceRequest) Activator.CreateInstance(typeof(DataServiceRequest<>).MakeGenericType(new Type[] { elementType }), new object[] { requestUri }));

        internal long GetQuerySetCount(DataServiceContext context)
        {
            long num2;
            this.QueryComponents.Version = Util.DataServiceVersion2;
            QueryResult result = null;
            DataServiceRequest<long> serviceRequest = new DataServiceRequest<long>(this.QueryComponents, null);
            HttpWebRequest request = context.CreateRequest(this.QueryComponents.Uri, "GET", false, null, this.QueryComponents.Version, false);
            request.Accept = "text/plain";
            result = new QueryResult(this, "Execute", serviceRequest, request, null, null);
            try
            {
                result.Execute();
                if (HttpStatusCode.NoContent == result.StatusCode)
                {
                    throw new DataServiceQueryException(Strings.DataServiceRequest_FailGetCount, result.Failure);
                }
                StreamReader reader = new StreamReader(result.GetResponseStream());
                long num = -1L;
                try
                {
                    num = XmlConvert.ToInt64(reader.ReadToEnd());
                }
                finally
                {
                    reader.Close();
                }
                num2 = num;
            }
            catch (InvalidOperationException exception)
            {
                QueryOperationResponse response = null;
                response = result.GetResponse<long>(MaterializeAtom.EmptyResults);
                if (response != null)
                {
                    response.Error = exception;
                    throw new DataServiceQueryException(Strings.DataServiceException_GeneralError, exception, response);
                }
                throw;
            }
            return num2;
        }

        internal static MaterializeAtom Materialize(DataServiceContext context, System.Data.Services.Client.QueryComponents queryComponents, ProjectionPlan plan, string contentType, Stream response)
        {
            string mime = null;
            Encoding encoding = null;
            if (!string.IsNullOrEmpty(contentType))
            {
                HttpProcessUtility.ReadContentType(contentType, out mime, out encoding);
            }
            if ((string.Equals(mime, "application/atom+xml", StringComparison.OrdinalIgnoreCase) || string.Equals(mime, "application/xml", StringComparison.OrdinalIgnoreCase)) && (response != null))
            {
                return new MaterializeAtom(context, XmlUtil.CreateXmlReader(response, encoding), queryComponents, plan, context.MergeOption);
            }
            return MaterializeAtom.EmptyResults;
        }

        public override string ToString() => 
            this.QueryComponents.Uri.ToString();

        public abstract Type ElementType { get; }

        internal abstract ProjectionPlan Plan { get; }

        internal abstract System.Data.Services.Client.QueryComponents QueryComponents { get; }

        public abstract Uri RequestUri { get; }
    }
}

