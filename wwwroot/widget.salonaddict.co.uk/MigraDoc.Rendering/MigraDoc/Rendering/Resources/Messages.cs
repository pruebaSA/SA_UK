namespace MigraDoc.Rendering.Resources
{
    using System;
    using System.Reflection;
    using System.Resources;

    internal static class Messages
    {
        private static System.Resources.ResourceManager resourceManager;

        internal static string BookmarkNotDefined(string bookmarkName) => 
            FormatMessage(IDs.BookmarkNotDefined, new object[] { bookmarkName });

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

        internal static string ImageNotFound(string imageName) => 
            FormatMessage(IDs.ImageNotFound, new object[] { imageName });

        internal static string ImageNotReadable(string imageName, string innerException) => 
            FormatMessage(IDs.ImageNotReadable, new object[] { imageName, innerException });

        internal static string InvalidImageType(string type) => 
            FormatMessage(IDs.InvalidImageType, new object[] { type });

        internal static string NumberTooLargeForLetters(int number) => 
            FormatMessage(IDs.NumberTooLargeForLetters, new object[] { number });

        internal static string NumberTooLargeForRoman(int number) => 
            FormatMessage(IDs.NumberTooLargeForRoman, new object[] { number });

        internal static string PropertyNotSetBefore(string propertyName, string functionName) => 
            FormatMessage(IDs.PropertyNotSetBefore, new object[] { propertyName, functionName });

        internal static string DisplayEmptyImageSize =>
            FormatMessage(IDs.DisplayEmptyImageSize, new object[0]);

        internal static string DisplayImageFileNotFound =>
            FormatMessage(IDs.DisplayImageFileNotFound, new object[0]);

        internal static string DisplayImageNotRead =>
            FormatMessage(IDs.DisplayImageNotRead, new object[0]);

        internal static string DisplayInvalidImageType =>
            FormatMessage(IDs.DisplayInvalidImageType, new object[0]);

        internal static string EmptyImageSize =>
            FormatMessage(IDs.EmptyImageSize, new object[0]);

        internal static string ObjectNotRenderable =>
            FormatMessage(IDs.ObjectNotRenderable, new object[0]);

        private static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (resourceManager == null)
                {
                    resourceManager = new System.Resources.ResourceManager("MigraDoc.Rendering.Resources.Messages", Assembly.GetExecutingAssembly());
                }
                return resourceManager;
            }
        }

        private enum IDs
        {
            PropertyNotSetBefore,
            BookmarkNotDefined,
            ImageNotFound,
            InvalidImageType,
            ImageNotReadable,
            EmptyImageSize,
            ObjectNotRenderable,
            NumberTooLargeForRoman,
            NumberTooLargeForLetters,
            DisplayEmptyImageSize,
            DisplayImageFileNotFound,
            DisplayInvalidImageType,
            DisplayImageNotRead
        }
    }
}

