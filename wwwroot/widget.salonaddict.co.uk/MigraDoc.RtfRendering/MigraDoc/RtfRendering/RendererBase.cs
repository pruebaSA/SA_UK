namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Tables;
    using System;
    using System.Collections;
    using System.Globalization;

    public abstract class RendererBase
    {
        protected DocumentObject docObject;
        protected RtfDocumentRenderer docRenderer;
        protected static Hashtable enumTranslationTable;
        internal RtfWriter rtfWriter;
        protected bool useEffectiveValue;

        internal RendererBase()
        {
        }

        internal RendererBase(DocumentObject domObj, RtfDocumentRenderer docRenderer)
        {
            if (enumTranslationTable == null)
            {
                CreateEnumTranslationTable();
            }
            this.docObject = domObj;
            this.docRenderer = docRenderer;
            if (docRenderer != null)
            {
                this.rtfWriter = docRenderer.RtfWriter;
            }
            this.useEffectiveValue = false;
        }

        internal static bool CollectionContainsObjectAssignableTo(DocumentObjectCollection coll, params Type[] types)
        {
            foreach (object obj2 in coll)
            {
                foreach (Type type in types)
                {
                    if (type.IsAssignableFrom(obj2.GetType()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void CreateEnumTranslationTable()
        {
            enumTranslationTable = new Hashtable();
            enumTranslationTable.Add(ParagraphAlignment.Left, "l");
            enumTranslationTable.Add(ParagraphAlignment.Right, "r");
            enumTranslationTable.Add(ParagraphAlignment.Center, "c");
            enumTranslationTable.Add(ParagraphAlignment.Justify, "j");
            enumTranslationTable.Add(LineSpacingRule.AtLeast, 0);
            enumTranslationTable.Add(LineSpacingRule.Exactly, 0);
            enumTranslationTable.Add(LineSpacingRule.Double, 1);
            enumTranslationTable.Add(LineSpacingRule.OnePtFive, 1);
            enumTranslationTable.Add(LineSpacingRule.Multiple, 1);
            enumTranslationTable.Add(LineSpacingRule.Single, 1);
            enumTranslationTable.Add(OutlineLevel.Level1, 0);
            enumTranslationTable.Add(OutlineLevel.Level2, 1);
            enumTranslationTable.Add(OutlineLevel.Level3, 2);
            enumTranslationTable.Add(OutlineLevel.Level4, 3);
            enumTranslationTable.Add(OutlineLevel.Level5, 4);
            enumTranslationTable.Add(OutlineLevel.Level6, 5);
            enumTranslationTable.Add(OutlineLevel.Level7, 6);
            enumTranslationTable.Add(OutlineLevel.Level8, 7);
            enumTranslationTable.Add(OutlineLevel.Level9, 8);
            enumTranslationTable.Add(Underline.Dash, "dash");
            enumTranslationTable.Add(Underline.DotDash, "dashd");
            enumTranslationTable.Add(Underline.DotDotDash, "dashdd");
            enumTranslationTable.Add(Underline.Dotted, "d");
            enumTranslationTable.Add(Underline.None, "none");
            enumTranslationTable.Add(Underline.Single, "");
            enumTranslationTable.Add(Underline.Words, "w");
            enumTranslationTable.Add(BorderStyle.DashDot, "dashd");
            enumTranslationTable.Add(BorderStyle.DashDotDot, "dashdd");
            enumTranslationTable.Add(BorderStyle.DashLargeGap, "dash");
            enumTranslationTable.Add(BorderStyle.DashSmallGap, "dashsm");
            enumTranslationTable.Add(BorderStyle.Dot, "dot");
            enumTranslationTable.Add(BorderStyle.Single, "s");
            enumTranslationTable.Add(TabLeader.Dashes, "hyph");
            enumTranslationTable.Add(TabLeader.Dots, "dot");
            enumTranslationTable.Add(TabLeader.Heavy, "th");
            enumTranslationTable.Add(TabLeader.Lines, "ul");
            enumTranslationTable.Add(TabLeader.MiddleDot, "mdot");
            enumTranslationTable.Add(TabAlignment.Center, "c");
            enumTranslationTable.Add(TabAlignment.Decimal, "dec");
            enumTranslationTable.Add(TabAlignment.Right, "r");
            enumTranslationTable.Add(TabAlignment.Left, "l");
            enumTranslationTable.Add(FootnoteNumberStyle.Arabic, "ar");
            enumTranslationTable.Add(FootnoteNumberStyle.LowercaseLetter, "alc");
            enumTranslationTable.Add(FootnoteNumberStyle.LowercaseRoman, "rlc");
            enumTranslationTable.Add(FootnoteNumberStyle.UppercaseLetter, "auc");
            enumTranslationTable.Add(FootnoteNumberStyle.UppercaseRoman, "ruc");
            enumTranslationTable.Add(FootnoteNumberingRule.RestartContinuous, "rstcont");
            enumTranslationTable.Add(FootnoteNumberingRule.RestartPage, "rstpg");
            enumTranslationTable.Add(FootnoteNumberingRule.RestartSection, "restart");
            enumTranslationTable.Add(FootnoteLocation.BeneathText, "tj");
            enumTranslationTable.Add(FootnoteLocation.BottomOfPage, "bj");
            enumTranslationTable.Add(BreakType.BreakEvenPage, "even");
            enumTranslationTable.Add(BreakType.BreakOddPage, "odd");
            enumTranslationTable.Add(BreakType.BreakNextPage, "page");
            enumTranslationTable.Add(ListType.BulletList1, 0x17);
            enumTranslationTable.Add(ListType.BulletList2, 0x17);
            enumTranslationTable.Add(ListType.BulletList3, 0x17);
            enumTranslationTable.Add(ListType.NumberList1, 0);
            enumTranslationTable.Add(ListType.NumberList2, 0);
            enumTranslationTable.Add(ListType.NumberList3, 4);
            enumTranslationTable.Add(RowAlignment.Center, "c");
            enumTranslationTable.Add(RowAlignment.Left, "l");
            enumTranslationTable.Add(RowAlignment.Right, "r");
            enumTranslationTable.Add(VerticalAlignment.Top, "t");
            enumTranslationTable.Add(VerticalAlignment.Center, "c");
            enumTranslationTable.Add(VerticalAlignment.Bottom, "b");
            enumTranslationTable.Add(RelativeHorizontal.Character, "margin");
            enumTranslationTable.Add(RelativeHorizontal.Column, "margin");
            enumTranslationTable.Add(RelativeHorizontal.Margin, "margin");
            enumTranslationTable.Add(RelativeHorizontal.Page, "page");
            enumTranslationTable.Add(RelativeVertical.Line, "para");
            enumTranslationTable.Add(RelativeVertical.Margin, "margin");
            enumTranslationTable.Add(RelativeVertical.Page, "page");
            enumTranslationTable.Add(RelativeVertical.Paragraph, "para");
            enumTranslationTable.Add(WrapStyle.None, 3);
            enumTranslationTable.Add(WrapStyle.Through, 3);
            enumTranslationTable.Add(WrapStyle.TopBottom, 1);
            enumTranslationTable.Add(LineStyle.Single, 0);
            enumTranslationTable.Add(DashStyle.Solid, 0);
            enumTranslationTable.Add(DashStyle.Dash, 1);
            enumTranslationTable.Add(DashStyle.SquareDot, 2);
            enumTranslationTable.Add(DashStyle.DashDot, 3);
            enumTranslationTable.Add(DashStyle.DashDotDot, 4);
            enumTranslationTable.Add(TextOrientation.Downward, 3);
            enumTranslationTable.Add(TextOrientation.Horizontal, 0);
            enumTranslationTable.Add(TextOrientation.HorizontalRotatedFarEast, 0);
            enumTranslationTable.Add(TextOrientation.Upward, 2);
            enumTranslationTable.Add(TextOrientation.Vertical, 3);
            enumTranslationTable.Add(TextOrientation.VerticalFarEast, 3);
        }

        protected virtual object GetValueAsIntended(string valueName) => 
            this.docObject.GetValue(valueName, GV.GetNull);

        protected object GetValueOrDefault(string valName, object valDefault)
        {
            object valueAsIntended = this.GetValueAsIntended(valName);
            if (valueAsIntended == null)
            {
                return valDefault;
            }
            return valueAsIntended;
        }

        internal abstract void Render();
        protected void RenderTrailingParagraph(DocumentElements elements)
        {
            if ((elements == null) || !(elements.LastObject is Paragraph))
            {
                this.rtfWriter.WriteControl("pard");
                this.rtfWriter.WriteControl("s", this.docRenderer.GetStyleIndex("Normal"));
                new ParagraphFormatRenderer(this.docRenderer.Document.Styles["Normal"].ParagraphFormat, this.docRenderer).Render();
                this.rtfWriter.WriteControl("par");
            }
        }

        protected void RenderUnit(string rtfControl, Unit value)
        {
            this.RenderUnit(rtfControl, value, RtfUnit.Twips, false);
        }

        protected void RenderUnit(string rtfControl, Unit value, RtfUnit rtfUnit)
        {
            this.RenderUnit(rtfControl, value, rtfUnit, false);
        }

        protected void RenderUnit(string rtfControl, Unit value, RtfUnit rtfUnit, bool withStar)
        {
            this.rtfWriter.WriteControl(rtfControl, ToRtfUnit(value, rtfUnit), withStar);
        }

        internal static int ToEmu(Unit unit) => 
            ToRtfUnit(unit, RtfUnit.EMU);

        internal static int ToRtfUnit(Unit unit, RtfUnit rtfUnit)
        {
            switch (rtfUnit)
            {
                case RtfUnit.Twips:
                    return (int) Math.Round((double) (unit.Point * 20.0));

                case RtfUnit.HalfPts:
                    return (int) Math.Round((double) (unit.Point * 2.0));

                case RtfUnit.Lines:
                    return (int) Math.Round((double) ((unit.Point * 12.0) * 20.0));

                case RtfUnit.EMU:
                    return (int) Math.Round((double) (unit.Point * 12700.0));

                case RtfUnit.CharUnit100:
                    return (int) Math.Round((double) (unit.Pica * 100.0));
            }
            return (int) unit.Point;
        }

        internal static int ToTwips(Unit unit) => 
            ToRtfUnit(unit, RtfUnit.Twips);

        protected void Translate(string valueName, string rtfCtrl)
        {
            this.Translate(valueName, rtfCtrl, RtfUnit.Twips, (string) null, false);
        }

        protected void Translate(string valueName, string rtfCtrl, RtfUnit unit, Unit val, bool withStar)
        {
            this.Translate(valueName, rtfCtrl, unit, ToRtfUnit(val, RtfUnit.Twips).ToString(CultureInfo.InvariantCulture), withStar);
        }

        protected void Translate(string valueName, string rtfCtrl, RtfUnit unit, string defaultValue, bool withStar)
        {
            object valueAsIntended = this.GetValueAsIntended(valueName);
            if (valueAsIntended == null)
            {
                if (defaultValue != null)
                {
                    this.rtfWriter.WriteControl(rtfCtrl, defaultValue);
                }
            }
            else if (valueAsIntended is Unit)
            {
                this.rtfWriter.WriteControl(rtfCtrl, ToRtfUnit((Unit) valueAsIntended, unit), withStar);
            }
            else if (valueAsIntended is bool)
            {
                if ((bool) valueAsIntended)
                {
                    this.rtfWriter.WriteControl(rtfCtrl, withStar);
                }
            }
            else if (valueAsIntended is Color)
            {
                int colorIndex = this.docRenderer.GetColorIndex((Color) valueAsIntended);
                this.rtfWriter.WriteControl(rtfCtrl, colorIndex, withStar);
            }
            else if (valueAsIntended is Enum)
            {
                this.rtfWriter.WriteControl(rtfCtrl, enumTranslationTable[valueAsIntended].ToString(), withStar);
            }
            else if (valueAsIntended is int)
            {
                this.rtfWriter.WriteControl(rtfCtrl, (int) valueAsIntended, withStar);
            }
        }

        protected void TranslateBool(string valueName, string rtfTrueCtrl, string rtfFalseCtrl, bool withStar)
        {
            object valueAsIntended = this.GetValueAsIntended(valueName);
            if (valueAsIntended != null)
            {
                if ((bool) valueAsIntended)
                {
                    this.rtfWriter.WriteControl(rtfTrueCtrl, withStar);
                }
                else if (rtfFalseCtrl != null)
                {
                    this.rtfWriter.WriteControl(rtfFalseCtrl, withStar);
                }
            }
        }
    }
}

