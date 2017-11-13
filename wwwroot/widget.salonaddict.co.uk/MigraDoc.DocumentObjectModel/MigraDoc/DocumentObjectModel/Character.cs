namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class Character : DocumentObject
    {
        public static readonly Character Blank = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-251658239));
        public static readonly Character Bullet = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-134217723));
        public static readonly Character Copyright = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-134217726));
        [DV]
        internal NInt count;
        public static readonly Character Em = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-251658237));
        public static readonly Character Em4 = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-251658236));
        public static readonly Character EmDash = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-134217721));
        public static readonly Character EmQuarter = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-251658236));
        public static readonly Character En = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-251658238));
        public static readonly Character EnDash = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-134217720));
        public static readonly Character Euro = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-134217727));
        public static readonly Character HardBlank = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-134217719));
        public static readonly Character LineBreak = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-201326591));
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        public static readonly Character NonBreakableBlank = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-134217719));
        public static readonly Character Not = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-134217722));
        public static readonly Character RegisteredTrademark = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-134217724));
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.SymbolName))]
        internal NEnum symbolName;
        public static readonly Character Tab = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-234881023));
        public static readonly Character Trademark = new Character((MigraDoc.DocumentObjectModel.SymbolName) (-134217725));

        public Character()
        {
            this.symbolName = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.SymbolName));
            this.count = new NInt(1);
        }

        internal Character(DocumentObject parent) : base(parent)
        {
            this.symbolName = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.SymbolName));
            this.count = new NInt(1);
        }

        private Character(MigraDoc.DocumentObjectModel.SymbolName name) : this()
        {
            this.symbolName.Value = (int) name;
        }

        internal override void Serialize(Serializer serializer)
        {
            string str = string.Empty;
            if (this.count == 1)
            {
                if (this.symbolName.Value == -234881023)
                {
                    str = @"\tab ";
                }
                else if (this.symbolName.Value == -201326591)
                {
                    str = "\\linebreak\r\n";
                }
                else if (this.symbolName.Value == -201326585)
                {
                    str = "\r\n\r\n";
                }
                if (str != "")
                {
                    serializer.Write(str);
                    return;
                }
            }
            if ((this.symbolName.Value & -268435456) == -268435456)
            {
                if ((this.symbolName.Value & -251658240) == -251658240)
                {
                    if (this.symbolName.Value == -251658239)
                    {
                        str = @"\space(" + this.Count + ")";
                    }
                    else if (this.count == 1)
                    {
                        str = @"\space(" + this.SymbolName + ")";
                    }
                    else
                    {
                        str = string.Concat(new object[] { @"\space(", this.SymbolName, ", ", this.Count, ")" });
                    }
                }
                else
                {
                    str = @"\symbol(" + this.SymbolName + ")";
                }
            }
            else
            {
                str = @" \chr(0x" + this.symbolName.Value.ToString("X") + ")";
            }
            serializer.Write(str);
        }

        public char Char
        {
            get
            {
                if ((this.symbolName.Value & -268435456) == 0)
                {
                    return (char) this.symbolName.Value;
                }
                return '\0';
            }
            set
            {
                this.symbolName.Value = value;
            }
        }

        public int Count
        {
            get => 
                this.count.Value;
            set
            {
                this.count.Value = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Character));
                }
                return meta;
            }
        }

        public MigraDoc.DocumentObjectModel.SymbolName SymbolName
        {
            get => 
                ((MigraDoc.DocumentObjectModel.SymbolName) this.symbolName.Value);
            set
            {
                this.symbolName.Value = (int) value;
            }
        }
    }
}

