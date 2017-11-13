namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;
    using System.Collections;
    using System.ComponentModel;

    public class Borders : DocumentObject, IEnumerable
    {
        [DV]
        internal Border bottom;
        protected bool clearAll;
        [DV]
        internal MigraDoc.DocumentObjectModel.Color color;
        [DV]
        internal Border diagonalDown;
        [DV]
        internal Border diagonalUp;
        [DV]
        internal Unit distanceFromBottom;
        [DV]
        internal Unit distanceFromLeft;
        [DV]
        internal Unit distanceFromRight;
        [DV]
        internal Unit distanceFromTop;
        [DV]
        internal Border left;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal Border right;
        [DV(Type=typeof(BorderStyle))]
        internal NEnum style;
        [DV]
        internal Border top;
        [DV]
        internal NBool visible;
        [DV]
        internal Unit width;

        public Borders()
        {
            this.visible = NBool.NullValue;
            this.style = NEnum.NullValue(typeof(BorderStyle));
            this.width = Unit.NullValue;
            this.color = MigraDoc.DocumentObjectModel.Color.Empty;
            this.distanceFromTop = Unit.NullValue;
            this.distanceFromBottom = Unit.NullValue;
            this.distanceFromLeft = Unit.NullValue;
            this.distanceFromRight = Unit.NullValue;
        }

        internal Borders(DocumentObject parent) : base(parent)
        {
            this.visible = NBool.NullValue;
            this.style = NEnum.NullValue(typeof(BorderStyle));
            this.width = Unit.NullValue;
            this.color = MigraDoc.DocumentObjectModel.Color.Empty;
            this.distanceFromTop = Unit.NullValue;
            this.distanceFromBottom = Unit.NullValue;
            this.distanceFromLeft = Unit.NullValue;
            this.distanceFromRight = Unit.NullValue;
        }

        public void ClearAll()
        {
            this.clearAll = true;
        }

        public Borders Clone() => 
            ((Borders) this.DeepCopy());

        protected override object DeepCopy()
        {
            Borders borders = (Borders) base.DeepCopy();
            if (borders.top != null)
            {
                borders.top = borders.top.Clone();
                borders.top.parent = borders;
            }
            if (borders.left != null)
            {
                borders.left = borders.left.Clone();
                borders.left.parent = borders;
            }
            if (borders.right != null)
            {
                borders.right = borders.right.Clone();
                borders.right.parent = borders;
            }
            if (borders.bottom != null)
            {
                borders.bottom = borders.bottom.Clone();
                borders.bottom.parent = borders;
            }
            if (borders.diagonalUp != null)
            {
                borders.diagonalUp = borders.diagonalUp.Clone();
                borders.diagonalUp.parent = borders;
            }
            if (borders.diagonalDown != null)
            {
                borders.diagonalDown = borders.diagonalDown.Clone();
                borders.diagonalDown.parent = borders;
            }
            return borders;
        }

        internal string GetMyName(Border border)
        {
            if (border == this.top)
            {
                return "Top";
            }
            if (border == this.bottom)
            {
                return "Bottom";
            }
            if (border == this.left)
            {
                return "Left";
            }
            if (border == this.right)
            {
                return "Right";
            }
            if (border == this.diagonalUp)
            {
                return "DiagonalUp";
            }
            if (border == this.diagonalDown)
            {
                return "DiagonalDown";
            }
            return null;
        }

        public bool HasBorder(BorderType type)
        {
            if (!Enum.IsDefined(typeof(BorderType), type))
            {
                throw new InvalidEnumArgumentException("type");
            }
            return !this.IsNull(type.ToString());
        }

        internal override void Serialize(Serializer serializer)
        {
            this.Serialize(serializer, null);
        }

        internal void Serialize(Serializer serializer, Borders refBorders)
        {
            if (this.clearAll)
            {
                serializer.WriteLine("Borders = null");
            }
            int pos = serializer.BeginContent("Borders");
            if (!this.visible.IsNull && (((refBorders == null) || refBorders.visible.IsNull) || (this.Visible != refBorders.Visible)))
            {
                serializer.WriteSimpleAttribute("Visible", this.Visible);
            }
            if (!this.style.IsNull && ((refBorders == null) || (this.Style != refBorders.Style)))
            {
                serializer.WriteSimpleAttribute("Style", this.Style);
            }
            if (!this.width.IsNull && ((refBorders == null) || (this.width.Value != refBorders.width.Value)))
            {
                serializer.WriteSimpleAttribute("Width", this.Width);
            }
            if (!this.color.IsNull && ((refBorders == null) || (this.Color.Argb != refBorders.Color.Argb)))
            {
                serializer.WriteSimpleAttribute("Color", this.Color);
            }
            if (!this.distanceFromTop.IsNull && ((refBorders == null) || (this.DistanceFromTop.Point != refBorders.DistanceFromTop.Point)))
            {
                serializer.WriteSimpleAttribute("DistanceFromTop", this.DistanceFromTop);
            }
            if (!this.distanceFromBottom.IsNull && ((refBorders == null) || (this.DistanceFromBottom.Point != refBorders.DistanceFromBottom.Point)))
            {
                serializer.WriteSimpleAttribute("DistanceFromBottom", this.DistanceFromBottom);
            }
            if (!this.distanceFromLeft.IsNull && ((refBorders == null) || (this.DistanceFromLeft.Point != refBorders.DistanceFromLeft.Point)))
            {
                serializer.WriteSimpleAttribute("DistanceFromLeft", this.DistanceFromLeft);
            }
            if (!this.distanceFromRight.IsNull && ((refBorders == null) || (this.DistanceFromRight.Point != refBorders.DistanceFromRight.Point)))
            {
                serializer.WriteSimpleAttribute("DistanceFromRight", this.DistanceFromRight);
            }
            if (!this.IsNull("Top"))
            {
                this.top.Serialize(serializer, "Top", null);
            }
            if (!this.IsNull("Left"))
            {
                this.left.Serialize(serializer, "Left", null);
            }
            if (!this.IsNull("Bottom"))
            {
                this.bottom.Serialize(serializer, "Bottom", null);
            }
            if (!this.IsNull("Right"))
            {
                this.right.Serialize(serializer, "Right", null);
            }
            if (!this.IsNull("DiagonalDown"))
            {
                this.diagonalDown.Serialize(serializer, "DiagonalDown", null);
            }
            if (!this.IsNull("DiagonalUp"))
            {
                this.diagonalUp.Serialize(serializer, "DiagonalUp", null);
            }
            serializer.EndContent(pos);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            new BorderEnumerator(new Hashtable { 
                { 
                    "Top",
                    this.top
                },
                { 
                    "Left",
                    this.left
                },
                { 
                    "Bottom",
                    this.bottom
                },
                { 
                    "Right",
                    this.right
                },
                { 
                    "DiagonalUp",
                    this.diagonalUp
                },
                { 
                    "DiagonalDown",
                    this.diagonalDown
                }
            });

        public bool BordersCleared
        {
            get => 
                this.clearAll;
            set
            {
                this.clearAll = value;
            }
        }

        public Border Bottom
        {
            get
            {
                if (this.bottom == null)
                {
                    this.bottom = new Border(this);
                }
                return this.bottom;
            }
            set
            {
                base.SetParent(value);
                this.bottom = value;
            }
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

        public Border DiagonalDown
        {
            get
            {
                if (this.diagonalDown == null)
                {
                    this.diagonalDown = new Border(this);
                }
                return this.diagonalDown;
            }
            set
            {
                base.SetParent(value);
                this.diagonalDown = value;
            }
        }

        public Border DiagonalUp
        {
            get
            {
                if (this.diagonalUp == null)
                {
                    this.diagonalUp = new Border(this);
                }
                return this.diagonalUp;
            }
            set
            {
                base.SetParent(value);
                this.diagonalUp = value;
            }
        }

        public Unit Distance
        {
            set
            {
                this.DistanceFromTop = value;
                this.DistanceFromBottom = value;
                this.DistanceFromLeft = value;
                this.distanceFromRight = value;
            }
        }

        public Unit DistanceFromBottom
        {
            get => 
                this.distanceFromBottom;
            set
            {
                this.distanceFromBottom = value;
            }
        }

        public Unit DistanceFromLeft
        {
            get => 
                this.distanceFromLeft;
            set
            {
                this.distanceFromLeft = value;
            }
        }

        public Unit DistanceFromRight
        {
            get => 
                this.distanceFromRight;
            set
            {
                this.distanceFromRight = value;
            }
        }

        public Unit DistanceFromTop
        {
            get => 
                this.distanceFromTop;
            set
            {
                this.distanceFromTop = value;
            }
        }

        public Border Left
        {
            get
            {
                if (this.left == null)
                {
                    this.left = new Border(this);
                }
                return this.left;
            }
            set
            {
                base.SetParent(value);
                this.left = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Borders));
                }
                return meta;
            }
        }

        public Border Right
        {
            get
            {
                if (this.right == null)
                {
                    this.right = new Border(this);
                }
                return this.right;
            }
            set
            {
                base.SetParent(value);
                this.right = value;
            }
        }

        public BorderStyle Style
        {
            get => 
                ((BorderStyle) this.style.Value);
            set
            {
                this.style.Value = (int) value;
            }
        }

        public Border Top
        {
            get
            {
                if (this.top == null)
                {
                    this.top = new Border(this);
                }
                return this.top;
            }
            set
            {
                base.SetParent(value);
                this.top = value;
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

        public class BorderEnumerator : IEnumerator
        {
            private Hashtable ht;
            private int index;

            public BorderEnumerator(Hashtable ht)
            {
                this.ht = ht;
                this.index = -1;
            }

            public bool MoveNext()
            {
                this.index++;
                return (this.index < this.ht.Count);
            }

            public void Reset()
            {
                this.index = -1;
            }

            public Border Current
            {
                get
                {
                    IEnumerator enumerator = this.ht.GetEnumerator();
                    enumerator.Reset();
                    for (int i = 0; i < (this.index + 1); i++)
                    {
                        enumerator.MoveNext();
                    }
                    DictionaryEntry current = (DictionaryEntry) enumerator.Current;
                    return (current.Value as Border);
                }
            }

            object IEnumerator.Current =>
                this.Current;
        }
    }
}

