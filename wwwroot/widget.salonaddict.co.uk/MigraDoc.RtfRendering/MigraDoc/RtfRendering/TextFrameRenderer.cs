namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Tables;
    using MigraDoc.RtfRendering.Resources;
    using System;
    using System.Diagnostics;

    internal class TextFrameRenderer : ShapeRenderer
    {
        private TextFrame textFrame;

        internal TextFrameRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.textFrame = domObj as TextFrame;
        }

        protected override void EndDummyParagraph()
        {
            base.rtfWriter.EndContent();
            base.rtfWriter.EndContent();
            base.EndDummyParagraph();
        }

        protected override Unit GetShapeHeight()
        {
            if (this.textFrame.IsNull("Height"))
            {
                return Unit.FromInch(1.0);
            }
            return base.GetShapeHeight();
        }

        protected override Unit GetShapeWidth()
        {
            if (this.textFrame.IsNull("Width"))
            {
                return Unit.FromInch(1.0);
            }
            return base.GetShapeWidth();
        }

        internal override void Render()
        {
            DocumentRelations.GetParent(this.textFrame);
            bool flag = this.RenderInParagraph();
            if (flag)
            {
                this.StartDummyParagraph();
            }
            this.StartShapeArea();
            base.RenderNameValuePair("shapeType", "202");
            base.TranslateAsNameValuePair("MarginLeft", "dxTextLeft", RtfUnit.EMU, "0");
            base.TranslateAsNameValuePair("MarginTop", "dyTextTop", RtfUnit.EMU, "0");
            base.TranslateAsNameValuePair("MarginRight", "dxTextRight", RtfUnit.EMU, "0");
            base.TranslateAsNameValuePair("MarginBottom", "dyTextBottom", RtfUnit.EMU, "0");
            if (this.textFrame.IsNull("Elements") || !RendererBase.CollectionContainsObjectAssignableTo(this.textFrame.Elements, new Type[] { typeof(Shape), typeof(Table) }))
            {
                base.TranslateAsNameValuePair("Orientation", "txflTextFlow", RtfUnit.Undefined, null);
            }
            else
            {
                TextOrientation orientation = this.textFrame.Orientation;
                if ((orientation != TextOrientation.Horizontal) && (orientation != TextOrientation.HorizontalRotatedFarEast))
                {
                    Trace.WriteLine(Messages.TextframeContentsNotTurned, "warning");
                }
            }
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("shptxt");
            base.rtfWriter.StartContent();
            foreach (DocumentObject obj2 in this.textFrame.Elements)
            {
                RendererBase base2 = RendererFactory.CreateRenderer(obj2, base.docRenderer);
                if (base2 != null)
                {
                    base2.Render();
                }
            }
            base.RenderTrailingParagraph(this.textFrame.Elements);
            base.rtfWriter.EndContent();
            base.rtfWriter.EndContent();
            this.EndShapeArea();
            if (flag)
            {
                this.RenderLayoutPicture();
                this.EndDummyParagraph();
            }
        }

        private void RenderLayoutPicture()
        {
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("pict");
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControlWithStar("picprop");
            base.rtfWriter.WriteControl("defshp");
            base.RenderNameValuePair("shapeType", "75");
            base.RenderNameValuePair("fPseudoInline", "1");
            base.RenderNameValuePair("fLockPosition", "1");
            base.RenderNameValuePair("fLockRotation", "1");
            base.rtfWriter.EndContent();
            base.rtfWriter.WriteControl("pich", (int) (this.GetShapeHeight().Millimeter * 100.0));
            base.rtfWriter.WriteControl("picw", (int) (this.GetShapeWidth().Millimeter * 100.0));
            base.RenderUnit("pichgoal", this.GetShapeHeight());
            base.RenderUnit("picwgoal", this.GetShapeWidth());
            base.rtfWriter.WriteControl("wmetafile", 8);
            base.rtfWriter.WriteControl("blipupi", 600);
            base.rtfWriter.EndContent();
        }

        protected override void StartDummyParagraph()
        {
            base.StartDummyParagraph();
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("field");
            base.rtfWriter.WriteControl("fldedit");
            base.rtfWriter.WriteControl("fldlock");
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("fldinst", true);
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteText("SHAPE");
            base.rtfWriter.WriteText(@" \*MERGEFORMAT");
            base.rtfWriter.EndContent();
            base.rtfWriter.EndContent();
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("fldrslt");
        }
    }
}

