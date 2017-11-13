namespace System.Web.UI.Design.WebControls
{
    using System;

    internal sealed class LinqDataSourceWhereExpression
    {
        private string _fieldName;
        private LinqDataSourceOperators _operator;
        private string _paramName;

        public LinqDataSourceWhereExpression(string fieldName, string operatorText, string parameterName)
        {
            this._fieldName = fieldName;
            this._operator = LinqDataSourceOperators.GetOperator(operatorText);
            this._paramName = parameterName;
        }

        public override string ToString() => 
            (this._fieldName + " " + this._operator.ToString() + " @" + this._paramName);

        public string FieldName =>
            this._fieldName;

        public LinqDataSourceOperators Operator =>
            this._operator;

        public string ParameterName =>
            this._paramName;
    }
}

