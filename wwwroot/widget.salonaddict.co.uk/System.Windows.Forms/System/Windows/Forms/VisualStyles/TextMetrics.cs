namespace System.Windows.Forms.VisualStyles
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TextMetrics
    {
        private int height;
        private int ascent;
        private int descent;
        private int internalLeading;
        private int externalLeading;
        private int aveCharWidth;
        private int maxCharWidth;
        private int weight;
        private int overhang;
        private int digitizedAspectX;
        private int digitizedAspectY;
        private char firstChar;
        private char lastChar;
        private char defaultChar;
        private char breakChar;
        private bool italic;
        private bool underlined;
        private bool struckOut;
        private TextMetricsPitchAndFamilyValues pitchAndFamily;
        private TextMetricsCharacterSet charSet;
        public int Height
        {
            get => 
                this.height;
            set
            {
                this.height = value;
            }
        }
        public int Ascent
        {
            get => 
                this.ascent;
            set
            {
                this.ascent = value;
            }
        }
        public int Descent
        {
            get => 
                this.descent;
            set
            {
                this.descent = value;
            }
        }
        public int InternalLeading
        {
            get => 
                this.internalLeading;
            set
            {
                this.internalLeading = value;
            }
        }
        public int ExternalLeading
        {
            get => 
                this.externalLeading;
            set
            {
                this.externalLeading = value;
            }
        }
        public int AverageCharWidth
        {
            get => 
                this.aveCharWidth;
            set
            {
                this.aveCharWidth = value;
            }
        }
        public int MaxCharWidth
        {
            get => 
                this.maxCharWidth;
            set
            {
                this.maxCharWidth = value;
            }
        }
        public int Weight
        {
            get => 
                this.weight;
            set
            {
                this.weight = value;
            }
        }
        public int Overhang
        {
            get => 
                this.overhang;
            set
            {
                this.overhang = value;
            }
        }
        public int DigitizedAspectX
        {
            get => 
                this.digitizedAspectX;
            set
            {
                this.digitizedAspectX = value;
            }
        }
        public int DigitizedAspectY
        {
            get => 
                this.digitizedAspectY;
            set
            {
                this.digitizedAspectY = value;
            }
        }
        public char FirstChar
        {
            get => 
                this.firstChar;
            set
            {
                this.firstChar = value;
            }
        }
        public char LastChar
        {
            get => 
                this.lastChar;
            set
            {
                this.lastChar = value;
            }
        }
        public char DefaultChar
        {
            get => 
                this.defaultChar;
            set
            {
                this.defaultChar = value;
            }
        }
        public char BreakChar
        {
            get => 
                this.breakChar;
            set
            {
                this.breakChar = value;
            }
        }
        public bool Italic
        {
            get => 
                this.italic;
            set
            {
                this.italic = value;
            }
        }
        public bool Underlined
        {
            get => 
                this.underlined;
            set
            {
                this.underlined = value;
            }
        }
        public bool StruckOut
        {
            get => 
                this.struckOut;
            set
            {
                this.struckOut = value;
            }
        }
        public TextMetricsPitchAndFamilyValues PitchAndFamily
        {
            get => 
                this.pitchAndFamily;
            set
            {
                this.pitchAndFamily = value;
            }
        }
        public TextMetricsCharacterSet CharSet
        {
            get => 
                this.charSet;
            set
            {
                this.charSet = value;
            }
        }
    }
}

