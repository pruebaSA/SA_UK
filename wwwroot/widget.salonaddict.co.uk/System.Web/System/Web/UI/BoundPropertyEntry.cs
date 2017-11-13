namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Compilation;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class BoundPropertyEntry : PropertyEntry
    {
        private string _controlID;
        private Type _controlType;
        private string _expression;
        private System.Web.Compilation.ExpressionBuilder _expressionBuilder;
        private string _expressionPrefix;
        private string _fieldName;
        private string _formatString;
        private bool _generated;
        private object _parsedExpressionData;
        private bool _readOnlyProperty;
        private bool _twoWayBound;
        private bool _useSetAttribute;

        internal BoundPropertyEntry()
        {
        }

        internal void ParseExpression(ExpressionBuilderContext context)
        {
            if (((this.Expression != null) && (this.ExpressionPrefix != null)) && (this.ExpressionBuilder != null))
            {
                this._parsedExpressionData = this.ExpressionBuilder.ParseExpression(this.Expression, base.Type, context);
            }
        }

        public string ControlID
        {
            get => 
                this._controlID;
            set
            {
                this._controlID = value;
            }
        }

        public Type ControlType
        {
            get => 
                this._controlType;
            set
            {
                this._controlType = value;
            }
        }

        public string Expression
        {
            get => 
                this._expression;
            set
            {
                this._expression = value;
            }
        }

        public System.Web.Compilation.ExpressionBuilder ExpressionBuilder
        {
            get => 
                this._expressionBuilder;
            set
            {
                this._expressionBuilder = value;
            }
        }

        public string ExpressionPrefix
        {
            get => 
                this._expressionPrefix;
            set
            {
                this._expressionPrefix = value;
            }
        }

        public string FieldName
        {
            get => 
                this._fieldName;
            set
            {
                this._fieldName = value;
            }
        }

        public string FormatString
        {
            get => 
                this._formatString;
            set
            {
                this._formatString = value;
            }
        }

        public bool Generated
        {
            get => 
                this._generated;
            set
            {
                this._generated = value;
            }
        }

        internal bool IsDataBindingEntry =>
            string.IsNullOrEmpty(this.ExpressionPrefix);

        public object ParsedExpressionData
        {
            get => 
                this._parsedExpressionData;
            set
            {
                this._parsedExpressionData = value;
            }
        }

        public bool ReadOnlyProperty
        {
            get => 
                this._readOnlyProperty;
            set
            {
                this._readOnlyProperty = value;
            }
        }

        public bool TwoWayBound
        {
            get => 
                this._twoWayBound;
            set
            {
                this._twoWayBound = value;
            }
        }

        public bool UseSetAttribute
        {
            get => 
                this._useSetAttribute;
            set
            {
                this._useSetAttribute = value;
            }
        }
    }
}

