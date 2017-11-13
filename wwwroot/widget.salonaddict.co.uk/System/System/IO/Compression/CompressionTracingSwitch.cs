namespace System.IO.Compression
{
    using System;
    using System.Diagnostics;

    internal class CompressionTracingSwitch : Switch
    {
        internal static CompressionTracingSwitch tracingSwitch = new CompressionTracingSwitch("CompressionSwitch", "Compression Library Tracing Switch");

        internal CompressionTracingSwitch(string displayName, string description) : base(displayName, description)
        {
        }

        public static bool Informational =>
            (tracingSwitch.SwitchSetting >= 1);

        public static bool Verbose =>
            (tracingSwitch.SwitchSetting >= 2);
    }
}

