namespace System.Xml.Xsl.Qil
{
    using System;

    internal class QilScopedVisitor : QilVisitor
    {
        protected virtual void AfterVisit(QilNode node)
        {
            switch (node.NodeType)
            {
                case QilNodeType.Loop:
                case QilNodeType.Filter:
                case QilNodeType.Sort:
                    this.EndScope(((QilLoop) node).Variable);
                    break;

                case QilNodeType.SortKey:
                case QilNodeType.DocOrderDistinct:
                    break;

                case QilNodeType.Function:
                    foreach (QilNode node5 in ((QilFunction) node).Arguments)
                    {
                        this.EndScope(node5);
                    }
                    break;

                case QilNodeType.QilExpression:
                {
                    QilExpression expression = (QilExpression) node;
                    foreach (QilNode node2 in expression.FunctionList)
                    {
                        this.EndScope(node2);
                    }
                    foreach (QilNode node3 in expression.GlobalVariableList)
                    {
                        this.EndScope(node3);
                    }
                    foreach (QilNode node4 in expression.GlobalParameterList)
                    {
                        this.EndScope(node4);
                    }
                    break;
                }
                default:
                    return;
            }
        }

        protected virtual void BeforeVisit(QilNode node)
        {
            switch (node.NodeType)
            {
                case QilNodeType.Loop:
                case QilNodeType.Filter:
                case QilNodeType.Sort:
                    this.BeginScope(((QilLoop) node).Variable);
                    break;

                case QilNodeType.SortKey:
                case QilNodeType.DocOrderDistinct:
                    break;

                case QilNodeType.Function:
                    foreach (QilNode node5 in ((QilFunction) node).Arguments)
                    {
                        this.BeginScope(node5);
                    }
                    break;

                case QilNodeType.QilExpression:
                {
                    QilExpression expression = (QilExpression) node;
                    foreach (QilNode node2 in expression.GlobalParameterList)
                    {
                        this.BeginScope(node2);
                    }
                    foreach (QilNode node3 in expression.GlobalVariableList)
                    {
                        this.BeginScope(node3);
                    }
                    foreach (QilNode node4 in expression.FunctionList)
                    {
                        this.BeginScope(node4);
                    }
                    break;
                }
                default:
                    return;
            }
        }

        protected virtual void BeginScope(QilNode node)
        {
        }

        protected virtual void EndScope(QilNode node)
        {
        }

        protected override QilNode Visit(QilNode n)
        {
            this.BeforeVisit(n);
            QilNode node = base.Visit(n);
            this.AfterVisit(n);
            return node;
        }
    }
}

