namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Xml;
    using System.Xml.Xsl.Qil;

    internal static class XmlILTrace
    {
        private static bool alreadyCheckedEnabled;
        private static string dirName;
        private const int MAX_REWRITES = 200;

        public static TextWriter GetTraceWriter(string fileName)
        {
            if (!IsEnabled)
            {
                return null;
            }
            return new StreamWriter(dirName + @"\" + fileName, true);
        }

        private static string OptimizationToString(int opt)
        {
            string name = Enum.GetName(typeof(XmlILOptimization), opt);
            if (name.StartsWith("Introduce", StringComparison.Ordinal))
            {
                return (name.Substring(9) + " introduction");
            }
            if (name.StartsWith("Eliminate", StringComparison.Ordinal))
            {
                return (name.Substring(9) + " elimination");
            }
            if (name.StartsWith("Commute", StringComparison.Ordinal))
            {
                return (name.Substring(7) + " commutation");
            }
            if (name.StartsWith("Fold", StringComparison.Ordinal))
            {
                return (name.Substring(4) + " folding");
            }
            if (name.StartsWith("Misc", StringComparison.Ordinal))
            {
                return name.Substring(4);
            }
            return name;
        }

        public static void PrepareTraceWriter(string fileName)
        {
            if (IsEnabled)
            {
                File.Delete(dirName + @"\" + fileName);
            }
        }

        public static void TraceOptimizations(QilExpression qil, string fileName)
        {
            if (IsEnabled)
            {
                XmlWriter w = XmlWriter.Create(dirName + @"\" + fileName);
                w.WriteStartDocument();
                w.WriteProcessingInstruction("xml-stylesheet", "href='qilo.xslt' type='text/xsl'");
                w.WriteStartElement("QilOptimizer");
                w.WriteAttributeString("timestamp", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                WriteQilRewrite(qil, w, null);
                try
                {
                    for (int i = 1; i < 200; i++)
                    {
                        QilExpression expression = (QilExpression) new QilCloneVisitor(qil.Factory).Clone(qil);
                        XmlILOptimizerVisitor visitor = new XmlILOptimizerVisitor(expression, !expression.IsDebug) {
                            Threshold = i
                        };
                        WriteQilRewrite(visitor.Optimize(), w, OptimizationToString(visitor.LastReplacement));
                        if (visitor.ReplacementCount < i)
                        {
                            return;
                        }
                    }
                }
                catch (Exception exception)
                {
                    if (!XmlException.IsCatchableException(exception))
                    {
                        throw;
                    }
                    w.WriteElementString("Exception", null, exception.ToString());
                    throw;
                }
                finally
                {
                    w.WriteEndElement();
                    w.WriteEndDocument();
                    w.Flush();
                    w.Close();
                }
            }
        }

        public static void WriteQil(QilExpression qil, string fileName)
        {
            if (IsEnabled)
            {
                XmlWriter w = XmlWriter.Create(dirName + @"\" + fileName);
                try
                {
                    WriteQil(qil, w);
                }
                finally
                {
                    w.Close();
                }
            }
        }

        private static void WriteQil(QilExpression qil, XmlWriter w)
        {
            new QilXmlWriter(w).ToXml(qil);
        }

        private static void WriteQilRewrite(QilExpression qil, XmlWriter w, string rewriteName)
        {
            w.WriteStartElement("Diff");
            if (rewriteName != null)
            {
                w.WriteAttributeString("rewrite", rewriteName);
            }
            WriteQil(qil, w);
            w.WriteEndElement();
        }

        public static bool IsEnabled
        {
            get
            {
                if (!alreadyCheckedEnabled)
                {
                    try
                    {
                        dirName = Environment.GetEnvironmentVariable("XmlILTrace");
                    }
                    catch (SecurityException)
                    {
                    }
                    alreadyCheckedEnabled = true;
                }
                return (dirName != null);
            }
        }
    }
}

