namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Collections;

    internal abstract class QilPatternVisitor : QilReplaceVisitor
    {
        private int lastReplacement;
        private QilPatterns patterns;
        private int replacementCnt;
        private int threshold;

        public QilPatternVisitor(QilPatterns patterns, QilFactory f) : base(f)
        {
            this.threshold = 0x7fffffff;
            this.Patterns = patterns;
        }

        protected virtual bool AllowReplace(int pattern, QilNode original)
        {
            if (this.Matching)
            {
                this.replacementCnt++;
                this.lastReplacement = pattern;
                return true;
            }
            return false;
        }

        protected virtual QilNode NoReplace(QilNode node) => 
            node;

        protected virtual QilNode Replace(int pattern, QilNode original, QilNode replacement)
        {
            replacement.SourceLine = original.SourceLine;
            return replacement;
        }

        protected override QilNode Visit(QilNode node)
        {
            if (node == null)
            {
                return this.VisitNull();
            }
            node = this.VisitChildren(node);
            return base.Visit(node);
        }

        protected override QilNode VisitActualParameterList(QilList n) => 
            this.NoReplace(n);

        protected override QilNode VisitAdd(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitAfter(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitAncestor(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitAncestorOrSelf(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitAnd(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitAttribute(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitAttributeCtor(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitAverage(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitBefore(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitBranchList(QilList n) => 
            this.NoReplace(n);

        protected override QilNode VisitChoice(QilChoice n) => 
            this.NoReplace(n);

        protected override QilNode VisitCommentCtor(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitConditional(QilTernary n) => 
            this.NoReplace(n);

        protected override QilNode VisitContent(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitDataSource(QilDataSource n) => 
            this.NoReplace(n);

        protected override QilNode VisitDeref(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitDescendant(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitDescendantOrSelf(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitDifference(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitDivide(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitDocOrderDistinct(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitDocumentCtor(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitElementCtor(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitEq(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitError(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitFalse(QilNode n) => 
            this.NoReplace(n);

        protected override QilNode VisitFilter(QilLoop n) => 
            this.NoReplace(n);

        protected override QilNode VisitFollowingSibling(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitFor(QilIterator n) => 
            this.NoReplace(n);

        protected override QilNode VisitFormalParameterList(QilList n) => 
            this.NoReplace(n);

        protected override QilNode VisitForReference(QilIterator n) => 
            this.NoReplace(n);

        protected override QilNode VisitFunction(QilFunction n) => 
            this.NoReplace(n);

        protected override QilNode VisitFunctionList(QilList n) => 
            this.NoReplace(n);

        protected override QilNode VisitFunctionReference(QilFunction n) => 
            this.NoReplace(n);

        protected override QilNode VisitGe(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitGlobalParameterList(QilList n) => 
            this.NoReplace(n);

        protected override QilNode VisitGlobalVariableList(QilList n) => 
            this.NoReplace(n);

        protected override QilNode VisitGt(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitIntersection(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitInvoke(QilInvoke n) => 
            this.NoReplace(n);

        protected override QilNode VisitIs(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitIsEmpty(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitIsType(QilTargetType n) => 
            this.NoReplace(n);

        protected override QilNode VisitLe(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitLength(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitLet(QilIterator n) => 
            this.NoReplace(n);

        protected override QilNode VisitLetReference(QilIterator n) => 
            this.NoReplace(n);

        protected override QilNode VisitLiteralDecimal(QilLiteral n) => 
            this.NoReplace(n);

        protected override QilNode VisitLiteralDouble(QilLiteral n) => 
            this.NoReplace(n);

        protected override QilNode VisitLiteralInt32(QilLiteral n) => 
            this.NoReplace(n);

        protected override QilNode VisitLiteralInt64(QilLiteral n) => 
            this.NoReplace(n);

        protected override QilNode VisitLiteralObject(QilLiteral n) => 
            this.NoReplace(n);

        protected override QilNode VisitLiteralQName(QilName n) => 
            this.NoReplace(n);

        protected override QilNode VisitLiteralString(QilLiteral n) => 
            this.NoReplace(n);

        protected override QilNode VisitLiteralType(QilLiteral n) => 
            this.NoReplace(n);

        protected override QilNode VisitLocalNameOf(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitLoop(QilLoop n) => 
            this.NoReplace(n);

        protected override QilNode VisitLt(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitMaximum(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitMinimum(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitModulo(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitMultiply(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitNameOf(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitNamespaceDecl(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitNamespaceUriOf(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitNe(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitNegate(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitNodeRange(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitNop(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitNot(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitOptimizeBarrier(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitOr(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitParameter(QilParameter n) => 
            this.NoReplace(n);

        protected override QilNode VisitParameterReference(QilParameter n) => 
            this.NoReplace(n);

        protected override QilNode VisitParent(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitPICtor(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitPositionOf(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitPreceding(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitPrecedingSibling(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitPrefixOf(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitQilExpression(QilExpression n) => 
            this.NoReplace(n);

        protected override QilNode VisitRawTextCtor(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitRoot(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitRtfCtor(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitSequence(QilList n) => 
            this.NoReplace(n);

        protected override QilNode VisitSort(QilLoop n) => 
            this.NoReplace(n);

        protected override QilNode VisitSortKey(QilSortKey n) => 
            this.NoReplace(n);

        protected override QilNode VisitSortKeyList(QilList n) => 
            this.NoReplace(n);

        protected override QilNode VisitStrConcat(QilStrConcat n) => 
            this.NoReplace(n);

        protected override QilNode VisitStrLength(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitStrParseQName(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitSubtract(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitSum(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitTextCtor(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitTrue(QilNode n) => 
            this.NoReplace(n);

        protected override QilNode VisitTypeAssert(QilTargetType n) => 
            this.NoReplace(n);

        protected override QilNode VisitUnion(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitUnknown(QilNode n) => 
            this.NoReplace(n);

        protected override QilNode VisitWarning(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitXmlContext(QilNode n) => 
            this.NoReplace(n);

        protected override QilNode VisitXPathFollowing(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitXPathNamespace(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitXPathNodeValue(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitXPathPreceding(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitXsltConvert(QilTargetType n) => 
            this.NoReplace(n);

        protected override QilNode VisitXsltCopy(QilBinary n) => 
            this.NoReplace(n);

        protected override QilNode VisitXsltCopyOf(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitXsltGenerateId(QilUnary n) => 
            this.NoReplace(n);

        protected override QilNode VisitXsltInvokeEarlyBound(QilInvokeEarlyBound n) => 
            this.NoReplace(n);

        protected override QilNode VisitXsltInvokeLateBound(QilInvokeLateBound n) => 
            this.NoReplace(n);

        public int LastReplacement =>
            this.lastReplacement;

        public bool Matching =>
            (this.ReplacementCount < this.Threshold);

        public QilPatterns Patterns
        {
            get => 
                this.patterns;
            set
            {
                this.patterns = value;
            }
        }

        public int ReplacementCount =>
            this.replacementCnt;

        public int Threshold
        {
            get => 
                this.threshold;
            set
            {
                this.threshold = value;
            }
        }

        internal sealed class QilPatterns
        {
            private BitArray bits;

            private QilPatterns(QilPatternVisitor.QilPatterns toCopy)
            {
                this.bits = new BitArray(toCopy.bits);
            }

            public QilPatterns(int szBits, bool allSet)
            {
                this.bits = new BitArray(szBits, allSet);
            }

            public void Add(int i)
            {
                this.bits.Set(i, true);
            }

            public void ClearAll()
            {
                this.bits.SetAll(false);
            }

            public QilPatternVisitor.QilPatterns Clone() => 
                new QilPatternVisitor.QilPatterns(this);

            public bool IsSet(int i) => 
                this.bits[i];
        }
    }
}

