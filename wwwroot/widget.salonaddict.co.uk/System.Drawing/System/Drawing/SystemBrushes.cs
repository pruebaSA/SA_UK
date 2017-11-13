namespace System.Drawing
{
    using System;

    public sealed class SystemBrushes
    {
        private static readonly object SystemBrushesKey = new object();

        private SystemBrushes()
        {
        }

        public static Brush FromSystemColor(Color c)
        {
            if (!c.IsSystemColor)
            {
                throw new ArgumentException(System.Drawing.SR.GetString("ColorNotSystemColor", new object[] { c.ToString() }));
            }
            Brush[] brushArray = (Brush[]) SafeNativeMethods.Gdip.ThreadData[SystemBrushesKey];
            if (brushArray == null)
            {
                brushArray = new Brush[0x21];
                SafeNativeMethods.Gdip.ThreadData[SystemBrushesKey] = brushArray;
            }
            int index = (int) c.ToKnownColor();
            if (index > 0xa7)
            {
                index -= 0x8d;
            }
            index--;
            if (brushArray[index] == null)
            {
                brushArray[index] = new SolidBrush(c, true);
            }
            return brushArray[index];
        }

        public static Brush ActiveBorder =>
            FromSystemColor(SystemColors.ActiveBorder);

        public static Brush ActiveCaption =>
            FromSystemColor(SystemColors.ActiveCaption);

        public static Brush ActiveCaptionText =>
            FromSystemColor(SystemColors.ActiveCaptionText);

        public static Brush AppWorkspace =>
            FromSystemColor(SystemColors.AppWorkspace);

        public static Brush ButtonFace =>
            FromSystemColor(SystemColors.ButtonFace);

        public static Brush ButtonHighlight =>
            FromSystemColor(SystemColors.ButtonHighlight);

        public static Brush ButtonShadow =>
            FromSystemColor(SystemColors.ButtonShadow);

        public static Brush Control =>
            FromSystemColor(SystemColors.Control);

        public static Brush ControlDark =>
            FromSystemColor(SystemColors.ControlDark);

        public static Brush ControlDarkDark =>
            FromSystemColor(SystemColors.ControlDarkDark);

        public static Brush ControlLight =>
            FromSystemColor(SystemColors.ControlLight);

        public static Brush ControlLightLight =>
            FromSystemColor(SystemColors.ControlLightLight);

        public static Brush ControlText =>
            FromSystemColor(SystemColors.ControlText);

        public static Brush Desktop =>
            FromSystemColor(SystemColors.Desktop);

        public static Brush GradientActiveCaption =>
            FromSystemColor(SystemColors.GradientActiveCaption);

        public static Brush GradientInactiveCaption =>
            FromSystemColor(SystemColors.GradientInactiveCaption);

        public static Brush GrayText =>
            FromSystemColor(SystemColors.GrayText);

        public static Brush Highlight =>
            FromSystemColor(SystemColors.Highlight);

        public static Brush HighlightText =>
            FromSystemColor(SystemColors.HighlightText);

        public static Brush HotTrack =>
            FromSystemColor(SystemColors.HotTrack);

        public static Brush InactiveBorder =>
            FromSystemColor(SystemColors.InactiveBorder);

        public static Brush InactiveCaption =>
            FromSystemColor(SystemColors.InactiveCaption);

        public static Brush InactiveCaptionText =>
            FromSystemColor(SystemColors.InactiveCaptionText);

        public static Brush Info =>
            FromSystemColor(SystemColors.Info);

        public static Brush InfoText =>
            FromSystemColor(SystemColors.InfoText);

        public static Brush Menu =>
            FromSystemColor(SystemColors.Menu);

        public static Brush MenuBar =>
            FromSystemColor(SystemColors.MenuBar);

        public static Brush MenuHighlight =>
            FromSystemColor(SystemColors.MenuHighlight);

        public static Brush MenuText =>
            FromSystemColor(SystemColors.MenuText);

        public static Brush ScrollBar =>
            FromSystemColor(SystemColors.ScrollBar);

        public static Brush Window =>
            FromSystemColor(SystemColors.Window);

        public static Brush WindowFrame =>
            FromSystemColor(SystemColors.WindowFrame);

        public static Brush WindowText =>
            FromSystemColor(SystemColors.WindowText);
    }
}

