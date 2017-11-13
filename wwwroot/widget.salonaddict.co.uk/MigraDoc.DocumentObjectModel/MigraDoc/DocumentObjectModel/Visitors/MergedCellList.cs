namespace MigraDoc.DocumentObjectModel.Visitors
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Tables;
    using System;
    using System.Collections;
    using System.Reflection;

    public class MergedCellList : ArrayList
    {
        public MergedCellList(Table table)
        {
            this.Init(table);
        }

        private Border GetBorderFromBorders(Borders borders, BorderType type)
        {
            Border border = borders.GetValue(type.ToString(), GV.ReadOnly) as Border;
            if (border == null)
            {
                border = new Border {
                    style = borders.style,
                    width = borders.width,
                    color = borders.color,
                    visible = borders.visible
                };
            }
            return border;
        }

        public Cell GetCoveringCell(Cell cell)
        {
            int num = this.BinarySearch(cell, new CellComparer());
            if ((num >= 0) && (num < this.Count))
            {
                return this[num];
            }
            num = ~num - 1;
            for (int i = num; i >= 0; i--)
            {
                Cell cell2 = this[i];
                if (((cell2.Column.Index <= cell.Column.Index) && ((cell2.Column.Index + cell2.MergeRight) >= cell.Column.Index)) && ((cell2.Row.Index <= cell.Row.Index) && ((cell2.Row.Index + cell2.MergeDown) >= cell.Row.Index)))
                {
                    return cell2;
                }
            }
            return null;
        }

        public Borders GetEffectiveBorders(Cell cell)
        {
            Borders borders = cell.GetValue("Borders", GV.ReadOnly) as Borders;
            if (borders != null)
            {
                Document document = borders.Document;
                borders = borders.Clone();
                borders.parent = cell;
                Document document2 = borders.Document;
            }
            else
            {
                borders = new Borders(cell.parent);
            }
            int cellIdx = this.BinarySearch(cell, new CellComparer());
            if ((cellIdx < 0) || (cellIdx >= this.Count))
            {
                throw new ArgumentException("cell is not a relevant cell", "cell");
            }
            if (cell.mergeRight > 0)
            {
                Cell cell2 = cell.Table[cell.Row.Index, cell.Column.Index + cell.mergeRight];
                if ((cell2.borders != null) && (cell2.borders.right != null))
                {
                    borders.Right = cell2.borders.right.Clone();
                }
                else
                {
                    borders.right = null;
                }
            }
            if (cell.mergeDown > 0)
            {
                Cell cell3 = cell.Table[cell.Row.Index + cell.mergeDown, cell.Column.Index];
                if ((cell3.borders != null) && (cell3.borders.bottom != null))
                {
                    borders.Bottom = cell3.borders.bottom.Clone();
                }
                else
                {
                    borders.bottom = null;
                }
            }
            Cell neighbor = this.GetNeighbor(cellIdx, NeighborPosition.Left);
            Cell cell5 = this.GetNeighbor(cellIdx, NeighborPosition.Right);
            Cell cell6 = this.GetNeighbor(cellIdx, NeighborPosition.Top);
            Cell cell7 = this.GetNeighbor(cellIdx, NeighborPosition.Bottom);
            if (neighbor != null)
            {
                Borders borders2 = neighbor.GetValue("Borders", GV.ReadWrite) as Borders;
                if ((borders2 != null) && (((float) this.GetEffectiveBorderWidth(borders2, BorderType.Right)) >= ((float) this.GetEffectiveBorderWidth(borders, BorderType.Left))))
                {
                    borders.SetValue("Left", this.GetBorderFromBorders(borders2, BorderType.Right));
                }
            }
            if (cell5 != null)
            {
                Borders borders3 = cell5.GetValue("Borders", GV.ReadWrite) as Borders;
                if ((borders3 != null) && (((float) this.GetEffectiveBorderWidth(borders3, BorderType.Left)) > ((float) this.GetEffectiveBorderWidth(borders, BorderType.Right))))
                {
                    borders.SetValue("Right", this.GetBorderFromBorders(borders3, BorderType.Left));
                }
            }
            if (cell6 != null)
            {
                Borders borders4 = cell6.GetValue("Borders", GV.ReadWrite) as Borders;
                if ((borders4 != null) && (((float) this.GetEffectiveBorderWidth(borders4, BorderType.Bottom)) >= ((float) this.GetEffectiveBorderWidth(borders, BorderType.Top))))
                {
                    borders.SetValue("Top", this.GetBorderFromBorders(borders4, BorderType.Bottom));
                }
            }
            if (cell7 != null)
            {
                Borders borders5 = cell7.GetValue("Borders", GV.ReadWrite) as Borders;
                if ((borders5 != null) && (((float) this.GetEffectiveBorderWidth(borders5, BorderType.Top)) > ((float) this.GetEffectiveBorderWidth(borders, BorderType.Bottom))))
                {
                    borders.SetValue("Bottom", this.GetBorderFromBorders(borders5, BorderType.Top));
                }
            }
            return borders;
        }

        private Unit GetEffectiveBorderWidth(Borders borders, BorderType type)
        {
            if (borders == null)
            {
                return 0;
            }
            Border border = borders.GetValue(type.ToString(), GV.GetNull) as Border;
            DocumentObject obj2 = border;
            if ((obj2 == null) || obj2.IsNull("Width"))
            {
                obj2 = borders;
            }
            object obj3 = obj2.GetValue("visible", GV.GetNull);
            object obj4 = obj2.GetValue("style", GV.GetNull);
            object obj5 = obj2.GetValue("width", GV.GetNull);
            object obj6 = obj2.GetValue("color", GV.GetNull);
            if (((obj3 == null) && (obj4 == null)) && ((obj5 == null) && (obj6 == null)))
            {
                return 0;
            }
            if ((obj3 != null) && !((bool) obj3))
            {
                return 0;
            }
            if (obj5 != null)
            {
                return (Unit) obj5;
            }
            return 0.5;
        }

        public override IEnumerator GetEnumerator() => 
            new Enumerator(this);

        private Cell GetNeighbor(int cellIdx, NeighborPosition position)
        {
            Cell cell = this[cellIdx];
            if ((((cell.Column.Index == 0) && (position == NeighborPosition.Left)) || ((cell.Row.Index == 0) && (position == NeighborPosition.Top))) || ((((cell.Row.Index + cell.MergeDown) == (cell.Table.Rows.Count - 1)) && (position == NeighborPosition.Bottom)) || (((cell.Column.Index + cell.MergeRight) == (cell.Table.Columns.Count - 1)) && (position == NeighborPosition.Right))))
            {
                return null;
            }
            switch (position)
            {
                case NeighborPosition.Top:
                case NeighborPosition.Left:
                    for (int j = cellIdx - 1; j >= 0; j--)
                    {
                        Cell cell2 = this[j];
                        if (this.IsNeighbor(cell, cell2, position))
                        {
                            return cell2;
                        }
                    }
                    goto Label_0152;

                case NeighborPosition.Right:
                {
                    if ((cellIdx + 1) >= this.Count)
                    {
                        break;
                    }
                    Cell cell3 = this[cellIdx + 1];
                    if (cell3.Row.Index != cell.Row.Index)
                    {
                        break;
                    }
                    return cell3;
                }
                case NeighborPosition.Bottom:
                    for (int k = cellIdx + 1; k < this.Count; k++)
                    {
                        Cell cell5 = this[k];
                        if (this.IsNeighbor(cell, cell5, position))
                        {
                            return cell5;
                        }
                    }
                    goto Label_0152;

                default:
                    goto Label_0152;
            }
            for (int i = cellIdx - 1; i >= 0; i--)
            {
                Cell cell4 = this[i];
                if (this.IsNeighbor(cell, cell4, position))
                {
                    return cell4;
                }
            }
        Label_0152:
            return null;
        }

        private void Init(Table table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    Cell cell = table[i, j];
                    if (!this.IsAlreadyCovered(cell))
                    {
                        this.Add(cell);
                    }
                }
            }
        }

        private bool IsAlreadyCovered(Cell cell)
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                Cell cell2 = this[i];
                if ((cell2.Column.Index <= cell.Column.Index) && ((cell2.Column.Index + cell2.MergeRight) >= cell.Column.Index))
                {
                    if ((cell2.Row.Index <= cell.Row.Index) && ((cell2.Row.Index + cell2.MergeDown) >= cell.Row.Index))
                    {
                        return true;
                    }
                    if ((cell2.Row.Index + cell2.MergeDown) == (cell.Row.Index - 1))
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        private bool IsNeighbor(Cell cell1, Cell cell2, NeighborPosition position)
        {
            switch (position)
            {
                case NeighborPosition.Top:
                {
                    int num4 = cell1.Row.Index - 1;
                    return ((((cell2.Row.Index + cell2.MergeDown) == num4) && ((cell2.Column.Index + cell2.MergeRight) >= cell1.Column.Index)) && (cell2.Column.Index <= cell1.Column.Index));
                }
                case NeighborPosition.Left:
                {
                    int num2 = cell1.Column.Index - 1;
                    return (((cell2.Row.Index <= cell1.Row.Index) && ((cell2.Row.Index + cell2.MergeDown) >= cell1.Row.Index)) && ((cell2.Column.Index + cell2.MergeRight) == num2));
                }
                case NeighborPosition.Right:
                {
                    int num3 = (cell1.Column.Index + cell1.MergeRight) + 1;
                    return (((cell2.Row.Index <= cell1.Row.Index) && ((cell2.Row.Index + cell2.MergeDown) >= cell1.Row.Index)) && (cell2.Column.Index == num3));
                }
                case NeighborPosition.Bottom:
                {
                    int num = (cell1.Row.Index + cell1.MergeDown) + 1;
                    return (((cell2.Row.Index == num) && (cell2.Column.Index <= cell1.Column.Index)) && ((cell2.Column.Index + cell2.MergeRight) >= cell1.Column.Index));
                }
            }
            return false;
        }

        public Cell this[int index] =>
            (base[index] as Cell);

        public class Enumerator : IEnumerator
        {
            private int index = -1;
            private MergedCellList list;

            internal Enumerator(MergedCellList list)
            {
                this.list = list;
            }

            public bool MoveNext() => 
                (++this.index < this.list.Count);

            public void Reset()
            {
                this.index = -1;
            }

            public Cell Current =>
                this.list[this.index];

            object IEnumerator.Current =>
                this.Current;
        }

        private enum NeighborPosition
        {
            Top,
            Left,
            Right,
            Bottom
        }
    }
}

