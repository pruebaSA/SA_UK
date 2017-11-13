namespace System.ComponentModel.Design.Data
{
    using System;
    using System.Collections;

    public sealed class DesignerDataRelationship
    {
        private ICollection _childColumns;
        private DesignerDataTable _childTable;
        private string _name;
        private ICollection _parentColumns;

        public DesignerDataRelationship(string name, ICollection parentColumns, DesignerDataTable childTable, ICollection childColumns)
        {
            this._childColumns = childColumns;
            this._childTable = childTable;
            this._name = name;
            this._parentColumns = parentColumns;
        }

        public ICollection ChildColumns =>
            this._childColumns;

        public DesignerDataTable ChildTable =>
            this._childTable;

        public string Name =>
            this._name;

        public ICollection ParentColumns =>
            this._parentColumns;
    }
}

