namespace System.Data.Linq.Mapping
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public sealed class FunctionAttribute : Attribute
    {
        private bool isComposable;
        private string name;

        public bool IsComposable
        {
            get => 
                this.isComposable;
            set
            {
                this.isComposable = value;
            }
        }

        public string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }
    }
}

