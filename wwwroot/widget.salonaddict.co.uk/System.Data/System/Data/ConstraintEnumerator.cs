namespace System.Data
{
    using System;
    using System.Collections;

    internal class ConstraintEnumerator
    {
        private IEnumerator constraints;
        private Constraint currentObject;
        private IEnumerator tables;

        public ConstraintEnumerator(DataSet dataSet)
        {
            this.tables = dataSet?.Tables.GetEnumerator();
            this.currentObject = null;
        }

        public Constraint GetConstraint() => 
            this.currentObject;

        public bool GetNext()
        {
            this.currentObject = null;
            while (this.tables != null)
            {
                if (this.constraints == null)
                {
                    if (!this.tables.MoveNext())
                    {
                        this.tables = null;
                        return false;
                    }
                    this.constraints = ((DataTable) this.tables.Current).Constraints.GetEnumerator();
                }
                if (!this.constraints.MoveNext())
                {
                    this.constraints = null;
                }
                else
                {
                    Constraint current = (Constraint) this.constraints.Current;
                    if (this.IsValidCandidate(current))
                    {
                        this.currentObject = current;
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual bool IsValidCandidate(Constraint constraint) => 
            true;

        protected Constraint CurrentObject =>
            this.currentObject;
    }
}

