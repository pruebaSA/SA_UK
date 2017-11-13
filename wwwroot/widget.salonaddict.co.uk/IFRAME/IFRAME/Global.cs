namespace IFRAME
{
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Text;
    using System.Web;

    public class Global : HttpApplication
    {
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                Exception lastError = base.Server.GetLastError();
                if (lastError == null)
                {
                    return;
                }
                if (lastError.GetType() == typeof(HttpException))
                {
                    HttpException exception2 = (HttpException) lastError;
                    if (exception2.GetHttpCode() == 0x194)
                    {
                        return;
                    }
                }
                if (lastError.InnerException != null)
                {
                    lastError = lastError.InnerException;
                }
                LogDB log = new LogDB {
                    CreatedOn = DateTime.Now,
                    Exception = lastError.StackTrace,
                    LogType = lastError.GetType().ToString(),
                    Message = lastError.Message,
                    PageURL = base.Request.Url.ToString()
                };
                if (base.Request.UrlReferrer != null)
                {
                    log.ReferrerURL = base.Request.UrlReferrer.ToString();
                }
                log.UserHostAddress = base.Request.UserHostAddress;
                SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
                if (workingUser != null)
                {
                    log.UserId = new Guid?(workingUser.UserId);
                }
                StringBuilder builder = new StringBuilder();
                builder.Append("<table class=\"details\" cellspacing=\"0\" cellpadding=\"0\" >");
                foreach (string str in base.Request.Params.Keys)
                {
                    builder.Append("<tr>");
                    builder.Append($"<td class="title">{str}:</td>");
                    builder.Append($"<td class="data-item">{base.Request.Params[str]}</td>");
                    builder.Append("</tr>");
                }
                builder.Append("</table>");
                log.Params = builder.ToString();
                log = IoC.Resolve<ILogManager>().InsertError(log);
            }
            catch
            {
            }
            try
            {
                if (!base.Request.Url.IsLoopback)
                {
                    base.Response.Clear();
                    base.Server.ClearError();
                    base.Response.Redirect(IFRMHelper.GetURL("~/error.aspx", new string[0]), true);
                }
            }
            catch
            {
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            IoC.InitializeWith(new DependencyResolverFactory(typeof(UnityDependencyResolver)));
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }
    }
}

