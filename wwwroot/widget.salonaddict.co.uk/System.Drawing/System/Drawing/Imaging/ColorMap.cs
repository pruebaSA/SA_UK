﻿namespace System.Drawing.Imaging
{
    using System;
    using System.Drawing;

    public sealed class ColorMap
    {
        private Color newColor = new Color();
        private Color oldColor = new Color();

        public Color NewColor
        {
            get => 
                this.newColor;
            set
            {
                this.newColor = value;
            }
        }

        public Color OldColor
        {
            get => 
                this.oldColor;
            set
            {
                this.oldColor = value;
            }
        }
    }
}

