namespace System.Windows.Markup
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class XmlnsCompatibleWithAttribute : Attribute
    {
        private string _newNamespace;
        private string _oldNamespace;

        public XmlnsCompatibleWithAttribute(string oldNamespace, string newNamespace)
        {
            if (oldNamespace == null)
            {
                throw new ArgumentNullException("oldNamespace");
            }
            if (newNamespace == null)
            {
                throw new ArgumentNullException("newNamespace");
            }
            this._oldNamespace = oldNamespace;
            this._newNamespace = newNamespace;
        }

        public string NewNamespace =>
            this._newNamespace;

        public string OldNamespace =>
            this._oldNamespace;
    }
}

