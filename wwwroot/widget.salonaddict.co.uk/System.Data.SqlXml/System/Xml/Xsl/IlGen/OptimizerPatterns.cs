namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Xml.Xsl.Qil;

    internal class OptimizerPatterns : IQilAnnotation
    {
        private object arg0;
        private object arg1;
        private object arg2;
        private static OptimizerPatterns DodDefault;
        private bool isReadOnly;
        private static OptimizerPatterns MaybeManyDefault;
        private static readonly int PatternCount = Enum.GetValues(typeof(OptimizerPatternName)).Length;
        private int patterns;
        private static OptimizerPatterns ZeroOrOneDefault;

        public void AddArgument(OptimizerPatternArgument argId, object arg)
        {
            switch (((int) argId))
            {
                case 0:
                    this.arg0 = arg;
                    return;

                case 1:
                    this.arg1 = arg;
                    return;

                case 2:
                    this.arg2 = arg;
                    return;
            }
        }

        public void AddPattern(OptimizerPatternName pattern)
        {
            this.patterns |= ((int) 1) << pattern;
        }

        public object GetArgument(OptimizerPatternArgument argNum)
        {
            switch (((int) argNum))
            {
                case 0:
                    return this.arg0;

                case 1:
                    return this.arg1;

                case 2:
                    return this.arg2;
            }
            return null;
        }

        public static void Inherit(QilNode ndSrc, QilNode ndDst, OptimizerPatternName pattern)
        {
            OptimizerPatterns patterns = Read(ndSrc);
            if (patterns.MatchesPattern(pattern))
            {
                OptimizerPatterns patterns2 = Write(ndDst);
                patterns2.AddPattern(pattern);
                switch (pattern)
                {
                    case OptimizerPatternName.DodReverse:
                    case OptimizerPatternName.JoinAndDod:
                        patterns2.AddArgument(OptimizerPatternArgument.ElementQName, patterns.GetArgument(OptimizerPatternArgument.ElementQName));
                        return;

                    case OptimizerPatternName.EqualityIndex:
                        patterns2.AddArgument(OptimizerPatternArgument.StepNode, patterns.GetArgument(OptimizerPatternArgument.StepNode));
                        patterns2.AddArgument(OptimizerPatternArgument.StepInput, patterns.GetArgument(OptimizerPatternArgument.StepInput));
                        return;

                    case OptimizerPatternName.FilterAttributeKind:
                    case OptimizerPatternName.IsDocOrderDistinct:
                    case OptimizerPatternName.IsPositional:
                    case OptimizerPatternName.SameDepth:
                        return;

                    case OptimizerPatternName.FilterContentKind:
                        patterns2.AddArgument(OptimizerPatternArgument.ElementQName, patterns.GetArgument(OptimizerPatternArgument.ElementQName));
                        return;

                    case OptimizerPatternName.FilterElements:
                        patterns2.AddArgument(OptimizerPatternArgument.ElementQName, patterns.GetArgument(OptimizerPatternArgument.ElementQName));
                        return;

                    case OptimizerPatternName.MaxPosition:
                        patterns2.AddArgument(OptimizerPatternArgument.ElementQName, patterns.GetArgument(OptimizerPatternArgument.ElementQName));
                        return;

                    case OptimizerPatternName.Step:
                        patterns2.AddArgument(OptimizerPatternArgument.StepNode, patterns.GetArgument(OptimizerPatternArgument.StepNode));
                        patterns2.AddArgument(OptimizerPatternArgument.StepInput, patterns.GetArgument(OptimizerPatternArgument.StepInput));
                        return;

                    case OptimizerPatternName.SingleTextRtf:
                        patterns2.AddArgument(OptimizerPatternArgument.ElementQName, patterns.GetArgument(OptimizerPatternArgument.ElementQName));
                        return;
                }
            }
        }

        public bool MatchesPattern(OptimizerPatternName pattern) => 
            ((this.patterns & (((int) 1) << pattern)) != 0);

        public static OptimizerPatterns Read(QilNode nd)
        {
            XmlILAnnotation annotation = nd.Annotation as XmlILAnnotation;
            OptimizerPatterns patterns = annotation?.Patterns;
            if (patterns != null)
            {
                return patterns;
            }
            if (!nd.XmlType.MaybeMany)
            {
                if (ZeroOrOneDefault == null)
                {
                    patterns = new OptimizerPatterns();
                    patterns.AddPattern(OptimizerPatternName.IsDocOrderDistinct);
                    patterns.AddPattern(OptimizerPatternName.SameDepth);
                    patterns.isReadOnly = true;
                    ZeroOrOneDefault = patterns;
                    return patterns;
                }
                return ZeroOrOneDefault;
            }
            if (nd.XmlType.IsDod)
            {
                if (DodDefault == null)
                {
                    patterns = new OptimizerPatterns();
                    patterns.AddPattern(OptimizerPatternName.IsDocOrderDistinct);
                    patterns.isReadOnly = true;
                    DodDefault = patterns;
                    return patterns;
                }
                return DodDefault;
            }
            if (MaybeManyDefault == null)
            {
                patterns = new OptimizerPatterns {
                    isReadOnly = true
                };
                MaybeManyDefault = patterns;
                return patterns;
            }
            return MaybeManyDefault;
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < PatternCount; i++)
            {
                if (this.MatchesPattern((OptimizerPatternName) i))
                {
                    if (str.Length != 0)
                    {
                        str = str + ", ";
                    }
                    str = str + ((OptimizerPatternName) i).ToString();
                }
            }
            return str;
        }

        public static OptimizerPatterns Write(QilNode nd)
        {
            XmlILAnnotation annotation = XmlILAnnotation.Write(nd);
            OptimizerPatterns patterns = annotation.Patterns;
            if ((patterns == null) || patterns.isReadOnly)
            {
                patterns = new OptimizerPatterns();
                annotation.Patterns = patterns;
                if (!nd.XmlType.MaybeMany)
                {
                    patterns.AddPattern(OptimizerPatternName.IsDocOrderDistinct);
                    patterns.AddPattern(OptimizerPatternName.SameDepth);
                    return patterns;
                }
                if (nd.XmlType.IsDod)
                {
                    patterns.AddPattern(OptimizerPatternName.IsDocOrderDistinct);
                }
            }
            return patterns;
        }

        public virtual string Name =>
            "Patterns";
    }
}

