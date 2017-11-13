namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;
    using System.Text;

    internal sealed class Avt
    {
        private string constAvt;
        private TextEvent[] events;

        private Avt(ArrayList eventList)
        {
            this.events = new TextEvent[eventList.Count];
            for (int i = 0; i < eventList.Count; i++)
            {
                this.events[i] = (TextEvent) eventList[i];
            }
        }

        private Avt(string constAvt)
        {
            this.constAvt = constAvt;
        }

        internal static Avt CompileAvt(Compiler compiler, string avtText)
        {
            bool flag;
            ArrayList eventList = compiler.CompileAvt(avtText, out flag);
            if (!flag)
            {
                return new Avt(eventList);
            }
            return new Avt(avtText);
        }

        internal string Evaluate(Processor processor, ActionFrame frame)
        {
            if (this.IsConstant)
            {
                return this.constAvt;
            }
            StringBuilder sharedStringBuilder = processor.GetSharedStringBuilder();
            for (int i = 0; i < this.events.Length; i++)
            {
                sharedStringBuilder.Append(this.events[i].Evaluate(processor, frame));
            }
            processor.ReleaseSharedStringBuilder();
            return sharedStringBuilder.ToString();
        }

        public bool IsConstant =>
            (this.events == null);
    }
}

