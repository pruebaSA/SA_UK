namespace MS.Internal
{
    using System;

    internal class AvTraceDetails
    {
        private int _id;
        private string[] _labels;

        public AvTraceDetails(int id, string[] labels)
        {
            this._id = id;
            this._labels = labels;
        }

        public int Id =>
            this._id;

        public string[] Labels =>
            this._labels;

        public virtual string Message =>
            this._labels[0];
    }
}

