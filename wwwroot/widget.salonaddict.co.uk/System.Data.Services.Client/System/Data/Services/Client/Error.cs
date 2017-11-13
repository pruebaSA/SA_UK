namespace System.Data.Services.Client
{
    using System;
    using System.Linq.Expressions;

    internal static class Error
    {
        internal static ArgumentException Argument(string message, string parameterName) => 
            Trace<ArgumentException>(new ArgumentException(message, parameterName));

        internal static Exception ArgumentNull(string paramName) => 
            new ArgumentNullException(paramName);

        internal static Exception ArgumentOutOfRange(string paramName) => 
            new ArgumentOutOfRangeException(paramName);

        internal static InvalidOperationException BatchStreamContentExpected(BatchStreamState state) => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_ContentExpected(state.ToString()));

        internal static InvalidOperationException BatchStreamContentUnexpected(BatchStreamState state) => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_ContentUnexpected(state.ToString()));

        internal static InvalidOperationException BatchStreamGetMethodNotSupportInChangeset() => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_GetMethodNotSupportedInChangeset);

        internal static InvalidOperationException BatchStreamInternalBufferRequestTooSmall() => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_InternalBufferRequestTooSmall);

        internal static InvalidOperationException BatchStreamInvalidBatchFormat() => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_InvalidBatchFormat);

        internal static InvalidOperationException BatchStreamInvalidContentLengthSpecified(string contentLength) => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_InvalidContentLengthSpecified(contentLength));

        internal static InvalidOperationException BatchStreamInvalidContentTypeSpecified(string headerName, string headerValue, string mime1, string mime2) => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_InvalidContentTypeSpecified(headerName, headerValue, mime1, mime2));

        internal static InvalidOperationException BatchStreamInvalidDelimiter(string delimiter) => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_InvalidDelimiter(delimiter));

        internal static InvalidOperationException BatchStreamInvalidHeaderValueSpecified(string headerValue) => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_InvalidHeaderValueSpecified(headerValue));

        internal static InvalidOperationException BatchStreamInvalidHttpMethodName(string methodName) => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_InvalidHttpMethodName(methodName));

        internal static InvalidOperationException BatchStreamInvalidHttpVersionSpecified(string actualVersion, string expectedVersion) => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_InvalidHttpVersionSpecified(actualVersion, expectedVersion));

        internal static InvalidOperationException BatchStreamInvalidMethodHeaderSpecified(string header) => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_InvalidMethodHeaderSpecified(header));

        internal static InvalidOperationException BatchStreamInvalidNumberOfHeadersAtChangeSetStart(string header1, string header2) => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_InvalidNumberOfHeadersAtChangeSetStart(header1, header2));

        internal static InvalidOperationException BatchStreamInvalidNumberOfHeadersAtOperationStart(string header1, string header2) => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_InvalidNumberOfHeadersAtOperationStart(header1, header2));

        internal static InvalidOperationException BatchStreamInvalidOperationHeaderSpecified() => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_InvalidOperationHeaderSpecified);

        internal static InvalidOperationException BatchStreamMissingBoundary() => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_MissingBoundary);

        internal static InvalidOperationException BatchStreamMissingContentTypeHeader(string headerName) => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_MissingContentTypeHeader(headerName));

        internal static InvalidOperationException BatchStreamMissingEndChangesetDelimiter() => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_MissingEndChangesetDelimiter);

        internal static InvalidOperationException BatchStreamMissingOrInvalidContentEncodingHeader(string headerName, string headerValue) => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_MissingOrInvalidContentEncodingHeader(headerName, headerValue));

        internal static InvalidOperationException BatchStreamMoreDataAfterEndOfBatch() => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_MoreDataAfterEndOfBatch);

        internal static InvalidOperationException BatchStreamOnlyGETOperationsCanBeSpecifiedInBatch() => 
            InvalidOperation(System.Data.Services.Client.Strings.BatchStream_OnlyGETOperationsCanBeSpecifiedInBatch);

        internal static InvalidOperationException HttpHeaderFailure(int errorCode, string message) => 
            Trace<InvalidOperationException>(new InvalidOperationException(message));

        internal static InvalidOperationException InternalError(System.Data.Services.Client.InternalError value) => 
            InvalidOperation(System.Data.Services.Client.Strings.Context_InternalError((int) value));

        internal static InvalidOperationException InvalidOperation(string message) => 
            Trace<InvalidOperationException>(new InvalidOperationException(message));

        internal static InvalidOperationException InvalidOperation(string message, Exception innerException) => 
            Trace<InvalidOperationException>(new InvalidOperationException(message, innerException));

        internal static NotSupportedException MethodNotSupported(MethodCallExpression m) => 
            NotSupported(System.Data.Services.Client.Strings.ALinq_MethodNotSupported(m.Method.Name));

        internal static Exception NotImplemented() => 
            new NotImplementedException();

        internal static Exception NotSupported() => 
            new NotSupportedException();

        internal static NotSupportedException NotSupported(string message) => 
            Trace<NotSupportedException>(new NotSupportedException(message));

        internal static void ThrowBatchExpectedResponse(System.Data.Services.Client.InternalError value)
        {
            throw InvalidOperation(System.Data.Services.Client.Strings.Batch_ExpectedResponse((int) value));
        }

        internal static void ThrowBatchUnexpectedContent(System.Data.Services.Client.InternalError value)
        {
            throw InvalidOperation(System.Data.Services.Client.Strings.Batch_UnexpectedContent((int) value));
        }

        internal static void ThrowInternalError(System.Data.Services.Client.InternalError value)
        {
            throw InternalError(value);
        }

        internal static void ThrowObjectDisposed(Type type)
        {
            throw Trace<ObjectDisposedException>(new ObjectDisposedException(type.ToString()));
        }

        private static T Trace<T>(T exception) where T: Exception => 
            exception;
    }
}

