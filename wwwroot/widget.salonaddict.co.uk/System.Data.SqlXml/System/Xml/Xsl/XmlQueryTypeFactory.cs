namespace System.Xml.Xsl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.XPath;

    internal static class XmlQueryTypeFactory
    {
        public static readonly XmlQueryType AnyAtomicType = Type(XmlTypeCode.AnyAtomicType, false);
        public static readonly XmlQueryType AnyAtomicTypeS = PrimeProduct(AnyAtomicType, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType Attribute = Type(XmlTypeCode.Attribute, false);
        public static readonly XmlQueryType AttributeOrContent = Choice(Attribute, Content);
        public static readonly XmlQueryType AttributeOrContentS = PrimeProduct(AttributeOrContent, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType AttributeQ = PrimeProduct(Attribute, XmlQueryCardinality.ZeroOrOne);
        public static readonly XmlQueryType AttributeS = PrimeProduct(Attribute, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType Boolean = Type(XmlTypeCode.Boolean, false);
        public static readonly XmlQueryType BooleanX = Type(XmlTypeCode.Boolean, true);
        public static readonly XmlQueryType Comment = Type(XmlTypeCode.Comment, false);
        public static readonly XmlQueryType CommentS = PrimeProduct(Comment, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType Content = Choice(new XmlQueryType[] { Element, Comment, PI, Text });
        public static readonly XmlQueryType ContentS = PrimeProduct(Content, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType DateTimeX = Type(XmlTypeCode.DateTime, true);
        public static readonly XmlQueryType DecimalX = Type(XmlTypeCode.Decimal, true);
        public static readonly XmlQueryType Document = Type(XmlTypeCode.Document, false);
        public static readonly XmlQueryType DocumentOrContent = Choice(Document, Content);
        public static readonly XmlQueryType DocumentOrContentS = PrimeProduct(DocumentOrContent, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType DocumentOrElement = Choice(Document, Element);
        public static readonly XmlQueryType DocumentOrElementQ = PrimeProduct(DocumentOrElement, XmlQueryCardinality.ZeroOrOne);
        public static readonly XmlQueryType DocumentOrElementS = PrimeProduct(DocumentOrElement, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType DocumentS = PrimeProduct(Document, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType Double = Type(XmlTypeCode.Double, false);
        public static readonly XmlQueryType DoubleX = Type(XmlTypeCode.Double, true);
        public static readonly XmlQueryType Element = Type(XmlTypeCode.Element, false);
        public static readonly XmlQueryType ElementS = PrimeProduct(Element, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType Empty = SequenceType.Zero;
        public static readonly XmlQueryType FloatX = Type(XmlTypeCode.Float, true);
        public static readonly XmlQueryType Int = Type(XmlTypeCode.Int, false);
        public static readonly XmlQueryType IntegerX = Type(XmlTypeCode.Integer, true);
        public static readonly XmlQueryType IntX = Type(XmlTypeCode.Int, true);
        public static readonly XmlQueryType IntXS = PrimeProduct(IntX, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType Item = Type(XmlTypeCode.Item, false);
        public static readonly XmlQueryType ItemS = PrimeProduct(Item, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType LongX = Type(XmlTypeCode.Long, true);
        public static readonly XmlQueryType Namespace = Type(XmlTypeCode.Namespace, false);
        public static readonly XmlQueryType NamespaceS = PrimeProduct(Namespace, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType Node = Type(XmlTypeCode.Node, false);
        public static readonly XmlQueryType NodeDodS = PrimeProduct(NodeNotRtf, XmlQueryCardinality.ZeroOrMore);
        private static readonly XmlTypeCode[] NodeKindToTypeCode = new XmlTypeCode[] { XmlTypeCode.Document, XmlTypeCode.Element, XmlTypeCode.Attribute, XmlTypeCode.Namespace, XmlTypeCode.Text, XmlTypeCode.Text, XmlTypeCode.Text, XmlTypeCode.ProcessingInstruction, XmlTypeCode.Comment, XmlTypeCode.Node };
        public static readonly XmlQueryType NodeNotRtf = ItemType.NodeNotRtf;
        public static readonly XmlQueryType NodeNotRtfQ = PrimeProduct(NodeNotRtf, XmlQueryCardinality.ZeroOrOne);
        public static readonly XmlQueryType NodeNotRtfS = PrimeProduct(NodeNotRtf, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType NodeS = PrimeProduct(Node, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType None = ChoiceType.None;
        public static readonly XmlQueryType PI = Type(XmlTypeCode.ProcessingInstruction, false);
        public static readonly XmlQueryType PIS = PrimeProduct(PI, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType QNameX = Type(XmlTypeCode.QName, true);
        public static readonly XmlQueryType String = Type(XmlTypeCode.String, false);
        public static readonly XmlQueryType StringX = Type(XmlTypeCode.String, true);
        public static readonly XmlQueryType StringXS = PrimeProduct(StringX, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType Text = Type(XmlTypeCode.Text, false);
        public static readonly XmlQueryType TextS = PrimeProduct(Text, XmlQueryCardinality.ZeroOrMore);
        public static readonly XmlQueryType UntypedAttribute = ItemType.UntypedAttribute;
        public static readonly XmlQueryType UntypedDocument = ItemType.UntypedDocument;
        public static readonly XmlQueryType UntypedElement = ItemType.UntypedElement;
        public static readonly XmlQueryType UntypedNode = Choice(new XmlQueryType[] { UntypedDocument, UntypedElement, UntypedAttribute, Namespace, Text, Comment, PI });
        public static readonly XmlQueryType UntypedNodeS = PrimeProduct(UntypedNode, XmlQueryCardinality.ZeroOrMore);

        private static void AddItemToChoice(List<XmlQueryType> accumulator, XmlQueryType itemType)
        {
            bool flag = true;
            for (int i = 0; i < accumulator.Count; i++)
            {
                if (itemType.IsSubtypeOf(accumulator[i]))
                {
                    return;
                }
                if (accumulator[i].IsSubtypeOf(itemType))
                {
                    if (flag)
                    {
                        flag = false;
                        accumulator[i] = itemType;
                    }
                    else
                    {
                        accumulator.RemoveAt(i);
                        i--;
                    }
                }
            }
            if (flag)
            {
                accumulator.Add(itemType);
            }
        }

        public static XmlQueryType AtMost(XmlQueryType t, XmlQueryCardinality c) => 
            PrimeProduct(t, c.AtMost());

        [Conditional("DEBUG")]
        public static void CheckSerializability(XmlQueryType type)
        {
            type.GetObjectData(new BinaryWriter(Stream.Null));
        }

        public static XmlQueryType Choice(params XmlQueryType[] types)
        {
            if (types.Length == 0)
            {
                return None;
            }
            if (types.Length == 1)
            {
                return types[0];
            }
            List<XmlQueryType> accumulator = new List<XmlQueryType>(types[0]);
            XmlQueryCardinality card = types[0].Cardinality;
            for (int i = 1; i < types.Length; i++)
            {
                PrimeChoice(accumulator, types[i]);
                card |= types[i].Cardinality;
            }
            return SequenceType.Create(ChoiceType.Create(accumulator), card);
        }

        public static XmlQueryType Choice(XmlQueryType left, XmlQueryType right) => 
            SequenceType.Create(ChoiceType.Create(PrimeChoice(new List<XmlQueryType>(left), right)), left.Cardinality | right.Cardinality);

        public static XmlQueryType Deserialize(BinaryReader reader)
        {
            switch (reader.ReadByte())
            {
                case 0:
                    return ItemType.Create(reader);

                case 1:
                    return ChoiceType.Create(reader);

                case 2:
                    return SequenceType.Create(reader);
            }
            return null;
        }

        public static XmlQueryType NodeChoice(XmlNodeKindFlags kinds) => 
            ChoiceType.Create(kinds);

        private static List<XmlQueryType> PrimeChoice(List<XmlQueryType> accumulator, IList<XmlQueryType> types)
        {
            foreach (XmlQueryType type in types)
            {
                AddItemToChoice(accumulator, type);
            }
            return accumulator;
        }

        public static XmlQueryType PrimeProduct(XmlQueryType t, XmlQueryCardinality c)
        {
            if ((t.Cardinality == c) && !t.IsDod)
            {
                return t;
            }
            return SequenceType.Create(t.Prime, c);
        }

        public static XmlQueryType Product(XmlQueryType t, XmlQueryCardinality c) => 
            PrimeProduct(t, t.Cardinality * c);

        public static XmlQueryType Sequence(XmlQueryType left, XmlQueryType right) => 
            SequenceType.Create(ChoiceType.Create(PrimeChoice(new List<XmlQueryType>(left), right)), left.Cardinality + right.Cardinality);

        public static void Serialize(BinaryWriter writer, XmlQueryType type)
        {
            sbyte num;
            if (type.GetType() == typeof(ItemType))
            {
                num = 0;
            }
            else if (type.GetType() == typeof(ChoiceType))
            {
                num = 1;
            }
            else if (type.GetType() == typeof(SequenceType))
            {
                num = 2;
            }
            else
            {
                num = -1;
            }
            writer.Write(num);
            type.GetObjectData(writer);
        }

        public static XmlQueryType Type(XmlSchemaSimpleType schemaType, bool isStrict)
        {
            if (schemaType.Datatype.Variety != XmlSchemaDatatypeVariety.Atomic)
            {
                while (schemaType.DerivedBy == XmlSchemaDerivationMethod.Restriction)
                {
                    schemaType = (XmlSchemaSimpleType) schemaType.BaseXmlSchemaType;
                }
                if (schemaType.DerivedBy == XmlSchemaDerivationMethod.List)
                {
                    return PrimeProduct(Type(((XmlSchemaSimpleTypeList) schemaType.Content).BaseItemType, isStrict), XmlQueryCardinality.ZeroOrMore);
                }
                XmlSchemaSimpleType[] baseMemberTypes = ((XmlSchemaSimpleTypeUnion) schemaType.Content).BaseMemberTypes;
                XmlQueryType[] types = new XmlQueryType[baseMemberTypes.Length];
                for (int i = 0; i < baseMemberTypes.Length; i++)
                {
                    types[i] = Type(baseMemberTypes[i], isStrict);
                }
                return Choice(types);
            }
            if (schemaType == DatatypeImplementation.AnySimpleType)
            {
                return AnyAtomicTypeS;
            }
            return ItemType.Create(schemaType, isStrict);
        }

        public static XmlQueryType Type(XmlTypeCode code, bool isStrict) => 
            ItemType.Create(code, isStrict);

        public static XmlQueryType Type(XPathNodeType kind, XmlQualifiedNameTest nameTest, XmlSchemaType contentType, bool isNillable) => 
            ItemType.Create(NodeKindToTypeCode[(int) kind], nameTest, contentType, isNillable);

        private sealed class ChoiceType : XmlQueryType
        {
            private XmlTypeCode code;
            private List<XmlQueryType> members;
            private XmlNodeKindFlags nodeKinds;
            private static readonly XmlTypeCode[] NodeKindToTypeCode;
            public static readonly XmlQueryType None = new XmlQueryTypeFactory.ChoiceType(new List<XmlQueryType>());
            private XmlSchemaType schemaType;

            static ChoiceType()
            {
                XmlTypeCode[] codeArray = new XmlTypeCode[8];
                codeArray[1] = XmlTypeCode.Document;
                codeArray[2] = XmlTypeCode.Element;
                codeArray[3] = XmlTypeCode.Attribute;
                codeArray[4] = XmlTypeCode.Text;
                codeArray[5] = XmlTypeCode.Comment;
                codeArray[6] = XmlTypeCode.ProcessingInstruction;
                codeArray[7] = XmlTypeCode.Namespace;
                NodeKindToTypeCode = codeArray;
            }

            private ChoiceType(List<XmlQueryType> members)
            {
                this.members = members;
                for (int i = 0; i < members.Count; i++)
                {
                    XmlQueryType type = members[i];
                    if (this.code == XmlTypeCode.None)
                    {
                        this.code = type.TypeCode;
                        this.schemaType = type.SchemaType;
                    }
                    else if (base.IsNode && type.IsNode)
                    {
                        if (this.code == type.TypeCode)
                        {
                            if (this.code == XmlTypeCode.Element)
                            {
                                this.schemaType = XmlSchemaComplexType.AnyType;
                            }
                            else if (this.code == XmlTypeCode.Attribute)
                            {
                                this.schemaType = DatatypeImplementation.AnySimpleType;
                            }
                        }
                        else
                        {
                            this.code = XmlTypeCode.Node;
                            this.schemaType = null;
                        }
                    }
                    else if (base.IsAtomicValue && type.IsAtomicValue)
                    {
                        this.code = XmlTypeCode.AnyAtomicType;
                        this.schemaType = DatatypeImplementation.AnyAtomicType;
                    }
                    else
                    {
                        this.code = XmlTypeCode.Item;
                        this.schemaType = null;
                    }
                    this.nodeKinds |= type.NodeKinds;
                }
            }

            public static XmlQueryType Create(List<XmlQueryType> members)
            {
                if (members.Count == 0)
                {
                    return None;
                }
                if (members.Count == 1)
                {
                    return members[0];
                }
                return new XmlQueryTypeFactory.ChoiceType(members);
            }

            public static XmlQueryType Create(BinaryReader reader)
            {
                int capacity = reader.ReadInt32();
                List<XmlQueryType> members = new List<XmlQueryType>(capacity);
                for (int i = 0; i < capacity; i++)
                {
                    members.Add(XmlQueryTypeFactory.Deserialize(reader));
                }
                return Create(members);
            }

            public static XmlQueryType Create(XmlNodeKindFlags nodeKinds)
            {
                if (Bits.ExactlyOne((uint) nodeKinds))
                {
                    return XmlQueryTypeFactory.ItemType.Create(NodeKindToTypeCode[Bits.LeastPosition((uint) nodeKinds)], false);
                }
                List<XmlQueryType> members = new List<XmlQueryType>();
                while (nodeKinds != XmlNodeKindFlags.None)
                {
                    members.Add(XmlQueryTypeFactory.ItemType.Create(NodeKindToTypeCode[Bits.LeastPosition((uint) nodeKinds)], false));
                    nodeKinds = (XmlNodeKindFlags) Bits.ClearLeast((uint) nodeKinds);
                }
                return Create(members);
            }

            public override void GetObjectData(BinaryWriter writer)
            {
                writer.Write(this.members.Count);
                for (int i = 0; i < this.members.Count; i++)
                {
                    XmlQueryTypeFactory.Serialize(writer, this.members[i]);
                }
            }

            public override XmlQueryCardinality Cardinality
            {
                get
                {
                    if (this.TypeCode != XmlTypeCode.None)
                    {
                        return XmlQueryCardinality.One;
                    }
                    return XmlQueryCardinality.None;
                }
            }

            public override XmlValueConverter ClrMapping
            {
                get
                {
                    if ((this.code == XmlTypeCode.None) || (this.code == XmlTypeCode.Item))
                    {
                        return XmlAnyConverter.Item;
                    }
                    if (base.IsAtomicValue)
                    {
                        return this.SchemaType.ValueConverter;
                    }
                    return XmlNodeConverter.Node;
                }
            }

            public override int Count =>
                this.members.Count;

            public override bool IsDod
            {
                get
                {
                    for (int i = 0; i < this.members.Count; i++)
                    {
                        if (!this.members[i].IsDod)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            public override bool IsNillable =>
                false;

            public override bool IsNotRtf
            {
                get
                {
                    for (int i = 0; i < this.members.Count; i++)
                    {
                        if (!this.members[i].IsNotRtf)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            public override bool IsStrict =>
                (this.members.Count == 0);

            public override XmlQueryType this[int index]
            {
                get => 
                    this.members[index];
                set
                {
                    throw new NotSupportedException();
                }
            }

            public override XmlQualifiedNameTest NameTest =>
                XmlQualifiedNameTest.Wildcard;

            public override XmlNodeKindFlags NodeKinds =>
                this.nodeKinds;

            public override XmlQueryType Prime =>
                this;

            public override XmlSchemaType SchemaType =>
                this.schemaType;

            public override XmlTypeCode TypeCode =>
                this.code;
        }

        private sealed class ItemType : XmlQueryType
        {
            private static XmlQueryType[] BuiltInItemTypes;
            private static XmlQueryType[] BuiltInItemTypesStrict;
            private XmlTypeCode code;
            private bool isNillable;
            private bool isNotRtf;
            private bool isStrict;
            private XmlQualifiedNameTest nameTest;
            public static readonly XmlQueryType NodeDod;
            private XmlNodeKindFlags nodeKinds;
            public static readonly XmlQueryType NodeNotRtf;
            private XmlSchemaType schemaType;
            private static XmlQueryType[] SpecialBuiltInItemTypes;
            public static readonly XmlQueryType UntypedAttribute;
            public static readonly XmlQueryType UntypedDocument;
            public static readonly XmlQueryType UntypedElement;

            static ItemType()
            {
                int num = 0x37;
                BuiltInItemTypes = new XmlQueryType[num];
                BuiltInItemTypesStrict = new XmlQueryType[num];
                for (int i = 0; i < num; i++)
                {
                    XmlTypeCode code = (XmlTypeCode) i;
                    switch (((XmlTypeCode) i))
                    {
                        case XmlTypeCode.None:
                            BuiltInItemTypes[i] = XmlQueryTypeFactory.ChoiceType.None;
                            BuiltInItemTypesStrict[i] = XmlQueryTypeFactory.ChoiceType.None;
                            break;

                        case XmlTypeCode.Item:
                        case XmlTypeCode.Node:
                            BuiltInItemTypes[i] = new XmlQueryTypeFactory.ItemType(code, XmlQualifiedNameTest.Wildcard, XmlSchemaComplexType.AnyType, false, false, false);
                            BuiltInItemTypesStrict[i] = BuiltInItemTypes[i];
                            break;

                        case XmlTypeCode.Document:
                        case XmlTypeCode.Element:
                        case XmlTypeCode.Namespace:
                        case XmlTypeCode.ProcessingInstruction:
                        case XmlTypeCode.Comment:
                        case XmlTypeCode.Text:
                            BuiltInItemTypes[i] = new XmlQueryTypeFactory.ItemType(code, XmlQualifiedNameTest.Wildcard, XmlSchemaComplexType.AnyType, false, false, true);
                            BuiltInItemTypesStrict[i] = BuiltInItemTypes[i];
                            break;

                        case XmlTypeCode.Attribute:
                            BuiltInItemTypes[i] = new XmlQueryTypeFactory.ItemType(code, XmlQualifiedNameTest.Wildcard, DatatypeImplementation.AnySimpleType, false, false, true);
                            BuiltInItemTypesStrict[i] = BuiltInItemTypes[i];
                            break;

                        case XmlTypeCode.AnyAtomicType:
                            BuiltInItemTypes[i] = new XmlQueryTypeFactory.ItemType(code, XmlQualifiedNameTest.Wildcard, DatatypeImplementation.AnyAtomicType, false, false, true);
                            BuiltInItemTypesStrict[i] = BuiltInItemTypes[i];
                            break;

                        case XmlTypeCode.UntypedAtomic:
                            BuiltInItemTypes[i] = new XmlQueryTypeFactory.ItemType(code, XmlQualifiedNameTest.Wildcard, DatatypeImplementation.UntypedAtomicType, false, true, true);
                            BuiltInItemTypesStrict[i] = BuiltInItemTypes[i];
                            break;

                        default:
                        {
                            XmlSchemaType builtInSimpleType = XmlSchemaType.GetBuiltInSimpleType(code);
                            BuiltInItemTypes[i] = new XmlQueryTypeFactory.ItemType(code, XmlQualifiedNameTest.Wildcard, builtInSimpleType, false, false, true);
                            BuiltInItemTypesStrict[i] = new XmlQueryTypeFactory.ItemType(code, XmlQualifiedNameTest.Wildcard, builtInSimpleType, false, true, true);
                            break;
                        }
                    }
                }
                UntypedDocument = new XmlQueryTypeFactory.ItemType(XmlTypeCode.Document, XmlQualifiedNameTest.Wildcard, XmlSchemaComplexType.UntypedAnyType, false, false, true);
                UntypedElement = new XmlQueryTypeFactory.ItemType(XmlTypeCode.Element, XmlQualifiedNameTest.Wildcard, XmlSchemaComplexType.UntypedAnyType, false, false, true);
                UntypedAttribute = new XmlQueryTypeFactory.ItemType(XmlTypeCode.Attribute, XmlQualifiedNameTest.Wildcard, DatatypeImplementation.UntypedAtomicType, false, false, true);
                NodeNotRtf = new XmlQueryTypeFactory.ItemType(XmlTypeCode.Node, XmlQualifiedNameTest.Wildcard, XmlSchemaComplexType.AnyType, false, false, true);
                NodeDod = new XmlQueryTypeFactory.ItemType(XmlTypeCode.Node, XmlQualifiedNameTest.Wildcard, XmlSchemaComplexType.AnyType, false, false, true);
                SpecialBuiltInItemTypes = new XmlQueryType[] { UntypedDocument, UntypedElement, UntypedAttribute, NodeNotRtf };
            }

            private ItemType(XmlTypeCode code, XmlQualifiedNameTest nameTest, XmlSchemaType schemaType, bool isNillable, bool isStrict, bool isNotRtf)
            {
                this.code = code;
                this.nameTest = nameTest;
                this.schemaType = schemaType;
                this.isNillable = isNillable;
                this.isStrict = isStrict;
                this.isNotRtf = isNotRtf;
                switch (code)
                {
                    case XmlTypeCode.Item:
                        this.nodeKinds = XmlNodeKindFlags.Any;
                        return;

                    case XmlTypeCode.Node:
                        this.nodeKinds = XmlNodeKindFlags.Any;
                        return;

                    case XmlTypeCode.Document:
                        this.nodeKinds = XmlNodeKindFlags.Document;
                        return;

                    case XmlTypeCode.Element:
                        this.nodeKinds = XmlNodeKindFlags.Element;
                        return;

                    case XmlTypeCode.Attribute:
                        this.nodeKinds = XmlNodeKindFlags.Attribute;
                        return;

                    case XmlTypeCode.Namespace:
                        this.nodeKinds = XmlNodeKindFlags.Namespace;
                        return;

                    case XmlTypeCode.ProcessingInstruction:
                        this.nodeKinds = XmlNodeKindFlags.PI;
                        return;

                    case XmlTypeCode.Comment:
                        this.nodeKinds = XmlNodeKindFlags.Comment;
                        return;

                    case XmlTypeCode.Text:
                        this.nodeKinds = XmlNodeKindFlags.Text;
                        return;
                }
                this.nodeKinds = XmlNodeKindFlags.None;
            }

            public static XmlQueryType Create(BinaryReader reader)
            {
                sbyte num = reader.ReadSByte();
                if (0 <= num)
                {
                    return Create((XmlTypeCode) num, reader.ReadBoolean());
                }
                return SpecialBuiltInItemTypes[~num];
            }

            public static XmlQueryType Create(XmlSchemaSimpleType schemaType, bool isStrict)
            {
                XmlTypeCode typeCode = schemaType.Datatype.TypeCode;
                if (schemaType == XmlSchemaType.GetBuiltInSimpleType(typeCode))
                {
                    return Create(typeCode, isStrict);
                }
                return new XmlQueryTypeFactory.ItemType(typeCode, XmlQualifiedNameTest.Wildcard, schemaType, false, isStrict, true);
            }

            public static XmlQueryType Create(XmlTypeCode code, bool isStrict)
            {
                if (isStrict)
                {
                    return BuiltInItemTypesStrict[(int) code];
                }
                return BuiltInItemTypes[(int) code];
            }

            public static XmlQueryType Create(XmlTypeCode code, XmlQualifiedNameTest nameTest, XmlSchemaType contentType, bool isNillable)
            {
                switch (code)
                {
                    case XmlTypeCode.Document:
                    case XmlTypeCode.Element:
                        if (!nameTest.IsWildcard)
                        {
                            break;
                        }
                        if (contentType != XmlSchemaComplexType.AnyType)
                        {
                            if (contentType == XmlSchemaComplexType.UntypedAnyType)
                            {
                                if (code == XmlTypeCode.Element)
                                {
                                    return UntypedElement;
                                }
                                if (code == XmlTypeCode.Document)
                                {
                                    return UntypedDocument;
                                }
                            }
                            break;
                        }
                        return Create(code, false);

                    case XmlTypeCode.Attribute:
                        if (!nameTest.IsWildcard)
                        {
                            goto Label_007E;
                        }
                        if (contentType != DatatypeImplementation.AnySimpleType)
                        {
                            if (contentType == DatatypeImplementation.UntypedAtomicType)
                            {
                                return UntypedAttribute;
                            }
                            goto Label_007E;
                        }
                        return Create(code, false);

                    default:
                        return Create(code, false);
                }
                return new XmlQueryTypeFactory.ItemType(code, nameTest, contentType, isNillable, false, true);
            Label_007E:
                return new XmlQueryTypeFactory.ItemType(code, nameTest, contentType, isNillable, false, true);
            }

            public override void GetObjectData(BinaryWriter writer)
            {
                sbyte code = (sbyte) this.code;
                for (int i = 0; i < SpecialBuiltInItemTypes.Length; i++)
                {
                    if (this == SpecialBuiltInItemTypes[i])
                    {
                        code = (sbyte) ~i;
                        break;
                    }
                }
                writer.Write(code);
                if (0 <= code)
                {
                    writer.Write(this.isStrict);
                }
            }

            public override XmlQueryCardinality Cardinality =>
                XmlQueryCardinality.One;

            public override XmlValueConverter ClrMapping
            {
                get
                {
                    if (base.IsAtomicValue)
                    {
                        return this.SchemaType.ValueConverter;
                    }
                    if (base.IsNode)
                    {
                        return XmlNodeConverter.Node;
                    }
                    return XmlAnyConverter.Item;
                }
            }

            public override int Count =>
                1;

            public override bool IsDod =>
                (this == NodeDod);

            public override bool IsNillable =>
                this.isNillable;

            public override bool IsNotRtf =>
                this.isNotRtf;

            public override bool IsStrict =>
                this.isStrict;

            public override XmlQueryType this[int index]
            {
                get
                {
                    if (index != 0)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    return this;
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            public override XmlQualifiedNameTest NameTest =>
                this.nameTest;

            public override XmlNodeKindFlags NodeKinds =>
                this.nodeKinds;

            public override XmlQueryType Prime =>
                this;

            public override XmlSchemaType SchemaType =>
                this.schemaType;

            public override XmlTypeCode TypeCode =>
                this.code;
        }

        private sealed class SequenceType : XmlQueryType
        {
            private XmlQueryCardinality card;
            private XmlValueConverter converter;
            private XmlQueryType prime;
            public static readonly XmlQueryType Zero = new XmlQueryTypeFactory.SequenceType(XmlQueryTypeFactory.ChoiceType.None, XmlQueryCardinality.Zero);

            private SequenceType(XmlQueryType prime, XmlQueryCardinality card)
            {
                this.prime = prime;
                this.card = card;
            }

            public static XmlQueryType Create(BinaryReader reader)
            {
                if (reader.ReadBoolean())
                {
                    return XmlQueryTypeFactory.NodeDodS;
                }
                XmlQueryType prime = XmlQueryTypeFactory.Deserialize(reader);
                XmlQueryCardinality card = new XmlQueryCardinality(reader);
                return Create(prime, card);
            }

            public static XmlQueryType Create(XmlQueryType prime, XmlQueryCardinality card)
            {
                if (prime.TypeCode == XmlTypeCode.None)
                {
                    if (XmlQueryCardinality.Zero > card)
                    {
                        return XmlQueryTypeFactory.None;
                    }
                    return Zero;
                }
                if (card == XmlQueryCardinality.None)
                {
                    return XmlQueryTypeFactory.None;
                }
                if (card == XmlQueryCardinality.Zero)
                {
                    return Zero;
                }
                if (card == XmlQueryCardinality.One)
                {
                    return prime;
                }
                return new XmlQueryTypeFactory.SequenceType(prime, card);
            }

            public override void GetObjectData(BinaryWriter writer)
            {
                writer.Write(this.IsDod);
                if (!this.IsDod)
                {
                    XmlQueryTypeFactory.Serialize(writer, this.prime);
                    this.card.GetObjectData(writer);
                }
            }

            public override XmlQueryCardinality Cardinality =>
                this.card;

            public override XmlValueConverter ClrMapping
            {
                get
                {
                    if (this.converter == null)
                    {
                        this.converter = XmlListConverter.Create(this.prime.ClrMapping);
                    }
                    return this.converter;
                }
            }

            public override int Count =>
                this.prime.Count;

            public override bool IsDod =>
                (this == XmlQueryTypeFactory.NodeDodS);

            public override bool IsNillable =>
                this.prime.IsNillable;

            public override bool IsNotRtf =>
                this.prime.IsNotRtf;

            public override bool IsStrict =>
                this.prime.IsStrict;

            public override XmlQueryType this[int index]
            {
                get => 
                    this.prime[index];
                set
                {
                    throw new NotSupportedException();
                }
            }

            public override XmlQualifiedNameTest NameTest =>
                this.prime.NameTest;

            public override XmlNodeKindFlags NodeKinds =>
                this.prime.NodeKinds;

            public override XmlQueryType Prime =>
                this.prime;

            public override XmlSchemaType SchemaType =>
                this.prime.SchemaType;

            public override XmlTypeCode TypeCode =>
                this.prime.TypeCode;
        }
    }
}

