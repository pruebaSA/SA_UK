namespace System.Web.Util
{
    using System;
    using System.Web;

    internal static class ExceptionUtil
    {
        internal static ArgumentException ParameterInvalid(string parameter) => 
            new ArgumentException(System.Web.SR.GetString("Parameter_Invalid", new object[] { parameter }), parameter);

        internal static ArgumentException ParameterNullOrEmpty(string parameter) => 
            new ArgumentException(System.Web.SR.GetString("Parameter_NullOrEmpty", new object[] { parameter }), parameter);

        internal static ArgumentException PropertyInvalid(string property) => 
            new ArgumentException(System.Web.SR.GetString("Property_Invalid", new object[] { property }), property);

        internal static ArgumentException PropertyNullOrEmpty(string property) => 
            new ArgumentException(System.Web.SR.GetString("Property_NullOrEmpty", new object[] { property }), property);

        internal static InvalidOperationException UnexpectedError(string methodName) => 
            new InvalidOperationException(System.Web.SR.GetString("Unexpected_Error", new object[] { methodName }));
    }
}

