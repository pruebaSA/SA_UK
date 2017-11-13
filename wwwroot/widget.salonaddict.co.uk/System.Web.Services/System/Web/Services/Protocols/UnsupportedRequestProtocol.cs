namespace System.Web.Services.Protocols
{
    using System;
    using System.IO;

    internal class UnsupportedRequestProtocol : ServerProtocol
    {
        private int httpCode;

        internal UnsupportedRequestProtocol(int httpCode)
        {
            this.httpCode = httpCode;
        }

        internal override bool Initialize() => 
            true;

        internal override object[] ReadParameters() => 
            new object[0];

        internal override bool WriteException(Exception e, Stream outputStream) => 
            false;

        internal override void WriteReturns(object[] returnValues, Stream outputStream)
        {
        }

        internal int HttpCode =>
            this.httpCode;

        internal override bool IsOneWay =>
            false;

        internal override LogicalMethodInfo MethodInfo =>
            null;

        internal override System.Web.Services.Protocols.ServerType ServerType =>
            null;
    }
}

