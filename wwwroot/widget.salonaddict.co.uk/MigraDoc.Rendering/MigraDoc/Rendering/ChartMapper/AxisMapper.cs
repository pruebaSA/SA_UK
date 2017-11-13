namespace MigraDoc.Rendering.ChartMapper
{
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using PdfSharp.Charting;
    using System;

    public class AxisMapper
    {
        internal static void Map(Axis axis, Axis domAxis)
        {
            new AxisMapper().MapObject(axis, domAxis);
        }

        private void MapObject(Axis axis, Axis domAxis)
        {
            if (!domAxis.IsNull("TickLabels.Format"))
            {
                axis.get_TickLabels().set_Format(domAxis.TickLabels.Format);
            }
            if (!domAxis.IsNull("TickLabels.Style"))
            {
                FontMapper.Map(axis.get_TickLabels().get_Font(), domAxis.TickLabels.Document, domAxis.TickLabels.Style);
            }
            if (!domAxis.IsNull("TickLabels.Font"))
            {
                FontMapper.Map(axis.get_TickLabels().get_Font(), domAxis.TickLabels.Font);
            }
            if (!domAxis.IsNull("MajorTickMark"))
            {
                axis.set_MajorTickMark((TickMarkType) domAxis.MajorTickMark);
            }
            if (!domAxis.IsNull("MinorTickMark"))
            {
                axis.set_MinorTickMark((TickMarkType) domAxis.MinorTickMark);
            }
            if (!domAxis.IsNull("MajorTick"))
            {
                axis.set_MajorTick(domAxis.MajorTick);
            }
            if (!domAxis.IsNull("MinorTick"))
            {
                axis.set_MinorTick(domAxis.MinorTick);
            }
            if (!domAxis.IsNull("Title"))
            {
                axis.get_Title().set_Caption(domAxis.Title.Caption);
                if (!domAxis.IsNull("Title.Style"))
                {
                    FontMapper.Map(axis.get_Title().get_Font(), domAxis.Title.Document, domAxis.Title.Style);
                }
                if (!domAxis.IsNull("Title.Font"))
                {
                    FontMapper.Map(axis.get_Title().get_Font(), domAxis.Title.Font);
                }
                axis.get_Title().set_Orientation(domAxis.Title.Orientation.Value);
                axis.get_Title().set_Alignment((HorizontalAlignment) domAxis.Title.Alignment);
                axis.get_Title().set_VerticalAlignment((VerticalAlignment) domAxis.Title.VerticalAlignment);
            }
            axis.set_HasMajorGridlines(domAxis.HasMajorGridlines);
            axis.set_HasMinorGridlines(domAxis.HasMinorGridlines);
            if (!domAxis.IsNull("MajorGridlines") && !domAxis.MajorGridlines.IsNull("LineFormat"))
            {
                LineFormatMapper.Map(axis.get_MajorGridlines().get_LineFormat(), domAxis.MajorGridlines.LineFormat);
            }
            if (!domAxis.IsNull("MinorGridlines") && !domAxis.MinorGridlines.IsNull("LineFormat"))
            {
                LineFormatMapper.Map(axis.get_MinorGridlines().get_LineFormat(), domAxis.MinorGridlines.LineFormat);
            }
            if (!domAxis.IsNull("MaximumScale"))
            {
                axis.set_MaximumScale(domAxis.MaximumScale);
            }
            if (!domAxis.IsNull("MinimumScale"))
            {
                axis.set_MinimumScale(domAxis.MinimumScale);
            }
            if (!domAxis.IsNull("LineFormat"))
            {
                LineFormatMapper.Map(axis.get_LineFormat(), domAxis.LineFormat);
            }
        }
    }
}

