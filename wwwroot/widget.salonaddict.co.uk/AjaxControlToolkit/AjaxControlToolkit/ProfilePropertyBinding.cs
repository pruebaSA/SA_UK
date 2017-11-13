namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;

    public class ProfilePropertyBinding
    {
        private string _extenderPropertyName;
        private string _profilePropertyName;

        [NotifyParentProperty(true)]
        public string ExtenderPropertyName
        {
            get => 
                this._extenderPropertyName;
            set
            {
                this._extenderPropertyName = value;
            }
        }

        [NotifyParentProperty(true)]
        public string ProfilePropertyName
        {
            get => 
                this._profilePropertyName;
            set
            {
                this._profilePropertyName = value;
            }
        }
    }
}

