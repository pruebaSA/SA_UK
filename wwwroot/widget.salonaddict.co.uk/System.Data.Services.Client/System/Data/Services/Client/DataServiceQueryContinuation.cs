namespace System.Data.Services.Client
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("{NextLinkUri}")]
    public abstract class DataServiceQueryContinuation
    {
        private readonly Uri nextLinkUri;
        private readonly ProjectionPlan plan;

        internal DataServiceQueryContinuation(Uri nextLinkUri, ProjectionPlan plan)
        {
            this.nextLinkUri = nextLinkUri;
            this.plan = plan;
        }

        internal static DataServiceQueryContinuation Create(Uri nextLinkUri, ProjectionPlan plan)
        {
            if (nextLinkUri == null)
            {
                return null;
            }
            return (DataServiceQueryContinuation) Util.ConstructorInvoke(typeof(DataServiceQueryContinuation<>).MakeGenericType(new Type[] { plan.ProjectedType }).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0], new object[] { nextLinkUri, plan });
        }

        internal QueryComponents CreateQueryComponents() => 
            new QueryComponents(this.NextLinkUri, Util.DataServiceVersionEmpty, this.Plan.LastSegmentType, null, null);

        public override string ToString() => 
            this.NextLinkUri.ToString();

        internal abstract Type ElementType { get; }

        public Uri NextLinkUri =>
            this.nextLinkUri;

        internal ProjectionPlan Plan =>
            this.plan;
    }
}

