namespace System.Drawing
{
    using System;
    using System.Drawing.Text;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;

    public sealed class FontFamily : MarshalByRefObject, IDisposable
    {
        private bool createDefaultOnFail;
        private const int LANG_NEUTRAL = 0;
        private IntPtr nativeFamily;

        public FontFamily(GenericFontFamilies genericFamily)
        {
            int num;
            IntPtr zero = IntPtr.Zero;
            switch (genericFamily)
            {
                case GenericFontFamilies.Serif:
                    num = SafeNativeMethods.Gdip.GdipGetGenericFontFamilySerif(out zero);
                    break;

                case GenericFontFamilies.SansSerif:
                    num = SafeNativeMethods.Gdip.GdipGetGenericFontFamilySansSerif(out zero);
                    break;

                default:
                    num = SafeNativeMethods.Gdip.GdipGetGenericFontFamilyMonospace(out zero);
                    break;
            }
            if (num != 0)
            {
                throw SafeNativeMethods.Gdip.StatusException(num);
            }
            this.SetNativeFamily(zero);
        }

        internal FontFamily(IntPtr family)
        {
            this.SetNativeFamily(family);
        }

        public FontFamily(string name)
        {
            this.CreateFontFamily(name, null);
        }

        internal FontFamily(string name, bool createDefaultOnFail)
        {
            this.createDefaultOnFail = createDefaultOnFail;
            this.CreateFontFamily(name, null);
        }

        public FontFamily(string name, FontCollection fontCollection)
        {
            this.CreateFontFamily(name, fontCollection);
        }

        private void CreateFontFamily(string name, FontCollection fontCollection)
        {
            IntPtr zero = IntPtr.Zero;
            IntPtr handle = (fontCollection == null) ? IntPtr.Zero : fontCollection.nativeFontCollection;
            int status = SafeNativeMethods.Gdip.GdipCreateFontFamilyFromName(name, new HandleRef(fontCollection, handle), out zero);
            if (status != 0)
            {
                if (!this.createDefaultOnFail)
                {
                    switch (status)
                    {
                        case 14:
                            throw new ArgumentException(System.Drawing.SR.GetString("GdiplusFontFamilyNotFound", new object[] { name }));

                        case 0x10:
                            throw new ArgumentException(System.Drawing.SR.GetString("GdiplusNotTrueTypeFont", new object[] { name }));
                    }
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }
                zero = GetGdipGenericSansSerif();
            }
            this.SetNativeFamily(zero);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.nativeFamily != IntPtr.Zero)
            {
                try
                {
                    SafeNativeMethods.Gdip.GdipDeleteFontFamily(new HandleRef(this, this.nativeFamily));
                }
                catch (Exception exception)
                {
                    if (System.Drawing.ClientUtils.IsCriticalException(exception))
                    {
                        throw;
                    }
                }
                finally
                {
                    this.nativeFamily = IntPtr.Zero;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            FontFamily family = obj as FontFamily;
            return (family?.NativeFamily == this.NativeFamily);
        }

        ~FontFamily()
        {
            this.Dispose(false);
        }

        public int GetCellAscent(FontStyle style)
        {
            int cellAscent = 0;
            int status = SafeNativeMethods.Gdip.GdipGetCellAscent(new HandleRef(this, this.NativeFamily), style, out cellAscent);
            if (status != 0)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
            return cellAscent;
        }

        public int GetCellDescent(FontStyle style)
        {
            int cellDescent = 0;
            int status = SafeNativeMethods.Gdip.GdipGetCellDescent(new HandleRef(this, this.NativeFamily), style, out cellDescent);
            if (status != 0)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
            return cellDescent;
        }

        public int GetEmHeight(FontStyle style)
        {
            int emHeight = 0;
            int status = SafeNativeMethods.Gdip.GdipGetEmHeight(new HandleRef(this, this.NativeFamily), style, out emHeight);
            if (status != 0)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
            return emHeight;
        }

        [Obsolete("Do not use method GetFamilies, use property Families instead")]
        public static FontFamily[] GetFamilies(Graphics graphics)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }
            return new InstalledFontCollection().Families;
        }

        private static IntPtr GetGdipGenericSansSerif()
        {
            IntPtr zero = IntPtr.Zero;
            int status = SafeNativeMethods.Gdip.GdipGetGenericFontFamilySansSerif(out zero);
            if (status != 0)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
            return zero;
        }

        public override int GetHashCode() => 
            this.GetName(0).GetHashCode();

        public int GetLineSpacing(FontStyle style)
        {
            int lineSpaceing = 0;
            int status = SafeNativeMethods.Gdip.GdipGetLineSpacing(new HandleRef(this, this.NativeFamily), style, out lineSpaceing);
            if (status != 0)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
            return lineSpaceing;
        }

        public string GetName(int language)
        {
            StringBuilder name = new StringBuilder(0x20);
            int status = SafeNativeMethods.Gdip.GdipGetFamilyName(new HandleRef(this, this.NativeFamily), name, language);
            if (status != 0)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
            return name.ToString();
        }

        private static IntPtr GetNativeGenericMonospace()
        {
            IntPtr zero = IntPtr.Zero;
            int status = SafeNativeMethods.Gdip.GdipGetGenericFontFamilyMonospace(out zero);
            if (status != 0)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
            return zero;
        }

        private static IntPtr GetNativeGenericSerif()
        {
            IntPtr zero = IntPtr.Zero;
            int status = SafeNativeMethods.Gdip.GdipGetGenericFontFamilySerif(out zero);
            if (status != 0)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
            return zero;
        }

        public bool IsStyleAvailable(FontStyle style)
        {
            int num;
            int status = SafeNativeMethods.Gdip.GdipIsStyleAvailable(new HandleRef(this, this.NativeFamily), style, out num);
            if (status != 0)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
            return (num != 0);
        }

        private void SetNativeFamily(IntPtr family)
        {
            this.nativeFamily = family;
        }

        public override string ToString() => 
            string.Format(CultureInfo.CurrentCulture, "[{0}: Name={1}]", new object[] { base.GetType().Name, this.Name });

        private static int CurrentLanguage =>
            CultureInfo.CurrentUICulture.LCID;

        public static FontFamily[] Families =>
            new InstalledFontCollection().Families;

        public static FontFamily GenericMonospace =>
            new FontFamily(GetNativeGenericMonospace());

        public static FontFamily GenericSansSerif =>
            new FontFamily(GetGdipGenericSansSerif());

        public static FontFamily GenericSerif =>
            new FontFamily(GetNativeGenericSerif());

        public string Name =>
            this.GetName(CurrentLanguage);

        internal IntPtr NativeFamily =>
            this.nativeFamily;
    }
}

