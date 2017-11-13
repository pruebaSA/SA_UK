namespace System.Web
{
    using System;
    using System.IO;
    using System.Security.Permissions;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class HttpPostedFileBase
    {
        protected HttpPostedFileBase()
        {
        }

        public virtual void SaveAs(string filename)
        {
            throw new NotImplementedException();
        }

        public virtual int ContentLength
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string ContentType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string FileName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual Stream InputStream
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}

