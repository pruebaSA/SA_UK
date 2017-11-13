namespace System.Web.UI.Design
{
    using System;

    public class ViewEventArgs : System.EventArgs
    {
        private System.EventArgs _eventArgs;
        private ViewEvent _eventType;
        private DesignerRegion _region;

        public ViewEventArgs(ViewEvent eventType, DesignerRegion region, System.EventArgs eventArgs)
        {
            this._eventType = eventType;
            this._region = region;
            this._eventArgs = eventArgs;
        }

        public System.EventArgs EventArgs =>
            this._eventArgs;

        public ViewEvent EventType =>
            this._eventType;

        public DesignerRegion Region =>
            this._region;
    }
}

