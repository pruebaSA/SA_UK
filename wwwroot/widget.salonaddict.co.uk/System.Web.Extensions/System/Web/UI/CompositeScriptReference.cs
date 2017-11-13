namespace System.Web.UI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Handlers;
    using System.Web.Util;

    [TypeConverter(typeof(EmptyStringExpandableObjectConverter)), DefaultProperty("Path"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class CompositeScriptReference : ScriptReferenceBase
    {
        private ScriptReferenceCollection _scripts;

        protected internal override string GetUrl(ScriptManager scriptManager, bool zip)
        {
            CultureInfo info;
            string str2;
            bool flag2;
            int isDebuggingEnabled;
            if (!scriptManager.DeploymentSectionRetail)
            {
                if (base.ScriptMode != ScriptMode.Debug)
                {
                    if ((base.ScriptMode == ScriptMode.Inherit) || (base.ScriptMode == ScriptMode.Auto))
                    {
                        isDebuggingEnabled = (int) scriptManager.IsDebuggingEnabled;
                    }
                    else
                    {
                        isDebuggingEnabled = 0;
                    }
                }
                else
                {
                    isDebuggingEnabled = 1;
                }
            }
            else
            {
                isDebuggingEnabled = 0;
            }
            bool flag = (bool) isDebuggingEnabled;
            if (string.IsNullOrEmpty(base.Path))
            {
                List<Pair<Assembly, List<Pair<string, CultureInfo>>>> assemblyResourceLists = new List<Pair<Assembly, List<Pair<string, CultureInfo>>>>();
                Pair<Assembly, List<Pair<string, CultureInfo>>> item = null;
                foreach (ScriptReference reference in this.Scripts)
                {
                    Assembly assembly;
                    Assembly assembly2;
                    int expressionStack_15C_0;
                    object systemWebExtensions;
                    object expressionStack_18E_0;
                    bool expressionStack_1D8_0;
                    bool flag3 = !string.IsNullOrEmpty(reference.Path);
                    if (!flag3)
                    {
                        if (!string.IsNullOrEmpty(scriptManager.ScriptPath))
                        {
                            expressionStack_15C_0 = (int) !reference.IgnoreScriptPath;
                        }
                        else
                        {
                            expressionStack_15C_0 = 0;
                        }
                    }
                    else
                    {
                        expressionStack_15C_0 = 1;
                    }
                    bool flag4 = (bool) expressionStack_15C_0;
                    if (!flag3)
                    {
                        Assembly assembly1 = reference.GetAssembly();
                        if (assembly1 != null)
                        {
                            systemWebExtensions = assembly1;
                            goto Label_0175;
                        }
                        else
                        {
                            Assembly expressionStack_16C_0 = assembly1;
                        }
                        systemWebExtensions = AssemblyCache.SystemWebExtensions;
                    }
                    else
                    {
                        systemWebExtensions = null;
                    }
                Label_0175:
                    assembly = (Assembly) systemWebExtensions;
                    if (!flag4)
                    {
                        Assembly assembly3 = reference.GetAssembly();
                        if (assembly3 != null)
                        {
                            expressionStack_18E_0 = assembly3;
                            goto Label_018E;
                        }
                        else
                        {
                            Assembly expressionStack_185_0 = assembly3;
                        }
                        expressionStack_18E_0 = AssemblyCache.SystemWebExtensions;
                    }
                    else
                    {
                        expressionStack_18E_0 = null;
                    }
                Label_018E:
                    assembly2 = (Assembly) expressionStack_18E_0;
                    CultureInfo culture = reference.DetermineCulture();
                    if ((item == null) || (item.First != assembly2))
                    {
                        item = new Pair<Assembly, List<Pair<string, CultureInfo>>>(assembly2, new List<Pair<string, CultureInfo>>());
                        assemblyResourceLists.Add(item);
                    }
                    string virtualPath = null;
                    ScriptMode effectiveScriptMode = reference.EffectiveScriptMode;
                    if (effectiveScriptMode != ScriptMode.Inherit)
                    {
                        expressionStack_1D8_0 = effectiveScriptMode == ScriptMode.Debug;
                    }
                    else
                    {
                        expressionStack_1D8_0 = flag;
                    }
                    bool flag5 = expressionStack_1D8_0;
                    if (flag4)
                    {
                        if (flag3)
                        {
                            virtualPath = reference.GetPath(reference.Path, flag5);
                            if (scriptManager.EnableScriptLocalization && !culture.Equals(CultureInfo.InvariantCulture))
                            {
                                virtualPath = virtualPath.Substring(0, virtualPath.Length - 2) + culture.ToString() + ".js";
                            }
                        }
                        else
                        {
                            virtualPath = ScriptReference.GetScriptPath(reference.GetResourceName(reference.Name, assembly, flag5), assembly, culture, scriptManager.ScriptPath);
                        }
                        if (UrlPath.IsRelativeUrl(virtualPath) && !UrlPath.IsAppRelativePath(virtualPath))
                        {
                            virtualPath = UrlPath.Combine(base.ClientUrlResolver.AppRelativeTemplateSourceDirectory, virtualPath);
                        }
                    }
                    else
                    {
                        virtualPath = reference.GetResourceName(reference.Name, assembly, flag5);
                    }
                    item.Second.Add(new Pair<string, CultureInfo>(virtualPath, culture));
                }
                return ScriptResourceHandler.GetScriptResourceUrl(assemblyResourceLists, zip, base.NotifyScriptLoaded);
            }
            string path = base.Path;
            if (!flag)
            {
                goto Label_0053;
            }
            path = ScriptReferenceBase.GetDebugPath(path);
        Label_0083:
            str2 = info.ToString();
            foreach (string str3 in base.ResourceUICultures)
            {
            }
            if (flag2)
            {
                goto Label_00D9;
            }
        Label_0053:
            if (!info.Equals(CultureInfo.InvariantCulture))
            {
                goto Label_0083;
            }
        Label_00D9:
            if (flag2)
            {
                path = path.Substring(0, path.Length - 2) + str2 + ".js";
            }
            return base.ClientUrlResolver.ResolveClientUrl(path);
        }

        protected internal override bool IsFromSystemWebExtensions()
        {
            foreach (ScriptReference reference in this.Scripts)
            {
                if (reference.GetAssembly() == AssemblyCache.SystemWebExtensions)
                {
                    return true;
                }
            }
            return false;
        }

        [MergableProperty(false), PersistenceMode(PersistenceMode.InnerProperty), Category("Behavior"), Editor("System.Web.UI.Design.CollectionEditorBase, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", typeof(UITypeEditor)), DefaultValue((string) null), ResourceDescription("CompositeScriptReference_Scripts"), NotifyParentProperty(true)]
        public ScriptReferenceCollection Scripts
        {
            get
            {
                if (this._scripts == null)
                {
                    this._scripts = new ScriptReferenceCollection();
                }
                return this._scripts;
            }
        }
    }
}

