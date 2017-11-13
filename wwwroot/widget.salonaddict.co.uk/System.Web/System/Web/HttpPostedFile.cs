namespace System.Web
{
    using System;
    using System.IO;
    using System.Security.Permissions;
    using System.Web.Configuration;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class HttpPostedFile
    {
        private string _contentType;
        private string _filename;
        private HttpInputStream _stream;

        internal HttpPostedFile(string filename, string contentType, HttpInputStream stream)
        {
            this._filename = filename;
            this._contentType = contentType;
            this._stream = stream;
        }

        public void SaveAs(string filename)
        {
            if (!Path.IsPathRooted(filename) && RuntimeConfig.GetConfig().HttpRuntime.RequireRootedSaveAsPath)
            {
                throw new HttpException(System.Web.SR.GetString("SaveAs_requires_rooted_path", new object[] { filename }));
            }
            FileStream s = new FileStream(filename, FileMode.Create);
            try
            {
                this._stream.WriteTo(s);
                s.Flush();
            }
            finally
            {
                s.Close();
            }
        }

        public int ContentLength =>
            ((int) this._stream.Length);

        public string ContentType =>
            this._contentType;

        public string FileName =>
            this._filename;

        public Stream InputStream =>
            this._stream;
    }
}

