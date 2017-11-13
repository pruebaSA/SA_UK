namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Web.UI;
    using System.Xml;

    [DefaultProperty("Name"), PersistChildren(false), ParseChildren(true)]
    public class Animation
    {
        private List<Animation> _children = new List<Animation>();
        private string _name;
        private Dictionary<string, string> _properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static JavaScriptSerializer _serializer = new JavaScriptSerializer();

        static Animation()
        {
            _serializer.RegisterConverters(new JavaScriptConverter[] { new AnimationJavaScriptConverter() });
        }

        public static Animation Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            return _serializer.Deserialize<Animation>(json);
        }

        public static Animation Deserialize(XmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            Animation animation = new Animation {
                Name = node.Name
            };
            foreach (XmlAttribute attribute in node.Attributes)
            {
                animation.Properties.Add(attribute.Name, attribute.Value);
            }
            if (node.HasChildNodes)
            {
                foreach (XmlNode node2 in node.ChildNodes)
                {
                    animation.Children.Add(Deserialize(node2));
                }
            }
            return animation;
        }

        private static int GetLineNumber(string source, string tag)
        {
            using (XmlTextReader reader = new XmlTextReader(new StringReader(source)))
            {
                if (reader.Read())
                {
                    while (reader.Read())
                    {
                        if (string.Compare(reader.Name, tag, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            return reader.LineNumber;
                        }
                        if ((reader.NodeType == XmlNodeType.Element) && !reader.IsEmptyElement)
                        {
                            reader.Skip();
                        }
                    }
                }
            }
            return 1;
        }

        public static void Parse(string value, ExtenderControl extenderControl)
        {
            if (extenderControl == null)
            {
                throw new ArgumentNullException("extenderControl");
            }
            if ((value != null) && !string.IsNullOrEmpty(value.Trim()))
            {
                value = "<Animations>" + value + "</Animations>";
                XmlDocument document = new XmlDocument();
                XmlTextReader reader = new XmlTextReader(new StringReader(value));
                try
                {
                    document.Load(reader);
                }
                catch (XmlException exception)
                {
                    string message = string.Format(CultureInfo.CurrentCulture, "Invalid Animation definition for TargetControlID=\"{0}\": {1}", new object[] { extenderControl.TargetControlID, exception.Message });
                    throw new HttpParseException(message, new ArgumentException(message, exception), HttpContext.Current.Request.Path, value, exception.LineNumber);
                }
                finally
                {
                    if (reader != null)
                    {
                        ((IDisposable) reader).Dispose();
                    }
                }
                foreach (XmlNode node in document.DocumentElement.ChildNodes)
                {
                    PropertyDescriptor descriptor = TypeDescriptor.GetProperties(extenderControl)[node.Name];
                    if ((descriptor == null) || descriptor.IsReadOnly)
                    {
                        string str2 = string.Format(CultureInfo.CurrentCulture, "Animation on TargetControlID=\"{0}\" uses property {1}.{2} that does not exist or cannot be set", new object[] { extenderControl.TargetControlID, extenderControl.GetType().FullName, node.Name });
                        throw new HttpParseException(str2, new ArgumentException(str2), HttpContext.Current.Request.Path, value, GetLineNumber(value, node.Name));
                    }
                    if (node.ChildNodes.Count != 1)
                    {
                        string str3 = string.Format(CultureInfo.CurrentCulture, "Animation {0} for TargetControlID=\"{1}\" can only have one child node.", new object[] { node.Name, extenderControl.TargetControlID });
                        throw new HttpParseException(str3, new ArgumentException(str3), HttpContext.Current.Request.Path, value, GetLineNumber(value, node.Name));
                    }
                    XmlNode node2 = node.ChildNodes[0];
                    Animation animation = Deserialize(node2);
                    descriptor.SetValue(extenderControl, animation);
                }
            }
        }

        public static string Serialize(Animation animation) => 
            _serializer.Serialize(animation);

        public override string ToString() => 
            Serialize(this);

        [Browsable(false)]
        public IList<Animation> Children =>
            this._children;

        [Browsable(false)]
        public string Name
        {
            get => 
                this._name;
            set
            {
                this._name = value;
            }
        }

        [Browsable(false)]
        public Dictionary<string, string> Properties =>
            this._properties;
    }
}

