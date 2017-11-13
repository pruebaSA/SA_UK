namespace MigraDoc.DocumentObjectModel.Shapes
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TopPosition : INullableValue
    {
        internal MigraDoc.DocumentObjectModel.Shapes.ShapePosition shapePosition;
        internal Unit position;
        private bool notNull;
        internal static readonly TopPosition NullValue;
        private TopPosition(Unit value)
        {
            this.shapePosition = MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Undefined;
            this.position = value;
            this.notNull = !value.IsNull;
        }

        private TopPosition(MigraDoc.DocumentObjectModel.Shapes.ShapePosition value)
        {
            if (!IsValid(value) && (value != MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Undefined))
            {
                throw new ArgumentException(DomSR.InvalidEnumForTopPosition);
            }
            this.shapePosition = value;
            this.position = Unit.NullValue;
            this.notNull = value != MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Undefined;
        }

        private static bool IsValid(MigraDoc.DocumentObjectModel.Shapes.ShapePosition shapePosition)
        {
            if ((shapePosition != MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Bottom) && (shapePosition != MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Top))
            {
                return (shapePosition == MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Center);
            }
            return true;
        }

        public static implicit operator TopPosition(MigraDoc.DocumentObjectModel.Shapes.ShapePosition value) => 
            new TopPosition(value);

        public static implicit operator TopPosition(Unit val) => 
            new TopPosition(val);

        public static implicit operator TopPosition(string value) => 
            new TopPosition(value);

        public static implicit operator TopPosition(double value) => 
            new TopPosition(value);

        public static implicit operator TopPosition(int value) => 
            new TopPosition(value);

        private void SetFromEnum(MigraDoc.DocumentObjectModel.Shapes.ShapePosition shapePosition)
        {
            if (!IsValid(shapePosition))
            {
                throw new ArgumentException(DomSR.InvalidEnumForTopPosition);
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
            this = new TopPosition();
        }

        bool INullableValue.IsNull =>
            !this.notNull;
        public Unit Position =>
            this.position;
        public MigraDoc.DocumentObjectModel.Shapes.ShapePosition ShapePosition =>
            this.shapePosition;
        public static TopPosition Parse(string value)
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
                serializer.WriteSimpleAttribute("Top", this.Position);
            }
            else
            {
                serializer.WriteSimpleAttribute("Top", this.ShapePosition);
            }
        }

        static TopPosition()
        {
            NullValue = new TopPosition();
        }
    }
}

