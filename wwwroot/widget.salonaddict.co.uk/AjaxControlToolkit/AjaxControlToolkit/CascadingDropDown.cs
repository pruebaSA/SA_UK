namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Xml;

    [TargetControlType(typeof(DropDownList)), ToolboxBitmap(typeof(CascadingDropDown), "CascadingDropDown.CascadingDropDown.ico"), Designer("AjaxControlToolkit.CascadingDropDownDesigner, AjaxControlToolkit"), ClientScriptResource("Sys.Extended.UI.CascadingDropDownBehavior", "CascadingDropDown.CascadingDropDownBehavior.js"), RequiredScript(typeof(CommonToolkitScripts))]
    public class CascadingDropDown : ExtenderControlBase
    {
        public CascadingDropDown()
        {
            base.ClientStateValuesLoaded += new EventHandler(this.CascadingDropDown_ClientStateValuesLoaded);
            base.EnableClientState = true;
        }

        private void CascadingDropDown_ClientStateValuesLoaded(object sender, EventArgs e)
        {
            DropDownList targetControl = (DropDownList) base.TargetControl;
            targetControl.Items.Clear();
            string str = ":::";
            string clientState = base.ClientState;
            int index = (clientState ?? "").IndexOf(str, StringComparison.Ordinal);
            if (-1 == index)
            {
                targetControl.Items.Add(clientState);
            }
            else
            {
                string[] strArray = Regex.Split(clientState, str);
                string str3 = strArray[0];
                string text = strArray[1];
                ListItem item = new ListItem(text, str3);
                if (strArray.Length > 2)
                {
                    string str5 = strArray[2];
                    item.Attributes.Add("title", str5);
                }
                targetControl.Items.Add(item);
            }
        }

        public static StringDictionary ParseKnownCategoryValuesString(string knownCategoryValues)
        {
            if (knownCategoryValues == null)
            {
                throw new ArgumentNullException("knownCategoryValues");
            }
            StringDictionary dictionary = new StringDictionary();
            if (knownCategoryValues != null)
            {
                foreach (string str in knownCategoryValues.Split(new char[] { ';' }))
                {
                    string[] strArray = str.Split(new char[] { ':' });
                    if (2 == strArray.Length)
                    {
                        dictionary.Add(strArray[0].ToLowerInvariant(), strArray[1]);
                    }
                }
            }
            return dictionary;
        }

        public static CascadingDropDownNameValue[] QuerySimpleCascadingDropDownDocument(XmlDocument document, string[] documentHierarchy, StringDictionary knownCategoryValuesDictionary, string category) => 
            QuerySimpleCascadingDropDownDocument(document, documentHierarchy, knownCategoryValuesDictionary, category, new Regex(@"^[^/'\*]*$"));

        public static CascadingDropDownNameValue[] QuerySimpleCascadingDropDownDocument(XmlDocument document, string[] documentHierarchy, StringDictionary knownCategoryValuesDictionary, string category, Regex inputValidationRegex)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            if (documentHierarchy == null)
            {
                throw new ArgumentNullException("documentHierarchy");
            }
            if (knownCategoryValuesDictionary == null)
            {
                throw new ArgumentNullException("knownCategoryValuesDictionary");
            }
            if (category == null)
            {
                throw new ArgumentNullException("category");
            }
            if (inputValidationRegex == null)
            {
                throw new ArgumentNullException("inputValidationRegex");
            }
            foreach (string str in knownCategoryValuesDictionary.Keys)
            {
                if (!inputValidationRegex.IsMatch(str) || !inputValidationRegex.IsMatch(knownCategoryValuesDictionary[str]))
                {
                    throw new ArgumentException("Invalid characters present.", "category");
                }
            }
            if (!inputValidationRegex.IsMatch(category))
            {
                throw new ArgumentException("Invalid characters present.", "category");
            }
            string xpath = "/" + document.DocumentElement.Name;
            foreach (string str3 in documentHierarchy)
            {
                if (knownCategoryValuesDictionary.ContainsKey(str3))
                {
                    xpath = xpath + string.Format(CultureInfo.InvariantCulture, "/{0}[(@name and @value='{1}') or (@name='{1}' and not(@value))]", new object[] { str3, knownCategoryValuesDictionary[str3] });
                }
            }
            xpath = xpath + "/" + category.ToLowerInvariant();
            List<CascadingDropDownNameValue> list = new List<CascadingDropDownNameValue>();
            foreach (XmlNode node in document.SelectNodes(xpath))
            {
                string name = node.Attributes.GetNamedItem("name").Value;
                XmlNode namedItem = node.Attributes.GetNamedItem("value");
                string str5 = (namedItem != null) ? namedItem.Value : name;
                XmlNode node3 = node.Attributes.GetNamedItem("default");
                bool defaultValue = (node3 != null) ? bool.Parse(node3.Value) : false;
                CascadingDropDownNameValue item = new CascadingDropDownNameValue(name, str5, defaultValue);
                XmlNode node4 = node.Attributes.GetNamedItem("optionTitle");
                string str6 = (node4 != null) ? node4.Value : "";
                item.optionTitle = str6;
                list.Add(item);
            }
            return list.ToArray();
        }

        private bool ShouldSerializeServicePath() => 
            !string.IsNullOrEmpty(this.ServiceMethod);

        [ExtenderControlProperty, RequiredProperty, DefaultValue("")]
        public string Category
        {
            get => 
                base.GetPropertyValue<string>("Category", "");
            set
            {
                base.SetPropertyValue<string>("Category", value);
            }
        }

        [DefaultValue((string) null), ExtenderControlProperty, ClientPropertyName("contextKey")]
        public string ContextKey
        {
            get => 
                base.GetPropertyValue<string>("ContextKey", null);
            set
            {
                base.SetPropertyValue<string>("ContextKey", value);
                this.UseContextKey = true;
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string EmptyText
        {
            get => 
                base.GetPropertyValue<string>("EmptyText", "");
            set
            {
                base.SetPropertyValue<string>("EmptyText", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string EmptyValue
        {
            get => 
                base.GetPropertyValue<string>("EmptyValue", "");
            set
            {
                base.SetPropertyValue<string>("EmptyValue", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string LoadingText
        {
            get => 
                base.GetPropertyValue<string>("LoadingText", "");
            set
            {
                base.SetPropertyValue<string>("LoadingText", value);
            }
        }

        [ExtenderControlProperty, IDReferenceProperty(typeof(DropDownList)), DefaultValue("")]
        public string ParentControlID
        {
            get => 
                base.GetPropertyValue<string>("ParentControlID", "");
            set
            {
                base.SetPropertyValue<string>("ParentControlID", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string PromptText
        {
            get => 
                base.GetPropertyValue<string>("PromptText", "");
            set
            {
                base.SetPropertyValue<string>("PromptText", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string PromptValue
        {
            get => 
                base.GetPropertyValue<string>("PromptValue", "");
            set
            {
                base.SetPropertyValue<string>("PromptValue", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty]
        public string SelectedValue
        {
            get => 
                (base.ClientState ?? "");
            set
            {
                base.ClientState = value;
            }
        }

        [ExtenderControlProperty, RequiredProperty, DefaultValue("")]
        public string ServiceMethod
        {
            get => 
                base.GetPropertyValue<string>("ServiceMethod", "");
            set
            {
                base.SetPropertyValue<string>("ServiceMethod", value);
            }
        }

        [TypeConverter(typeof(ServicePathConverter)), UrlProperty, ExtenderControlProperty]
        public string ServicePath
        {
            get => 
                base.GetPropertyValue<string>("ServicePath", "");
            set
            {
                base.SetPropertyValue<string>("ServicePath", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(false), ClientPropertyName("useContextKey")]
        public bool UseContextKey
        {
            get => 
                base.GetPropertyValue<bool>("UseContextKey", false);
            set
            {
                base.SetPropertyValue<bool>("UseContextKey", value);
            }
        }
    }
}

