namespace System.Web.Services.Protocols
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HttpMethodAttribute : Attribute
    {
        private Type parameterFormatter;
        private Type returnFormatter;

        public HttpMethodAttribute()
        {
            this.returnFormatter = null;
            this.parameterFormatter = null;
        }

        public HttpMethodAttribute(Type returnFormatter, Type parameterFormatter)
        {
            this.returnFormatter = returnFormatter;
            this.parameterFormatter = parameterFormatter;
        }

        public Type ParameterFormatter
        {
            get => 
                this.parameterFormatter;
            set
            {
                this.parameterFormatter = value;
            }
        }

        public Type ReturnFormatter
        {
            get => 
                this.returnFormatter;
            set
            {
                this.returnFormatter = value;
            }
        }
    }
}

