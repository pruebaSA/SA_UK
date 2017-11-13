namespace PdfSharp.Drawing
{
    using System;
    using System.Drawing;
    using System.Drawing.Text;
    using System.Runtime.InteropServices;

    public sealed class XPrivateFontCollection : IDisposable
    {
        private PrivateFontCollection privateFontCollection = new PrivateFontCollection();
        internal static XPrivateFontCollection s_global = new XPrivateFontCollection();

        public void AddFont(string filename)
        {
            throw new NotImplementedException("AddFont");
        }

        public void AddFont(byte[] data)
        {
            throw new NotImplementedException("AddFont");
        }

        public void AddFont(byte[] data, string familyName)
        {
            if (string.IsNullOrEmpty(familyName))
            {
                throw new ArgumentNullException("familyName");
            }
            int length = data.Length;
            IntPtr destination = Marshal.AllocCoTaskMem(length);
            Marshal.Copy(data, 0, destination, length);
            this.privateFontCollection.AddMemoryFont(destination, length);
            Marshal.FreeCoTaskMem(destination);
        }

        public void AddFont(byte[] data, string fontName, bool bold, bool italic)
        {
            throw new NotImplementedException("AddFont");
        }

        public void Dispose()
        {
            this.privateFontCollection.Dispose();
            this.privateFontCollection = new PrivateFontCollection();
            s_global = null;
        }

        public static XPrivateFontCollection SetGlobalFontCollection(XPrivateFontCollection fontCollection)
        {
            if (fontCollection == null)
            {
                throw new ArgumentNullException("fontCollection");
            }
            XPrivateFontCollection fonts = s_global;
            s_global = fontCollection;
            return fonts;
        }

        internal static Font TryFindPrivateFont(string name, double size, FontStyle style)
        {
            try
            {
                PrivateFontCollection globalPrivateFontCollection = GlobalPrivateFontCollection;
                if (globalPrivateFontCollection == null)
                {
                    return null;
                }
                foreach (FontFamily family in globalPrivateFontCollection.Families)
                {
                    if (string.Compare(family.Name, name, true) == 0)
                    {
                        return new Font(family, (float) size, style, GraphicsUnit.World);
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public static XPrivateFontCollection Global =>
            s_global;

        internal static PrivateFontCollection GlobalPrivateFontCollection
        {
            get
            {
                if (s_global == null)
                {
                    return null;
                }
                return s_global.privateFontCollection;
            }
        }
    }
}

