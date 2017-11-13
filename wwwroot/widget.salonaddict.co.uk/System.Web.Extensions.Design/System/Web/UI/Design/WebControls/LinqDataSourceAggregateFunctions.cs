namespace System.Web.UI.Design.WebControls
{
    using System;

    internal class LinqDataSourceAggregateFunctions
    {
        private string _name;
        public static LinqDataSourceAggregateFunctions Average = new LinqDataSourceAggregateFunctions("Average");
        public static LinqDataSourceAggregateFunctions Count = new LinqDataSourceAggregateFunctions("Count");
        public static LinqDataSourceAggregateFunctions Max = new LinqDataSourceAggregateFunctions("Max");
        public static LinqDataSourceAggregateFunctions Min = new LinqDataSourceAggregateFunctions("Min");
        public static LinqDataSourceAggregateFunctions None = new LinqDataSourceAggregateFunctions(AtlasWebDesign.Combo_NoneOption);
        public static LinqDataSourceAggregateFunctions Sum = new LinqDataSourceAggregateFunctions("Sum");
        public static LinqDataSourceAggregateFunctions Unknown = new LinqDataSourceAggregateFunctions(string.Empty);

        private LinqDataSourceAggregateFunctions(string name)
        {
            this._name = name;
        }

        public static LinqDataSourceAggregateFunctions FromString(string name)
        {
            if (string.Equals(name, "Average", StringComparison.OrdinalIgnoreCase))
            {
                return Average;
            }
            if (string.Equals(name, "Count", StringComparison.OrdinalIgnoreCase))
            {
                return Count;
            }
            if (string.Equals(name, "Max", StringComparison.OrdinalIgnoreCase))
            {
                return Max;
            }
            if (string.Equals(name, "Min", StringComparison.OrdinalIgnoreCase))
            {
                return Min;
            }
            if (string.Equals(name, "Sum", StringComparison.OrdinalIgnoreCase))
            {
                return Sum;
            }
            return Unknown;
        }

        public override string ToString() => 
            this._name;
    }
}

