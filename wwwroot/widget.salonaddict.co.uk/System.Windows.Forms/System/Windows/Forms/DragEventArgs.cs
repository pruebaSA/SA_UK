namespace System.Windows.Forms
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DragEventArgs : EventArgs
    {
        private readonly DragDropEffects allowedEffect;
        private readonly IDataObject data;
        private DragDropEffects effect;
        private readonly int keyState;
        private readonly int x;
        private readonly int y;

        public DragEventArgs(IDataObject data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect)
        {
            this.data = data;
            this.keyState = keyState;
            this.x = x;
            this.y = y;
            this.allowedEffect = allowedEffect;
            this.effect = effect;
        }

        public DragDropEffects AllowedEffect =>
            this.allowedEffect;

        public IDataObject Data =>
            this.data;

        public DragDropEffects Effect
        {
            get => 
                this.effect;
            set
            {
                this.effect = value;
            }
        }

        public int KeyState =>
            this.keyState;

        public int X =>
            this.x;

        public int Y =>
            this.y;
    }
}

