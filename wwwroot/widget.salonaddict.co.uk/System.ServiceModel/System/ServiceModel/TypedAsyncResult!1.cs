namespace System.ServiceModel
{
    using System;

    internal abstract class TypedAsyncResult<T> : AsyncResult
    {
        private T data;

        public TypedAsyncResult(AsyncCallback callback, object state) : base(callback, state)
        {
        }

        protected void Complete(T data, bool completedSynchronously)
        {
            this.data = data;
            base.Complete(completedSynchronously);
        }

        public static T End(IAsyncResult result) => 
            AsyncResult.End<TypedAsyncResult<T>>(result).Data;

        public T Data =>
            this.data;
    }
}

