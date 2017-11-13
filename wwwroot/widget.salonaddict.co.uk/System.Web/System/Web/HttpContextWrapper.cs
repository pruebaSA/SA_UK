﻿namespace System.Web
{
    using System;
    using System.Web.Hosting;

    internal class HttpContextWrapper : IDisposable
    {
        private bool _needToUndo;
        private HttpContext _savedContext;

        internal HttpContextWrapper(HttpContext context)
        {
            if (context != null)
            {
                this._savedContext = SwitchContext(context);
                this._needToUndo = this._savedContext != context;
            }
        }

        internal static HttpContext SwitchContext(HttpContext context) => 
            (ContextBase.SwitchContext(context) as HttpContext);

        void IDisposable.Dispose()
        {
            if (this._needToUndo)
            {
                SwitchContext(this._savedContext);
                this._savedContext = null;
                this._needToUndo = false;
            }
        }
    }
}

