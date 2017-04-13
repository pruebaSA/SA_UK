<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LogDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.LogDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="images/ico-system.png" alt="" />
        View log entry details <a href="Logs.aspx" title="Back to system log">(back to system log)</a>
    </div>
    <div class="options">
        <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" CausesValidation="false" ToolTip="Delete log entry" OnClientClick="return confirm('Are you sure?');" />
    </div>
</div>
<table class="details">
    <col width="100px" />
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblLogType" 
                runat="server" 
                Text="Log type:" 
                ToolTip="The type of log entry." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrLogType" runat="server"></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblSeverity" 
                runat="server" 
                Text="Severity:" 
                ToolTip="The severity of the log entry." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrSeverity" runat="server"></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblMessage" 
                runat="server" 
                Text="Message:" 
                ToolTip="The log entry message." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrMessage" runat="server"></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblException" 
                runat="server" 
                Text="Severity:" 
                ToolTip="The exception details for the log entry." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrException" runat="server"></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblIPAddress" 
                runat="server" 
                Text="IP address:" 
                ToolTip="IP address of the machine that caused the exception." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrIPAddress" runat="server"></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblUser" 
                runat="server" 
                Text="User:" 
                ToolTip="Name of the user who caused the exception." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:HyperLink ID="hlUser" runat="server" ></asp:HyperLink>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblPageURL" 
                runat="server" 
                Text="Page URL:" 
                ToolTip="Originating page of exception." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrPageURL" runat="server"></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblCreatedOn" 
                runat="server" 
                Text="Created on:" 
                ToolTip="Originating page of exception." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrCreatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
</table>
