namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    public class PopupEventArgs : CancelEventArgs
    {
        private Control associatedControl;
        private IWin32Window associatedWindow;
        private bool isBalloon;
        private Size size;

        public PopupEventArgs(IWin32Window associatedWindow, Control associatedControl, bool isBalloon, Size size)
        {
            this.associatedWindow = associatedWindow;
            this.size = size;
            this.associatedControl = associatedControl;
            this.isBalloon = isBalloon;
        }

        public Control AssociatedControl =>
            this.associatedControl;

        public IWin32Window AssociatedWindow =>
            this.associatedWindow;

        public bool IsBalloon =>
            this.isBalloon;

        public Size ToolTipSize
        {
            get => 
                this.size;
            set
            {
                this.size = value;
            }
        }
    }
}

