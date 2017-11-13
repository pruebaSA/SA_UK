namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;
    using System.Runtime.InteropServices;

    public class PageSetup : DocumentObject
    {
        [DV]
        internal Unit bottomMargin;
        [DV]
        internal NString comment;
        private static PageSetup defaultPageSetup;
        [DV]
        internal NBool differentFirstPageHeaderFooter;
        [DV]
        internal Unit footerDistance;
        [DV]
        internal Unit headerDistance;
        [DV]
        internal NBool horizontalPageBreak;
        [DV]
        internal Unit leftMargin;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NBool mirrorMargins;
        [DV]
        internal NBool oddAndEvenPagesHeaderFooter;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.Orientation))]
        internal NEnum orientation;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.PageFormat))]
        internal NEnum pageFormat;
        [DV]
        internal Unit pageHeight;
        [DV]
        internal Unit pageWidth;
        [DV]
        internal Unit rightMargin;
        [DV(Type=typeof(BreakType))]
        internal NEnum sectionStart;
        [DV]
        internal NInt startingNumber;
        [DV]
        internal Unit topMargin;

        public PageSetup()
        {
            this.sectionStart = NEnum.NullValue(typeof(BreakType));
            this.orientation = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Orientation));
            this.pageWidth = Unit.NullValue;
            this.startingNumber = NInt.NullValue;
            this.pageHeight = Unit.NullValue;
            this.topMargin = Unit.NullValue;
            this.bottomMargin = Unit.NullValue;
            this.leftMargin = Unit.NullValue;
            this.rightMargin = Unit.NullValue;
            this.oddAndEvenPagesHeaderFooter = NBool.NullValue;
            this.differentFirstPageHeaderFooter = NBool.NullValue;
            this.headerDistance = Unit.NullValue;
            this.footerDistance = Unit.NullValue;
            this.mirrorMargins = NBool.NullValue;
            this.horizontalPageBreak = NBool.NullValue;
            this.pageFormat = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.PageFormat));
            this.comment = NString.NullValue;
        }

        internal PageSetup(DocumentObject parent) : base(parent)
        {
            this.sectionStart = NEnum.NullValue(typeof(BreakType));
            this.orientation = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Orientation));
            this.pageWidth = Unit.NullValue;
            this.startingNumber = NInt.NullValue;
            this.pageHeight = Unit.NullValue;
            this.topMargin = Unit.NullValue;
            this.bottomMargin = Unit.NullValue;
            this.leftMargin = Unit.NullValue;
            this.rightMargin = Unit.NullValue;
            this.oddAndEvenPagesHeaderFooter = NBool.NullValue;
            this.differentFirstPageHeaderFooter = NBool.NullValue;
            this.headerDistance = Unit.NullValue;
            this.footerDistance = Unit.NullValue;
            this.mirrorMargins = NBool.NullValue;
            this.horizontalPageBreak = NBool.NullValue;
            this.pageFormat = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.PageFormat));
            this.comment = NString.NullValue;
        }

        public PageSetup Clone() => 
            ((PageSetup) this.DeepCopy());

        public static void GetPageSize(MigraDoc.DocumentObjectModel.PageFormat pageFormat, out Unit pageWidth, out Unit pageHeight)
        {
            pageWidth = 0;
            pageHeight = 0;
            int num = 0x4a5;
            int num2 = 0x349;
            int num3 = 0;
            int num4 = 0;
            switch (pageFormat)
            {
                case MigraDoc.DocumentObjectModel.PageFormat.A0:
                    num3 = num;
                    num4 = num2;
                    break;

                case MigraDoc.DocumentObjectModel.PageFormat.A1:
                    num3 = num2;
                    num4 = num / 2;
                    break;

                case MigraDoc.DocumentObjectModel.PageFormat.A2:
                    num3 = num / 2;
                    num4 = num2 / 2;
                    break;

                case MigraDoc.DocumentObjectModel.PageFormat.A3:
                    num3 = num2 / 2;
                    num4 = num / 4;
                    break;

                case MigraDoc.DocumentObjectModel.PageFormat.A4:
                    num3 = num / 4;
                    num4 = num2 / 4;
                    break;

                case MigraDoc.DocumentObjectModel.PageFormat.A5:
                    num3 = num2 / 4;
                    num4 = num / 8;
                    break;

                case MigraDoc.DocumentObjectModel.PageFormat.A6:
                    num3 = num / 8;
                    num4 = num2 / 8;
                    break;

                case MigraDoc.DocumentObjectModel.PageFormat.B5:
                    num3 = 0x101;
                    num4 = 0xb6;
                    break;

                case MigraDoc.DocumentObjectModel.PageFormat.Letter:
                    pageWidth = Unit.FromPoint(612.0);
                    pageHeight = Unit.FromPoint(792.0);
                    break;

                case MigraDoc.DocumentObjectModel.PageFormat.Legal:
                    pageWidth = Unit.FromPoint(612.0);
                    pageHeight = Unit.FromPoint(1008.0);
                    break;

                case MigraDoc.DocumentObjectModel.PageFormat.Ledger:
                    pageWidth = Unit.FromPoint(1224.0);
                    pageHeight = Unit.FromPoint(792.0);
                    break;

                case MigraDoc.DocumentObjectModel.PageFormat.P11x17:
                    pageWidth = Unit.FromPoint(792.0);
                    pageHeight = Unit.FromPoint(1224.0);
                    break;
            }
            if (num3 > 0)
            {
                pageHeight = Unit.FromMillimeter((double) num3);
            }
            if (num4 > 0)
            {
                pageWidth = Unit.FromMillimeter((double) num4);
            }
        }

        public PageSetup PreviousPageSetup()
        {
            Section parent = base.Parent as Section;
            if (parent != null)
            {
                parent = parent.PreviousSection();
                if (parent != null)
                {
                    return parent.PageSetup;
                }
            }
            return null;
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(this.comment.Value);
            int pos = serializer.BeginContent("PageSetup");
            if (!this.pageHeight.IsNull)
            {
                serializer.WriteSimpleAttribute("PageHeight", this.PageHeight);
            }
            if (!this.pageWidth.IsNull)
            {
                serializer.WriteSimpleAttribute("PageWidth", this.PageWidth);
            }
            if (!this.orientation.IsNull)
            {
                serializer.WriteSimpleAttribute("Orientation", this.Orientation);
            }
            if (!this.leftMargin.IsNull)
            {
                serializer.WriteSimpleAttribute("LeftMargin", this.LeftMargin);
            }
            if (!this.rightMargin.IsNull)
            {
                serializer.WriteSimpleAttribute("RightMargin", this.RightMargin);
            }
            if (!this.topMargin.IsNull)
            {
                serializer.WriteSimpleAttribute("TopMargin", this.TopMargin);
            }
            if (!this.bottomMargin.IsNull)
            {
                serializer.WriteSimpleAttribute("BottomMargin", this.BottomMargin);
            }
            if (!this.footerDistance.IsNull)
            {
                serializer.WriteSimpleAttribute("FooterDistance", this.FooterDistance);
            }
            if (!this.headerDistance.IsNull)
            {
                serializer.WriteSimpleAttribute("HeaderDistance", this.HeaderDistance);
            }
            if (!this.oddAndEvenPagesHeaderFooter.IsNull)
            {
                serializer.WriteSimpleAttribute("OddAndEvenPagesHeaderFooter", this.OddAndEvenPagesHeaderFooter);
            }
            if (!this.differentFirstPageHeaderFooter.IsNull)
            {
                serializer.WriteSimpleAttribute("DifferentFirstPageHeaderFooter", this.DifferentFirstPageHeaderFooter);
            }
            if (!this.sectionStart.IsNull)
            {
                serializer.WriteSimpleAttribute("SectionStart", this.SectionStart);
            }
            if (!this.pageFormat.IsNull)
            {
                serializer.WriteSimpleAttribute("PageFormat", this.PageFormat);
            }
            if (!this.mirrorMargins.IsNull)
            {
                serializer.WriteSimpleAttribute("MirrorMargins", this.MirrorMargins);
            }
            if (!this.horizontalPageBreak.IsNull)
            {
                serializer.WriteSimpleAttribute("HorizontalPageBreak", this.HorizontalPageBreak);
            }
            if (!this.startingNumber.IsNull)
            {
                serializer.WriteSimpleAttribute("StartingNumber", this.StartingNumber);
            }
            serializer.EndContent(pos);
        }

        public Unit BottomMargin
        {
            get => 
                this.bottomMargin;
            set
            {
                this.bottomMargin = value;
            }
        }

        public string Comment
        {
            get => 
                this.comment.Value;
            set
            {
                this.comment.Value = value;
            }
        }

        internal static PageSetup DefaultPageSetup
        {
            get
            {
                if (defaultPageSetup == null)
                {
                    defaultPageSetup = new PageSetup();
                    defaultPageSetup.PageFormat = MigraDoc.DocumentObjectModel.PageFormat.A4;
                    defaultPageSetup.SectionStart = BreakType.BreakNextPage;
                    defaultPageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Portrait;
                    defaultPageSetup.PageWidth = "21cm";
                    defaultPageSetup.PageHeight = "29.7cm";
                    defaultPageSetup.TopMargin = "2.5cm";
                    defaultPageSetup.BottomMargin = "2cm";
                    defaultPageSetup.LeftMargin = "2.5cm";
                    defaultPageSetup.RightMargin = "2.5cm";
                    defaultPageSetup.HeaderDistance = "1.25cm";
                    defaultPageSetup.FooterDistance = "1.25cm";
                    defaultPageSetup.OddAndEvenPagesHeaderFooter = false;
                    defaultPageSetup.DifferentFirstPageHeaderFooter = false;
                    defaultPageSetup.MirrorMargins = false;
                    defaultPageSetup.HorizontalPageBreak = false;
                }
                return defaultPageSetup;
            }
        }

        public bool DifferentFirstPageHeaderFooter
        {
            get => 
                this.differentFirstPageHeaderFooter.Value;
            set
            {
                this.differentFirstPageHeaderFooter.Value = value;
            }
        }

        public Unit FooterDistance
        {
            get => 
                this.footerDistance;
            set
            {
                this.footerDistance = value;
            }
        }

        public Unit HeaderDistance
        {
            get => 
                this.headerDistance;
            set
            {
                this.headerDistance = value;
            }
        }

        public bool HorizontalPageBreak
        {
            get => 
                this.horizontalPageBreak.Value;
            set
            {
                this.horizontalPageBreak.Value = value;
            }
        }

        public Unit LeftMargin
        {
            get => 
                this.leftMargin;
            set
            {
                this.leftMargin = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(PageSetup));
                }
                return meta;
            }
        }

        public bool MirrorMargins
        {
            get => 
                this.mirrorMargins.Value;
            set
            {
                this.mirrorMargins.Value = value;
            }
        }

        public bool OddAndEvenPagesHeaderFooter
        {
            get => 
                this.oddAndEvenPagesHeaderFooter.Value;
            set
            {
                this.oddAndEvenPagesHeaderFooter.Value = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Orientation Orientation
        {
            get => 
                ((MigraDoc.DocumentObjectModel.Orientation) this.orientation.Value);
            set
            {
                this.orientation.Value = (int) value;
            }
        }

        public MigraDoc.DocumentObjectModel.PageFormat PageFormat
        {
            get => 
                ((MigraDoc.DocumentObjectModel.PageFormat) this.pageFormat.Value);
            set
            {
                this.pageFormat.Value = (int) value;
            }
        }

        public Unit PageHeight
        {
            get => 
                this.pageHeight;
            set
            {
                this.pageHeight = value;
            }
        }

        public Unit PageWidth
        {
            get => 
                this.pageWidth;
            set
            {
                this.pageWidth = value;
            }
        }

        public Unit RightMargin
        {
            get => 
                this.rightMargin;
            set
            {
                this.rightMargin = value;
            }
        }

        public BreakType SectionStart
        {
            get => 
                ((BreakType) this.sectionStart.Value);
            set
            {
                this.sectionStart.Value = (int) value;
            }
        }

        public int StartingNumber
        {
            get => 
                this.startingNumber.Value;
            set
            {
                this.startingNumber.Value = value;
            }
        }

        public Unit TopMargin
        {
            get => 
                this.topMargin;
            set
            {
                this.topMargin = value;
            }
        }
    }
}

