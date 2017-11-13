namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Web;
    using System.Web.UI;
    using System.Web.Util;

    internal class ContentBuilderInternal : TemplateBuilder
    {
        private string _contentPlaceHolder;
        private string _contentPlaceHolderFilter;
        private const string _contentPlaceHolderIDPropName = "ContentPlaceHolderID";

        public override object BuildObject()
        {
            if (base.InDesigner)
            {
                return base.BuildObjectInternal();
            }
            return base.BuildObject();
        }

        public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string ID, IDictionary attribs)
        {
            foreach (FilteredAttributeDictionary dictionary in ControlBuilder.ConvertDictionaryToParsedAttributeCollection(attribs).GetFilteredAttributeDictionaries())
            {
                string filter = dictionary.Filter;
                foreach (DictionaryEntry entry in (IEnumerable) dictionary)
                {
                    if (StringUtil.EqualsIgnoreCase((string) entry.Key, "ContentPlaceHolderID"))
                    {
                        if (this._contentPlaceHolder != null)
                        {
                            throw new HttpException(System.Web.SR.GetString("Content_only_one_contentPlaceHolderID_allowed"));
                        }
                        entry.Key.ToString();
                        this._contentPlaceHolder = entry.Value.ToString();
                        this._contentPlaceHolderFilter = filter;
                    }
                }
            }
            if (!parser.FInDesigner)
            {
                if (this._contentPlaceHolder == null)
                {
                    throw new HttpException(System.Web.SR.GetString("Control_Missing_Attribute", new object[] { "ContentPlaceHolderID", type.Name }));
                }
                attribs.Clear();
            }
            base.Init(parser, parentBuilder, type, tagName, ID, attribs);
        }

        public override void InstantiateIn(Control container)
        {
            base.InstantiateIn(container);
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                TemplateControl templateControl = current.TemplateControl;
                if ((templateControl != null) && templateControl.NoCompile)
                {
                    foreach (Control control2 in container.Controls)
                    {
                        control2.TemplateControl = templateControl;
                    }
                }
            }
        }

        internal override void SetParentBuilder(ControlBuilder parentBuilder)
        {
            if (!base.InDesigner && !(parentBuilder is FileLevelPageControlBuilder))
            {
                throw new HttpException(System.Web.SR.GetString("Content_allowed_in_top_level_only"));
            }
            base.SetParentBuilder(parentBuilder);
        }

        public override Type BindingContainerType =>
            typeof(Control);

        internal string ContentPlaceHolder =>
            this._contentPlaceHolder;

        internal string ContentPlaceHolderFilter =>
            this._contentPlaceHolderFilter;
    }
}

