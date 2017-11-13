namespace PdfSharp.Pdf
{
    using System;

    internal sealed class KeyDescriptor
    {
        private string fixedValue;
        private PdfSharp.Pdf.KeyType keyType;
        private string keyValue;
        private Type objectType;
        private string version;

        public KeyDescriptor(KeyInfoAttribute attribute)
        {
            this.version = attribute.Version;
            this.keyType = attribute.KeyType;
            this.fixedValue = attribute.FixedValue;
            this.objectType = attribute.ObjectType;
            if (this.version == "")
            {
                this.version = "1.0";
            }
        }

        public Type GetValueType()
        {
            Type objectType = this.objectType;
            if (objectType == null)
            {
                PdfSharp.Pdf.KeyType type2 = this.keyType & PdfSharp.Pdf.KeyType.TypeMask;
                if (type2 <= PdfSharp.Pdf.KeyType.ArrayOrDictionary)
                {
                    switch (type2)
                    {
                        case PdfSharp.Pdf.KeyType.Name:
                            return typeof(PdfName);

                        case PdfSharp.Pdf.KeyType.String:
                            return typeof(PdfString);

                        case PdfSharp.Pdf.KeyType.Boolean:
                            return typeof(PdfBoolean);

                        case PdfSharp.Pdf.KeyType.Integer:
                            return typeof(PdfInteger);

                        case PdfSharp.Pdf.KeyType.Real:
                            return typeof(PdfReal);

                        case PdfSharp.Pdf.KeyType.Date:
                            return typeof(PdfDate);

                        case PdfSharp.Pdf.KeyType.Rectangle:
                            return typeof(PdfRectangle);

                        case PdfSharp.Pdf.KeyType.Array:
                            return typeof(PdfArray);

                        case PdfSharp.Pdf.KeyType.Dictionary:
                            return typeof(PdfDictionary);

                        case PdfSharp.Pdf.KeyType.Stream:
                            return typeof(PdfDictionary);

                        case PdfSharp.Pdf.KeyType.NumberTree:
                            throw new NotImplementedException("KeyType.NumberTree");

                        case PdfSharp.Pdf.KeyType.Function:
                        case PdfSharp.Pdf.KeyType.TextString:
                        case (PdfSharp.Pdf.KeyType.Function | PdfSharp.Pdf.KeyType.String):
                        case (PdfSharp.Pdf.KeyType.TextString | PdfSharp.Pdf.KeyType.String):
                            return objectType;

                        case PdfSharp.Pdf.KeyType.NameOrArray:
                            throw new NotImplementedException("KeyType.NameOrArray");

                        case PdfSharp.Pdf.KeyType.ArrayOrDictionary:
                            throw new NotImplementedException("KeyType.ArrayOrDictionary");
                    }
                    return objectType;
                }
                switch (type2)
                {
                    case PdfSharp.Pdf.KeyType.StreamOrArray:
                        throw new NotImplementedException("KeyType.StreamOrArray");

                    case PdfSharp.Pdf.KeyType.ArrayOrNameOrString:
                        throw new NotImplementedException("KeyType.ArrayOrNameOrString");
                }
            }
            return objectType;
        }

        public bool CanBeIndirect =>
            ((this.keyType & PdfSharp.Pdf.KeyType.MustNotBeIndirect) == 0);

        public string FixedValue =>
            this.fixedValue;

        public PdfSharp.Pdf.KeyType KeyType
        {
            get => 
                this.keyType;
            set
            {
                this.keyType = value;
            }
        }

        public string KeyValue
        {
            get => 
                this.keyValue;
            set
            {
                this.keyValue = value;
            }
        }

        public Type ObjectType
        {
            get => 
                this.objectType;
            set
            {
                this.objectType = value;
            }
        }

        public string Version
        {
            get => 
                this.version;
            set
            {
                this.version = value;
            }
        }
    }
}

