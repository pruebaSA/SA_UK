namespace System.Security.Policy
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Util;

    [Serializable, ComVisible(true)]
    public sealed class ApplicationDirectory : IBuiltInEvidence
    {
        private URLString m_appDirectory;

        internal ApplicationDirectory()
        {
            this.m_appDirectory = null;
        }

        public ApplicationDirectory(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this.m_appDirectory = new URLString(name);
        }

        public object Copy() => 
            new ApplicationDirectory { m_appDirectory = this.m_appDirectory };

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }
            if (!(o is ApplicationDirectory))
            {
                return false;
            }
            ApplicationDirectory directory = (ApplicationDirectory) o;
            if (this.m_appDirectory == null)
            {
                return (directory.m_appDirectory == null);
            }
            if (directory.m_appDirectory == null)
            {
                return false;
            }
            return (this.m_appDirectory.IsSubsetOf(directory.m_appDirectory) && directory.m_appDirectory.IsSubsetOf(this.m_appDirectory));
        }

        public override int GetHashCode() => 
            this.Directory.GetHashCode();

        int IBuiltInEvidence.GetRequiredSize(bool verbose)
        {
            if (verbose)
            {
                return (this.Directory.Length + 3);
            }
            return (this.Directory.Length + 1);
        }

        int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
        {
            int intFromCharArray = BuiltInEvidenceHelper.GetIntFromCharArray(buffer, position);
            position += 2;
            this.m_appDirectory = new URLString(new string(buffer, position, intFromCharArray));
            return (position + intFromCharArray);
        }

        int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
        {
            buffer[position++] = '\0';
            string directory = this.Directory;
            int length = directory.Length;
            if (verbose)
            {
                BuiltInEvidenceHelper.CopyIntToCharArray(length, buffer, position);
                position += 2;
            }
            directory.CopyTo(0, buffer, position, length);
            return (length + position);
        }

        public override string ToString() => 
            this.ToXml().ToString();

        internal SecurityElement ToXml()
        {
            SecurityElement element = new SecurityElement("System.Security.Policy.ApplicationDirectory");
            element.AddAttribute("version", "1");
            if (this.m_appDirectory != null)
            {
                element.AddChild(new SecurityElement("Directory", this.m_appDirectory.ToString()));
            }
            return element;
        }

        public string Directory =>
            this.m_appDirectory.ToString();
    }
}

