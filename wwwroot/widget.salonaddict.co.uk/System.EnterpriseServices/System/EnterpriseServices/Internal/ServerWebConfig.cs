namespace System.EnterpriseServices.Internal
{
    using System;
    using System.EnterpriseServices;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Xml;

    public class ServerWebConfig : IServerWebConfig
    {
        private const string indent = "  ";
        private string webconfig = "";

        public void AddElement(string FilePath, string AssemblyName, string TypeName, string ProgId, string WkoMode, out string Error)
        {
            Error = "";
            try
            {
                new SecurityPermission(SecurityPermissionFlag.RemotingConfiguration).Demand();
                string str = TypeName + ", " + AssemblyName;
                XmlDocument document = new XmlDocument();
                document.Load(FilePath);
                XmlNode documentElement = document.DocumentElement;
                while (documentElement.Name != "configuration")
                {
                    documentElement = documentElement.NextSibling;
                }
                documentElement = documentElement.FirstChild;
                while (documentElement.Name != "system.runtime.remoting")
                {
                    documentElement = documentElement.NextSibling;
                }
                documentElement = documentElement.FirstChild;
                while (documentElement.Name != "application")
                {
                    documentElement = documentElement.NextSibling;
                }
                documentElement = documentElement.FirstChild;
                while (documentElement.Name != "service")
                {
                    documentElement = documentElement.NextSibling;
                }
                XmlNodeList list = documentElement.SelectNodes("descendant::*[attribute::type='" + str + "']");
                while ((list != null) && (list.Count > 0))
                {
                    XmlNode oldChild = list.Item(0);
                    if (oldChild.ParentNode != null)
                    {
                        oldChild.ParentNode.RemoveChild(oldChild);
                        list = documentElement.SelectNodes("descendant::*[attribute::type='" + str + "']");
                    }
                }
                XmlElement newChild = document.CreateElement("", "wellknown", "");
                newChild.SetAttribute("mode", WkoMode);
                newChild.SetAttribute("type", str);
                newChild.SetAttribute("objectUri", ProgId + ".soap");
                documentElement.AppendChild(newChild);
                XmlElement element2 = document.CreateElement("", "activated", "");
                element2.SetAttribute("type", str);
                documentElement.AppendChild(element2);
                document.Save(FilePath);
            }
            catch (Exception exception)
            {
                Error = exception.ToString();
                ComSoapPublishError.Report(exception.ToString());
            }
            catch
            {
                ComSoapPublishError.Report(Resource.FormatString("Err_NonClsException", "ServerWebConfig.AddElement"));
            }
        }

        internal void AddGacElement(string FilePath, string AssemblyName, string TypeName, string ProgId, string WkoMode, string AssemblyFile)
        {
            try
            {
                new SecurityPermission(SecurityPermissionFlag.RemotingConfiguration).Demand();
                AssemblyManager manager = new AssemblyManager();
                string str = TypeName + ", " + manager.GetFullName(AssemblyFile, AssemblyName);
                string str2 = TypeName + ", " + AssemblyName;
                XmlDocument document = new XmlDocument();
                document.Load(FilePath);
                XmlNode documentElement = document.DocumentElement;
                while (documentElement.Name != "configuration")
                {
                    documentElement = documentElement.NextSibling;
                }
                documentElement = documentElement.FirstChild;
                while (documentElement.Name != "system.runtime.remoting")
                {
                    documentElement = documentElement.NextSibling;
                }
                documentElement = documentElement.FirstChild;
                while (documentElement.Name != "application")
                {
                    documentElement = documentElement.NextSibling;
                }
                documentElement = documentElement.FirstChild;
                while (documentElement.Name != "service")
                {
                    documentElement = documentElement.NextSibling;
                }
                XmlNodeList list = documentElement.SelectNodes("descendant::*[attribute::type='" + str2 + "']");
                while ((list != null) && (list.Count > 0))
                {
                    XmlNode oldChild = list.Item(0);
                    if (oldChild.ParentNode != null)
                    {
                        oldChild.ParentNode.RemoveChild(oldChild);
                        list = documentElement.SelectNodes("descendant::*[attribute::type='" + str2 + "']");
                    }
                }
                list = documentElement.SelectNodes("descendant::*[attribute::type='" + str + "']");
                while ((list != null) && (list.Count > 0))
                {
                    XmlNode node3 = list.Item(0);
                    if (node3.ParentNode != null)
                    {
                        node3.ParentNode.RemoveChild(node3);
                        list = documentElement.SelectNodes("descendant::*[attribute::type='" + str + "']");
                    }
                }
                XmlElement newChild = document.CreateElement("", "wellknown", "");
                newChild.SetAttribute("mode", WkoMode);
                newChild.SetAttribute("type", str);
                newChild.SetAttribute("objectUri", ProgId + ".soap");
                documentElement.AppendChild(newChild);
                XmlElement element2 = document.CreateElement("", "activated", "");
                element2.SetAttribute("type", str2);
                documentElement.AppendChild(element2);
                document.Save(FilePath);
            }
            catch (RegistrationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                ComSoapPublishError.Report(exception.ToString());
            }
            catch
            {
                ComSoapPublishError.Report(Resource.FormatString("Err_NonClsException", "ServerWebConfig.AddGacElement"));
            }
        }

        public void Create(string FilePath, string FilePrefix, out string Error)
        {
            Error = "";
            try
            {
                new SecurityPermission(SecurityPermissionFlag.RemotingConfiguration).Demand();
            }
            catch (Exception exception)
            {
                ComSoapPublishError.Report(exception.ToString());
                throw;
            }
            catch
            {
                ComSoapPublishError.Report(Resource.FormatString("Err_NonClsException", "ServerWebConfig.Create"));
                throw;
            }
            if (!FilePath.EndsWith("/", StringComparison.Ordinal) && !FilePath.EndsWith(@"\", StringComparison.Ordinal))
            {
                FilePath = FilePath + @"\";
            }
            if (!File.Exists(FilePath + FilePrefix + ".config"))
            {
                this.webconfig = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n";
                this.webconfig = this.webconfig + "<configuration>\r\n";
                this.webconfig = this.webconfig + "  <system.runtime.remoting>\r\n";
                this.webconfig = this.webconfig + "    <application>\r\n";
                this.webconfig = this.webconfig + "      <service>\r\n";
                this.webconfig = this.webconfig + "      </service>\r\n";
                this.webconfig = this.webconfig + "    </application>\r\n";
                this.webconfig = this.webconfig + "  </system.runtime.remoting>\r\n";
                this.webconfig = this.webconfig + "</configuration>\r\n";
                if (!this.WriteFile(FilePath, FilePrefix, ".config"))
                {
                    Error = Resource.FormatString("Soap_WebConfigFailed");
                    ComSoapPublishError.Report(Error);
                }
            }
        }

        internal void DeleteElement(string FilePath, string AssemblyName, string TypeName, string ProgId, string WkoMode, string AssemblyFile)
        {
            try
            {
                new SecurityPermission(SecurityPermissionFlag.RemotingConfiguration).Demand();
                AssemblyManager manager = new AssemblyManager();
                string str = TypeName + ", " + manager.GetFullName(AssemblyFile, AssemblyName);
                string str2 = TypeName + ", " + AssemblyName;
                XmlDocument document = new XmlDocument();
                document.Load(FilePath);
                XmlNode documentElement = document.DocumentElement;
                while (documentElement.Name != "configuration")
                {
                    documentElement = documentElement.NextSibling;
                }
                documentElement = documentElement.FirstChild;
                while (documentElement.Name != "system.runtime.remoting")
                {
                    documentElement = documentElement.NextSibling;
                }
                documentElement = documentElement.FirstChild;
                while (documentElement.Name != "application")
                {
                    documentElement = documentElement.NextSibling;
                }
                documentElement = documentElement.FirstChild;
                while (documentElement.Name != "service")
                {
                    documentElement = documentElement.NextSibling;
                }
                XmlNodeList list = documentElement.SelectNodes("descendant::*[attribute::type='" + str2 + "']");
                while ((list != null) && (list.Count > 0))
                {
                    XmlNode oldChild = list.Item(0);
                    if (oldChild.ParentNode != null)
                    {
                        oldChild.ParentNode.RemoveChild(oldChild);
                        list = documentElement.SelectNodes("descendant::*[attribute::type='" + str2 + "']");
                    }
                }
                list = documentElement.SelectNodes("descendant::*[attribute::type='" + str + "']");
                while ((list != null) && (list.Count > 0))
                {
                    XmlNode node3 = list.Item(0);
                    if (node3.ParentNode != null)
                    {
                        node3.ParentNode.RemoveChild(node3);
                        list = documentElement.SelectNodes("descendant::*[attribute::type='" + str + "']");
                    }
                }
                document.Save(FilePath);
            }
            catch (DirectoryNotFoundException)
            {
            }
            catch (FileNotFoundException)
            {
            }
            catch (Exception exception)
            {
                ComSoapPublishError.Report(exception.ToString());
            }
            catch
            {
                ComSoapPublishError.Report(Resource.FormatString("Err_NonClsException", "ServerWebConfig.DeleteElement"));
            }
        }

        private bool WriteFile(string PhysicalDirectory, string FilePrefix, string FileSuffix)
        {
            try
            {
                string path = PhysicalDirectory + FilePrefix + FileSuffix;
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                FileStream stream = new FileStream(path, FileMode.Create);
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(this.webconfig);
                writer.Close();
                stream.Close();
                return true;
            }
            catch (Exception exception)
            {
                ComSoapPublishError.Report(exception.ToString());
                return false;
            }
            catch
            {
                ComSoapPublishError.Report(Resource.FormatString("Err_NonClsException", "ServerWebConfig.WriteFile"));
                return false;
            }
        }
    }
}

