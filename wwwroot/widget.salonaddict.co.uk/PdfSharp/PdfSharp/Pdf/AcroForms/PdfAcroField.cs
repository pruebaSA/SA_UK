namespace PdfSharp.Pdf.AcroForms
{
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Advanced;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class PdfAcroField : PdfDictionary
    {
        private PdfAcroFieldCollection fields;

        protected PdfAcroField(PdfDictionary dict) : base(dict)
        {
        }

        internal PdfAcroField(PdfDocument document) : base(document)
        {
        }

        internal virtual void GetDescendantNames(ref List<PdfName> names, string partialName)
        {
            if (this.HasKids)
            {
                PdfAcroFieldCollection fields = this.Fields;
                string str = base.Elements.GetString("/T");
                if (str.Length > 0)
                {
                    if ((partialName != null) && (partialName.Length > 0))
                    {
                        partialName = partialName + "." + str;
                    }
                    else
                    {
                        partialName = str;
                    }
                    fields.GetDescendantNames(ref names, partialName);
                }
            }
            else
            {
                string str2 = base.Elements.GetString("/T");
                if (str2.Length > 0)
                {
                    if (!string.IsNullOrEmpty(partialName))
                    {
                        names.Add(new PdfName(partialName + "." + str2));
                    }
                    else
                    {
                        names.Add(new PdfName(str2));
                    }
                }
            }
        }

        protected virtual PdfAcroField GetValue(string name)
        {
            if ((name == null) || (name.Length == 0))
            {
                return this;
            }
            if (this.HasKids)
            {
                return this.Fields.GetValue(name);
            }
            return null;
        }

        public string[] DescendantNames
        {
            get
            {
                List<PdfName> names = new List<PdfName>();
                if (this.HasKids)
                {
                    this.Fields.GetDescendantNames(ref names, null);
                }
                List<string> list2 = new List<string>();
                foreach (PdfName name in names)
                {
                    list2.Add(name.ToString());
                }
                return list2.ToArray();
            }
        }

        public PdfAcroFieldCollection Fields
        {
            get
            {
                if (this.fields == null)
                {
                    object obj2 = base.Elements.GetValue("/Kids", VCF.CreateIndirect);
                    this.fields = (PdfAcroFieldCollection) obj2;
                }
                return this.fields;
            }
        }

        public PdfAcroFieldFlags Flags =>
            ((PdfAcroFieldFlags) base.Elements.GetInteger("/Ff"));

        public bool HasKids
        {
            get
            {
                PdfItem item = base.Elements["/Kids"];
                if (item == null)
                {
                    return false;
                }
                return ((item is PdfArray) && (((PdfArray) item).Elements.Count > 0));
            }
        }

        public PdfAcroField this[string name] =>
            this.GetValue(name);

        public string Name =>
            base.Elements.GetString("/T");

        public bool ReadOnly
        {
            get => 
                ((this.Flags & PdfAcroFieldFlags.ReadOnly) != ((PdfAcroFieldFlags) 0));
            set
            {
                if (value)
                {
                    this.SetFlags |= PdfAcroFieldFlags.ReadOnly;
                }
                else
                {
                    this.SetFlags &= ~PdfAcroFieldFlags.ReadOnly;
                }
            }
        }

        internal PdfAcroFieldFlags SetFlags
        {
            get => 
                ((PdfAcroFieldFlags) base.Elements.GetInteger("/Ff"));
            set
            {
                base.Elements.SetInteger("/Ff", (int) value);
            }
        }

        public PdfItem Value
        {
            get => 
                base.Elements["/V"];
            set
            {
                if (this.ReadOnly)
                {
                    throw new InvalidOperationException("The field is read only.");
                }
                if (!(value is PdfString) && !(value is PdfName))
                {
                    throw new NotImplementedException("Values other than string cannot be set.");
                }
                base.Elements["/V"] = value;
            }
        }

        public class Keys : KeysBase
        {
            [KeyInfo(KeyType.Optional | KeyType.Dictionary)]
            public const string AA = "/AA";
            [KeyInfo(KeyType.Required | KeyType.String)]
            public const string DA = "/DA";
            [KeyInfo(KeyType.Required | KeyType.Dictionary)]
            public const string DR = "/DR";
            [KeyInfo(KeyType.Optional | KeyType.Various)]
            public const string DV = "/DV";
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string Ff = "/Ff";
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string FT = "/FT";
            [KeyInfo(KeyType.Optional | KeyType.Array, typeof(PdfAcroField.PdfAcroFieldCollection))]
            public const string Kids = "/Kids";
            [KeyInfo(KeyType.Dictionary)]
            public const string Parent = "/Parent";
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string Q = "/Q";
            [KeyInfo(KeyType.Optional | KeyType.TextString)]
            public const string T = "/T";
            [KeyInfo(KeyType.Optional | KeyType.TextString)]
            public const string TM = "/TM";
            [KeyInfo(KeyType.Optional | KeyType.TextString)]
            public const string TU = "/TU";
            [KeyInfo(KeyType.Optional | KeyType.Various)]
            public const string V = "/V";
        }

        public sealed class PdfAcroFieldCollection : PdfArray
        {
            private PdfAcroFieldCollection(PdfArray array) : base(array)
            {
            }

            private PdfAcroField CreateAcroField(PdfDictionary dict)
            {
                string name = dict.Elements.GetName("/FT");
                PdfAcroFieldFlags integer = (PdfAcroFieldFlags) dict.Elements.GetInteger("/Ff");
                switch (name)
                {
                    case "/Btn":
                        if ((integer & PdfAcroFieldFlags.Pushbutton) != ((PdfAcroFieldFlags) 0))
                        {
                            return new PdfPushButtonField(dict);
                        }
                        if ((integer & PdfAcroFieldFlags.Radio) != ((PdfAcroFieldFlags) 0))
                        {
                            return new PdfRadioButtonField(dict);
                        }
                        return new PdfCheckBoxField(dict);

                    case "/Tx":
                        return new PdfTextField(dict);

                    case "/Ch":
                        if ((integer & PdfAcroFieldFlags.Combo) != ((PdfAcroFieldFlags) 0))
                        {
                            return new PdfComboBoxField(dict);
                        }
                        return new PdfListBoxField(dict);

                    case "/Sig":
                        return new PdfSignatureField(dict);
                }
                return new PdfGenericField(dict);
            }

            internal void GetDescendantNames(ref List<PdfName> names, string partialName)
            {
                int count = base.Elements.Count;
                for (int i = 0; i < count; i++)
                {
                    PdfAcroField field = this[i];
                    if (field != null)
                    {
                        field.GetDescendantNames(ref names, partialName);
                    }
                }
            }

            internal PdfAcroField GetValue(string name)
            {
                if ((name != null) && (name.Length != 0))
                {
                    int index = name.IndexOf('.');
                    string str = (index == -1) ? name : name.Substring(0, index);
                    string str2 = (index == -1) ? "" : name.Substring(index + 1);
                    int count = base.Elements.Count;
                    for (int i = 0; i < count; i++)
                    {
                        PdfAcroField field = this[i];
                        if (field.Name == str)
                        {
                            return field.GetValue(str2);
                        }
                    }
                }
                return null;
            }

            public string[] DescendantNames
            {
                get
                {
                    List<PdfName> names = new List<PdfName>();
                    this.GetDescendantNames(ref names, null);
                    List<string> list2 = new List<string>();
                    foreach (PdfName name in names)
                    {
                        list2.Add(name.ToString());
                    }
                    return list2.ToArray();
                }
            }

            public PdfAcroField this[int index]
            {
                get
                {
                    PdfItem item = base.Elements[index];
                    PdfDictionary dict = ((PdfReference) item).Value as PdfDictionary;
                    PdfAcroField field = dict as PdfAcroField;
                    if ((field == null) && (dict != null))
                    {
                        field = this.CreateAcroField(dict);
                    }
                    return field;
                }
            }

            public PdfAcroField this[string name] =>
                this.GetValue(name);

            public string[] Names
            {
                get
                {
                    int count = base.Elements.Count;
                    string[] strArray = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        strArray[i] = ((PdfDictionary) ((PdfReference) base.Elements[i]).Value).Elements.GetString("/T");
                    }
                    return strArray;
                }
            }
        }
    }
}

