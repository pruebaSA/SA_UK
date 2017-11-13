namespace PdfSharp.Pdf
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class DictionaryMeta
    {
        private readonly Dictionary<string, KeyDescriptor> keyDescriptors = new Dictionary<string, KeyDescriptor>();

        public DictionaryMeta(Type type)
        {
            foreach (FieldInfo info in type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static))
            {
                object[] customAttributes = info.GetCustomAttributes(typeof(KeyInfoAttribute), false);
                if (customAttributes.Length == 1)
                {
                    KeyInfoAttribute attribute = (KeyInfoAttribute) customAttributes[0];
                    KeyDescriptor descriptor = new KeyDescriptor(attribute) {
                        KeyValue = (string) info.GetValue(null)
                    };
                    this.keyDescriptors[descriptor.KeyValue] = descriptor;
                }
            }
        }

        public KeyDescriptor this[string key] =>
            this.keyDescriptors[key];
    }
}

