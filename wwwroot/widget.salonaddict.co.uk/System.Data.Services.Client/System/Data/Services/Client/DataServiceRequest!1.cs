namespace System.Data.Services.Client
{
    using System;

    public sealed class DataServiceRequest<TElement> : DataServiceRequest
    {
        private readonly ProjectionPlan plan;
        private readonly System.Data.Services.Client.QueryComponents queryComponents;

        public DataServiceRequest(Uri requestUri)
        {
            Util.CheckArgumentNull<Uri>(requestUri, "requestUri");
            Type type = typeof(TElement);
            type = ClientConvert.IsKnownType(type) ? type : TypeSystem.GetElementType(type);
            this.queryComponents = new System.Data.Services.Client.QueryComponents(requestUri, Util.DataServiceVersionEmpty, type, null, null);
        }

        internal DataServiceRequest(System.Data.Services.Client.QueryComponents queryComponents, ProjectionPlan plan)
        {
            this.queryComponents = queryComponents;
            this.plan = plan;
        }

        public override Type ElementType =>
            typeof(TElement);

        internal override ProjectionPlan Plan =>
            this.plan;

        internal override System.Data.Services.Client.QueryComponents QueryComponents =>
            this.queryComponents;

        public override Uri RequestUri =>
            this.queryComponents.Uri;
    }
}

