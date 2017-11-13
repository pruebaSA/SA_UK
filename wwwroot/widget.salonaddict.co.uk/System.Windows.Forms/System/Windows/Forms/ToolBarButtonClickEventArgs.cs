namespace System.Windows.Forms
{
    using System;

    public class ToolBarButtonClickEventArgs : EventArgs
    {
        private ToolBarButton button;

        public ToolBarButtonClickEventArgs(ToolBarButton button)
        {
            this.button = button;
        }

        public ToolBarButton Button
        {
            get => 
                this.button;
            set
            {
                this.button = value;
            }
        }
    }
}

