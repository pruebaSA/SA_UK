namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;

    internal class KeyVec
    {
        private VarVec m_keys;
        private bool m_noKeys;

        internal KeyVec(Command itree)
        {
            this.m_keys = itree.CreateVarVec();
            this.m_noKeys = true;
        }

        internal void Clear()
        {
            this.m_noKeys = true;
            this.m_keys.Clear();
        }

        internal void InitFrom(IEnumerable<Var> varSet)
        {
            this.InitFrom(varSet, false);
        }

        internal void InitFrom(List<KeyVec> keyVecList)
        {
            this.m_noKeys = false;
            this.m_keys.Clear();
            foreach (KeyVec vec in keyVecList)
            {
                if (vec.m_noKeys)
                {
                    this.m_noKeys = true;
                    break;
                }
                this.m_keys.Or(vec.m_keys);
            }
        }

        internal void InitFrom(KeyVec keyset)
        {
            this.m_keys.InitFrom(keyset.m_keys);
            this.m_noKeys = keyset.m_noKeys;
        }

        internal void InitFrom(IEnumerable<Var> varSet, bool ignoreParameters)
        {
            this.m_keys.InitFrom(varSet, ignoreParameters);
            this.m_noKeys = false;
        }

        internal void InitFrom(KeyVec left, KeyVec right)
        {
            if (left.m_noKeys || right.m_noKeys)
            {
                this.m_noKeys = true;
            }
            else
            {
                this.m_noKeys = false;
                this.m_keys.InitFrom(left.m_keys);
                this.m_keys.Or(right.m_keys);
            }
        }

        internal VarVec KeyVars =>
            this.m_keys;

        internal bool NoKeys
        {
            get => 
                this.m_noKeys;
            set
            {
                this.m_noKeys = value;
            }
        }
    }
}

