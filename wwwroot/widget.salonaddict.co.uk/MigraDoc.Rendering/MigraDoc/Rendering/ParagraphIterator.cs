namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using System;
    using System.Collections;

    internal class ParagraphIterator
    {
        private readonly DocumentObject current;
        private readonly ArrayList positionIndices;
        private readonly ParagraphElements rootNode;

        internal ParagraphIterator(ParagraphElements rootNode)
        {
            this.rootNode = rootNode;
            this.current = rootNode;
            this.positionIndices = new ArrayList();
        }

        private ParagraphIterator(ParagraphElements rootNode, DocumentObject current, ArrayList indices)
        {
            this.rootNode = rootNode;
            this.positionIndices = indices;
            this.current = current;
        }

        internal ParagraphIterator GetFirstLeaf()
        {
            if (this.rootNode.Count == 0)
            {
                return null;
            }
            return this.SeekFirstLeaf();
        }

        internal ParagraphIterator GetLastLeaf()
        {
            if (this.rootNode.Count == 0)
            {
                return null;
            }
            return this.SeekLastLeaf();
        }

        internal ParagraphIterator GetNextLeaf()
        {
            ParagraphIterator parentIterator = this.GetParentIterator();
            if (parentIterator == null)
            {
                return null;
            }
            int lastIndex = this.LastIndex;
            ParagraphElements current = (ParagraphElements) parentIterator.current;
            while (lastIndex == (current.Count - 1))
            {
                lastIndex = parentIterator.LastIndex;
                parentIterator = parentIterator.GetParentIterator();
                if (parentIterator == null)
                {
                    break;
                }
                current = (ParagraphElements) parentIterator.current;
            }
            if (parentIterator == null)
            {
                return null;
            }
            int num2 = lastIndex + 1;
            if (num2 >= current.Count)
            {
                return null;
            }
            ArrayList indices = (ArrayList) parentIterator.positionIndices.Clone();
            indices.Add(num2);
            DocumentObject nodeObject = this.GetNodeObject(current[num2]);
            ParagraphIterator iterator2 = new ParagraphIterator(this.rootNode, nodeObject, indices);
            return iterator2.SeekFirstLeaf();
        }

        private DocumentObject GetNodeObject(DocumentObject obj)
        {
            if (obj is FormattedText)
            {
                return ((FormattedText) obj).Elements;
            }
            if (obj is Hyperlink)
            {
                return ((Hyperlink) obj).Elements;
            }
            return obj;
        }

        private ParagraphIterator GetParentIterator()
        {
            if (this.positionIndices.Count == 0)
            {
                return null;
            }
            ArrayList indices = (ArrayList) this.positionIndices.Clone();
            indices.RemoveAt(indices.Count - 1);
            return new ParagraphIterator(this.rootNode, DocumentRelations.GetParentOfType(this.current, typeof(ParagraphElements)), indices);
        }

        internal ParagraphIterator GetPreviousLeaf()
        {
            ParagraphIterator parentIterator = this.GetParentIterator();
            if (parentIterator == null)
            {
                return null;
            }
            int lastIndex = this.LastIndex;
            ParagraphElements current = (ParagraphElements) parentIterator.current;
            while (lastIndex == 0)
            {
                lastIndex = parentIterator.LastIndex;
                parentIterator = parentIterator.GetParentIterator();
                if (parentIterator == null)
                {
                    break;
                }
                current = (ParagraphElements) parentIterator.current;
            }
            if (parentIterator == null)
            {
                return null;
            }
            int num2 = lastIndex - 1;
            if (num2 < 0)
            {
                return null;
            }
            ArrayList indices = (ArrayList) parentIterator.positionIndices.Clone();
            indices.Add(num2);
            DocumentObject nodeObject = this.GetNodeObject(current[num2]);
            ParagraphIterator iterator2 = new ParagraphIterator(this.rootNode, nodeObject, indices);
            return iterator2.SeekLastLeaf();
        }

        private ParagraphIterator SeekFirstLeaf()
        {
            DocumentObject current = this.Current;
            if (!(current is ParagraphElements))
            {
                return this;
            }
            ArrayList indices = (ArrayList) this.positionIndices.Clone();
            while (current is ParagraphElements)
            {
                ParagraphElements elements = (ParagraphElements) current;
                if (elements.Count == 0)
                {
                    return new ParagraphIterator(this.rootNode, current, indices);
                }
                indices.Add(0);
                current = this.GetNodeObject(elements[0]);
            }
            return new ParagraphIterator(this.rootNode, current, indices);
        }

        private ParagraphIterator SeekLastLeaf()
        {
            DocumentObject current = this.Current;
            if (!(current is ParagraphElements))
            {
                return this;
            }
            ArrayList indices = (ArrayList) this.positionIndices.Clone();
            while (current is ParagraphElements)
            {
                ParagraphElements elements = (ParagraphElements) current;
                if (((ParagraphElements) current).Count == 0)
                {
                    return new ParagraphIterator(this.rootNode, current, indices);
                }
                int num = ((ParagraphElements) current).Count - 1;
                indices.Add(num);
                current = this.GetNodeObject(elements[num]);
            }
            return new ParagraphIterator(this.rootNode, current, indices);
        }

        internal DocumentObject Current =>
            this.current;

        internal bool IsFirstLeaf =>
            (!(this.current is DocumentElements) && (this.GetPreviousLeaf() == null));

        internal bool IsLastLeaf =>
            (!(this.current is DocumentElements) && (this.GetNextLeaf() == null));

        private int LastIndex
        {
            get
            {
                if (this.positionIndices.Count == 0)
                {
                    return -1;
                }
                return (int) this.positionIndices[this.positionIndices.Count - 1];
            }
        }
    }
}

