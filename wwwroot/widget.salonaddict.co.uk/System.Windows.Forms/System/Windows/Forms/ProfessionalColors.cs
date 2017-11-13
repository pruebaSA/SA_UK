namespace System.Windows.Forms
{
    using Microsoft.Win32;
    using System;
    using System.Drawing;
    using System.Windows.Forms.VisualStyles;

    public sealed class ProfessionalColors
    {
        [ThreadStatic]
        private static object colorFreshnessKey;
        [ThreadStatic]
        private static string colorScheme;
        [ThreadStatic]
        private static ProfessionalColorTable professionalColorTable;

        static ProfessionalColors()
        {
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(ProfessionalColors.OnUserPreferenceChanged);
            SetScheme();
        }

        private ProfessionalColors()
        {
        }

        private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            SetScheme();
            if (e.Category == UserPreferenceCategory.Color)
            {
                colorFreshnessKey = new object();
            }
        }

        private static void SetScheme()
        {
            if (VisualStyleRenderer.IsSupported)
            {
                colorScheme = VisualStyleInformation.ColorScheme;
            }
            else
            {
                colorScheme = null;
            }
        }

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonCheckedGradientBeginDescr")]
        public static System.Drawing.Color ButtonCheckedGradientBegin =>
            ColorTable.ButtonCheckedGradientBegin;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonCheckedGradientEndDescr")]
        public static System.Drawing.Color ButtonCheckedGradientEnd =>
            ColorTable.ButtonCheckedGradientEnd;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonCheckedGradientMiddleDescr")]
        public static System.Drawing.Color ButtonCheckedGradientMiddle =>
            ColorTable.ButtonCheckedGradientMiddle;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonCheckedHighlightDescr")]
        public static System.Drawing.Color ButtonCheckedHighlight =>
            ColorTable.ButtonCheckedHighlight;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonCheckedHighlightBorderDescr")]
        public static System.Drawing.Color ButtonCheckedHighlightBorder =>
            ColorTable.ButtonCheckedHighlightBorder;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonPressedBorderDescr")]
        public static System.Drawing.Color ButtonPressedBorder =>
            ColorTable.ButtonPressedBorder;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonPressedGradientBeginDescr")]
        public static System.Drawing.Color ButtonPressedGradientBegin =>
            ColorTable.ButtonPressedGradientBegin;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonPressedGradientEndDescr")]
        public static System.Drawing.Color ButtonPressedGradientEnd =>
            ColorTable.ButtonPressedGradientEnd;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonPressedGradientMiddleDescr")]
        public static System.Drawing.Color ButtonPressedGradientMiddle =>
            ColorTable.ButtonPressedGradientMiddle;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonPressedHighlightDescr")]
        public static System.Drawing.Color ButtonPressedHighlight =>
            ColorTable.ButtonPressedHighlight;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonPressedHighlightBorderDescr")]
        public static System.Drawing.Color ButtonPressedHighlightBorder =>
            ColorTable.ButtonPressedHighlightBorder;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonSelectedBorderDescr")]
        public static System.Drawing.Color ButtonSelectedBorder =>
            ColorTable.ButtonCheckedGradientBegin;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonSelectedGradientBeginDescr")]
        public static System.Drawing.Color ButtonSelectedGradientBegin =>
            ColorTable.ButtonSelectedGradientBegin;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonSelectedGradientEndDescr")]
        public static System.Drawing.Color ButtonSelectedGradientEnd =>
            ColorTable.ButtonSelectedGradientEnd;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonSelectedGradientMiddleDescr")]
        public static System.Drawing.Color ButtonSelectedGradientMiddle =>
            ColorTable.ButtonSelectedGradientMiddle;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonSelectedHighlightDescr")]
        public static System.Drawing.Color ButtonSelectedHighlight =>
            ColorTable.ButtonSelectedHighlight;

        [System.Windows.Forms.SRDescription("ProfessionalColorsButtonSelectedHighlightBorderDescr")]
        public static System.Drawing.Color ButtonSelectedHighlightBorder =>
            ColorTable.ButtonSelectedHighlightBorder;

        [System.Windows.Forms.SRDescription("ProfessionalColorsCheckBackgroundDescr")]
        public static System.Drawing.Color CheckBackground =>
            ColorTable.CheckBackground;

        [System.Windows.Forms.SRDescription("ProfessionalColorsCheckPressedBackgroundDescr")]
        public static System.Drawing.Color CheckPressedBackground =>
            ColorTable.CheckPressedBackground;

        [System.Windows.Forms.SRDescription("ProfessionalColorsCheckSelectedBackgroundDescr")]
        public static System.Drawing.Color CheckSelectedBackground =>
            ColorTable.CheckSelectedBackground;

        internal static object ColorFreshnessKey =>
            colorFreshnessKey;

        internal static string ColorScheme =>
            colorScheme;

        internal static ProfessionalColorTable ColorTable
        {
            get
            {
                if (professionalColorTable == null)
                {
                    professionalColorTable = new ProfessionalColorTable();
                }
                return professionalColorTable;
            }
        }

        [System.Windows.Forms.SRDescription("ProfessionalColorsGripDarkDescr")]
        public static System.Drawing.Color GripDark =>
            ColorTable.GripDark;

        [System.Windows.Forms.SRDescription("ProfessionalColorsGripLightDescr")]
        public static System.Drawing.Color GripLight =>
            ColorTable.GripLight;

        [System.Windows.Forms.SRDescription("ProfessionalColorsImageMarginGradientBeginDescr")]
        public static System.Drawing.Color ImageMarginGradientBegin =>
            ColorTable.ImageMarginGradientBegin;

        [System.Windows.Forms.SRDescription("ProfessionalColorsImageMarginGradientEndDescr")]
        public static System.Drawing.Color ImageMarginGradientEnd =>
            ColorTable.ImageMarginGradientEnd;

        [System.Windows.Forms.SRDescription("ProfessionalColorsImageMarginGradientMiddleDescr")]
        public static System.Drawing.Color ImageMarginGradientMiddle =>
            ColorTable.ImageMarginGradientMiddle;

        [System.Windows.Forms.SRDescription("ProfessionalColorsImageMarginRevealedGradientBeginDescr")]
        public static System.Drawing.Color ImageMarginRevealedGradientBegin =>
            ColorTable.ImageMarginRevealedGradientBegin;

        [System.Windows.Forms.SRDescription("ProfessionalColorsImageMarginRevealedGradientEndDescr")]
        public static System.Drawing.Color ImageMarginRevealedGradientEnd =>
            ColorTable.ImageMarginRevealedGradientEnd;

        [System.Windows.Forms.SRDescription("ProfessionalColorsImageMarginRevealedGradientMiddleDescr")]
        public static System.Drawing.Color ImageMarginRevealedGradientMiddle =>
            ColorTable.ImageMarginRevealedGradientMiddle;

        [System.Windows.Forms.SRDescription("ProfessionalColorsMenuBorderDescr")]
        public static System.Drawing.Color MenuBorder =>
            ColorTable.MenuBorder;

        [System.Windows.Forms.SRDescription("ProfessionalColorsMenuItemBorderDescr")]
        public static System.Drawing.Color MenuItemBorder =>
            ColorTable.MenuItemBorder;

        [System.Windows.Forms.SRDescription("ProfessionalColorsMenuItemPressedGradientBeginDescr")]
        public static System.Drawing.Color MenuItemPressedGradientBegin =>
            ColorTable.MenuItemPressedGradientBegin;

        [System.Windows.Forms.SRDescription("ProfessionalColorsMenuItemPressedGradientEndDescr")]
        public static System.Drawing.Color MenuItemPressedGradientEnd =>
            ColorTable.MenuItemPressedGradientEnd;

        [System.Windows.Forms.SRDescription("ProfessionalColorsMenuItemPressedGradientMiddleDescr")]
        public static System.Drawing.Color MenuItemPressedGradientMiddle =>
            ColorTable.MenuItemPressedGradientMiddle;

        [System.Windows.Forms.SRDescription("ProfessionalColorsMenuItemSelectedDescr")]
        public static System.Drawing.Color MenuItemSelected =>
            ColorTable.MenuItemBorder;

        [System.Windows.Forms.SRDescription("ProfessionalColorsMenuItemSelectedGradientBeginDescr")]
        public static System.Drawing.Color MenuItemSelectedGradientBegin =>
            ColorTable.MenuItemSelectedGradientBegin;

        [System.Windows.Forms.SRDescription("ProfessionalColorsMenuItemSelectedGradientEndDescr")]
        public static System.Drawing.Color MenuItemSelectedGradientEnd =>
            ColorTable.MenuItemSelectedGradientEnd;

        [System.Windows.Forms.SRDescription("ProfessionalColorsMenuStripGradientBeginDescr")]
        public static System.Drawing.Color MenuStripGradientBegin =>
            ColorTable.MenuStripGradientBegin;

        [System.Windows.Forms.SRDescription("ProfessionalColorsMenuStripGradientEndDescr")]
        public static System.Drawing.Color MenuStripGradientEnd =>
            ColorTable.MenuStripGradientEnd;

        [System.Windows.Forms.SRDescription("ProfessionalColorsOverflowButtonGradientBeginDescr")]
        public static System.Drawing.Color OverflowButtonGradientBegin =>
            ColorTable.OverflowButtonGradientBegin;

        [System.Windows.Forms.SRDescription("ProfessionalColorsOverflowButtonGradientEndDescr")]
        public static System.Drawing.Color OverflowButtonGradientEnd =>
            ColorTable.OverflowButtonGradientEnd;

        [System.Windows.Forms.SRDescription("ProfessionalColorsOverflowButtonGradientMiddleDescr")]
        public static System.Drawing.Color OverflowButtonGradientMiddle =>
            ColorTable.OverflowButtonGradientMiddle;

        [System.Windows.Forms.SRDescription("ProfessionalColorsRaftingContainerGradientBeginDescr")]
        public static System.Drawing.Color RaftingContainerGradientBegin =>
            ColorTable.RaftingContainerGradientBegin;

        [System.Windows.Forms.SRDescription("ProfessionalColorsRaftingContainerGradientEndDescr")]
        public static System.Drawing.Color RaftingContainerGradientEnd =>
            ColorTable.RaftingContainerGradientEnd;

        [System.Windows.Forms.SRDescription("ProfessionalColorsSeparatorDarkDescr")]
        public static System.Drawing.Color SeparatorDark =>
            ColorTable.SeparatorDark;

        [System.Windows.Forms.SRDescription("ProfessionalColorsSeparatorLightDescr")]
        public static System.Drawing.Color SeparatorLight =>
            ColorTable.SeparatorLight;

        [System.Windows.Forms.SRDescription("ProfessionalColorsStatusStripGradientBeginDescr")]
        public static System.Drawing.Color StatusStripGradientBegin =>
            ColorTable.StatusStripGradientBegin;

        [System.Windows.Forms.SRDescription("ProfessionalColorsStatusStripGradientEndDescr")]
        public static System.Drawing.Color StatusStripGradientEnd =>
            ColorTable.StatusStripGradientEnd;

        [System.Windows.Forms.SRDescription("ProfessionalColorsToolStripBorderDescr")]
        public static System.Drawing.Color ToolStripBorder =>
            ColorTable.ToolStripBorder;

        [System.Windows.Forms.SRDescription("ProfessionalColorsToolStripContentPanelGradientBeginDescr")]
        public static System.Drawing.Color ToolStripContentPanelGradientBegin =>
            ColorTable.ToolStripContentPanelGradientBegin;

        [System.Windows.Forms.SRDescription("ProfessionalColorsToolStripContentPanelGradientEndDescr")]
        public static System.Drawing.Color ToolStripContentPanelGradientEnd =>
            ColorTable.ToolStripContentPanelGradientEnd;

        [System.Windows.Forms.SRDescription("ProfessionalColorsToolStripDropDownBackgroundDescr")]
        public static System.Drawing.Color ToolStripDropDownBackground =>
            ColorTable.ToolStripDropDownBackground;

        [System.Windows.Forms.SRDescription("ProfessionalColorsToolStripGradientBeginDescr")]
        public static System.Drawing.Color ToolStripGradientBegin =>
            ColorTable.ToolStripGradientBegin;

        [System.Windows.Forms.SRDescription("ProfessionalColorsToolStripGradientEndDescr")]
        public static System.Drawing.Color ToolStripGradientEnd =>
            ColorTable.ToolStripGradientEnd;

        [System.Windows.Forms.SRDescription("ProfessionalColorsToolStripGradientMiddleDescr")]
        public static System.Drawing.Color ToolStripGradientMiddle =>
            ColorTable.ToolStripGradientMiddle;

        [System.Windows.Forms.SRDescription("ProfessionalColorsToolStripPanelGradientBeginDescr")]
        public static System.Drawing.Color ToolStripPanelGradientBegin =>
            ColorTable.ToolStripPanelGradientBegin;

        [System.Windows.Forms.SRDescription("ProfessionalColorsToolStripPanelGradientEndDescr")]
        public static System.Drawing.Color ToolStripPanelGradientEnd =>
            ColorTable.ToolStripPanelGradientEnd;
    }
}

