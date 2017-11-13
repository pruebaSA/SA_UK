namespace System.Web.Security
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Handlers;
    using System.Web.Management;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class FormsAuthenticationModule : IHttpModule
    {
        private static bool _fAuthChecked;
        private static bool _fAuthRequired;
        private bool _fOnEnterCalled;
        private static string _LoginUrl;
        private const string CONFIG_DEFAULT_COOKIE = ".ASPXAUTH";
        private const string CONFIG_DEFAULT_LOGINURL = "login.aspx";

        public event FormsAuthenticationEventHandler Authenticate;

        public void Dispose()
        {
        }

        private static FormsAuthenticationTicket ExtractTicketFromCookie(HttpContext context, string name, out bool cookielessTicket)
        {
            FormsAuthenticationTicket ticket = null;
            string encryptedTicket = null;
            FormsAuthenticationTicket ticket2;
            bool flag = false;
            bool flag2 = false;
            try
            {
                try
                {
                    cookielessTicket = CookielessHelperClass.UseCookieless(context, false, FormsAuthentication.CookieMode);
                    if (cookielessTicket)
                    {
                        encryptedTicket = context.CookielessHelper.GetCookieValue('F');
                    }
                    else
                    {
                        HttpCookie cookie = context.Request.Cookies[name];
                        if (cookie != null)
                        {
                            encryptedTicket = cookie.Value;
                        }
                    }
                    if ((encryptedTicket != null) && (encryptedTicket.Length > 1))
                    {
                        try
                        {
                            ticket = FormsAuthentication.Decrypt(encryptedTicket);
                        }
                        catch
                        {
                            if (cookielessTicket)
                            {
                                context.CookielessHelper.SetCookieValue('F', null);
                            }
                            else
                            {
                                context.Request.Cookies.Remove(name);
                            }
                            flag2 = true;
                        }
                        if (ticket == null)
                        {
                            flag2 = true;
                        }
                        if (((ticket != null) && !ticket.Expired) && ((cookielessTicket || !FormsAuthentication.RequireSSL) || context.Request.IsSecureConnection))
                        {
                            return ticket;
                        }
                        if ((ticket != null) && ticket.Expired)
                        {
                            flag = true;
                        }
                        ticket = null;
                        if (cookielessTicket)
                        {
                            context.CookielessHelper.SetCookieValue('F', null);
                        }
                        else
                        {
                            context.Request.Cookies.Remove(name);
                        }
                    }
                    if (FormsAuthentication.EnableCrossAppRedirects)
                    {
                        encryptedTicket = context.Request.QueryString[name];
                        if ((encryptedTicket != null) && (encryptedTicket.Length > 1))
                        {
                            if (!cookielessTicket && (FormsAuthentication.CookieMode == HttpCookieMode.AutoDetect))
                            {
                                cookielessTicket = CookielessHelperClass.UseCookieless(context, true, FormsAuthentication.CookieMode);
                            }
                            try
                            {
                                ticket = FormsAuthentication.Decrypt(encryptedTicket);
                            }
                            catch
                            {
                                flag2 = true;
                            }
                            if (ticket == null)
                            {
                                flag2 = true;
                            }
                        }
                        if ((ticket == null) || ticket.Expired)
                        {
                            encryptedTicket = context.Request.Form[name];
                            if ((encryptedTicket != null) && (encryptedTicket.Length > 1))
                            {
                                if (!cookielessTicket && (FormsAuthentication.CookieMode == HttpCookieMode.AutoDetect))
                                {
                                    cookielessTicket = CookielessHelperClass.UseCookieless(context, true, FormsAuthentication.CookieMode);
                                }
                                try
                                {
                                    ticket = FormsAuthentication.Decrypt(encryptedTicket);
                                }
                                catch
                                {
                                    flag2 = true;
                                }
                                if (ticket == null)
                                {
                                    flag2 = true;
                                }
                            }
                        }
                    }
                    if ((ticket == null) || ticket.Expired)
                    {
                        if ((ticket != null) && ticket.Expired)
                        {
                            flag = true;
                        }
                        return null;
                    }
                    if (FormsAuthentication.RequireSSL && !context.Request.IsSecureConnection)
                    {
                        throw new HttpException(System.Web.SR.GetString("Connection_not_secure_creating_secure_cookie"));
                    }
                    if (cookielessTicket)
                    {
                        if (ticket.CookiePath != "/")
                        {
                            ticket = FormsAuthenticationTicket.FromUtc(ticket.Version, ticket.Name, ticket.IssueDateUtc, ticket.ExpirationUtc, ticket.IsPersistent, ticket.UserData, "/");
                            encryptedTicket = FormsAuthentication.Encrypt(ticket);
                        }
                        context.CookielessHelper.SetCookieValue('F', encryptedTicket);
                        string url = FormsAuthentication.RemoveQueryStringVariableFromUrl(GetReturnUrl(context), name);
                        context.Response.Redirect(url);
                    }
                    else
                    {
                        HttpCookie cookie2 = new HttpCookie(name, encryptedTicket) {
                            HttpOnly = true,
                            Path = ticket.CookiePath
                        };
                        if (ticket.IsPersistent)
                        {
                            cookie2.Expires = ticket.Expiration;
                        }
                        cookie2.Secure = FormsAuthentication.RequireSSL;
                        if (FormsAuthentication.CookieDomain != null)
                        {
                            cookie2.Domain = FormsAuthentication.CookieDomain;
                        }
                        context.Response.Cookies.Remove(cookie2.Name);
                        context.Response.Cookies.Add(cookie2);
                    }
                    ticket2 = ticket;
                }
                finally
                {
                    if (flag2)
                    {
                        WebBaseEvent.RaiseSystemEvent(null, 0xfa5, 0xc419);
                    }
                    else if (flag)
                    {
                        WebBaseEvent.RaiseSystemEvent(null, 0xfa5, 0xc41a);
                    }
                }
            }
            catch
            {
                throw;
            }
            return ticket2;
        }

        private static string GetReturnUrl(HttpContext context)
        {
            if (!((context.WorkerRequest != null) && context.WorkerRequest.IsRewriteModuleEnabled))
            {
                return context.Request.PathWithQueryString;
            }
            return context.Request.RawUrl;
        }

        public void Init(HttpApplication app)
        {
            if (!_fAuthChecked)
            {
                AuthenticationSection authentication = RuntimeConfig.GetAppConfig().Authentication;
                authentication.ValidateAuthenticationMode();
                _fAuthRequired = authentication.Mode == AuthenticationMode.Forms;
                _LoginUrl = authentication.Forms.LoginUrl;
                if (_LoginUrl == null)
                {
                    _LoginUrl = "login.aspx";
                }
                _fAuthChecked = true;
            }
            if (_fAuthRequired)
            {
                FormsAuthentication.Initialize();
                app.AuthenticateRequest += new EventHandler(this.OnEnter);
                app.EndRequest += new EventHandler(this.OnLeave);
            }
        }

        private void OnAuthenticate(FormsAuthenticationEventArgs e)
        {
            HttpCookie cookie = null;
            if (this._eventHandler != null)
            {
                this._eventHandler(this, e);
            }
            if (e.Context.User == null)
            {
                if (e.User != null)
                {
                    e.Context.SetPrincipalNoDemand(e.User);
                }
                else
                {
                    FormsAuthenticationTicket tOld = null;
                    bool cookielessTicket = false;
                    try
                    {
                        tOld = ExtractTicketFromCookie(e.Context, FormsAuthentication.FormsCookieName, out cookielessTicket);
                    }
                    catch
                    {
                        tOld = null;
                    }
                    if ((tOld != null) && !tOld.Expired)
                    {
                        FormsAuthenticationTicket ticket = tOld;
                        if (FormsAuthentication.SlidingExpiration)
                        {
                            ticket = FormsAuthentication.RenewTicketIfOld(tOld);
                        }
                        e.Context.SetPrincipalNoDemand(new GenericPrincipal(new FormsIdentity(ticket), new string[0]));
                        if (!cookielessTicket && !ticket.CookiePath.Equals("/"))
                        {
                            cookie = e.Context.Request.Cookies[FormsAuthentication.FormsCookieName];
                            if (cookie != null)
                            {
                                cookie.Path = ticket.CookiePath;
                            }
                        }
                        if (ticket != tOld)
                        {
                            if ((cookielessTicket && (ticket.CookiePath != "/")) && (ticket.CookiePath.Length > 1))
                            {
                                ticket = FormsAuthenticationTicket.FromUtc(ticket.Version, ticket.Name, ticket.IssueDateUtc, ticket.ExpirationUtc, ticket.IsPersistent, ticket.UserData, "/");
                            }
                            string cookieValue = FormsAuthentication.Encrypt(ticket);
                            if (cookielessTicket)
                            {
                                e.Context.CookielessHelper.SetCookieValue('F', cookieValue);
                                e.Context.Response.Redirect(GetReturnUrl(e.Context));
                            }
                            else
                            {
                                if (cookie != null)
                                {
                                    cookie = e.Context.Request.Cookies[FormsAuthentication.FormsCookieName];
                                }
                                if (cookie == null)
                                {
                                    cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieValue) {
                                        Path = ticket.CookiePath
                                    };
                                }
                                if (ticket.IsPersistent)
                                {
                                    cookie.Expires = ticket.Expiration;
                                }
                                cookie.Value = cookieValue;
                                cookie.Secure = FormsAuthentication.RequireSSL;
                                cookie.HttpOnly = true;
                                if (FormsAuthentication.CookieDomain != null)
                                {
                                    cookie.Domain = FormsAuthentication.CookieDomain;
                                }
                                e.Context.Response.Cookies.Remove(cookie.Name);
                                e.Context.Response.Cookies.Add(cookie);
                            }
                        }
                    }
                }
            }
        }

        private void OnEnter(object source, EventArgs eventArgs)
        {
            this._fOnEnterCalled = true;
            HttpApplication application = (HttpApplication) source;
            HttpContext context = application.Context;
            Trace("*******************Request path: " + GetReturnUrl(context));
            this.OnAuthenticate(new FormsAuthenticationEventArgs(context));
            CookielessHelperClass cookielessHelper = context.CookielessHelper;
            if (AuthenticationConfig.AccessingLoginPage(context, _LoginUrl))
            {
                context.SetSkipAuthorizationNoDemand(true, false);
                cookielessHelper.RedirectWithDetectionIfRequired(null, FormsAuthentication.CookieMode);
            }
            if (!context.SkipAuthorization)
            {
                context.SetSkipAuthorizationNoDemand(AssemblyResourceLoader.IsValidWebResourceRequest(context), false);
            }
        }

        private void OnLeave(object source, EventArgs eventArgs)
        {
            if (this._fOnEnterCalled)
            {
                this._fOnEnterCalled = false;
            }
            else
            {
                return;
            }
            HttpApplication application = (HttpApplication) source;
            HttpContext context = application.Context;
            if (!context.Request.IsSecureConnection && (context.Response.Cookies.GetNoCreate(FormsAuthentication.FormsCookieName) != null))
            {
                context.Response.Cache.SetCacheability(HttpCacheability.NoCache, "Set-Cookie");
            }
            if (context.Response.StatusCode == 0x191)
            {
                string returnUrl = GetReturnUrl(context);
                if ((returnUrl.IndexOf("?" + FormsAuthentication.ReturnUrlVar + "=", StringComparison.Ordinal) == -1) && (returnUrl.IndexOf("&" + FormsAuthentication.ReturnUrlVar + "=", StringComparison.Ordinal) == -1))
                {
                    string str3;
                    string strUrl = null;
                    if (!string.IsNullOrEmpty(_LoginUrl))
                    {
                        strUrl = AuthenticationConfig.GetCompleteLoginUrl(context, _LoginUrl);
                    }
                    if ((strUrl == null) || (strUrl.Length <= 0))
                    {
                        throw new HttpException(System.Web.SR.GetString("Auth_Invalid_Login_Url"));
                    }
                    CookielessHelperClass cookielessHelper = context.CookielessHelper;
                    if (strUrl.IndexOf('?') >= 0)
                    {
                        strUrl = FormsAuthentication.RemoveQueryStringVariableFromUrl(strUrl, FormsAuthentication.ReturnUrlVar);
                        str3 = strUrl + "&" + FormsAuthentication.ReturnUrlVar + "=" + HttpUtility.UrlEncode(returnUrl, context.Request.ContentEncoding);
                    }
                    else
                    {
                        str3 = strUrl + "?" + FormsAuthentication.ReturnUrlVar + "=" + HttpUtility.UrlEncode(returnUrl, context.Request.ContentEncoding);
                    }
                    int index = returnUrl.IndexOf('?');
                    if ((index >= 0) && (index < (returnUrl.Length - 1)))
                    {
                        str3 = str3 + "&" + returnUrl.Substring(index + 1);
                    }
                    cookielessHelper.SetCookieValue('F', null);
                    cookielessHelper.RedirectWithDetectionIfRequired(str3, FormsAuthentication.CookieMode);
                    context.Response.Redirect(str3, false);
                }
            }
        }

        private static void Trace(string str)
        {
        }
    }
}

