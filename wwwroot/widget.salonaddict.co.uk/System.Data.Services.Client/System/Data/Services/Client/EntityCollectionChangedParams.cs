namespace System.Data.Services.Client
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;

    public sealed class EntityCollectionChangedParams
    {
        private readonly NotifyCollectionChangedAction action;
        private readonly ICollection collection;
        private readonly DataServiceContext context;
        private readonly string propertyName;
        private readonly object sourceEntity;
        private readonly string sourceEntitySet;
        private readonly object targetEntity;
        private readonly string targetEntitySet;

        internal EntityCollectionChangedParams(DataServiceContext context, object sourceEntity, string propertyName, string sourceEntitySet, ICollection collection, object targetEntity, string targetEntitySet, NotifyCollectionChangedAction action)
        {
            this.context = context;
            this.sourceEntity = sourceEntity;
            this.propertyName = propertyName;
            this.sourceEntitySet = sourceEntitySet;
            this.collection = collection;
            this.targetEntity = targetEntity;
            this.targetEntitySet = targetEntitySet;
            this.action = action;
        }

        public NotifyCollectionChangedAction Action =>
            this.action;

        public ICollection Collection =>
            this.collection;

        public DataServiceContext Context =>
            this.context;

        public string PropertyName =>
            this.propertyName;

        public object SourceEntity =>
            this.sourceEntity;

        public string SourceEntitySet =>
            this.sourceEntitySet;

        public object TargetEntity =>
            this.targetEntity;

        public string TargetEntitySet =>
            this.targetEntitySet;
    }
}

