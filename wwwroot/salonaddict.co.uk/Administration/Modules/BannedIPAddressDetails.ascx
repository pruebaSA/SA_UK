<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BannedIPAddressDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.BannedIPAddressDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>

<% if (this.Action == ActionType.Create)
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-blacklist.png" alt="" />
        Add IP to blacklist <a href="Blacklist.aspx" title="Back to blacklist">(back to blacklist)</a>
    </div>
    <div class="options">
        <asp:Button ID="btnAddButton" runat="server" Text="Save" OnClick="btnAddButton_Click" ToolTip="Save IP to blacklist" />
    </div>
</div>
<% }
   else
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-blacklist.png" alt="" />
        Edit IP from blacklist <a href="Blacklist.aspx" title="Back to blacklist">(back to blacklist)</a>
    </div>
    <div class="options">
        <asp:Button ID="btnSaveButton" runat="server" Text="Save" OnClick="btnSaveButton_Click" ToolTip="Save IP to blacklist" />
        <asp:Button ID="btnDeleteButton" runat="server" Text="Delete" OnClick="btnDeleteButton_Click" CausesValidation="false" OnClientClick="return confirm('Are you sure?')" ToolTip="Delete IP address" />
    </div>
</div>
<% }%>
<table class="details" >
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblBannedIP" 
                runat="server" 
                Text="Banned IP address"
                IsRequired="true"
                ToolTip="Banned IP address or network" 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <SA:TextBox 
                ID="txtBannedIP" 
                runat="server" 
                MaxLength="50" 
                ErrorMessage="IP Address is a required field." />
            &nbsp; format: <b>192.168.1.100</b> &nbsp; <asp:Label ID="lblBannedIPError" runat="server" ForeColor="Red" Font-Bold="true" Visible="false" ></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblComment" 
                runat="server" 
                Text="Comment" 
                ToolTip="Reason why the IP address or network was banned"
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Rows="5" Width="400px"></asp:TextBox>
        </td>
    </tr>
    <% if (this.Action == ActionType.Edit)
       { %>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblCreatedOnTitle" 
                runat="server" 
                Text="Created on"
                ToolTip="Date and time when the IP was banned" 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <asp:Literal ID="ltrCreatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblUpdatedOnTitle" 
                runat="server" 
                Text="Updated on"
                ToolTip="Date and time when the IP was last updated" 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <asp:Literal ID="ltrUpdatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
    <% } %>
</table>