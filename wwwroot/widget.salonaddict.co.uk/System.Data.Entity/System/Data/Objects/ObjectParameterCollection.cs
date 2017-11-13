namespace System.Data.Objects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Reflection;
    using System.Text;

    public sealed class ObjectParameterCollection : ICollection<ObjectParameter>, IEnumerable<ObjectParameter>, IEnumerable
    {
        private string _cacheKey;
        private bool _locked;
        private List<ObjectParameter> _parameters;
        private ClrPerspective _perspective;

        internal ObjectParameterCollection(ClrPerspective perspective)
        {
            EntityUtil.CheckArgumentNull<ClrPerspective>(perspective, "perspective");
            this._perspective = perspective;
            this._parameters = new List<ObjectParameter>();
        }

        public void Add(ObjectParameter parameter)
        {
            EntityUtil.CheckArgumentNull<ObjectParameter>(parameter, "parameter");
            this.CheckUnlocked();
            if (this.Contains(parameter))
            {
                throw EntityUtil.Argument(Strings.ObjectParameterCollection_ParameterAlreadyExists(parameter.Name), "parameter");
            }
            if (this.Contains(parameter.Name))
            {
                throw EntityUtil.Argument(Strings.ObjectParameterCollection_DuplicateParameterName(parameter.Name), "parameter");
            }
            if (!parameter.ValidateParameterType(this._perspective))
            {
                throw EntityUtil.ArgumentOutOfRange(Strings.ObjectParameter_InvalidParameterType(parameter.ParameterType.FullName), "parameter");
            }
            this._parameters.Add(parameter);
            this._cacheKey = null;
        }

        private void CheckUnlocked()
        {
            if (this._locked)
            {
                throw EntityUtil.InvalidOperation(Strings.ObjectParameterCollection_ParametersLocked);
            }
        }

        public void Clear()
        {
            this.CheckUnlocked();
            this._parameters.Clear();
            this._cacheKey = null;
        }

        public bool Contains(ObjectParameter parameter)
        {
            EntityUtil.CheckArgumentNull<ObjectParameter>(parameter, "parameter");
            return this._parameters.Contains(parameter);
        }

        public bool Contains(string name)
        {
            EntityUtil.CheckArgumentNull<string>(name, "name");
            return (this.IndexOf(name) != -1);
        }

        public void CopyTo(ObjectParameter[] array, int index)
        {
            this._parameters.CopyTo(array, index);
        }

        internal static ObjectParameterCollection DeepCopy(ObjectParameterCollection copyParams)
        {
            if (copyParams == null)
            {
                return null;
            }
            ObjectParameterCollection parameters = new ObjectParameterCollection(copyParams._perspective);
            foreach (ObjectParameter parameter in (IEnumerable<ObjectParameter>) copyParams)
            {
                parameters.Add(parameter.ShallowCopy());
            }
            return parameters;
        }

        internal string GetCacheKey()
        {
            if ((this._cacheKey == null) && (this._parameters.Count > 0))
            {
                if (1 == this._parameters.Count)
                {
                    ObjectParameter parameter = this._parameters[0];
                    this._cacheKey = "@@1" + parameter.Name + ":" + parameter.ParameterType.FullName;
                }
                else
                {
                    StringBuilder builder = new StringBuilder(this._parameters.Count * 20);
                    builder.Append("@@");
                    builder.Append(this._parameters.Count);
                    for (int i = 0; i < this._parameters.Count; i++)
                    {
                        if (i > 0)
                        {
                            builder.Append(";");
                        }
                        ObjectParameter parameter2 = this._parameters[i];
                        builder.Append(parameter2.Name);
                        builder.Append(":");
                        builder.Append(parameter2.ParameterType.FullName);
                    }
                    this._cacheKey = builder.ToString();
                }
            }
            return this._cacheKey;
        }

        private int IndexOf(string name)
        {
            int num = 0;
            foreach (ObjectParameter parameter in this._parameters)
            {
                if (string.Compare(name, parameter.Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return num;
                }
                num++;
            }
            return -1;
        }

        public bool Remove(ObjectParameter parameter)
        {
            EntityUtil.CheckArgumentNull<ObjectParameter>(parameter, "parameter");
            this.CheckUnlocked();
            bool flag = this._parameters.Remove(parameter);
            if (flag)
            {
                this._cacheKey = null;
            }
            return flag;
        }

        internal void SetReadOnly(bool isReadOnly)
        {
            this._locked = isReadOnly;
        }

        IEnumerator<ObjectParameter> IEnumerable<ObjectParameter>.GetEnumerator() => 
            this._parameters.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this._parameters.GetEnumerator();

        public int Count =>
            this._parameters.Count;

        public ObjectParameter this[string name]
        {
            get
            {
                int index = this.IndexOf(name);
                if (index == -1)
                {
                    throw EntityUtil.ArgumentOutOfRange(Strings.ObjectParameterCollection_ParameterNameNotFound(name), "name");
                }
                return this._parameters[index];
            }
        }

        bool ICollection<ObjectParameter>.IsReadOnly =>
            this._locked;
    }
}

