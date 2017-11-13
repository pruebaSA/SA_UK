namespace System.Deployment.Application
{
    using System;

    internal class AssemblyModule
    {
        private byte[] _hash;
        private string _name;

        public AssemblyModule(string name, byte[] hash)
        {
            this._name = name;
            this._hash = hash;
        }

        public byte[] Hash =>
            this._hash;

        public string Name =>
            this._name;
    }
}

