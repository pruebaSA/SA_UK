namespace MigraDoc.DocumentObjectModel.Shapes
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class FillFormat : DocumentObject
    {
        [DV]
        internal MigraDoc.DocumentObjectModel.Color color;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NBool visible;

        public FillFormat()
        {
            this.color = MigraDoc.DocumentObjectModel.Color.Empty;
            this.visible = NBool.NullValue;
        }

        internal FillFormat(DocumentObject parent) : base(parent)
        {
            this.color = MigraDoc.DocumentObjectModel.Color.Empty;
            this.visible = NBool.NullValue;
        }

        public FillFormat Clone() => 
            ((FillFormat) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            serializer.BeginContent("FillFormat");
            if (!this.visible.IsNull)
            {
                serializer.WriteSimpleAttribute("Visible", this.Visible);
            }
            if (!this.color.IsNull)
            {
                serializer.WriteSimpleAttribute("Color", this.Color);
            }
            serializer.EndContent();
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

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(FillFormat));
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

