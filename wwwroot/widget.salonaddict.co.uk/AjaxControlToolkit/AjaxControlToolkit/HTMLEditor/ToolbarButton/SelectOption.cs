namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using System;

    public class SelectOption
    {
        private string _text = "";
        private string _value = "";

        public string Text
        {
            get => 
                this._text;
            set
            {
                this._text = value;
            }
        }

        public string Value
        {
            get => 
                this._value;
            set
            {
                this._value = value;
            }
        }
    }
}

