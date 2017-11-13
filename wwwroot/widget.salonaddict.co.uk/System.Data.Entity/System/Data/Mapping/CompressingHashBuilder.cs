namespace System.Data.Mapping
{
    using System;
    using System.Globalization;

    internal class CompressingHashBuilder : StringHashBuilder
    {
        private int _indent;
        private const int HashCharacterCompressionThreshold = 0x800;
        private const int SpacesPerIndent = 4;

        internal CompressingHashBuilder() : base(0x1800)
        {
        }

        internal override void Append(string content)
        {
            base.Append(string.Empty.PadLeft(4 * this._indent, ' '));
            base.Append(content);
            this.CompressHash();
        }

        internal override void AppendLine(string content)
        {
            base.Append(string.Empty.PadLeft(4 * this._indent, ' '));
            base.AppendLine(content);
            this.CompressHash();
        }

        internal void AppendObjectEndDump()
        {
            this._indent--;
        }

        internal void AppendObjectStartDump(object o, int objectIndex)
        {
            base.Append(string.Empty.PadLeft(4 * this._indent, ' '));
            base.Append(o.GetType().ToString());
            base.Append(" Instance#");
            base.AppendLine(objectIndex.ToString(CultureInfo.InvariantCulture));
            this.CompressHash();
            this._indent++;
        }

        private void CompressHash()
        {
            if (base.CharCount >= 0x800)
            {
                string s = base.ComputeHash();
                base.Clear();
                base.Append(s);
            }
        }
    }
}

