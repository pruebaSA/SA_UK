namespace System.Configuration
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Xml;

    public abstract class ConfigurationSection : ConfigurationElement
    {
        private System.Configuration.SectionInformation _section;

        protected ConfigurationSection()
        {
            this._section = new System.Configuration.SectionInformation(this);
        }

        protected internal virtual void DeserializeSection(XmlReader reader)
        {
            if (!reader.Read() || (reader.NodeType != XmlNodeType.Element))
            {
                throw new ConfigurationErrorsException(System.Configuration.SR.GetString("Config_base_expected_to_find_element"), reader);
            }
            this.DeserializeElement(reader, false);
        }

        protected internal virtual object GetRuntimeObject() => 
            this;

        protected internal override bool IsModified()
        {
            if (!this.SectionInformation.IsModifiedFlags())
            {
                return base.IsModified();
            }
            return true;
        }

        protected internal override void ResetModified()
        {
            this.SectionInformation.ResetModifiedFlags();
            base.ResetModified();
        }

        protected internal virtual string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
        {
            ConfigurationElement.ValidateElement(this, null, true);
            ConfigurationElement element = base.CreateElement(base.GetType());
            element.Unmerge(this, parentElement, saveMode);
            StringWriter w = new StringWriter(CultureInfo.InvariantCulture);
            XmlTextWriter writer = new XmlTextWriter(w) {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };
            element.DataToWriteInternal = saveMode != ConfigurationSaveMode.Minimal;
            element.SerializeToXmlElement(writer, name);
            writer.Flush();
            return w.ToString();
        }

        public System.Configuration.SectionInformation SectionInformation =>
            this._section;
    }
}

