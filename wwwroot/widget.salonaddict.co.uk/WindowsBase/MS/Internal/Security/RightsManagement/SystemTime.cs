namespace MS.Internal.Security.RightsManagement
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [StructLayout(LayoutKind.Sequential), SecurityCritical(SecurityCriticalScope.Everything)]
    internal class SystemTime
    {
        private ushort Year;
        private ushort Month;
        private ushort DayOfWeek;
        private ushort Day;
        private ushort Hour;
        private ushort Minute;
        private ushort Second;
        private ushort Milliseconds;
        internal SystemTime(DateTime dateTime)
        {
            this.Year = (ushort) dateTime.Year;
            this.Month = (ushort) dateTime.Month;
            this.DayOfWeek = (ushort) dateTime.DayOfWeek;
            this.Day = (ushort) dateTime.Day;
            this.Hour = (ushort) dateTime.Hour;
            this.Minute = (ushort) dateTime.Minute;
            this.Second = (ushort) dateTime.Second;
            this.Milliseconds = (ushort) dateTime.Millisecond;
        }

        internal static uint Size =>
            0x10;
        internal SystemTime(byte[] dataBuffer)
        {
            this.Year = BitConverter.ToUInt16(dataBuffer, 0);
            this.Month = BitConverter.ToUInt16(dataBuffer, 2);
            this.DayOfWeek = BitConverter.ToUInt16(dataBuffer, 4);
            this.Day = BitConverter.ToUInt16(dataBuffer, 6);
            this.Hour = BitConverter.ToUInt16(dataBuffer, 8);
            this.Minute = BitConverter.ToUInt16(dataBuffer, 10);
            this.Second = BitConverter.ToUInt16(dataBuffer, 12);
            this.Milliseconds = BitConverter.ToUInt16(dataBuffer, 14);
        }

        internal DateTime GetDateTime(DateTime defaultValue)
        {
            if ((((this.Year == 0) && (this.Month == 0)) && ((this.Day == 0) && (this.Hour == 0))) && (((this.Minute == 0) && (this.Second == 0)) && (this.Milliseconds == 0)))
            {
                return defaultValue;
            }
            return new DateTime(this.Year, this.Month, this.Day, this.Hour, this.Minute, this.Second, this.Milliseconds);
        }
    }
}

