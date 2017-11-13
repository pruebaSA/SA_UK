namespace System.Data.Services.Client
{
    using System;

    public sealed class DataServiceQueryContinuation<T> : DataServiceQueryContinuation
    {
        internal DataServiceQueryContinuation(Uri nextLinkUri, ProjectionPlan plan) : base(nextLinkUri, plan)
        {
        }

        internal override Type ElementType =>
            typeof(T);
    }
}

