namespace IFRAME.Controllers
{
    using SA.BAL;
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Web;
    using System.Web.Security;

    public class IFRMContextHttpModule : IHttpModule
    {
        private void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (IFRMHelper.IsContentPageRequested())
            {
                string str = string.Empty;
                string str2 = string.Empty;
                string str3 = string.Empty;
                HttpContext current = HttpContext.Current;
                HttpApplication application = (HttpApplication) sender;
                if (current != null)
                {
                    HttpRequest request = current.Request;
                    if (request != null)
                    {
                        NameValueCollection queryString = request.QueryString;
                        if (queryString != null)
                        {
                            if (queryString["api_key"] != null)
                            {
                                str = queryString["api_key"].Trim();
                                if (str != string.Empty)
                                {
                                    str = str.ToLowerInvariant();
                                }
                            }
                            if (queryString["lang"] != null)
                            {
                                str3 = queryString["lang"].Trim();
                                if (str3 != string.Empty)
                                {
                                    str3 = str3.Replace('.', '-').ToLowerInvariant();
                                }
                            }
                            if (queryString["theme"] != null)
                            {
                                str2 = queryString["theme"].Trim();
                                if (str2 != string.Empty)
                                {
                                    str2 = str2.ToLowerInvariant();
                                }
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(str) && (application.Request.Url.LocalPath != "/apikeyinstall.aspx"))
                {
                    if (application.Request.Url.IsLoopback)
                    {
                        str = "1abba94aca5943Cab4f5500c740c2cf0";
                    }
                    else
                    {
                        application.Response.Redirect("~/apikeyinstall.aspx", true);
                    }
                }
                IFRMContext.Current.APIKey = str;
                string str4 = IFRAME.Controllers.Settings.IFRMCONTEXT_DEFAULT_WORKING_LANGUAGE;
                if (str3 == string.Empty)
                {
                    if (str4 != string.Empty)
                    {
                        IFRMContext.Current.WorkingLanguage = str4;
                    }
                }
                else if (str3 != "en-GB")
                {
                    IFRMContext.Current.WorkingLanguage = str4;
                }
                else
                {
                    IFRMContext.Current.WorkingLanguage = str3;
                }
                IFRMContext.Current.SetCulture(new CultureInfo(IFRMContext.Current.WorkingLanguage));
                string str5 = IFRAME.Controllers.Settings.IFRMCONTEXT_DEFAULT_WORKING_THEME;
                switch (str2)
                {
                    case "white":
                    case "black":
                    case "pink":
                    case "green":
                    case "red":
                    case "blue":
                        IFRMContext.Current.WorkingTheme = str2;
                        break;

                    case string.Empty:
                        if (str5 != string.Empty)
                        {
                            IFRMContext.Current.WorkingTheme = str5;
                        }
                        break;

                    default:
                        IFRMContext.Current.WorkingTheme = str5;
                        break;
                }
                if (!string.IsNullOrEmpty(IFRMContext.Current.APIKey))
                {
                    WidgetApiKeyDB widgetApiKeyByVerificationToken = IoC.Resolve<IUserManager>().GetWidgetApiKeyByVerificationToken(IFRMContext.Current.APIKey);
                    if (widgetApiKeyByVerificationToken != null)
                    {
                        IFRMContext.Current.Salon = IoC.Resolve<ISalonManager>().GetSalonById(widgetApiKeyByVerificationToken.SalonId);
                    }
                }
                string localPath = application.Request.Url.LocalPath;
                if ((IFRMContext.Current.Salon == null) && (localPath != "/apikeyinstall.aspx"))
                {
                    application.Response.Redirect("~/apikeyinstall.aspx", true);
                }
                if (IFRMContext.Current.Salon != null)
                {
                    if (((!IFRMContext.Current.Salon.BookOnWidget && (localPath != "/offline.aspx")) && ((localPath != "/expired.aspx") && (localPath != "/login.aspx"))) && (((localPath != "/logout.aspx") && !localPath.StartsWith("/securearea")) && !localPath.StartsWith("/admin")))
                    {
                        application.Response.Redirect(IFRMHelper.GetURL("~/offline.aspx", new string[0]), true);
                    }
                    WSPDB wSPCurrent = IoC.Resolve<IBillingManager>().GetWSPCurrent(IFRMContext.Current.Salon.SalonId);
                    if ((((wSPCurrent == null) || !wSPCurrent.Active) || (DateTime.Today >= IFRMHelper.FromUrlFriendlyDate(wSPCurrent.PlanEndDate).AddDays(7.0))) && ((((localPath != "/offline.aspx") && (localPath != "/expired.aspx")) && ((localPath != "/login.aspx") && (localPath != "/logout.aspx"))) && (!localPath.StartsWith("/securearea") && !localPath.StartsWith("/admin"))))
                    {
                        application.Response.Redirect(IFRMHelper.GetURL("~/expired.aspx", new string[0]), true);
                    }
                }
                if (((HttpContext.Current != null) && (HttpContext.Current.User != null)) && (HttpContext.Current.User.Identity != null))
                {
                    Action action = delegate {
                        if (HttpContext.Current.Session != null)
                        {
                            HttpContext.Current.Session.Abandon();
                        }
                        FormsAuthentication.SignOut();
                        if (HttpContext.Current != null)
                        {
                            string url = IFRMHelper.GetURL(FormsAuthentication.LoginUrl, new string[0]);
                            HttpContext.Current.Response.Redirect(url, true);
                        }
                    };
                    string name = HttpContext.Current.User.Identity.Name;
                    if (IFRMContext.Current.Salon == null)
                    {
                        action();
                    }
                    else if (string.IsNullOrEmpty(name))
                    {
                        action();
                    }
                    else
                    {
                        SalonUserDB salonUserByUsername = IoC.Resolve<IUserManager>().GetSalonUserByUsername(name);
                        if (((salonUserByUsername == null) || !salonUserByUsername.Active) || salonUserByUsername.Deleted)
                        {
                            action();
                        }
                        else if (!salonUserByUsername.IsAdmin && (salonUserByUsername.SalonId != IFRMContext.Current.Salon.SalonId))
                        {
                            action();
                        }
                        else
                        {
                            IFRMContext.Current.WorkingUser = salonUserByUsername;
                        }
                    }
                }
            }
        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        private void Application_EndRequest(object sender, EventArgs e)
        {
        }

        private void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
        }

        private void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
        }

        private void Application_PreSendRequestContent(object sender, EventArgs e)
        {
        }

        private void Application_ReleaseRequestState(object sender, EventArgs e)
        {
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication application)
        {
            application.BeginRequest += new EventHandler(this.Application_BeginRequest);
            application.EndRequest += new EventHandler(this.Application_EndRequest);
            application.PostAcquireRequestState += new EventHandler(this.Application_PostAcquireRequestState);
            application.ReleaseRequestState += new EventHandler(this.Application_ReleaseRequestState);
            application.AuthenticateRequest += new EventHandler(this.Application_AuthenticateRequest);
            application.PreSendRequestContent += new EventHandler(this.Application_PreSendRequestContent);
            application.PostRequestHandlerExecute += new EventHandler(this.Application_PostRequestHandlerExecute);
        }
    }
}

