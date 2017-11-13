namespace MigraDoc.Rendering.ChartMapper
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using MigraDoc.Rendering;
    using PdfSharp.Charting;
    using PdfSharp.Drawing;
    using System;

    public class SeriesCollectionMapper
    {
        internal static void Map(SeriesCollection seriesCollection, SeriesCollection domSeriesCollection)
        {
            new SeriesCollectionMapper().MapObject(seriesCollection, domSeriesCollection);
        }

        private void MapObject(SeriesCollection seriesCollection, SeriesCollection domSeriesCollection)
        {
            foreach (Series series in domSeriesCollection)
            {
                Series series2 = seriesCollection.AddSeries();
                series2.set_Name(series.Name);
                if (series.IsNull("ChartType"))
                {
                    Chart parentOfType = (Chart) DocumentRelations.GetParentOfType(series, typeof(Chart));
                    series2.set_ChartType((ChartType) parentOfType.Type);
                }
                else
                {
                    series2.set_ChartType((ChartType) series.ChartType);
                }
                if (!series.IsNull("DataLabel"))
                {
                    DataLabelMapper.Map(series2.get_DataLabel(), series.DataLabel);
                }
                if (!series.IsNull("LineFormat"))
                {
                    LineFormatMapper.Map(series2.get_LineFormat(), series.LineFormat);
                }
                if (!series.IsNull("FillFormat"))
                {
                    FillFormatMapper.Map(series2.get_FillFormat(), series.FillFormat);
                }
                series2.set_HasDataLabel(series.HasDataLabel);
                if (series.MarkerBackgroundColor.IsEmpty)
                {
                    series2.set_MarkerBackgroundColor(XColor.Empty);
                }
                else
                {
                    series2.set_MarkerBackgroundColor(ColorHelper.ToXColor(series.MarkerBackgroundColor, series.Document.UseCmykColor));
                }
                if (series.MarkerForegroundColor.IsEmpty)
                {
                    series2.set_MarkerForegroundColor(XColor.Empty);
                }
                else
                {
                    series2.set_MarkerForegroundColor(ColorHelper.ToXColor(series.MarkerForegroundColor, series.Document.UseCmykColor));
                }
                series2.set_MarkerSize(series.MarkerSize.Point);
                if (!series.IsNull("MarkerStyle"))
                {
                    series2.set_MarkerStyle((MarkerStyle) series.MarkerStyle);
                }
                foreach (Point point in series.Elements)
                {
                    if (point != null)
                    {
                        Point point2 = series2.Add(point.Value);
                        FillFormatMapper.Map(point2.get_FillFormat(), point.FillFormat);
                        LineFormatMapper.Map(point2.get_LineFormat(), point.LineFormat);
                    }
                    else
                    {
                        series2.Add(double.NaN);
                    }
                }
            }
        }
    }
}

