namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    [ClientCssResource("AjaxFileUpload.AjaxFileUpload.css"), RequiredScript(typeof(CommonToolkitScripts)), Designer("AjaxControlToolkit.AjaxFileUploadDesigner, AjaxControlToolkit"), ClientScriptResource("Sys.Extended.UI.AjaxFileUpload", "AjaxFileUpload.AjaxFileUpload.js")]
    public class AjaxFileUpload : ScriptControlBase
    {
        private const string ContextKey = "{DA8BEDC8-B952-4d5d-8CC2-59FE922E2923}";
        private HttpPostedFile postedFile;

        public event EventHandler<AjaxFileUploadEventArgs> UploadComplete;

        public AjaxFileUpload() : base(true, HtmlTextWriterTag.Div)
        {
        }

        protected override void CreateChildControls()
        {
            this.GenerateHtmlInputControls();
        }

        internal void CreateChilds()
        {
            this.Controls.Clear();
            this.CreateChildControls();
        }

        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            if (!this.IsDesignMode)
            {
                descriptor.AddProperty("contextKey", "{DA8BEDC8-B952-4d5d-8CC2-59FE922E2923}");
                descriptor.AddProperty("postBackUrl", this.Page.Request.RawUrl);
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

        private HtmlGenericControl GenerateHtmlFooterContainer(Control progressBar)
        {
            HtmlGenericControl control = new HtmlGenericControl("div");
            control.Attributes.Add("class", "ajax__fileupload_footer");
            control.Attributes.Add("id", this.ClientID + "_Footer");
            control.Attributes["align"] = "right";
            HtmlGenericControl child = new HtmlGenericControl("div");
            child.Attributes.Add("id", this.ClientID + "_UploadOrCancelButton");
            child.Attributes.Add("class", "ajax__fileupload_uploadbutton");
            HtmlGenericControl control3 = new HtmlGenericControl("div");
            control3.Attributes.Add("id", this.ClientID + "_ProgressBarContainer");
            control3.Attributes["align"] = "left";
            control3.Style["float"] = "left";
            control3.Style["width"] = "100%";
            control3.Controls.Add(progressBar);
            HtmlGenericControl control4 = new HtmlGenericControl("div");
            control4.Attributes.Add("class", "ajax__fileupload_ProgressBarHolder");
            control4.Controls.Add(control3);
            control.Controls.Add(control4);
            control.Controls.Add(child);
            return control;
        }

        protected string GenerateHtmlInputControls()
        {
            HtmlGenericControl child = new HtmlGenericControl("div");
            child.Attributes.Add("class", "ajax__fileupload");
            this.Controls.Add(child);
            string str = "opacity:0; -moz-opacity: 0.0; filter: alpha(opacity=0);";
            HtmlInputFile inputFileElement = new HtmlInputFile();
            if (!this.Enabled)
            {
                inputFileElement.Disabled = true;
            }
            inputFileElement.Attributes.Add("id", this.ClientID + "_Html5InputFile");
            inputFileElement.Attributes.Add("multiple", "multiple");
            inputFileElement.Attributes.Add("style", str);
            HtmlInputFile file2 = new HtmlInputFile();
            if (!this.Enabled)
            {
                file2.Disabled = true;
            }
            file2.Attributes.Add("id", this.ClientID + "_InputFileElement");
            file2.Attributes.Add("style", str);
            HtmlGenericControl control2 = new HtmlGenericControl("div");
            control2.Attributes.Add("class", "ajax__fileupload_dropzone");
            control2.Attributes.Add("id", this.ClientID + "_Html5DropZone");
            child.Controls.Add(control2);
            HtmlGenericControl fileStatusContainer = new HtmlGenericControl("div");
            fileStatusContainer.Attributes.Add("id", this.ClientID + "_FileStatusContainer");
            HtmlGenericControl control4 = this.GenerateHtmlSelectFileContainer(file2, inputFileElement, fileStatusContainer);
            child.Controls.Add(control4);
            HtmlGenericControl control5 = new HtmlGenericControl("div");
            control5.Attributes.Add("id", this.ClientID + "_QueueContainer");
            control5.Attributes.Add("class", "ajax__fileupload_queueContainer");
            child.Controls.Add(control5);
            HtmlGenericControl progressBar = new HtmlGenericControl("div");
            progressBar.Attributes.Add("id", this.ClientID + "_ProgressBar");
            progressBar.Attributes.Add("class", "ajax__fileupload_progressBar");
            progressBar.Attributes.Add("style", "width: 100%; display: none; visibility: hidden; overflow:visible;white-space:nowrap;");
            HtmlGenericControl control7 = this.GenerateHtmlFooterContainer(progressBar);
            child.Controls.Add(control7);
            return child.ClientID;
        }

        private HtmlGenericControl GenerateHtmlSelectFileContainer(Control html5InputFileElement, Control inputFileElement, Control fileStatusContainer)
        {
            HtmlGenericControl control = new HtmlGenericControl("span");
            control.Attributes.Add("id", this.ClientID + "_SelectFileContainer");
            control.Attributes.Add("class", "ajax__fileupload_selectFileContainer");
            HtmlGenericControl child = new HtmlGenericControl("span");
            child.Attributes.Add("id", this.ClientID + "_SelectFileButton");
            child.Attributes.Add("class", "ajax__fileupload_selectFileButton");
            HtmlGenericControl control3 = new HtmlGenericControl("div");
            control3.Attributes.Add("class", "ajax__fileupload_topFileStatus");
            control3.Style[HtmlTextWriterStyle.Overflow] = "hidden";
            control3.Controls.Add(fileStatusContainer);
            control.Controls.Add(child);
            control.Controls.Add(inputFileElement);
            control.Controls.Add(html5InputFileElement);
            control.Controls.Add(control3);
            return control;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if ((!this.IsDesignMode && !string.IsNullOrEmpty(this.Page.Request.QueryString["contextkey"])) && (this.Page.Request.QueryString["contextkey"] == "{DA8BEDC8-B952-4d5d-8CC2-59FE922E2923}"))
            {
                this.IsInFileUploadPostBack = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ScriptManager.RegisterOnSubmitStatement(this, typeof(AjaxFileUpload), "AjaxFileUploadOnSubmit", "null;");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if ((this.Page.Request.QueryString["contextkey"] == "{DA8BEDC8-B952-4d5d-8CC2-59FE922E2923}") && (this.Page.Request.Files.Count > 0))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string str = "";
                string fileId = this.Page.Request.QueryString["guid"];
                HttpPostedFile file = this.Page.Request.Files[0];
                this.postedFile = file;
                try
                {
                    int contentLength = file.ContentLength;
                    byte[] buffer = new byte[contentLength];
                    MemoryStream stream = new MemoryStream(contentLength);
                    file.InputStream.Read(buffer, 0, contentLength);
                    stream.Write(buffer, 0, contentLength);
                    AjaxFileUploadEventArgs args = new AjaxFileUploadEventArgs(fileId, AjaxFileUploadState.Success, "Success", file.FileName, file.ContentLength, file.ContentType, stream.ToArray());
                    if (this.UploadComplete != null)
                    {
                        this.UploadComplete(this, args);
                    }
                    str = serializer.Serialize(args);
                }
                catch (Exception)
                {
                    AjaxFileUploadEventArgs args2 = new AjaxFileUploadEventArgs(fileId, AjaxFileUploadState.Failed, "Failed", file.FileName, file.ContentLength, file.ContentType, null);
                    str = serializer.Serialize(args2);
                }
                this.Page.Response.ClearContent();
                this.Page.Response.Write("<html><body>" + str + "</body></html>");
                this.Page.Response.End();
            }
        }

        public void SaveAs(string fileName)
        {
            this.postedFile.SaveAs(fileName);
        }

        [ClientPropertyName("allowedFileTypes"), DefaultValue(""), ExtenderControlProperty]
        public string AllowedFileTypes { get; set; }

        [ClientPropertyName("contextKeys"), ExtenderControlProperty, DefaultValue((string) null)]
        public string ContextKeys { get; set; }

        private bool IsDesignMode =>
            (HttpContext.Current == null);

        [DefaultValue(false), Browsable(false)]
        public bool IsInFileUploadPostBack { get; set; }

        [DefaultValue(10), ClientPropertyName("maximumNumberOfFiles"), ExtenderControlProperty]
        public int MaximumNumberOfFiles { get; set; }

        [ExtenderControlEvent, ClientPropertyName("uploadComplete"), DefaultValue(""), Category("Behavior")]
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

        [Description("ID of Throbber"), DefaultValue(""), Category("Behavior")]
        public string ThrobberID
        {
            get => 
                (this.ViewState["ThrobberID"] ?? string.Empty);
            set
            {
                this.ViewState["ThrobberID"] = value;
            }
        }
    }
}

