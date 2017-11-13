namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class ParagraphFormat : DocumentObject
    {
        [DV(Type=typeof(ParagraphAlignment))]
        internal NEnum alignment;
        [DV]
        internal MigraDoc.DocumentObjectModel.Borders borders;
        [DV]
        internal Unit firstLineIndent;
        [DV]
        internal MigraDoc.DocumentObjectModel.Font font;
        [DV]
        internal NBool keepTogether;
        [DV]
        internal NBool keepWithNext;
        [DV]
        internal Unit leftIndent;
        [DV]
        internal Unit lineSpacing;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.LineSpacingRule))]
        internal NEnum lineSpacingRule;
        [DV]
        internal MigraDoc.DocumentObjectModel.ListInfo listInfo;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.OutlineLevel))]
        internal NEnum outlineLevel;
        [DV]
        internal NBool pageBreakBefore;
        [DV]
        internal Unit rightIndent;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shading shading;
        [DV]
        internal Unit spaceAfter;
        [DV]
        internal Unit spaceBefore;
        [DV]
        internal MigraDoc.DocumentObjectModel.TabStops tabStops;
        [DV]
        internal NBool widowControl;

        public ParagraphFormat()
        {
            this.alignment = NEnum.NullValue(typeof(ParagraphAlignment));
            this.firstLineIndent = Unit.NullValue;
            this.keepTogether = NBool.NullValue;
            this.keepWithNext = NBool.NullValue;
            this.leftIndent = Unit.NullValue;
            this.lineSpacing = Unit.NullValue;
            this.lineSpacingRule = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.LineSpacingRule));
            this.outlineLevel = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.OutlineLevel));
            this.pageBreakBefore = NBool.NullValue;
            this.rightIndent = Unit.NullValue;
            this.spaceAfter = Unit.NullValue;
            this.spaceBefore = Unit.NullValue;
            this.widowControl = NBool.NullValue;
        }

        internal ParagraphFormat(DocumentObject parent) : base(parent)
        {
            this.alignment = NEnum.NullValue(typeof(ParagraphAlignment));
            this.firstLineIndent = Unit.NullValue;
            this.keepTogether = NBool.NullValue;
            this.keepWithNext = NBool.NullValue;
            this.leftIndent = Unit.NullValue;
            this.lineSpacing = Unit.NullValue;
            this.lineSpacingRule = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.LineSpacingRule));
            this.outlineLevel = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.OutlineLevel));
            this.pageBreakBefore = NBool.NullValue;
            this.rightIndent = Unit.NullValue;
            this.spaceAfter = Unit.NullValue;
            this.spaceBefore = Unit.NullValue;
            this.widowControl = NBool.NullValue;
        }

        public void Add(TabStop tabStop)
        {
            this.TabStops.AddTabStop(tabStop);
        }

        public TabStop AddTabStop(Unit position) => 
            this.TabStops.AddTabStop(position);

        public TabStop AddTabStop(Unit position, TabAlignment alignment) => 
            this.TabStops.AddTabStop(position, alignment);

        public TabStop AddTabStop(Unit position, TabLeader leader) => 
            this.TabStops.AddTabStop(position, leader);

        public TabStop AddTabStop(Unit position, TabAlignment alignment, TabLeader leader) => 
            this.TabStops.AddTabStop(position, alignment, leader);

        public void ClearAll()
        {
            this.TabStops.ClearAll();
        }

        public ParagraphFormat Clone() => 
            ((ParagraphFormat) this.DeepCopy());

        protected override object DeepCopy()
        {
            ParagraphFormat format = (ParagraphFormat) base.DeepCopy();
            if (format.font != null)
            {
                format.font = format.font.Clone();
                format.font.parent = format;
            }
            if (format.shading != null)
            {
                format.shading = format.shading.Clone();
                format.shading.parent = format;
            }
            if (format.borders != null)
            {
                format.borders = format.borders.Clone();
                format.borders.parent = format;
            }
            if (format.tabStops != null)
            {
                format.tabStops = format.tabStops.Clone();
                format.tabStops.parent = format;
            }
            if (format.listInfo != null)
            {
                format.listInfo = format.listInfo.Clone();
                format.listInfo.parent = format;
            }
            return format;
        }

        public void RemoveTabStop(Unit position)
        {
            this.TabStops.RemoveTabStop(position);
        }

        internal override void Serialize(Serializer serializer)
        {
            if (base.parent is Style)
            {
                this.Serialize(serializer, "ParagraphFormat", null);
            }
            else
            {
                this.Serialize(serializer, "Format", null);
            }
        }

        internal void Serialize(Serializer serializer, string name, ParagraphFormat refFormat)
        {
            int pos = serializer.BeginContent(name);
            if (!this.IsNull("Font") && (base.Parent.GetType() != typeof(Style)))
            {
                this.Font.Serialize(serializer);
            }
            if (!this.alignment.IsNull && ((refFormat == null) || (this.alignment != refFormat.alignment)))
            {
                serializer.WriteSimpleAttribute("Alignment", this.Alignment);
            }
            if (!this.leftIndent.IsNull && ((refFormat == null) || (this.leftIndent != refFormat.leftIndent)))
            {
                serializer.WriteSimpleAttribute("LeftIndent", this.LeftIndent);
            }
            if (!this.firstLineIndent.IsNull && ((refFormat == null) || (this.firstLineIndent != refFormat.firstLineIndent)))
            {
                serializer.WriteSimpleAttribute("FirstLineIndent", this.FirstLineIndent);
            }
            if (!this.rightIndent.IsNull && ((refFormat == null) || (this.rightIndent != refFormat.rightIndent)))
            {
                serializer.WriteSimpleAttribute("RightIndent", this.RightIndent);
            }
            if (!this.spaceBefore.IsNull && ((refFormat == null) || (this.spaceBefore != refFormat.spaceBefore)))
            {
                serializer.WriteSimpleAttribute("SpaceBefore", this.SpaceBefore);
            }
            if (!this.spaceAfter.IsNull && ((refFormat == null) || (this.spaceAfter != refFormat.spaceAfter)))
            {
                serializer.WriteSimpleAttribute("SpaceAfter", this.SpaceAfter);
            }
            if (!this.lineSpacingRule.IsNull && ((refFormat == null) || (this.lineSpacingRule != refFormat.lineSpacingRule)))
            {
                serializer.WriteSimpleAttribute("LineSpacingRule", this.LineSpacingRule);
            }
            if (!this.lineSpacing.IsNull && ((refFormat == null) || (this.lineSpacing != refFormat.lineSpacing)))
            {
                serializer.WriteSimpleAttribute("LineSpacing", this.LineSpacing);
            }
            if (!this.keepTogether.IsNull && ((refFormat == null) || (this.keepTogether != refFormat.keepTogether)))
            {
                serializer.WriteSimpleAttribute("KeepTogether", this.KeepTogether);
            }
            if (!this.keepWithNext.IsNull && ((refFormat == null) || (this.keepWithNext != refFormat.keepWithNext)))
            {
                serializer.WriteSimpleAttribute("KeepWithNext", this.KeepWithNext);
            }
            if (!this.widowControl.IsNull && ((refFormat == null) || (this.widowControl != refFormat.widowControl)))
            {
                serializer.WriteSimpleAttribute("WidowControl", this.WidowControl);
            }
            if (!this.pageBreakBefore.IsNull && ((refFormat == null) || (this.pageBreakBefore != refFormat.pageBreakBefore)))
            {
                serializer.WriteSimpleAttribute("PageBreakBefore", this.PageBreakBefore);
            }
            if (!this.outlineLevel.IsNull && ((refFormat == null) || (this.outlineLevel != refFormat.outlineLevel)))
            {
                serializer.WriteSimpleAttribute("OutlineLevel", this.OutlineLevel);
            }
            if (!this.IsNull("ListInfo"))
            {
                this.ListInfo.Serialize(serializer);
            }
            if (!this.IsNull("TabStops"))
            {
                this.tabStops.Serialize(serializer);
            }
            if (!this.IsNull("Borders"))
            {
                if (refFormat != null)
                {
                    this.borders.Serialize(serializer, refFormat.Borders);
                }
                else
                {
                    this.borders.Serialize(serializer, null);
                }
            }
            if (!this.IsNull("Shading"))
            {
                this.shading.Serialize(serializer);
            }
            serializer.EndContent(pos);
        }

        public ParagraphAlignment Alignment
        {
            get => 
                ((ParagraphAlignment) this.alignment.Value);
            set
            {
                this.alignment.Value = (int) value;
            }
        }

        public MigraDoc.DocumentObjectModel.Borders Borders
        {
            get
            {
                if (this.borders == null)
                {
                    this.borders = new MigraDoc.DocumentObjectModel.Borders(this);
                }
                return this.borders;
            }
            set
            {
                base.SetParent(value);
                this.borders = value;
            }
        }

        public Unit FirstLineIndent
        {
            get => 
                this.firstLineIndent;
            set
            {
                this.firstLineIndent = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Font Font
        {
            get
            {
                if (this.font == null)
                {
                    this.font = new MigraDoc.DocumentObjectModel.Font(this);
                }
                return this.font;
            }
            set
            {
                base.SetParent(value);
                this.font = value;
            }
        }

        public bool HasTabStops =>
            (this.tabStops != null);

        public bool KeepTogether
        {
            get => 
                this.keepTogether.Value;
            set
            {
                this.keepTogether.Value = value;
            }
        }

        public bool KeepWithNext
        {
            get => 
                this.keepWithNext.Value;
            set
            {
                this.keepWithNext.Value = value;
            }
        }

        public Unit LeftIndent
        {
            get => 
                this.leftIndent;
            set
            {
                this.leftIndent = value;
            }
        }

        public Unit LineSpacing
        {
            get => 
                this.lineSpacing;
            set
            {
                this.lineSpacing = value;
            }
        }

        public MigraDoc.DocumentObjectModel.LineSpacingRule LineSpacingRule
        {
            get => 
                ((MigraDoc.DocumentObjectModel.LineSpacingRule) this.lineSpacingRule.Value);
            set
            {
                this.lineSpacingRule.Value = (int) value;
            }
        }

        public MigraDoc.DocumentObjectModel.ListInfo ListInfo
        {
            get
            {
                if (this.listInfo == null)
                {
                    this.listInfo = new MigraDoc.DocumentObjectModel.ListInfo(this);
                }
                return this.listInfo;
            }
            set
            {
                base.SetParent(value);
                this.listInfo = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(ParagraphFormat));
                }
                return meta;
            }
        }

        public MigraDoc.DocumentObjectModel.OutlineLevel OutlineLevel
        {
            get => 
                ((MigraDoc.DocumentObjectModel.OutlineLevel) this.outlineLevel.Value);
            set
            {
                this.outlineLevel.Value = (int) value;
            }
        }

        public bool PageBreakBefore
        {
            get => 
                this.pageBreakBefore.Value;
            set
            {
                this.pageBreakBefore.Value = value;
            }
        }

        public Unit RightIndent
        {
            get => 
                this.rightIndent;
            set
            {
                this.rightIndent = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Shading Shading
        {
            get
            {
                if (this.shading == null)
                {
                    this.shading = new MigraDoc.DocumentObjectModel.Shading(this);
                }
                return this.shading;
            }
            set
            {
                base.SetParent(value);
                this.shading = value;
            }
        }

        public Unit SpaceAfter
        {
            get => 
                this.spaceAfter;
            set
            {
                this.spaceAfter = value;
            }
        }

        public Unit SpaceBefore
        {
            get => 
                this.spaceBefore;
            set
            {
                this.spaceBefore = value;
            }
        }

        public MigraDoc.DocumentObjectModel.TabStops TabStops
        {
            get
            {
                if (this.tabStops == null)
                {
                    this.tabStops = new MigraDoc.DocumentObjectModel.TabStops(this);
                }
                return this.tabStops;
            }
            set
            {
                base.SetParent(value);
                this.tabStops = value;
            }
        }

        public bool WidowControl
        {
            get => 
                this.widowControl.Value;
            set
            {
                this.widowControl.Value = value;
            }
        }
    }
}

