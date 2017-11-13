namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;
    using System.Collections;

    internal class ListInfoOverrideRenderer : RendererBase
    {
        private ListInfo listInfo;
        private static int listNumber = 1;
        private static Hashtable numberList = new Hashtable();

        public ListInfoOverrideRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.listInfo = domObj as ListInfo;
        }

        public static void Clear()
        {
            numberList.Clear();
            listNumber = 1;
        }

        internal static int GetListNumber(ListInfo li)
        {
            if (numberList.ContainsKey(li))
            {
                return (int) numberList[li];
            }
            return -1;
        }

        internal override void Render()
        {
            int listID = ListInfoRenderer.GetListID(this.listInfo);
            if (listID > -1)
            {
                base.rtfWriter.StartContent();
                base.rtfWriter.WriteControl("listoverride");
                base.rtfWriter.WriteControl("listid", listID);
                base.rtfWriter.WriteControl("listoverridecount", 0);
                base.rtfWriter.WriteControl("ls", listNumber);
                base.rtfWriter.EndContent();
                numberList.Add(this.listInfo, listNumber);
                listNumber++;
            }
        }
    }
}

