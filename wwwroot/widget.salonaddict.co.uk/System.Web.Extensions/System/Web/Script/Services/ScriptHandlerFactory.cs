namespace System.Web.Script.Services
{
    using System;
    using System.Web;
    using System.Web.SessionState;

    internal class ScriptHandlerFactory : IHttpHandlerFactory
    {
        private IHttpHandlerFactory _restHandlerFactory = new RestHandlerFactory();
        private IHttpHandlerFactory _webServiceHandlerFactory = new WebServiceHandlerFactory();

        public virtual IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            IHttpHandlerFactory factory;
            if (RestHandlerFactory.IsRestRequest(context))
            {
                factory = this._restHandlerFactory;
            }
            else
            {
                factory = this._webServiceHandlerFactory;
            }
            IHttpHandler originalHandler = factory.GetHandler(context, requestType, url, pathTranslated);
            bool flag = originalHandler is IRequiresSessionState;
            if (originalHandler is IHttpAsyncHandler)
            {
                if (flag)
                {
                    return new AsyncHandlerWrapperWithSession(originalHandler, factory);
                }
                return new AsyncHandlerWrapper(originalHandler, factory);
            }
            if (flag)
            {
                return new HandlerWrapperWithSession(originalHandler, factory);
            }
            return new HandlerWrapper(originalHandler, factory);
        }

        public virtual void ReleaseHandler(IHttpHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            ((HandlerWrapper) handler).ReleaseHandler();
        }

        private class AsyncHandlerWrapper : ScriptHandlerFactory.HandlerWrapper, IHttpAsyncHandler, IHttpHandler
        {
            internal AsyncHandlerWrapper(IHttpHandler originalHandler, IHttpHandlerFactory originalFactory) : base(originalHandler, originalFactory)
            {
            }

            public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) => 
                ((IHttpAsyncHandler) base._originalHandler).BeginProcessRequest(context, cb, extraData);

            public void EndProcessRequest(IAsyncResult result)
            {
                ((IHttpAsyncHandler) base._originalHandler).EndProcessRequest(result);
            }
        }

        private class AsyncHandlerWrapperWithSession : ScriptHandlerFactory.AsyncHandlerWrapper, IRequiresSessionState
        {
            internal AsyncHandlerWrapperWithSession(IHttpHandler originalHandler, IHttpHandlerFactory originalFactory) : base(originalHandler, originalFactory)
            {
            }
        }

        internal class HandlerWrapper : IHttpHandler
        {
            private IHttpHandlerFactory _originalFactory;
            protected IHttpHandler _originalHandler;

            internal HandlerWrapper(IHttpHandler originalHandler, IHttpHandlerFactory originalFactory)
            {
                this._originalFactory = originalFactory;
                this._originalHandler = originalHandler;
            }

            public void ProcessRequest(HttpContext context)
            {
                this._originalHandler.ProcessRequest(context);
            }

            internal void ReleaseHandler()
            {
                this._originalFactory.ReleaseHandler(this._originalHandler);
            }

            public bool IsReusable =>
                this._originalHandler.IsReusable;
        }

        internal class HandlerWrapperWithSession : ScriptHandlerFactory.HandlerWrapper, IRequiresSessionState
        {
            internal HandlerWrapperWithSession(IHttpHandler originalHandler, IHttpHandlerFactory originalFactory) : base(originalHandler, originalFactory)
            {
            }
        }
    }
}

