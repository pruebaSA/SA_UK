namespace PdfSharp.Drawing
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct XUnit : IFormattable
    {
        internal const double PointFactor = 1.0;
        internal const double InchFactor = 72.0;
        internal const double MillimeterFactor = 2.8346456692913389;
        internal const double CentimeterFactor = 28.346456692913385;
        internal const double PresentationFactor = 0.75;
        internal const double PointFactorWpf = 1.3333333333333333;
        internal const double InchFactorWpf = 96.0;
        internal const double MillimeterFactorWpf = 3.7795275590551185;
        internal const double CentimeterFactorWpf = 37.795275590551178;
        internal const double PresentationFactorWpf = 1.0;
        public static readonly XUnit Zero;
        private double value;
        private XGraphicsUnit type;
        public XUnit(double point)
        {
            this.value = point;
            this.type = XGraphicsUnit.Point;
        }

        public XUnit(double value, XGraphicsUnit type)
        {
            if (!Enum.IsDefined(typeof(XGraphicsUnit), type))
            {
                throw new InvalidEnumArgumentException("type");
            }
            this.value = value;
            this.type = type;
        }

        public double Value =>
            this.value;
        public XGraphicsUnit Type =>
            this.type;
        public double Point
        {
            get
            {
                switch (this.type)
                {
                    case XGraphicsUnit.Point:
                        return this.value;

                    case XGraphicsUnit.Inch:
                        return (this.value * 72.0);

                    case XGraphicsUnit.Millimeter:
                        return ((this.value * 72.0) / 25.4);

                    case XGraphicsUnit.Centimeter:
                        return ((this.value * 72.0) / 2.54);

                    case XGraphicsUnit.Presentation:
                        return ((this.value * 72.0) / 96.0);
                }
                throw new InvalidCastException();
            }
            set
            {
                this.value = value;
                this.type = XGraphicsUnit.Point;
            }
        }
        public double Inch
        {
            get
            {
                switch (this.type)
                {
                    case XGraphicsUnit.Point:
                        return (this.value / 72.0);

                    case XGraphicsUnit.Inch:
                        return this.value;

                    case XGraphicsUnit.Millimeter:
                        return (this.value / 25.4);

                    case XGraphicsUnit.Centimeter:
                        return (this.value / 2.54);

                    case XGraphicsUnit.Presentation:
                        return (this.value / 96.0);
                }
                throw new InvalidCastException();
            }
            set
            {
                this.value = value;
                this.type = XGraphicsUnit.Inch;
            }
        }
        public double Millimeter
        {
            get
            {
                switch (this.type)
                {
                    case XGraphicsUnit.Point:
                        return ((this.value * 25.4) / 72.0);

                    case XGraphicsUnit.Inch:
                        return (this.value * 25.4);

                    case XGraphicsUnit.Millimeter:
                        return this.value;

                    case XGraphicsUnit.Centimeter:
                        return (this.value * 10.0);

                    case XGraphicsUnit.Presentation:
                        return ((this.value * 25.4) / 96.0);
                }
                throw new InvalidCastException();
            }
            set
            {
                this.value = value;
                this.type = XGraphicsUnit.Millimeter;
            }
        }
        public double Centimeter
        {
            get
            {
                switch (this.type)
                {
                    case XGraphicsUnit.Point:
                        return ((this.value * 2.54) / 72.0);

                    case XGraphicsUnit.Inch:
                        return (this.value * 2.54);

                    case XGraphicsUnit.Millimeter:
                        return (this.value / 10.0);

                    case XGraphicsUnit.Centimeter:
                        return this.value;

                    case XGraphicsUnit.Presentation:
                        return ((this.value * 2.54) / 96.0);
                }
                throw new InvalidCastException();
            }
            set
            {
                this.value = value;
                this.type = XGraphicsUnit.Centimeter;
            }
        }
        public double Presentation
        {
            get
            {
                switch (this.type)
                {
                    case XGraphicsUnit.Point:
                        return ((this.value * 96.0) / 72.0);

                    case XGraphicsUnit.Inch:
                        return (this.value * 96.0);

                    case XGraphicsUnit.Millimeter:
                        return ((this.value * 96.0) / 25.4);

                    case XGraphicsUnit.Centimeter:
                        return ((this.value * 96.0) / 2.54);

                    case XGraphicsUnit.Presentation:
                        return this.value;
                }
                throw new InvalidCastException();
            }
            set
            {
                this.value = value;
                this.type = XGraphicsUnit.Point;
            }
        }
        public string ToString(IFormatProvider formatProvider) => 
            (this.value.ToString(formatProvider) + this.GetSuffix());

        string IFormattable.ToString(string format, IFormatProvider formatProvider) => 
            (this.value.ToString(format, formatProvider) + this.GetSuffix());

        public override string ToString() => 
            (this.value.ToString(CultureInfo.InvariantCulture) + this.GetSuffix());

        private string GetSuffix()
        {
            switch (this.type)
            {
                case XGraphicsUnit.Point:
                    return "pt";

                case XGraphicsUnit.Inch:
                    return "in";

                case XGraphicsUnit.Millimeter:
                    return "mm";

                case XGraphicsUnit.Centimeter:
                    return "cm";

                case XGraphicsUnit.Presentation:
                    return "pu";
            }
            throw new InvalidCastException();
        }

        public static XUnit FromPoint(double value)
        {
            XUnit unit;
            unit.value = value;
            unit.type = XGraphicsUnit.Point;
            return unit;
        }

        public static XUnit FromInch(double value)
        {
            XUnit unit;
            unit.value = value;
            unit.type = XGraphicsUnit.Inch;
            return unit;
        }

        public static XUnit FromMillimeter(double value)
        {
            XUnit unit;
            unit.value = value;
            unit.type = XGraphicsUnit.Millimeter;
            return unit;
        }

        public static XUnit FromCentimeter(double value)
        {
            XUnit unit;
            unit.value = value;
            unit.type = XGraphicsUnit.Centimeter;
            return unit;
        }

        public static XUnit FromPresentation(double value)
        {
            XUnit unit;
            unit.value = value;
            unit.type = XGraphicsUnit.Presentation;
            return unit;
        }

        public static implicit operator XUnit(string value)
        {
            XUnit unit;
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
            try
            {
                unit.value = double.Parse(value.Substring(0, num2).Trim(), CultureInfo.InvariantCulture);
            }
            catch (Exception exception)
            {
                unit.value = 1.0;
                throw new ArgumentException($"String '{value}' is not a valid value for structure 'XUnit'.", exception);
            }
            string str2 = value.Substring(num2).Trim().ToLower();
            unit.type = XGraphicsUnit.Point;
            switch (str2)
            {
                case "cm":
                    unit.type = XGraphicsUnit.Centimeter;
                    return unit;

                case "in":
                    unit.type = XGraphicsUnit.Inch;
                    return unit;

                case "mm":
                    unit.type = XGraphicsUnit.Millimeter;
                    return unit;

                case "":
                case "pt":
                    unit.type = XGraphicsUnit.Point;
                    return unit;

                case "pu":
                    unit.type = XGraphicsUnit.Presentation;
                    return unit;
            }
            throw new ArgumentException("Unknown unit type: '" + str2 + "'");
        }

        public static implicit operator XUnit(int value)
        {
            XUnit unit;
            unit.value = value;
            unit.type = XGraphicsUnit.Point;
            return unit;
        }

        public static implicit operator XUnit(double value)
        {
            XUnit unit;
            unit.value = value;
            unit.type = XGraphicsUnit.Point;
            return unit;
        }

        public static implicit operator double(XUnit value) => 
            value.Point;

        public static bool operator ==(XUnit value1, XUnit value2) => 
            ((value1.type == value2.type) && (value1.value == value2.value));

        public static bool operator !=(XUnit value1, XUnit value2) => 
            !(value1 == value2);

        public override bool Equals(object obj) => 
            ((obj is XUnit) && (this == ((XUnit) obj)));

        public override int GetHashCode() => 
            (this.value.GetHashCode() ^ this.type.GetHashCode());

        public static XUnit Parse(string value) => 
            value;

        public void ConvertType(XGraphicsUnit type)
        {
            if (this.type != type)
            {
                switch (type)
                {
                    case XGraphicsUnit.Point:
                        this.value = this.Point;
                        this.type = XGraphicsUnit.Point;
                        return;

                    case XGraphicsUnit.Inch:
                        this.value = this.Inch;
                        this.type = XGraphicsUnit.Inch;
                        return;

                    case XGraphicsUnit.Millimeter:
                        this.value = this.Millimeter;
                        this.type = XGraphicsUnit.Millimeter;
                        return;

                    case XGraphicsUnit.Centimeter:
                        this.value = this.Centimeter;
                        this.type = XGraphicsUnit.Centimeter;
                        return;

                    case XGraphicsUnit.Presentation:
                        this.value = this.Presentation;
                        this.type = XGraphicsUnit.Presentation;
                        return;
                }
                throw new ArgumentException("Unknown unit type: '" + type + "'");
            }
        }
    }
}

