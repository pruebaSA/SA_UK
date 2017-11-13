namespace AjaxControlToolkit
{
    using System;

    [Serializable]
    public class CascadingDropDownNameValue
    {
        public bool isDefaultValue;
        public string name;
        public string optionTitle;
        public string value;

        public CascadingDropDownNameValue()
        {
        }

        public CascadingDropDownNameValue(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public CascadingDropDownNameValue(string name, string value, bool defaultValue)
        {
            this.name = name;
            this.value = value;
            this.isDefaultValue = defaultValue;
        }
    }
}

