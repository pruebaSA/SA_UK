namespace System.Xml.Xsl.XsltOld
{
    using System;

    internal abstract class TemplateBaseAction : ContainerAction
    {
        protected int variableCount;
        private int variableFreeSlot;

        protected TemplateBaseAction()
        {
        }

        public int AllocateVariableSlot()
        {
            int variableFreeSlot = this.variableFreeSlot;
            this.variableFreeSlot++;
            if (this.variableCount < this.variableFreeSlot)
            {
                this.variableCount = this.variableFreeSlot;
            }
            return variableFreeSlot;
        }

        public void ReleaseVariableSlots(int n)
        {
        }
    }
}

