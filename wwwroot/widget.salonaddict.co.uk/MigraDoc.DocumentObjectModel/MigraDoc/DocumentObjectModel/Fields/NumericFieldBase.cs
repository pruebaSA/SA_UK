namespace MigraDoc.DocumentObjectModel.Fields
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public abstract class NumericFieldBase : DocumentObject
    {
        [DV]
        internal NString format;
        protected static string[] validFormatStrings = new string[] { "", "ROMAN", "roman", "ALPHABETIC", "alphabetic" };

        internal NumericFieldBase()
        {
            this.format = NString.NullValue;
        }

        internal NumericFieldBase(DocumentObject parent) : base(parent)
        {
            this.format = NString.NullValue;
        }

        public NumericFieldBase Clone() => 
            ((NumericFieldBase) this.DeepCopy());

        protected override object DeepCopy() => 
            ((NumericFieldBase) base.DeepCopy());

        public override bool IsNull() => 
            false;

        protected bool IsValidFormat(string format)
        {
            foreach (string str in validFormatStrings)
            {
                if (str == this.Format)
                {
                    return true;
                }
            }
            return false;
        }

        public string Format
        {
            get => 
                this.format.Value;
            set
            {
                if (!this.IsValidFormat(value))
                {
                    throw new ArgumentException(DomSR.InvalidFieldFormat(value));
                }
                this.format.Value = value;
            }
        }
    }
}

