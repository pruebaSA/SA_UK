namespace System.ComponentModel
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;

    [StructLayout(LayoutKind.Sequential)]
    public struct SortDescription
    {
        private string _propertyName;
        private ListSortDirection _direction;
        private bool _sealed;
        public SortDescription(string propertyName, ListSortDirection direction)
        {
            if ((direction != ListSortDirection.Ascending) && (direction != ListSortDirection.Descending))
            {
                throw new InvalidEnumArgumentException("direction", (int) direction, typeof(ListSortDirection));
            }
            this._propertyName = propertyName;
            this._direction = direction;
            this._sealed = false;
        }

        public string PropertyName
        {
            get => 
                this._propertyName;
            set
            {
                if (this._sealed)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("CannotChangeAfterSealed", new object[] { "SortDescription" }));
                }
                this._propertyName = value;
            }
        }
        public ListSortDirection Direction
        {
            get => 
                this._direction;
            set
            {
                if (this._sealed)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("CannotChangeAfterSealed", new object[] { "SortDescription" }));
                }
                if ((value < ListSortDirection.Ascending) || (value > ListSortDirection.Descending))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(ListSortDirection));
                }
                this._direction = value;
            }
        }
        public bool IsSealed =>
            this._sealed;
        public override bool Equals(object obj) => 
            ((obj is SortDescription) && (this == ((SortDescription) obj)));

        public static bool operator ==(SortDescription sd1, SortDescription sd2) => 
            ((sd1.PropertyName == sd2.PropertyName) && (sd1.Direction == sd2.Direction));

        public static bool operator !=(SortDescription sd1, SortDescription sd2) => 
            !(sd1 == sd2);

        public override int GetHashCode()
        {
            int hashCode = this.Direction.GetHashCode();
            if (this.PropertyName != null)
            {
                hashCode = this.PropertyName.GetHashCode() + hashCode;
            }
            return hashCode;
        }

        internal void Seal()
        {
            this._sealed = true;
        }
    }
}

