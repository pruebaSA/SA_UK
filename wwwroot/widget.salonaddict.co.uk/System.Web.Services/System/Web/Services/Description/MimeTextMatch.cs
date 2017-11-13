namespace System.Web.Services.Description
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Web.Services;
    using System.Xml.Serialization;

    public sealed class MimeTextMatch
    {
        private int capture;
        private int group = 1;
        private bool ignoreCase;
        private MimeTextMatchCollection matches = new MimeTextMatchCollection();
        private string name;
        private string pattern;
        private int repeats = 1;
        private string type;

        [DefaultValue(0), XmlAttribute("capture")]
        public int Capture
        {
            get => 
                this.capture;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(Res.GetString("WebNegativeValue", new object[] { "capture" }));
                }
                this.capture = value;
            }
        }

        [DefaultValue(1), XmlAttribute("group")]
        public int Group
        {
            get => 
                this.group;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(Res.GetString("WebNegativeValue", new object[] { "group" }));
                }
                this.group = value;
            }
        }

        [XmlAttribute("ignoreCase")]
        public bool IgnoreCase
        {
            get => 
                this.ignoreCase;
            set
            {
                this.ignoreCase = value;
            }
        }

        [XmlElement("match")]
        public MimeTextMatchCollection Matches =>
            this.matches;

        [XmlAttribute("name")]
        public string Name
        {
            get
            {
                if (this.name != null)
                {
                    return this.name;
                }
                return string.Empty;
            }
            set
            {
                this.name = value;
            }
        }

        [XmlAttribute("pattern")]
        public string Pattern
        {
            get
            {
                if (this.pattern != null)
                {
                    return this.pattern;
                }
                return string.Empty;
            }
            set
            {
                this.pattern = value;
            }
        }

        [XmlIgnore]
        public int Repeats
        {
            get => 
                this.repeats;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(Res.GetString("WebNegativeValue", new object[] { "repeats" }));
                }
                this.repeats = value;
            }
        }

        [XmlAttribute("repeats"), DefaultValue("1")]
        public string RepeatsString
        {
            get
            {
                if (this.repeats != 0x7fffffff)
                {
                    return this.repeats.ToString(CultureInfo.InvariantCulture);
                }
                return "*";
            }
            set
            {
                if (value == "*")
                {
                    this.repeats = 0x7fffffff;
                }
                else
                {
                    this.Repeats = int.Parse(value, CultureInfo.InvariantCulture);
                }
            }
        }

        [XmlAttribute("type")]
        public string Type
        {
            get
            {
                if (this.type != null)
                {
                    return this.type;
                }
                return string.Empty;
            }
            set
            {
                this.type = value;
            }
        }
    }
}

