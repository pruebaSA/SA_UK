namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    internal class VarVec : IEnumerable<Var>, IEnumerable
    {
        private BitArray m_bitVector = new BitArray(0x40);
        private Command m_command;

        internal VarVec(Command command)
        {
            this.m_command = command;
        }

        private void Align(VarVec other)
        {
            if (other.m_bitVector.Count != this.m_bitVector.Count)
            {
                if (other.m_bitVector.Count > this.m_bitVector.Count)
                {
                    this.m_bitVector.Length = other.m_bitVector.Count;
                }
                else
                {
                    other.m_bitVector.Length = this.m_bitVector.Count;
                }
            }
        }

        private void Align(int idx)
        {
            if (idx >= this.m_bitVector.Count)
            {
                this.m_bitVector.Length = idx + 1;
            }
        }

        internal void And(VarVec other)
        {
            this.Align(other);
            this.m_bitVector.And(other.m_bitVector);
        }

        internal void Clear()
        {
            this.m_bitVector.Length = 0;
        }

        internal void Clear(Var v)
        {
            this.Align(v.Id);
            this.m_bitVector.Set(v.Id, false);
        }

        public VarVec Clone()
        {
            VarVec vec = this.m_command.CreateVarVec();
            vec.InitFrom(this);
            return vec;
        }

        public IEnumerator<Var> GetEnumerator() => 
            this.m_command.GetVarVecEnumerator(this);

        internal void InitFrom(IEnumerable<Var> other)
        {
            this.InitFrom(other, false);
        }

        internal void InitFrom(VarVec other)
        {
            this.Clear();
            this.m_bitVector.Length = other.m_bitVector.Length;
            this.m_bitVector.Or(other.m_bitVector);
        }

        internal void InitFrom(IEnumerable<Var> other, bool ignoreParameters)
        {
            this.Clear();
            foreach (Var var in other)
            {
                if (!ignoreParameters || (var.VarType != VarType.Parameter))
                {
                    this.Set(var);
                }
            }
        }

        internal bool IsSet(Var v)
        {
            this.Align(v.Id);
            return this.m_bitVector.Get(v.Id);
        }

        internal void Minus(VarVec other)
        {
            VarVec vec = this.m_command.CreateVarVec(other);
            vec.m_bitVector.Length = this.m_bitVector.Length;
            vec.m_bitVector.Not();
            this.And(vec);
            this.m_command.ReleaseVarVec(vec);
        }

        internal void Or(VarVec other)
        {
            this.Align(other);
            this.m_bitVector.Or(other.m_bitVector);
        }

        internal bool Overlaps(VarVec other)
        {
            VarVec vec = this.m_command.CreateVarVec(other);
            vec.And(this);
            bool flag = !vec.IsEmpty;
            this.m_command.ReleaseVarVec(vec);
            return flag;
        }

        internal VarVec Remap(Dictionary<Var, Var> varMap)
        {
            VarVec vec = this.m_command.CreateVarVec();
            foreach (Var var in this)
            {
                Var var2;
                if (!varMap.TryGetValue(var, out var2))
                {
                    var2 = var;
                }
                vec.Set(var2);
            }
            return vec;
        }

        internal void Set(Var v)
        {
            this.Align(v.Id);
            this.m_bitVector.Set(v.Id, true);
        }

        internal bool Subsumes(VarVec other)
        {
            for (int i = 0; i < other.m_bitVector.Count; i++)
            {
                if (other.m_bitVector[i] && ((i >= this.m_bitVector.Count) || !this.m_bitVector[i]))
                {
                    return false;
                }
            }
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string str = string.Empty;
            foreach (Var var in this)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", new object[] { str, var.Id });
                str = ",";
            }
            return builder.ToString();
        }

        internal int Count
        {
            get
            {
                int num = 0;
                using (IEnumerator<Var> enumerator = this.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Var current = enumerator.Current;
                        num++;
                    }
                }
                return num;
            }
        }

        internal Var First
        {
            get
            {
                using (IEnumerator<Var> enumerator = this.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        return enumerator.Current;
                    }
                }
                return null;
            }
        }

        internal bool IsEmpty =>
            (this.First == null);

        internal class VarVecEnumerator : IEnumerator<Var>, IEnumerator, IDisposable
        {
            private BitArray m_bitArray;
            private Command m_command;
            private int m_position;

            internal VarVecEnumerator(VarVec vec)
            {
                this.Init(vec);
            }

            public void Dispose()
            {
                this.m_bitArray = null;
                this.m_command.ReleaseVarVecEnumerator(this);
            }

            internal void Init(VarVec vec)
            {
                this.m_position = -1;
                this.m_command = vec.m_command;
                this.m_bitArray = vec.m_bitVector;
            }

            public bool MoveNext()
            {
                this.m_position++;
                while (this.m_position < this.m_bitArray.Count)
                {
                    if (this.m_bitArray[this.m_position])
                    {
                        return true;
                    }
                    this.m_position++;
                }
                return false;
            }

            public void Reset()
            {
                this.m_position = -1;
            }

            public Var Current
            {
                get
                {
                    if ((this.m_position >= 0) && (this.m_position < this.m_bitArray.Count))
                    {
                        return this.m_command.GetVar(this.m_position);
                    }
                    return null;
                }
            }

            object IEnumerator.Current =>
                this.Current;
        }
    }
}

