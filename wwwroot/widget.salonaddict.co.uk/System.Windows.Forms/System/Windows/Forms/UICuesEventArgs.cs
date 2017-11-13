namespace System.Windows.Forms
{
    using System;

    public class UICuesEventArgs : EventArgs
    {
        private readonly UICues uicues;

        public UICuesEventArgs(UICues uicues)
        {
            this.uicues = uicues;
        }

        public UICues Changed =>
            (this.uicues & UICues.Changed);

        public bool ChangeFocus =>
            ((this.uicues & UICues.ChangeFocus) != UICues.None);

        public bool ChangeKeyboard =>
            ((this.uicues & UICues.ChangeKeyboard) != UICues.None);

        public bool ShowFocus =>
            ((this.uicues & UICues.ShowFocus) != UICues.None);

        public bool ShowKeyboard =>
            ((this.uicues & UICues.ShowKeyboard) != UICues.None);
    }
}

