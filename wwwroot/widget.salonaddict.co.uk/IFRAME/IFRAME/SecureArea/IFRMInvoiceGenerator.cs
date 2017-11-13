namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Tables;
    using SA.BAL;
    using System;
    using System.Collections.Generic;

    internal sealed class IFRMInvoiceGenerator
    {
        private string _accountname;
        private string _address1;
        private string _address2;
        private string _address3;
        private List<SalonInvoiceADDB> _adjustments;
        private DateTime _billend;
        private string _billphone;
        private DateTime _billstart;
        private readonly MigraDoc.DocumentObjectModel.Document _document = new MigraDoc.DocumentObjectModel.Document();
        private DateTime _invoicedate;
        private string _invoicenum;
        private bool _isvoid;
        private DateTime _paymentdate;
        private int _paymentreceived;
        private string _plan;
        private DateTime _planend;
        private int _planfee;
        private int _prevamountdue;
        private string _recipientattn;
        private int _subtotalexcltax;
        private decimal _taxrate;
        private int _totalAdjustment;
        private int _totalamountdue;
        private int _totalexcltax;
        private int _totalincltax;
        private int _totaloverdue;
        private int _totalplan;
        private int _totaltax;
        private int _totalwidget;
        private int _totalwidgetcount;
        private List<SalonInvoiceWTDB> _transactions;
        private string _vatnum;

        internal IFRMInvoiceGenerator(string accountname, string address1, string address2, string address3, DateTime billend, DateTime billstart, string billphone, List<SalonInvoiceWTDB> transactions, List<SalonInvoiceADDB> adjustments, DateTime invoicedate, string invoicenum, DateTime paymentdate, int paymentreceived, string plan, DateTime planend, int planfee, int prevamountdue, string recipientattn, int subtotalexcltax, decimal taxrate, int totalamountdue, int totalAdjustment, int totalexcltax, int totalincltax, int totaloverdue, int totalplan, int totaltax, int totalwidgetcount, int totalwidget, string vatnum, bool isvoid)
        {
            this._accountname = accountname;
            this._address1 = address1;
            this._address2 = address2;
            this._address3 = address3;
            this._billend = billend;
            this._billstart = billstart;
            this._billphone = billphone;
            this._transactions = transactions;
            this._adjustments = adjustments;
            this._invoicedate = invoicedate;
            this._invoicenum = invoicenum;
            this._paymentdate = paymentdate;
            this._paymentreceived = paymentreceived;
            this._plan = plan;
            this._planend = planend;
            this._planfee = planfee;
            this._prevamountdue = prevamountdue;
            this._recipientattn = recipientattn;
            this._subtotalexcltax = subtotalexcltax;
            this._taxrate = taxrate;
            this._totalamountdue = totalamountdue;
            this._totalAdjustment = totalAdjustment;
            this._totalexcltax = totalexcltax;
            this._totalincltax = totalincltax;
            this._totaloverdue = totaloverdue;
            this._totalplan = totalplan;
            this._totaltax = totaltax;
            this._totalwidgetcount = totalwidgetcount;
            this._totalwidget = totalwidget;
            this._vatnum = vatnum;
            this._isvoid = isvoid;
            this.Init();
        }

        public static MigraDoc.DocumentObjectModel.Document GenerateInvoice(string accountname, string address1, string address2, string address3, DateTime billend, DateTime billstart, string billphone, List<SalonInvoiceWTDB> transactions, List<SalonInvoiceADDB> credits, DateTime invoicedate, string invoicenum, DateTime paymentdate, int paymentreceived, string plan, DateTime planend, int planfee, int prevamountdue, string recipientattn, int subtotalexcltax, decimal taxrate, int totalamountdue, int totalcredit, int totalexcltax, int totalincltax, int totaloverdue, int totalplan, int totaltax, int totalwidgetcount, int totalwidget, string vatnum, bool isvoid)
        {
            IFRMInvoiceGenerator generator = new IFRMInvoiceGenerator(accountname, address1, address2, address3, billend, billstart, billphone, transactions, credits, invoicedate, invoicenum, paymentdate, paymentreceived, plan, planend, planfee, prevamountdue, recipientattn, subtotalexcltax, taxrate, totalamountdue, totalcredit, totalexcltax, totalincltax, totaloverdue, totalplan, totaltax, totalwidgetcount, totalwidget, vatnum, isvoid);
            generator.LoadCoverPage();
            generator.LoadChargesPage();
            generator.LoadItemizedBillPage();
            return generator.Document;
        }

        private void Init()
        {
            MigraDoc.DocumentObjectModel.Document document = this._document;
            document.Info.Title = string.Empty;
            document.Info.Subject = string.Empty;
            document.Info.Author = string.Empty;
            document.UseCmykColor = true;
            Style style = document.Styles["Normal"];
            style.Font.Name = "Arial";
            style = document.Styles["Header"];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);
            style = document.Styles["Footer"];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);
        }

        public void LoadChargesPage()
        {
            Section section = this._document.AddSection();
            this.LoadChargesPageHeader(ref section);
            this.LoadChargesPageServiceCharges(ref section);
            this.LoadChargesPageFooter(ref section);
        }

        private void LoadChargesPageFooter(ref Section section)
        {
            string str = (this._vatnum ?? string.Empty).ToUpper();
            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            paragraph.Format.Font.Size = 8;
            paragraph.Format.Font.Name = "Arial";
            paragraph.Format.Font.Bold = false;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText($"SalonAddict Ltd · Unit 3 Sandyford Office Park · Dublin 18, Ireland  · VAT Reg No. {str}");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddPageField();
        }

        private void LoadChargesPageHeader(ref Section section)
        {
            string paragraphText = this._accountname;
            string str2 = this._billphone;
            string str3 = this._invoicenum;
            DateTime time = this._invoicedate;
            paragraphText = (paragraphText ?? string.Empty).ToUpper();
            str2 = str2 ?? string.Empty;
            str3 = (str3 ?? string.Empty).ToUpper();
            TextFrame frame = section.Headers.Primary.AddTextFrame();
            frame.Height = "3.0cm";
            frame.Width = "7.0cm";
            frame.Left = 1;
            frame.RelativeHorizontal = RelativeHorizontal.Margin;
            frame.Top = "1.0cm";
            frame.RelativeVertical = RelativeVertical.Page;
            Table table = frame.AddTable();
            table.Format.Font.Name = "Arial";
            table.Format.Font.Size = 9;
            table.Format.Alignment = ParagraphAlignment.Right;
            table.AddColumn("3.5cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("3.5cm").Format.Alignment = ParagraphAlignment.Left;
            Row row = table.AddRow();
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Account Name");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph(paragraphText);
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            row = table.AddRow();
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Telephone Number");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph(str2);
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            frame = section.Headers.Primary.AddTextFrame();
            frame.Height = "3.0cm";
            frame.Width = "7.0cm";
            frame.Left = 2;
            frame.RelativeHorizontal = RelativeHorizontal.Margin;
            frame.Top = "1.0cm";
            frame.RelativeVertical = RelativeVertical.Page;
            table = frame.AddTable();
            table.Format.Font.Name = "Arial";
            table.Format.Font.Size = 9;
            table.Format.Alignment = ParagraphAlignment.Right;
            table.AddColumn("3.5cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("3.5cm").Format.Alignment = ParagraphAlignment.Left;
            row = table.AddRow();
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Invoice Number");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph(str3);
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            row = table.AddRow();
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Date of Invoice");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph(time.ToString("dd MMM yyyy"));
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
        }

        private void LoadChargesPageServiceCharges(ref Section section)
        {
            string str = this._plan;
            int num = this._planfee;
            int num2 = this._totalplan;
            int num3 = this._totalexcltax;
            int num4 = this._totalwidget;
            int num5 = this._totalwidgetcount;
            int num6 = this._totalAdjustment;
            DateTime time = this._invoicedate;
            DateTime time2 = this._planend;
            List<SalonInvoiceADDB> list = this._adjustments;
            str = str ?? string.Empty;
            Paragraph paragraph = section.AddParagraph();
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.Format.Font.Name = "Arial";
            paragraph.Format.Font.Size = 9.5;
            paragraph.AddText("SUMMARY OF MONTHLY CHARGES");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph = section.AddParagraph();
            paragraph.Format.Font.Name = "Arial";
            paragraph.Format.Font.Size = 9.5;
            paragraph.Format.Font.Bold = true;
            paragraph.AddText("Minimum Committment");
            Table table = section.AddTable();
            table.Format.Font.Name = "Arial";
            table.Format.Font.Size = 8.5;
            table.Format.Alignment = ParagraphAlignment.Left;
            table.Format.LeftIndent = 10;
            table.AddColumn("12.0cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("3.2cm").Format.Alignment = ParagraphAlignment.Left;
            Row row = table.AddRow();
            row.TopPadding = 5;
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            double num7 = ((double) num) / 100.0;
            row.Cells[0].AddParagraph($"{str} {num7.ToString("C")}");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            if (time <= time2)
            {
                row.Cells[1].AddParagraph("");
                row.Cells[1].Format.Font.Bold = false;
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
                row = table.AddRow();
                row.TopPadding = 2;
                row.BottomPadding = 2;
                row.HeadingFormat = false;
                row.Format.Alignment = ParagraphAlignment.Left;
                row.Format.Font.Bold = false;
                row.Cells[0].AddParagraph(time.ToString("dd MMM yyyy") + " - " + time2.ToString("dd MMM yyyy"));
                row.Cells[0].Format.Font.Bold = false;
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
                double num8 = ((double) num2) / 100.0;
                row.Cells[1].AddParagraph(num8.ToString("C"));
                row.Cells[1].Format.Font.Bold = true;
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            }
            else
            {
                double num9 = ((double) num2) / 100.0;
                row.Cells[1].AddParagraph(num9.ToString("C"));
                row.Cells[1].Format.Font.Bold = true;
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            }
            paragraph = section.AddParagraph();
            paragraph.Format.Font.Name = "Arial";
            paragraph.Format.Font.Size = 9.5;
            paragraph.Format.Font.Bold = true;
            paragraph.AddLineBreak();
            paragraph.AddText("Service Charges");
            table = section.AddTable();
            table.Format.Font.Name = "Arial";
            table.Format.Font.Size = 8.5;
            table.Format.Alignment = ParagraphAlignment.Left;
            table.Format.LeftIndent = 10;
            table.AddColumn("12.0cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("3.2cm").Format.Alignment = ParagraphAlignment.Left;
            row = table.AddRow();
            row.TopPadding = 5;
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph($"Widget ({num5})");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            double num10 = ((double) num4) / 100.0;
            row.Cells[1].AddParagraph(num10.ToString("C"));
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            row = table.AddRow();
            row.TopPadding = 2;
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Total Charges this Month");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            double num11 = ((double) num4) / 100.0;
            row.Cells[1].AddParagraph(num11.ToString("C"));
            row.Cells[1].Format.Font.Bold = true;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            paragraph = section.AddParagraph();
            paragraph.Format.Font.Name = "Arial";
            paragraph.Format.Font.Size = 9.5;
            paragraph.Format.Font.Bold = true;
            paragraph.AddLineBreak();
            paragraph.AddText("Other Credits and Charges");
            table = section.AddTable();
            table.Format.Font.Name = "Arial";
            table.Format.Font.Size = 8.5;
            table.Format.Alignment = ParagraphAlignment.Left;
            table.Format.LeftIndent = 10;
            table.AddColumn("12.0cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("3.2cm").Format.Alignment = ParagraphAlignment.Left;
            foreach (SalonInvoiceADDB eaddb in list)
            {
                row = table.AddRow();
                row.TopPadding = 2;
                row.BottomPadding = 2;
                row.HeadingFormat = false;
                row.Format.Alignment = ParagraphAlignment.Left;
                row.Format.Font.Bold = false;
                row.Cells[0].AddParagraph(eaddb.Description);
                row.Cells[0].Format.Font.Bold = false;
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
                double num12 = ((double) Math.Abs(eaddb.Amount)) / 100.0;
                row.Cells[1].AddParagraph(num12.ToString("C") + ((eaddb.Amount > 0) ? string.Empty : " CR"));
                row.Cells[1].Format.Font.Bold = false;
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            }
            row = table.AddRow();
            row.TopPadding = 2;
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Total Other Charges and Credits");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            double num13 = ((double) Math.Abs(num6)) / 100.0;
            row.Cells[1].AddParagraph(num13.ToString("C") + ((num6 > 0) ? string.Empty : " CR"));
            row.Cells[1].Format.Font.Bold = true;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            row = table.AddRow();
            row.TopPadding = 2;
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph(string.Empty);
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            row.Cells[1].AddParagraph(string.Empty);
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            row = table.AddRow();
            row.TopPadding = 2;
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Total Charges This Month (excl. VAT)");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            double num14 = ((double) num3) / 100.0;
            row.Cells[1].AddParagraph(num14.ToString("C"));
            row.Cells[1].Format.Font.Bold = true;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
        }

        private void LoadCoverAccountFrame(ref Section section)
        {
            string paragraphText = this._accountname;
            string str2 = this._invoicenum;
            DateTime time = this._billstart;
            DateTime time2 = this._billend;
            DateTime time3 = this._planend;
            DateTime time4 = this._invoicedate;
            paragraphText = (paragraphText ?? string.Empty).ToUpper();
            str2 = (str2 ?? string.Empty).ToUpper();
            TextFrame frame = section.AddTextFrame();
            frame.Height = "3.0cm";
            frame.Width = "6.0cm";
            frame.Left = 2;
            frame.RelativeHorizontal = RelativeHorizontal.Margin;
            frame.Top = "2.5cm";
            frame.RelativeVertical = RelativeVertical.Page;
            Table table = frame.AddTable();
            table.Format.Alignment = ParagraphAlignment.Right;
            table.Format.Font.Name = "Arial";
            table.Format.Font.Size = 8;
            table.AddColumn("2.5cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("3.2cm").Format.Alignment = ParagraphAlignment.Left;
            Row row = table.AddRow();
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Account Name");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph(paragraphText);
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            row = table.AddRow();
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Invoice Number");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph(str2);
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            row = table.AddRow();
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Date of Invoice");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph(time4.ToString("dd MMM yyyy"));
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            row = table.AddRow();
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph(string.Empty);
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph(string.Empty);
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            row = table.AddRow();
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Format.Font.Size = 6.5;
            row.Cells[0].AddParagraph("Charges");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph(time.ToString("dd MMM yyyy") + " - " + time2.ToString("dd MMM yyyy"));
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            if (time4 <= time3)
            {
                row = table.AddRow();
                row.BottomPadding = 2;
                row.HeadingFormat = false;
                row.Format.Alignment = ParagraphAlignment.Left;
                row.Format.Font.Bold = false;
                row.Format.Font.Size = 6.5;
                row.Cells[0].AddParagraph("Min Committment");
                row.Cells[0].Format.Font.Bold = false;
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
                row.Cells[1].AddParagraph(time4.ToString("dd MMM yyyy") + " - " + time3.ToString("dd MMM yyyy"));
                row.Cells[1].Format.Font.Bold = false;
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            }
        }

        private void LoadCoverBillingSummary(ref Section section)
        {
            int num = this._prevamountdue;
            int num2 = this._paymentreceived;
            int num3 = this._totalincltax;
            int num4 = this._totaloverdue;
            int num5 = this._totalamountdue;
            TextFrame frame = section.AddTextFrame();
            frame.Height = "3.0cm";
            frame.Width = "15.8cm";
            frame.Left = 1;
            frame.RelativeHorizontal = RelativeHorizontal.Margin;
            frame.Top = "8.5cm";
            frame.RelativeVertical = RelativeVertical.Page;
            Table table = frame.AddTable();
            table.Format.Font.Name = "Arial";
            table.Format.Font.Size = 8;
            table.Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("3.2cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("3.2cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("3.2cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("3.2cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("3.0cm").Format.Alignment = ParagraphAlignment.Left;
            Row row = table.AddRow();
            row.BottomPadding = 6;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Previous Balance (incl. VAT)");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            row.Cells[1].AddParagraph("Payments Received");
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            row.Cells[2].AddParagraph("Overdue Amount");
            row.Cells[2].Format.Font.Bold = false;
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[2].VerticalAlignment = VerticalAlignment.Top;
            row.Cells[3].AddParagraph("Total Charges this Month (incl. VAT)");
            row.Cells[3].Format.Font.Bold = true;
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[3].VerticalAlignment = VerticalAlignment.Top;
            row.Cells[4].AddParagraph("Total Amount Due (incl. VAT)");
            row.Cells[4].Format.Font.Bold = false;
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[4].VerticalAlignment = VerticalAlignment.Top;
            row = table.AddRow();
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            double num6 = ((double) num) / 100.0;
            row.Cells[0].AddParagraph(num6.ToString("C"));
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            double num7 = ((double) num2) / 100.0;
            row.Cells[1].AddParagraph(num7.ToString("C") + " CR");
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            double num8 = ((double) num4) / 100.0;
            row.Cells[2].AddParagraph(num8.ToString("C"));
            row.Cells[2].Format.Font.Bold = false;
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[2].VerticalAlignment = VerticalAlignment.Top;
            double num9 = ((double) num3) / 100.0;
            row.Cells[3].AddParagraph(num9.ToString("C"));
            row.Cells[3].Format.Font.Bold = false;
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[3].VerticalAlignment = VerticalAlignment.Top;
            double num10 = ((double) num5) / 100.0;
            row.Cells[4].AddParagraph(num10.ToString("C"));
            row.Cells[4].Format.Font.Bold = true;
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[4].VerticalAlignment = VerticalAlignment.Top;
        }

        private void LoadCoverChargesSummary(ref Section section)
        {
            int num = this._paymentreceived;
            int num2 = this._totaloverdue;
            int num3 = this._subtotalexcltax;
            decimal num4 = this._taxrate;
            int num5 = this._totaltax;
            int num6 = this._totalincltax;
            int num7 = this._totalamountdue;
            DateTime time = this._paymentdate;
            TextFrame frame = section.AddTextFrame();
            frame.Height = "7.0cm";
            frame.Width = "15.2cm";
            frame.Left = 1;
            frame.RelativeHorizontal = RelativeHorizontal.Margin;
            frame.Top = "12.0cm";
            frame.RelativeVertical = RelativeVertical.Page;
            Paragraph paragraph = frame.AddParagraph();
            paragraph.Format.Font.Name = "Arial";
            paragraph.Format.Font.Size = 9.5;
            paragraph.AddText("SUMMARY OF MONTHLY CHARGES");
            Table table = frame.AddTable();
            table.Format.Font.Name = "Arial";
            table.Format.Font.Size = 8.5;
            table.Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("12.0cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("3.2cm").Format.Alignment = ParagraphAlignment.Left;
            Row row = table.AddRow();
            row.TopPadding = 10;
            row.BottomPadding = 10;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Total payments this period - Thank You");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            double num8 = ((double) num) / 100.0;
            row.Cells[1].AddParagraph(num8.ToString("C") + " CR");
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            row = table.AddRow();
            row.BottomPadding = 10;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Overdue Amount (Due Immediately)");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            double num9 = ((double) num2) / 100.0;
            row.Cells[1].AddParagraph(num9.ToString("C"));
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            row = table.AddRow();
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Current Monthly Charges (excl. VAT)");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            double num10 = ((double) num3) / 100.0;
            row.Cells[1].AddParagraph(num10.ToString("C"));
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            row = table.AddRow();
            row.BottomPadding = 10;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph($"VAT @ {num4 * 100M:0.##}%");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            double num11 = ((double) num5) / 100.0;
            row.Cells[1].AddParagraph(num11.ToString("C"));
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            row = table.AddRow();
            row.BottomPadding = 10;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Total Current Amount");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            double num12 = ((double) num6) / 100.0;
            row.Cells[1].AddParagraph(num12.ToString("C"));
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            row = table.AddRow();
            row.BottomPadding = 10;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Total Amount Due (incl Overdue Amount)");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            double num13 = ((double) num7) / 100.0;
            row.Cells[1].AddParagraph(num13.ToString("C"));
            row.Cells[1].Format.Font.Bold = true;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            paragraph = frame.AddParagraph();
            paragraph.Format.Font.Size = 9;
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddText($"Current Amount Due by {time.ToString("dd MMMM yyyy")}");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph = frame.AddParagraph();
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Font.Size = 8;
            paragraph.AddText("Please Note: Non-payment may result in withdrawl of service without prior notice.");
        }

        private void LoadCoverFooter(ref Section section)
        {
            string str = (this._vatnum ?? string.Empty).ToUpper();
            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            paragraph.Format.Font.Size = 8;
            paragraph.Format.Font.Name = "Arial";
            paragraph.Format.Font.Bold = false;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText($"SalonAddict Ltd · Unit 3 Sandyford Office Park · Dublin 18, Ireland  · VAT Reg No. {str}");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddPageField();
        }

        public void LoadCoverPage()
        {
            Section section = this._document.AddSection();
            if (this._isvoid)
            {
                this.LoadCoverVoidFrame(ref section);
            }
            this.LoadCoverRecipientFrame(ref section);
            this.LoadCoverAccountFrame(ref section);
            this.LoadCoverBillingSummary(ref section);
            this.LoadCoverChargesSummary(ref section);
            this.LoadCoverFooter(ref section);
        }

        private void LoadCoverRecipientFrame(ref Section section)
        {
            string text = this._recipientattn;
            string str2 = this._address1;
            string str3 = this._address2;
            string str4 = this._address3;
            text = (text ?? string.Empty).ToUpper();
            str2 = (str2 ?? string.Empty).ToUpper();
            str3 = (str3 ?? string.Empty).ToUpper();
            str4 = (str4 ?? string.Empty).ToUpper();
            TextFrame frame = section.AddTextFrame();
            frame.Height = "3.0cm";
            frame.Width = "9.0cm";
            frame.Left = 1;
            frame.RelativeHorizontal = RelativeHorizontal.Margin;
            frame.Top = "5.0cm";
            frame.RelativeVertical = RelativeVertical.Page;
            Paragraph paragraph = frame.AddParagraph();
            paragraph.Format.Font.Name = "ArialBold";
            paragraph.Format.Font.Size = 10;
            paragraph.AddText(text);
            paragraph.AddLineBreak();
            paragraph.AddText(str2);
            paragraph.AddLineBreak();
            paragraph.AddText(str3);
            paragraph.AddLineBreak();
            paragraph.AddText(str4);
        }

        private void LoadCoverVoidFrame(ref Section section)
        {
            TextFrame frame = section.AddTextFrame();
            frame.Height = "0.5cm";
            frame.Width = "3.0cm";
            frame.Left = 2;
            frame.RelativeHorizontal = RelativeHorizontal.Margin;
            frame.Top = "0.5cm";
            frame.RelativeVertical = RelativeVertical.Page;
            Paragraph paragraph = frame.AddParagraph("VOID");
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Font.Color = Color.Parse("red");
            paragraph.Format.Font.Size = 0x11;
        }

        public void LoadItemizedBillPage()
        {
            List<SalonInvoiceWTDB> list = this._transactions;
            Table table = null;
            Row row = null;
            Section section = this._document.AddSection();
            this.LoadItemizedBillPageHeader(ref section);
            this.LoadItemizedBillPageHeader(ref section);
            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.Font.Name = "Arial";
            paragraph.Format.Font.Size = 9.5;
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddText("Itemized Bill");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            for (int i = 0; i < list.Count; i++)
            {
                if ((i % 0x20) == 0)
                {
                    if (i != 0)
                    {
                        section = this._document.AddSection();
                        this.LoadItemizedBillPageHeader(ref section);
                        this.LoadItemizedBillPageFooter(ref section);
                    }
                    table = section.AddTable();
                    table.Format.Font.Name = "Arial";
                    table.Format.Font.Size = 7;
                    table.Format.Alignment = ParagraphAlignment.Left;
                    table.AddColumn("2.5cm").Format.Alignment = ParagraphAlignment.Left;
                    table.AddColumn("2.8cm").Format.Alignment = ParagraphAlignment.Left;
                    table.AddColumn("4.2cm").Format.Alignment = ParagraphAlignment.Left;
                    table.AddColumn("2.0cm").Format.Alignment = ParagraphAlignment.Left;
                    table.AddColumn("1.0cm").Format.Alignment = ParagraphAlignment.Left;
                    table.AddColumn("1.2cm").Format.Alignment = ParagraphAlignment.Left;
                    row = table.AddRow();
                    row.TopPadding = 5;
                    row.BottomPadding = 2;
                    row.HeadingFormat = false;
                    row.Format.Alignment = ParagraphAlignment.Left;
                    row.Format.Font.Bold = false;
                    row.Cells[0].AddParagraph("Timestamp");
                    row.Cells[0].Format.Font.Bold = true;
                    row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                    row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
                    row.Cells[1].AddParagraph("Name");
                    row.Cells[1].Format.Font.Bold = true;
                    row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                    row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
                    row.Cells[2].AddParagraph("Service");
                    row.Cells[2].Format.Font.Bold = true;
                    row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                    row.Cells[2].VerticalAlignment = VerticalAlignment.Top;
                    row.Cells[3].AddParagraph("Date");
                    row.Cells[3].Format.Font.Bold = true;
                    row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
                    row.Cells[3].VerticalAlignment = VerticalAlignment.Top;
                    row.Cells[4].AddParagraph("Time");
                    row.Cells[4].Format.Font.Bold = true;
                    row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
                    row.Cells[4].VerticalAlignment = VerticalAlignment.Top;
                    row.Cells[5].AddParagraph("Amount");
                    row.Cells[5].Format.Font.Bold = true;
                    row.Cells[5].Format.Alignment = ParagraphAlignment.Left;
                    row.Cells[5].VerticalAlignment = VerticalAlignment.Top;
                }
                row = table.AddRow();
                row.TopPadding = 5;
                row.BottomPadding = 2;
                row.HeadingFormat = false;
                row.Format.Alignment = ParagraphAlignment.Left;
                row.Format.Font.Bold = false;
                row.Cells[0].AddParagraph(list[i].TimeStamp.ToString("dd.MM.yyyy HH:mm"));
                row.Cells[0].Format.Font.Bold = false;
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
                row.Cells[1].AddParagraph((list[i].FirstName + " " + list[i].LastName).Trim());
                row.Cells[1].Format.Font.Bold = false;
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
                row.Cells[2].AddParagraph(list[i].ServiceName);
                row.Cells[2].Format.Font.Bold = false;
                row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[2].VerticalAlignment = VerticalAlignment.Top;
                row.Cells[3].AddParagraph(IFRMHelper.FromUrlFriendlyDate(list[i].AppointmentDate).ToString("MMM dd yyyy"));
                row.Cells[3].Format.Font.Bold = false;
                row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[3].VerticalAlignment = VerticalAlignment.Top;
                row.Cells[4].AddParagraph(list[i].AppointmentTime);
                row.Cells[4].Format.Font.Bold = false;
                row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[4].VerticalAlignment = VerticalAlignment.Top;
                double num2 = ((double) list[i].ItemPrice) / 100.0;
                row.Cells[5].AddParagraph(num2.ToString("C"));
                row.Cells[5].Format.Font.Bold = false;
                row.Cells[5].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[5].VerticalAlignment = VerticalAlignment.Top;
            }
        }

        private void LoadItemizedBillPageFooter(ref Section section)
        {
            string str = (this._vatnum ?? string.Empty).ToUpper();
            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            paragraph.Format.Font.Size = 8;
            paragraph.Format.Font.Name = "Arial";
            paragraph.Format.Font.Bold = false;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText($"SalonAddict Ltd · Unit 3 Sandyford Office Park · Dublin 18, Ireland  · VAT Reg No. {str}");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddPageField();
        }

        private void LoadItemizedBillPageHeader(ref Section section)
        {
            string paragraphText = this._accountname;
            string str2 = this._billphone;
            string str3 = this._invoicenum;
            DateTime time = this._invoicedate;
            paragraphText = (paragraphText ?? string.Empty).ToUpper();
            str2 = str2 ?? string.Empty;
            str3 = (str3 ?? string.Empty).ToUpper();
            TextFrame frame = section.Headers.Primary.AddTextFrame();
            frame.Height = "3.0cm";
            frame.Width = "7.0cm";
            frame.Left = 1;
            frame.RelativeHorizontal = RelativeHorizontal.Margin;
            frame.Top = "1.0cm";
            frame.RelativeVertical = RelativeVertical.Page;
            Table table = frame.AddTable();
            table.Format.Font.Name = "Arial";
            table.Format.Font.Size = 9;
            table.Format.Alignment = ParagraphAlignment.Right;
            table.AddColumn("3.5cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("3.5cm").Format.Alignment = ParagraphAlignment.Left;
            Row row = table.AddRow();
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Account Name");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph(paragraphText);
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            row = table.AddRow();
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Telephone Number");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph(str2);
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            frame = section.Headers.Primary.AddTextFrame();
            frame.Height = "3.0cm";
            frame.Width = "7.0cm";
            frame.Left = 2;
            frame.RelativeHorizontal = RelativeHorizontal.Margin;
            frame.Top = "1.0cm";
            frame.RelativeVertical = RelativeVertical.Page;
            table = frame.AddTable();
            table.Format.Font.Name = "Arial";
            table.Format.Font.Size = 9;
            table.Format.Alignment = ParagraphAlignment.Right;
            table.AddColumn("3.5cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("3.5cm").Format.Alignment = ParagraphAlignment.Left;
            row = table.AddRow();
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Invoice Number");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph(str3);
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            row = table.AddRow();
            row.BottomPadding = 2;
            row.HeadingFormat = false;
            row.Format.Alignment = ParagraphAlignment.Left;
            row.Format.Font.Bold = false;
            row.Cells[0].AddParagraph("Date of Invoice");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph(time.ToString("dd MMM yyyy"));
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
        }

        public MigraDoc.DocumentObjectModel.Document Document =>
            this._document;
    }
}

