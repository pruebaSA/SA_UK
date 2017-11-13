namespace MigraDoc.Rendering.ChartMapper
{
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using PdfSharp.Charting;
    using System;

    public class XValuesMapper
    {
        internal static void Map(XValues xValues, XValues domXValues)
        {
            new XValuesMapper().MapObject(xValues, domXValues);
        }

        private void MapObject(XValues xValues, XValues domXValues)
        {
            foreach (XSeries series in domXValues)
            {
                XSeries series2 = xValues.AddXSeries();
                XSeriesElements elements = series.GetValue("XSeriesElements") as XSeriesElements;
                foreach (XValue value2 in elements)
                {
                    if (value2 == null)
                    {
                        series2.AddBlank();
                    }
                    else
                    {
                        series2.Add(value2.GetValue("Value").ToString());
                    }
                }
            }
        }
    }
}

