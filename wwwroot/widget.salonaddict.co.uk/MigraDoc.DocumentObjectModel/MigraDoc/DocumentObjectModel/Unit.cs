namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Unit : IFormattable, INullableValue
    {
        public static readonly Unit Empty;
        public static readonly Unit Zero;
        internal static readonly Unit NullValue;
        private bool initialized;
        private float value;
        private UnitType type;
        public Unit(double point)
        {
            this.value = (float) point;
            this.type = UnitType.Point;
            this.initialized = true;
        }

        public Unit(double value, UnitType type)
        {
            if (!Enum.IsDefined(typeof(UnitType), type))
            {
                throw new InvalidEnumArgumentException("type", (int) type, type.GetType());
            }
            this.value = (float) value;
            this.type = type;
            this.initialized = true;
        }

        public bool IsEmpty =>
            this.IsNull;
        object INullableValue.GetValue() => 
            this;

        void INullableValue.SetValue(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value is Unit)
            {
                this = (Unit) value;
            }
            else
            {
                this = value.ToString();
            }
        }

        void INullableValue.SetNull()
        {
            this.value = 0f;
            this.type = UnitType.Point;
            this.initialized = false;
        }

        bool INullableValue.IsNull =>
            this.IsNull;
        internal bool IsNull =>
            !this.initialized;
        public double Value
        {
            get => 
                (this.IsNull ? ((double) 0f) : ((double) this.value));
            set
            {
                this.value = (float) value;
                this.initialized = true;
            }
        }
        public UnitType Type =>
            this.type;
        public double Point
        {
            get
            {
                if (!this.IsNull)
                {
                    switch (this.type)
                    {
                        case UnitType.Point:
                            return (double) this.value;

                        case UnitType.Centimeter:
                            return (((double) (this.value * 72f)) / 2.54);

                        case UnitType.Inch:
                            return (double) (this.value * 72f);

                        case UnitType.Millimeter:
                            return (((double) (this.value * 72f)) / 25.4);

                        case UnitType.Pica:
                            return (double) (this.value * 12f);
                    }
                }
                return 0.0;
            }
            set
            {
                this.value = (float) value;
                this.type = UnitType.Point;
                this.initialized = true;
            }
        }
        [Obsolete("Use Point")]
        public double Pt
        {
            get => 
                this.Point;
            set
            {
                this.Point = value;
            }
        }
        public double Centimeter
        {
            get
            {
                if (!this.IsNull)
                {
                    switch (this.type)
                    {
                        case UnitType.Point:
                            return ((this.value * 2.54) / 72.0);

                        case UnitType.Centimeter:
                            return (double) this.value;

                        case UnitType.Inch:
                            return (this.value * 2.54);

                        case UnitType.Millimeter:
                            return (double) (this.value / 10f);

                        case UnitType.Pica:
                            return (((this.value * 12f) * 2.54) / 72.0);
                    }
                }
                return 0.0;
            }
            set
            {
                this.value = (float) value;
                this.type = UnitType.Centimeter;
                this.initialized = true;
            }
        }
        [Obsolete("Use Centimeter")]
        public double Cm
        {
            get => 
                this.Centimeter;
            set
            {
                this.Centimeter = value;
            }
        }
        public double Inch
        {
            get
            {
                if (!this.IsNull)
                {
                    switch (this.type)
                    {
                        case UnitType.Point:
                            return (double) (this.value / 72f);

                        case UnitType.Centimeter:
                            return (((double) this.value) / 2.54);

                        case UnitType.Inch:
                            return (double) this.value;

                        case UnitType.Millimeter:
                            return (((double) this.value) / 25.4);

                        case UnitType.Pica:
                            return (double) ((this.value * 12f) / 72f);
                    }
                }
                return 0.0;
            }
            set
            {
                this.value = (float) value;
                this.type = UnitType.Inch;
                this.initialized = true;
            }
        }
        [Obsolete("Use Inch")]
        public double In
        {
            get => 
                this.Inch;
            set
            {
                this.Inch = value;
            }
        }
        public double Millimeter
        {
            get
            {
                if (!this.IsNull)
                {
                    switch (this.type)
                    {
                        case UnitType.Point:
                            return ((this.value * 25.4) / 72.0);

                        case UnitType.Centimeter:
                            return (double) (this.value * 10f);

                        case UnitType.Inch:
                            return (this.value * 25.4);

                        case UnitType.Millimeter:
                            return (double) this.value;

                        case UnitType.Pica:
                            return (((this.value * 12f) * 25.4) / 72.0);
                    }
                }
                return 0.0;
            }
            set
            {
                this.value = (float) value;
                this.type = UnitType.Millimeter;
                this.initialized = true;
            }
        }
        [Obsolete("Use Millimeter")]
        public double Mm
        {
            get => 
                this.Millimeter;
            set
            {
                this.Millimeter = value;
            }
        }
        public double Pica
        {
            get
            {
                if (!this.IsNull)
                {
                    switch (this.type)
                    {
                        case UnitType.Point:
                            return (double) (this.value / 12f);

                        case UnitType.Centimeter:
                            return ((((double) (this.value * 72f)) / 2.54) / 12.0);

                        case UnitType.Inch:
                            return (double) ((this.value * 72f) / 12f);

                        case UnitType.Millimeter:
                            return ((((double) (this.value * 72f)) / 25.4) / 12.0);

                        case UnitType.Pica:
                            return (double) this.value;
                    }
                }
                return 0.0;
            }
            set
            {
                this.value = (float) value;
                this.type = UnitType.Pica;
                this.initialized = true;
            }
        }
        [Obsolete("Use Pica")]
        public double Pc
        {
            get => 
                this.Pica;
            set
            {
                this.Pica = value;
            }
        }
        public string ToString(IFormatProvider formatProvider)
        {
            if (this.IsNull)
            {
                int num = 0;
                return num.ToString(formatProvider);
            }
            return (this.value.ToString(formatProvider) + this.GetSuffix());
        }

        public string ToString(string format)
        {
            if (this.IsNull)
            {
                int num = 0;
                return num.ToString(format);
            }
            return (this.value.ToString(format) + this.GetSuffix());
        }

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            if (this.IsNull)
            {
                int num = 0;
                return num.ToString(format, formatProvider);
            }
            return (this.value.ToString(format, formatProvider) + this.GetSuffix());
        }

        public override string ToString()
        {
            if (this.IsNull)
            {
                int num = 0;
                return num.ToString(CultureInfo.InvariantCulture);
            }
            return (this.value.ToString(CultureInfo.InvariantCulture) + this.GetSuffix());
        }

        private string GetSuffix()
        {
            switch (this.type)
            {
                case UnitType.Point:
                    return "";

                case UnitType.Centimeter:
                    return "cm";

                case UnitType.Inch:
                    return "in";

                case UnitType.Millimeter:
                    return "mm";

                case UnitType.Pica:
                    return "pc";
            }
            return "";
        }

        public static Unit FromCentimeter(double value)
        {
            Unit zero = Zero;
            zero.value = (float) value;
            zero.type = UnitType.Centimeter;
            return zero;
        }

        [Obsolete("Use FromCentimer")]
        public static Unit FromCm(double value) => 
            FromCentimeter(value);

        public static Unit FromMillimeter(double value)
        {
            Unit zero = Zero;
            zero.value = (float) value;
            zero.type = UnitType.Millimeter;
            return zero;
        }

        [Obsolete("Use FromMillimeter")]
        public static Unit FromMm(double value) => 
            FromMillimeter(value);

        public static Unit FromPoint(double value)
        {
            Unit zero = Zero;
            zero.value = (float) value;
            zero.type = UnitType.Point;
            return zero;
        }

        public static Unit FromInch(double value)
        {
            Unit zero = Zero;
            zero.value = (float) value;
            zero.type = UnitType.Inch;
            return zero;
        }

        public static Unit FromPica(double value)
        {
            Unit zero = Zero;
            zero.value = (float) value;
            zero.type = UnitType.Pica;
            return zero;
        }

        public static implicit operator Unit(string value)
        {
            Unit zero = Zero;
            value = value.Trim();
            value = value.Replace(',', '.');
            int length = value.Length;
            int num2 = 0;
            while (num2 < length)
            {
                char c = value[num2];
                if (((c != '.') && (c != '-')) && ((c != '+') && !char.IsNumber(c)))
                {
                    break;
                }
                num2++;
            }
            zero.value = 1f;
            try
            {
                zero.value = float.Parse(value.Substring(0, num2).Trim(), CultureInfo.InvariantCulture);
            }
            catch (FormatException exception)
            {
                throw new ArgumentException(DomSR.InvalidUnitValue(value), exception);
            }
            string unitType = value.Substring(num2).Trim().ToLower();
            zero.type = UnitType.Point;
            switch (unitType)
            {
                case "cm":
                    zero.type = UnitType.Centimeter;
                    return zero;

                case "in":
                    zero.type = UnitType.Inch;
                    return zero;

                case "mm":
                    zero.type = UnitType.Millimeter;
                    return zero;

                case "pc":
                    zero.type = UnitType.Pica;
                    return zero;

                case "":
                case "pt":
                    zero.type = UnitType.Point;
                    return zero;
            }
            throw new ArgumentException(DomSR.InvalidUnitType(unitType));
        }

        public static implicit operator Unit(int value)
        {
            Unit zero = Zero;
            zero.value = value;
            zero.type = UnitType.Point;
            return zero;
        }

        public static implicit operator Unit(float value)
        {
            Unit zero = Zero;
            zero.value = value;
            zero.type = UnitType.Point;
            return zero;
        }

        public static implicit operator Unit(double value)
        {
            Unit zero = Zero;
            zero.value = (float) value;
            zero.type = UnitType.Point;
            return zero;
        }

        public static implicit operator double(Unit value) => 
            value.Point;

        public static implicit operator float(Unit value) => 
            ((float) value.Point);

        public static bool operator ==(Unit l, Unit r) => 
            (((l.initialized == r.initialized) && (l.type == r.type)) && (l.value == r.value));

        public static bool operator !=(Unit l, Unit r) => 
            !(l == r);

        public override bool Equals(object obj) => 
            base.Equals(obj);

        public override int GetHashCode() => 
            base.GetHashCode();

        public static Unit Parse(string value) => 
            value;

        public void ConvertType(UnitType type)
        {
            if (this.type != type)
            {
                if (!Enum.IsDefined(typeof(UnitType), type))
                {
                    throw new ArgumentException(DomSR.InvalidUnitType(type.ToString()));
                }
                switch (type)
                {
                    case UnitType.Point:
                        this.value = (float) this.Point;
                        this.type = UnitType.Point;
                        return;

                    case UnitType.Centimeter:
                        this.value = (float) this.Centimeter;
                        this.type = UnitType.Centimeter;
                        return;

                    case UnitType.Inch:
                        this.value = (float) this.Inch;
                        this.type = UnitType.Inch;
                        return;

                    case UnitType.Millimeter:
                        this.value = (float) this.Millimeter;
                        this.type = UnitType.Millimeter;
                        return;

                    case UnitType.Pica:
                        this.value = (float) this.Pica;
                        this.type = UnitType.Pica;
                        return;
                }
            }
        }

        static Unit()
        {
            Empty = new Unit();
            Zero = new Unit(0.0);
            NullValue = Empty;
        }
    }
}

