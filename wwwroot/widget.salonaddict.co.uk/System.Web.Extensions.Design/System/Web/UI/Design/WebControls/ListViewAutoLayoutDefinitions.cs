namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;

    internal static class ListViewAutoLayoutDefinitions
    {
        private static Dictionary<string, object> BULLETEDLISTBLUE = new Dictionary<string, object>();
        private static Dictionary<string, object> BULLETEDLISTCOLORFUL = new Dictionary<string, object>();
        private static Dictionary<string, object> BULLETEDLISTPROFESSIONAL = new Dictionary<string, object>();
        private static Dictionary<string, object> FLOWBLUE = new Dictionary<string, object>();
        private static Dictionary<string, object> FLOWCOLORFUL = new Dictionary<string, object>();
        private static Dictionary<string, object> FLOWPROFESSIONAL = new Dictionary<string, object>();
        private static Dictionary<string, object> GRIDBLUE = new Dictionary<string, object>();
        private static Dictionary<string, object> GRIDCOLORFUL = new Dictionary<string, object>();
        private static Dictionary<string, object> GRIDPROFESSIONAL = new Dictionary<string, object>();
        private static Dictionary<string, object> SINGLEROWBLUE = new Dictionary<string, object>();
        private static Dictionary<string, object> SINGLEROWCOLORFUL = new Dictionary<string, object>();
        private static Dictionary<string, object> SINGLEROWPROFESSIONAL = new Dictionary<string, object>();
        private static Dictionary<string, object> TILEDBLUE = new Dictionary<string, object>();
        private static Dictionary<string, object> TILEDCOLORFUL = new Dictionary<string, object>();
        private static Dictionary<string, object> TILEDPROFESSIONAL = new Dictionary<string, object>();

        private static void FillBulletedListBlue()
        {
            if (BULLETEDLISTBLUE.Count == 0)
            {
                BULLETEDLISTBLUE.Add("listStyle", "font-family: Verdana, Arial, Helvetica, sans-serif;");
                BULLETEDLISTBLUE.Add("selectedItemLIStyle", "background-color: #E2DED6;font-weight: bold;color: #333333;");
                BULLETEDLISTBLUE.Add("itemLIStyle", "background-color: #E0FFFF;color: #333333;");
                BULLETEDLISTBLUE.Add("alternatingItemLIStyle", "background-color: #FFFFFF;color: #284775;");
                BULLETEDLISTBLUE.Add("editItemLIStyle", "background-color: #999999;");
                BULLETEDLISTBLUE.Add("insertItemLIStyle", "");
                BULLETEDLISTBLUE.Add("pagerDivStyle", "text-align: center;background-color: #5D7B9D;font-family: Verdana, Arial, Helvetica, sans-serif;color: #FFFFFF;");
            }
        }

        private static void FillBulletedListColorful()
        {
            if (BULLETEDLISTCOLORFUL.Count == 0)
            {
                BULLETEDLISTCOLORFUL.Add("listStyle", "font-family: Verdana, Arial, Helvetica, sans-serif;");
                BULLETEDLISTCOLORFUL.Add("selectedItemLIStyle", "background-color: #FFCC66;font-weight: bold;color: #000080;");
                BULLETEDLISTCOLORFUL.Add("itemLIStyle", "background-color: #FFFBD6;color: #333333;");
                BULLETEDLISTCOLORFUL.Add("alternatingItemLIStyle", "background-color: #FAFAD2;color: #284775;");
                BULLETEDLISTCOLORFUL.Add("editItemLIStyle", "background-color: #FFCC66;color: #000080;");
                BULLETEDLISTCOLORFUL.Add("insertItemLIStyle", "");
                BULLETEDLISTCOLORFUL.Add("pagerDivStyle", "text-align: center;background-color: #FFCC66;font-family: Verdana, Arial, Helvetica, sans-serif;color: #333333;");
            }
        }

        private static void FillBulletedListProfessional()
        {
            if (BULLETEDLISTPROFESSIONAL.Count == 0)
            {
                BULLETEDLISTPROFESSIONAL.Add("listStyle", "font-family: Verdana, Arial, Helvetica, sans-serif;");
                BULLETEDLISTPROFESSIONAL.Add("selectedItemLIStyle", "background-color: #008A8C;font-weight: bold;color: #FFFFFF;");
                BULLETEDLISTPROFESSIONAL.Add("itemLIStyle", "background-color: #DCDCDC;color: #000000;");
                BULLETEDLISTPROFESSIONAL.Add("alternatingItemLIStyle", "background-color: #FFF8DC;");
                BULLETEDLISTPROFESSIONAL.Add("editItemLIStyle", "background-color: #008A8C;color: #FFFFFF;");
                BULLETEDLISTPROFESSIONAL.Add("insertItemLIStyle", "");
                BULLETEDLISTPROFESSIONAL.Add("pagerDivStyle", "text-align: center;background-color: #CCCCCC;font-family: Verdana, Arial, Helvetica, sans-serif;color: #000000;");
            }
        }

        private static void FillFlowBlue()
        {
            if (FLOWBLUE.Count == 0)
            {
                FLOWBLUE.Add("divStyle", "font-family: Verdana, Arial, Helvetica, sans-serif;");
                FLOWBLUE.Add("selectedItemSpanStyle", "background-color: #E2DED6;font-weight: bold;color: #333333;");
                FLOWBLUE.Add("alternatingItemSpanStyle", "background-color: #FFFFFF;color: #284775;");
                FLOWBLUE.Add("editItemSpanStyle", "background-color: #999999;");
                FLOWBLUE.Add("insertItemSpanStyle", "");
                FLOWBLUE.Add("itemSpanStyle", "background-color: #E0FFFF;color: #333333;");
                FLOWBLUE.Add("pagerStyle", "text-align: center;background-color: #5D7B9D;font-family: Verdana, Arial, Helvetica, sans-serif;color: #FFFFFF;");
                FLOWBLUE.Add("pagerDivStyle", "text-align: center;background-color: #5D7B9D;font-family: Verdana, Arial, Helvetica, sans-serif;color: #FFFFFF;");
            }
        }

        private static void FillFlowColorful()
        {
            if (FLOWCOLORFUL.Count == 0)
            {
                FLOWCOLORFUL.Add("divStyle", "font-family: Verdana, Arial, Helvetica, sans-serif;");
                FLOWCOLORFUL.Add("selectedItemSpanStyle", "background-color: #FFCC66;font-weight: bold;color: #000080;");
                FLOWCOLORFUL.Add("alternatingItemSpanStyle", "background-color: #FAFAD2;color: #284775;");
                FLOWCOLORFUL.Add("editItemSpanStyle", "background-color: #FFCC66;color: #000080;");
                FLOWCOLORFUL.Add("insertItemSpanStyle", "");
                FLOWCOLORFUL.Add("itemSpanStyle", "background-color: #FFFBD6;color: #333333;");
                FLOWCOLORFUL.Add("pagerStyle", "text-align: center;background-color: #FFCC66;font-family: Verdana, Arial, Helvetica, sans-serif;color: #333333;");
                FLOWCOLORFUL.Add("pagerDivStyle", "text-align: center;background-color: #FFCC66;font-family: Verdana, Arial, Helvetica, sans-serif;color: #333333;");
            }
        }

        private static void FillFlowProfessional()
        {
            if (FLOWPROFESSIONAL.Count == 0)
            {
                FLOWPROFESSIONAL.Add("divStyle", "font-family: Verdana, Arial, Helvetica, sans-serif;");
                FLOWPROFESSIONAL.Add("selectedItemSpanStyle", "background-color: #008A8C;font-weight: bold;color: #FFFFFF;");
                FLOWPROFESSIONAL.Add("alternatingItemSpanStyle", "background-color: #FFF8DC;");
                FLOWPROFESSIONAL.Add("editItemSpanStyle", "background-color: #008A8C;color: #FFFFFF;");
                FLOWPROFESSIONAL.Add("insertItemSpanStyle", "");
                FLOWPROFESSIONAL.Add("itemSpanStyle", "background-color: #DCDCDC;color: #000000;");
                FLOWPROFESSIONAL.Add("pagerStyle", "text-align: center;background-color: #CCCCCC;font-family: Verdana, Arial, Helvetica, sans-serif;color: #000000;");
                FLOWPROFESSIONAL.Add("pagerDivStyle", "text-align: center;background-color: #CCCCCC;font-family: Verdana, Arial, Helvetica, sans-serif;color: #000000;");
            }
        }

        private static void FillGridBlue()
        {
            if (GRIDBLUE.Count == 0)
            {
                GRIDBLUE.Add("tableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;font-family: Verdana, Arial, Helvetica, sans-serif;");
                GRIDBLUE.Add("tableBorder", 1);
                GRIDBLUE.Add("pagerCellStyle", "text-align: center;background-color: #5D7B9D;font-family: Verdana, Arial, Helvetica, sans-serif;color: #FFFFFF");
                GRIDBLUE.Add("emptyDataTableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;");
                GRIDBLUE.Add("itemRowStyle", "background-color: #E0FFFF;color: #333333;");
                GRIDBLUE.Add("editItemRowStyle", "background-color: #999999;");
                GRIDBLUE.Add("selectedItemRowStyle", "background-color: #E2DED6;font-weight: bold;color: #333333;");
                GRIDBLUE.Add("alternatingItemRowStyle", "background-color: #FFFFFF;color: #284775;");
                GRIDBLUE.Add("insertItemRowStyle", "");
            }
        }

        private static void FillGridColorful()
        {
            if (GRIDCOLORFUL.Count == 0)
            {
                GRIDCOLORFUL.Add("tableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;font-family: Verdana, Arial, Helvetica, sans-serif;");
                GRIDCOLORFUL.Add("tableBorder", 1);
                GRIDCOLORFUL.Add("pagerCellStyle", "text-align: center;background-color: #FFCC66;font-family: Verdana, Arial, Helvetica, sans-serif;color: #333333;");
                GRIDCOLORFUL.Add("emptyDataTableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;");
                GRIDCOLORFUL.Add("itemRowStyle", "background-color: #FFFBD6;color: #333333;");
                GRIDCOLORFUL.Add("editItemRowStyle", "background-color: #FFCC66;color: #000080;");
                GRIDCOLORFUL.Add("selectedItemRowStyle", "background-color: #FFCC66;font-weight: bold;color: #000080;");
                GRIDCOLORFUL.Add("alternatingItemRowStyle", "background-color: #FAFAD2;color: #284775;");
                GRIDCOLORFUL.Add("insertItemRowStyle", "");
            }
        }

        private static void FillGridProfessional()
        {
            if (GRIDPROFESSIONAL.Count == 0)
            {
                GRIDPROFESSIONAL.Add("tableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;font-family: Verdana, Arial, Helvetica, sans-serif;");
                GRIDPROFESSIONAL.Add("tableBorder", 1);
                GRIDPROFESSIONAL.Add("pagerCellStyle", "text-align: center;background-color: #CCCCCC;font-family: Verdana, Arial, Helvetica, sans-serif;color: #000000;");
                GRIDPROFESSIONAL.Add("emptyDataTableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;");
                GRIDPROFESSIONAL.Add("itemRowStyle", "background-color:#DCDCDC;color: #000000;");
                GRIDPROFESSIONAL.Add("editItemRowStyle", "background-color:#008A8C;color: #FFFFFF;");
                GRIDPROFESSIONAL.Add("selectedItemRowStyle", "background-color:#008A8C;font-weight: bold;color: #FFFFFF;");
                GRIDPROFESSIONAL.Add("alternatingItemRowStyle", "background-color:#FFF8DC;");
                GRIDPROFESSIONAL.Add("insertItemRowStyle", "");
            }
        }

        private static void FillSingleRowBlue()
        {
            if (SINGLEROWBLUE.Count == 0)
            {
                SINGLEROWBLUE.Add("tableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;font-family: Verdana, Arial, Helvetica, sans-serif;");
                SINGLEROWBLUE.Add("tableBorder", 1);
                SINGLEROWBLUE.Add("pagerStyle", "text-align: center;background-color: #5D7B9D;font-family: Verdana, Arial, Helvetica, sans-serif;color: #FFFFFF");
                SINGLEROWBLUE.Add("emptyDataTableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;");
                SINGLEROWBLUE.Add("itemCellStyle", "background-color: #E0FFFF;color: #333333;");
                SINGLEROWBLUE.Add("editItemCellStyle", "background-color: #999999;");
                SINGLEROWBLUE.Add("selectedItemCellStyle", "background-color: #E2DED6;font-weight: bold;color: #333333;");
                SINGLEROWBLUE.Add("alternatingItemCellStyle", "background-color: #FFFFFF;color: #284775;");
                SINGLEROWBLUE.Add("insertItemCellStyle", "");
                SINGLEROWBLUE.Add("pagerDivStyle", "text-align: center;background-color: #5D7B9D;font-family: Verdana, Arial, Helvetica, sans-serif;color: #FFFFFF");
            }
        }

        private static void FillSingleRowColorful()
        {
            if (SINGLEROWCOLORFUL.Count == 0)
            {
                SINGLEROWCOLORFUL.Add("tableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;font-family: Verdana, Arial, Helvetica, sans-serif;");
                SINGLEROWCOLORFUL.Add("tableBorder", 1);
                SINGLEROWCOLORFUL.Add("pagerStyle", "text-align: center;background-color: #FFCC66;font-family: Verdana, Arial, Helvetica, sans-serif;color: #333333;");
                SINGLEROWCOLORFUL.Add("emptyDataTableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;");
                SINGLEROWCOLORFUL.Add("itemCellStyle", "background-color: #FFFBD6;color: #333333;");
                SINGLEROWCOLORFUL.Add("editItemCellStyle", "background-color: #FFCC66;color: #000080;");
                SINGLEROWCOLORFUL.Add("selectedItemCellStyle", "background-color: #FFCC66;font-weight: bold;color: #000080;");
                SINGLEROWCOLORFUL.Add("alternatingItemCellStyle", "background-color: #FAFAD2;color: #284775;");
                SINGLEROWCOLORFUL.Add("insertItemCellStyle", "");
                SINGLEROWCOLORFUL.Add("pagerDivStyle", "text-align: center;background-color: #FFCC66;font-family: Verdana, Arial, Helvetica, sans-serif;color: #333333;");
            }
        }

        private static void FillSingleRowProfessional()
        {
            if (SINGLEROWPROFESSIONAL.Count == 0)
            {
                SINGLEROWPROFESSIONAL.Add("tableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;font-family: Verdana, Arial, Helvetica, sans-serif;");
                SINGLEROWPROFESSIONAL.Add("tableBorder", 1);
                SINGLEROWPROFESSIONAL.Add("pagerStyle", "text-align: center;background-color: #CCCCCC;font-family: Verdana, Arial, Helvetica, sans-serif;color: #000000;");
                SINGLEROWPROFESSIONAL.Add("emptyDataTableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;font-family: Verdana, Arial, Helvetica, sans-serif;");
                SINGLEROWPROFESSIONAL.Add("itemCellStyle", "background-color:#DCDCDC;color: #000000;");
                SINGLEROWPROFESSIONAL.Add("editItemCellStyle", "background-color:#008A8C;color: #FFFFFF;");
                SINGLEROWPROFESSIONAL.Add("selectedItemCellStyle", "background-color:#008A8C;font-weight: bold;color: #FFFFFF;");
                SINGLEROWPROFESSIONAL.Add("alternatingItemCellStyle", "background-color:#FFF8DC;");
                SINGLEROWPROFESSIONAL.Add("insertItemCellStyle", "");
                SINGLEROWPROFESSIONAL.Add("pagerDivStyle", "text-align: center;background-color: #CCCCCC;font-family: Verdana, Arial, Helvetica, sans-serif;color: #000000;");
            }
        }

        private static void FillTiledBlue()
        {
            if (TILEDBLUE.Count == 0)
            {
                TILEDBLUE.Add("tableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;font-family: Verdana, Arial, Helvetica, sans-serif;");
                TILEDBLUE.Add("tableBorder", 1);
                TILEDBLUE.Add("pagerCellStyle", "text-align: center;background-color: #5D7B9D;font-family: Verdana, Arial, Helvetica, sans-serif;color: #FFFFFF");
                TILEDBLUE.Add("emptyTableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;");
                TILEDBLUE.Add("itemCellStyle", "background-color: #E0FFFF;color: #333333;");
                TILEDBLUE.Add("editItemCellStyle", "background-color: #999999;");
                TILEDBLUE.Add("selectedItemCellStyle", "background-color: #E2DED6;font-weight: bold;color: #333333;");
                TILEDBLUE.Add("alternatingItemCellStyle", "background-color: #FFFFFF;color: #284775;");
                TILEDBLUE.Add("insertItemCellStyle", "");
            }
        }

        private static void FillTiledColorful()
        {
            if (TILEDCOLORFUL.Count == 0)
            {
                TILEDCOLORFUL.Add("tableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;font-family: Verdana, Arial, Helvetica, sans-serif;");
                TILEDCOLORFUL.Add("tableBorder", 1);
                TILEDCOLORFUL.Add("pagerCellStyle", "text-align: center;background-color: #FFCC66;font-family: Verdana, Arial, Helvetica, sans-serif;color: #333333;");
                TILEDCOLORFUL.Add("emptyTableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;");
                TILEDCOLORFUL.Add("itemCellStyle", "background-color: #FFFBD6;color: #333333;");
                TILEDCOLORFUL.Add("editItemCellStyle", "background-color: #FFCC66;color: #000080;");
                TILEDCOLORFUL.Add("selectedItemCellStyle", "background-color: #FFCC66;font-weight: bold;color: #000080;");
                TILEDCOLORFUL.Add("alternatingItemCellStyle", "background-color: #FAFAD2;color: #284775;");
                TILEDCOLORFUL.Add("insertItemCellStyle", "");
            }
        }

        private static void FillTiledProfessional()
        {
            if (TILEDPROFESSIONAL.Count == 0)
            {
                TILEDPROFESSIONAL.Add("tableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;font-family: Verdana, Arial, Helvetica, sans-serif;");
                TILEDPROFESSIONAL.Add("tableBorder", 1);
                TILEDPROFESSIONAL.Add("pagerCellStyle", "text-align: center;background-color: #CCCCCC;font-family: Verdana, Arial, Helvetica, sans-serif;color: #000000;");
                TILEDPROFESSIONAL.Add("emptyTableStyle", "background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;");
                TILEDPROFESSIONAL.Add("itemCellStyle", "background-color:#DCDCDC;color: #000000;");
                TILEDPROFESSIONAL.Add("editItemCellStyle", "background-color:#008A8C;color: #FFFFFF;");
                TILEDPROFESSIONAL.Add("selectedItemCellStyle", "background-color:#008A8C;font-weight: bold;color: #FFFFFF;");
                TILEDPROFESSIONAL.Add("alternatingItemCellStyle", "background-color:#FFF8DC;");
                TILEDPROFESSIONAL.Add("insertItemCellStyle", "");
            }
        }

        public static object GetValue(string autoLayoutName, string key)
        {
            Dictionary<string, object> gRIDCOLORFUL = null;
            switch (autoLayoutName)
            {
                case "grid_colorful":
                    FillGridColorful();
                    gRIDCOLORFUL = GRIDCOLORFUL;
                    break;

                case "grid_professional":
                    FillGridProfessional();
                    gRIDCOLORFUL = GRIDPROFESSIONAL;
                    break;

                case "grid_blue":
                    FillGridBlue();
                    gRIDCOLORFUL = GRIDBLUE;
                    break;

                case "tiled_colorful":
                    FillTiledColorful();
                    gRIDCOLORFUL = TILEDCOLORFUL;
                    break;

                case "tiled_professional":
                    FillTiledProfessional();
                    gRIDCOLORFUL = TILEDPROFESSIONAL;
                    break;

                case "tiled_blue":
                    FillTiledBlue();
                    gRIDCOLORFUL = TILEDBLUE;
                    break;

                case "singlerow_colorful":
                    FillSingleRowColorful();
                    gRIDCOLORFUL = SINGLEROWCOLORFUL;
                    break;

                case "singlerow_professional":
                    FillSingleRowProfessional();
                    gRIDCOLORFUL = SINGLEROWPROFESSIONAL;
                    break;

                case "singlerow_blue":
                    FillSingleRowBlue();
                    gRIDCOLORFUL = SINGLEROWBLUE;
                    break;

                case "bulletedlist_colorful":
                    FillBulletedListColorful();
                    gRIDCOLORFUL = BULLETEDLISTCOLORFUL;
                    break;

                case "bulletedlist_professional":
                    FillBulletedListProfessional();
                    gRIDCOLORFUL = BULLETEDLISTPROFESSIONAL;
                    break;

                case "bulletedlist_blue":
                    FillBulletedListBlue();
                    gRIDCOLORFUL = BULLETEDLISTBLUE;
                    break;

                case "flow_colorful":
                    FillFlowColorful();
                    gRIDCOLORFUL = FLOWCOLORFUL;
                    break;

                case "flow_professional":
                    FillFlowProfessional();
                    gRIDCOLORFUL = FLOWPROFESSIONAL;
                    break;

                case "flow_blue":
                    FillFlowBlue();
                    gRIDCOLORFUL = FLOWBLUE;
                    break;
            }
            if (gRIDCOLORFUL != null)
            {
                return gRIDCOLORFUL[key];
            }
            return null;
        }
    }
}

