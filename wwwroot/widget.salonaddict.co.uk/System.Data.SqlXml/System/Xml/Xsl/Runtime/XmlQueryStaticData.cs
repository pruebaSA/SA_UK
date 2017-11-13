namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.Xsl;
    using System.Xml.Xsl.IlGen;
    using System.Xml.Xsl.Qil;

    internal class XmlQueryStaticData
    {
        private XmlCollation[] collations;
        private const int CurrentFormatVersion = 0;
        public const string DataFieldName = "staticData";
        private XmlWriterSettings defaultWriterSettings;
        private EarlyBoundInfo[] earlyBound;
        private Int32Pair[] filters;
        private string[] globalNames;
        private string[] names;
        private StringPair[][] prefixMappingsList;
        private XmlQueryType[] types;
        public const string TypesFieldName = "ebTypes";
        private IList<WhitespaceRule> whitespaceRules;

        public XmlQueryStaticData(byte[] data, Type[] ebTypes)
        {
            MemoryStream input = new MemoryStream(data, false);
            XmlQueryDataReader reader = new XmlQueryDataReader(input);
            if ((reader.ReadInt32Encoded() & -256) > 0)
            {
                throw new NotSupportedException();
            }
            this.defaultWriterSettings = new XmlWriterSettings(reader);
            int num = reader.ReadInt32();
            if (num != 0)
            {
                this.whitespaceRules = new WhitespaceRule[num];
                for (int i = 0; i < num; i++)
                {
                    this.whitespaceRules[i] = new WhitespaceRule(reader);
                }
            }
            num = reader.ReadInt32();
            if (num != 0)
            {
                this.names = new string[num];
                for (int j = 0; j < num; j++)
                {
                    this.names[j] = reader.ReadString();
                }
            }
            num = reader.ReadInt32();
            if (num != 0)
            {
                this.prefixMappingsList = new StringPair[num][];
                for (int k = 0; k < num; k++)
                {
                    int num6 = reader.ReadInt32();
                    this.prefixMappingsList[k] = new StringPair[num6];
                    for (int m = 0; m < num6; m++)
                    {
                        this.prefixMappingsList[k][m] = new StringPair(reader.ReadString(), reader.ReadString());
                    }
                }
            }
            num = reader.ReadInt32();
            if (num != 0)
            {
                this.filters = new Int32Pair[num];
                for (int n = 0; n < num; n++)
                {
                    this.filters[n] = new Int32Pair(reader.ReadInt32Encoded(), reader.ReadInt32Encoded());
                }
            }
            num = reader.ReadInt32();
            if (num != 0)
            {
                this.types = new XmlQueryType[num];
                for (int num9 = 0; num9 < num; num9++)
                {
                    this.types[num9] = XmlQueryTypeFactory.Deserialize(reader);
                }
            }
            num = reader.ReadInt32();
            if (num != 0)
            {
                this.collations = new XmlCollation[num];
                for (int num10 = 0; num10 < num; num10++)
                {
                    this.collations[num10] = new XmlCollation(reader);
                }
            }
            num = reader.ReadInt32();
            if (num != 0)
            {
                this.globalNames = new string[num];
                for (int num11 = 0; num11 < num; num11++)
                {
                    this.globalNames[num11] = reader.ReadString();
                }
            }
            num = reader.ReadInt32();
            if (num != 0)
            {
                this.earlyBound = new EarlyBoundInfo[num];
                for (int num12 = 0; num12 < num; num12++)
                {
                    this.earlyBound[num12] = new EarlyBoundInfo(reader.ReadString(), ebTypes[num12]);
                }
            }
            reader.Close();
        }

        public XmlQueryStaticData(XmlWriterSettings defaultWriterSettings, IList<WhitespaceRule> whitespaceRules, StaticDataManager staticData)
        {
            this.defaultWriterSettings = defaultWriterSettings;
            this.whitespaceRules = whitespaceRules;
            this.names = staticData.Names;
            this.prefixMappingsList = staticData.PrefixMappingsList;
            this.filters = staticData.NameFilters;
            this.types = staticData.XmlTypes;
            this.collations = staticData.Collations;
            this.globalNames = staticData.GlobalNames;
            this.earlyBound = staticData.EarlyBound;
        }

        public void GetObjectData(out byte[] data, out Type[] ebTypes)
        {
            MemoryStream output = new MemoryStream(0x1000);
            XmlQueryDataWriter writer = new XmlQueryDataWriter(output);
            writer.WriteInt32Encoded(0);
            this.defaultWriterSettings.GetObjectData(writer);
            if (this.whitespaceRules == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.whitespaceRules.Count);
                foreach (WhitespaceRule rule in this.whitespaceRules)
                {
                    rule.GetObjectData(writer);
                }
            }
            if (this.names == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.names.Length);
                foreach (string str in this.names)
                {
                    writer.Write(str);
                }
            }
            if (this.prefixMappingsList == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.prefixMappingsList.Length);
                foreach (StringPair[] pairArray in this.prefixMappingsList)
                {
                    writer.Write(pairArray.Length);
                    foreach (StringPair pair in pairArray)
                    {
                        writer.Write(pair.Left);
                        writer.Write(pair.Right);
                    }
                }
            }
            if (this.filters == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.filters.Length);
                foreach (Int32Pair pair2 in this.filters)
                {
                    writer.WriteInt32Encoded(pair2.Left);
                    writer.WriteInt32Encoded(pair2.Right);
                }
            }
            if (this.types == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.types.Length);
                foreach (XmlQueryType type in this.types)
                {
                    XmlQueryTypeFactory.Serialize(writer, type);
                }
            }
            if (this.collations == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.collations.Length);
                foreach (XmlCollation collation in this.collations)
                {
                    collation.GetObjectData(writer);
                }
            }
            if (this.globalNames == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.globalNames.Length);
                foreach (string str2 in this.globalNames)
                {
                    writer.Write(str2);
                }
            }
            if (this.earlyBound == null)
            {
                writer.Write(0);
                ebTypes = null;
            }
            else
            {
                writer.Write(this.earlyBound.Length);
                ebTypes = new Type[this.earlyBound.Length];
                int num = 0;
                foreach (EarlyBoundInfo info in this.earlyBound)
                {
                    writer.Write(info.NamespaceUri);
                    ebTypes[num++] = info.EarlyBoundType;
                }
            }
            writer.Close();
            data = output.ToArray();
        }

        public XmlCollation[] Collations =>
            this.collations;

        public XmlWriterSettings DefaultWriterSettings =>
            this.defaultWriterSettings;

        public EarlyBoundInfo[] EarlyBound =>
            this.earlyBound;

        public Int32Pair[] Filters =>
            this.filters;

        public string[] GlobalNames =>
            this.globalNames;

        public string[] Names =>
            this.names;

        public StringPair[][] PrefixMappingsList =>
            this.prefixMappingsList;

        public XmlQueryType[] Types =>
            this.types;

        public IList<WhitespaceRule> WhitespaceRules =>
            this.whitespaceRules;
    }
}

