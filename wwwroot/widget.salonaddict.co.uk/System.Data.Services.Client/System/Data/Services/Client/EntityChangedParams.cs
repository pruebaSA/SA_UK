namespace System.Data.Services.Client
{
    using System;

    public sealed class EntityChangedParams
    {
        private readonly DataServiceContext context;
        private readonly object entity;
        private readonly string propertyName;
        private readonly object propertyValue;
        private readonly string sourceEntitySet;
        private readonly string targetEntitySet;

        internal EntityChangedParams(DataServiceContext context, object entity, string propertyName, object propertyValue, string sourceEntitySet, string targetEntitySet)
        {
            this.context = context;
            this.entity = entity;
            this.propertyName = propertyName;
            this.propertyValue = propertyValue;
            this.sourceEntitySet = sourceEntitySet;
            this.targetEntitySet = targetEntitySet;
        }

        public DataServiceContext Context =>
            this.context;

        public object Entity =>
            this.entity;

        public string PropertyName =>
            this.propertyName;

        public object PropertyValue =>
            this.propertyValue;

        public string SourceEntitySet =>
            this.sourceEntitySet;

        public string TargetEntitySet =>
            this.targetEntitySet;
    }
}

