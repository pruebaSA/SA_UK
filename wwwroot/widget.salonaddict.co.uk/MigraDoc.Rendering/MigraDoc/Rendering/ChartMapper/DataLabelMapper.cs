namespace MigraDoc.Rendering.ChartMapper
{
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using PdfSharp.Charting;
    using System;

    internal class DataLabelMapper
    {
        private DataLabelMapper()
        {
        }

        internal static void Map(DataLabel dataLabel, DataLabel domDataLabel)
        {
            new DataLabelMapper().MapObject(dataLabel, domDataLabel);
        }

        private void MapObject(DataLabel dataLabel, DataLabel domDataLabel)
        {
            if (!domDataLabel.IsNull("Style"))
            {
                FontMapper.Map(dataLabel.get_Font(), domDataLabel.Document, domDataLabel.Style);
            }
            if (!domDataLabel.IsNull("Font"))
            {
                FontMapper.Map(dataLabel.get_Font(), domDataLabel.Font);
            }
            dataLabel.set_Format(domDataLabel.Format);
            if (!domDataLabel.IsNull("Position"))
            {
                dataLabel.set_Position((DataLabelPosition) domDataLabel.Position);
            }
            if (!domDataLabel.IsNull("Type"))
            {
                dataLabel.set_Type((DataLabelType) domDataLabel.Type);
            }
        }
    }
}

