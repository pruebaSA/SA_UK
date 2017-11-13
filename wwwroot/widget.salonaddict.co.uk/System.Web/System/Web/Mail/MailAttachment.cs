namespace System.Web.Mail
{
    using System;
    using System.IO;
    using System.Security.Permissions;
    using System.Web;

    [Obsolete("The recommended alternative is System.Net.Mail.Attachment. http://go.microsoft.com/fwlink/?linkid=14202"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class MailAttachment
    {
        private MailEncoding _encoding;
        private string _filename;

        public MailAttachment(string filename)
        {
            this._filename = filename;
            this._encoding = MailEncoding.Base64;
            this.VerifyFile();
        }

        public MailAttachment(string filename, MailEncoding encoding)
        {
            this._filename = filename;
            this._encoding = encoding;
            this.VerifyFile();
        }

        private void VerifyFile()
        {
            try
            {
                File.Open(this._filename, FileMode.Open, FileAccess.Read, FileShare.Read).Close();
            }
            catch
            {
                throw new HttpException(System.Web.SR.GetString("Bad_attachment", new object[] { this._filename }));
            }
        }

        public MailEncoding Encoding =>
            this._encoding;

        public string Filename =>
            this._filename;
    }
}

