namespace AjaxControlToolkit
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=false, Inherited=true)]
    public sealed class ScriptCombineAttribute : Attribute
    {
        private string _excludeScripts;
        private string _includeScripts;

        public string ExcludeScripts
        {
            get => 
                this._excludeScripts;
            set
            {
                this._excludeScripts = value;
            }
        }

        public string IncludeScripts
        {
            get => 
                this._includeScripts;
            set
            {
                this._includeScripts = value;
            }
        }
    }
}

