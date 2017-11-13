namespace AjaxControlToolkit
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public sealed class RequiredScriptAttribute : Attribute
    {
        private Type _extenderType;
        private int _order;
        private string _scriptName;

        public RequiredScriptAttribute()
        {
        }

        public RequiredScriptAttribute(string scriptName)
        {
            this._scriptName = scriptName;
        }

        public RequiredScriptAttribute(Type extenderType) : this(extenderType, 0)
        {
        }

        public RequiredScriptAttribute(Type extenderType, int loadOrder)
        {
            this._extenderType = extenderType;
            this._order = loadOrder;
        }

        public override bool IsDefaultAttribute() => 
            (this._extenderType == null);

        public Type ExtenderType =>
            this._extenderType;

        public int LoadOrder =>
            this._order;

        public string ScriptName =>
            this._scriptName;
    }
}

