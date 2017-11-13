namespace PdfSharp.Pdf.Security
{
    using PdfSharp;
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfSecuritySettings
    {
        private PdfDocument document;
        private PdfDocumentSecurityLevel documentSecurityLevel;
        internal bool hasOwnerPermissions = true;

        internal PdfSecuritySettings(PdfDocument document)
        {
            this.document = document;
        }

        internal bool CanSave(ref string message)
        {
            if (((this.documentSecurityLevel != PdfDocumentSecurityLevel.None) && ((this.SecurityHandler.userPassword == null) || (this.SecurityHandler.userPassword.Length == 0))) && ((this.SecurityHandler.ownerPassword == null) || (this.SecurityHandler.ownerPassword.Length == 0)))
            {
                message = PSSR.UserOrOwnerPasswordRequired;
                return false;
            }
            return true;
        }

        public PdfDocumentSecurityLevel DocumentSecurityLevel
        {
            get => 
                this.documentSecurityLevel;
            set
            {
                this.documentSecurityLevel = value;
            }
        }

        public bool HasOwnerPermissions =>
            this.hasOwnerPermissions;

        public string OwnerPassword
        {
            set
            {
                this.SecurityHandler.OwnerPassword = value;
            }
        }

        public bool PermitAccessibilityExtractContent
        {
            get => 
                ((this.SecurityHandler.Permission & PdfUserAccessPermission.PermitAccessibilityExtractContent) != 0);
            set
            {
                PdfUserAccessPermission permission = this.SecurityHandler.Permission;
                if (value)
                {
                    permission |= PdfUserAccessPermission.PermitAccessibilityExtractContent;
                }
                else
                {
                    permission &= ~PdfUserAccessPermission.PermitAccessibilityExtractContent;
                }
                this.SecurityHandler.Permission = permission;
            }
        }

        public bool PermitAnnotations
        {
            get => 
                ((this.SecurityHandler.Permission & PdfUserAccessPermission.PermitAnnotations) != 0);
            set
            {
                PdfUserAccessPermission permission = this.SecurityHandler.Permission;
                if (value)
                {
                    permission |= PdfUserAccessPermission.PermitAnnotations;
                }
                else
                {
                    permission &= ~PdfUserAccessPermission.PermitAnnotations;
                }
                this.SecurityHandler.Permission = permission;
            }
        }

        public bool PermitAssembleDocument
        {
            get => 
                ((this.SecurityHandler.Permission & PdfUserAccessPermission.PermitAssembleDocument) != 0);
            set
            {
                PdfUserAccessPermission permission = this.SecurityHandler.Permission;
                if (value)
                {
                    permission |= PdfUserAccessPermission.PermitAssembleDocument;
                }
                else
                {
                    permission &= ~PdfUserAccessPermission.PermitAssembleDocument;
                }
                this.SecurityHandler.Permission = permission;
            }
        }

        public bool PermitExtractContent
        {
            get => 
                ((this.SecurityHandler.Permission & PdfUserAccessPermission.PermitExtractContent) != 0);
            set
            {
                PdfUserAccessPermission permission = this.SecurityHandler.Permission;
                if (value)
                {
                    permission |= PdfUserAccessPermission.PermitExtractContent;
                }
                else
                {
                    permission &= ~PdfUserAccessPermission.PermitExtractContent;
                }
                this.SecurityHandler.Permission = permission;
            }
        }

        public bool PermitFormsFill
        {
            get => 
                ((this.SecurityHandler.Permission & PdfUserAccessPermission.PermitFormsFill) != 0);
            set
            {
                PdfUserAccessPermission permission = this.SecurityHandler.Permission;
                if (value)
                {
                    permission |= PdfUserAccessPermission.PermitFormsFill;
                }
                else
                {
                    permission &= ~PdfUserAccessPermission.PermitFormsFill;
                }
                this.SecurityHandler.Permission = permission;
            }
        }

        public bool PermitFullQualityPrint
        {
            get => 
                ((this.SecurityHandler.Permission & PdfUserAccessPermission.PermitFullQualityPrint) != 0);
            set
            {
                PdfUserAccessPermission permission = this.SecurityHandler.Permission;
                if (value)
                {
                    permission |= PdfUserAccessPermission.PermitFullQualityPrint;
                }
                else
                {
                    permission &= ~PdfUserAccessPermission.PermitFullQualityPrint;
                }
                this.SecurityHandler.Permission = permission;
            }
        }

        public bool PermitModifyDocument
        {
            get => 
                ((this.SecurityHandler.Permission & PdfUserAccessPermission.PermitModifyDocument) != 0);
            set
            {
                PdfUserAccessPermission permission = this.SecurityHandler.Permission;
                if (value)
                {
                    permission |= PdfUserAccessPermission.PermitModifyDocument;
                }
                else
                {
                    permission &= ~PdfUserAccessPermission.PermitModifyDocument;
                }
                this.SecurityHandler.Permission = permission;
            }
        }

        public bool PermitPrint
        {
            get => 
                ((this.SecurityHandler.Permission & PdfUserAccessPermission.PermitPrint) != 0);
            set
            {
                PdfUserAccessPermission permission = this.SecurityHandler.Permission;
                if (value)
                {
                    permission |= PdfUserAccessPermission.PermitPrint;
                }
                else
                {
                    permission &= ~PdfUserAccessPermission.PermitPrint;
                }
                this.SecurityHandler.Permission = permission;
            }
        }

        internal PdfStandardSecurityHandler SecurityHandler =>
            this.document.trailer.SecurityHandler;

        public string UserPassword
        {
            set
            {
                this.SecurityHandler.UserPassword = value;
            }
        }
    }
}

