namespace System.Xml.Schema
{
    using System;
    using System.Collections;

    internal class ActiveAxis
    {
        private ArrayList axisStack;
        private Asttree axisTree;
        private int currentDepth;
        private bool isActive;

        internal ActiveAxis(Asttree axisTree)
        {
            this.axisTree = axisTree;
            this.currentDepth = -1;
            this.axisStack = new ArrayList(axisTree.SubtreeArray.Count);
            foreach (ForwardAxis axis in axisTree.SubtreeArray)
            {
                AxisStack stack = new AxisStack(axis, this);
                this.axisStack.Add(stack);
            }
            this.isActive = true;
        }

        public virtual bool EndElement(string localname, string URN)
        {
            if (this.currentDepth == 0)
            {
                this.isActive = false;
                this.currentDepth--;
            }
            if (this.isActive)
            {
                foreach (AxisStack stack in this.axisStack)
                {
                    stack.MoveToParent(localname, URN, this.currentDepth);
                }
                this.currentDepth--;
            }
            return false;
        }

        public bool MoveToAttribute(string localname, string URN)
        {
            if (!this.isActive)
            {
                return false;
            }
            bool flag = false;
            foreach (AxisStack stack in this.axisStack)
            {
                if (stack.MoveToAttribute(localname, URN, this.currentDepth + 1))
                {
                    flag = true;
                }
            }
            return flag;
        }

        public bool MoveToStartElement(string localname, string URN)
        {
            if (!this.isActive)
            {
                return false;
            }
            this.currentDepth++;
            bool flag = false;
            foreach (AxisStack stack in this.axisStack)
            {
                if (stack.Subtree.IsSelfAxis)
                {
                    if (stack.Subtree.IsDss || (this.CurrentDepth == 0))
                    {
                        flag = true;
                    }
                }
                else if ((this.CurrentDepth != 0) && stack.MoveToChild(localname, URN, this.currentDepth))
                {
                    flag = true;
                }
            }
            return flag;
        }

        internal void Reactivate()
        {
            this.isActive = true;
            this.currentDepth = -1;
        }

        public int CurrentDepth =>
            this.currentDepth;
    }
}

