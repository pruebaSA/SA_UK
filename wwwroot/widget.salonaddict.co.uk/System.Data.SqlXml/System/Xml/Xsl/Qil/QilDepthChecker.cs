namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Collections.Generic;
    using System.Xml.XmlConfiguration;
    using System.Xml.Xsl;

    internal class QilDepthChecker
    {
        private const int MAX_QIL_DEPTH = 800;
        private Dictionary<QilNode, bool> visitedRef = new Dictionary<QilNode, bool>();

        public static void Check(QilNode input)
        {
            if (XsltConfigSection.LimitXPathComplexity)
            {
                new QilDepthChecker().Check(input, 0);
            }
        }

        private void Check(QilNode input, int depth)
        {
            if (depth > 800)
            {
                throw XsltException.Create("Xslt_CompileError2", new string[0]);
            }
            if (input is QilReference)
            {
                if (this.visitedRef.ContainsKey(input))
                {
                    return;
                }
                this.visitedRef[input] = true;
            }
            int num = depth + 1;
            for (int i = 0; i < input.Count; i++)
            {
                QilNode node = input[i];
                if (node != null)
                {
                    this.Check(node, num);
                }
            }
        }
    }
}

