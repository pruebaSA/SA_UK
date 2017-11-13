namespace MS.Internal.ComponentModel
{
    using System;
    using System.Reflection;
    using System.Windows;

    internal class AttachInfo
    {
        private AttachedPropertyBrowsableAttribute[] _attributes;
        private bool _attributesChecked;
        private readonly DependencyProperty _dp;
        private MethodInfo _getMethod;
        private bool _getMethodChecked;
        private AttachedPropertyBrowsableAttribute _paramTypeAttribute;
        private bool _paramTypeAttributeChecked;

        internal AttachInfo(DependencyProperty dp)
        {
            this._dp = dp;
        }

        internal bool CanAttach(DependencyObject instance)
        {
            if (this.AttachMethod == null)
            {
                return false;
            }
            int length = 0;
            AttachedPropertyBrowsableAttribute[] attributes = this.Attributes;
            if (attributes != null)
            {
                length = attributes.Length;
                for (int i = 0; i < length; i++)
                {
                    AttachedPropertyBrowsableAttribute attribute = attributes[i];
                    if (!attribute.IsBrowsable(instance, this._dp))
                    {
                        bool flag = false;
                        if (attribute.UnionResults)
                        {
                            Type type = attribute.GetType();
                            for (int j = 0; j < length; j++)
                            {
                                AttachedPropertyBrowsableAttribute attribute2 = attributes[j];
                                if ((type == attribute2.GetType()) && attribute2.IsBrowsable(instance, this._dp))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        if (!flag)
                        {
                            return false;
                        }
                    }
                }
            }
            return (length > 0);
        }

        private MethodInfo AttachMethod
        {
            get
            {
                if (!this._getMethodChecked)
                {
                    this._getMethod = DependencyObjectPropertyDescriptor.GetAttachedPropertyMethod(this._dp);
                    this._getMethodChecked = true;
                }
                return this._getMethod;
            }
        }

        private AttachedPropertyBrowsableAttribute[] Attributes
        {
            get
            {
                if (!this._attributesChecked)
                {
                    MethodInfo attachMethod = this.AttachMethod;
                    object[] customAttributes = null;
                    if (attachMethod != null)
                    {
                        AttachedPropertyBrowsableAttribute[] attributeArray = null;
                        customAttributes = attachMethod.GetCustomAttributes(typeof(AttachedPropertyBrowsableAttribute), false);
                        bool flag = false;
                        for (int i = 0; i < customAttributes.Length; i++)
                        {
                            if (customAttributes[i] is AttachedPropertyBrowsableForTypeAttribute)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag && (customAttributes.Length > 0))
                        {
                            attributeArray = new AttachedPropertyBrowsableAttribute[customAttributes.Length + 1];
                            for (int j = 0; j < customAttributes.Length; j++)
                            {
                                attributeArray[j] = (AttachedPropertyBrowsableAttribute) customAttributes[j];
                            }
                            attributeArray[customAttributes.Length] = this.ParameterTypeAttribute;
                        }
                        else
                        {
                            attributeArray = new AttachedPropertyBrowsableAttribute[customAttributes.Length];
                            for (int k = 0; k < customAttributes.Length; k++)
                            {
                                attributeArray[k] = (AttachedPropertyBrowsableAttribute) customAttributes[k];
                            }
                        }
                        this._attributes = attributeArray;
                    }
                    this._attributesChecked = true;
                }
                return this._attributes;
            }
        }

        private AttachedPropertyBrowsableAttribute ParameterTypeAttribute
        {
            get
            {
                if (!this._paramTypeAttributeChecked)
                {
                    MethodInfo attachMethod = this.AttachMethod;
                    if (attachMethod != null)
                    {
                        ParameterInfo[] parameters = attachMethod.GetParameters();
                        this._paramTypeAttribute = new AttachedPropertyBrowsableForTypeAttribute(parameters[0].ParameterType);
                    }
                    this._paramTypeAttributeChecked = true;
                }
                return this._paramTypeAttribute;
            }
        }
    }
}

