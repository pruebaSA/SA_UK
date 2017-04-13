<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ServiceAverageReport.aspx.cs" Inherits="SalonAddict.Administration.ServiceAverageReportPage" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Import Namespace="SalonAddict.BusinessAccess.Implementation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-system.png" alt="System" />
        System Log
    </div>
    <div class="options">
        <asp:Button ID="btnExportXML" runat="server" Text="Export to XML" OnClick="btnExportXML_Click" ValidationGroup="ExportXML" ToolTip="Export report to a xml file" />
    </div>
</div>
<br />
<asp:GridView 
    ID="gv" 
    runat="server" 
    Width="100%"
    AutoGenerateColumns="false" >
    <Columns>
       <asp:TemplateField HeaderText="Service" >
          <ItemTemplate>
             <%# Eval("ServiceType") + " >> " + Eval("ServiceCategory") %>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Today" >
          <ItemTemplate>
             <%# Eval("CountTodayOrders") %>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Today Total" >
          <ItemTemplate>
             <%# CurrencyManager.FormatPrice(Convert.ToDecimal(Eval("SumTodayOrders"))) %>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="This Week" >
          <ItemTemplate>
             <%# Eval("CountThisWeekOrders") %>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="This Week Total" >
          <ItemTemplate>
             <%# CurrencyManager.FormatPrice(Convert.ToDecimal(Eval("SumThisWeekOrders"))) %>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="This Month" >
          <ItemTemplate>
             <%# Eval("CountThisMonthOrders")%>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="This Month Total" >
          <ItemTemplate>
             <%# CurrencyManager.FormatPrice(Convert.ToDecimal(Eval("SumThisMonthOrders"))) %>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="This Year" >
          <ItemTemplate>
             <%# Eval("CountThisYearOrders")%>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="This Year Total" >
          <ItemTemplate>
             <%# CurrencyManager.FormatPrice(Convert.ToDecimal(Eval("SumThisYearOrders"))) %>
          </ItemTemplate>
       </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
