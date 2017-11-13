namespace MigraDoc.Rendering.ChartMapper
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using PdfSharp.Charting;
    using System;

    internal class LegendMapper
    {
        private LegendMapper()
        {
        }

        internal static void Map(Chart chart, Chart domChart)
        {
            new LegendMapper().MapObject(chart, domChart);
        }

        private void MapObject(Chart chart, Chart domChart)
        {
            Legend legend = null;
            TextArea bottomArea = null;
            foreach (DocumentObject obj2 in domChart.BottomArea.Elements)
            {
                if (obj2 is Legend)
                {
                    chart.get_Legend().set_Docking(1);
                    legend = obj2 as Legend;
                    bottomArea = domChart.BottomArea;
                }
            }
            foreach (DocumentObject obj3 in domChart.RightArea.Elements)
            {
                if (obj3 is Legend)
                {
                    chart.get_Legend().set_Docking(3);
                    legend = obj3 as Legend;
                    bottomArea = domChart.RightArea;
                }
            }
            foreach (DocumentObject obj4 in domChart.LeftArea.Elements)
            {
                if (obj4 is Legend)
                {
                    chart.get_Legend().set_Docking(2);
                    legend = obj4 as Legend;
                    bottomArea = domChart.LeftArea;
                }
            }
            foreach (DocumentObject obj5 in domChart.TopArea.Elements)
            {
                if (obj5 is Legend)
                {
                    chart.get_Legend().set_Docking(0);
                    legend = obj5 as Legend;
                    bottomArea = domChart.TopArea;
                }
            }
            foreach (DocumentObject obj6 in domChart.HeaderArea.Elements)
            {
                if (obj6 is Legend)
                {
                    chart.get_Legend().set_Docking(0);
                    legend = obj6 as Legend;
                    bottomArea = domChart.HeaderArea;
                }
            }
            foreach (DocumentObject obj7 in domChart.FooterArea.Elements)
            {
                if (obj7 is Legend)
                {
                    chart.get_Legend().set_Docking(1);
                    legend = obj7 as Legend;
                    bottomArea = domChart.FooterArea;
                }
            }
            if (legend != null)
            {
                if (!legend.IsNull("LineFormat"))
                {
                    LineFormatMapper.Map(chart.get_Legend().get_LineFormat(), legend.LineFormat);
                }
                if (!bottomArea.IsNull("Style"))
                {
                    FontMapper.Map(chart.get_Legend().get_Font(), bottomArea.Document, bottomArea.Style);
                }
                if (!legend.IsNull("Format.Font"))
                {
                    FontMapper.Map(chart.get_Legend().get_Font(), legend.Format.Font);
                }
            }
        }
    }
}

