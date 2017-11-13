namespace MigraDoc.RtfRendering.Resources
{
    using System;
    using System.Reflection;
    using System.Resources;

    internal class Messages
    {
        private static System.Resources.ResourceManager resourceManager;

        internal static string CharacterNotAllowedInDateFormat(char character)
        {
            string str = "";
            str = str + character;
            return FormatMessage(IDs.CharacterNotAllowedInDateFormat, new object[] { str });
        }

        private static string FormatMessage(IDs id, params object[] args)
        {
            string str;
            try
            {
                str = ResourceManager.GetString(id.ToString());
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

        internal static string ImageFreelyPlacedInWrongContext(string imageName) => 
            FormatMessage(IDs.ImageFreelyPlacedInWrongContext, new object[] { imageName });

        internal static string ImageNotFound(string imageName) => 
            FormatMessage(IDs.ImageNotFound, new object[] { imageName });

        internal static string ImageNotReadable(string imageName, string innerException) => 
            FormatMessage(IDs.ImageNotReadable, new object[] { imageName, innerException });

        internal static string ImageTypeNotSupported(string imageName) => 
            FormatMessage(IDs.ImageTypeNotSupported, new object[] { imageName });

        internal static string InvalidNumericFieldFormat(string format) => 
            FormatMessage(IDs.InvalidNumericFieldFormat, new object[] { format });

        internal static string ChartFreelyPlacedInWrongContext =>
            FormatMessage(IDs.ChartFreelyPlacedInWrongContext, new object[0]);

        private static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (resourceManager == null)
                {
                    resourceManager = new System.Resources.ResourceManager("MigraDoc.RtfRendering.Resources.Messages", Assembly.GetExecutingAssembly());
                }
                return resourceManager;
            }
        }

        internal static string TextframeContentsNotTurned =>
            FormatMessage(IDs.TextframeContentsNotTurned, new object[0]);

        internal static string UpdateField =>
            FormatMessage(IDs.UpdateField, new object[0]);

        private enum IDs
        {
            UpdateField,
            TextframeContentsNotTurned,
            InvalidNumericFieldFormat,
            ImageFreelyPlacedInWrongContext,
            ChartFreelyPlacedInWrongContext,
            ImageNotFound,
            ImageNotReadable,
            ImageTypeNotSupported,
            CharacterNotAllowedInDateFormat
        }
    }
}

