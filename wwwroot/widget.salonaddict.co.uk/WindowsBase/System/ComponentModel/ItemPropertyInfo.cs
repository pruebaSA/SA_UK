namespace System.ComponentModel
{
    using System;

    public class ItemPropertyInfo
    {
        private object _descriptor;
        private string _name;
        private Type _type;

        public ItemPropertyInfo(string name, Type type, object descriptor)
        {
            this._name = name;
            this._type = type;
            this._descriptor = descriptor;
        }

        public object Descriptor =>
            this._descriptor;

        public string Name =>
            this._name;

        public Type PropertyType =>
            this._type;
    }
}

