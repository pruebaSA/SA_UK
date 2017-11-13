namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using PdfSharp.Drawing;
    using System;
    using System.Collections;

    internal class TopDownFormatter
    {
        private IAreaProvider areaProvider;
        private DocumentRenderer documentRenderer;
        private DocumentElements elements;
        private XGraphics gfx;
        internal static readonly int MaxCombineElements = 10;

        internal TopDownFormatter(IAreaProvider areaProvider, DocumentRenderer documentRenderer, DocumentElements elements)
        {
            this.documentRenderer = documentRenderer;
            this.areaProvider = areaProvider;
            this.elements = elements;
        }

        private RenderInfo FinishPage(RenderInfo lastRenderInfo, bool pagebreakBefore, ref ArrayList renderInfos)
        {
            RenderInfo info;
            if (lastRenderInfo.FormatInfo.IsEmpty || pagebreakBefore)
            {
                info = null;
            }
            else
            {
                info = lastRenderInfo;
                renderInfos.Add(lastRenderInfo);
                if (lastRenderInfo.FormatInfo.IsEnding)
                {
                    info = null;
                }
            }
            this.areaProvider.StoreRenderInfos(renderInfos);
            renderInfos = new ArrayList();
            return info;
        }

        public void FormatOnAreas(XGraphics gfx, bool topLevel)
        {
            this.gfx = gfx;
            XUnit marginBottom = 0;
            RenderInfo prevRenderInfo = null;
            FormatInfo previousFormatInfo = null;
            ArrayList renderInfos = new ArrayList();
            bool flag = this.elements.Count == 0;
            bool isFirstOnPage = true;
            Area nextArea = this.areaProvider.GetNextArea();
            XUnit height = nextArea.Height;
            if (flag)
            {
                this.areaProvider.StoreRenderInfos(renderInfos);
            }
            else
            {
                int idx = 0;
                while (!flag && (nextArea != null))
                {
                    DocumentObject documentObject = this.elements[idx];
                    Renderer renderer = Renderer.Create(gfx, this.documentRenderer, documentObject, this.areaProvider.AreaFieldInfos);
                    if (renderer != null)
                    {
                        renderer.MaxElementHeight = height;
                    }
                    if (topLevel && this.documentRenderer.HasPrepareDocumentProgress)
                    {
                        this.documentRenderer.OnPrepareDocumentProgress((this.documentRenderer.ProgressCompleted + idx) + 1, this.documentRenderer.ProgressMaximum);
                    }
                    if (renderer == null)
                    {
                        flag = idx == (this.elements.Count - 1);
                        if (flag)
                        {
                            this.areaProvider.StoreRenderInfos(renderInfos);
                        }
                        idx++;
                    }
                    else
                    {
                        if (previousFormatInfo == null)
                        {
                            LayoutInfo initialLayoutInfo = renderer.InitialLayoutInfo;
                            XUnit nextTopMargin = marginBottom;
                            if ((initialLayoutInfo.VerticalReference == VerticalReference.PreviousElement) && (initialLayoutInfo.Floating != Floating.None))
                            {
                                nextTopMargin = this.MarginMax(initialLayoutInfo.MarginTop, nextTopMargin);
                            }
                            nextArea = nextArea.Lower(nextTopMargin);
                        }
                        renderer.Format(nextArea, previousFormatInfo);
                        this.areaProvider.PositionHorizontally(renderer.RenderInfo.LayoutInfo);
                        bool pagebreakBefore = (this.areaProvider.IsAreaBreakBefore(renderer.RenderInfo.LayoutInfo) && !isFirstOnPage) || (!isFirstOnPage && this.IsForcedAreaBreak(idx, renderer, nextArea));
                        if (!pagebreakBefore && renderer.RenderInfo.FormatInfo.IsEnding)
                        {
                            if (this.PreviousRendererNeedsRemoveEnding(prevRenderInfo, renderer.RenderInfo, nextArea))
                            {
                                prevRenderInfo.RemoveEnding();
                                renderer = Renderer.Create(gfx, this.documentRenderer, documentObject, this.areaProvider.AreaFieldInfos);
                                renderer.MaxElementHeight = height;
                                renderer.Format(nextArea, prevRenderInfo.FormatInfo);
                            }
                            else if (this.NeedsEndingOnNextArea(idx, renderer, nextArea, isFirstOnPage))
                            {
                                renderer.RenderInfo.RemoveEnding();
                                prevRenderInfo = this.FinishPage(renderer.RenderInfo, pagebreakBefore, ref renderInfos);
                                if (prevRenderInfo != null)
                                {
                                    previousFormatInfo = prevRenderInfo.FormatInfo;
                                }
                                else
                                {
                                    previousFormatInfo = null;
                                    isFirstOnPage = true;
                                }
                                marginBottom = 0;
                                nextArea = this.areaProvider.GetNextArea();
                                height = nextArea.Height;
                            }
                            else
                            {
                                renderInfos.Add(renderer.RenderInfo);
                                isFirstOnPage = false;
                                this.areaProvider.PositionVertically(renderer.RenderInfo.LayoutInfo);
                                if ((renderer.RenderInfo.LayoutInfo.VerticalReference == VerticalReference.PreviousElement) && (renderer.RenderInfo.LayoutInfo.Floating != Floating.None))
                                {
                                    marginBottom = renderer.RenderInfo.LayoutInfo.MarginBottom;
                                    if (renderer.RenderInfo.LayoutInfo.Floating != Floating.None)
                                    {
                                        nextArea = nextArea.Lower(renderer.RenderInfo.LayoutInfo.ContentArea.Height);
                                    }
                                }
                                else
                                {
                                    marginBottom = 0;
                                }
                                previousFormatInfo = null;
                                prevRenderInfo = null;
                                idx++;
                            }
                        }
                        else
                        {
                            if (renderer.RenderInfo.FormatInfo.IsEmpty && isFirstOnPage)
                            {
                                nextArea = nextArea.Unite(new Rectangle(nextArea.X, nextArea.Y, nextArea.Width, 1.7976931348623157E+308));
                                renderer = Renderer.Create(gfx, this.documentRenderer, documentObject, this.areaProvider.AreaFieldInfos);
                                renderer.MaxElementHeight = height;
                                renderer.Format(nextArea, previousFormatInfo);
                                previousFormatInfo = null;
                                this.areaProvider.PositionHorizontally(renderer.RenderInfo.LayoutInfo);
                                this.areaProvider.PositionVertically(renderer.RenderInfo.LayoutInfo);
                                flag = idx == (this.elements.Count - 1);
                                idx++;
                            }
                            prevRenderInfo = this.FinishPage(renderer.RenderInfo, pagebreakBefore, ref renderInfos);
                            if (prevRenderInfo != null)
                            {
                                previousFormatInfo = prevRenderInfo.FormatInfo;
                            }
                            else
                            {
                                previousFormatInfo = null;
                            }
                            isFirstOnPage = true;
                            marginBottom = 0;
                            if (!flag)
                            {
                                nextArea = this.areaProvider.GetNextArea();
                                height = nextArea.Height;
                            }
                        }
                        if ((idx == this.elements.Count) && !flag)
                        {
                            this.areaProvider.StoreRenderInfos(renderInfos);
                            flag = true;
                        }
                    }
                }
            }
        }

        private bool IsForcedAreaBreak(int idx, Renderer renderer, Area remainingArea)
        {
            FormatInfo formatInfo = renderer.RenderInfo.FormatInfo;
            LayoutInfo layoutInfo = renderer.RenderInfo.LayoutInfo;
            if (formatInfo.IsStarting && !formatInfo.StartingIsComplete)
            {
                return true;
            }
            if (layoutInfo.KeepTogether && !formatInfo.IsComplete)
            {
                return true;
            }
            if (layoutInfo.KeepTogether && layoutInfo.KeepWithNext)
            {
                Area area = remainingArea.Lower(layoutInfo.ContentArea.Height);
                return this.NextElementsDontFit(idx, area, layoutInfo.MarginBottom);
            }
            return false;
        }

        private XUnit MarginMax(XUnit prevBottomMargin, XUnit nextTopMargin)
        {
            if ((((double) prevBottomMargin) >= 0.0) && (((double) nextTopMargin) >= 0.0))
            {
                return Math.Max((double) prevBottomMargin, (double) nextTopMargin);
            }
            return (((double) prevBottomMargin) + ((double) nextTopMargin));
        }

        private bool NeedsEndingOnNextArea(int idx, Renderer renderer, Area remainingArea, bool isFirstOnPage)
        {
            LayoutInfo layoutInfo = renderer.RenderInfo.LayoutInfo;
            if (!isFirstOnPage || !layoutInfo.KeepTogether)
            {
                if (!renderer.RenderInfo.FormatInfo.EndingIsComplete)
                {
                    return false;
                }
                if (layoutInfo.KeepWithNext)
                {
                    remainingArea = remainingArea.Lower(layoutInfo.ContentArea.Height);
                    return this.NextElementsDontFit(idx, remainingArea, layoutInfo.MarginBottom);
                }
            }
            return false;
        }

        private bool NextElementsDontFit(int idx, Area remainingArea, XUnit previousMarginBottom)
        {
            XUnit prevBottomMargin = previousMarginBottom;
            Area area = remainingArea;
            for (int i = idx + 1; i < this.elements.Count; i++)
            {
                if ((i - idx) > MaxCombineElements)
                {
                    return false;
                }
                DocumentObject documentObject = this.elements[i];
                Renderer renderer = Renderer.Create(this.gfx, this.documentRenderer, documentObject, this.areaProvider.AreaFieldInfos);
                prevBottomMargin = this.MarginMax(prevBottomMargin, renderer.InitialLayoutInfo.MarginTop);
                area = area.Lower(prevBottomMargin);
                if (((double) area.Height) <= 0.0)
                {
                    return true;
                }
                renderer.Format(area, null);
                FormatInfo formatInfo = renderer.RenderInfo.FormatInfo;
                LayoutInfo layoutInfo = renderer.RenderInfo.LayoutInfo;
                if (layoutInfo.VerticalReference != VerticalReference.PreviousElement)
                {
                    return false;
                }
                if (!formatInfo.StartingIsComplete)
                {
                    return true;
                }
                if (layoutInfo.KeepTogether && !formatInfo.IsComplete)
                {
                    return true;
                }
                if (!layoutInfo.KeepTogether || !layoutInfo.KeepWithNext)
                {
                    return false;
                }
                area = area.Lower(layoutInfo.ContentArea.Height);
                if (((double) area.Height) <= 0.0)
                {
                    return true;
                }
                prevBottomMargin = layoutInfo.MarginBottom;
            }
            return false;
        }

        private bool PreviousRendererNeedsRemoveEnding(RenderInfo prevRenderInfo, RenderInfo succedingRenderInfo, Area remainingArea)
        {
            if (prevRenderInfo == null)
            {
                return false;
            }
            LayoutInfo layoutInfo = succedingRenderInfo.LayoutInfo;
            FormatInfo formatInfo = succedingRenderInfo.FormatInfo;
            LayoutInfo info3 = prevRenderInfo.LayoutInfo;
            return ((formatInfo.IsEnding && !formatInfo.EndingIsComplete) && (((double) this.areaProvider.ProbeNextArea().Height) > ((((double) info3.TrailingHeight) + ((double) layoutInfo.TrailingHeight)) + ((double) Renderer.Tolerance))));
        }
    }
}

