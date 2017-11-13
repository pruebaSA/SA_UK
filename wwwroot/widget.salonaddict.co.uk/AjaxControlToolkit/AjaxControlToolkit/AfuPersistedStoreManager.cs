namespace AjaxControlToolkit
{
    using AjaxControlToolkit.Properties;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Web;
    using System.Web.SessionState;

    internal sealed class AfuPersistedStoreManager
    {
        private string extendedFileUploadGUID;
        private static readonly string IdSeperator = "~!~";
        private PersistedStoreTypeEnum persistedStorageType;

        private AfuPersistedStoreManager()
        {
        }

        public void AddFileToSession(string controlId, string filename, HttpPostedFile fileUpload)
        {
            if (fileUpload == null)
            {
                throw new ArgumentNullException("fileUpload");
            }
            if (controlId == string.Empty)
            {
                throw new ArgumentNullException("controlId");
            }
            HttpContext currentContext = null;
            currentContext = this.GetCurrentContext();
            if (currentContext != null)
            {
                if (currentContext.Session.Mode != SessionStateMode.InProc)
                {
                    throw new InvalidOperationException(Resources.SessionStateOutOfProcessNotSupported);
                }
                currentContext.Session.Add(this.GetFullID(controlId), fileUpload);
            }
        }

        public void ClearAllFilesFromSession(string controlId)
        {
            HttpContext currentContext = null;
            currentContext = this.GetCurrentContext();
            if (currentContext != null)
            {
                Collection<string> collection = new Collection<string>();
                foreach (string str in currentContext.Session.Keys)
                {
                    if (str.StartsWith(this.extendedFileUploadGUID))
                    {
                        collection.Add(str);
                    }
                }
                foreach (string str2 in collection)
                {
                    currentContext.Session.Remove(str2);
                }
            }
        }

        public bool FileExists(string controlId)
        {
            if (controlId == null)
            {
                throw new ArgumentNullException("controlId");
            }
            HttpContext context = null;
            return ((((context = this.GetCurrentContext()) != null) && (context.Session[this.GetFullID(controlId)] != null)) && (context.Session[this.GetFullID(controlId)] is HttpPostedFile));
        }

        public List<HttpPostedFile> GetAllFilesFromSession(string controlId)
        {
            List<HttpPostedFile> list = new List<HttpPostedFile>();
            HttpContext currentContext = null;
            currentContext = this.GetCurrentContext();
            if (currentContext != null)
            {
                foreach (string str in currentContext.Session.Keys)
                {
                    if (str.StartsWith(this.extendedFileUploadGUID) && (HttpContext.Current.Session[str] != null))
                    {
                        HttpPostedFile item = HttpContext.Current.Session[str] as HttpPostedFile;
                        if (item != null)
                        {
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        public string GetContentType(string controlId)
        {
            if (controlId == null)
            {
                throw new ArgumentNullException("controlId");
            }
            HttpContext currentContext = null;
            currentContext = this.GetCurrentContext();
            if (currentContext != null)
            {
                HttpPostedFile file = currentContext.Session[this.GetFullID(controlId)] as HttpPostedFile;
                if (file != null)
                {
                    return file.ContentType;
                }
            }
            return string.Empty;
        }

        private HttpContext GetCurrentContext()
        {
            if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
            {
                return HttpContext.Current;
            }
            return null;
        }

        public HttpPostedFile GetFileFromSession(string controlId)
        {
            if (controlId == null)
            {
                throw new ArgumentNullException("controlId");
            }
            HttpContext currentContext = null;
            currentContext = this.GetCurrentContext();
            if (currentContext == null)
            {
                return null;
            }
            if (currentContext.Session[this.GetFullID(controlId)] == null)
            {
                return null;
            }
            HttpPostedFile file = currentContext.Session[this.GetFullID(controlId)] as HttpPostedFile;
            if (file == null)
            {
                throw new InvalidCastException("postedFile");
            }
            return file;
        }

        public string GetFileName(string controlId)
        {
            if (controlId == null)
            {
                throw new ArgumentNullException("controlId");
            }
            HttpContext currentContext = null;
            currentContext = this.GetCurrentContext();
            if (currentContext != null)
            {
                HttpPostedFile file = currentContext.Session[this.GetFullID(controlId)] as HttpPostedFile;
                if (file != null)
                {
                    return file.FileName;
                }
            }
            return string.Empty;
        }

        public string GetFullID(string controlId) => 
            (this.extendedFileUploadGUID + IdSeperator + controlId);

        public void RemoveFileFromSession(string controlId)
        {
            HttpContext currentContext = null;
            currentContext = this.GetCurrentContext();
            if (currentContext != null)
            {
                Collection<string> collection = new Collection<string>();
                foreach (string str in currentContext.Session.Keys)
                {
                    if (str.StartsWith(this.GetFullID(controlId)))
                    {
                        collection.Add(str);
                    }
                }
                foreach (string str2 in collection)
                {
                    currentContext.Session.Remove(str2);
                }
            }
        }

        public string ExtendedFileUploadGUID
        {
            get => 
                this.extendedFileUploadGUID;
            set
            {
                this.extendedFileUploadGUID = value;
            }
        }

        public static AfuPersistedStoreManager Instance =>
            InstanceInitializer.instance;

        public PersistedStoreTypeEnum PersistedStorageType
        {
            get => 
                this.persistedStorageType;
            set
            {
                this.persistedStorageType = value;
            }
        }

        private class InstanceInitializer
        {
            internal static readonly AfuPersistedStoreManager instance = new AfuPersistedStoreManager();
        }

        public enum PersistedStoreTypeEnum
        {
            Session
        }
    }
}

