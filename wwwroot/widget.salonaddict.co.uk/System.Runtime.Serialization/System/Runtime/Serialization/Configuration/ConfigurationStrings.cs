namespace System.Runtime.Serialization.Configuration
{
    using System;

    internal static class ConfigurationStrings
    {
        internal const string DataContractSerializerSectionName = "dataContractSerializer";
        internal const string DeclaredTypes = "declaredTypes";
        internal const string DefaultCollectionName = "";
        internal const string Index = "index";
        internal const string Parameter = "parameter";
        internal const string SectionGroupName = "system.runtime.serialization";
        internal const string Type = "type";

        private static string GetSectionPath(string sectionName) => 
            ("system.runtime.serialization" + "/" + sectionName);

        internal static string DataContractSerializerSectionPath =>
            GetSectionPath("dataContractSerializer");
    }
}

