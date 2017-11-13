namespace System.Deployment.Application.Manifest
{
    using System;
    using System.Deployment.Internal.Isolation.Manifest;

    internal class FileAssociation
    {
        private readonly string _defaultIcon;
        private readonly string _description;
        private readonly string _extension;
        private readonly string _parameter;
        private readonly string _progId;

        public FileAssociation(System.Deployment.Internal.Isolation.Manifest.FileAssociationEntry fileAssociationEntry)
        {
            this._extension = fileAssociationEntry.Extension;
            this._description = fileAssociationEntry.Description;
            this._progId = fileAssociationEntry.ProgID;
            this._defaultIcon = fileAssociationEntry.DefaultIcon;
            this._parameter = fileAssociationEntry.Parameter;
        }

        public string DefaultIcon =>
            this._defaultIcon;

        public string Description =>
            this._description;

        public string Extension =>
            this._extension;

        public string Parameter =>
            this._parameter;

        public string ProgID =>
            this._progId;
    }
}

