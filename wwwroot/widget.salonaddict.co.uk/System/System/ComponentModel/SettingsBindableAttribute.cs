namespace System.ComponentModel
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SettingsBindableAttribute : Attribute
    {
        private bool _bindable;
        public static readonly SettingsBindableAttribute No = new SettingsBindableAttribute(false);
        public static readonly SettingsBindableAttribute Yes = new SettingsBindableAttribute(true);

        public SettingsBindableAttribute(bool bindable)
        {
            this._bindable = bindable;
        }

        public override bool Equals(object obj) => 
            ((obj == this) || (((obj != null) && (obj is SettingsBindableAttribute)) && (((SettingsBindableAttribute) obj).Bindable == this._bindable)));

        public override int GetHashCode() => 
            this._bindable.GetHashCode();

        public bool Bindable =>
            this._bindable;
    }
}

