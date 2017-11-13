namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Globalization;
    using System.IO;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Caching;
    using System.Web.UI;
    using System.Web.Util;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    [ControlBuilder(typeof(XmlBuilder)), Designer("System.Web.UI.Design.WebControls.XmlDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultProperty("DocumentSource"), PersistChildren(false, true), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class Xml : Control
    {
        private XslCompiledTransform _compiledTransform;
        private XmlDocument _document;
        private string _documentContent;
        private string _documentSource;
        private static XslTransform _identityTransform;
        private XslTransform _transform;
        private XsltArgumentList _transformArgumentList;
        private string _transformSource;
        private XPathDocument _xpathDocument;
        private System.Xml.XPath.XPathNavigator _xpathNavigator;
        private const string identityXslStr = "<xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'><xsl:template match=\"/\"> <xsl:copy-of select=\".\"/> </xsl:template> </xsl:stylesheet>";

        static Xml()
        {
            XmlTextReader stylesheet = new XmlTextReader(new StringReader("<xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'><xsl:template match=\"/\"> <xsl:copy-of select=\".\"/> </xsl:template> </xsl:stylesheet>"));
            _identityTransform = new XslTransform();
            _identityTransform.Load(stylesheet, null, null);
        }

        protected override void AddParsedSubObject(object obj)
        {
            if (!(obj is LiteralControl))
            {
                throw new HttpException(System.Web.SR.GetString("Cannot_Have_Children_Of_Type", new object[] { "Xml", obj.GetType().Name.ToString(CultureInfo.InvariantCulture) }));
            }
            string text = ((LiteralControl) obj).Text;
            int startIndex = Util.FirstNonWhiteSpaceIndex(text);
            this.DocumentContent = text.Substring(startIndex);
            if (base.DesignMode)
            {
                this.ViewState["OriginalContent"] = text;
            }
        }

        protected override ControlCollection CreateControlCollection() => 
            new EmptyControlCollection(this);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Control FindControl(string id) => 
            base.FindControl(id);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Focus()
        {
            throw new NotSupportedException(System.Web.SR.GetString("NoFocusSupport", new object[] { base.GetType().Name }));
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        protected override IDictionary GetDesignModeState() => 
            new HybridDictionary { ["OriginalContent"] = this.ViewState["OriginalContent"] };

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool HasControls() => 
            base.HasControls();

        private void LoadTransformFromSource()
        {
            if ((this._transform == null) && (!string.IsNullOrEmpty(this._transformSource) && (this._transformSource.Trim().Length != 0)))
            {
                VirtualPath path;
                string str;
                base.ResolvePhysicalOrVirtualPath(this._transformSource, out path, out str);
                CacheInternal cacheInternal = HttpRuntime.CacheInternal;
                string key = "p" + ((str != null) ? str : path.VirtualPathString);
                object obj2 = cacheInternal.Get(key);
                if (obj2 == null)
                {
                    CacheDependency dependency;
                    using (Stream stream = base.OpenFileAndGetDependency(path, str, out dependency))
                    {
                        if (str == null)
                        {
                            str = path.MapPath();
                        }
                        XmlTextReader stylesheet = new XmlTextReader(str, stream);
                        if (AppSettings.RestrictXmlControls)
                        {
                            stylesheet.ProhibitDtd = true;
                            this._compiledTransform = new XslCompiledTransform();
                            this._compiledTransform.Load(stylesheet, null, null);
                        }
                        else
                        {
                            this._transform = new XslTransform();
                            this._transform.Load(stylesheet);
                        }
                    }
                    if (dependency == null)
                    {
                        return;
                    }
                    using (dependency)
                    {
                        cacheInternal.UtcInsert(key, AppSettings.RestrictXmlControls ? ((object) this._compiledTransform) : ((object) this._transform), dependency);
                        return;
                    }
                }
                if (AppSettings.RestrictXmlControls)
                {
                    this._compiledTransform = (XslCompiledTransform) obj2;
                }
                else
                {
                    this._transform = (XslTransform) obj2;
                }
            }
        }

        private void LoadXmlDocument()
        {
            if (!string.IsNullOrEmpty(this._documentContent))
            {
                this._document = new XmlDocument();
                this._document.LoadXml(this._documentContent);
            }
            else if (!string.IsNullOrEmpty(this._documentSource))
            {
                string physicalPath = base.MapPathSecure(this._documentSource);
                CacheInternal cacheInternal = HttpRuntime.CacheInternal;
                string key = "q" + physicalPath;
                this._document = (XmlDocument) cacheInternal.Get(key);
                if (this._document == null)
                {
                    CacheDependency dependency;
                    using (Stream stream = base.OpenFileAndGetDependency(null, physicalPath, out dependency))
                    {
                        XmlTextReader reader;
                        if (AppSettings.RestrictXmlControls)
                        {
                            reader = new NoEntitiesXmlReader(physicalPath, stream);
                        }
                        else
                        {
                            reader = new XmlTextReader(physicalPath, stream);
                        }
                        this._document = new XmlDocument();
                        this._document.Load(reader);
                        cacheInternal.UtcInsert(key, this._document, dependency);
                    }
                }
                lock (this._document)
                {
                    this._document = (XmlDocument) this._document.CloneNode(true);
                }
            }
        }

        private void LoadXPathDocument()
        {
            if (!string.IsNullOrEmpty(this._documentContent))
            {
                StringReader textReader = new StringReader(this._documentContent);
                this._xpathDocument = new XPathDocument(textReader);
            }
            else if (!string.IsNullOrEmpty(this._documentSource))
            {
                VirtualPath path;
                string str;
                base.ResolvePhysicalOrVirtualPath(this._documentSource, out path, out str);
                CacheInternal cacheInternal = HttpRuntime.CacheInternal;
                string key = "p" + ((str != null) ? str : path.VirtualPathString);
                this._xpathDocument = (XPathDocument) cacheInternal.Get(key);
                if (this._xpathDocument == null)
                {
                    CacheDependency dependency;
                    using (Stream stream = base.OpenFileAndGetDependency(path, str, out dependency))
                    {
                        XmlTextReader reader2;
                        if (str == null)
                        {
                            str = path.MapPath();
                        }
                        if (AppSettings.RestrictXmlControls)
                        {
                            reader2 = new NoEntitiesXmlReader(str, stream);
                        }
                        else
                        {
                            reader2 = new XmlTextReader(str, stream);
                        }
                        this._xpathDocument = new XPathDocument(reader2);
                    }
                    if (dependency != null)
                    {
                        using (dependency)
                        {
                            cacheInternal.UtcInsert(key, this._xpathDocument, dependency);
                        }
                    }
                }
            }
        }

        protected internal override void Render(HtmlTextWriter output)
        {
            if ((this._document == null) && (this._xpathNavigator == null))
            {
                this.LoadXPathDocument();
            }
            this.LoadTransformFromSource();
            if (((this._document != null) || (this._xpathDocument != null)) || (this._xpathNavigator != null))
            {
                if (this._transform == null)
                {
                    this._transform = _identityTransform;
                }
                XmlUrlResolver resolver = null;
                if (HttpRuntime.HasUnmanagedPermission())
                {
                    resolver = new XmlUrlResolver();
                }
                IXPathNavigable input = null;
                if (this._document != null)
                {
                    input = this._document;
                }
                else if (this._xpathNavigator != null)
                {
                    input = this._xpathNavigator;
                }
                else
                {
                    input = this._xpathDocument;
                }
                if (AppSettings.RestrictXmlControls && (this._compiledTransform != null))
                {
                    this._compiledTransform.Transform(input, this._transformArgumentList, output);
                }
                else
                {
                    this._transform.Transform(input, this._transformArgumentList, (TextWriter) output, resolver);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ClientID =>
            base.ClientID;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ControlCollection Controls =>
            base.Controls;

        [Browsable(false), WebSysDescription("Xml_Document"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("The recommended alternative is the XPathNavigator property. Create a System.Xml.XPath.XPathDocument and call CreateNavigator() to create an XPathNavigator. http://go.microsoft.com/fwlink/?linkid=14202")]
        public XmlDocument Document
        {
            get
            {
                if (this._document == null)
                {
                    this.LoadXmlDocument();
                }
                return this._document;
            }
            set
            {
                this.DocumentSource = null;
                this._xpathDocument = null;
                this._documentContent = null;
                this._document = value;
            }
        }

        [WebSysDescription("Xml_DocumentContent"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public string DocumentContent
        {
            get
            {
                if (this._documentContent == null)
                {
                    return string.Empty;
                }
                return this._documentContent;
            }
            set
            {
                this._document = null;
                this._xpathDocument = null;
                this._xpathNavigator = null;
                this._documentContent = value;
                if (base.DesignMode)
                {
                    this.ViewState["OriginalContent"] = null;
                }
            }
        }

        [DefaultValue(""), WebSysDescription("Xml_DocumentSource"), WebCategory("Behavior"), Editor("System.Web.UI.Design.XmlUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty]
        public string DocumentSource
        {
            get
            {
                if (this._documentSource != null)
                {
                    return this._documentSource;
                }
                return string.Empty;
            }
            set
            {
                this._document = null;
                this._xpathDocument = null;
                this._documentContent = null;
                this._xpathNavigator = null;
                this._documentSource = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false)]
        public override bool EnableTheming
        {
            get => 
                false;
            set
            {
                throw new NotSupportedException(System.Web.SR.GetString("NoThemingSupport", new object[] { base.GetType().Name }));
            }
        }

        [Browsable(false), DefaultValue(""), EditorBrowsable(EditorBrowsableState.Never)]
        public override string SkinID
        {
            get => 
                string.Empty;
            set
            {
                throw new NotSupportedException(System.Web.SR.GetString("NoThemingSupport", new object[] { base.GetType().Name }));
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), WebSysDescription("Xml_Transform")]
        public XslTransform Transform
        {
            get
            {
                if (!AppSettings.RestrictXmlControls)
                {
                    return this._transform;
                }
                return null;
            }
            set
            {
                if (!AppSettings.RestrictXmlControls)
                {
                    this.TransformSource = null;
                    this._transform = value;
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebSysDescription("Xml_TransformArgumentList")]
        public XsltArgumentList TransformArgumentList
        {
            get => 
                this._transformArgumentList;
            set
            {
                this._transformArgumentList = value;
            }
        }

        [WebCategory("Behavior"), WebSysDescription("Xml_TransformSource"), DefaultValue(""), Editor("System.Web.UI.Design.XslUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string TransformSource
        {
            get
            {
                if (this._transformSource != null)
                {
                    return this._transformSource;
                }
                return string.Empty;
            }
            set
            {
                this._transform = null;
                this._transformSource = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebSysDescription("Xml_XPathNavigator")]
        public System.Xml.XPath.XPathNavigator XPathNavigator
        {
            get => 
                this._xpathNavigator;
            set
            {
                this.DocumentSource = null;
                this._xpathDocument = null;
                this._documentContent = null;
                this._document = null;
                this._xpathNavigator = value;
            }
        }
    }
}

