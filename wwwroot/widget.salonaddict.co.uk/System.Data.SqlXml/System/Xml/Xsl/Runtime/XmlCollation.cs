namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Xml.Xsl;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class XmlCollation
    {
        private const int CollationFlagsMask = 0x7000;
        private CompareOptions compops;
        private static XmlCollation cp = new XmlCollation(CultureInfo.InvariantCulture, CompareOptions.Ordinal);
        private CultureInfo cultinfo;
        private const int deDE = 0x407;
        private const int deDEphon = 0x10407;
        private bool descendingOrder;
        private const string descendingOrderStr = "DESCENDINGORDER";
        private bool emptyGreatest;
        private const string emptyGreatestStr = "EMPTYGREATEST";
        private const int FlagDescendingOrder = 0x4000;
        private const int FlagEmptyGreatest = 0x2000;
        private const int FlagUpperFirst = 0x1000;
        private const int huHU = 0x40e;
        private const int huHUtech = 0x1040e;
        private const string ignoreCaseStr = "IGNORECASE";
        private const string ignoreKanatypeStr = "IGNOREKANATYPE";
        private const string ignoreNonspaceStr = "IGNORENONSPACE";
        private const string ignoreSymbolsStr = "IGNORESYMBOLS";
        private const string ignoreWidthStr = "IGNOREWIDTH";
        private const int jaJP = 0x411;
        private const int kaGE = 0x437;
        private const int kaGEmode = 0x10437;
        private const int koKR = 0x412;
        private const int LOCALE_CURRENT = -1;
        private const string sortStr = "SORT";
        private const int strksort = 2;
        private const int unicsort = 1;
        private bool upperFirst;
        private const string upperFirstStr = "UPPERFIRST";
        private const int zhCN = 0x804;
        private const int zhHK = 0xc04;
        private const int zhMO = 0x1404;
        private const int zhSG = 0x1004;
        private const int zhTW = 0x404;
        private const int zhTWbopo = 0x30404;

        private XmlCollation() : this(null, CompareOptions.None)
        {
        }

        internal XmlCollation(BinaryReader reader)
        {
            int culture = reader.ReadInt32();
            this.cultinfo = (culture != -1) ? new CultureInfo(culture) : null;
            this.SetOptions(reader.ReadInt32());
        }

        private XmlCollation(CultureInfo cultureInfo, CompareOptions compareOptions)
        {
            this.cultinfo = cultureInfo;
            this.compops = compareOptions;
        }

        internal static XmlCollation Create(string collationLiteral)
        {
            string str3;
            if (collationLiteral == "http://www.w3.org/2004/10/xpath-functions/collation/codepoint")
            {
                return CodePointCollation;
            }
            XmlCollation collation = new XmlCollation();
            Uri uri = new Uri(collationLiteral);
            if (uri.GetLeftPart(UriPartial.Authority) == "http://collations.microsoft.com")
            {
                string name = uri.LocalPath.Substring(1);
                if (name.Length == 0)
                {
                    goto Label_00AC;
                }
                try
                {
                    collation.cultinfo = new CultureInfo(name);
                    goto Label_00AC;
                }
                catch (ArgumentException)
                {
                    throw new XslTransformException("Coll_UnsupportedLanguage", new string[] { name });
                }
            }
            if (!uri.IsBaseOf(new Uri("http://www.w3.org/2004/10/xpath-functions/collation/codepoint")))
            {
                throw new XslTransformException("Coll_Unsupported", new string[] { collationLiteral });
            }
            collation.compops = CompareOptions.Ordinal;
        Label_00AC:
            str3 = uri.Query;
            string str4 = null;
            if (str3.Length != 0)
            {
                foreach (string str5 in str3.Substring(1).Split(new char[] { '&' }))
                {
                    string[] strArray = str5.Split(new char[] { '=' });
                    if (strArray.Length != 2)
                    {
                        throw new XslTransformException("Coll_BadOptFormat", new string[] { str5 });
                    }
                    string str6 = strArray[0].ToUpper(CultureInfo.InvariantCulture);
                    string str7 = strArray[1].ToUpper(CultureInfo.InvariantCulture);
                    if (str6 == "SORT")
                    {
                        str4 = str7;
                    }
                    else
                    {
                        switch (str7)
                        {
                            case "1":
                            case "TRUE":
                                switch (str6)
                                {
                                    case "IGNORECASE":
                                        collation.compops |= CompareOptions.IgnoreCase;
                                        goto Label_0479;

                                    case "IGNOREKANATYPE":
                                        collation.compops |= CompareOptions.IgnoreKanaType;
                                        goto Label_0479;

                                    case "IGNORENONSPACE":
                                        collation.compops |= CompareOptions.IgnoreNonSpace;
                                        goto Label_0479;

                                    case "IGNORESYMBOLS":
                                        collation.compops |= CompareOptions.IgnoreSymbols;
                                        goto Label_0479;

                                    case "IGNOREWIDTH":
                                        collation.compops |= CompareOptions.IgnoreWidth;
                                        goto Label_0479;

                                    case "UPPERFIRST":
                                        collation.upperFirst = true;
                                        goto Label_0479;

                                    case "EMPTYGREATEST":
                                        collation.emptyGreatest = true;
                                        goto Label_0479;

                                    case "DESCENDINGORDER":
                                        collation.descendingOrder = true;
                                        goto Label_0479;
                                }
                                throw new XslTransformException("Coll_UnsupportedOpt", new string[] { strArray[0] });

                            case "0":
                            case "FALSE":
                                switch (str6)
                                {
                                    case "IGNORECASE":
                                        collation.compops &= ~CompareOptions.IgnoreCase;
                                        goto Label_0479;

                                    case "IGNOREKANATYPE":
                                        collation.compops &= ~CompareOptions.IgnoreKanaType;
                                        goto Label_0479;

                                    case "IGNORENONSPACE":
                                        collation.compops &= ~CompareOptions.IgnoreNonSpace;
                                        goto Label_0479;

                                    case "IGNORESYMBOLS":
                                        collation.compops &= ~CompareOptions.IgnoreSymbols;
                                        goto Label_0479;

                                    case "IGNOREWIDTH":
                                        collation.compops &= ~CompareOptions.IgnoreWidth;
                                        goto Label_0479;

                                    case "UPPERFIRST":
                                        collation.upperFirst = false;
                                        goto Label_0479;

                                    case "EMPTYGREATEST":
                                        collation.emptyGreatest = false;
                                        goto Label_0479;

                                    case "DESCENDINGORDER":
                                        collation.descendingOrder = false;
                                        goto Label_0479;
                                }
                                throw new XslTransformException("Coll_UnsupportedOpt", new string[] { strArray[0] });

                            default:
                                throw new XslTransformException("Coll_UnsupportedOptVal", new string[] { strArray[0], strArray[1] });
                        }
                    Label_0479:;
                    }
                }
            }
            if (collation.upperFirst && ((collation.compops & CompareOptions.IgnoreCase) != CompareOptions.None))
            {
                collation.upperFirst = false;
            }
            if ((collation.compops & CompareOptions.Ordinal) != CompareOptions.None)
            {
                collation.compops = CompareOptions.Ordinal;
                collation.upperFirst = false;
            }
            if ((str4 == null) || (collation.cultinfo == null))
            {
                return collation;
            }
            int langID = GetLangID(collation.cultinfo.LCID);
            switch (str4)
            {
                case "bopo":
                    if (langID == 0x404)
                    {
                        collation.cultinfo = new CultureInfo(0x30404);
                    }
                    return collation;

                case "strk":
                    switch (langID)
                    {
                        case 0x804:
                        case 0xc04:
                        case 0x1004:
                        case 0x1404:
                            collation.cultinfo = new CultureInfo(MakeLCID(collation.cultinfo.LCID, 2));
                            break;
                    }
                    return collation;

                case "uni":
                    switch (langID)
                    {
                        case 0x411:
                        case 0x412:
                            collation.cultinfo = new CultureInfo(MakeLCID(collation.cultinfo.LCID, 1));
                            break;
                    }
                    return collation;

                case "phn":
                    if (langID == 0x407)
                    {
                        collation.cultinfo = new CultureInfo(0x10407);
                    }
                    return collation;

                case "tech":
                    if (langID == 0x40e)
                    {
                        collation.cultinfo = new CultureInfo(0x1040e);
                    }
                    return collation;

                case "mod":
                    if (langID == 0x437)
                    {
                        collation.cultinfo = new CultureInfo(0x10437);
                    }
                    return collation;

                case "pron":
                case "dict":
                case "trad":
                    return collation;
            }
            throw new XslTransformException("Coll_UnsupportedSortOpt", new string[] { str4 });
        }

        internal XmlSortKey CreateSortKey(string s)
        {
            SortKey sortKey = this.Culture.CompareInfo.GetSortKey(s, this.compops);
            if (!this.upperFirst)
            {
                return new XmlStringSortKey(sortKey, this.descendingOrder);
            }
            byte[] keyData = sortKey.KeyData;
            if (this.upperFirst && (keyData.Length != 0))
            {
                int index = 0;
                while (keyData[index] != 1)
                {
                    index++;
                }
                do
                {
                    index++;
                }
                while (keyData[index] != 1);
                do
                {
                    index++;
                    keyData[index] = (byte) (keyData[index] ^ 0xff);
                }
                while (keyData[index] != 0xfe);
            }
            return new XmlStringSortKey(keyData, this.descendingOrder);
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            XmlCollation collation = obj as XmlCollation;
            return (((collation != null) && (this.GetOptions() == collation.GetOptions())) && object.Equals(this.cultinfo, collation.cultinfo));
        }

        public override int GetHashCode()
        {
            int options = this.GetOptions();
            if (this.cultinfo != null)
            {
                options ^= this.cultinfo.GetHashCode();
            }
            return options;
        }

        private static int GetLangID(int lcid) => 
            (lcid & 0xffff);

        internal void GetObjectData(BinaryWriter writer)
        {
            writer.Write((this.cultinfo != null) ? this.cultinfo.LCID : -1);
            writer.Write(this.GetOptions());
        }

        private int GetOptions()
        {
            int compops = (int) this.compops;
            if (this.upperFirst)
            {
                compops |= 0x1000;
            }
            if (this.emptyGreatest)
            {
                compops |= 0x2000;
            }
            if (this.descendingOrder)
            {
                compops |= 0x4000;
            }
            return compops;
        }

        private static int MakeLCID(int langid, int sortid) => 
            ((langid & 0xffff) | ((sortid & 15) << 0x10));

        private void SetOptions(int options)
        {
            this.upperFirst = (options & 0x1000) != 0;
            this.emptyGreatest = (options & 0x2000) != 0;
            this.descendingOrder = (options & 0x4000) != 0;
            this.compops = ((CompareOptions) options) & ((CompareOptions) (-28673));
        }

        internal static XmlCollation CodePointCollation =>
            cp;

        internal CultureInfo Culture
        {
            get
            {
                if (this.cultinfo == null)
                {
                    return CultureInfo.CurrentCulture;
                }
                return this.cultinfo;
            }
        }

        internal bool DescendingOrder =>
            this.descendingOrder;

        internal bool EmptyGreatest =>
            this.emptyGreatest;
    }
}

