namespace System.Web.Services
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Services.Protocols;
    using System.Web.SessionState;

    public class WebService : MarshalByValueComponent
    {
        private HttpContext context;
        internal static readonly string SoapVersionContextSlot = "WebServiceSoapVersion";

        internal void SetContext(HttpContext context)
        {
            this.context = context;
        }

        [Description("The ASP.NET application object for the current request."), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HttpApplicationState Application =>
            this.Context.Application;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebServicesDescription("WebServiceContext"), Browsable(false)]
        public HttpContext Context
        {
            [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
            get
            {
                if (this.context == null)
                {
                    this.context = HttpContext.Current;
                }
                if (this.context == null)
                {
                    throw new InvalidOperationException(Res.GetString("WebMissingHelpContext"));
                }
                return this.context;
            }
        }

        [Browsable(false), WebServicesDescription("WebServiceServer"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HttpServerUtility Server =>
            this.Context.Server;

        [WebServicesDescription("WebServiceSession"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HttpSessionState Session =>
            this.Context.Session;

        [ComVisible(false), WebServicesDescription("WebServiceSoapVersion"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SoapProtocolVersion SoapVersion
        {
            [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
            get
            {
                object obj2 = this.Context.Items[SoapVersionContextSlot];
                if ((obj2 != null) && (obj2 is SoapProtocolVersion))
                {
                    return (SoapProtocolVersion) obj2;
                }
                return SoapProtocolVersion.Default;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebServicesDescription("WebServiceUser")]
        public IPrincipal User =>
            this.Context.User;
    }
}

