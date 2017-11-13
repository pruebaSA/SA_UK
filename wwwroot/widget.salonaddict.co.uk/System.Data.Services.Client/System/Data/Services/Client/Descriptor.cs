namespace System.Data.Services.Client
{
    using System;

    public abstract class Descriptor
    {
        private uint changeOrder = uint.MaxValue;
        private bool saveContentGenerated;
        private Exception saveError;
        private EntityStates saveResultProcessed;
        private EntityStates state;

        internal Descriptor(EntityStates state)
        {
            this.state = state;
        }

        internal uint ChangeOrder
        {
            get => 
                this.changeOrder;
            set
            {
                this.changeOrder = value;
            }
        }

        internal bool ContentGeneratedForSave
        {
            get => 
                this.saveContentGenerated;
            set
            {
                this.saveContentGenerated = value;
            }
        }

        internal virtual bool IsModified =>
            (EntityStates.Unchanged != this.state);

        internal abstract bool IsResource { get; }

        internal Exception SaveError
        {
            get => 
                this.saveError;
            set
            {
                this.saveError = value;
            }
        }

        internal EntityStates SaveResultWasProcessed
        {
            get => 
                this.saveResultProcessed;
            set
            {
                this.saveResultProcessed = value;
            }
        }

        public EntityStates State
        {
            get => 
                this.state;
            internal set
            {
                this.state = value;
            }
        }
    }
}

