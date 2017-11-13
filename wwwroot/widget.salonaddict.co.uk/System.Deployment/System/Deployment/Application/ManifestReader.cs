namespace System.Deployment.Application
{
    using Microsoft.Internal.Performance;
    using System;
    using System.Deployment.Application.Manifest;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;

    internal static class ManifestReader
    {
        internal static AssemblyManifest FromDocument(string localPath, AssemblyManifest.ManifestType manifestType, Uri sourceUri)
        {
            AssemblyManifest manifest;
            CodeMarker_Singleton.Instance.CodeMarker(CodeMarkerEvent.perfParseBegin);
            FileInfo info = new FileInfo(localPath);
            if (info.Length > 0x1000000L)
            {
                throw new DeploymentException(Resources.GetString("Ex_ManifestFileTooLarge"));
            }
            FileStream input = new FileStream(localPath, FileMode.Open, FileAccess.Read);
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings {
                    ProhibitDtd = true,
                    XmlResolver = null
                };
                XmlReader reader = PolicyKeys.SkipSchemaValidation() ? XmlReader.Create(input, settings) : ManifestValidatingReader.Create(input);
                while (reader.Read())
                {
                }
                manifest = new AssemblyManifest(input);
                if (!PolicyKeys.SkipSemanticValidation())
                {
                    manifest.ValidateSemantics(manifestType);
                }
                if (!PolicyKeys.SkipSignatureValidation())
                {
                    input.Position = 0L;
                    manifest.ValidateSignature(input);
                }
            }
            catch (XmlException exception)
            {
                string message = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("Ex_ManifestFromDocument"), new object[] { (sourceUri != null) ? sourceUri.AbsoluteUri : Path.GetFileName(localPath) });
                throw new InvalidDeploymentException(ExceptionTypes.ManifestParse, message, exception);
            }
            catch (XmlSchemaValidationException exception2)
            {
                string str2 = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("Ex_ManifestFromDocument"), new object[] { (sourceUri != null) ? sourceUri.AbsoluteUri : Path.GetFileName(localPath) });
                throw new InvalidDeploymentException(ExceptionTypes.ManifestParse, str2, exception2);
            }
            catch (InvalidDeploymentException exception3)
            {
                string str3 = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("Ex_ManifestFromDocument"), new object[] { (sourceUri != null) ? sourceUri.AbsoluteUri : Path.GetFileName(localPath) });
                throw new InvalidDeploymentException(ExceptionTypes.ManifestParse, str3, exception3);
            }
            finally
            {
                if (input != null)
                {
                    input.Dispose();
                }
            }
            CodeMarker_Singleton.Instance.CodeMarker(CodeMarkerEvent.perfParseEnd);
            return manifest;
        }

        internal static AssemblyManifest FromDocumentNoValidation(string localPath)
        {
            AssemblyManifest manifest;
            CodeMarker_Singleton.Instance.CodeMarker(CodeMarkerEvent.perfParseBegin);
            FileInfo info = new FileInfo(localPath);
            if (info.Length > 0x1000000L)
            {
                throw new DeploymentException(Resources.GetString("Ex_ManifestFileTooLarge"));
            }
            using (FileStream stream = new FileStream(localPath, FileMode.Open, FileAccess.Read))
            {
                manifest = new AssemblyManifest(stream);
            }
            CodeMarker_Singleton.Instance.CodeMarker(CodeMarkerEvent.perfParseEnd);
            return manifest;
        }
    }
}

