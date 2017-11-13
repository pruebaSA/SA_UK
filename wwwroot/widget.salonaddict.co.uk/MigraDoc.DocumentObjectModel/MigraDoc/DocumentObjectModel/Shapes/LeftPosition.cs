namespace MigraDoc.DocumentObjectModel.Shapes
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct LeftPosition : INullableValue
    {
        internal MigraDoc.DocumentObjectModel.Shapes.ShapePosition shapePosition;
        internal Unit position;
        private bool notNull;
        internal static readonly LeftPosition NullValue;
        private LeftPosition(Unit value)
        {
            this.shapePosition = MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Undefined;
            this.position = value;
            this.notNull = !value.IsNull;
        }

        private LeftPosition(MigraDoc.DocumentObjectModel.Shapes.ShapePosition value)
        {
            if ((value != MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Undefined) && !IsValid(value))
            {
                throw new ArgumentException(DomSR.InvalidEnumForLeftPosition);
            }
            this.shapePosition = value;
            this.position = Unit.NullValue;
            this.notNull = value != MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Undefined;
        }

        private void SetFromEnum(MigraDoc.DocumentObjectModel.Shapes.ShapePosition shapePosition)
        {
            if (!IsValid(shapePosition))
            {
                throw new ArgumentException(DomSR.InvalidEnumForLeftPosition);
            }
            this.shapePosition = shapePosition;
            this.position = Unit.NullValue;
        }

        private void SetFromUnit(Unit unit)
        {
            this.shapePosition = MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Undefined;
            this.position = unit;
        }

        void INullableValue.SetValue(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value is MigraDoc.DocumentObjectModel.Shapes.ShapePosition)
            {
                this.SetFromEnum((MigraDoc.DocumentObjectModel.Shapes.ShapePosition) value);
            }
            else if ((value is string) && Enum.IsDefined(typeof(MigraDoc.DocumentObjectModel.Shapes.ShapePosition), value))
            {
                this.SetFromEnum((MigraDoc.DocumentObjectModel.Shapes.ShapePosition) Enum.Parse(typeof(MigraDoc.DocumentObjectModel.Shapes.ShapePosition), (string) value));
            }
            else
            {
                this.SetFromUnit(value.ToString());
            }
            this.notNull = true;
        }

        object INullableValue.GetValue()
        {
            if (this.shapePosition == MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Undefined)
            {
                return this.position;
            }
            return this.shapePosition;
        }

        void INullableValue.SetNull()
        {
            this = new LeftPosition();
        }

        bool INullableValue.IsNull =>
            !this.notNull;
        public Unit Position =>
            this.position;
        public MigraDoc.DocumentObjectModel.Shapes.ShapePosition ShapePosition =>
            this.shapePosition;
        private static bool IsValid(MigraDoc.DocumentObjectModel.Shapes.ShapePosition shapePosition)
        {
            if (((shapePosition != MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Left) && (shapePosition != MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Center)) && ((shapePosition != MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Right) && (shapePosition != MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Inside)))
            {
                return (shapePosition == MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Outside);
            }
            return true;
        }

        public static implicit operator LeftPosition(MigraDoc.DocumentObjectModel.Shapes.ShapePosition value) => 
            new LeftPosition(value);

        public static implicit operator LeftPosition(Unit value) => 
            new LeftPosition(value);

        public static implicit operator LeftPosition(string value) => 
            new LeftPosition(value);

        public static implicit operator LeftPosition(double value) => 
            new LeftPosition(value);

        public static implicit operator LeftPosition(int value) => 
            new LeftPosition(value);

        public static LeftPosition Parse(string value)
        {
            if ((value == null) || (value.Length == 0))
            {
                throw new ArgumentNullException("value");
            }
            value = value.Trim();
            char c = value[0];
            if (((c != '+') && (c != '-')) && !char.IsNumber(c))
            {
                return (MigraDoc.DocumentObjectModel.Shapes.ShapePosition) Enum.Parse(typeof(MigraDoc.DocumentObjectModel.Shapes.ShapePosition), value, true);
            }
            return Unit.Parse(value);
        }

        internal void Serialize(Serializer serializer)
        {
            if (this.shapePosition == MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Undefined)
            {
                serializer.WriteSimpleAttribute("Left", this.Position);
            }
            else
            {
                serializer.WriteSimpleAttribute("Left", this.ShapePosition);
            }
        }

        static LeftPosition()
        {
            NullValue = new LeftPosition();
        }
    }
}

