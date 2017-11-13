namespace System.Runtime.Remoting.Services
{
    using System.ComponentModel;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Security.Principal;
    using System.Web;
    using System.Web.SessionState;

    public class RemotingService : Component
    {
        public HttpApplicationState Application =>
            this.Context.Application;

        public HttpContext Context
        {
            get
            {
                HttpContext current = HttpContext.Current;
                if (current == null)
                {
                    throw new RemotingException(CoreChannel.GetResourceString("Remoting_HttpContextNotAvailable"));
                }
                return current;
            }
        }

        public HttpServerUtility Server =>
            this.Context.Server;

        public HttpSessionState Session =>
            this.Context.Session;

        public IPrincipal User =>
            this.Context.User;
    }
}

