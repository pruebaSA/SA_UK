<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="PurgeOrphanedPictures.aspx.cs" Inherits="SalonAddict.Administration.PurgeOrphanedPictures" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-system.png" alt="System" />
        Purge Orphaned Pictures
    </div>
    <div class="options">
        <asp:Button ID="btnPurge" runat="server" Text="Purge" ToolTip="Purge Orphaned Pictures" OnClick="btnPurge_Click" />
    </div>
</div>
<SA:Message ID="lblRecordsEffected" runat="server" IsError="false" Auto="false" />
<table class="details" cellpadding="0" cellspacing="0" >
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblStartDate" 
                runat="server" 
                Text="Start date:"
                IsRequired="true"
                ToolTip="The start date of the pictures to purge."
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
                ToolTip="The end date of the pictures to purge."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtEndDate" runat="server"  />
            <asp:ImageButton ID="iEndDate" runat="Server" ImageUrl="~/Administration/images/ico-calendar.png"  AlternateText="Click to show calendar" /><br />
            <ajaxToolkit:CalendarExtender ID="cEndDateButtonExtender" runat="server" TargetControlID="txtEndDate" PopupButtonID="iEndDate" />
        </td>
    </tr>
</table>
<br />
</asp:Content>