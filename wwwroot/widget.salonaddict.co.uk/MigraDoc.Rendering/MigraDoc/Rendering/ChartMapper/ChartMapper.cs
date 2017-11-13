namespace MigraDoc.Rendering.ChartMapper
{
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using PdfSharp.Charting;
    using PdfSharp.Drawing;
    using System;

    public class ChartMapper
    {
        public static ChartFrame Map(Chart domChart)
        {
            MigraDoc.Rendering.ChartMapper.ChartMapper mapper = new MigraDoc.Rendering.ChartMapper.ChartMapper();
            return mapper.MapObject(domChart);
        }

        private ChartFrame MapObject(Chart domChart)
        {
            ChartFrame frame = new ChartFrame();
            frame.set_Size(new XSize(domChart.Width.Point, domChart.Height.Point));
            frame.set_Location(new XPoint(domChart.Left.Position.Point, domChart.Top.Position.Point));
            Chart chart = new Chart((ChartType) domChart.Type);
            if (!domChart.IsNull("XAxis"))
            {
                AxisMapper.Map(chart.get_XAxis(), domChart.XAxis);
            }
            if (!domChart.IsNull("YAxis"))
            {
                AxisMapper.Map(chart.get_YAxis(), domChart.YAxis);
            }
            PlotAreaMapper.Map(chart.get_PlotArea(), domChart.PlotArea);
            SeriesCollectionMapper.Map(chart.get_SeriesCollection(), domChart.SeriesCollection);
            LegendMapper.Map(chart, domChart);
            chart.set_DisplayBlanksAs((BlankType) domChart.DisplayBlanksAs);
            chart.set_HasDataLabel(domChart.HasDataLabel);
            if (!domChart.IsNull("DataLabel"))
            {
                DataLabelMapper.Map(chart.get_DataLabel(), domChart.DataLabel);
            }
            if (!domChart.IsNull("Style"))
            {
                FontMapper.Map(chart.get_Font(), domChart.Document, domChart.Style);
            }
            if (!domChart.IsNull("Format.Font"))
            {
                FontMapper.Map(chart.get_Font(), domChart.Format.Font);
            }
            if (!domChart.IsNull("XValues"))
            {
                XValuesMapper.Map(chart.get_XValues(), domChart.XValues);
            }
            frame.Add(chart);
            return frame;
        }
    }
}

