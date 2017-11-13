namespace System.Web.UI.Design
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Web.UI;

    public class ExtenderControlDesigner : System.Web.UI.Design.ControlDesigner, IControlDesigner
    {
        private readonly IControlDesigner _controlDesigner;
        private static readonly string[] _hiddenProperties = new string[] { "EnableViewState", "ID", "TargetControlID" };
        private Control _targetControlUsedToUpdateDesignTimeHtml;

        public ExtenderControlDesigner()
        {
        }

        internal ExtenderControlDesigner(IControlDesigner controlDesigner)
        {
            this._controlDesigner = controlDesigner;
        }

        private void ComponentAdded(object sender, ComponentEventArgs e)
        {
            this.UpdateDesignTimeHtmlIfNeeded();
        }

        private void ComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            this.UpdateDesignTimeHtmlIfNeeded();
        }

        private void ComponentRemoved(object sender, ComponentEventArgs e)
        {
            this.UpdateDesignTimeHtmlIfNeeded();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                IComponentChangeService service = (IComponentChangeService) this.GetService(typeof(IComponentChangeService));
                if (service != null)
                {
                    service.ComponentAdded -= new ComponentEventHandler(this.ComponentAdded);
                    service.ComponentChanged -= new ComponentChangedEventHandler(this.ComponentChanged);
                    service.ComponentRemoved -= new ComponentEventHandler(this.ComponentRemoved);
                }
            }
            base.Dispose(disposing);
        }

        internal static ReadOnlyCollection<Type> ExtractTargetControlTypes(Type type)
        {
            Type[] typeArray;
            if (type == null)
            {
                typeArray = new Type[0];
            }
            else
            {
                object[] customAttributes = type.GetCustomAttributes(typeof(TargetControlTypeAttribute), true);
                typeArray = new Type[customAttributes.Length];
                for (int i = 0; i < customAttributes.Length; i++)
                {
                    typeArray[i] = ((TargetControlTypeAttribute) customAttributes[i]).TargetControlType;
                }
            }
            return new ReadOnlyCollection<Type>(typeArray);
        }

        public override string GetDesignTimeHtml() => 
            this.ControlDesigner.CreatePlaceHolderDesignTimeHtml();

        private Control GetTargetControl()
        {
            IComponent component = base.Component;
            if (component != null)
            {
                ISite site = component.Site;
                if (site != null)
                {
                    IContainer container = site.Container;
                    if (container != null)
                    {
                        ComponentCollection components = container.Components;
                        if (components != null)
                        {
                            return (components[this.ExtenderControl.TargetControlID] as Control);
                        }
                    }
                }
            }
            return null;
        }

        private bool HasValidTargetControl()
        {
            Control targetControl = this.GetTargetControl();
            if ((targetControl != null) && (this.ExtenderControl != null))
            {
                ReadOnlyCollection<Type> onlys = ExtractTargetControlTypes(this.ExtenderControl.GetType());
                Type c = targetControl.GetType();
                foreach (Type type2 in onlys)
                {
                    if (type2.IsAssignableFrom(c))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Initialize(IComponent component)
        {
            System.Web.UI.Design.ControlDesigner.VerifyInitializeArgument(component, typeof(System.Web.UI.ExtenderControl));
            base.Initialize(component);
            IComponentChangeService service = (IComponentChangeService) this.GetService(typeof(IComponentChangeService));
            if (service != null)
            {
                service.ComponentAdded += new ComponentEventHandler(this.ComponentAdded);
                service.ComponentChanged += new ComponentChangedEventHandler(this.ComponentChanged);
                service.ComponentRemoved += new ComponentEventHandler(this.ComponentRemoved);
            }
            base.SetViewFlags(ViewFlags.DesignTimeHtmlRequiresLoadComplete, true);
        }

        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            if (this.ExtenderControlFeaturesPresent && this.HasValidTargetControl())
            {
                foreach (string str in _hiddenProperties)
                {
                    PropertyDescriptor oldPropertyDescriptor = (PropertyDescriptor) properties[str];
                    if (oldPropertyDescriptor != null)
                    {
                        properties[str] = TypeDescriptor.CreateProperty(oldPropertyDescriptor.ComponentType, oldPropertyDescriptor, new Attribute[] { BrowsableAttribute.No });
                    }
                }
            }
        }

        string IControlDesigner.CreatePlaceHolderDesignTimeHtml() => 
            base.CreatePlaceHolderDesignTimeHtml();

        private void UpdateDesignTimeHtmlIfNeeded()
        {
            Control targetControl = this.GetTargetControl();
            if (targetControl != this._targetControlUsedToUpdateDesignTimeHtml)
            {
                this._targetControlUsedToUpdateDesignTimeHtml = targetControl;
                this.ControlDesigner.UpdateDesignTimeHtml();
            }
        }

        private IControlDesigner ControlDesigner
        {
            get
            {
                if (this._controlDesigner != null)
                {
                    return this._controlDesigner;
                }
                return this;
            }
        }

        private System.Web.UI.ExtenderControl ExtenderControl =>
            ((System.Web.UI.ExtenderControl) base.Component);

        private bool ExtenderControlFeaturesPresent
        {
            get
            {
                IDesignerHost host = (IDesignerHost) this.GetService(typeof(IDesignerHost));
                if (host != null)
                {
                    IComponent rootComponent = host.RootComponent;
                    if (rootComponent != null)
                    {
                        ISite site = rootComponent.Site;
                        if (site != null)
                        {
                            IDictionaryService service = (IDictionaryService) site.GetService(typeof(IDictionaryService));
                            if (service != null)
                            {
                                return object.Equals(service.GetValue("ExtenderControlFeaturesPresent"), true);
                            }
                        }
                    }
                }
                return false;
            }
        }

        bool IControlDesigner.Visible =>
            base.Visible;

        protected override bool Visible
        {
            get
            {
                if (this.ExtenderControlFeaturesPresent && this.HasValidTargetControl())
                {
                    return false;
                }
                return this.ControlDesigner.Visible;
            }
        }
    }
}

