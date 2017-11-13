namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;

    internal class HtmlAttributeProps
    {
        private bool abr;
        private bool name;
        private static Hashtable s_table = CreatePropsTable();
        private bool uri;

        public static HtmlAttributeProps Create(bool abr, bool uri, bool name) => 
            new HtmlAttributeProps { 
                abr = abr,
                uri = uri,
                name = name
            };

        private static Hashtable CreatePropsTable()
        {
            bool abr = false;
            bool uri = true;
            return new Hashtable(0x1a, StringComparer.OrdinalIgnoreCase) { 
                { 
                    "action",
                    Create(abr, uri, abr)
                },
                { 
                    "checked",
                    Create(uri, abr, abr)
                },
                { 
                    "cite",
                    Create(abr, uri, abr)
                },
                { 
                    "classid",
                    Create(abr, uri, abr)
                },
                { 
                    "codebase",
                    Create(abr, uri, abr)
                },
                { 
                    "compact",
                    Create(uri, abr, abr)
                },
                { 
                    "data",
                    Create(abr, uri, abr)
                },
                { 
                    "datasrc",
                    Create(abr, uri, abr)
                },
                { 
                    "declare",
                    Create(uri, abr, abr)
                },
                { 
                    "defer",
                    Create(uri, abr, abr)
                },
                { 
                    "disabled",
                    Create(uri, abr, abr)
                },
                { 
                    "for",
                    Create(abr, uri, abr)
                },
                { 
                    "href",
                    Create(abr, uri, abr)
                },
                { 
                    "ismap",
                    Create(uri, abr, abr)
                },
                { 
                    "longdesc",
                    Create(abr, uri, abr)
                },
                { 
                    "multiple",
                    Create(uri, abr, abr)
                },
                { 
                    "name",
                    Create(abr, abr, uri)
                },
                { 
                    "nohref",
                    Create(uri, abr, abr)
                },
                { 
                    "noresize",
                    Create(uri, abr, abr)
                },
                { 
                    "noshade",
                    Create(uri, abr, abr)
                },
                { 
                    "nowrap",
                    Create(uri, abr, abr)
                },
                { 
                    "profile",
                    Create(abr, uri, abr)
                },
                { 
                    "readonly",
                    Create(uri, abr, abr)
                },
                { 
                    "selected",
                    Create(uri, abr, abr)
                },
                { 
                    "src",
                    Create(abr, uri, abr)
                },
                { 
                    "usemap",
                    Create(abr, uri, abr)
                }
            };
        }

        public static HtmlAttributeProps GetProps(string name) => 
            ((HtmlAttributeProps) s_table[name]);

        public bool Abr =>
            this.abr;

        public bool Name =>
            this.name;

        public bool Uri =>
            this.uri;
    }
}

