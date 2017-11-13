namespace System.ServiceModel.Activation
{
    using System;
    using System.Net;
    using System.Security;

    internal class HostedRequestContainer
    {
        private bool isClosed;
        private HostedHttpRequestAsyncResult result;
        private object thisLock;

        public HostedRequestContainer(HostedHttpRequestAsyncResult result)
        {
            this.result = result;
            this.thisLock = new object();
        }

        public void Close()
        {
            lock (this.ThisLock)
            {
                this.isClosed = true;
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        public void CopyHeaders(WebHeaderCollection headers)
        {
            if (!this.isClosed)
            {
                lock (this.ThisLock)
                {
                    if (!this.isClosed)
                    {
                        headers.Add(this.result.Application.Request.Headers);
                    }
                }
            }
        }

        [SecurityTreatAsSafe, SecurityCritical]
        public string GetRemoteAddress()
        {
            if (!this.isClosed)
            {
                lock (this.ThisLock)
                {
                    if (!this.isClosed)
                    {
                        return this.result.Application.Request.UserHostAddress;
                    }
                }
            }
            return string.Empty;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        public int GetRemotePort()
        {
            int result = 0;
            if (this.isClosed)
            {
                return result;
            }
            lock (this.ThisLock)
            {
                if (this.isClosed)
                {
                    return result;
                }
                string str = this.result.Application.Request.ServerVariables["REMOTE_PORT"];
                if (!string.IsNullOrEmpty(str) && int.TryParse(str, out result))
                {
                    return result;
                }
                return 0;
            }
        }

        private object ThisLock =>
            this.thisLock;
    }
}

