namespace System.Web.Mail
{
    using System;
    using System.Collections;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;

    [Obsolete("The recommended alternative is System.Net.Mail.MailMessage. http://go.microsoft.com/fwlink/?linkid=14202"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class MailMessage
    {
        private ArrayList _attachments = new ArrayList();
        private Hashtable _fields = new Hashtable();
        private Hashtable _headers = new Hashtable();
        private string bcc;
        private string body;
        private Encoding bodyEncoding = Encoding.Default;
        private MailFormat bodyFormat;
        private string cc;
        private string from;
        private MailPriority priority;
        private string subject;
        private string to;
        private string urlContentBase;
        private string urlContentLocation;

        public IList Attachments =>
            this._attachments;

        public string Bcc
        {
            get => 
                this.bcc;
            set
            {
                this.bcc = value;
            }
        }

        public string Body
        {
            get => 
                this.body;
            set
            {
                this.body = value;
            }
        }

        public Encoding BodyEncoding
        {
            get => 
                this.bodyEncoding;
            set
            {
                this.bodyEncoding = value;
            }
        }

        public MailFormat BodyFormat
        {
            get => 
                this.bodyFormat;
            set
            {
                this.bodyFormat = value;
            }
        }

        public string Cc
        {
            get => 
                this.cc;
            set
            {
                this.cc = value;
            }
        }

        public IDictionary Fields =>
            this._fields;

        public string From
        {
            get => 
                this.from;
            set
            {
                this.from = value;
            }
        }

        public IDictionary Headers =>
            this._headers;

        public MailPriority Priority
        {
            get => 
                this.priority;
            set
            {
                this.priority = value;
            }
        }

        public string Subject
        {
            get => 
                this.subject;
            set
            {
                this.subject = value;
            }
        }

        public string To
        {
            get => 
                this.to;
            set
            {
                this.to = value;
            }
        }

        public string UrlContentBase
        {
            get => 
                this.urlContentBase;
            set
            {
                this.urlContentBase = value;
            }
        }

        public string UrlContentLocation
        {
            get => 
                this.urlContentLocation;
            set
            {
                this.urlContentLocation = value;
            }
        }
    }
}

