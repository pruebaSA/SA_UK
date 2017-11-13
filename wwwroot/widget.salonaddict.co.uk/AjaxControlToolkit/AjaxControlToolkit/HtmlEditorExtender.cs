namespace AjaxControlToolkit
{
    using AjaxControlToolkit.Sanitizer;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    [PersistChildren(false), ParseChildren(true), TargetControlType(typeof(TextBox)), ToolboxBitmap(typeof(HtmlEditorExtender), "HtmlEditorExtender.html_editor_extender.ico"), ClientCssResource("HtmlEditorExtender.HtmlEditorExtender_resource.css"), RequiredScript(typeof(ColorPickerExtender), 1), ClientScriptResource("Sys.Extended.UI.HtmlEditorExtenderBehavior", "HtmlEditorExtender.HtmlEditorExtenderBehavior.js"), RequiredScript(typeof(CommonToolkitScripts), 0)]
    public class HtmlEditorExtender : ExtenderControlBase
    {
        private AjaxControlToolkit.AjaxFileUpload ajaxFileUpload;
        internal const int ButtonHeightDef = 0x15;
        private HtmlEditorExtenderButtonCollection buttonList;
        internal const int ButtonWidthDef = 0x17;
        private bool enableSanitization = true;
        private AjaxControlToolkit.Sanitizer.SanitizerProvider sanitizerProvider;
        private bool tracked;

        public event EventHandler<AjaxFileUploadEventArgs> ImageUploadComplete;

        public HtmlEditorExtender()
        {
            base.EnableClientState = true;
            this.sanitizerProvider = AjaxControlToolkit.Sanitizer.Sanitizer.GetProvider();
        }

        protected virtual void CreateButtons()
        {
            this.buttonList = new HtmlEditorExtenderButtonCollection();
            if (!this.tracked)
            {
                this.tracked = true;
            }
            else
            {
                this.tracked = false;
                this.buttonList.Add(new Undo());
                this.buttonList.Add(new Redo());
                this.buttonList.Add(new Bold());
                this.buttonList.Add(new Italic());
                this.buttonList.Add(new Underline());
                this.buttonList.Add(new StrikeThrough());
                this.buttonList.Add(new Subscript());
                this.buttonList.Add(new Superscript());
                this.buttonList.Add(new JustifyLeft());
                this.buttonList.Add(new JustifyCenter());
                this.buttonList.Add(new JustifyRight());
                this.buttonList.Add(new JustifyFull());
                this.buttonList.Add(new InsertOrderedList());
                this.buttonList.Add(new InsertUnorderedList());
                this.buttonList.Add(new CreateLink());
                this.buttonList.Add(new UnLink());
                this.buttonList.Add(new RemoveFormat());
                this.buttonList.Add(new SelectAll());
                this.buttonList.Add(new UnSelect());
                this.buttonList.Add(new Delete());
                this.buttonList.Add(new Cut());
                this.buttonList.Add(new Copy());
                this.buttonList.Add(new Paste());
                this.buttonList.Add(new BackgroundColorSelector());
                this.buttonList.Add(new ForeColorSelector());
                this.buttonList.Add(new FontNameSelector());
                this.buttonList.Add(new FontSizeSelector());
                this.buttonList.Add(new Indent());
                this.buttonList.Add(new Outdent());
                this.buttonList.Add(new InsertHorizontalRule());
                this.buttonList.Add(new HorizontalSeparator());
            }
        }

        public string Decode(string value)
        {
            this.EnsureButtons();
            string str = "font|div|span|br|strong|em|strike|sub|sup|center|blockquote|hr|ol|ul|li|br|s|p|b|i|u|img";
            string str2 = "style|size|color|face|align|dir|src";
            string str3 = @"\'\,\w\-#\s\:\;\?\&\.\-\=";
            string input = Regex.Replace(Regex.Replace(Regex.Replace(value, @"\&quot\;", "\"", RegexOptions.IgnoreCase), "&apos;", "'", RegexOptions.IgnoreCase), @"(?:\&lt\;|\<)(\/?)((?:" + str + @")(?:\s(?:" + str2 + ")=\"[" + str3 + "]*\")*)(?:\\&gt\\;|\\>)", "<$1$2>", RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
            string str5 = "^\\\"\\>\\<\\\\";
            input = Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(input, "(?:\\&lt\\;|\\<)(\\/?)(a(?:(?:\\shref\\=\\\"[" + str5 + "]*\\\")|(?:\\sstyle\\=\\\"[" + str3 + "]*\\\"))*)(?:\\&gt\\;|\\>)", "<$1$2>", RegexOptions.ECMAScript | RegexOptions.IgnoreCase), "&amp;", "&", RegexOptions.IgnoreCase), "&nbsp;", "\x00a0", RegexOptions.IgnoreCase), "<[^>]*expression[^>]*>", "_", RegexOptions.ECMAScript | RegexOptions.IgnoreCase), @"<[^>]*data\:[^>]*>", "_", RegexOptions.ECMAScript | RegexOptions.IgnoreCase), "<[^>]*script[^>]*>", "_", RegexOptions.ECMAScript | RegexOptions.IgnoreCase), "<[^>]*filter[^>]*>", "_", RegexOptions.ECMAScript | RegexOptions.IgnoreCase), "<[^>]*behavior[^>]*>", "_", RegexOptions.ECMAScript | RegexOptions.IgnoreCase), "<[^>]*url[^>]*>", "_", RegexOptions.ECMAScript | RegexOptions.IgnoreCase), @"<[^>]*javascript\:[^>]*>", "_", RegexOptions.ECMAScript | RegexOptions.IgnoreCase), @"<[^>]*position\:[^>]*>", "_", RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
            if (this.EnableSanitization && (this.sanitizerProvider != null))
            {
                Dictionary<string, string[]> elementWhiteList = this.MakeCombinedElementList();
                Dictionary<string, string[]> attributeWhiteList = this.MakeCombinedAttributeList();
                input = this.sanitizerProvider.GetSafeHtmlFragment(input, elementWhiteList, attributeWhiteList);
            }
            return input;
        }

        private void EnsureButtons()
        {
            if ((this.buttonList == null) || (this.buttonList.Count == 0))
            {
                this.CreateButtons();
            }
        }

        private Dictionary<string, string[]> MakeCombinedAttributeList()
        {
            Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
            foreach (HtmlEditorExtenderButton button in this.ToolbarButtons)
            {
                if (button.AttributeWhiteList != null)
                {
                    foreach (KeyValuePair<string, string[]> pair in button.AttributeWhiteList)
                    {
                        if (dictionary.ContainsKey(pair.Key))
                        {
                            string[] strArray;
                            bool flag = false;
                            if (dictionary.TryGetValue(pair.Key, out strArray))
                            {
                                List<string> list = strArray.ToList<string>();
                                foreach (string str in pair.Value)
                                {
                                    if (!strArray.Contains<string>(str))
                                    {
                                        list.Add(str);
                                        flag = true;
                                    }
                                }
                                if (flag)
                                {
                                    dictionary[pair.Key] = list.ToArray();
                                }
                            }
                        }
                        else
                        {
                            dictionary.Add(pair.Key, pair.Value);
                        }
                    }
                }
            }
            return dictionary;
        }

        private Dictionary<string, string[]> MakeCombinedElementList()
        {
            Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
            foreach (HtmlEditorExtenderButton button in this.ToolbarButtons)
            {
                if (button.ElementWhiteList != null)
                {
                    foreach (KeyValuePair<string, string[]> pair in button.ElementWhiteList)
                    {
                        if (dictionary.ContainsKey(pair.Key))
                        {
                            string[] strArray;
                            bool flag = false;
                            if (dictionary.TryGetValue(pair.Key, out strArray))
                            {
                                List<string> list = strArray.ToList<string>();
                                foreach (string str in pair.Value)
                                {
                                    if (!strArray.Contains<string>(str))
                                    {
                                        list.Add(str);
                                        flag = true;
                                    }
                                }
                                if (flag)
                                {
                                    dictionary[pair.Key] = list.ToArray();
                                }
                            }
                        }
                        else
                        {
                            dictionary.Add(pair.Key, pair.Value);
                        }
                    }
                }
            }
            return dictionary;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!base.DesignMode)
            {
                if (this.EnableSanitization && (this.sanitizerProvider == null))
                {
                    throw new Exception("Sanitizer provider is not configured in the web.config file. If you are using the HtmlEditorExtender with a public website then please configure a Sanitizer provider. Otherwise, set the EnableSanitization property to false.");
                }
                HtmlGenericControl child = new HtmlGenericControl("div");
                child.Attributes.Add("Id", this.ClientID + "_popupDiv");
                child.Attributes.Add("style", "opacity: 0;");
                child.Attributes.Add("class", "popupDiv");
                this.ajaxFileUpload = new AjaxControlToolkit.AjaxFileUpload();
                this.ajaxFileUpload.ID = this.ID + "_ajaxFileUpload";
                this.ajaxFileUpload.MaximumNumberOfFiles = 10;
                this.ajaxFileUpload.AllowedFileTypes = "jpg,jpeg,gif,png";
                this.ajaxFileUpload.Enabled = true;
                this.ajaxFileUpload.OnClientUploadComplete = "ajaxClientUploadComplete";
                if (this.ImageUploadComplete != null)
                {
                    this.ajaxFileUpload.UploadComplete += this.ImageUploadComplete;
                }
                child.Controls.Add(this.ajaxFileUpload);
                HtmlGenericControl control2 = new HtmlGenericControl("div");
                control2.Attributes.Add("Id", this.ClientID + "_btnCancel");
                control2.Attributes.Add("style", "float: right; position:relative; padding-left: 20px; top:10px; width: 55px; border-color:black;border-style: solid; border-width: 1px;cursor:pointer;");
                control2.Attributes.Add("float", "right");
                control2.Attributes.Add("unselectable", "on");
                control2.InnerText = "Cancel";
                child.Controls.Add(control2);
                this.Controls.Add(child);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ScriptManager.RegisterOnSubmitStatement(this, typeof(HtmlEditorExtender), "HtmlEditorExtenderOnSubmit", "null;");
            base.ClientState = (string.Compare(this.Page.Form.DefaultFocus, base.TargetControlID, StringComparison.OrdinalIgnoreCase) == 0) ? "Focused" : null;
            TextBox targetControl = (TextBox) base.TargetControl;
            if (targetControl != null)
            {
                targetControl.Text = this.Decode(targetControl.Text);
            }
            bool flag = false;
            foreach (HtmlEditorExtenderButton button in this.buttonList)
            {
                if (button.CommandName == "InsertImage")
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                this.ajaxFileUpload.Visible = false;
            }
        }

        [Browsable(false)]
        public AjaxControlToolkit.AjaxFileUpload AjaxFileUpload =>
            this.ajaxFileUpload;

        [DefaultValue(false), ClientPropertyName("displaySourceTab"), ExtenderControlProperty]
        public bool DisplaySourceTab
        {
            get => 
                base.GetPropertyValue<bool>("DisplaySourceTab", false);
            set
            {
                base.SetPropertyValue<bool>("DisplaySourceTab", value);
            }
        }

        [DefaultValue(true), Browsable(true)]
        public bool EnableSanitization
        {
            get => 
                this.enableSanitization;
            set
            {
                this.enableSanitization = value;
            }
        }

        [ClientPropertyName("change"), ExtenderControlEvent, DefaultValue("")]
        public string OnClientChange
        {
            get => 
                base.GetPropertyValue<string>("OnClientChange", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientChange", value);
            }
        }

        public AjaxControlToolkit.Sanitizer.SanitizerProvider SanitizerProvider
        {
            get => 
                this.sanitizerProvider;
            set
            {
                this.sanitizerProvider = value;
            }
        }

        [Description("Costumize visible buttons, leave empty to show all buttons"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), PersistenceMode(PersistenceMode.InnerProperty), Editor(typeof(HtmlEditorExtenderButtonCollectionEditor), typeof(UITypeEditor)), NotifyParentProperty(true), DefaultValue((string) null)]
        public HtmlEditorExtenderButtonCollection Toolbar
        {
            get
            {
                if (this.buttonList == null)
                {
                    this.buttonList = new HtmlEditorExtenderButtonCollection();
                }
                return this.buttonList;
            }
        }

        [Browsable(false), ExtenderControlProperty(true, true), EditorBrowsable(EditorBrowsableState.Never), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HtmlEditorExtenderButtonCollection ToolbarButtons
        {
            get
            {
                this.EnsureButtons();
                return this.buttonList;
            }
        }
    }
}

