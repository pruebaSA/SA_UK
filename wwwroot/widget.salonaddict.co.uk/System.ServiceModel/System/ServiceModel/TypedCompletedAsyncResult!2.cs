namespace System.ServiceModel
{
    using System;
    using System.Runtime.InteropServices;

    internal class TypedCompletedAsyncResult<T, U> : TypedAsyncResult<T>
    {
        private U parameter;

        public TypedCompletedAsyncResult(T data, U parameter, AsyncCallback callback, object state) : base(callback, state)
        {
            this.parameter = parameter;
            base.Complete(data, true);
        }

        public static T End(IAsyncResult result, out U parameter)
        {
            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("result"));
            }
            TypedCompletedAsyncResult<T, U> result2 = result as TypedCompletedAsyncResult<T, U>;
            if (result2 == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.ServiceModel.SR.GetString("InvalidAsyncResult"), "result"));
            }
            parameter = result2.parameter;
            return TypedAsyncResult<T>.End(result2);
        }
    }
}

