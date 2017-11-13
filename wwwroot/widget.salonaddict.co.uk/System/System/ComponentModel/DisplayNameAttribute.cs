﻿namespace System.ComponentModel
{
    using System;

    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class)]
    public class DisplayNameAttribute : Attribute
    {
        private string _displayName;
        public static readonly DisplayNameAttribute Default = new DisplayNameAttribute();

        public DisplayNameAttribute() : this(string.Empty)
        {
        }

        public DisplayNameAttribute(string displayName)
        {
            this._displayName = displayName;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            DisplayNameAttribute attribute = obj as DisplayNameAttribute;
            return ((attribute != null) && (attribute.DisplayName == this.DisplayName));
        }

        public override int GetHashCode() => 
            this.DisplayName.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public virtual string DisplayName =>
            this.DisplayNameValue;

        protected string DisplayNameValue
        {
            get => 
                this._displayName;
            set
            {
                this._displayName = value;
            }
        }
    }
}

