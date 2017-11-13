namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Diagnostics;
    using System.Xml.Utils;

    internal class QilValidationVisitor : QilScopedVisitor
    {
        private SubstitutionList subs = new SubstitutionList();
        private QilTypeChecker typeCheck = new QilTypeChecker();

        protected QilValidationVisitor()
        {
        }

        [Conditional("DEBUG")]
        internal static void SetError(QilNode n, string message)
        {
            message = Res.GetString("Qil_Validation", new object[] { message });
            string annotation = n.Annotation as string;
            if (annotation != null)
            {
                message = annotation + "\n" + message;
            }
            n.Annotation = message;
        }

        [Conditional("DEBUG")]
        public static void Validate(QilNode node)
        {
            new QilValidationVisitor().VisitAssumeReference(node);
        }
    }
}

