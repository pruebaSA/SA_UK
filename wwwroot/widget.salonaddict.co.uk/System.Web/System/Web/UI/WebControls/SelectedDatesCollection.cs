namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class SelectedDatesCollection : ICollection, IEnumerable
    {
        private ArrayList dateList;

        public SelectedDatesCollection(ArrayList dateList)
        {
            this.dateList = dateList;
        }

        public void Add(DateTime date)
        {
            int num;
            if (!this.FindIndex(date.Date, out num))
            {
                this.dateList.Insert(num, date.Date);
            }
        }

        public void Clear()
        {
            this.dateList.Clear();
        }

        public bool Contains(DateTime date)
        {
            int num;
            return this.FindIndex(date.Date, out num);
        }

        public void CopyTo(Array array, int index)
        {
            IEnumerator enumerator = this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                array.SetValue(enumerator.Current, index++);
            }
        }

        private bool FindIndex(DateTime date, out int index)
        {
            int count = this.Count;
            int num2 = 0;
            int num3 = count;
            while (num2 < num3)
            {
                index = (num2 + num3) / 2;
                if (date == this[index])
                {
                    return true;
                }
                if (date < this[index])
                {
                    num3 = index;
                }
                else
                {
                    num2 = index + 1;
                }
            }
            index = num2;
            return false;
        }

        public IEnumerator GetEnumerator() => 
            this.dateList.GetEnumerator();

        public void Remove(DateTime date)
        {
            int num;
            if (this.FindIndex(date.Date, out num))
            {
                this.dateList.RemoveAt(num);
            }
        }

        public void SelectRange(DateTime fromDate, DateTime toDate)
        {
            this.dateList.Clear();
            if (fromDate <= toDate)
            {
                this.dateList.Add(fromDate);
                DateTime time = fromDate;
                while (time < toDate)
                {
                    time = time.AddDays(1.0);
                    this.dateList.Add(time);
                }
            }
        }

        public int Count =>
            this.dateList.Count;

        public bool IsReadOnly =>
            false;

        public bool IsSynchronized =>
            false;

        public DateTime this[int index] =>
            ((DateTime) this.dateList[index]);

        public object SyncRoot =>
            this;
    }
}

