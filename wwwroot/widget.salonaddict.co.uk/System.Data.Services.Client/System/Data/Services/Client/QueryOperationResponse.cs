namespace System.Data.Services.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class QueryOperationResponse : OperationResponse, IEnumerable
    {
        private readonly DataServiceRequest query;
        private readonly MaterializeAtom results;

        internal QueryOperationResponse(Dictionary<string, string> headers, DataServiceRequest query, MaterializeAtom results) : base(headers)
        {
            this.query = query;
            this.results = results;
        }

        public DataServiceQueryContinuation GetContinuation() => 
            this.results.GetContinuation(null);

        public DataServiceQueryContinuation<T> GetContinuation<T>(IEnumerable<T> collection) => 
            ((DataServiceQueryContinuation<T>) this.results.GetContinuation(collection));

        public DataServiceQueryContinuation GetContinuation(IEnumerable collection) => 
            this.results.GetContinuation(collection);

        public IEnumerator GetEnumerator() => 
            this.Results.GetEnumerator();

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static QueryOperationResponse GetInstance(Type elementType, Dictionary<string, string> headers, DataServiceRequest query, MaterializeAtom results) => 
            ((QueryOperationResponse) Activator.CreateInstance(typeof(QueryOperationResponse<>).MakeGenericType(new Type[] { elementType }), BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { headers, query, results }, CultureInfo.InvariantCulture));

        public DataServiceRequest Query =>
            this.query;

        internal MaterializeAtom Results
        {
            get
            {
                if (base.Error != null)
                {
                    throw Error.InvalidOperation(Strings.Context_BatchExecuteError, base.Error);
                }
                return this.results;
            }
        }

        public virtual long TotalCount
        {
            get
            {
                throw new NotSupportedException();
            }
        }
    }
}

