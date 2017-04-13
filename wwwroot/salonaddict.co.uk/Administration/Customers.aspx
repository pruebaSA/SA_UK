<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Customers.aspx.cs" Inherits="SalonAddict.Administration.Customers" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-customers.png" alt="Customers" />
        Manage Customers
    </div>
    <div class="options">
        <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" ToolTip="Search for customers based on the criteria below" />
        <asp:Button ID="btnExportXML" runat="server" Text="Export to XML" OnClick="btnExportXML_Click" ValidationGroup="ExportXML" ToolTip="Export customers list to a xml file" />
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new customer" OnClientClick="location.href='CustomerCreate.aspx';return false" />
    </div>
</div>
<table>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblStartDate" 
                runat="server" 
                Text="Registration from:"
                IsRequired="true"
                ToolTip="The registration date for the search."
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
                Text="Registration to:"
                IsRequired="true"
                ToolTip="The registration to date for the search."
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
                Text="Email:" 
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
                ID="lblUsername" 
                runat="server" Text="Username:" 
                ToolTip="A customer username."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtUsername" runat="server"></asp:TextBox>
        </td>
    </tr>
</table>
<br />
<asp:GridView 
    ID="gvCustomers" 
    runat="server" 
    AutoGenerateColumns="False"
    Width="100%"
    DataKeyNames="CustomerID"
    OnPageIndexChanging="gvCustomers_PageIndexChanging" 
    AllowPaging="true" 
    PageSize="15">
    <Columns>
        <asp:TemplateField HeaderText="Email" ItemStyle-Width="20%">
            <ItemTemplate>
                <a href="CustomerDetails.aspx?CustomerGUID=<%#Eval("CustomerGUID")%>" title="Edit customer details">
                    <%#Server.HtmlEncode(Eval("Email").ToString())%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Name" ItemStyle-Width="20%">
            <ItemTemplate>
                <a href="CustomerDetails.aspx?CustomerGUID=<%#Eval("CustomerGUID")%>" title="Edit customer details">
                    <%#Server.HtmlEncode(Eval("FullName").ToString())%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Username" ItemStyle-Width="20%">
            <ItemTemplate>
                <a href="CustomerDetails.aspx?CustomerGUID=<%#Eval("CustomerGUID")%>" title="Edit customer details">
                    <%# Server.HtmlEncode(Eval("Username").ToString()) %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" >
            <ItemTemplate>
                <a href="CustomerDetails.aspx?CustomerGUID=<%#Eval("CustomerGUID")%>" title="Edit customer details">
                    Edit </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Is Guest" >
            <ItemTemplate>
                <%# Eval("IsGuest") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Is Admin" >
            <ItemTemplate>
                <%# Eval("IsAdmin") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Registration date" >
            <ItemTemplate>
                <%# ((DateTime)Eval("CreatedOn")).ToString() %>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
