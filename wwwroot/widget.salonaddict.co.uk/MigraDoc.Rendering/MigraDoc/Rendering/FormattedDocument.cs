namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using PdfSharp;
    using PdfSharp.Drawing;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class FormattedDocument : IAreaProvider
    {
        private Dictionary<string, FieldInfos.BookmarkInfo> bookmarks;
        private FieldInfos currentFieldInfos;
        private int currentPage;
        private Section currentSection;
        private Document document;
        private DocumentRenderer documentRenderer;
        private Dictionary<int, object> emptyPages = new Dictionary<int, object>();
        private Dictionary<HeaderFooterPosition, FormattedHeaderFooter> formattedFooters;
        private Dictionary<HeaderFooterPosition, FormattedHeaderFooter> formattedHeaders;
        private XGraphics gfx;
        private bool isNewSection;
        private int pageCount;
        private Dictionary<int, FieldInfos> pageFieldInfos;
        private Dictionary<int, PageInfo> pageInfos;
        private Dictionary<int, ArrayList> pageRenderInfos;
        private int sectionNumber;
        private int sectionPages;
        private int shownPageNumber;

        internal FormattedDocument(Document document, DocumentRenderer documentRenderer)
        {
            this.document = document;
            this.documentRenderer = documentRenderer;
        }

        private Rectangle CalcContentRect(int page)
        {
            XUnit unit;
            XUnit unit2;
            XUnit unit3;
            PageSetup pageSetup = this.currentSection.PageSetup;
            if (pageSetup.Orientation == Orientation.Portrait)
            {
                unit = pageSetup.PageWidth.Point;
            }
            else
            {
                unit = pageSetup.PageHeight.Point;
            }
            unit = ((double) unit) - pageSetup.RightMargin.Point;
            unit = ((double) unit) - pageSetup.LeftMargin.Point;
            if (pageSetup.Orientation == Orientation.Portrait)
            {
                unit2 = pageSetup.PageHeight.Point;
            }
            else
            {
                unit2 = pageSetup.PageWidth.Point;
            }
            unit2 = ((double) unit2) - pageSetup.TopMargin.Point;
            unit2 = ((double) unit2) - pageSetup.BottomMargin.Point;
            XUnit point = pageSetup.TopMargin.Point;
            if (pageSetup.MirrorMargins)
            {
                unit3 = ((page % 2) == 0) ? pageSetup.RightMargin.Point : pageSetup.LeftMargin.Point;
            }
            else
            {
                unit3 = pageSetup.LeftMargin.Point;
            }
            return new Rectangle(unit3, point, unit, unit2);
        }

        private PageOrientation CalcPageOrientation(PageSetup pageSetup)
        {
            PageOrientation portrait = PageOrientation.Portrait;
            if (this.currentSection.PageSetup.Orientation == Orientation.Landscape)
            {
                portrait = PageOrientation.Landscape;
            }
            return portrait;
        }

        private XSize CalcPageSize(PageSetup pageSetup) => 
            new XSize(pageSetup.PageWidth.Point, pageSetup.PageHeight.Point);

        private HeaderFooter ChooseHeaderFooter(HeadersFooters hfs, PagePosition pagePos)
        {
            if (hfs == null)
            {
                return null;
            }
            PageSetup pageSetup = this.currentSection.PageSetup;
            if ((pagePos == PagePosition.First) && pageSetup.DifferentFirstPageHeaderFooter)
            {
                return (HeaderFooter) hfs.GetValue("FirstPage", GV.ReadOnly);
            }
            if (((pagePos == PagePosition.Even) || ((this.currentPage % 2) == 0)) && pageSetup.OddAndEvenPagesHeaderFooter)
            {
                return (HeaderFooter) hfs.GetValue("EvenPage", GV.ReadOnly);
            }
            return (HeaderFooter) hfs.GetValue("Primary", GV.ReadOnly);
        }

        private void FillNumPagesInfo()
        {
            for (int i = 1; i <= this.pageCount; i++)
            {
                if (!this.IsEmptyPage(i))
                {
                    FieldInfos infos = this.pageFieldInfos[i];
                    infos.numPages = this.pageCount;
                }
            }
        }

        private void FillSectionPagesInfo()
        {
            for (int i = this.currentPage; i > 0; i--)
            {
                if (!this.IsEmptyPage(i))
                {
                    FieldInfos infos = this.pageFieldInfos[i];
                    if (infos.section != this.sectionNumber)
                    {
                        return;
                    }
                    infos.sectionPages = this.sectionPages;
                }
            }
        }

        internal void Format(XGraphics gfx)
        {
            this.bookmarks = new Dictionary<string, FieldInfos.BookmarkInfo>();
            this.pageRenderInfos = new Dictionary<int, ArrayList>();
            this.pageInfos = new Dictionary<int, PageInfo>();
            this.pageFieldInfos = new Dictionary<int, FieldInfos>();
            this.formattedHeaders = new Dictionary<HeaderFooterPosition, FormattedHeaderFooter>();
            this.formattedFooters = new Dictionary<HeaderFooterPosition, FormattedHeaderFooter>();
            this.gfx = gfx;
            this.currentPage = 0;
            this.sectionNumber = 0;
            this.pageCount = 0;
            this.shownPageNumber = 0;
            this.documentRenderer.ProgressCompleted = 0;
            this.documentRenderer.ProgressMaximum = 0;
            if (this.documentRenderer.HasPrepareDocumentProgress)
            {
                foreach (Section section in this.document.Sections)
                {
                    this.documentRenderer.ProgressMaximum += section.Elements.Count;
                }
            }
            foreach (Section section2 in this.document.Sections)
            {
                this.isNewSection = true;
                this.currentSection = section2;
                this.sectionNumber++;
                if (this.NeedsEmptyPage())
                {
                    this.InsertEmptyPage();
                }
                new TopDownFormatter(this, this.documentRenderer, section2.Elements).FormatOnAreas(gfx, true);
                this.FillSectionPagesInfo();
                this.documentRenderer.ProgressCompleted += section2.Elements.Count;
            }
            this.pageCount = this.currentPage;
            this.FillNumPagesInfo();
        }

        private void FormatFooter(HeaderFooterPosition hfp, HeaderFooter footer)
        {
            if ((footer != null) && !this.formattedFooters.ContainsKey(hfp))
            {
                FormattedHeaderFooter footer2 = new FormattedHeaderFooter(footer, this.documentRenderer, this.currentFieldInfos) {
                    ContentRect = this.GetFooterArea(this.currentSection, this.currentPage)
                };
                footer2.Format(this.gfx);
                this.formattedFooters.Add(hfp, footer2);
            }
        }

        private void FormatHeader(HeaderFooterPosition hfp, HeaderFooter header)
        {
            if ((header != null) && !this.formattedHeaders.ContainsKey(hfp))
            {
                FormattedHeaderFooter footer = new FormattedHeaderFooter(header, this.documentRenderer, this.currentFieldInfos) {
                    ContentRect = this.GetHeaderArea(this.currentSection, this.currentPage)
                };
                footer.Format(this.gfx);
                this.formattedHeaders.Add(hfp, footer);
            }
        }

        private void FormatHeadersFooters()
        {
            HeadersFooters hfs = (HeadersFooters) this.currentSection.GetValue("Headers", GV.ReadOnly);
            if (hfs != null)
            {
                PagePosition currentPagePosition = this.CurrentPagePosition;
                HeaderFooterPosition key = new HeaderFooterPosition(this.sectionNumber, currentPagePosition);
                if (!this.formattedHeaders.ContainsKey(key))
                {
                    this.FormatHeader(key, this.ChooseHeaderFooter(hfs, currentPagePosition));
                }
            }
            HeadersFooters footers2 = (HeadersFooters) this.currentSection.GetValue("Footers", GV.ReadOnly);
            if (footers2 != null)
            {
                PagePosition pagePosition = this.CurrentPagePosition;
                HeaderFooterPosition position4 = new HeaderFooterPosition(this.sectionNumber, pagePosition);
                if (!this.formattedFooters.ContainsKey(position4))
                {
                    this.FormatFooter(position4, this.ChooseHeaderFooter(footers2, pagePosition));
                }
            }
        }

        private ElementAlignment GetCurrentAlignment(ElementAlignment alignment)
        {
            ElementAlignment alignment2 = alignment;
            if (alignment2 == ElementAlignment.Inside)
            {
                if ((this.currentPage % 2) == 0)
                {
                    return ElementAlignment.Far;
                }
                return ElementAlignment.Near;
            }
            if (alignment2 != ElementAlignment.Outside)
            {
                return alignment2;
            }
            if ((this.currentPage % 2) == 0)
            {
                return ElementAlignment.Near;
            }
            return ElementAlignment.Far;
        }

        internal FieldInfos GetFieldInfos(int page) => 
            this.pageFieldInfos[page];

        internal Rectangle GetFooterArea(int page)
        {
            FieldInfos infos = this.pageFieldInfos[page];
            Section section = this.document.Sections[infos.section - 1];
            return this.GetFooterArea(section, page);
        }

        private Rectangle GetFooterArea(Section section, int page)
        {
            XUnit point;
            XUnit unit2;
            XUnit unit3;
            PageSetup pageSetup = section.PageSetup;
            if (pageSetup.MirrorMargins && ((page % 2) == 0))
            {
                point = pageSetup.RightMargin.Point;
            }
            else
            {
                point = pageSetup.LeftMargin.Point;
            }
            if (pageSetup.Orientation == Orientation.Portrait)
            {
                unit2 = pageSetup.PageWidth.Point;
            }
            else
            {
                unit2 = pageSetup.PageHeight.Point;
            }
            unit2 = ((double) unit2) - (((float) pageSetup.LeftMargin) + ((float) pageSetup.RightMargin));
            if (pageSetup.Orientation == Orientation.Portrait)
            {
                unit3 = pageSetup.PageHeight.Point;
            }
            else
            {
                unit3 = pageSetup.PageWidth.Point;
            }
            unit3 = ((double) unit3) - pageSetup.BottomMargin.Point;
            return new Rectangle(point, unit3, unit2, (XUnit) (((float) pageSetup.BottomMargin) - ((float) pageSetup.FooterDistance)));
        }

        internal FormattedHeaderFooter GetFormattedFooter(int page)
        {
            PagePosition pagePosition = ((page % 2) == 0) ? PagePosition.Even : PagePosition.Odd;
            FieldInfos infos = this.pageFieldInfos[page];
            if (page == 1)
            {
                pagePosition = PagePosition.First;
            }
            else if (this.IsEmptyPage(page - 1))
            {
                pagePosition = PagePosition.First;
            }
            else
            {
                FieldInfos infos2 = this.pageFieldInfos[page - 1];
                if (infos.section != infos2.section)
                {
                    pagePosition = PagePosition.First;
                }
            }
            HeaderFooterPosition key = new HeaderFooterPosition(infos.section, pagePosition);
            if (this.formattedFooters.ContainsKey(key))
            {
                return this.formattedFooters[key];
            }
            return null;
        }

        internal FormattedHeaderFooter GetFormattedHeader(int page)
        {
            PagePosition pagePosition = ((page % 2) == 0) ? PagePosition.Even : PagePosition.Odd;
            FieldInfos infos = this.pageFieldInfos[page];
            if (page == 1)
            {
                pagePosition = PagePosition.First;
            }
            else if (this.IsEmptyPage(page - 1))
            {
                pagePosition = PagePosition.First;
            }
            else
            {
                FieldInfos infos2 = this.pageFieldInfos[page - 1];
                if (infos.section != infos2.section)
                {
                    pagePosition = PagePosition.First;
                }
            }
            HeaderFooterPosition key = new HeaderFooterPosition(infos.section, pagePosition);
            if (this.formattedHeaders.ContainsKey(key))
            {
                return this.formattedHeaders[key];
            }
            return null;
        }

        internal Rectangle GetHeaderArea(int page)
        {
            FieldInfos infos = this.pageFieldInfos[page];
            Section section = this.document.Sections[infos.section - 1];
            return this.GetHeaderArea(section, page);
        }

        private Rectangle GetHeaderArea(Section section, int page)
        {
            XUnit unit;
            XUnit unit2;
            PageSetup pageSetup = section.PageSetup;
            if (pageSetup.MirrorMargins && ((page % 2) == 0))
            {
                unit = pageSetup.RightMargin.Point;
            }
            else
            {
                unit = pageSetup.LeftMargin.Point;
            }
            if (pageSetup.Orientation == Orientation.Portrait)
            {
                unit2 = pageSetup.PageWidth.Point;
            }
            else
            {
                unit2 = pageSetup.PageHeight.Point;
            }
            unit2 = ((double) unit2) - (((float) pageSetup.LeftMargin) + ((float) pageSetup.RightMargin));
            XUnit point = pageSetup.HeaderDistance.Point;
            return new Rectangle(unit, point, unit2, (XUnit) (((float) pageSetup.TopMargin) - ((float) pageSetup.HeaderDistance)));
        }

        public PageInfo GetPageInfo(int page)
        {
            if ((page < 1) || (page > this.pageCount))
            {
                throw new ArgumentOutOfRangeException("page");
            }
            return this.pageInfos[page];
        }

        internal RenderInfo[] GetRenderInfos(int page)
        {
            if (this.pageRenderInfos.ContainsKey(page))
            {
                return (RenderInfo[]) this.pageRenderInfos[page].ToArray(typeof(RenderInfo));
            }
            return null;
        }

        private void InitFieldInfos()
        {
            this.currentFieldInfos = new FieldInfos(this.bookmarks);
            this.currentFieldInfos.pyhsicalPageNr = this.currentPage;
            this.currentFieldInfos.section = this.sectionNumber;
            if (this.isNewSection && !this.currentSection.PageSetup.IsNull("StartingNumber"))
            {
                this.shownPageNumber = this.currentSection.PageSetup.StartingNumber;
            }
            this.currentFieldInfos.displayPageNr = this.shownPageNumber;
        }

        private void InsertEmptyPage()
        {
            this.currentPage++;
            this.shownPageNumber++;
            this.emptyPages.Add(this.currentPage, null);
            XSize size = this.CalcPageSize(this.currentSection.PageSetup);
            PageOrientation orientation = this.CalcPageOrientation(this.currentSection.PageSetup);
            PageInfo info = new PageInfo(size.Width, size.Height, orientation);
            this.pageInfos.Add(this.currentPage, info);
        }

        internal bool IsEmptyPage(int page) => 
            this.emptyPages.ContainsKey(page);

        Area IAreaProvider.GetNextArea()
        {
            if (this.isNewSection)
            {
                this.sectionPages = 0;
            }
            this.currentPage++;
            this.shownPageNumber++;
            this.sectionPages++;
            this.InitFieldInfos();
            this.FormatHeadersFooters();
            this.isNewSection = false;
            return this.CalcContentRect(this.currentPage);
        }

        bool IAreaProvider.IsAreaBreakBefore(LayoutInfo layoutInfo) => 
            layoutInfo.PageBreakBefore;

        bool IAreaProvider.PositionHorizontally(LayoutInfo layoutInfo)
        {
            switch (layoutInfo.HorizontalReference)
            {
                case HorizontalReference.AreaBoundary:
                case HorizontalReference.PageMargin:
                    return this.PositionHorizontallyToMargin(layoutInfo);

                case HorizontalReference.Page:
                    return this.PositionHorizontallyToPage(layoutInfo);
            }
            return false;
        }

        bool IAreaProvider.PositionVertically(LayoutInfo layoutInfo)
        {
            switch (layoutInfo.VerticalReference)
            {
                case VerticalReference.PreviousElement:
                    return false;

                case VerticalReference.AreaBoundary:
                case VerticalReference.PageMargin:
                    return this.PositionVerticallyToMargin(layoutInfo);

                case VerticalReference.Page:
                    return this.PositionVerticallyToPage(layoutInfo);
            }
            return false;
        }

        Area IAreaProvider.ProbeNextArea() => 
            this.CalcContentRect(this.currentPage + 1);

        void IAreaProvider.StoreRenderInfos(ArrayList renderInfos)
        {
            this.pageRenderInfos.Add(this.currentPage, renderInfos);
            XSize size = this.CalcPageSize(this.currentSection.PageSetup);
            PageOrientation orientation = this.CalcPageOrientation(this.currentSection.PageSetup);
            PageInfo info = new PageInfo(size.Width, size.Height, orientation);
            this.pageInfos.Add(this.currentPage, info);
            this.pageFieldInfos.Add(this.currentPage, this.currentFieldInfos);
        }

        private bool NeedsEmptyPage()
        {
            int num = this.currentPage + 1;
            PageSetup pageSetup = this.currentSection.PageSetup;
            bool flag = pageSetup.SectionStart == BreakType.BreakEvenPage;
            if (pageSetup.SectionStart == BreakType.BreakOddPage)
            {
                return ((num % 2) == 0);
            }
            return (flag && ((num % 2) == 1));
        }

        private bool PositionHorizontallyToMargin(LayoutInfo layoutInfo)
        {
            XUnit unit;
            Rectangle rectangle = this.CalcContentRect(this.currentPage);
            switch (this.GetCurrentAlignment(layoutInfo.HorizontalAlignment))
            {
                case ElementAlignment.Near:
                {
                    if (layoutInfo.Left == 0)
                    {
                        if (layoutInfo.MarginLeft != 0)
                        {
                            Area area2 = layoutInfo.ContentArea;
                            area2.X = ((double) area2.X) + ((double) layoutInfo.MarginLeft);
                            return true;
                        }
                        return false;
                    }
                    Area contentArea = layoutInfo.ContentArea;
                    contentArea.X = ((double) contentArea.X) + ((double) layoutInfo.Left);
                    return true;
                }
                case ElementAlignment.Center:
                    unit = ((double) rectangle.Width) - ((double) layoutInfo.ContentArea.Width);
                    unit = ((double) rectangle.X) + (((double) unit) / 2.0);
                    layoutInfo.ContentArea.X = unit;
                    return true;

                case ElementAlignment.Far:
                    unit = ((double) rectangle.X) + ((double) rectangle.Width);
                    unit = ((double) unit) - ((double) layoutInfo.ContentArea.Width);
                    unit = ((double) unit) - ((double) layoutInfo.MarginRight);
                    layoutInfo.ContentArea.X = unit;
                    return true;
            }
            return false;
        }

        private bool PositionHorizontallyToPage(LayoutInfo layoutInfo)
        {
            XUnit marginLeft;
            switch (this.GetCurrentAlignment(layoutInfo.HorizontalAlignment))
            {
                case ElementAlignment.Near:
                    if ((layoutInfo.HorizontalReference != HorizontalReference.Page) && (layoutInfo.HorizontalReference != HorizontalReference.PageMargin))
                    {
                        marginLeft = Math.Max((double) layoutInfo.MarginLeft, (double) layoutInfo.Left);
                        break;
                    }
                    marginLeft = layoutInfo.MarginLeft;
                    break;

                case ElementAlignment.Center:
                    marginLeft = this.currentSection.PageSetup.PageWidth.Point;
                    marginLeft = ((double) marginLeft) - ((double) layoutInfo.ContentArea.Width);
                    marginLeft = ((double) marginLeft) / 2.0;
                    layoutInfo.ContentArea.X = marginLeft;
                    goto Label_0142;

                case ElementAlignment.Far:
                    marginLeft = this.currentSection.PageSetup.PageWidth.Point;
                    marginLeft = ((double) marginLeft) - ((double) layoutInfo.ContentArea.Width);
                    marginLeft = ((double) marginLeft) - ((double) layoutInfo.MarginRight);
                    layoutInfo.ContentArea.X = marginLeft;
                    goto Label_0142;

                default:
                    goto Label_0142;
            }
            layoutInfo.ContentArea.X = marginLeft;
        Label_0142:
            return true;
        }

        private bool PositionVerticallyToMargin(LayoutInfo layoutInfo)
        {
            XUnit y;
            Rectangle rectangle = this.CalcContentRect(this.currentPage);
            switch (layoutInfo.VerticalAlignment)
            {
                case ElementAlignment.Near:
                    y = rectangle.Y;
                    if (layoutInfo.Top != 0)
                    {
                        y = ((double) y) + ((double) layoutInfo.Top);
                        break;
                    }
                    y = ((double) y) + ((double) layoutInfo.MarginTop);
                    break;

                case ElementAlignment.Center:
                    y = ((double) rectangle.Height) - ((double) layoutInfo.ContentArea.Height);
                    y = ((double) rectangle.Y) + (((double) y) / 2.0);
                    layoutInfo.ContentArea.Y = y;
                    goto Label_014F;

                case ElementAlignment.Far:
                    y = ((double) rectangle.Y) + ((double) rectangle.Height);
                    y = ((double) y) - ((double) layoutInfo.ContentArea.Height);
                    y = ((double) y) - ((double) layoutInfo.MarginBottom);
                    layoutInfo.ContentArea.Y = y;
                    goto Label_014F;

                default:
                    goto Label_014F;
            }
            layoutInfo.ContentArea.Y = y;
        Label_014F:
            return true;
        }

        private bool PositionVerticallyToPage(LayoutInfo layoutInfo)
        {
            XUnit point;
            switch (layoutInfo.VerticalAlignment)
            {
                case ElementAlignment.Near:
                    point = Math.Max((double) layoutInfo.MarginTop, (double) layoutInfo.Top);
                    layoutInfo.ContentArea.Y = point;
                    break;

                case ElementAlignment.Center:
                    point = this.currentSection.PageSetup.PageHeight.Point;
                    point = ((double) point) - ((double) layoutInfo.ContentArea.Height);
                    point = ((double) point) / 2.0;
                    layoutInfo.ContentArea.Y = point;
                    break;

                case ElementAlignment.Far:
                    point = this.currentSection.PageSetup.PageHeight.Point;
                    point = ((double) point) - ((double) layoutInfo.ContentArea.Height);
                    point = ((double) point) - ((double) layoutInfo.MarginBottom);
                    layoutInfo.ContentArea.Y = point;
                    break;
            }
            return true;
        }

        private PagePosition CurrentPagePosition
        {
            get
            {
                if (this.isNewSection)
                {
                    return PagePosition.First;
                }
                if ((this.currentPage % 2) == 0)
                {
                    return PagePosition.Even;
                }
                return PagePosition.Odd;
            }
        }

        FieldInfos IAreaProvider.AreaFieldInfos =>
            this.currentFieldInfos;

        public int PageCount =>
            this.pageCount;

        [StructLayout(LayoutKind.Sequential)]
        private struct HeaderFooterPosition
        {
            internal int sectionNr;
            internal FormattedDocument.PagePosition pagePosition;
            internal HeaderFooterPosition(int sectionNr, FormattedDocument.PagePosition pagePosition)
            {
                this.sectionNr = sectionNr;
                this.pagePosition = pagePosition;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is FormattedDocument.HeaderFooterPosition))
                {
                    return false;
                }
                FormattedDocument.HeaderFooterPosition position = (FormattedDocument.HeaderFooterPosition) obj;
                return ((this.sectionNr == position.sectionNr) && (this.pagePosition == position.pagePosition));
            }

            public override int GetHashCode() => 
                (this.sectionNr.GetHashCode() ^ this.pagePosition.GetHashCode());
        }

        private enum PagePosition
        {
            First,
            Odd,
            Even
        }
    }
}

