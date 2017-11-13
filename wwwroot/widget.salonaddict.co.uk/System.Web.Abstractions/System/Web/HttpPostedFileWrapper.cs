namespace System.Web
{
    using System;
    using System.IO;
    using System.Security.Permissions;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HttpPostedFileWrapper : HttpPostedFileBase
    {
        private HttpPostedFile _file;

        public HttpPostedFileWrapper(HttpPostedFile httpPostedFile)
        {
            if (httpPostedFile == null)
            {
                throw new ArgumentNullException("httpPostedFile");
            }
            this._file = httpPostedFile;
        }

        public override void SaveAs(string filename)
        {
            this._file.SaveAs(filename);
        }

        public override int ContentLength =>
            this._file.ContentLength;

        public override string ContentType =>
            this._file.ContentType;

        public override string FileName =>
            this._file.FileName;

        public override Stream InputStream =>
            this._file.InputStream;
    }
}

