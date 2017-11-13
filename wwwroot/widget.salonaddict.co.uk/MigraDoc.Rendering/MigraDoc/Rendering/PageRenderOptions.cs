namespace MigraDoc.Rendering
{
    using System;

    [Flags]
    public enum PageRenderOptions
    {
        All = 0x1f,
        None = 0,
        RemovePage = 0x20,
        RenderContent = 4,
        RenderFooter = 2,
        RenderHeader = 1,
        RenderPdfBackground = 8,
        RenderPdfContent = 0x10
    }
}

