﻿namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Globalization;
    using System.ServiceModel;

    internal static class QueryValueModel
    {
        internal static bool Boolean(double dblVal) => 
            ((dblVal != 0.0) && !double.IsNaN(dblVal));

        internal static bool Boolean(NodeSequence sequence) => 
            sequence.IsNotEmpty;

        internal static bool Boolean(string val) => 
            (val.Length > 0);

        internal static bool Compare(bool x, bool y, RelationOperator op)
        {
            switch (op)
            {
                case RelationOperator.Eq:
                    return (x == y);

                case RelationOperator.Ne:
                    return (x != y);
            }
            return Compare(Double(x), Double(y), op);
        }

        internal static bool Compare(bool x, double y, RelationOperator op)
        {
            switch (op)
            {
                case RelationOperator.Eq:
                    return (x == Boolean(y));

                case RelationOperator.Ne:
                    return (x != Boolean(y));
            }
            return Compare(Double(x), y, op);
        }

        internal static bool Compare(bool x, NodeSequence y, RelationOperator op) => 
            Compare(x, Boolean(y), op);

        internal static bool Compare(bool x, string y, RelationOperator op)
        {
            switch (op)
            {
                case RelationOperator.Eq:
                    return (x == Boolean(y));

                case RelationOperator.Ne:
                    return (x != Boolean(y));
            }
            return Compare(Double(x), Double(y), op);
        }

        internal static bool Compare(double x, bool y, RelationOperator op)
        {
            switch (op)
            {
                case RelationOperator.Eq:
                    return (Boolean(x) == y);

                case RelationOperator.Ne:
                    return (Boolean(x) != y);
            }
            return Compare(x, Double(y), op);
        }

        internal static bool Compare(double x, double y, RelationOperator op)
        {
            switch (op)
            {
                case RelationOperator.Eq:
                    return (x == y);

                case RelationOperator.Ne:
                    return (x != y);

                case RelationOperator.Gt:
                    return (x > y);

                case RelationOperator.Ge:
                    return (x >= y);

                case RelationOperator.Lt:
                    return (x < y);

                case RelationOperator.Le:
                    return (x <= y);
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperCritical(new QueryProcessingException(QueryProcessingError.TypeMismatch));
        }

        internal static bool Compare(double x, NodeSequence y, RelationOperator op)
        {
            switch (op)
            {
                case RelationOperator.Gt:
                    return y.Compare(x, RelationOperator.Lt);

                case RelationOperator.Ge:
                    return y.Compare(x, RelationOperator.Le);

                case RelationOperator.Lt:
                    return y.Compare(x, RelationOperator.Gt);

                case RelationOperator.Le:
                    return y.Compare(x, RelationOperator.Ge);
            }
            return y.Compare(x, op);
        }

        internal static bool Compare(double x, string y, RelationOperator op) => 
            Compare(x, Double(y), op);

        internal static bool Compare(NodeSequence x, bool y, RelationOperator op) => 
            Compare(Boolean(x), y, op);

        internal static bool Compare(NodeSequence x, double y, RelationOperator op) => 
            x.Compare(y, op);

        internal static bool Compare(NodeSequence x, NodeSequence y, RelationOperator op) => 
            x.Compare(y, op);

        internal static bool Compare(NodeSequence x, string y, RelationOperator op) => 
            x.Compare(y, op);

        internal static bool Compare(string x, bool y, RelationOperator op)
        {
            switch (op)
            {
                case RelationOperator.Eq:
                    return (y == Boolean(x));

                case RelationOperator.Ne:
                    return (y != Boolean(x));
            }
            return Compare(Double(x), Double(y), op);
        }

        internal static bool Compare(string x, double y, RelationOperator op) => 
            Compare(Double(x), y, op);

        internal static bool Compare(string x, NodeSequence y, RelationOperator op)
        {
            switch (op)
            {
                case RelationOperator.Gt:
                    return y.Compare(x, RelationOperator.Lt);

                case RelationOperator.Ge:
                    return y.Compare(x, RelationOperator.Le);

                case RelationOperator.Lt:
                    return y.Compare(x, RelationOperator.Gt);

                case RelationOperator.Le:
                    return y.Compare(x, RelationOperator.Ge);
            }
            return y.Compare(x, op);
        }

        internal static bool Compare(string x, string y, RelationOperator op)
        {
            switch (op)
            {
                case RelationOperator.Eq:
                    return Equals(x, y);

                case RelationOperator.Ne:
                    return ((x.Length != y.Length) || (0 != string.CompareOrdinal(x, y)));

                case RelationOperator.Gt:
                case RelationOperator.Ge:
                case RelationOperator.Lt:
                case RelationOperator.Le:
                    return Compare(Double(x), Double(y), op);
            }
            return false;
        }

        internal static bool CompileTimeCompare(object x, object y, RelationOperator op)
        {
            if (x is string)
            {
                if (y is double)
                {
                    return Compare((string) x, (double) y, op);
                }
                if (y is string)
                {
                    return Compare((string) x, (string) y, op);
                }
            }
            else if (x is double)
            {
                if (y is double)
                {
                    return Compare((double) x, (double) y, op);
                }
                if (y is string)
                {
                    return Compare((double) x, (string) y, op);
                }
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new QueryCompileException(QueryCompileError.InvalidComparison));
        }

        internal static double Double(bool val) => 
            (val ? ((double) 1) : ((double) 0));

        internal static double Double(NodeSequence sequence) => 
            Double(sequence.StringValue());

        internal static double Double(string val)
        {
            double num;
            val = val.TrimStart(new char[0]);
            if (((val.Length > 0) && (val[0] != '+')) && double.TryParse(val, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite, (IFormatProvider) NumberFormatInfo.InvariantInfo, out num))
            {
                return num;
            }
            return double.NaN;
        }

        internal static bool Equals(bool x, double y) => 
            (x == Boolean(y));

        internal static bool Equals(bool x, string y) => 
            (x == Boolean(y));

        internal static bool Equals(double x, double y) => 
            (x == y);

        internal static bool Equals(double x, string y) => 
            (x == Double(y));

        internal static bool Equals(NodeSequence x, double y) => 
            x.Equals(y);

        internal static bool Equals(NodeSequence x, string y) => 
            x.Equals(y);

        internal static bool Equals(string x, string y) => 
            ((x.Length == y.Length) && (x == y));

        internal static double Round(double val)
        {
            if ((-0.5 <= val) && (val <= 0.0))
            {
                return Math.Round(val);
            }
            return Math.Floor((double) (val + 0.5));
        }

        internal static string String(bool val)
        {
            if (!val)
            {
                return "false";
            }
            return "true";
        }

        internal static string String(double val) => 
            val.ToString(CultureInfo.InvariantCulture);

        internal static string String(NodeSequence sequence) => 
            sequence.StringValue();
    }
}

