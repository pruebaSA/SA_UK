<%@ Page Title="" Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="GiftCards.aspx.cs" Inherits="SalonAddict.Administration.GiftCards" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Import Namespace="SalonAddict.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="Sales" />
        Manage Gift Cards
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new gift card" OnClientClick="location.href='GiftCardCreate.aspx';return false" />
    </div>
</div>
<br />
<table class="details" cellpadding="0" cellspacing="0" >
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblActivated" 
                runat="server" 
                Text="Activated:" 
                IsRequired="true"
                ToolTip="Search by a specific gift card status. (eg Activated)."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true" Width="250px" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged" >
                <asp:ListItem Text="All" Value="0" ></asp:ListItem>
                <asp:ListItem Text="Activated" Value="1" ></asp:ListItem>
                <asp:ListItem Text="Deactivated" Value="2" ></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblGiftCardCouponCode" 
                runat="server" 
                Text="Gift card coupon code:" 
                IsRequired="true"
                ToolTip="Search by a specific gift card coupon code."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
           <SA:TextBox 
             ID="txtGiftCardCouponCode" 
             runat="server" 
             Width="250px"
             MaxLength="50"
             ErrorMessage="Gift card coupon code is a required field." 
             ValidationGroup="direct" />
           &nbsp;
           <asp:Button ID="btnGo" runat="server" Text="Go" OnClick="btnGo_Click" ValidationGroup="direct" />
        </td>
    </tr>
</table>
<br /><br />
<asp:GridView 
    ID="gv" 
    runat="server" 
    AutoGenerateColumns="False"
    Width="100%"
    DataKeyNames="GiftCardID" >
    <Columns>
        <asp:TemplateField HeaderText="Sender Name" >
            <ItemTemplate>
                <%# Eval("SenderName").ToString().HtmlEncode()%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Sender Email" >
            <ItemTemplate>
                <%# Eval("SenderEmail").ToString().HtmlEncode() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Recipient Name" >
            <ItemTemplate>
                <%# Eval("RecipientName").ToString().HtmlEncode() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Recipient Email" >
            <ItemTemplate>
                <%# Eval("RecipientEmail").ToString().HtmlEncode() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Code" ItemStyle-Width="130px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <%# Eval("GiftCardCouponCode") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Amount" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <%# Math.Round((decimal)Eval("Amount"), 2) %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Expires On" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <%# ((DateTime)Eval("ExpiresOn") == DateTime.MinValue)? String.Empty : ((DateTime)Eval("ExpiresOn")).ToShortDateString() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <a href="GiftCardDetails.aspx?GiftCardID=<%#Eval("GiftCardID")%>" title="Edit gift card details">
                   Edit
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
