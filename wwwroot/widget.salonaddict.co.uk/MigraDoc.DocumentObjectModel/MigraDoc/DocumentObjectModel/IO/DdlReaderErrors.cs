namespace MigraDoc.DocumentObjectModel.IO
{
    using System;
    using System.Collections;
    using System.Reflection;

    public class DdlReaderErrors : IEnumerable
    {
        private ArrayList errors = new ArrayList();

        public void AddError(DdlReaderError error)
        {
            this.errors.Add(error);
        }

        public IEnumerator GetEnumerator() => 
            this.errors.GetEnumerator();

        public int ErrorCount
        {
            get
            {
                int num = 0;
                for (int i = 0; i < this.errors.Count; i++)
                {
                    if (((DdlReaderError) this.errors[i]).ErrorLevel == DdlErrorLevel.Error)
                    {
                        num++;
                    }
                }
                return num;
            }
        }

        public DdlReaderError this[int index] =>
            ((DdlReaderError) this.errors[index]);
    }
}

