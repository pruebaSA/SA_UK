namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public sealed class Shading : DocumentObject
    {
        [DV]
        internal MigraDoc.DocumentObjectModel.Color color;
        internal bool isCleared;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NBool visible;

        public Shading()
        {
            this.visible = NBool.NullValue;
            this.color = MigraDoc.DocumentObjectModel.Color.Empty;
        }

        internal Shading(DocumentObject parent) : base(parent)
        {
            this.visible = NBool.NullValue;
            this.color = MigraDoc.DocumentObjectModel.Color.Empty;
        }

        public void Clear()
        {
            this.isCleared = true;
        }

        public Shading Clone() => 
            ((Shading) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            if (this.isCleared)
            {
                serializer.WriteLine("Shading = null");
            }
            int pos = serializer.BeginContent("Shading");
            if (!this.visible.IsNull)
            {
                serializer.WriteSimpleAttribute("Visible", this.Visible);
            }
            if (!this.color.IsNull)
            {
                serializer.WriteSimpleAttribute("Color", this.Color);
            }
            serializer.EndContent(pos);
        }

        public MigraDoc.DocumentObjectModel.Color Color
        {
            get => 
                this.color;
            set
            {
                this.color = value;
            }
        }

        public bool IsCleared =>
            this.isCleared;

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Shading));
                }
                return meta;
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
    }
}

