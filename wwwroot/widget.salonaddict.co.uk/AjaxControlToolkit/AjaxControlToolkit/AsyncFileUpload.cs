namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    [Designer("AjaxControlToolkit.AsyncFileUploadDesigner, AjaxControlToolkit"), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.AsyncFileUpload", "AsyncFileUpload.AsyncFileUpload.js")]
    public class AsyncFileUpload : ScriptControlBase
    {
        private UploaderStyleEnum controlStyle;
        private bool failedValidation;
        private string hiddenFieldID;
        private string innerTBID;
        private HtmlInputFile inputFile;
        private string lastError;
        private bool persistFile;
        private PersistedStoreTypeEnum persistStorageType;
        private HttpPostedFile postedFile;

        [Category("Server Events"), Bindable(true)]
        public event EventHandler<AsyncFileUploadEventArgs> UploadedComplete;

        [Category("Server Events"), Bindable(true)]
        public event EventHandler<AsyncFileUploadEventArgs> UploadedFileError;

        public AsyncFileUpload() : base(true, HtmlTextWriterTag.Div)
        {
            this.lastError = string.Empty;
            this.hiddenFieldID = string.Empty;
            this.innerTBID = string.Empty;
        }

        public void ClearAllFilesFromPersistedStore()
        {
            AfuPersistedStoreManager.Instance.ClearAllFilesFromSession(this.ClientID);
        }

        public void ClearFileFromPersistedStore()
        {
            AfuPersistedStoreManager.Instance.RemoveFileFromSession(this.ClientID);
        }

        protected override void CreateChildControls()
        {
            AfuPersistedStoreManager.Instance.ExtendedFileUploadGUID = Constants.fileUploadGUID;
            string str = null;
            if (!this.IsDesignMode)
            {
                str = this.Page.Request.QueryString[Constants.FileUploadIDKey];
            }
            if ((this.IsDesignMode || (str == null)) || (str == string.Empty))
            {
                this.hiddenFieldID = this.GenerateHtmlInputHiddenControl();
                string lastFileName = string.Empty;
                if (this.persistFile)
                {
                    if (AfuPersistedStoreManager.Instance.FileExists(this.ClientID))
                    {
                        lastFileName = AfuPersistedStoreManager.Instance.GetFileName(this.ClientID);
                    }
                }
                else if (this.postedFile != null)
                {
                    lastFileName = this.postedFile.FileName;
                }
                this.GenerateHtmlInputFileControl(lastFileName);
            }
        }

        internal void CreateChilds()
        {
            this.Controls.Clear();
            this.CreateChildControls();
        }

        protected override Style CreateControlStyle() => 
            new AsyncFileUploadStyle(this.ViewState);

        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            if (!this.IsDesignMode)
            {
                if (this.hiddenFieldID != string.Empty)
                {
                    descriptor.AddElementProperty("hiddenField", this.hiddenFieldID);
                }
                if (this.innerTBID != string.Empty)
                {
                    descriptor.AddElementProperty("innerTB", this.innerTBID);
                }
                if (this.inputFile != null)
                {
                    descriptor.AddElementProperty("inputFile", this.inputFile.Name.Replace("$", "_"));
                }
                descriptor.AddProperty("postBackUrl", this.Page.Request.RawUrl);
                descriptor.AddProperty("formName", Path.GetFileName(this.Page.Form.Name));
                if (this.CompleteBackColor != Color.Empty)
                {
                    descriptor.AddProperty("completeBackColor", ColorTranslator.ToHtml(this.CompleteBackColor));
                }
                if (this.ErrorBackColor != Color.Empty)
                {
                    descriptor.AddProperty("errorBackColor", ColorTranslator.ToHtml(this.ErrorBackColor));
                }
                if (this.UploadingBackColor != Color.Empty)
                {
                    descriptor.AddProperty("uploadingBackColor", ColorTranslator.ToHtml(this.UploadingBackColor));
                }
                if (this.ThrobberID != string.Empty)
                {
                    Control control = this.FindControl(this.ThrobberID);
                    if (control != null)
                    {
                        descriptor.AddElementProperty("throbber", control.ClientID);
                    }
                }
            }
        }

        protected string GenerateHtmlInputFileControl(string lastFileName)
        {
            HtmlGenericControl child = new HtmlGenericControl("div");
            this.Controls.Add(child);
            if (this.UploaderStyle == UploaderStyleEnum.Modern)
            {
                string webResourceUrl = string.Empty;
                webResourceUrl = this.Page.ClientScript.GetWebResourceUrl(typeof(AsyncFileUpload), "AsyncFileUpload.images.fileupload.png");
                string str2 = "background:url(" + webResourceUrl + ") no-repeat 100% 1px; height:24px; margin:0px; text-align:right;";
                if (!this.Width.IsEmpty)
                {
                    string str4 = str2;
                    str2 = str4 + "min-width:" + this.Width.ToString() + ";width:" + this.Width.ToString() + " !important;";
                }
                else
                {
                    str2 = str2 + "width:355px;";
                }
                child.Attributes.Add("style", str2);
            }
            if ((this.UploaderStyle != UploaderStyleEnum.Modern) || !this.IsDesignMode)
            {
                this.inputFile = new HtmlInputFile();
                if (!this.Enabled)
                {
                    this.inputFile.Disabled = true;
                }
                child.Controls.Add(this.inputFile);
                this.inputFile.Attributes.Add("id", this.inputFile.Name.Replace("$", "_"));
                if (this.UploaderStyle != UploaderStyleEnum.Modern)
                {
                    if (this.BackColor != Color.Empty)
                    {
                        this.inputFile.Style[HtmlTextWriterStyle.BackgroundColor] = ColorTranslator.ToHtml(this.BackColor);
                    }
                    if (!this.Width.IsEmpty)
                    {
                        this.inputFile.Style[HtmlTextWriterStyle.Width] = this.Width.ToString();
                    }
                    else
                    {
                        this.inputFile.Style[HtmlTextWriterStyle.Width] = "355px";
                    }
                }
            }
            if (this.UploaderStyle == UploaderStyleEnum.Modern)
            {
                string str3 = "opacity:0.0; -moz-opacity: 0.0; filter: alpha(opacity=00); font-size:14px;";
                if (!this.Width.IsEmpty)
                {
                    str3 = str3 + "width:" + this.Width.ToString() + ";";
                }
                if (this.inputFile != null)
                {
                    this.inputFile.Attributes.Add("style", str3);
                }
                TextBox box = new TextBox();
                if (!this.IsDesignMode)
                {
                    HtmlGenericControl control2 = new HtmlGenericControl("div");
                    child.Controls.Add(control2);
                    str3 = "margin-top:-23px;text-align:left;";
                    control2.Attributes.Add("style", str3);
                    control2.Attributes.Add("type", "text");
                    control2.Controls.Add(box);
                    str3 = "height:17px; font-size:12px; font-family:Tahoma;";
                }
                else
                {
                    child.Controls.Add(box);
                    str3 = "height:23px; font-size:12px; font-family:Tahoma;";
                }
                if (!this.Width.IsEmpty && (this.Width.ToString().IndexOf("px") > 0))
                {
                    str3 = str3 + "width:" + ((int.Parse(this.Width.ToString().Substring(0, this.Width.ToString().IndexOf("px"))) - 0x6b)).ToString() + "px;";
                }
                else
                {
                    str3 = str3 + "width:248px;";
                }
                if ((lastFileName != string.Empty) || this.failedValidation)
                {
                    if (((this.FileBytes != null) && (this.FileBytes.Length > 0)) && !this.failedValidation)
                    {
                        str3 = str3 + "background-color:#00FF00;";
                    }
                    else
                    {
                        this.failedValidation = false;
                        str3 = str3 + "background-color:#FF0000;";
                    }
                    box.Text = lastFileName;
                }
                else if (this.BackColor != Color.Empty)
                {
                    str3 = str3 + "background-color:" + ColorTranslator.ToHtml(this.BackColor) + ";";
                }
                box.ReadOnly = true;
                box.Attributes.Add("style", str3);
                this.innerTBID = box.ClientID;
            }
            else if (this.IsDesignMode)
            {
                this.Controls.Clear();
                this.Controls.Add(this.inputFile);
            }
            return child.ClientID;
        }

        protected string GenerateHtmlInputHiddenControl()
        {
            HiddenField child = new HiddenField();
            this.Controls.Add(child);
            return child.ClientID;
        }

        public byte[] GetBytesFromStream(Stream stream)
        {
            byte[] buffer2;
            byte[] buffer = new byte[0x8000];
            using (MemoryStream stream2 = new MemoryStream())
            {
                int num;
                stream.Seek(0L, SeekOrigin.Begin);
            Label_001B:
                num = stream.Read(buffer, 0, buffer.Length);
                if (num <= 0)
                {
                    buffer2 = stream2.ToArray();
                }
                else
                {
                    stream2.Write(buffer, 0, num);
                    goto Label_001B;
                }
            }
            return buffer2;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            AfuPersistedStoreManager.Instance.PersistedStorageType = (AfuPersistedStoreManager.PersistedStoreTypeEnum) this.PersistedStoreType;
            string str = this.Page.Request.QueryString[Constants.FileUploadIDKey];
            if (((str != null) && (str == this.ClientID)) || (str == null))
            {
                this.ReceivedFile(this.ClientID);
                if ((str != null) && str.StartsWith(this.ClientID))
                {
                    string str2;
                    if (this.lastError == string.Empty)
                    {
                        byte[] fileBytes = this.FileBytes;
                        if (fileBytes != null)
                        {
                            str2 = fileBytes.Length.ToString() + "------" + this.ContentType;
                        }
                        else
                        {
                            str2 = "";
                        }
                    }
                    else
                    {
                        str2 = "error------" + this.lastError;
                    }
                    TextWriter output = this.Page.Response.Output;
                    output.Write("<div id='" + this.ClientID + "'>");
                    output.Write(str2);
                    output.Write("</div>");
                }
            }
        }

        protected virtual void OnUploadedComplete(AsyncFileUploadEventArgs e)
        {
            if (this.UploadedComplete != null)
            {
                this.UploadedComplete(this, e);
            }
        }

        protected virtual void OnUploadedFileError(AsyncFileUploadEventArgs e)
        {
            if (this.UploadedFileError != null)
            {
                this.UploadedFileError(this, e);
            }
        }

        private void PopulatObjectPriorToRender(string controlId)
        {
            bool flag;
            if (this.persistFile)
            {
                flag = AfuPersistedStoreManager.Instance.FileExists(controlId);
            }
            else
            {
                flag = this.postedFile != null;
            }
            if ((!flag && (this.Page != null)) && (this.Page.Request.Files.Count != 0))
            {
                this.ReceivedFile(controlId);
            }
        }

        private void ReceivedFile(string sendingControlID)
        {
            AsyncFileUploadEventArgs e = null;
            this.lastError = string.Empty;
            if (this.Page.Request.Files.Count > 0)
            {
                HttpPostedFile file = null;
                if ((sendingControlID == null) || (sendingControlID == string.Empty))
                {
                    file = this.Page.Request.Files[0];
                }
                else
                {
                    foreach (string str in this.Page.Request.Files)
                    {
                        if (str.Replace("$", "_").Replace("_ctl02", "").EndsWith(sendingControlID))
                        {
                            file = this.Page.Request.Files[str];
                            break;
                        }
                    }
                }
                if (file == null)
                {
                    this.lastError = Constants.Errors.FileNull;
                    e = new AsyncFileUploadEventArgs(AsyncFileUploadState.Failed, Constants.Errors.FileNull, string.Empty, string.Empty);
                    this.OnUploadedFileError(e);
                }
                else if (file.FileName == string.Empty)
                {
                    this.lastError = Constants.Errors.NoFileName;
                    e = new AsyncFileUploadEventArgs(AsyncFileUploadState.Unknown, Constants.Errors.NoFileName, file.FileName, file.ContentLength.ToString());
                    this.OnUploadedFileError(e);
                }
                else if (file.InputStream == null)
                {
                    this.lastError = Constants.Errors.NoFileName;
                    e = new AsyncFileUploadEventArgs(AsyncFileUploadState.Failed, Constants.Errors.NoFileName, file.FileName, file.ContentLength.ToString());
                    this.OnUploadedFileError(e);
                }
                else if (file.ContentLength < 1)
                {
                    this.lastError = Constants.Errors.EmptyContentLength;
                    e = new AsyncFileUploadEventArgs(AsyncFileUploadState.Unknown, Constants.Errors.EmptyContentLength, file.FileName, file.ContentLength.ToString());
                    this.OnUploadedFileError(e);
                }
                else
                {
                    e = new AsyncFileUploadEventArgs(AsyncFileUploadState.Success, string.Empty, file.FileName, file.ContentLength.ToString());
                    if (this.persistFile)
                    {
                        GC.SuppressFinalize(file);
                        AfuPersistedStoreManager.Instance.AddFileToSession(this.ClientID, file.FileName, file);
                    }
                    else
                    {
                        this.postedFile = file;
                    }
                    this.OnUploadedComplete(e);
                }
            }
        }

        public void SaveAs(string fileName)
        {
            this.PopulatObjectPriorToRender(this.ClientID);
            this.CurrentFile.SaveAs(fileName);
        }

        [DefaultValue(typeof(Color), "Lime"), TypeConverter(typeof(WebColorConverter)), Description("Control's background color on upload complete."), Category("Appearance")]
        public Color CompleteBackColor
        {
            get => 
                (this.ViewState["CompleteBackColor"] ?? Color.Lime);
            set
            {
                this.ViewState["CompleteBackColor"] = value;
            }
        }

        [Browsable(false)]
        public string ContentType
        {
            get
            {
                this.PopulatObjectPriorToRender(this.ClientID);
                if (this.persistFile)
                {
                    return AfuPersistedStoreManager.Instance.GetContentType(this.ClientID);
                }
                if (this.postedFile != null)
                {
                    return this.postedFile.ContentType;
                }
                return string.Empty;
            }
        }

        private HttpPostedFile CurrentFile
        {
            get
            {
                if (!this.persistFile)
                {
                    return this.postedFile;
                }
                return AfuPersistedStoreManager.Instance.GetFileFromSession(this.ClientID);
            }
        }

        [Category("Appearance"), DefaultValue(typeof(Color), "Red"), TypeConverter(typeof(WebColorConverter)), Description("Control's background color on upload error.")]
        public Color ErrorBackColor
        {
            get => 
                (this.ViewState["ErrorBackColor"] ?? Color.Red);
            set
            {
                this.ViewState["ErrorBackColor"] = value;
            }
        }

        [Browsable(false)]
        public bool FailedValidation
        {
            get => 
                this.failedValidation;
            set
            {
                this.failedValidation = value;
            }
        }

        [Browsable(false)]
        public byte[] FileBytes
        {
            get
            {
                this.PopulatObjectPriorToRender(this.ClientID);
                HttpPostedFile currentFile = this.CurrentFile;
                if (currentFile != null)
                {
                    try
                    {
                        return this.GetBytesFromStream(currentFile.InputStream);
                    }
                    catch
                    {
                    }
                }
                return null;
            }
        }

        [Browsable(false)]
        public Stream FileContent
        {
            get
            {
                this.PopulatObjectPriorToRender(this.ClientID);
                HttpPostedFile currentFile = this.CurrentFile;
                if ((currentFile != null) && (currentFile.InputStream != null))
                {
                    return currentFile.InputStream;
                }
                return null;
            }
        }

        [Browsable(false)]
        public string FileName
        {
            get
            {
                this.PopulatObjectPriorToRender(this.ClientID);
                if (this.persistFile)
                {
                    return Path.GetFileName(AfuPersistedStoreManager.Instance.GetFileName(this.ClientID));
                }
                if (this.postedFile != null)
                {
                    return Path.GetFileName(this.postedFile.FileName);
                }
                return string.Empty;
            }
        }

        [Browsable(false)]
        public bool HasFile
        {
            get
            {
                this.PopulatObjectPriorToRender(this.ClientID);
                if (this.persistFile)
                {
                    return AfuPersistedStoreManager.Instance.FileExists(this.ClientID);
                }
                return (this.postedFile != null);
            }
        }

        private bool IsDesignMode =>
            (HttpContext.Current == null);

        [Browsable(false)]
        public bool IsUploading =>
            (this.Page.Request.QueryString[Constants.FileUploadIDKey] != null);

        [DefaultValue(""), ClientPropertyName("uploadComplete"), Category("Behavior"), ExtenderControlEvent]
        public string OnClientUploadComplete
        {
            get => 
                (this.ViewState["OnClientUploadComplete"] ?? string.Empty);
            set
            {
                this.ViewState["OnClientUploadComplete"] = value;
            }
        }

        [ClientPropertyName("uploadError"), DefaultValue(""), Category("Behavior"), ExtenderControlEvent]
        public string OnClientUploadError
        {
            get => 
                (this.ViewState["OnClientUploadError"] ?? string.Empty);
            set
            {
                this.ViewState["OnClientUploadError"] = value;
            }
        }

        [DefaultValue(""), ExtenderControlEvent, ClientPropertyName("uploadStarted"), Category("Behavior")]
        public string OnClientUploadStarted
        {
            get => 
                (this.ViewState["OnClientUploadStarted"] ?? string.Empty);
            set
            {
                this.ViewState["OnClientUploadStarted"] = value;
            }
        }

        [Bindable(true), Category("Behavior"), DefaultValue(0)]
        public PersistedStoreTypeEnum PersistedStoreType
        {
            get => 
                this.persistStorageType;
            set
            {
                this.persistStorageType = value;
            }
        }

        [DefaultValue(false), Browsable(true), Bindable(true)]
        public bool PersistFile
        {
            get => 
                this.persistFile;
            set
            {
                this.persistFile = value;
            }
        }

        [Browsable(false)]
        public HttpPostedFile PostedFile
        {
            get
            {
                this.PopulatObjectPriorToRender(this.ClientID);
                return this.CurrentFile;
            }
        }

        [DefaultValue(""), Category("Behavior"), Description("ID of Throbber")]
        public string ThrobberID
        {
            get => 
                (this.ViewState["ThrobberID"] ?? string.Empty);
            set
            {
                this.ViewState["ThrobberID"] = value;
            }
        }

        [DefaultValue(0), Bindable(true), Category("Appearance"), Browsable(true)]
        public UploaderStyleEnum UploaderStyle
        {
            get => 
                this.controlStyle;
            set
            {
                this.controlStyle = value;
            }
        }

        [TypeConverter(typeof(WebColorConverter)), Category("Appearance"), Description("Control's background color when uploading is in progress."), DefaultValue(typeof(Color), "White")]
        public Color UploadingBackColor
        {
            get => 
                (this.ViewState["UploadingBackColor"] ?? Color.White);
            set
            {
                this.ViewState["UploadingBackColor"] = value;
            }
        }

        [DefaultValue(typeof(Unit), ""), Category("Layout")]
        public override Unit Width
        {
            get => 
                base.Width;
            set
            {
                base.Width = value;
            }
        }

        private sealed class AsyncFileUploadStyle : Style
        {
            public AsyncFileUploadStyle(StateBag state) : base(state)
            {
            }

            protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
            {
                base.FillStyleAttributes(attributes, urlResolver);
                attributes.Remove(HtmlTextWriterStyle.BackgroundColor);
                attributes.Remove(HtmlTextWriterStyle.Width);
            }
        }

        public static class Constants
        {
            public static readonly string fileUploadGUID = "b3b89160-3224-476e-9076-70b500c816cf";
            public static readonly string FileUploadIDKey = "AsyncFileUploadID";
            public static readonly string InternalErrorInvalidIFrame = "The ExtendedFileUpload control has encountered an error with the uploader in this page. Please refresh the page and try again.";

            public static class Errors
            {
                public static readonly string EmptyContentLength = "The file attached is empty.";
                public static readonly string FileNull = "The file attached is invalid.";
                public static readonly string InputStreamNull = "The file attached could not be read.";
                public static readonly string NoFileName = "The file attached has an invalid filename.";
                public static readonly string NoFiles = "No files are attached to the upload.";
            }

            public static class StatusMessages
            {
                public static readonly string UploadSuccessful = "The file uploaded successfully.";
            }
        }

        public enum PersistedStoreTypeEnum
        {
            Session
        }

        public enum UploaderStyleEnum
        {
            Traditional,
            Modern
        }
    }
}

