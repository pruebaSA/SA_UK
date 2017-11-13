namespace System.Web
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Text;

    internal class ConfigErrorFormatter : FormatterWithFileInfo
    {
        private StringCollection _adaptiveMiscContent;
        private bool _allowSourceCode;
        private System.Exception _e;
        protected string _message;

        internal ConfigErrorFormatter(ConfigurationException e) : base(null, e.Filename, null, e.Line)
        {
            this._adaptiveMiscContent = new StringCollection();
            this._e = e;
            PerfCounters.IncrementCounter(AppPerfCounter.ERRORS_PRE_PROCESSING);
            PerfCounters.IncrementCounter(AppPerfCounter.ERRORS_TOTAL);
            this._message = HttpUtility.FormatPlainTextAsHtml(e.BareMessage);
            this._adaptiveMiscContent.Add(this._message);
        }

        protected override StringCollection AdaptiveMiscContent =>
            this._adaptiveMiscContent;

        public bool AllowSourceCode
        {
            get => 
                this._allowSourceCode;
            set
            {
                this._allowSourceCode = value;
            }
        }

        protected override string ColoredSquareContent
        {
            get
            {
                if (!this.AllowSourceCode)
                {
                    return System.Web.SR.GetString("Generic_Err_Remote_Desc");
                }
                return base.ColoredSquareContent;
            }
        }

        protected override string ColoredSquareTitle =>
            System.Web.SR.GetString("Parser_Source_Error");

        protected override string Description =>
            System.Web.SR.GetString("Config_Desc");

        protected override string ErrorTitle =>
            System.Web.SR.GetString("Config_Error");

        protected override System.Exception Exception =>
            this._e;

        protected override string MiscSectionContent =>
            this._message;

        protected override string MiscSectionTitle =>
            System.Web.SR.GetString("Parser_Error_Message");

        protected override Encoding SourceFileEncoding =>
            Encoding.UTF8;
    }
}

