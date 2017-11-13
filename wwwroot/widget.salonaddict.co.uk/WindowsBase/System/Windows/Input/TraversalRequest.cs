namespace System.Windows.Input
{
    using System;
    using System.ComponentModel;

    [Serializable]
    public class TraversalRequest
    {
        private System.Windows.Input.FocusNavigationDirection _focusNavigationDirection;
        private bool _wrapped;

        public TraversalRequest(System.Windows.Input.FocusNavigationDirection focusNavigationDirection)
        {
            if ((((focusNavigationDirection != System.Windows.Input.FocusNavigationDirection.Next) && (focusNavigationDirection != System.Windows.Input.FocusNavigationDirection.Previous)) && ((focusNavigationDirection != System.Windows.Input.FocusNavigationDirection.First) && (focusNavigationDirection != System.Windows.Input.FocusNavigationDirection.Last))) && (((focusNavigationDirection != System.Windows.Input.FocusNavigationDirection.Left) && (focusNavigationDirection != System.Windows.Input.FocusNavigationDirection.Right)) && ((focusNavigationDirection != System.Windows.Input.FocusNavigationDirection.Up) && (focusNavigationDirection != System.Windows.Input.FocusNavigationDirection.Down))))
            {
                throw new InvalidEnumArgumentException("focusNavigationDirection", (int) focusNavigationDirection, typeof(System.Windows.Input.FocusNavigationDirection));
            }
            this._focusNavigationDirection = focusNavigationDirection;
        }

        public System.Windows.Input.FocusNavigationDirection FocusNavigationDirection =>
            this._focusNavigationDirection;

        public bool Wrapped
        {
            get => 
                this._wrapped;
            set
            {
                this._wrapped = value;
            }
        }
    }
}

