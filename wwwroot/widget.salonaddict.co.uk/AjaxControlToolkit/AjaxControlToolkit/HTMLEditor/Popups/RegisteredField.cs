namespace AjaxControlToolkit.HTMLEditor.Popups
{
    using System;
    using System.Web.UI;

    [Serializable]
    public class RegisteredField
    {
        [NonSerialized]
        private System.Web.UI.Control _control;
        private string _name;

        public RegisteredField()
        {
            this._name = "";
        }

        public RegisteredField(string name, System.Web.UI.Control control)
        {
            this._name = "";
            this._name = name;
            this._control = control;
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

        public string Name
        {
            get => 
                this._name;
            set
            {
                this._name = value;
            }
        }
    }
}

