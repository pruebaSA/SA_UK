namespace System.Data.Linq.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public abstract class MappingSource
    {
        private MetaModel primaryModel;
        private ReaderWriterLock rwlock;
        private Dictionary<Type, MetaModel> secondaryModels;

        protected MappingSource()
        {
        }

        protected abstract MetaModel CreateModel(Type dataContextType);
        public MetaModel GetModel(Type dataContextType)
        {
            MetaModel model2;
            if (dataContextType == null)
            {
                throw Error.ArgumentNull("dataContextType");
            }
            MetaModel model = null;
            if (this.primaryModel == null)
            {
                model = this.CreateModel(dataContextType);
                Interlocked.CompareExchange<MetaModel>(ref this.primaryModel, model, null);
            }
            if (this.primaryModel.ContextType == dataContextType)
            {
                return this.primaryModel;
            }
            if (this.secondaryModels == null)
            {
                Interlocked.CompareExchange<Dictionary<Type, MetaModel>>(ref this.secondaryModels, new Dictionary<Type, MetaModel>(), null);
            }
            if (this.rwlock == null)
            {
                Interlocked.CompareExchange<ReaderWriterLock>(ref this.rwlock, new ReaderWriterLock(), null);
            }
            this.rwlock.AcquireReaderLock(-1);
            try
            {
                if (this.secondaryModels.TryGetValue(dataContextType, out model2))
                {
                    return model2;
                }
            }
            finally
            {
                this.rwlock.ReleaseReaderLock();
            }
            this.rwlock.AcquireWriterLock(-1);
            try
            {
                if (this.secondaryModels.TryGetValue(dataContextType, out model2))
                {
                    return model2;
                }
                if (model == null)
                {
                    model = this.CreateModel(dataContextType);
                }
                this.secondaryModels.Add(dataContextType, model);
            }
            finally
            {
                this.rwlock.ReleaseWriterLock();
            }
            return model;
        }
    }
}

