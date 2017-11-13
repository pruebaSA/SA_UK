namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Xml.Xsl.Qil;

    internal static class TailCallAnalyzer
    {
        public static void Analyze(QilExpression qil)
        {
            foreach (QilFunction function in qil.FunctionList)
            {
                if (XmlILConstructInfo.Read(function).ConstructMethod == XmlILConstructMethod.Writer)
                {
                    AnalyzeDefinition(function.Definition);
                }
            }
        }

        private static void AnalyzeDefinition(QilNode nd)
        {
            QilNodeType nodeType = nd.NodeType;
            if (nodeType <= QilNodeType.Sequence)
            {
                switch (nodeType)
                {
                    case QilNodeType.Conditional:
                    {
                        QilTernary ternary = (QilTernary) nd;
                        AnalyzeDefinition(ternary.Center);
                        AnalyzeDefinition(ternary.Right);
                        return;
                    }
                    case QilNodeType.Choice:
                    {
                        QilChoice choice = (QilChoice) nd;
                        for (int i = 0; i < choice.Branches.Count; i++)
                        {
                            AnalyzeDefinition(choice.Branches[i]);
                        }
                        return;
                    }
                    case QilNodeType.Length:
                        return;

                    case QilNodeType.Sequence:
                    {
                        QilList list = (QilList) nd;
                        if (list.Count > 0)
                        {
                            AnalyzeDefinition(list[list.Count - 1]);
                        }
                        return;
                    }
                    case QilNodeType.Nop:
                        AnalyzeDefinition(((QilUnary) nd).Child);
                        return;
                }
            }
            else if (nodeType != QilNodeType.Loop)
            {
                if ((nodeType == QilNodeType.Invoke) && (XmlILConstructInfo.Read(nd).ConstructMethod == XmlILConstructMethod.Writer))
                {
                    OptimizerPatterns.Write(nd).AddPattern(OptimizerPatternName.TailCall);
                }
            }
            else
            {
                QilLoop loop = (QilLoop) nd;
                if ((loop.Variable.NodeType == QilNodeType.Let) || !loop.Variable.Binding.XmlType.MaybeMany)
                {
                    AnalyzeDefinition(loop.Body);
                }
            }
        }
    }
}

