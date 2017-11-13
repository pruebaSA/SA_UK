namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.Linq.Expressions;

    internal class RecordStateScratchpad
    {
        private int _columnCount;
        private System.Data.Common.DataRecordInfo _dataRecordInfo;
        private Expression _gatherData;
        private List<RecordStateScratchpad> _nestedRecordStateScratchpads = new List<RecordStateScratchpad>();
        private string[] _propertyNames;
        private int _stateSlotNumber;
        private TypeUsage[] _typeUsages;

        internal RecordStateFactory Compile()
        {
            RecordStateFactory[] factoryArray = new RecordStateFactory[this._nestedRecordStateScratchpads.Count];
            for (int i = 0; i < factoryArray.Length; i++)
            {
                factoryArray[i] = this._nestedRecordStateScratchpads[i].Compile();
            }
            return (RecordStateFactory) Activator.CreateInstance(typeof(RecordStateFactory), new object[] { this.StateSlotNumber, this.ColumnCount, factoryArray, this.DataRecordInfo, this.GatherData, this.PropertyNames, this.TypeUsages });
        }

        internal int ColumnCount
        {
            get => 
                this._columnCount;
            set
            {
                this._columnCount = value;
            }
        }

        internal System.Data.Common.DataRecordInfo DataRecordInfo
        {
            get => 
                this._dataRecordInfo;
            set
            {
                this._dataRecordInfo = value;
            }
        }

        internal Expression GatherData
        {
            get => 
                this._gatherData;
            set
            {
                this._gatherData = value;
            }
        }

        internal string[] PropertyNames
        {
            get => 
                this._propertyNames;
            set
            {
                this._propertyNames = value;
            }
        }

        internal int StateSlotNumber
        {
            get => 
                this._stateSlotNumber;
            set
            {
                this._stateSlotNumber = value;
            }
        }

        internal TypeUsage[] TypeUsages
        {
            get => 
                this._typeUsages;
            set
            {
                this._typeUsages = value;
            }
        }
    }
}

