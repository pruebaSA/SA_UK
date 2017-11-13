namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;
    using System.Reflection;

    public class TabStops : DocumentObjectCollection
    {
        internal bool fClearAll;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        public static readonly double TabStopPrecision = 1.5;

        public TabStops()
        {
        }

        internal TabStops(DocumentObject parent) : base(parent)
        {
        }

        public TabStop AddTabStop(TabStop tabStop)
        {
            if (tabStop == null)
            {
                throw new ArgumentNullException("tabStop");
            }
            if (this.TabStopExists(tabStop.Position))
            {
                int index = base.IndexOf(this.GetTabStopAt(tabStop.Position));
                base.RemoveObjectAt(index);
                this.InsertObject(index, tabStop);
                return tabStop;
            }
            int count = base.Count;
            for (int i = 0; i < count; i++)
            {
                if (tabStop.Position.Point < this[i].Position.Point)
                {
                    this.InsertObject(i, tabStop);
                    return tabStop;
                }
            }
            this.Add(tabStop);
            return tabStop;
        }

        public TabStop AddTabStop(Unit position)
        {
            if (this.TabStopExists(position))
            {
                return this.GetTabStopAt(position);
            }
            TabStop tabStop = new TabStop(position);
            return this.AddTabStop(tabStop);
        }

        public TabStop AddTabStop(Unit position, TabAlignment alignment)
        {
            TabStop stop = this.AddTabStop(position);
            stop.Alignment = alignment;
            return stop;
        }

        public TabStop AddTabStop(Unit position, TabLeader leader)
        {
            TabStop stop = this.AddTabStop(position);
            stop.Leader = leader;
            return stop;
        }

        public TabStop AddTabStop(Unit position, TabAlignment alignment, TabLeader leader)
        {
            TabStop stop = this.AddTabStop(position);
            stop.Alignment = alignment;
            stop.Leader = leader;
            return stop;
        }

        public void ClearAll()
        {
            base.Clear();
            this.fClearAll = true;
        }

        public TabStops Clone() => 
            ((TabStops) this.DeepCopy());

        public TabStop GetTabStopAt(Unit position)
        {
            int count = base.Count;
            for (int i = 0; i < count; i++)
            {
                TabStop stop = this[i];
                if (Math.Abs((double) (stop.Position.Point - position.Point)) < TabStopPrecision)
                {
                    return stop;
                }
            }
            return null;
        }

        public override bool IsNull() => 
            (base.IsNull() && !this.fClearAll);

        public void RemoveTabStop(Unit position)
        {
            this.AddTabStop(position).AddTab = false;
        }

        internal override void Serialize(Serializer serializer)
        {
            if (this.fClearAll)
            {
                serializer.WriteLine("TabStops = null");
            }
            int count = base.Count;
            for (int i = 0; i < count; i++)
            {
                this[i].Serialize(serializer);
            }
        }

        public bool TabStopExists(Unit position) => 
            (this.GetTabStopAt(position) != null);

        public TabStop this[int index] =>
            (base[index] as TabStop);

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(TabStops));
                }
                return meta;
            }
        }

        public bool TabsCleared =>
            this.fClearAll;
    }
}

