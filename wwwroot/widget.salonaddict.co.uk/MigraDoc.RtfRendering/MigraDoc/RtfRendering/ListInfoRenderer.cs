namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;

    internal class ListInfoRenderer : RendererBase
    {
        private static Hashtable idList = new Hashtable();
        private static int listID = 1;
        private ListInfo listInfo;
        private static KeyValuePair<ListInfo, int> prevListInfoID = new KeyValuePair<ListInfo, int>();
        private static int templateID = 2;

        public ListInfoRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.listInfo = domObj as ListInfo;
        }

        public static void Clear()
        {
            idList.Clear();
            listID = 1;
            templateID = 2;
        }

        internal static int GetListID(ListInfo li)
        {
            if (idList.ContainsKey(li))
            {
                return (int) idList[li];
            }
            return -1;
        }

        internal override void Render()
        {
            if ((prevListInfoID.Key != null) && this.listInfo.ContinuePreviousList)
            {
                idList.Add(this.listInfo, prevListInfoID.Value);
            }
            else
            {
                idList.Add(this.listInfo, listID);
                base.rtfWriter.StartContent();
                base.rtfWriter.WriteControl("list");
                base.rtfWriter.WriteControl("listsimple", 1);
                this.WriteListLevel();
                base.rtfWriter.WriteControl("listrestarthdn", 0);
                base.rtfWriter.WriteControl("listid", listID.ToString(CultureInfo.InvariantCulture));
                base.rtfWriter.EndContent();
                prevListInfoID = new KeyValuePair<ListInfo, int>(this.listInfo, listID);
                listID += 2;
                templateID += 2;
            }
        }

        private void WriteListLevel()
        {
            ListType listType = this.listInfo.ListType;
            string str = "";
            string str2 = "";
            string levelNumbers = "";
            int fontIdx = -1;
            switch (listType)
            {
                case ListType.BulletList1:
                    str = "'01";
                    str2 = "u-3913 ?";
                    fontIdx = base.docRenderer.GetFontIndex("Symbol");
                    break;

                case ListType.BulletList2:
                    str = "'01o";
                    str2 = "";
                    fontIdx = base.docRenderer.GetFontIndex("Courier New");
                    break;

                case ListType.BulletList3:
                    str = "'01";
                    str2 = "u-3929 ?";
                    fontIdx = base.docRenderer.GetFontIndex("Wingdings");
                    break;

                case ListType.NumberList1:
                    str = "'02";
                    str2 = "'00.";
                    levelNumbers = "'01";
                    break;

                case ListType.NumberList2:
                case ListType.NumberList3:
                    str = "'02";
                    str2 = "'00)";
                    levelNumbers = "'01";
                    break;
            }
            this.WriteListLevel(str, str2, levelNumbers, fontIdx);
        }

        private void WriteListLevel(string levelText1, string levelText2, string levelNumbers, int fontIdx)
        {
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("listlevel");
            base.Translate("ListType", "levelnfcn", RtfUnit.Undefined, "4", false);
            base.Translate("ListType", "levelnfc", RtfUnit.Undefined, "4", false);
            base.rtfWriter.WriteControl("leveljcn", 0);
            base.rtfWriter.WriteControl("levelstartat", 1);
            base.rtfWriter.WriteControl("levelold", 0);
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("leveltext");
            base.rtfWriter.WriteControl("leveltemplateid", templateID);
            base.rtfWriter.WriteControl(levelText1);
            if (levelText2 != "")
            {
                base.rtfWriter.WriteControl(levelText2);
            }
            base.rtfWriter.WriteSeparator();
            base.rtfWriter.EndContent();
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("levelnumbers");
            if (levelNumbers != "")
            {
                base.rtfWriter.WriteControl(levelNumbers);
            }
            base.rtfWriter.WriteSeparator();
            base.rtfWriter.EndContent();
            if (fontIdx >= 0)
            {
                base.rtfWriter.WriteControl("f", fontIdx);
            }
            base.rtfWriter.WriteControl("levelfollow", 0);
            base.rtfWriter.EndContent();
        }
    }
}

