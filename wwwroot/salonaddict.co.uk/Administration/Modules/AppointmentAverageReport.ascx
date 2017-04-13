<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppointmentAverageReport.ascx.cs" Inherits="SalonAddict.Administration.Modules.AppointmentAverageReport" %>
<%@ Import Namespace="SalonAddict.BusinessAccess.Implementation" %>

<p><b>Order Totals</b></p>
<asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
       <asp:TemplateField HeaderText="Order Status" ItemStyle-Width="20%">
            <ItemTemplate>
                <%# Eval("Key") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Today" ItemStyle-Width="16%">
            <ItemTemplate>
                <%# CurrencyManager.FormatPrice(Convert.ToDecimal(Eval("Value.SumTodayOrders")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="This Week" ItemStyle-Width="16%">
            <ItemTemplate>
                <%# CurrencyManager.FormatPrice(Convert.ToDecimal(Eval("Value.SumThisWeekOrders")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="This Month" ItemStyle-Width="16%">
            <ItemTemplate>
                <%# CurrencyManager.FormatPrice(Convert.ToDecimal(Eval("Value.SumThisMonthOrders")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="This Year" ItemStyle-Width="16%">
            <ItemTemplate>
                <%# CurrencyManager.FormatPrice(Convert.ToDecimal(Eval("Value.SumThisYearOrders")))%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>