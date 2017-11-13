namespace System.Drawing
{
    using System;

    public sealed class SystemPens
    {
        private static readonly object SystemPensKey = new object();

        private SystemPens()
        {
        }

        public static Pen FromSystemColor(Color c)
        {
            if (!c.IsSystemColor)
            {
                throw new ArgumentException(System.Drawing.SR.GetString("ColorNotSystemColor", new object[] { c.ToString() }));
            }
            Pen[] penArray = (Pen[]) SafeNativeMethods.Gdip.ThreadData[SystemPensKey];
            if (penArray == null)
            {
                penArray = new Pen[0x21];
                SafeNativeMethods.Gdip.ThreadData[SystemPensKey] = penArray;
            }
            int index = (int) c.ToKnownColor();
            if (index > 0xa7)
            {
                index -= 0x8d;
            }
            index--;
            if (penArray[index] == null)
            {
                penArray[index] = new Pen(c, true);
            }
            return penArray[index];
        }

        public static Pen ActiveBorder =>
            FromSystemColor(SystemColors.ActiveBorder);

        public static Pen ActiveCaption =>
            FromSystemColor(SystemColors.ActiveCaption);

        public static Pen ActiveCaptionText =>
            FromSystemColor(SystemColors.ActiveCaptionText);

        public static Pen AppWorkspace =>
            FromSystemColor(SystemColors.AppWorkspace);

        public static Pen ButtonFace =>
            FromSystemColor(SystemColors.ButtonFace);

        public static Pen ButtonHighlight =>
            FromSystemColor(SystemColors.ButtonHighlight);

        public static Pen ButtonShadow =>
            FromSystemColor(SystemColors.ButtonShadow);

        public static Pen Control =>
            FromSystemColor(SystemColors.Control);

        public static Pen ControlDark =>
            FromSystemColor(SystemColors.ControlDark);

        public static Pen ControlDarkDark =>
            FromSystemColor(SystemColors.ControlDarkDark);

        public static Pen ControlLight =>
            FromSystemColor(SystemColors.ControlLight);

        public static Pen ControlLightLight =>
            FromSystemColor(SystemColors.ControlLightLight);

        public static Pen ControlText =>
            FromSystemColor(SystemColors.ControlText);

        public static Pen Desktop =>
            FromSystemColor(SystemColors.Desktop);

        public static Pen GradientActiveCaption =>
            FromSystemColor(SystemColors.GradientActiveCaption);

        public static Pen GradientInactiveCaption =>
            FromSystemColor(SystemColors.GradientInactiveCaption);

        public static Pen GrayText =>
            FromSystemColor(SystemColors.GrayText);

        public static Pen Highlight =>
            FromSystemColor(SystemColors.Highlight);

        public static Pen HighlightText =>
            FromSystemColor(SystemColors.HighlightText);

        public static Pen HotTrack =>
            FromSystemColor(SystemColors.HotTrack);

        public static Pen InactiveBorder =>
            FromSystemColor(SystemColors.InactiveBorder);

        public static Pen InactiveCaption =>
            FromSystemColor(SystemColors.InactiveCaption);

        public static Pen InactiveCaptionText =>
            FromSystemColor(SystemColors.InactiveCaptionText);

        public static Pen Info =>
            FromSystemColor(SystemColors.Info);

        public static Pen InfoText =>
            FromSystemColor(SystemColors.InfoText);

        public static Pen Menu =>
            FromSystemColor(SystemColors.Menu);

        public static Pen MenuBar =>
            FromSystemColor(SystemColors.MenuBar);

        public static Pen MenuHighlight =>
            FromSystemColor(SystemColors.MenuHighlight);

        public static Pen MenuText =>
            FromSystemColor(SystemColors.MenuText);

        public static Pen ScrollBar =>
            FromSystemColor(SystemColors.ScrollBar);

        public static Pen Window =>
            FromSystemColor(SystemColors.Window);

        public static Pen WindowFrame =>
            FromSystemColor(SystemColors.WindowFrame);

        public static Pen WindowText =>
            FromSystemColor(SystemColors.WindowText);
    }
}

