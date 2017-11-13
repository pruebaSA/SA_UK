namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class Border : DocumentObject
    {
        [DV]
        internal MigraDoc.DocumentObjectModel.Color color;
        internal NBool fClear;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV(Type=typeof(BorderStyle))]
        internal NEnum style;
        [DV]
        internal NBool visible;
        [DV]
        internal Unit width;

        public Border()
        {
            this.visible = NBool.NullValue;
            this.style = NEnum.NullValue(typeof(BorderStyle));
            this.width = Unit.NullValue;
            this.color = MigraDoc.DocumentObjectModel.Color.Empty;
            this.fClear = new NBool(false);
        }

        internal Border(DocumentObject parent) : base(parent)
        {
            this.visible = NBool.NullValue;
            this.style = NEnum.NullValue(typeof(BorderStyle));
            this.width = Unit.NullValue;
            this.color = MigraDoc.DocumentObjectModel.Color.Empty;
            this.fClear = new NBool(false);
        }

        public void Clear()
        {
            this.fClear.Value = true;
        }

        public Border Clone() => 
            ((Border) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            throw new Exception("A Border cannot be serialized alone.");
        }

        internal void Serialize(Serializer serializer, string name, Border refBorder)
        {
            if (this.fClear.Value)
            {
                serializer.WriteLine(name + " = null");
            }
            int pos = serializer.BeginContent(name);
            if (!this.visible.IsNull && ((refBorder == null) || (this.Visible != refBorder.Visible)))
            {
                serializer.WriteSimpleAttribute("Visible", this.Visible);
            }
            if (!this.style.IsNull && ((refBorder == null) || (this.Style != refBorder.Style)))
            {
                serializer.WriteSimpleAttribute("Style", this.Style);
            }
            if (!this.width.IsNull && ((refBorder == null) || (this.Width != refBorder.Width)))
            {
                serializer.WriteSimpleAttribute("Width", this.Width);
            }
            if (!this.color.IsNull && ((refBorder == null) || (this.Color != refBorder.Color)))
            {
                serializer.WriteSimpleAttribute("Color", this.Color);
            }
            serializer.EndContent(pos);
        }

        public bool BorderCleared =>
            this.fClear.Value;

        public MigraDoc.DocumentObjectModel.Color Color
        {
            get => 
                this.color;
            set
            {
                this.color = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Border));
                }
                return meta;
            }
        }

        public string Name =>
            ((Borders) base.parent).GetMyName(this);

        public BorderStyle Style
        {
            get => 
                ((BorderStyle) this.style.Value);
            set
            {
                this.style.Value = (int) value;
            }
        }

        public bool Visible
        {
            get => 
                this.visible.Value;
            set
            {
                this.visible.Value = value;
            }
        }

        public Unit Width
        {
            get => 
                this.width;
            set
            {
                this.width = value;
            }
        }
    }
}

