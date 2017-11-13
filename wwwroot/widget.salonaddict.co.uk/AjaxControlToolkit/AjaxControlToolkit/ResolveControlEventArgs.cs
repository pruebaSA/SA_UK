namespace AjaxControlToolkit
{
    using System;
    using System.Web.UI;

    public class ResolveControlEventArgs : EventArgs
    {
        private System.Web.UI.Control _control;
        private string _controlID;

        public ResolveControlEventArgs(string controlId)
        {
            this._controlID = controlId;
        }

        public System.Web.UI.Control Control
        {
            get => 
                this._control;
            set
            {
                this._control = value;
            }
        }

        public string ControlID =>
            this._controlID;
    }
}

