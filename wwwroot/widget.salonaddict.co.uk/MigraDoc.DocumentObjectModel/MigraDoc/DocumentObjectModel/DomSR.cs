namespace MigraDoc.DocumentObjectModel
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Resources;

    internal static class DomSR
    {
        private static ResourceManager resmngr;

        public static string FormatMessage(DomMsgID id, params object[] args)
        {
            string str;
            try
            {
                str = GetString(id);
                if (str != null)
                {
                    return string.Format(str, args);
                }
                return "<<<error: message not found>>>";
            }
            catch (Exception exception)
            {
                str = "INTERNAL ERROR while formatting error message: " + exception.ToString();
            }
            return str;
        }

        public static string GetString(DomMsgID id) => 
            ResMngr.GetString(id.ToString());

        public static string InsertNullNotAllowed() => 
            "Insert null not allowed.";

        public static string InvalidColorString(string colorString) => 
            FormatMessage(DomMsgID.InvalidColorString, new object[] { colorString });

        public static string InvalidFieldFormat(string format) => 
            FormatMessage(DomMsgID.InvalidFieldFormat, new object[] { format });

        public static string InvalidFontSize(double value) => 
            FormatMessage(DomMsgID.InvalidFontSize, new object[] { value });

        public static string InvalidInfoFieldName(string name) => 
            FormatMessage(DomMsgID.InvalidInfoFieldName, new object[] { name });

        public static string InvalidUnitType(string unitType) => 
            FormatMessage(DomMsgID.InvalidUnitType, new object[] { unitType });

        public static string InvalidUnitValue(string unitValue) => 
            FormatMessage(DomMsgID.InvalidUnitValue, new object[] { unitValue });

        public static string InvalidValueName(string name) => 
            FormatMessage(DomMsgID.InvalidValueName, new object[] { name });

        public static string MissingObligatoryProperty(string propertyName, string className) => 
            $"ObigatoryProperty '{propertyName}' not set in '{className}'.";

        public static string ParentAlreadySet(DocumentObject value, DocumentObject docObject) => 
            $"Value of type '{value.GetType().ToString()}' must be cloned before set into '{docObject.GetType().ToString()}'.";

        [Conditional("DEBUG")]
        public static void TestResourceMessages()
        {
            foreach (string str in Enum.GetNames(typeof(DomMsgID)))
            {
                $"{str}: '{ResMngr.GetString(str)}'";
            }
        }

        public static string UndefinedBaseStyle(string baseStyle) => 
            FormatMessage(DomMsgID.UndefinedBaseStyle, new object[] { baseStyle });

        public static string BaseStyleRequired =>
            GetString(DomMsgID.BaseStyleRequired);

        public static string CompareJustCells =>
            "Only cells can be compared by this Comparer.";

        public static string EmptyBaseStyle =>
            GetString(DomMsgID.EmptyBaseStyle);

        public static string InvalidDocumentObjectType =>
            "The given document object is not valid in this context.";

        public static string InvalidEnumForLeftPosition =>
            GetString(DomMsgID.InvalidEnumForLeftPosition);

        public static string InvalidEnumForTopPosition =>
            GetString(DomMsgID.InvalidEnumForTopPosition);

        public static ResourceManager ResMngr
        {
            get
            {
                if (resmngr == null)
                {
                    resmngr = new ResourceManager("MigraDoc.DocumentObjectModel.Resources.Messages", Assembly.GetExecutingAssembly());
                }
                return resmngr;
            }
        }

        public static string StyleExpected =>
            GetString(DomMsgID.StyleExpected);
    }
}

