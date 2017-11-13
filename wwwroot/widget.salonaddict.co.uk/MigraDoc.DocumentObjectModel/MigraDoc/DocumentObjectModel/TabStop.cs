namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class TabStop : DocumentObject
    {
        internal bool AddTab;
        [DV(Type=typeof(TabAlignment))]
        internal NEnum alignment;
        [DV(Type=typeof(TabLeader))]
        internal NEnum leader;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal Unit position;

        public TabStop()
        {
            this.position = Unit.NullValue;
            this.alignment = NEnum.NullValue(typeof(TabAlignment));
            this.leader = NEnum.NullValue(typeof(TabLeader));
            this.AddTab = true;
        }

        internal TabStop(DocumentObject parent) : base(parent)
        {
            this.position = Unit.NullValue;
            this.alignment = NEnum.NullValue(typeof(TabAlignment));
            this.leader = NEnum.NullValue(typeof(TabLeader));
            this.AddTab = true;
        }

        public TabStop(Unit position) : this()
        {
            this.position = position;
        }

        public TabStop Clone() => 
            ((TabStop) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            if (this.AddTab)
            {
                serializer.WriteLine("TabStops +=");
                serializer.BeginContent();
                serializer.WriteSimpleAttribute("Position", this.Position);
                if (!this.alignment.IsNull)
                {
                    serializer.WriteSimpleAttribute("Alignment", this.Alignment);
                }
                if (!this.leader.IsNull)
                {
                    serializer.WriteSimpleAttribute("Leader", this.Leader);
                }
                serializer.EndContent();
            }
            else
            {
                serializer.WriteLine("TabStops -= \"" + this.Position.ToString() + "\"");
            }
        }

        public TabAlignment Alignment
        {
            get => 
                ((TabAlignment) this.alignment.Value);
            set
            {
                this.alignment.Value = (int) value;
            }
        }

        public TabLeader Leader
        {
            get => 
                ((TabLeader) this.leader.Value);
            set
            {
                this.leader.Value = (int) value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(TabStop));
                }
                return meta;
            }
        }

        public Unit Position =>
            this.position;
    }
}

