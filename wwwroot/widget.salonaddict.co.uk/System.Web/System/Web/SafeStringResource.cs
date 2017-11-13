namespace System.Web
{
    using System;

    internal class SafeStringResource
    {
        private int _resourceSize;
        private IntPtr _stringResourcePointer;

        internal SafeStringResource(IntPtr stringResourcePointer, int resourceSize)
        {
            this._stringResourcePointer = stringResourcePointer;
            this._resourceSize = resourceSize;
        }

        internal int ResourceSize =>
            this._resourceSize;

        internal IntPtr StringResourcePointer =>
            this._stringResourcePointer;
    }
}

