namespace System.ServiceModel
{
    using System;

    internal class TypedCompletedAsyncResult<T> : TypedAsyncResult<T>
    {
        public TypedCompletedAsyncResult(T data, AsyncCallback callback, object state) : base(callback, state)
        {
            base.Complete(data, true);
        }

        public static T End(IAsyncResult result)
        {
            TypedCompletedAsyncResult<T> result2 = result as TypedCompletedAsyncResult<T>;
            if (result2 == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.ServiceModel.SR.GetString("InvalidAsyncResult"), "result"));
            }
            return TypedAsyncResult<T>.End(result2);
        }
    }
}

