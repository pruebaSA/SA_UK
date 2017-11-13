﻿namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Reflection;

    internal class OpcodeList
    {
        private QueryBuffer<Opcode> opcodes;

        public OpcodeList(int capacity)
        {
            this.opcodes = new QueryBuffer<Opcode>(capacity);
        }

        public void Add(Opcode opcode)
        {
            this.opcodes.Add(opcode);
        }

        public int IndexOf(Opcode opcode) => 
            this.opcodes.IndexOf(opcode);

        public void Remove(Opcode opcode)
        {
            this.opcodes.Remove(opcode);
        }

        public void Trim()
        {
            this.opcodes.TrimToCount();
        }

        public int Count =>
            this.opcodes.count;

        public Opcode this[int index]
        {
            get => 
                this.opcodes[index];
            set
            {
                this.opcodes[index] = value;
            }
        }
    }
}

