namespace MigraDoc.Rendering
{
    using System;
    using System.Collections;

    internal interface IAreaProvider
    {
        Area GetNextArea();
        bool IsAreaBreakBefore(LayoutInfo layoutInfo);
        bool PositionHorizontally(LayoutInfo layoutInfo);
        bool PositionVertically(LayoutInfo layoutInfo);
        Area ProbeNextArea();
        void StoreRenderInfos(ArrayList renderInfos);

        FieldInfos AreaFieldInfos { get; }
    }
}

