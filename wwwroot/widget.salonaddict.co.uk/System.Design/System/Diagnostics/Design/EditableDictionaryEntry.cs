namespace System.Diagnostics.Design
{
    using System;

    internal class EditableDictionaryEntry
    {
        public string _name;
        public string _value;

        public EditableDictionaryEntry(string name, string value)
        {
            this._name = name;
            this._value = value;
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

