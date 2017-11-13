namespace AjaxControlToolkit.Sanitizer
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    internal class HtmlAgilityPackSanitizerProvider : SanitizerProvider
    {
        private string _applicationName;
        private string[] encodedCharacters = new string[0x100];

        public HtmlAgilityPackSanitizerProvider()
        {
            for (int i = 0; i < 0xff; i++)
            {
                if ((((i >= 0x30) && (i <= 0x39)) || ((i >= 0x41) && (i <= 90))) || ((i >= 0x61) && (i <= 0x7a)))
                {
                    this.encodedCharacters[i] = null;
                }
                else
                {
                    this.encodedCharacters[i] = i.ToString("X2");
                }
            }
        }

        private void CleanAttributeValues(HtmlAttribute attribute)
        {
            attribute.set_Value(HttpUtility.HtmlEncode(attribute.get_Value()));
            attribute.set_Value(Regex.Replace(attribute.get_Value(), @"\s*j\s*a\s*v\s*a\s*s\s*c\s*r\s*i\s*p\s*t\s*", "", RegexOptions.IgnoreCase));
            attribute.set_Value(Regex.Replace(attribute.get_Value(), @"\s*s\s*c\s*r\s*i\s*p\s*t\s*", "", RegexOptions.IgnoreCase));
            if (attribute.get_Name().ToLower() == "style")
            {
                attribute.set_Value(Regex.Replace(attribute.get_Value(), @"\s*e\s*x\s*p\s*r\s*e\s*s\s*s\s*i\s*o\s*n\s*", "", RegexOptions.IgnoreCase));
                attribute.set_Value(Regex.Replace(attribute.get_Value(), @"\s*b\s*e\s*h\s*a\s*v\s*i\s*o\s*r\s*", "", RegexOptions.IgnoreCase));
            }
            if ((attribute.get_Name().ToLower() == "href") || (attribute.get_Name().ToLower() == "src"))
            {
                attribute.set_Value(Regex.Replace(attribute.get_Value(), @"\s*m\s*o\s*c\s*h\s*a\s*", "", RegexOptions.IgnoreCase));
            }
            StringBuilder builder = new StringBuilder();
            foreach (char ch in attribute.get_Value().ToCharArray())
            {
                builder.Append(this.EncodeCharacterToHtmlEntityEscape(ch));
            }
            attribute.set_Value(builder.ToString());
        }

        private void CleanChildren(HtmlNode parent, string[] tagWhiteList)
        {
            for (int i = parent.get_ChildNodes().get_Count() - 1; i >= 0; i--)
            {
                this.CleanNodes(parent.get_ChildNodes().get_Item(i), tagWhiteList);
            }
        }

        private void CleanNodes(HtmlNode node, string[] tagWhiteList)
        {
            if ((node.get_NodeType() == 1) && !tagWhiteList.Contains<string>(node.get_Name()))
            {
                node.get_ParentNode().RemoveChild(node);
            }
            else if (node.get_HasChildNodes())
            {
                this.CleanChildren(node, tagWhiteList);
            }
        }

        private string EncodeCharacterToHtmlEntityEscape(char c)
        {
            string str;
            if (c < '\x00ff')
            {
                str = this.encodedCharacters[c];
                if (str == null)
                {
                    return (c);
                }
            }
            else
            {
                str = ((int) c).ToString("X2");
            }
            if ((((c <= '\x001f') && (c != '\t')) && ((c != '\n') && (c != '\r'))) || ((c >= '\x007f') && (c <= '\x009f')))
            {
                str = "fffd";
            }
            return ("&#x" + str + ";");
        }

        public override string GetSafeHtmlFragment(string htmlFragment, Dictionary<string, string[]> elementWhiteList, Dictionary<string, string[]> attributeWhiteList) => 
            this.SanitizeHtml(htmlFragment, elementWhiteList, attributeWhiteList);

        private string SanitizeHtml(string htmlText, Dictionary<string, string[]> elementWhiteList, Dictionary<string, string[]> attributeWhiteList)
        {
            HtmlDocument document = new HtmlDocument {
                OptionFixNestedTags = true,
                OptionAutoCloseOnEnd = true,
                OptionDefaultStreamEncoding = Encoding.UTF8
            };
            document.LoadHtml(htmlText);
            if (document == null)
            {
                return string.Empty;
            }
            HtmlNode node = document.get_DocumentNode();
            Dictionary<string, string[]> dictionary = elementWhiteList;
            string[] tagWhiteList = (from kv in dictionary select kv.Key).ToArray<string>();
            this.CleanNodes(node, tagWhiteList);
            using (Dictionary<string, string[]>.Enumerator enumerator = dictionary.GetEnumerator())
            {
                Func<HtmlNode, bool> predicate = null;
                KeyValuePair<string, string[]> tag;
                while (enumerator.MoveNext())
                {
                    tag = enumerator.Current;
                    if (predicate == null)
                    {
                        predicate = n => n.get_Name() == tag.Key;
                    }
                    IEnumerable<HtmlNode> enumerable = node.DescendantsAndSelf().Where<HtmlNode>(predicate);
                    if (enumerable != null)
                    {
                        foreach (HtmlNode node2 in enumerable)
                        {
                            if (node2.get_HasAttributes())
                            {
                                foreach (HtmlAttribute attribute in node2.get_Attributes().ToArray<HtmlAttribute>())
                                {
                                    if (!tag.Value.Contains<string>(attribute.get_Name()))
                                    {
                                        attribute.Remove();
                                    }
                                    else
                                    {
                                        this.CleanAttributeValues(attribute);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return node.get_InnerHtml();
        }

        public override string ApplicationName
        {
            get => 
                this._applicationName;
            set
            {
                this._applicationName = value;
            }
        }

        public override bool RequiresFullTrust =>
            false;
    }
}

