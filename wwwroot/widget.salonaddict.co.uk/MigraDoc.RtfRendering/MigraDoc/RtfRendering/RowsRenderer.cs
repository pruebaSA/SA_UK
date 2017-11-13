namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Tables;
    using System;

    internal class RowsRenderer : RendererBase
    {
        private Rows rows;

        internal RowsRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.rows = domObj as Rows;
        }

        internal static Unit CalculateLeftIndent(Rows rows)
        {
            object obj2 = rows.GetValue("LeftIndent", GV.GetNull);
            if (obj2 == null)
            {
                obj2 = rows.Table.Columns[0].GetValue("LeftPadding", GV.GetNull);
                if (obj2 == null)
                {
                    obj2 = Unit.FromCentimeter(-0.12);
                }
                else
                {
                    obj2 = Unit.FromPoint((double) -((float) ((Unit) obj2)));
                }
                Cell cell = rows[0].Cells[0];
                object obj3 = cell.GetValue("Borders.Left.Visible", GV.GetNull);
                object obj4 = cell.GetValue("Borders.Left.Width", GV.GetNull);
                object obj5 = cell.GetValue("Borders.Left.Style", GV.GetNull);
                object obj6 = cell.GetValue("Borders.Left.Color", GV.GetNull);
                if ((((obj3 == null) || ((bool) obj3)) && (((obj4 != null) || (obj5 != null)) || (obj6 != null))) && ((obj5 != null) && (((BorderStyle) obj5) != BorderStyle.None)))
                {
                    if (obj4 != null)
                    {
                        Unit unit = (Unit) obj2;
                        Unit unit2 = (Unit) obj4;
                        obj2 = Unit.FromPoint(unit.Point - unit2.Point);
                    }
                    else
                    {
                        Unit unit3 = (Unit) obj2;
                        obj2 = Unit.FromPoint(unit3.Point - 0.5);
                    }
                }
            }
            return (Unit) obj2;
        }

        internal override void Render()
        {
            base.Translate("Alignment", "trq");
            base.rtfWriter.WriteControl("trleft", RendererBase.ToTwips(CalculateLeftIndent(this.rows)));
        }
    }
}

