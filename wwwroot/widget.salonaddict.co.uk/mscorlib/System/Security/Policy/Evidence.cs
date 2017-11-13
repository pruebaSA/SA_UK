namespace System.Security.Policy
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security;
    using System.Security.Permissions;

    [Serializable, ComVisible(true)]
    public sealed class Evidence : ICollection, IEnumerable
    {
        private IList m_assemblyList;
        private IList m_hostList;
        private bool m_locked;

        public Evidence()
        {
            this.m_hostList = null;
            this.m_assemblyList = null;
            this.m_locked = false;
        }

        public Evidence(Evidence evidence)
        {
            if (evidence != null)
            {
                this.m_locked = false;
                this.Merge(evidence);
            }
        }

        internal Evidence(char[] buffer)
        {
            int position = 0;
            while (position < buffer.Length)
            {
                switch (buffer[position++])
                {
                    case '\0':
                    {
                        IBuiltInEvidence id = new ApplicationDirectory();
                        position = id.InitFromBuffer(buffer, position);
                        this.AddAssembly(id);
                        continue;
                    }
                    case '\x0001':
                    {
                        IBuiltInEvidence evidence2 = new Publisher();
                        position = evidence2.InitFromBuffer(buffer, position);
                        this.AddHost(evidence2);
                        continue;
                    }
                    case '\x0002':
                    {
                        IBuiltInEvidence evidence3 = new StrongName();
                        position = evidence3.InitFromBuffer(buffer, position);
                        this.AddHost(evidence3);
                        continue;
                    }
                    case '\x0003':
                    {
                        IBuiltInEvidence evidence4 = new Zone();
                        position = evidence4.InitFromBuffer(buffer, position);
                        this.AddHost(evidence4);
                        continue;
                    }
                    case '\x0004':
                    {
                        IBuiltInEvidence evidence5 = new Url();
                        position = evidence5.InitFromBuffer(buffer, position);
                        this.AddHost(evidence5);
                        continue;
                    }
                    case '\x0006':
                    {
                        IBuiltInEvidence evidence6 = new Site();
                        position = evidence6.InitFromBuffer(buffer, position);
                        this.AddHost(evidence6);
                        continue;
                    }
                    case '\a':
                    {
                        IBuiltInEvidence evidence7 = new PermissionRequestEvidence();
                        position = evidence7.InitFromBuffer(buffer, position);
                        this.AddHost(evidence7);
                        continue;
                    }
                    case '\b':
                    {
                        IBuiltInEvidence evidence8 = new Hash();
                        position = evidence8.InitFromBuffer(buffer, position);
                        this.AddHost(evidence8);
                        continue;
                    }
                    case '\t':
                    {
                        IBuiltInEvidence evidence9 = new GacInstalled();
                        position = evidence9.InitFromBuffer(buffer, position);
                        this.AddHost(evidence9);
                        continue;
                    }
                }
                throw new SerializationException(Environment.GetResourceString("Serialization_UnableToFixup"));
            }
        }

        public Evidence(object[] hostEvidence, object[] assemblyEvidence)
        {
            this.m_locked = false;
            if (hostEvidence != null)
            {
                this.m_hostList = ArrayList.Synchronized(new ArrayList(hostEvidence));
            }
            if (assemblyEvidence != null)
            {
                this.m_assemblyList = ArrayList.Synchronized(new ArrayList(assemblyEvidence));
            }
        }

        public void AddAssembly(object id)
        {
            if (this.m_assemblyList == null)
            {
                this.m_assemblyList = ArrayList.Synchronized(new ArrayList());
            }
            this.m_assemblyList.Add(id);
        }

        public void AddHost(object id)
        {
            if (this.m_hostList == null)
            {
                this.m_hostList = ArrayList.Synchronized(new ArrayList());
            }
            if (this.m_locked)
            {
                new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
            }
            this.m_hostList.Add(id);
        }

        [ComVisible(false)]
        public void Clear()
        {
            this.m_hostList = null;
            this.m_assemblyList = null;
        }

        internal Evidence Copy()
        {
            char[] buffer = PolicyManager.MakeEvidenceArray(this, true);
            if (buffer != null)
            {
                return new Evidence(buffer);
            }
            new PermissionSet(true).Assert();
            MemoryStream serializationStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(serializationStream, this);
            serializationStream.Position = 0L;
            return (Evidence) formatter.Deserialize(serializationStream);
        }

        public void CopyTo(Array array, int index)
        {
            int num = index;
            if (this.m_hostList != null)
            {
                this.m_hostList.CopyTo(array, num);
                num += this.m_hostList.Count;
            }
            if (this.m_assemblyList != null)
            {
                this.m_assemblyList.CopyTo(array, num);
            }
        }

        [ComVisible(false)]
        public override bool Equals(object obj)
        {
            Evidence evidence = obj as Evidence;
            if (evidence == null)
            {
                return false;
            }
            if ((this.m_hostList != null) && (evidence.m_hostList != null))
            {
                if (this.m_hostList.Count != evidence.m_hostList.Count)
                {
                    return false;
                }
                int count = this.m_hostList.Count;
                for (int i = 0; i < count; i++)
                {
                    bool flag = false;
                    for (int j = 0; j < count; j++)
                    {
                        if (object.Equals(this.m_hostList[i], evidence.m_hostList[j]))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        return false;
                    }
                }
            }
            else if ((this.m_hostList != null) || (evidence.m_hostList != null))
            {
                return false;
            }
            if ((this.m_assemblyList != null) && (evidence.m_assemblyList != null))
            {
                if (this.m_assemblyList.Count != evidence.m_assemblyList.Count)
                {
                    return false;
                }
                int num4 = this.m_assemblyList.Count;
                for (int k = 0; k < num4; k++)
                {
                    bool flag2 = false;
                    for (int m = 0; m < num4; m++)
                    {
                        if (object.Equals(this.m_assemblyList[k], evidence.m_assemblyList[m]))
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2)
                    {
                        return false;
                    }
                }
            }
            else if ((this.m_assemblyList != null) || (evidence.m_assemblyList != null))
            {
                return false;
            }
            return true;
        }

        internal object FindType(Type t)
        {
            int num;
            for (num = 0; num < ((this.m_hostList == null) ? 0 : this.m_hostList.Count); num++)
            {
                if (this.m_hostList[num].GetType() == t)
                {
                    return this.m_hostList[num];
                }
            }
            for (num = 0; num < ((this.m_assemblyList == null) ? 0 : this.m_assemblyList.Count); num++)
            {
                if (this.m_assemblyList[num].GetType() == t)
                {
                    return this.m_hostList[num];
                }
            }
            return null;
        }

        public IEnumerator GetAssemblyEnumerator() => 
            this.m_assemblyList?.GetEnumerator();

        public IEnumerator GetEnumerator() => 
            new EvidenceEnumerator(this);

        [ComVisible(false)]
        public override int GetHashCode()
        {
            int num = 0;
            if (this.m_hostList != null)
            {
                int count = this.m_hostList.Count;
                for (int i = 0; i < count; i++)
                {
                    num ^= this.m_hostList[i].GetHashCode();
                }
            }
            if (this.m_assemblyList != null)
            {
                int num4 = this.m_assemblyList.Count;
                for (int j = 0; j < num4; j++)
                {
                    num ^= this.m_assemblyList[j].GetHashCode();
                }
            }
            return num;
        }

        public IEnumerator GetHostEnumerator() => 
            this.m_hostList?.GetEnumerator();

        internal void MarkAllEvidenceAsUsed()
        {
            foreach (object obj2 in this)
            {
                IDelayEvaluatedEvidence evidence = obj2 as IDelayEvaluatedEvidence;
                if (evidence != null)
                {
                    evidence.MarkUsed();
                }
            }
        }

        public void Merge(Evidence evidence)
        {
            if (evidence != null)
            {
                IEnumerator enumerator;
                if (evidence.m_hostList != null)
                {
                    if (this.m_hostList == null)
                    {
                        this.m_hostList = ArrayList.Synchronized(new ArrayList());
                    }
                    if ((evidence.m_hostList.Count != 0) && this.m_locked)
                    {
                        new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
                    }
                    enumerator = evidence.m_hostList.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        this.m_hostList.Add(enumerator.Current);
                    }
                }
                if (evidence.m_assemblyList != null)
                {
                    if (this.m_assemblyList == null)
                    {
                        this.m_assemblyList = ArrayList.Synchronized(new ArrayList());
                    }
                    enumerator = evidence.m_assemblyList.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        this.m_assemblyList.Add(enumerator.Current);
                    }
                }
            }
        }

        internal void MergeWithNoDuplicates(Evidence evidence)
        {
            if (evidence != null)
            {
                IEnumerator enumerator;
                IEnumerator enumerator2;
                if (evidence.m_hostList != null)
                {
                    if (this.m_hostList == null)
                    {
                        this.m_hostList = ArrayList.Synchronized(new ArrayList());
                    }
                    enumerator2 = evidence.m_hostList.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        Type type = enumerator2.Current.GetType();
                        enumerator = this.m_hostList.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current.GetType() == type)
                            {
                                this.m_hostList.Remove(enumerator.Current);
                                break;
                            }
                        }
                        this.m_hostList.Add(enumerator2.Current);
                    }
                }
                if (evidence.m_assemblyList != null)
                {
                    if (this.m_assemblyList == null)
                    {
                        this.m_assemblyList = ArrayList.Synchronized(new ArrayList());
                    }
                    enumerator2 = evidence.m_assemblyList.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        Type type2 = enumerator2.Current.GetType();
                        enumerator = this.m_assemblyList.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current.GetType() == type2)
                            {
                                this.m_assemblyList.Remove(enumerator.Current);
                                break;
                            }
                        }
                        this.m_assemblyList.Add(enumerator2.Current);
                    }
                }
            }
        }

        [ComVisible(false)]
        public void RemoveType(Type t)
        {
            int num;
            for (num = 0; num < ((this.m_hostList == null) ? 0 : this.m_hostList.Count); num++)
            {
                if (this.m_hostList[num].GetType() == t)
                {
                    this.m_hostList.RemoveAt(num--);
                }
            }
            for (num = 0; num < ((this.m_assemblyList == null) ? 0 : this.m_assemblyList.Count); num++)
            {
                if (this.m_assemblyList[num].GetType() == t)
                {
                    this.m_assemblyList.RemoveAt(num--);
                }
            }
        }

        internal Evidence ShallowCopy()
        {
            Evidence evidence = new Evidence();
            IEnumerator hostEnumerator = this.GetHostEnumerator();
            while (hostEnumerator.MoveNext())
            {
                evidence.AddHost(hostEnumerator.Current);
            }
            hostEnumerator = this.GetAssemblyEnumerator();
            while (hostEnumerator.MoveNext())
            {
                evidence.AddAssembly(hostEnumerator.Current);
            }
            return evidence;
        }

        private bool WasStrongNameEvidenceUsed()
        {
            IDelayEvaluatedEvidence evidence = this.FindType(typeof(StrongName)) as IDelayEvaluatedEvidence;
            return ((evidence != null) && evidence.WasUsed);
        }

        public int Count =>
            (((this.m_hostList != null) ? this.m_hostList.Count : 0) + ((this.m_assemblyList != null) ? this.m_assemblyList.Count : 0));

        public bool IsReadOnly =>
            false;

        public bool IsSynchronized =>
            false;

        public bool Locked
        {
            get => 
                this.m_locked;
            set
            {
                if (!value)
                {
                    new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
                    this.m_locked = false;
                }
                else
                {
                    this.m_locked = true;
                }
            }
        }

        public object SyncRoot =>
            this;
    }
}

