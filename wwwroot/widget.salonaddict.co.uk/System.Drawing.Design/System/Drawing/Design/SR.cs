namespace System.Drawing.Design
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Threading;

    internal sealed class SR
    {
        internal const string bitmapFileDescription = "bitmapFileDescription";
        internal const string ColorEditorAccName = "ColorEditorAccName";
        internal const string ColorEditorPaletteTab = "ColorEditorPaletteTab";
        internal const string ColorEditorStandardTab = "ColorEditorStandardTab";
        internal const string ColorEditorSystemTab = "ColorEditorSystemTab";
        internal const string ContentAlignmentEditorAccName = "ContentAlignmentEditorAccName";
        internal const string ContentAlignmentEditorBottomCenterAccName = "ContentAlignmentEditorBottomCenterAccName";
        internal const string ContentAlignmentEditorBottomLeftAccName = "ContentAlignmentEditorBottomLeftAccName";
        internal const string ContentAlignmentEditorBottomRightAccName = "ContentAlignmentEditorBottomRightAccName";
        internal const string ContentAlignmentEditorMiddleCenterAccName = "ContentAlignmentEditorMiddleCenterAccName";
        internal const string ContentAlignmentEditorMiddleLeftAccName = "ContentAlignmentEditorMiddleLeftAccName";
        internal const string ContentAlignmentEditorMiddleRightAccName = "ContentAlignmentEditorMiddleRightAccName";
        internal const string ContentAlignmentEditorTopCenterAccName = "ContentAlignmentEditorTopCenterAccName";
        internal const string ContentAlignmentEditorTopLeftAccName = "ContentAlignmentEditorTopLeftAccName";
        internal const string ContentAlignmentEditorTopRightAccName = "ContentAlignmentEditorTopRightAccName";
        internal const string iconFileDescription = "iconFileDescription";
        internal const string imageFileDescription = "imageFileDescription";
        private static System.Drawing.Design.SR loader;
        internal const string metafileFileDescription = "metafileFileDescription";
        private ResourceManager resources;
        private static object s_InternalSyncObject;
        internal const string ToolboxServiceAssemblyNotFound = "ToolboxServiceAssemblyNotFound";
        internal const string ToolboxServiceBadToolboxItem = "ToolboxServiceBadToolboxItem";
        internal const string ToolboxServiceBadToolboxItemWithException = "ToolboxServiceBadToolboxItemWithException";

        internal SR()
        {
            this.resources = new ResourceManager("System.Drawing.Design.SR", base.GetType().Assembly);
        }

        private static System.Drawing.Design.SR GetLoader()
        {
            if (loader == null)
            {
                lock (InternalSyncObject)
                {
                    if (loader == null)
                    {
                        loader = new System.Drawing.Design.SR();
                    }
                }
            }
            return loader;
        }

        public static object GetObject(string name)
        {
            System.Drawing.Design.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetObject(name, Culture);
        }

        public static string GetString(string name)
        {
            System.Drawing.Design.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetString(name, Culture);
        }

        public static string GetString(string name, params object[] args)
        {
            System.Drawing.Design.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            string format = loader.resources.GetString(name, Culture);
            if ((args == null) || (args.Length <= 0))
            {
                return format;
            }
            for (int i = 0; i < args.Length; i++)
            {
                string str2 = args[i] as string;
                if ((str2 != null) && (str2.Length > 0x400))
                {
                    args[i] = str2.Substring(0, 0x3fd) + "...";
                }
            }
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        private static CultureInfo Culture =>
            null;

        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, obj2, null);
                }
                return s_InternalSyncObject;
            }
        }

        public static ResourceManager Resources =>
            GetLoader().resources;
    }
}

