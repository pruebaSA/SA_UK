namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;

    internal class HtmlElementProps
    {
        private bool abrParent;
        private bool blockWS;
        private bool empty;
        private bool head;
        private bool nameParent;
        private bool noEntities;
        private static Hashtable s_table = CreatePropsTable();
        private bool uriParent;

        public static HtmlElementProps Create(bool empty, bool abrParent, bool uriParent, bool noEntities, bool blockWS, bool head, bool nameParent) => 
            new HtmlElementProps { 
                empty = empty,
                abrParent = abrParent,
                uriParent = uriParent,
                noEntities = noEntities,
                blockWS = blockWS,
                head = head,
                nameParent = nameParent
            };

        private static Hashtable CreatePropsTable()
        {
            bool empty = false;
            bool uriParent = true;
            return new Hashtable(0x47, StringComparer.OrdinalIgnoreCase) { 
                { 
                    "a",
                    Create(empty, empty, uriParent, empty, empty, empty, uriParent)
                },
                { 
                    "address",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "applet",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "area",
                    Create(uriParent, uriParent, uriParent, empty, uriParent, empty, empty)
                },
                { 
                    "base",
                    Create(uriParent, empty, uriParent, empty, uriParent, empty, empty)
                },
                { 
                    "basefont",
                    Create(uriParent, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "blockquote",
                    Create(empty, empty, uriParent, empty, uriParent, empty, empty)
                },
                { 
                    "body",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "br",
                    Create(uriParent, empty, empty, empty, empty, empty, empty)
                },
                { 
                    "button",
                    Create(empty, uriParent, empty, empty, empty, empty, empty)
                },
                { 
                    "caption",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "center",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "col",
                    Create(uriParent, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "colgroup",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "dd",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "del",
                    Create(empty, empty, uriParent, empty, uriParent, empty, empty)
                },
                { 
                    "dir",
                    Create(empty, uriParent, empty, empty, uriParent, empty, empty)
                },
                { 
                    "div",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "dl",
                    Create(empty, uriParent, empty, empty, uriParent, empty, empty)
                },
                { 
                    "dt",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "fieldset",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "font",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "form",
                    Create(empty, empty, uriParent, empty, uriParent, empty, empty)
                },
                { 
                    "frame",
                    Create(uriParent, uriParent, empty, empty, uriParent, empty, empty)
                },
                { 
                    "frameset",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "h1",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "h2",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "h3",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "h4",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "h5",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "h6",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "head",
                    Create(empty, empty, uriParent, empty, uriParent, uriParent, empty)
                },
                { 
                    "hr",
                    Create(uriParent, uriParent, empty, empty, uriParent, empty, empty)
                },
                { 
                    "html",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "iframe",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "img",
                    Create(uriParent, uriParent, uriParent, empty, empty, empty, empty)
                },
                { 
                    "input",
                    Create(uriParent, uriParent, uriParent, empty, empty, empty, empty)
                },
                { 
                    "ins",
                    Create(empty, empty, uriParent, empty, uriParent, empty, empty)
                },
                { 
                    "isindex",
                    Create(uriParent, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "legend",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "li",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "link",
                    Create(uriParent, empty, uriParent, empty, uriParent, empty, empty)
                },
                { 
                    "map",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "menu",
                    Create(empty, uriParent, empty, empty, uriParent, empty, empty)
                },
                { 
                    "meta",
                    Create(uriParent, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "noframes",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "noscript",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "object",
                    Create(empty, uriParent, uriParent, empty, empty, empty, empty)
                },
                { 
                    "ol",
                    Create(empty, uriParent, empty, empty, uriParent, empty, empty)
                },
                { 
                    "optgroup",
                    Create(empty, uriParent, empty, empty, uriParent, empty, empty)
                },
                { 
                    "option",
                    Create(empty, uriParent, empty, empty, uriParent, empty, empty)
                },
                { 
                    "p",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "param",
                    Create(uriParent, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "pre",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "q",
                    Create(empty, empty, uriParent, empty, empty, empty, empty)
                },
                { 
                    "s",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "script",
                    Create(empty, uriParent, uriParent, uriParent, empty, empty, empty)
                },
                { 
                    "select",
                    Create(empty, uriParent, empty, empty, empty, empty, empty)
                },
                { 
                    "strike",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "style",
                    Create(empty, empty, empty, uriParent, uriParent, empty, empty)
                },
                { 
                    "table",
                    Create(empty, empty, uriParent, empty, uriParent, empty, empty)
                },
                { 
                    "tbody",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "td",
                    Create(empty, uriParent, empty, empty, uriParent, empty, empty)
                },
                { 
                    "textarea",
                    Create(empty, uriParent, empty, empty, empty, empty, empty)
                },
                { 
                    "tfoot",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "th",
                    Create(empty, uriParent, empty, empty, uriParent, empty, empty)
                },
                { 
                    "thead",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "title",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "tr",
                    Create(empty, empty, empty, empty, uriParent, empty, empty)
                },
                { 
                    "ul",
                    Create(empty, uriParent, empty, empty, uriParent, empty, empty)
                },
                { 
                    "xmp",
                    Create(empty, empty, empty, empty, empty, empty, empty)
                }
            };
        }

        public static HtmlElementProps GetProps(string name) => 
            ((HtmlElementProps) s_table[name]);

        public bool AbrParent =>
            this.abrParent;

        public bool BlockWS =>
            this.blockWS;

        public bool Empty =>
            this.empty;

        public bool Head =>
            this.head;

        public bool NameParent =>
            this.nameParent;

        public bool NoEntities =>
            this.noEntities;

        public bool UriParent =>
            this.uriParent;
    }
}

