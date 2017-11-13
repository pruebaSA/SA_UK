namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections;
    using System.Web.UI.WebControls;

    internal sealed class SqlDataSourceQuery
    {
        private string _command;
        private SqlDataSourceCommandType _commandType;
        private ICollection _parameters;

        public SqlDataSourceQuery(string command, SqlDataSourceCommandType commandType, ICollection parameters)
        {
            this._command = command;
            this._commandType = commandType;
            this._parameters = parameters;
        }

        public string Command =>
            this._command;

        public SqlDataSourceCommandType CommandType =>
            this._commandType;

        public ICollection Parameters =>
            this._parameters;
    }
}

