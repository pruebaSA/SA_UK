namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.Util;

    internal class HtmlTagNameToTypeMapper : ITagNameToTypeMapper
    {
        private static Hashtable _inputTypes;
        private static Hashtable _tagMap;

        internal HtmlTagNameToTypeMapper()
        {
        }

        Type ITagNameToTypeMapper.GetControlType(string tagName, IDictionary attributeBag)
        {
            Type type;
            if (_tagMap == null)
            {
                Hashtable hashtable = new Hashtable(10, StringComparer.OrdinalIgnoreCase) {
                    { 
                        "a",
                        typeof(HtmlAnchor)
                    },
                    { 
                        "button",
                        typeof(HtmlButton)
                    },
                    { 
                        "form",
                        typeof(HtmlForm)
                    },
                    { 
                        "head",
                        typeof(HtmlHead)
                    },
                    { 
                        "img",
                        typeof(HtmlImage)
                    },
                    { 
                        "textarea",
                        typeof(HtmlTextArea)
                    },
                    { 
                        "select",
                        typeof(HtmlSelect)
                    },
                    { 
                        "table",
                        typeof(HtmlTable)
                    },
                    { 
                        "tr",
                        typeof(HtmlTableRow)
                    },
                    { 
                        "td",
                        typeof(HtmlTableCell)
                    },
                    { 
                        "th",
                        typeof(HtmlTableCell)
                    }
                };
                _tagMap = hashtable;
            }
            if (_inputTypes == null)
            {
                Hashtable hashtable2 = new Hashtable(10, StringComparer.OrdinalIgnoreCase) {
                    { 
                        "text",
                        typeof(HtmlInputText)
                    },
                    { 
                        "password",
                        typeof(HtmlInputPassword)
                    },
                    { 
                        "button",
                        typeof(HtmlInputButton)
                    },
                    { 
                        "submit",
                        typeof(HtmlInputSubmit)
                    },
                    { 
                        "reset",
                        typeof(HtmlInputReset)
                    },
                    { 
                        "image",
                        typeof(HtmlInputImage)
                    },
                    { 
                        "checkbox",
                        typeof(HtmlInputCheckBox)
                    },
                    { 
                        "radio",
                        typeof(HtmlInputRadioButton)
                    },
                    { 
                        "hidden",
                        typeof(HtmlInputHidden)
                    },
                    { 
                        "file",
                        typeof(HtmlInputFile)
                    }
                };
                _inputTypes = hashtable2;
            }
            if (StringUtil.EqualsIgnoreCase("input", tagName))
            {
                string str = (string) attributeBag["type"];
                if (str == null)
                {
                    str = "text";
                }
                type = (Type) _inputTypes[str];
                if (type == null)
                {
                    throw new HttpException(System.Web.SR.GetString("Invalid_type_for_input_tag", new object[] { str }));
                }
                return type;
            }
            type = (Type) _tagMap[tagName];
            if (type == null)
            {
                type = typeof(HtmlGenericControl);
            }
            return type;
        }
    }
}

