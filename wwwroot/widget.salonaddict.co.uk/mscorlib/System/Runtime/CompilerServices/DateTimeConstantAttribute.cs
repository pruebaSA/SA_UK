namespace System.Runtime.CompilerServices
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field, Inherited=false)]
    public sealed class DateTimeConstantAttribute : CustomConstantAttribute
    {
        private DateTime date;

        public DateTimeConstantAttribute(long ticks)
        {
            this.date = new DateTime(ticks);
        }

        public override object Value =>
            this.date;
    }
}

