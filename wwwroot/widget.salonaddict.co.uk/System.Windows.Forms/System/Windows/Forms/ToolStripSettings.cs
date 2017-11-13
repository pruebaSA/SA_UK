namespace System.Windows.Forms
{
    using System;
    using System.Configuration;
    using System.Drawing;

    internal class ToolStripSettings : ApplicationSettingsBase
    {
        internal ToolStripSettings(string settingsKey) : base(settingsKey)
        {
        }

        public override void Save()
        {
            this.IsDefault = false;
            base.Save();
        }

        [UserScopedSetting, DefaultSettingValue("true")]
        public bool IsDefault
        {
            get => 
                ((bool) this["IsDefault"]);
            set
            {
                this["IsDefault"] = value;
            }
        }

        [UserScopedSetting]
        public string ItemOrder
        {
            get => 
                (this["ItemOrder"] as string);
            set
            {
                this["ItemOrder"] = value;
            }
        }

        [UserScopedSetting, DefaultSettingValue("0,0")]
        public Point Location
        {
            get => 
                ((Point) this["Location"]);
            set
            {
                this["Location"] = value;
            }
        }

        [UserScopedSetting]
        public string Name
        {
            get => 
                (this["Name"] as string);
            set
            {
                this["Name"] = value;
            }
        }

        [UserScopedSetting, DefaultSettingValue("0,0")]
        public System.Drawing.Size Size
        {
            get => 
                ((System.Drawing.Size) this["Size"]);
            set
            {
                this["Size"] = value;
            }
        }

        [UserScopedSetting]
        public string ToolStripPanelName
        {
            get => 
                (this["ToolStripPanelName"] as string);
            set
            {
                this["ToolStripPanelName"] = value;
            }
        }

        [UserScopedSetting, DefaultSettingValue("true")]
        public bool Visible
        {
            get => 
                ((bool) this["Visible"]);
            set
            {
                this["Visible"] = value;
            }
        }
    }
}

