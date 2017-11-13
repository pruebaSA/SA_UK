namespace LinqToSqlShared.Mapping
{
    using System;
    using System.Collections.Generic;

    internal class FunctionMapping
    {
        private ReturnMapping funReturn;
        private bool isComposable;
        private string methodName;
        private string name;
        private List<ParameterMapping> parameters = new List<ParameterMapping>();
        private List<TypeMapping> types = new List<TypeMapping>();

        internal FunctionMapping()
        {
        }

        internal ReturnMapping FunReturn
        {
            get => 
                this.funReturn;
            set
            {
                this.funReturn = value;
            }
        }

        internal bool IsComposable
        {
            get => 
                this.isComposable;
            set
            {
                this.isComposable = value;
            }
        }

        internal string MethodName
        {
            get => 
                this.methodName;
            set
            {
                this.methodName = value;
            }
        }

        internal string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        internal List<ParameterMapping> Parameters =>
            this.parameters;

        internal List<TypeMapping> Types =>
            this.types;

        internal string XmlIsComposable
        {
            get
            {
                if (!this.isComposable)
                {
                    return null;
                }
                return "true";
            }
            set
            {
                this.isComposable = (value != null) ? bool.Parse(value) : false;
            }
        }
    }
}

