namespace System.Xml
{
    using System;

    internal class SingleArrayHelperWithDictionaryString : ArrayHelper<XmlDictionaryString, float>
    {
        public static readonly SingleArrayHelperWithDictionaryString Instance = new SingleArrayHelperWithDictionaryString();

        protected override int ReadArray(XmlDictionaryReader reader, XmlDictionaryString localName, XmlDictionaryString namespaceUri, float[] array, int offset, int count) => 
            reader.ReadArray(localName, namespaceUri, array, offset, count);

        protected override void WriteArray(XmlDictionaryWriter writer, string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, float[] array, int offset, int count)
        {
            writer.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }
    }
}

