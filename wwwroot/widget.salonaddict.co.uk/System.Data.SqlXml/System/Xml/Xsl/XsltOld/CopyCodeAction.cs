namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;

    internal class CopyCodeAction : Action
    {
        private ArrayList copyEvents = new ArrayList();
        private const int Outputting = 2;

        internal CopyCodeAction()
        {
        }

        internal void AddEvent(Event copyEvent)
        {
            this.copyEvents.Add(copyEvent);
        }

        internal void AddEvents(ArrayList copyEvents)
        {
            this.copyEvents.AddRange(copyEvents);
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                    frame.Counter = 0;
                    frame.State = 2;
                    break;

                case 1:
                    return;

                case 2:
                    break;

                default:
                    return;
            }
            while (processor.CanContinue)
            {
                Event event2 = (Event) this.copyEvents[frame.Counter];
                if (!event2.Output(processor, frame))
                {
                    return;
                }
                if (frame.IncrementCounter() >= this.copyEvents.Count)
                {
                    frame.Finished();
                    return;
                }
            }
        }

        internal override DbgData GetDbgData(ActionFrame frame) => 
            ((Event) this.copyEvents[frame.Counter]).DbgData;

        internal override void ReplaceNamespaceAlias(Compiler compiler)
        {
            int count = this.copyEvents.Count;
            for (int i = 0; i < count; i++)
            {
                ((Event) this.copyEvents[i]).ReplaceNamespaceAlias(compiler);
            }
        }
    }
}

