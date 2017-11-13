namespace MigraDoc.Rendering.ChartMapper
{
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using PdfSharp.Charting;
    using System;

    public class PlotAreaMapper
    {
        internal static void Map(PlotArea plotArea, PlotArea domPlotArea)
        {
            new PlotAreaMapper().MapObject(plotArea, domPlotArea);
        }

        private void MapObject(PlotArea plotArea, PlotArea domPlotArea)
        {
            plotArea.set_BottomPadding(domPlotArea.BottomPadding.Point);
            plotArea.set_RightPadding(domPlotArea.RightPadding.Point);
            plotArea.set_LeftPadding(domPlotArea.LeftPadding.Point);
            plotArea.set_TopPadding(domPlotArea.TopPadding.Point);
            if (!domPlotArea.IsNull("LineFormat"))
            {
                LineFormatMapper.Map(plotArea.get_LineFormat(), domPlotArea.LineFormat);
            }
            if (!domPlotArea.IsNull("FillFormat"))
            {
                FillFormatMapper.Map(plotArea.get_FillFormat(), domPlotArea.FillFormat);
            }
        }
    }
}

