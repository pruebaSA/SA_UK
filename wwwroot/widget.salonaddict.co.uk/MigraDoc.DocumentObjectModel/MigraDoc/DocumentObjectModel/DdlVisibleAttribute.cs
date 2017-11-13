namespace MigraDoc.DocumentObjectModel
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal class DdlVisibleAttribute : Attribute
    {
        private bool canAddValue;
        private bool canRemoveValue;
        private bool visible;

        public DdlVisibleAttribute()
        {
            this.visible = true;
        }

        public DdlVisibleAttribute(bool _visible)
        {
            this.visible = _visible;
        }

        public bool CanAddValue
        {
            get => 
                this.canAddValue;
            set
            {
                this.canAddValue = value;
            }
        }

        public bool CanRemoveValue
        {
            get => 
                this.canRemoveValue;
            set
            {
                this.canRemoveValue = value;
            }
        }

        public bool Visible
        {
            get => 
                this.visible;
            set
            {
                this.visible = value;
            }
        }
    }
}

