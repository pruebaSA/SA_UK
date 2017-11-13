namespace System.Data.Services.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class DataServiceResponse : IEnumerable<OperationResponse>, IEnumerable
    {
        private bool batchResponse;
        private Dictionary<string, string> headers;
        private IEnumerable<OperationResponse> response;
        private int statusCode;

        internal DataServiceResponse(Dictionary<string, string> headers, int statusCode, IEnumerable<OperationResponse> response, bool batchResponse)
        {
            this.headers = headers ?? new Dictionary<string, string>(EqualityComparer<string>.Default);
            this.statusCode = statusCode;
            this.batchResponse = batchResponse;
            this.response = response;
        }

        public IEnumerator<OperationResponse> GetEnumerator() => 
            this.response.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public IDictionary<string, string> BatchHeaders =>
            this.headers;

        public int BatchStatusCode =>
            this.statusCode;

        public bool IsBatchResponse =>
            this.batchResponse;
    }
}

