namespace MigraDoc.DocumentObjectModel.IO
{
    using System;
    using System.Collections;

    internal class KeyWords
    {
        protected static Hashtable enumToName = new Hashtable();
        protected static Hashtable nameToEnum = new Hashtable();

        static KeyWords()
        {
            enumToName.Add(Symbol.True, "true");
            enumToName.Add(Symbol.False, "false");
            enumToName.Add(Symbol.Null, "null");
            enumToName.Add(Symbol.Styles, @"\styles");
            enumToName.Add(Symbol.Document, @"\document");
            enumToName.Add(Symbol.Section, @"\section");
            enumToName.Add(Symbol.Paragraph, @"\paragraph");
            enumToName.Add(Symbol.Header, @"\header");
            enumToName.Add(Symbol.Footer, @"\footer");
            enumToName.Add(Symbol.PrimaryHeader, @"\primaryheader");
            enumToName.Add(Symbol.PrimaryFooter, @"\primaryfooter");
            enumToName.Add(Symbol.FirstPageHeader, @"\firstpageheader");
            enumToName.Add(Symbol.FirstPageFooter, @"\firstpagefooter");
            enumToName.Add(Symbol.EvenPageHeader, @"\evenpageheader");
            enumToName.Add(Symbol.EvenPageFooter, @"\evenpagefooter");
            enumToName.Add(Symbol.Table, @"\table");
            enumToName.Add(Symbol.Columns, @"\columns");
            enumToName.Add(Symbol.Column, @"\column");
            enumToName.Add(Symbol.Rows, @"\rows");
            enumToName.Add(Symbol.Row, @"\row");
            enumToName.Add(Symbol.Cell, @"\cell");
            enumToName.Add(Symbol.Image, @"\image");
            enumToName.Add(Symbol.TextFrame, @"\textframe");
            enumToName.Add(Symbol.PageBreak, @"\pagebreak");
            enumToName.Add(Symbol.Barcode, @"\barcode");
            enumToName.Add(Symbol.Chart, @"\chart");
            enumToName.Add(Symbol.HeaderArea, @"\headerarea");
            enumToName.Add(Symbol.FooterArea, @"\footerarea");
            enumToName.Add(Symbol.TopArea, @"\toparea");
            enumToName.Add(Symbol.BottomArea, @"\bottomarea");
            enumToName.Add(Symbol.LeftArea, @"\leftarea");
            enumToName.Add(Symbol.RightArea, @"\rightarea");
            enumToName.Add(Symbol.PlotArea, @"\plotarea");
            enumToName.Add(Symbol.Legend, @"\legend");
            enumToName.Add(Symbol.XAxis, @"\xaxis");
            enumToName.Add(Symbol.YAxis, @"\yaxis");
            enumToName.Add(Symbol.ZAxis, @"\zaxis");
            enumToName.Add(Symbol.Series, @"\series");
            enumToName.Add(Symbol.XValues, @"\xvalues");
            enumToName.Add(Symbol.Point, @"\point");
            enumToName.Add(Symbol.Bold, @"\bold");
            enumToName.Add(Symbol.Italic, @"\italic");
            enumToName.Add(Symbol.Underline, @"\underline");
            enumToName.Add(Symbol.FontSize, @"\fontsize");
            enumToName.Add(Symbol.FontColor, @"\fontcolor");
            enumToName.Add(Symbol.Font, @"\font");
            enumToName.Add(Symbol.Field, @"\field");
            enumToName.Add(Symbol.Symbol, @"\symbol");
            enumToName.Add(Symbol.Chr, @"\chr");
            enumToName.Add(Symbol.Footnote, @"\footnote");
            enumToName.Add(Symbol.Hyperlink, @"\hyperlink");
            enumToName.Add(Symbol.SoftHyphen, @"\-");
            enumToName.Add(Symbol.Tab, @"\tab");
            enumToName.Add(Symbol.LineBreak, @"\linebreak");
            enumToName.Add(Symbol.Space, @"\space");
            enumToName.Add(Symbol.NoSpace, @"\nospace");
            enumToName.Add(Symbol.BraceLeft, "{");
            enumToName.Add(Symbol.BraceRight, "}");
            enumToName.Add(Symbol.BracketLeft, "[");
            enumToName.Add(Symbol.BracketRight, "]");
            enumToName.Add(Symbol.ParenLeft, "(");
            enumToName.Add(Symbol.ParenRight, ")");
            enumToName.Add(Symbol.Colon, ":");
            enumToName.Add(Symbol.Semicolon, ";");
            enumToName.Add(Symbol.Dot, ".");
            enumToName.Add(Symbol.Comma, ",");
            enumToName.Add(Symbol.Percent, "%");
            enumToName.Add(Symbol.Dollar, "$");
            enumToName.Add(Symbol.Hash, "#");
            enumToName.Add(Symbol.Assign, "=");
            enumToName.Add(Symbol.Slash, "/");
            enumToName.Add(Symbol.BackSlash, @"\");
            enumToName.Add(Symbol.Plus, "+");
            enumToName.Add(Symbol.PlusAssign, "+=");
            enumToName.Add(Symbol.Minus, "-");
            enumToName.Add(Symbol.MinusAssign, "-=");
            enumToName.Add(Symbol.Blank, " ");
            nameToEnum.Add("true", Symbol.True);
            nameToEnum.Add("false", Symbol.False);
            nameToEnum.Add("null", Symbol.Null);
            nameToEnum.Add(@"\styles", Symbol.Styles);
            nameToEnum.Add(@"\document", Symbol.Document);
            nameToEnum.Add(@"\section", Symbol.Section);
            nameToEnum.Add(@"\paragraph", Symbol.Paragraph);
            nameToEnum.Add(@"\header", Symbol.Header);
            nameToEnum.Add(@"\footer", Symbol.Footer);
            nameToEnum.Add(@"\primaryheader", Symbol.PrimaryHeader);
            nameToEnum.Add(@"\primaryfooter", Symbol.PrimaryFooter);
            nameToEnum.Add(@"\firstpageheader", Symbol.FirstPageHeader);
            nameToEnum.Add(@"\firstpagefooter", Symbol.FirstPageFooter);
            nameToEnum.Add(@"\evenpageheader", Symbol.EvenPageHeader);
            nameToEnum.Add(@"\evenpagefooter", Symbol.EvenPageFooter);
            nameToEnum.Add(@"\table", Symbol.Table);
            nameToEnum.Add(@"\columns", Symbol.Columns);
            nameToEnum.Add(@"\column", Symbol.Column);
            nameToEnum.Add(@"\rows", Symbol.Rows);
            nameToEnum.Add(@"\row", Symbol.Row);
            nameToEnum.Add(@"\cell", Symbol.Cell);
            nameToEnum.Add(@"\image", Symbol.Image);
            nameToEnum.Add(@"\textframe", Symbol.TextFrame);
            nameToEnum.Add(@"\pagebreak", Symbol.PageBreak);
            nameToEnum.Add(@"\barcode", Symbol.Barcode);
            nameToEnum.Add(@"\chart", Symbol.Chart);
            nameToEnum.Add(@"\headerarea", Symbol.HeaderArea);
            nameToEnum.Add(@"\footerarea", Symbol.FooterArea);
            nameToEnum.Add(@"\toparea", Symbol.TopArea);
            nameToEnum.Add(@"\bottomarea", Symbol.BottomArea);
            nameToEnum.Add(@"\leftarea", Symbol.LeftArea);
            nameToEnum.Add(@"\rightarea", Symbol.RightArea);
            nameToEnum.Add(@"\plotarea", Symbol.PlotArea);
            nameToEnum.Add(@"\legend", Symbol.Legend);
            nameToEnum.Add(@"\xaxis", Symbol.XAxis);
            nameToEnum.Add(@"\yaxis", Symbol.YAxis);
            nameToEnum.Add(@"\zaxis", Symbol.ZAxis);
            nameToEnum.Add(@"\series", Symbol.Series);
            nameToEnum.Add(@"\xvalues", Symbol.XValues);
            nameToEnum.Add(@"\point", Symbol.Point);
            nameToEnum.Add(@"\bold", Symbol.Bold);
            nameToEnum.Add(@"\italic", Symbol.Italic);
            nameToEnum.Add(@"\underline", Symbol.Underline);
            nameToEnum.Add(@"\fontsize", Symbol.FontSize);
            nameToEnum.Add(@"\fontcolor", Symbol.FontColor);
            nameToEnum.Add(@"\font", Symbol.Font);
            nameToEnum.Add(@"\field", Symbol.Field);
            nameToEnum.Add(@"\symbol", Symbol.Symbol);
            nameToEnum.Add(@"\chr", Symbol.Chr);
            nameToEnum.Add(@"\footnote", Symbol.Footnote);
            nameToEnum.Add(@"\hyperlink", Symbol.Hyperlink);
            nameToEnum.Add(@"\-", Symbol.SoftHyphen);
            nameToEnum.Add(@"\tab", Symbol.Tab);
            nameToEnum.Add(@"\linebreak", Symbol.LineBreak);
            nameToEnum.Add(@"\space", Symbol.Space);
            nameToEnum.Add(@"\nospace", Symbol.NoSpace);
        }

        internal static string NameFromSymbol(Symbol symbol)
        {
            object obj2 = enumToName[symbol];
            return ((obj2 != null) ? ((string) obj2) : null);
        }

        internal static Symbol SymbolFromName(string name)
        {
            object obj2 = nameToEnum[name];
            if (obj2 == null)
            {
                if (string.Compare(name, "True", false) == 0)
                {
                    return Symbol.True;
                }
                if (string.Compare(name, "False", false) == 0)
                {
                    return Symbol.False;
                }
                if (string.Compare(name, "Null", false) == 0)
                {
                    return Symbol.Null;
                }
                return Symbol.None;
            }
            return (Symbol) obj2;
        }
    }
}

