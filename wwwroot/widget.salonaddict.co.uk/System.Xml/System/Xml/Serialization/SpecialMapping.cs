namespace System.Xml.Serialization
{
    using System;

    internal class SpecialMapping : TypeMapping
    {
        private bool namedAny;

        internal bool NamedAny
        {
            get => 
                this.namedAny;
            set
            {
                this.namedAny = value;
            }
        }
    }
}

