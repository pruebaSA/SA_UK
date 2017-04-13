<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="GuestOrders.aspx.cs" Inherits="SalonAddict.Administration.GuestOrders" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="OrderStatusList" Src="~/Administration/Modules/OrderStatusDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="PaymentStatusList" Src="~/Administration/Modules/PaymentStatusDropDownList.ascx" %>
<%@ Import Namespace="SalonAddict.Common" %>
<%@ Import Namespace="SalonAddict.BusinessAccess.Implementation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="Orders" />
        Manage Orders
    </div>
    <div class="options">
        <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" ToolTip="Search for orders based on the criteria below" />
        <asp:Button ID="btnExportXML" runat="server" Text="Export to XML" OnClick="btnExportXML_Click" ValidationGroup="ExportXML" ToolTip="Export order list to a xml file" />
    </div>
</div>
<table class="details" cellpadding="0" cellspacing="0" >
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblStartDate" 
                runat="server" 
                Text="Start date:"
                IsRequired="true"
                ToolTip="The start date for the search."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox runat="server" ID="txtStartDate" />
            <asp:ImageButton ID="iStartDate" runat="Server" ImageUrl="~/Administration/images/ico-calendar.png" AlternateText="Click to show calendar" /><br />
            <ajaxToolkit:CalendarExtender ID="cStartDateButtonExtender" runat="server" TargetControlID="txtStartDate" PopupButtonID="iStartDate" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblEndDate" 
                runat="server" 
                Text="End date:"
                IsRequired="true"
                ToolTip="The end date for the search."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtEndDate" runat="server"  />
            <asp:ImageButton ID="iEndDate" runat="Server" ImageUrl="~/Administration/images/ico-calendar.png"  AlternateText="Click to show calendar" /><br />
            <ajaxToolkit:CalendarExtender ID="cEndDateButtonExtender" runat="server" TargetControlID="txtEndDate" PopupButtonID="iEndDate" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblEmail" 
                runat="server" 
                Text="Customer email address:" 
                ToolTip="A customer Email."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtEmail" runat="server" MaxLength="120" ></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblReferenceNo" 
                runat="server" 
                Text="Go directly to reference no:" 
                ToolTip="Search by a specific payment status."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
           <SA:TextBox 
             ID="txtReferenceNo" 
             runat="server" 
             Width="180px"
             MaxLength="10"
             ErrorMessage="Reference no is a required field." 
             ValidationExpression=".{4,}" 
             ValidationMessage="You must enter atleast 4 characters."
             ValidationGroup="direct" />
           &nbsp;
           <asp:Button ID="btnGo" runat="server" Text="Go" OnClick="btnGo_Click" ValidationGroup="direct" />
        </td>
    </tr>
</table>
<br />
<asp:GridView 
    ID="gvOrders" 
    runat="server" 
    AutoGenerateColumns="False"
    Width="100%"
    DataKeyNames="OrderID"
    OnPageIndexChanging="gvOrders_PageIndexChanging" 
    AllowPaging="true" 
    PageSize="25">
    <Columns>
        <asp:TemplateField HeaderText="Order ID" >
            <ItemTemplate>
                <%# Eval("OrderID") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Reference No." >
            <ItemTemplate>
                <%# Eval("ReferenceNo") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Order Total" >
            <ItemTemplate>
                <%# CurrencyManager.FormatPrice((decimal)Eval("Total")) %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Order Status" >
            <ItemTemplate>
                <%# Eval("OrderStatus") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Payment Status" >
            <ItemTemplate>
                <%# Eval("PaymentStatus") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Customer" >
            <ItemTemplate>
                <%# Eval("BillingEmail") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" >
            <ItemTemplate>
                <a href="OrderDetails.aspx?OrderID=<%#Eval("OrderID")%>" title="Edit order details">
                    view 
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Created on" >
            <ItemTemplate>
                <%# ((DateTime)Eval("CreatedOn")).ToFriendlyTimeFormat(TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")).ToString("f").ToString()%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>