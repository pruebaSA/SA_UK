namespace MS.Internal.FontRasterization
{
    using MS.Internal.WindowsBase;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    [FriendAccessAllowed]
    internal interface IFontRasterizer : IDisposable
    {
        void GetBitmap(GetMemoryCallback getMemoryCallback, IntPtr currentPointer, int currentSize, out GlyphBitmap glyphBitmap, out GlyphMetrics glyphMetrics);
        void GetMetrics(out GlyphMetrics glyphMetrics);
        void GetOutline(GetMemoryCallback getMemoryCallback, IntPtr currentPointer, int currentSize, out GlyphOutline glyphOutline, out GlyphMetrics glyphMetrics);
        ushort NewFont(UnmanagedMemoryStream fontFileStream, Uri sourceUri, int faceIndex);
        void NewGlyph(ushort glyphIndex);
        void NewTransform(ushort pointSize, Transform transform, OverscaleMode overscaleMode, RenderingFlags renderingFlags);
    }
}

