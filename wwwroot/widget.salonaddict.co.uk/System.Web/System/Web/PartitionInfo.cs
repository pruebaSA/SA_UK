namespace System.Web
{
    using System;
    using System.Web.Util;

    internal class PartitionInfo : IDisposable, IPartitionInfo
    {
        private ResourcePool _rpool;

        internal PartitionInfo(ResourcePool rpool)
        {
            this._rpool = rpool;
        }

        public void Dispose()
        {
            if (this._rpool != null)
            {
                lock (this)
                {
                    if (this._rpool != null)
                    {
                        this._rpool.Dispose();
                        this._rpool = null;
                    }
                }
            }
        }

        internal object RetrieveResource() => 
            this._rpool.RetrieveResource();

        internal void StoreResource(IDisposable o)
        {
            this._rpool.StoreResource(o);
        }

        string IPartitionInfo.GetTracingPartitionString() => 
            this.TracingPartitionString;

        protected virtual string TracingPartitionString =>
            string.Empty;
    }
}

