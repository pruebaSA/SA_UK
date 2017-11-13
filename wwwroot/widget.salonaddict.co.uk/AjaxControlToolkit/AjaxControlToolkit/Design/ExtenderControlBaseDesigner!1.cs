namespace AjaxControlToolkit.Design
{
    using AjaxControlToolkit;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Web.Script.Services;
    using System.Web.Services;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Windows.Forms;

    public class ExtenderControlBaseDesigner<T> : ExtenderControlDesigner, IExtenderProvider where T: ExtenderControlBase
    {
        private DesignerActionListCollection _actionLists;
        private int _disableDesignerFeatures;
        private ExtenderPropertyRenameDescProv<T> _renameProvider;
        private const int DisableDesignerFeaturesNo = 2;
        private const int DisableDesignerFeaturesUnknown = 0;
        private const int DisableDesignerFeaturesYes = 1;
        private const string ExtenderControlDictionaryKey = "ExtenderControlFeaturesPresent";

        static ExtenderControlBaseDesigner()
        {
            TypeDescriptor.AddAttributes(typeof(ExtenderControlBaseDesigner<T>), new System.Attribute[] { new ProvidePropertyAttribute("Extender", typeof(System.Web.UI.Control)) });
        }

        public bool CanExtend(object extendee)
        {
            System.Web.UI.Control target = extendee as System.Web.UI.Control;
            bool flag = false;
            if (this.DesignerFeaturesEnabled && (target != null))
            {
                flag = target.ID == this.ExtenderControl.TargetControlID;
                if (flag && (this._renameProvider == null))
                {
                    this._renameProvider = new ExtenderPropertyRenameDescProv<T>((ExtenderControlBaseDesigner<T>) this, target);
                    TypeDescriptor.AddProvider(this._renameProvider, target);
                }
            }
            return flag;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this._renameProvider != null))
            {
                TypeDescriptor.RemoveProvider(this._renameProvider, base.Component);
                this._renameProvider.Dispose();
                this._renameProvider = null;
            }
            base.Dispose(disposing);
        }

        [Browsable(true), TypeConverter(typeof(ExtenderPropertiesTypeDescriptor)), Category("Extenders")]
        public object GetExtender(object control)
        {
            System.Web.UI.Control control2 = control as System.Web.UI.Control;
            if (this.DesignerFeaturesEnabled && (control2 != null))
            {
                return new ExtenderPropertiesProxy(this.ExtenderControl, new string[] { "TargetControlID" });
            }
            return null;
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
        }

        protected override void PreFilterAttributes(IDictionary attributes)
        {
            base.PreFilterAttributes(attributes);
            if (this.DesignerFeaturesEnabled)
            {
                TargetControlTypeAttribute attribute = (TargetControlTypeAttribute) TypeDescriptor.GetAttributes(this)[typeof(TargetControlTypeAttribute)];
                if ((attribute != null) && !attribute.IsDefaultAttribute())
                {
                    attributes[typeof(TargetControlTypeAttribute)] = attribute;
                }
            }
        }

        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            if (this.DesignerFeaturesEnabled)
            {
                string[] array = new string[properties.Keys.Count];
                properties.Keys.CopyTo(array, 0);
                foreach (string str in array)
                {
                    PropertyDescriptor oldPropertyDescriptor = (PropertyDescriptor) properties[str];
                    if (str == "TargetControlID")
                    {
                        TargetControlTypeAttribute attribute = (TargetControlTypeAttribute) TypeDescriptor.GetAttributes(this.ExtenderControl)[typeof(TargetControlTypeAttribute)];
                        if ((attribute != null) && !attribute.IsDefaultAttribute())
                        {
                            System.Type type = typeof(TypedControlIDConverter<>).MakeGenericType(new System.Type[] { attribute.TargetControlType });
                            properties[str] = TypeDescriptor.CreateProperty(oldPropertyDescriptor.ComponentType, oldPropertyDescriptor, new System.Attribute[] { new TypeConverterAttribute(type) });
                        }
                    }
                    ExtenderControlPropertyAttribute attribute2 = (ExtenderControlPropertyAttribute) oldPropertyDescriptor.Attributes[typeof(ExtenderControlPropertyAttribute)];
                    if ((attribute2 != null) && attribute2.IsScriptProperty)
                    {
                        BrowsableAttribute attribute3 = (BrowsableAttribute) oldPropertyDescriptor.Attributes[typeof(BrowsableAttribute)];
                        if (attribute3.Browsable == BrowsableAttribute.Yes.Browsable)
                        {
                            properties[str] = TypeDescriptor.CreateProperty(oldPropertyDescriptor.ComponentType, oldPropertyDescriptor, new System.Attribute[] { BrowsableAttribute.No, ExtenderVisiblePropertyAttribute.Yes });
                        }
                    }
                }
            }
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (this._actionLists == null)
                {
                    this._actionLists = new DesignerActionListCollection();
                    this._actionLists.AddRange(base.ActionLists);
                    foreach (System.Type type in base.GetType().GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
                    {
                        if (type.IsSubclassOf(typeof(Delegate)))
                        {
                            PageMethodSignatureAttribute customAttribute = System.Attribute.GetCustomAttribute(type, typeof(PageMethodSignatureAttribute)) as PageMethodSignatureAttribute;
                            if (customAttribute != null)
                            {
                                MethodInfo method = type.GetMethod("Invoke");
                                if (method != null)
                                {
                                    this._actionLists.Add(new PageMethodDesignerActionList<T>(base.Component, method, customAttribute));
                                }
                            }
                        }
                    }
                }
                return this._actionLists;
            }
        }

        protected bool DesignerFeaturesEnabled
        {
            get
            {
                if (this._disableDesignerFeatures == 0)
                {
                    this._disableDesignerFeatures = 2;
                    IDesignerHost host = (IDesignerHost) this.GetService(typeof(IDesignerHost));
                    if (host != null)
                    {
                        IComponent rootComponent = host.RootComponent;
                        if ((rootComponent != null) && (rootComponent.Site != null))
                        {
                            IDictionaryService service = (IDictionaryService) rootComponent.Site.GetService(typeof(IDictionaryService));
                            if ((service != null) && (service.GetValue("ExtenderControlFeaturesPresent") != null))
                            {
                                this._disableDesignerFeatures = 1;
                            }
                        }
                    }
                }
                return (this._disableDesignerFeatures == 2);
            }
        }

        protected T ExtenderControl =>
            (base.Component as T);

        protected virtual string ExtenderPropertyName =>
            string.Format(CultureInfo.InvariantCulture, "{0} ({1})", new object[] { TypeDescriptor.GetComponentName(base.Component), this.ExtenderControl.GetType().Name });

        private class ExtenderPropertyRenameDescProv : FilterTypeDescriptionProvider<IComponent>
        {
            private ExtenderControlBaseDesigner<T> _owner;

            public ExtenderPropertyRenameDescProv(ExtenderControlBaseDesigner<T> owner, IComponent target) : base(target)
            {
                this._owner = owner;
                base.FilterExtendedProperties = true;
            }

            protected override PropertyDescriptor ProcessProperty(PropertyDescriptor baseProp)
            {
                if (((baseProp.Name == "Extender") && (baseProp.ComponentType == this._owner.GetType())) && (this._owner.ExtenderPropertyName != null))
                {
                    return TypeDescriptor.CreateProperty(baseProp.ComponentType, baseProp, new System.Attribute[] { new DisplayNameAttribute(this._owner.ExtenderPropertyName) });
                }
                return base.ProcessProperty(baseProp);
            }
        }

        private class PageMethodDesignerActionList : DesignerActionList
        {
            private PageMethodSignatureAttribute _attribute;
            private MethodInfo _signature;

            public PageMethodDesignerActionList(IComponent component, MethodInfo signature, PageMethodSignatureAttribute attribute) : base(component)
            {
                this._signature = signature;
                this._attribute = attribute;
            }

            private void AddPageMethod()
            {
                try
                {
                    IEventBindingService service;
                    string name = this._signature.DeclaringType.Name;
                    PropertyDescriptor descriptor = TypeDescriptor.GetProperties(base.Component)[this._attribute.ServicePathProperty];
                    if (descriptor != null)
                    {
                        string str2 = descriptor.GetValue(base.Component) as string;
                        if (!string.IsNullOrEmpty(str2))
                        {
                            ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.ShowMessage(string.Format(CultureInfo.CurrentCulture, "Cannot create page method \"{0}\" because the extender is using web service \"{1}\" instead!", new object[] { name, str2 }));
                            return;
                        }
                    }
                    if (this._attribute.IncludeContextParameter)
                    {
                        PropertyDescriptor descriptor2 = TypeDescriptor.GetProperties(base.Component)[this._attribute.UseContextKeyProperty];
                        if (descriptor2 != null)
                        {
                            descriptor2.SetValue(base.Component, true);
                        }
                    }
                    if (this.EnsureService<IEventBindingService>(out service))
                    {
                        service.ShowCode();
                        object reference = base.GetService(ReferencedAssemblies.EnvDTE.GetType("EnvDTE._DTE"));
                        if (reference == null)
                        {
                            ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.ShowMessage(string.Format(CultureInfo.CurrentCulture, "Cannot create page method \"{0}\" because {1} could not be acquired!", new object[] { this._signature.DeclaringType.Name, "EnvDTE._DTE" }));
                        }
                        else
                        {
                            DTE2 dte = new DTE2(reference);
                            try
                            {
                                FileCodeModel2 model = ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.LoadFileCodeModel(dte.ActiveDocument.ProjectItem);
                                if ((model == null) || (model.Reference == null))
                                {
                                    ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.ShowMessage(string.Format(CultureInfo.CurrentCulture, "Cannot create page method \"{0}\" because no CodeBehind or CodeFile file was found!", new object[] { name }));
                                }
                                else
                                {
                                    IDesignerHost host;
                                    if (this.EnsureService<IDesignerHost>(out host))
                                    {
                                        CodeClass2 classModel = ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.FindClass(model, host.RootComponentClassName);
                                        if ((classModel == null) || (classModel.Reference == null))
                                        {
                                            ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.ShowMessage(string.Format(CultureInfo.CurrentCulture, "Cannot create page method \"{0}\" because no CodeBehind or CodeFile file was found!", new object[] { name }));
                                        }
                                        else
                                        {
                                            PropertyDescriptor descriptor3 = TypeDescriptor.GetProperties(base.Component)[this._attribute.ServiceMethodProperty];
                                            if (descriptor3 != null)
                                            {
                                                string str3 = descriptor3.GetValue(base.Component) as string;
                                                if (!string.IsNullOrEmpty(str3))
                                                {
                                                    name = str3;
                                                }
                                                else
                                                {
                                                    string str4 = name;
                                                    int num = 2;
                                                    while (ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.FindMethod(classModel, name, this._signature) != null)
                                                    {
                                                        name = str4 + num++;
                                                    }
                                                    descriptor3.SetValue(base.Component, name);
                                                }
                                            }
                                            UndoContext undoContext = dte.UndoContext;
                                            if (((undoContext != null) && (undoContext.Reference != null)) && undoContext.IsOpen)
                                            {
                                                undoContext = null;
                                            }
                                            try
                                            {
                                                CodeFunction2 method = ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.FindMethod(classModel, name, this._signature);
                                                if ((method != null) && (method.Reference != null))
                                                {
                                                    if (this.PageMethodNeedsRepair(method) && (ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.ShowMessage(string.Format(CultureInfo.CurrentCulture, "Would you like to repair the existing page method \"{0}\"?", new object[] { name }), MessageBoxButtons.YesNo) == DialogResult.Yes))
                                                    {
                                                        if (undoContext != null)
                                                        {
                                                            undoContext.Open(string.Format(CultureInfo.CurrentCulture, "Repair \"{0}\" page method", new object[] { name }), false);
                                                        }
                                                        this.RepairPageMethod(method);
                                                    }
                                                }
                                                else
                                                {
                                                    if (undoContext != null)
                                                    {
                                                        undoContext.Open(string.Format(CultureInfo.CurrentCulture, "Add \"{0}\" page method", new object[] { name }), false);
                                                    }
                                                    this.CreatePageMethod(classModel, name);
                                                }
                                            }
                                            finally
                                            {
                                                if ((undoContext != null) && undoContext.IsOpen)
                                                {
                                                    undoContext.Close();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.UnloadWebProjectItem(dte.ActiveDocument.ProjectItem);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.ShowMessage(string.Format(CultureInfo.CurrentCulture, "Unexpected error ({0}): {1}{2}{3}", new object[] { exception.GetType().Name, exception.Message, Environment.NewLine, exception.StackTrace }));
                }
            }

            private static bool AreSameType(CodeTypeRef modelType, System.Type type)
            {
                if ((modelType == null) || (modelType.Reference == null))
                {
                    return (type == null);
                }
                if (modelType.TypeKind == vsCMTypeRef.vsCMTypeRefArray)
                {
                    if (!type.IsArray)
                    {
                        return false;
                    }
                    if (modelType.Rank != type.GetArrayRank())
                    {
                        return false;
                    }
                    return ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.AreSameType(modelType.ElementType, type.GetElementType());
                }
                if (!(modelType.TypeKind == vsCMTypeRef.vsCMTypeRefPointer))
                {
                    return (string.CompareOrdinal(modelType.AsFullName, ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.CreateCodeTypeRefName(type)) == 0);
                }
                if (!type.IsPointer)
                {
                    return false;
                }
                return ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.AreSameType(modelType.ElementType, type.GetElementType());
            }

            private static string CreateCodeTypeRefName(System.Type t) => 
                t.FullName.Replace('+', '.');

            private void CreatePageMethod(CodeClass2 classModel, string name)
            {
                ParameterInfo[] parameters = this._signature.GetParameters();
                CodeFunction2 function = classModel.AddFunction(name, vsCMFunction.vsCMFunctionFunction, ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.CreateCodeTypeRefName(this._signature.ReturnType), -1, vsCMAccess.vsCMAccessPublic, null);
                function.IsShared = true;
                foreach (ParameterInfo info in parameters)
                {
                    function.AddParameter(info.Name, ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.CreateCodeTypeRefName(info.ParameterType), -1);
                }
                function.AddAttribute(typeof(WebMethodAttribute).FullName, "", -1);
                function.AddAttribute(typeof(ScriptMethodAttribute).FullName, "", -1);
            }

            private bool EnsureService<S>(out S service) where S: class
            {
                service = base.GetService(typeof(S)) as S;
                if (((S) service) == null)
                {
                    ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.ShowMessage(string.Format(CultureInfo.CurrentCulture, "Cannot create page method \"{0}\" because {1} could not be acquired!", new object[] { this._signature.DeclaringType.Name, typeof(S).Name }));
                    return false;
                }
                return true;
            }

            private static CodeClass2 FindClass(FileCodeModel2 model, string name)
            {
                Queue<CodeElement2> queue = new Queue<CodeElement2>();
                foreach (object obj2 in model.CodeElements)
                {
                    queue.Enqueue(new CodeElement2(obj2));
                }
                while (queue.Count > 0)
                {
                    CodeElement2 element = queue.Dequeue();
                    if (((element != null) && (element.Reference != null)) && (element.IsCodeType || (element.Kind == vsCMElement.vsCMElementNamespace)))
                    {
                        if ((element.Kind == vsCMElement.vsCMElementClass) && (string.CompareOrdinal(element.FullName, name) == 0))
                        {
                            return new CodeClass2(element.Reference);
                        }
                        if (element.Children != null)
                        {
                            foreach (object obj3 in element.Children)
                            {
                                queue.Enqueue(new CodeElement2(obj3));
                            }
                        }
                    }
                }
                return null;
            }

            private static CodeFunction2 FindMethod(CodeClass2 classModel, string name, MethodInfo signature)
            {
                ParameterInfo[] parameters = signature.GetParameters();
                foreach (CodeFunction2 function in ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.FindMethods(classModel, name))
                {
                    if (((function == null) || (function.Reference == null)) || (!ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.AreSameType(function.Type, signature.ReturnType) || (function.Parameters.Count != parameters.Length)))
                    {
                        continue;
                    }
                    bool flag = false;
                    int num = 0;
                    foreach (object obj2 in function.Parameters)
                    {
                        CodeParameter2 parameter = new CodeParameter2(obj2);
                        if ((parameter.Reference == null) || !ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.AreSameType(parameter.Type, parameters[num++].ParameterType))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        return function;
                    }
                }
                return null;
            }

            private static CodeFunction2[] FindMethods(CodeClass2 classModel, string name)
            {
                List<CodeFunction2> list = new List<CodeFunction2>();
                foreach (object obj2 in classModel.Children)
                {
                    CodeElement2 element = new CodeElement2(obj2);
                    if (((element.Reference != null) && !(element.Kind != vsCMElement.vsCMElementFunction)) && (string.CompareOrdinal(element.Name, name) == 0))
                    {
                        list.Add(new CodeFunction2(obj2));
                    }
                }
                return list.ToArray();
            }

            public override DesignerActionItemCollection GetSortedActionItems()
            {
                DesignerActionItemCollection items = new DesignerActionItemCollection();
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(base.Component)[this._attribute.ServicePathProperty];
                if (((descriptor == null) || ((descriptor != null) && string.IsNullOrEmpty(descriptor.GetValue(base.Component) as string))) && (base.GetService(typeof(IEventBindingService)) != null))
                {
                    string displayName = string.Format(CultureInfo.CurrentCulture, "Add {0} page method", new object[] { this._attribute.FriendlyName });
                    items.Add(new DesignerActionMethodItem(this, "AddPageMethod", displayName, "Page Methods", displayName, true));
                }
                return items;
            }

            private static FileCodeModel2 LoadFileCodeModel(ProjectItem projectItem)
            {
                if ((projectItem == null) || (projectItem.Reference == null))
                {
                    throw new ArgumentNullException("projectItem", "projectItem cannot be null");
                }
                VSWebProjectItem item = new VSWebProjectItem(projectItem.Object);
                if (item.Reference != null)
                {
                    item.Load();
                    return item.ProjectItem.FileCodeModel;
                }
                return projectItem.FileCodeModel;
            }

            private bool PageMethodNeedsRepair(CodeFunction2 method)
            {
                if ((method == null) || (method.Reference == null))
                {
                    return false;
                }
                ParameterInfo[] parameters = this._signature.GetParameters();
                if (method.IsShared)
                {
                    if (method.Access != vsCMAccess.vsCMAccessPublic)
                    {
                        return true;
                    }
                    int num = 0;
                    foreach (object obj2 in method.Parameters)
                    {
                        CodeParameter2 parameter = new CodeParameter2(obj2);
                        if ((parameter.Reference == null) || (string.Compare(parameter.Name, parameters[num++].Name, StringComparison.Ordinal) != 0))
                        {
                            return true;
                        }
                    }
                    bool flag = false;
                    bool flag2 = false;
                    foreach (object obj3 in method.Attributes)
                    {
                        CodeAttribute2 attribute = new CodeAttribute2(obj3);
                        if (attribute.Reference != null)
                        {
                            flag |= !string.IsNullOrEmpty(attribute.Name) && attribute.Name.Contains("WebMethod");
                            flag2 |= !string.IsNullOrEmpty(attribute.Name) && attribute.Name.Contains("ScriptMethod");
                            if (flag && flag2)
                            {
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        return !flag2;
                    }
                }
                return true;
            }

            private void RepairPageMethod(CodeFunction2 method)
            {
                if ((method != null) && (method.Reference != null))
                {
                    method.IsShared = true;
                    method.Access = vsCMAccess.vsCMAccessPublic;
                    int num = 0;
                    ParameterInfo[] parameters = this._signature.GetParameters();
                    foreach (object obj2 in method.Parameters)
                    {
                        CodeParameter2 parameter = new CodeParameter2(obj2);
                        if (parameter.Reference != null)
                        {
                            parameter.Name = parameters[num++].Name;
                        }
                    }
                    bool flag = false;
                    bool flag2 = false;
                    foreach (object obj3 in method.Attributes)
                    {
                        CodeAttribute2 attribute = new CodeAttribute2(obj3);
                        if (attribute.Reference != null)
                        {
                            flag |= !string.IsNullOrEmpty(attribute.Name) && attribute.Name.Contains("WebMethod");
                            flag2 |= !string.IsNullOrEmpty(attribute.Name) && attribute.Name.Contains("ScriptMethod");
                            if (flag && flag2)
                            {
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        method.AddAttribute(typeof(WebMethodAttribute).FullName, "", -1);
                    }
                    if (!flag2)
                    {
                        method.AddAttribute(typeof(ScriptMethodAttribute).FullName, "", -1);
                    }
                }
            }

            private static DialogResult ShowMessage(string message) => 
                ExtenderControlBaseDesigner<T>.PageMethodDesignerActionList.ShowMessage(message, MessageBoxButtons.OK);

            private static DialogResult ShowMessage(string message, MessageBoxButtons buttons) => 
                MessageBox.Show(message, "Ajax Control Toolkit", buttons);

            private static void UnloadWebProjectItem(ProjectItem projectItem)
            {
                VSWebProjectItem item = new VSWebProjectItem(projectItem.Object);
                if (item.Reference != null)
                {
                    item.Unload();
                }
            }
        }
    }
}

