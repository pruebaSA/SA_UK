namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Tables;
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal abstract class ShapeRenderer : RendererBase
    {
        private Shape shape;

        internal ShapeRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.shape = domObj as Shape;
        }

        private void AlignHorizontally(ShapePosition shpPos, Unit distanceLeftRight, out Unit leftValue, out Unit rightValue)
        {
            double point = this.GetShapeWidth().Point;
            leftValue = 0;
            rightValue = point;
            Unit valueOrDefault = (Unit) base.GetValueOrDefault("WrapFormat.DistanceLeft", (Unit) 0);
            Unit unit2 = (Unit) base.GetValueOrDefault("WrapFormat.DistanceRight", (Unit) 0);
            switch (shpPos)
            {
                case ShapePosition.Left:
                case ShapePosition.Inside:
                    leftValue = valueOrDefault;
                    rightValue = ((double) valueOrDefault) + point;
                    break;

                case ShapePosition.Right:
                case ShapePosition.Outside:
                    leftValue = (distanceLeftRight.Point - point) - ((double) unit2);
                    rightValue = ((float) distanceLeftRight) - ((float) unit2);
                    return;

                case ShapePosition.Center:
                {
                    double num2 = distanceLeftRight.Point / 2.0;
                    leftValue = num2 - (point / 2.0);
                    rightValue = num2 + (point / 2.0);
                    return;
                }
                case ShapePosition.Top:
                case ShapePosition.Bottom:
                    break;

                default:
                    return;
            }
        }

        private void AlignVertically(ShapePosition shpPos, Unit distanceTopBottom, out Unit topValue, out Unit bottomValue)
        {
            double point = this.GetShapeHeight().Point;
            topValue = 0;
            bottomValue = point;
            Unit valueOrDefault = (Unit) base.GetValueOrDefault("WrapFormat.DistanceTop", (Unit) 0);
            Unit unit2 = (Unit) base.GetValueOrDefault("WrapFormat.DistanceBottom", (Unit) 0);
            switch (shpPos)
            {
                case ShapePosition.Center:
                {
                    Unit unit3 = ((double) distanceTopBottom) / 2.0;
                    topValue = ((double) unit3) - (point / 2.0);
                    bottomValue = ((double) unit3) + (point / 2.0);
                    return;
                }
                case ShapePosition.Top:
                    topValue = valueOrDefault;
                    bottomValue = ((double) valueOrDefault) + point;
                    return;

                case ShapePosition.Bottom:
                    topValue = (((double) distanceTopBottom) - point) - ((double) unit2);
                    bottomValue = ((float) distanceTopBottom) - ((float) unit2);
                    return;
            }
        }

        protected virtual void EndDummyParagraph()
        {
            base.rtfWriter.WriteControl("par");
        }

        protected void EndNameValuePair()
        {
            base.rtfWriter.EndContent();
            base.rtfWriter.EndContent();
        }

        protected virtual void EndShapeArea()
        {
            base.rtfWriter.EndContent();
            base.rtfWriter.EndContent();
        }

        protected Unit GetLineWidth()
        {
            LineFormat valueAsIntended = this.GetValueAsIntended("LineFormat") as LineFormat;
            if ((valueAsIntended == null) || (!valueAsIntended.IsNull("Visible") && !valueAsIntended.Visible))
            {
                return 0;
            }
            if (valueAsIntended.IsNull("Width"))
            {
                return 1;
            }
            return valueAsIntended.Width;
        }

        protected virtual Unit GetShapeHeight() => 
            this.shape.Height;

        protected virtual Unit GetShapeWidth() => 
            this.shape.Width;

        protected void RenderFillFormat()
        {
            FillFormat valueAsIntended = this.GetValueAsIntended("FillFormat") as FillFormat;
            if ((valueAsIntended != null) && (valueAsIntended.IsNull("Visible") || valueAsIntended.Visible))
            {
                this.RenderNameValuePair("fFilled", "1");
                this.TranslateAsNameValuePair("FillFormat.Color", "fillColor", RtfUnit.Undefined, null);
            }
            else
            {
                this.RenderNameValuePair("fFilled", "0");
            }
        }

        protected virtual bool RenderInParagraph()
        {
            if ((!this.shape.IsNull("RelativeVertical") && (this.shape.RelativeVertical != RelativeVertical.Line)) && (this.shape.RelativeVertical != RelativeVertical.Paragraph))
            {
                return false;
            }
            DocumentObjectCollection parent = DocumentRelations.GetParent(this.shape) as DocumentObjectCollection;
            if (DocumentRelations.GetParent(parent) is Paragraph)
            {
                return false;
            }
            if (!this.shape.IsNull("WrapFormat.Style"))
            {
                return (this.shape.WrapFormat.Style == WrapStyle.TopBottom);
            }
            return true;
        }

        protected void RenderLeftPosition()
        {
            this.RenderLeftRight();
            ShapePosition shapePosition = this.shape.Left.ShapePosition;
            string str = "";
            switch (shapePosition)
            {
                case ShapePosition.Left:
                {
                    object valueAsIntended = this.GetValueAsIntended("WrapFormat.DistanceLeft");
                    if (valueAsIntended != null)
                    {
                        Unit unit = (Unit) valueAsIntended;
                        if (unit.Point != 0.0)
                        {
                            break;
                        }
                    }
                    str = "1";
                    break;
                }
                case ShapePosition.Right:
                {
                    object obj3 = this.GetValueAsIntended("WrapFormat.DistanceRight");
                    if (obj3 != null)
                    {
                        Unit unit2 = (Unit) obj3;
                        if (unit2.Point != 0.0)
                        {
                            break;
                        }
                    }
                    str = "3";
                    break;
                }
                case ShapePosition.Center:
                    str = "2";
                    break;
            }
            if ((str != "") && !this.RenderInParagraph())
            {
                this.RenderNameValuePair("posh", str);
            }
        }

        private void RenderLeftRight()
        {
            Unit shapeWidth = this.GetShapeWidth();
            Unit leftValue = 0;
            Unit rightValue = shapeWidth;
            if (!this.RenderInParagraph())
            {
                RelativeHorizontal valueOrDefault = (RelativeHorizontal) base.GetValueOrDefault("RelativeHorizontal", RelativeHorizontal.Margin);
                LeftPosition position = (LeftPosition) base.GetValueOrDefault("Left", new LeftPosition());
                Section parentOfType = (Section) DocumentRelations.GetParentOfType(this.shape, typeof(Section));
                PageSetup pageSetup = parentOfType.PageSetup;
                Unit unit4 = (Unit) pageSetup.GetValue("LeftMargin", GV.ReadOnly);
                Unit unit5 = (Unit) pageSetup.GetValue("RightMargin", GV.ReadOnly);
                Unit pageHeight = pageSetup.PageHeight;
                Unit pageWidth = pageSetup.PageWidth;
                if (position.ShapePosition == ShapePosition.Undefined)
                {
                    leftValue = position.Position;
                    rightValue = ((float) leftValue) + ((float) shapeWidth);
                }
                else
                {
                    switch (valueOrDefault)
                    {
                        case RelativeHorizontal.Character:
                        case RelativeHorizontal.Column:
                        case RelativeHorizontal.Margin:
                            this.AlignHorizontally(position.ShapePosition, (pageWidth.Point - unit4.Point) - unit5.Point, out leftValue, out rightValue);
                            break;

                        case RelativeHorizontal.Page:
                            this.AlignHorizontally(position.ShapePosition, pageWidth, out leftValue, out rightValue);
                            break;
                    }
                }
            }
            base.RenderUnit("shpleft", leftValue);
            base.RenderUnit("shpright", rightValue);
        }

        protected void RenderLineFormat()
        {
            LineFormat valueAsIntended = this.GetValueAsIntended("LineFormat") as LineFormat;
            if ((valueAsIntended != null) && (valueAsIntended.IsNull("Visible") || valueAsIntended.Visible))
            {
                this.RenderNameValuePair("fLine", "1");
                this.TranslateAsNameValuePair("LineFormat.Color", "lineColor", RtfUnit.Undefined, "0");
                this.TranslateAsNameValuePair("LineFormat.Width", "lineWidth", RtfUnit.EMU, RendererBase.ToEmu(1).ToString(CultureInfo.InvariantCulture));
                this.TranslateAsNameValuePair("LineFormat.DashStyle", "lineDashing", RtfUnit.Undefined, "0");
            }
            else
            {
                this.RenderNameValuePair("fLine", "0");
            }
        }

        protected void RenderNameValuePair(string name, string value)
        {
            this.StartNameValuePair(name);
            base.rtfWriter.WriteText(value);
            this.EndNameValuePair();
        }

        private void RenderParagraphAlignment()
        {
            if (!this.shape.IsNull("Left"))
            {
                LeftPosition shapePosition = this.shape.Left.ShapePosition;
                switch (shapePosition.ShapePosition)
                {
                    case ShapePosition.Right:
                        base.rtfWriter.WriteControl("q", "r");
                        return;

                    case ShapePosition.Center:
                        base.rtfWriter.WriteControl("q", "c");
                        return;

                    case ShapePosition.Undefined:
                        return;
                }
                base.rtfWriter.WriteControl("q", "l");
            }
        }

        protected virtual void RenderParagraphAttributes()
        {
            if (DocumentRelations.HasParentOfType(this.shape, typeof(Cell)))
            {
                base.rtfWriter.WriteControl("intbl");
            }
            this.RenderParagraphAlignment();
            this.RenderParagraphIndents();
            this.RenderParagraphDistances();
        }

        private void RenderParagraphDistances()
        {
            Unit unit = 0;
            Unit unit2 = 0;
            TopPosition valueOrDefault = (TopPosition) base.GetValueOrDefault("Top", new TopPosition());
            if (valueOrDefault.ShapePosition == ShapePosition.Undefined)
            {
                unit2 = ((float) valueOrDefault.Position) + ((float) ((Unit) base.GetValueOrDefault("WrapFormat.DistanceTop", (Unit) 0)));
            }
            unit = (Unit) base.GetValueOrDefault("WrapFormat.DistanceBottom", (Unit) 0);
            base.RenderUnit("sa", unit);
            base.RenderUnit("sb", unit2);
        }

        private void RenderParagraphIndents()
        {
            object valueAsIntended = this.GetValueAsIntended("RelativeHorizontal");
            double num = 0.0;
            double num2 = 0.0;
            if ((valueAsIntended != null) && (((RelativeHorizontal) valueAsIntended) == RelativeHorizontal.Page))
            {
                Section parentOfType = (Section) DocumentRelations.GetParentOfType(this.shape, typeof(Section));
                Unit unit = (Unit) parentOfType.PageSetup.GetValue("LeftMargin", GV.ReadOnly);
                num = -unit.Point;
                Unit unit2 = (Unit) parentOfType.PageSetup.GetValue("RightMargin", GV.ReadOnly);
                num2 = -((float) unit2);
            }
            LeftPosition valueOrDefault = (LeftPosition) base.GetValueOrDefault("Left", new LeftPosition());
            switch (valueOrDefault.ShapePosition)
            {
                case ShapePosition.Undefined:
                {
                    num += (double) valueOrDefault.Position;
                    Unit unit3 = (Unit) base.GetValueOrDefault("WrapFormat.DistanceLeft", (Unit) 0);
                    num += unit3.Point;
                    break;
                }
                case ShapePosition.Left:
                {
                    Unit unit4 = (Unit) base.GetValueOrDefault("WrapFormat.DistanceLeft", (Unit) 0);
                    num += unit4.Point;
                    break;
                }
                case ShapePosition.Right:
                {
                    Unit unit5 = (Unit) base.GetValueOrDefault("WrapFormat.DistanceRight", (Unit) 0);
                    num2 += unit5.Point;
                    break;
                }
            }
            base.RenderUnit("li", num);
            base.RenderUnit("lin", num);
            base.RenderUnit("ri", num2);
            base.RenderUnit("rin", num2);
        }

        private void RenderRelativeHorizontal()
        {
            if (this.RenderInParagraph())
            {
                base.rtfWriter.WriteControl("shpbx", "para");
                base.rtfWriter.WriteControl("shpbx", "ignore");
                this.RenderNameValuePair("posrelh", "3");
            }
            else
            {
                base.Translate("RelativeHorizontal", "shpbx", RtfUnit.Undefined, "margin", false);
                base.rtfWriter.WriteControl("shpbx", "ignore");
                object valueAsIntended = this.GetValueAsIntended("RelativeHorizontal");
                switch (((valueAsIntended == null) ? RelativeHorizontal.Margin : ((RelativeHorizontal) valueAsIntended)))
                {
                    case RelativeHorizontal.Character:
                        this.RenderNameValuePair("posrelh", "3");
                        return;

                    case RelativeHorizontal.Column:
                        this.RenderNameValuePair("posrelh", "2");
                        return;

                    case RelativeHorizontal.Margin:
                        this.RenderNameValuePair("posrelh", "0");
                        return;

                    case RelativeHorizontal.Page:
                        this.RenderNameValuePair("posrelh", "1");
                        return;
                }
            }
        }

        private void RenderRelativeVertical()
        {
            if (this.RenderInParagraph())
            {
                base.rtfWriter.WriteControl("shpby", "para");
                base.rtfWriter.WriteControl("shpby", "ignore");
                this.RenderNameValuePair("posrelv", "3");
            }
            else
            {
                base.Translate("RelativeVertical", "shpby", RtfUnit.Undefined, "para", false);
                base.rtfWriter.WriteControl("shpby", "ignore");
                object valueAsIntended = this.GetValueAsIntended("RelativeVertical");
                switch (((valueAsIntended == null) ? RelativeVertical.Paragraph : ((RelativeVertical) valueAsIntended)))
                {
                    case RelativeVertical.Line:
                        this.RenderNameValuePair("posrelv", "3");
                        return;

                    case RelativeVertical.Margin:
                        this.RenderNameValuePair("posrelv", "0");
                        return;

                    case RelativeVertical.Page:
                        this.RenderNameValuePair("posrelv", "1");
                        return;

                    case RelativeVertical.Paragraph:
                        this.RenderNameValuePair("posrelv", "2");
                        return;
                }
            }
        }

        private void RenderShapeAttributes()
        {
            this.RenderTopPosition();
            this.RenderLeftPosition();
            if (DocumentRelations.HasParentOfType(this.shape, typeof(HeaderFooter)))
            {
                base.rtfWriter.WriteControl("shpfhdr", "1");
            }
            else
            {
                base.rtfWriter.WriteControl("shpfhdr", "0");
            }
            this.RenderWrapFormat();
            this.RenderRelativeHorizontal();
            this.RenderRelativeVertical();
            if (this.RenderInParagraph())
            {
                base.rtfWriter.WriteControl("shplockanchor");
                this.RenderNameValuePair("fPseudoInline", "1");
            }
            this.RenderLineFormat();
            this.RenderFillFormat();
        }

        private void RenderTopBottom()
        {
            Unit shapeHeight = this.GetShapeHeight();
            Unit topValue = 0;
            Unit bottomValue = shapeHeight;
            if (!this.RenderInParagraph())
            {
                RelativeVertical valueOrDefault = (RelativeVertical) base.GetValueOrDefault("RelativeVertical", RelativeVertical.Paragraph);
                TopPosition position = (TopPosition) base.GetValueOrDefault("Top", new TopPosition());
                Section parentOfType = (Section) DocumentRelations.GetParentOfType(this.shape, typeof(Section));
                PageSetup pageSetup = parentOfType.PageSetup;
                Unit unit4 = (Unit) pageSetup.GetValue("TopMargin", GV.ReadOnly);
                Unit unit5 = (Unit) pageSetup.GetValue("BottomMargin", GV.ReadOnly);
                Unit pageHeight = pageSetup.PageHeight;
                Unit pageWidth = pageSetup.PageWidth;
                if (position.ShapePosition == ShapePosition.Undefined)
                {
                    topValue = position.Position;
                    bottomValue = ((float) topValue) + ((float) shapeHeight);
                }
                else
                {
                    switch (valueOrDefault)
                    {
                        case RelativeVertical.Line:
                            this.AlignVertically(position.ShapePosition, shapeHeight, out topValue, out bottomValue);
                            break;

                        case RelativeVertical.Margin:
                            this.AlignVertically(position.ShapePosition, (pageHeight.Point - unit4.Point) - unit5.Point, out topValue, out bottomValue);
                            break;

                        case RelativeVertical.Page:
                            this.AlignVertically(position.ShapePosition, pageHeight, out topValue, out bottomValue);
                            break;
                    }
                }
            }
            base.RenderUnit("shptop", topValue);
            base.RenderUnit("shpbottom", bottomValue);
        }

        protected void RenderTopPosition()
        {
            this.RenderTopBottom();
            string str = "";
            switch (this.shape.Top.ShapePosition)
            {
                case ShapePosition.Center:
                    str = "2";
                    break;

                case ShapePosition.Top:
                {
                    object valueAsIntended = this.GetValueAsIntended("WrapFormat.DistanceTop");
                    if (valueAsIntended != null)
                    {
                        Unit unit = (Unit) valueAsIntended;
                        if (unit.Point != 0.0)
                        {
                            break;
                        }
                    }
                    str = "1";
                    break;
                }
                case ShapePosition.Bottom:
                {
                    object obj3 = this.GetValueAsIntended("WrapFormat.DistanceBottom");
                    if (obj3 != null)
                    {
                        Unit unit2 = (Unit) obj3;
                        if (unit2.Point != 0.0)
                        {
                            break;
                        }
                    }
                    str = "3";
                    break;
                }
            }
            if ((str != "") && !this.RenderInParagraph())
            {
                this.RenderNameValuePair("posv", str);
            }
        }

        private void RenderWrapFormat()
        {
            if (!this.RenderInParagraph())
            {
                base.Translate("WrapFormat.Style", "shpwr", RtfUnit.Undefined, "3", false);
            }
            else
            {
                base.rtfWriter.WriteControl("shpwrk", "0");
                base.rtfWriter.WriteControl("shpwr", "3");
            }
        }

        protected virtual void StartDummyParagraph()
        {
            base.rtfWriter.WriteControl("pard");
            this.RenderParagraphAttributes();
        }

        protected void StartNameValuePair(string name)
        {
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("sp");
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("sn");
            base.rtfWriter.WriteText(name);
            base.rtfWriter.EndContent();
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("sv");
        }

        protected virtual void StartShapeArea()
        {
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("shp");
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControlWithStar("shpinst");
            this.RenderShapeAttributes();
        }

        protected void TranslateAsNameValuePair(string domValueName, string rtfName, RtfUnit unit, string defaultValue)
        {
            object valueAsIntended = this.GetValueAsIntended(domValueName);
            if ((valueAsIntended != null) || (defaultValue != null))
            {
                string str = "";
                if (valueAsIntended == null)
                {
                    str = defaultValue;
                }
                else if (valueAsIntended is Unit)
                {
                    str = RendererBase.ToRtfUnit((Unit) valueAsIntended, unit).ToString(CultureInfo.InvariantCulture);
                }
                else if (valueAsIntended is Color)
                {
                    Color mixedTransparencyColor = (Color) valueAsIntended;
                    mixedTransparencyColor = mixedTransparencyColor.GetMixedTransparencyColor();
                    str = ((mixedTransparencyColor.R + (mixedTransparencyColor.G * 0x100)) + (mixedTransparencyColor.B * 0x10000)).ToString(CultureInfo.InvariantCulture);
                }
                else if (valueAsIntended is Enum)
                {
                    str = RendererBase.enumTranslationTable[valueAsIntended].ToString();
                }
                else if (valueAsIntended is bool)
                {
                    str = ((bool) valueAsIntended) ? "1" : "0";
                }
                this.RenderNameValuePair(rtfName, str);
            }
        }
    }
}

