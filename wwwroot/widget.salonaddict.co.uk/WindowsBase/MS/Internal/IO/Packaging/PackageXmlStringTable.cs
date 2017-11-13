namespace MS.Internal.IO.Packaging
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal static class PackageXmlStringTable
    {
        private static System.Xml.NameTable _nameTable = new System.Xml.NameTable();
        private static XmlStringTableStruct[] _xmlstringtable = new XmlStringTableStruct[0x1b];

        static PackageXmlStringTable()
        {
            object nameString = _nameTable.Add("http://www.w3.org/2001/XMLSchema-instance");
            _xmlstringtable[1] = new XmlStringTableStruct(nameString, PackageXmlEnum.NotDefined, null);
            nameString = _nameTable.Add("xsi");
            _xmlstringtable[2] = new XmlStringTableStruct(nameString, PackageXmlEnum.NotDefined, null);
            nameString = _nameTable.Add("xmlns");
            _xmlstringtable[3] = new XmlStringTableStruct(nameString, PackageXmlEnum.NotDefined, null);
            nameString = _nameTable.Add("http://schemas.openxmlformats.org/package/2006/metadata/core-properties");
            _xmlstringtable[4] = new XmlStringTableStruct(nameString, PackageXmlEnum.NotDefined, null);
            nameString = _nameTable.Add("http://purl.org/dc/elements/1.1/");
            _xmlstringtable[5] = new XmlStringTableStruct(nameString, PackageXmlEnum.NotDefined, null);
            nameString = _nameTable.Add("http://purl.org/dc/terms/");
            _xmlstringtable[6] = new XmlStringTableStruct(nameString, PackageXmlEnum.NotDefined, null);
            nameString = _nameTable.Add("dc");
            _xmlstringtable[7] = new XmlStringTableStruct(nameString, PackageXmlEnum.NotDefined, null);
            nameString = _nameTable.Add("dcterms");
            _xmlstringtable[8] = new XmlStringTableStruct(nameString, PackageXmlEnum.NotDefined, null);
            nameString = _nameTable.Add("coreProperties");
            _xmlstringtable[9] = new XmlStringTableStruct(nameString, PackageXmlEnum.PackageCorePropertiesNamespace, "NotSpecified");
            nameString = _nameTable.Add("type");
            _xmlstringtable[10] = new XmlStringTableStruct(nameString, PackageXmlEnum.NotDefined, "NotSpecified");
            nameString = _nameTable.Add("creator");
            _xmlstringtable[11] = new XmlStringTableStruct(nameString, PackageXmlEnum.DublinCorePropertiesNamespace, "String");
            nameString = _nameTable.Add("identifier");
            _xmlstringtable[12] = new XmlStringTableStruct(nameString, PackageXmlEnum.DublinCorePropertiesNamespace, "String");
            nameString = _nameTable.Add("title");
            _xmlstringtable[13] = new XmlStringTableStruct(nameString, PackageXmlEnum.DublinCorePropertiesNamespace, "String");
            nameString = _nameTable.Add("subject");
            _xmlstringtable[14] = new XmlStringTableStruct(nameString, PackageXmlEnum.DublinCorePropertiesNamespace, "String");
            nameString = _nameTable.Add("description");
            _xmlstringtable[15] = new XmlStringTableStruct(nameString, PackageXmlEnum.DublinCorePropertiesNamespace, "String");
            nameString = _nameTable.Add("language");
            _xmlstringtable[0x10] = new XmlStringTableStruct(nameString, PackageXmlEnum.DublinCorePropertiesNamespace, "String");
            nameString = _nameTable.Add("created");
            _xmlstringtable[0x11] = new XmlStringTableStruct(nameString, PackageXmlEnum.DublinCoreTermsNamespace, "DateTime");
            nameString = _nameTable.Add("modified");
            _xmlstringtable[0x12] = new XmlStringTableStruct(nameString, PackageXmlEnum.DublinCoreTermsNamespace, "DateTime");
            nameString = _nameTable.Add("contentType");
            _xmlstringtable[0x13] = new XmlStringTableStruct(nameString, PackageXmlEnum.PackageCorePropertiesNamespace, "String");
            nameString = _nameTable.Add("keywords");
            _xmlstringtable[20] = new XmlStringTableStruct(nameString, PackageXmlEnum.PackageCorePropertiesNamespace, "String");
            nameString = _nameTable.Add("category");
            _xmlstringtable[0x15] = new XmlStringTableStruct(nameString, PackageXmlEnum.PackageCorePropertiesNamespace, "String");
            nameString = _nameTable.Add("version");
            _xmlstringtable[0x16] = new XmlStringTableStruct(nameString, PackageXmlEnum.PackageCorePropertiesNamespace, "String");
            nameString = _nameTable.Add("lastModifiedBy");
            _xmlstringtable[0x17] = new XmlStringTableStruct(nameString, PackageXmlEnum.PackageCorePropertiesNamespace, "String");
            nameString = _nameTable.Add("contentStatus");
            _xmlstringtable[0x18] = new XmlStringTableStruct(nameString, PackageXmlEnum.PackageCorePropertiesNamespace, "String");
            nameString = _nameTable.Add("revision");
            _xmlstringtable[0x19] = new XmlStringTableStruct(nameString, PackageXmlEnum.PackageCorePropertiesNamespace, "String");
            nameString = _nameTable.Add("lastPrinted");
            _xmlstringtable[0x1a] = new XmlStringTableStruct(nameString, PackageXmlEnum.PackageCorePropertiesNamespace, "DateTime");
        }

        private static void CheckIdRange(PackageXmlEnum id)
        {
            if ((id <= PackageXmlEnum.NotDefined) || (id >= (PackageXmlEnum.LastPrinted | PackageXmlEnum.XmlSchemaInstanceNamespace)))
            {
                throw new ArgumentOutOfRangeException("id");
            }
        }

        internal static PackageXmlEnum GetEnumOf(object xmlString)
        {
            for (int i = 1; i < _xmlstringtable.GetLength(0); i++)
            {
                if (object.ReferenceEquals(_xmlstringtable[i].Name, xmlString))
                {
                    return (PackageXmlEnum) i;
                }
            }
            return PackageXmlEnum.NotDefined;
        }

        internal static string GetValueType(PackageXmlEnum id)
        {
            CheckIdRange(id);
            return _xmlstringtable[(int) id].ValueType;
        }

        internal static PackageXmlEnum GetXmlNamespace(PackageXmlEnum id)
        {
            CheckIdRange(id);
            return _xmlstringtable[(int) id].Namespace;
        }

        internal static string GetXmlString(PackageXmlEnum id)
        {
            CheckIdRange(id);
            return (string) _xmlstringtable[(int) id].Name;
        }

        internal static object GetXmlStringAsObject(PackageXmlEnum id)
        {
            CheckIdRange(id);
            return _xmlstringtable[(int) id].Name;
        }

        internal static System.Xml.NameTable NameTable =>
            _nameTable;

        [StructLayout(LayoutKind.Sequential)]
        private struct XmlStringTableStruct
        {
            private object _nameString;
            private PackageXmlEnum _namespace;
            private string _valueType;
            internal XmlStringTableStruct(object nameString, PackageXmlEnum ns, string valueType)
            {
                this._nameString = nameString;
                this._namespace = ns;
                this._valueType = valueType;
            }

            internal object Name =>
                ((string) this._nameString);
            internal PackageXmlEnum Namespace =>
                this._namespace;
            internal string ValueType =>
                this._valueType;
        }
    }
}

