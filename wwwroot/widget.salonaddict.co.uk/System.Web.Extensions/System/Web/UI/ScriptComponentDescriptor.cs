namespace System.Web.UI
{
    using System;
    using System.Collections.Generic;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Web.Resources;
    using System.Web.Script.Serialization;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ScriptComponentDescriptor : ScriptDescriptor
    {
        private string _elementIDInternal;
        private SortedList<string, string> _events;
        private string _id;
        private SortedList<string, Expression> _properties;
        private bool _registerDispose;
        private JavaScriptSerializer _serializer;
        private string _type;

        public ScriptComponentDescriptor(string type)
        {
            this._registerDispose = true;
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException(AtlasWeb.Common_NullOrEmpty, "type");
            }
            this._type = type;
        }

        internal ScriptComponentDescriptor(string type, string elementID) : this(type)
        {
            if (string.IsNullOrEmpty(elementID))
            {
                throw new ArgumentException(AtlasWeb.Common_NullOrEmpty, "elementID");
            }
            this._elementIDInternal = elementID;
        }

        public void AddComponentProperty(string name, string componentID)
        {
            if (string.IsNullOrEmpty(componentID))
            {
                throw new ArgumentException(AtlasWeb.Common_NullOrEmpty, "componentID");
            }
            this.AddProperty(name, (Expression) new ComponentReference(componentID));
        }

        public void AddElementProperty(string name, string elementID)
        {
            if (string.IsNullOrEmpty(elementID))
            {
                throw new ArgumentException(AtlasWeb.Common_NullOrEmpty, "elementID");
            }
            this.AddProperty(name, (Expression) new ElementReference(elementID));
        }

        public void AddEvent(string name, string handler)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(AtlasWeb.Common_NullOrEmpty, "name");
            }
            if (string.IsNullOrEmpty(handler))
            {
                throw new ArgumentException(AtlasWeb.Common_NullOrEmpty, "handler");
            }
            this.Events[name] = handler;
        }

        public void AddProperty(string name, object value)
        {
            this.AddProperty(name, (Expression) new ObjectReference(value));
        }

        private void AddProperty(string name, Expression value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(AtlasWeb.Common_NullOrEmpty, "name");
            }
            this.Properties[name] = value;
        }

        public void AddScriptProperty(string name, string script)
        {
            if (string.IsNullOrEmpty(script))
            {
                throw new ArgumentException(AtlasWeb.Common_NullOrEmpty, "script");
            }
            this.AddProperty(name, (Expression) new ScriptExpression(script));
        }

        private void AppendEventsScript(StringBuilder builder)
        {
            if ((this._events != null) && (this._events.Count > 0))
            {
                builder.Append('{');
                bool flag = true;
                foreach (KeyValuePair<string, string> pair in this._events)
                {
                    if (flag)
                    {
                        flag = false;
                    }
                    else
                    {
                        builder.Append(',');
                    }
                    builder.Append('"');
                    builder.Append(JavaScriptString.QuoteString(pair.Key));
                    builder.Append('"');
                    builder.Append(':');
                    builder.Append(pair.Value);
                }
                builder.Append("}");
            }
            else
            {
                builder.Append("null");
            }
        }

        private void AppendPropertiesScript(StringBuilder builder)
        {
            bool flag = true;
            if ((this._properties != null) && (this._properties.Count > 0))
            {
                foreach (KeyValuePair<string, Expression> pair in this._properties)
                {
                    if (pair.Value.Type == ExpressionType.Script)
                    {
                        if (flag)
                        {
                            builder.Append("{");
                            flag = false;
                        }
                        else
                        {
                            builder.Append(",");
                        }
                        builder.Append('"');
                        builder.Append(JavaScriptString.QuoteString(pair.Key));
                        builder.Append('"');
                        builder.Append(':');
                        pair.Value.AppendValue(this.Serializer, builder);
                    }
                }
            }
            if (flag)
            {
                builder.Append("null");
            }
            else
            {
                builder.Append("}");
            }
        }

        private void AppendReferencesScript(StringBuilder builder)
        {
            bool flag = true;
            if ((this._properties != null) && (this._properties.Count > 0))
            {
                foreach (KeyValuePair<string, Expression> pair in this._properties)
                {
                    if (pair.Value.Type == ExpressionType.ComponentReference)
                    {
                        if (flag)
                        {
                            builder.Append("{");
                            flag = false;
                        }
                        else
                        {
                            builder.Append(",");
                        }
                        builder.Append('"');
                        builder.Append(JavaScriptString.QuoteString(pair.Key));
                        builder.Append('"');
                        builder.Append(':');
                        pair.Value.AppendValue(this.Serializer, builder);
                    }
                }
            }
            if (flag)
            {
                builder.Append("null");
            }
            else
            {
                builder.Append("}");
            }
        }

        protected internal override string GetScript()
        {
            if (!string.IsNullOrEmpty(this.ID))
            {
                this.AddProperty("id", this.ID);
            }
            StringBuilder builder = new StringBuilder();
            builder.Append("$create(");
            builder.Append(this.Type);
            builder.Append(", ");
            this.AppendPropertiesScript(builder);
            builder.Append(", ");
            this.AppendEventsScript(builder);
            builder.Append(", ");
            this.AppendReferencesScript(builder);
            if (this.ElementIDInternal != null)
            {
                builder.Append(", ");
                builder.Append("$get(\"");
                builder.Append(JavaScriptString.QuoteString(this.ElementIDInternal));
                builder.Append("\")");
            }
            builder.Append(");");
            return builder.ToString();
        }

        internal override void RegisterDisposeForDescriptor(ScriptManager scriptManager, Control owner)
        {
            if (this.RegisterDispose && scriptManager.SupportsPartialRendering)
            {
                scriptManager.RegisterDispose(owner, "$find('" + this.ID + "').dispose();");
            }
        }

        public virtual string ClientID =>
            this.ID;

        internal string ElementIDInternal =>
            this._elementIDInternal;

        private SortedList<string, string> Events
        {
            get
            {
                if (this._events == null)
                {
                    this._events = new SortedList<string, string>(StringComparer.Ordinal);
                }
                return this._events;
            }
        }

        public virtual string ID
        {
            get => 
                (this._id ?? string.Empty);
            set
            {
                this._id = value;
            }
        }

        private SortedList<string, Expression> Properties
        {
            get
            {
                if (this._properties == null)
                {
                    this._properties = new SortedList<string, Expression>(StringComparer.Ordinal);
                }
                return this._properties;
            }
        }

        internal bool RegisterDispose
        {
            get => 
                this._registerDispose;
            set
            {
                this._registerDispose = value;
            }
        }

        private JavaScriptSerializer Serializer
        {
            get
            {
                if (this._serializer == null)
                {
                    this._serializer = new JavaScriptSerializer();
                }
                return this._serializer;
            }
        }

        public string Type
        {
            get => 
                this._type;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(AtlasWeb.Common_NullOrEmpty, "value");
                }
                this._type = value;
            }
        }

        private sealed class ComponentReference : ScriptComponentDescriptor.Expression
        {
            private string _componentID;

            public ComponentReference(string componentID)
            {
                this._componentID = componentID;
            }

            public override void AppendValue(JavaScriptSerializer serializer, StringBuilder builder)
            {
                builder.Append('"');
                builder.Append(JavaScriptString.QuoteString(this._componentID));
                builder.Append('"');
            }

            public override ScriptComponentDescriptor.ExpressionType Type =>
                ScriptComponentDescriptor.ExpressionType.ComponentReference;
        }

        private sealed class ElementReference : ScriptComponentDescriptor.Expression
        {
            private string _elementID;

            public ElementReference(string elementID)
            {
                this._elementID = elementID;
            }

            public override void AppendValue(JavaScriptSerializer serializer, StringBuilder builder)
            {
                builder.Append("$get(\"");
                builder.Append(JavaScriptString.QuoteString(this._elementID));
                builder.Append("\")");
            }

            public override ScriptComponentDescriptor.ExpressionType Type =>
                ScriptComponentDescriptor.ExpressionType.Script;
        }

        private abstract class Expression
        {
            protected Expression()
            {
            }

            public abstract void AppendValue(JavaScriptSerializer serializer, StringBuilder builder);

            public abstract ScriptComponentDescriptor.ExpressionType Type { get; }
        }

        private enum ExpressionType
        {
            Script,
            ComponentReference
        }

        private sealed class ObjectReference : ScriptComponentDescriptor.Expression
        {
            private object _value;

            public ObjectReference(object value)
            {
                this._value = value;
            }

            public override void AppendValue(JavaScriptSerializer serializer, StringBuilder builder)
            {
                serializer.Serialize(this._value, builder, JavaScriptSerializer.SerializationFormat.JavaScript);
            }

            public override ScriptComponentDescriptor.ExpressionType Type =>
                ScriptComponentDescriptor.ExpressionType.Script;
        }

        private sealed class ScriptExpression : ScriptComponentDescriptor.Expression
        {
            private string _script;

            public ScriptExpression(string script)
            {
                this._script = script;
            }

            public override void AppendValue(JavaScriptSerializer serializer, StringBuilder builder)
            {
                builder.Append(this._script);
            }

            public override ScriptComponentDescriptor.ExpressionType Type =>
                ScriptComponentDescriptor.ExpressionType.Script;
        }
    }
}

