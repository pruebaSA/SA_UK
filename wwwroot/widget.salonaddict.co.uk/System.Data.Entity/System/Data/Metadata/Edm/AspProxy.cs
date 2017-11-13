namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal class AspProxy
    {
        private bool _triedLoadingWebAssembly;
        private Assembly _webAssembly;
        private const string BUILD_MANAGER_TYPE_NAME = "System.Web.Compilation.BuildManager";

        internal IEnumerable<Assembly> GetBuildManagerReferencedAssemblies()
        {
            Type type;
            IEnumerable<Assembly> enumerable;
            if (!this.TryGetBuildManagerType(out type))
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.UnableToFindReflectedType("System.Web.Compilation.BuildManager", "System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
            }
            MethodInfo method = type.GetMethod("GetReferencedAssemblies", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                return new List<Assembly>();
            }
            ICollection source = null;
            try
            {
                source = (ICollection) method.Invoke(null, null);
                if (source == null)
                {
                    return new List<Assembly>();
                }
                enumerable = source.Cast<Assembly>();
            }
            catch (TargetException exception)
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.UnableToDetermineApplicationContext, exception);
            }
            catch (TargetInvocationException exception2)
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.UnableToDetermineApplicationContext, exception2);
            }
            catch (MethodAccessException exception3)
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.UnableToDetermineApplicationContext, exception3);
            }
            return enumerable;
        }

        internal bool HasBuildManagerType()
        {
            Type type;
            return this.TryGetBuildManagerType(out type);
        }

        private void InitializeWebAssembly()
        {
            if (!this.TryInitializeWebAssembly())
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.UnableToDetermineApplicationContext);
            }
        }

        internal bool IsAspNetEnvironment()
        {
            if (!this.TryInitializeWebAssembly())
            {
                return false;
            }
            try
            {
                return (this.PrivateMapWebPath("~") != null);
            }
            catch (Exception exception)
            {
                if (!EntityUtil.IsCatchableExceptionType(exception))
                {
                    throw;
                }
                return false;
            }
        }

        internal string MapWebPath(string path)
        {
            path = this.PrivateMapWebPath(path);
            if (path == null)
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.InvalidUseOfWebPath("~"));
            }
            return path;
        }

        private string PrivateMapWebPath(string path)
        {
            string str;
            this.InitializeWebAssembly();
            try
            {
                str = (string) this._webAssembly.GetType("System.Web.Hosting.HostingEnvironment", true).GetMethod("MapPath").Invoke(null, new object[] { path });
            }
            catch (TargetException exception)
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.UnableToDetermineApplicationContext, exception);
            }
            catch (ArgumentException exception2)
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.UnableToDetermineApplicationContext, exception2);
            }
            catch (TargetInvocationException exception3)
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.UnableToDetermineApplicationContext, exception3);
            }
            catch (TargetParameterCountException exception4)
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.UnableToDetermineApplicationContext, exception4);
            }
            catch (MethodAccessException exception5)
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.UnableToDetermineApplicationContext, exception5);
            }
            catch (MemberAccessException exception6)
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.UnableToDetermineApplicationContext, exception6);
            }
            catch (TypeLoadException exception7)
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.UnableToDetermineApplicationContext, exception7);
            }
            return str;
        }

        private bool TryGetBuildManagerType(out Type buildManager)
        {
            this.InitializeWebAssembly();
            buildManager = this._webAssembly.GetType("System.Web.Compilation.BuildManager", false);
            return (buildManager != null);
        }

        private bool TryInitializeWebAssembly()
        {
            if (this._webAssembly != null)
            {
                return true;
            }
            if (!this._triedLoadingWebAssembly)
            {
                this._triedLoadingWebAssembly = true;
                try
                {
                    this._webAssembly = Assembly.Load("System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                    return (this._webAssembly != null);
                }
                catch (Exception exception)
                {
                    if (!EntityUtil.IsCatchableExceptionType(exception))
                    {
                        throw;
                    }
                }
            }
            return false;
        }
    }
}

