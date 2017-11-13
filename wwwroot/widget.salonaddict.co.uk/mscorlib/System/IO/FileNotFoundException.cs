﻿namespace System.IO
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;

    [Serializable, ComVisible(true)]
    public class FileNotFoundException : IOException
    {
        private string _fileName;
        private string _fusionLog;

        public FileNotFoundException() : base(Environment.GetResourceString("IO.FileNotFound"))
        {
            base.SetErrorCode(-2147024894);
        }

        public FileNotFoundException(string message) : base(message)
        {
            base.SetErrorCode(-2147024894);
        }

        protected FileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this._fileName = info.GetString("FileNotFound_FileName");
            try
            {
                this._fusionLog = info.GetString("FileNotFound_FusionLog");
            }
            catch
            {
                this._fusionLog = null;
            }
        }

        public FileNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
            base.SetErrorCode(-2147024894);
        }

        public FileNotFoundException(string message, string fileName) : base(message)
        {
            base.SetErrorCode(-2147024894);
            this._fileName = fileName;
        }

        public FileNotFoundException(string message, string fileName, Exception innerException) : base(message, innerException)
        {
            base.SetErrorCode(-2147024894);
            this._fileName = fileName;
        }

        private FileNotFoundException(string fileName, string fusionLog, int hResult) : base(null)
        {
            base.SetErrorCode(hResult);
            this._fileName = fileName;
            this._fusionLog = fusionLog;
            this.SetMessageField();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("FileNotFound_FileName", this._fileName, typeof(string));
            try
            {
                info.AddValue("FileNotFound_FusionLog", this.FusionLog, typeof(string));
            }
            catch (SecurityException)
            {
            }
        }

        private void SetMessageField()
        {
            if (base._message == null)
            {
                if ((this._fileName == null) && (base.HResult == -2146233088))
                {
                    base._message = Environment.GetResourceString("IO.FileNotFound");
                }
                else if (this._fileName != null)
                {
                    base._message = FileLoadException.FormatFileLoadExceptionMessage(this._fileName, base.HResult);
                }
            }
        }

        public override string ToString()
        {
            string str = base.GetType().FullName + ": " + this.Message;
            if ((this._fileName != null) && (this._fileName.Length != 0))
            {
                str = str + Environment.NewLine + string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("IO.FileName_Name"), new object[] { this._fileName });
            }
            if (base.InnerException != null)
            {
                str = str + " ---> " + base.InnerException.ToString();
            }
            if (this.StackTrace != null)
            {
                str = str + Environment.NewLine + this.StackTrace;
            }
            try
            {
                if (this.FusionLog == null)
                {
                    return str;
                }
                if (str == null)
                {
                    str = " ";
                }
                str = str + Environment.NewLine;
                str = str + Environment.NewLine;
                str = str + this.FusionLog;
            }
            catch (SecurityException)
            {
            }
            return str;
        }

        public string FileName =>
            this._fileName;

        public string FusionLog =>
            this._fusionLog;

        public override string Message
        {
            get
            {
                this.SetMessageField();
                return base._message;
            }
        }
    }
}
