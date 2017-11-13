namespace System.Windows.Threading
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows;

    [StructLayout(LayoutKind.Sequential)]
    internal struct PriorityRange
    {
        public static readonly PriorityRange All;
        public static readonly PriorityRange None;
        private DispatcherPriority _min;
        private bool _isMinInclusive;
        private DispatcherPriority _max;
        private bool _isMaxInclusive;
        public PriorityRange(DispatcherPriority min, DispatcherPriority max)
        {
            this = new PriorityRange();
            this.Initialize(min, true, max, true);
        }

        public PriorityRange(DispatcherPriority min, bool isMinInclusive, DispatcherPriority max, bool isMaxInclusive)
        {
            this = new PriorityRange();
            this.Initialize(min, isMinInclusive, max, isMaxInclusive);
        }

        public DispatcherPriority Min =>
            this._min;
        public DispatcherPriority Max =>
            this._max;
        public bool IsMinInclusive =>
            this._isMinInclusive;
        public bool IsMaxInclusive =>
            this._isMaxInclusive;
        public bool IsValid =>
            ((((this._min > DispatcherPriority.Invalid) && (this._min <= DispatcherPriority.Send)) && (this._max > DispatcherPriority.Invalid)) && (this._max <= DispatcherPriority.Send));
        public bool Contains(DispatcherPriority priority)
        {
            if ((priority <= DispatcherPriority.Invalid) || (priority > DispatcherPriority.Send))
            {
                return false;
            }
            if (!this.IsValid)
            {
                return false;
            }
            bool flag = false;
            if (this._isMinInclusive)
            {
                flag = priority >= this._min;
            }
            else
            {
                flag = priority > this._min;
            }
            if (!flag)
            {
                return flag;
            }
            if (this._isMaxInclusive)
            {
                return (priority <= this._max);
            }
            return (priority < this._max);
        }

        public bool Contains(PriorityRange priorityRange)
        {
            if (!priorityRange.IsValid)
            {
                return false;
            }
            if (!this.IsValid)
            {
                return false;
            }
            bool flag = false;
            if (priorityRange._isMinInclusive)
            {
                flag = this.Contains(priorityRange.Min);
            }
            else if ((priorityRange.Min >= this._min) && (priorityRange.Min < this._max))
            {
                flag = true;
            }
            if (flag)
            {
                if (priorityRange._isMaxInclusive)
                {
                    return this.Contains(priorityRange.Max);
                }
                if ((priorityRange.Max > this._min) && (priorityRange.Max <= this._max))
                {
                    flag = true;
                }
            }
            return flag;
        }

        public override bool Equals(object o) => 
            ((o is PriorityRange) && this.Equals((PriorityRange) o));

        public bool Equals(PriorityRange priorityRange) => 
            ((((priorityRange._min == this._min) && (priorityRange._isMinInclusive == this._isMinInclusive)) && (priorityRange._max == this._max)) && (priorityRange._isMaxInclusive == this._isMaxInclusive));

        public static bool operator ==(PriorityRange priorityRange1, PriorityRange priorityRange2) => 
            priorityRange1.Equals(priorityRange2);

        public static bool operator !=(PriorityRange priorityRange1, PriorityRange priorityRange2) => 
            !(priorityRange1 == priorityRange2);

        public override int GetHashCode() => 
            base.GetHashCode();

        private void Initialize(DispatcherPriority min, bool isMinInclusive, DispatcherPriority max, bool isMaxInclusive)
        {
            if ((min < DispatcherPriority.Invalid) || (min > DispatcherPriority.Send))
            {
                throw new InvalidEnumArgumentException("min", (int) min, typeof(DispatcherPriority));
            }
            if (min == DispatcherPriority.Inactive)
            {
                throw new ArgumentException(System.Windows.SR.Get("InvalidPriority"), "min");
            }
            if ((max < DispatcherPriority.Invalid) || (max > DispatcherPriority.Send))
            {
                throw new InvalidEnumArgumentException("max", (int) max, typeof(DispatcherPriority));
            }
            if (max == DispatcherPriority.Inactive)
            {
                throw new ArgumentException(System.Windows.SR.Get("InvalidPriority"), "max");
            }
            if (max < min)
            {
                throw new ArgumentException(System.Windows.SR.Get("InvalidPriorityRangeOrder"));
            }
            this._min = min;
            this._isMinInclusive = isMinInclusive;
            this._max = max;
            this._isMaxInclusive = isMaxInclusive;
        }

        private PriorityRange(DispatcherPriority min, DispatcherPriority max, bool ignored)
        {
            this._min = min;
            this._isMinInclusive = true;
            this._max = max;
            this._isMaxInclusive = true;
        }

        static PriorityRange()
        {
            All = new PriorityRange(DispatcherPriority.Inactive, DispatcherPriority.Send, true);
            None = new PriorityRange(DispatcherPriority.Invalid, DispatcherPriority.Invalid, true);
        }
    }
}

