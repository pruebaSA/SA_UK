<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MessageQueueDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.MessageQueueDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>

<div class="section-header">
<% if (this.Action == ActionType.Edit)
   { %>
    <div class="title">
        <img src="images/ico-system.png" alt="" />
        Edit message queue details <a href="MessageQueue.aspx" title="Back to queue list">(back to queue list)</a>
    </div>
    <div class="options">
        <asp:Button ID="RequeueButton" runat="server" Text="Requeue" OnClick="RequeueButton_Click" CausesValidation="false" ToolTip="Requeue the message" />
        <% if(Roles.IsUserInRole("SYS_ADMIN"))
           { %>
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save message" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete message" OnClientClick="return confirm('Are you sure?')" />
        <% } %>
    </div>
<% } 
   else
   {%>
    <div class="title">
        <img src="images/ico-configuration.png" alt="" />
        Add a new message <a href="MessageQueue.aspx" title="Back to queue list">(back to queue list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Send" OnClick="AddButton_Click" ToolTip="Send message" />
    </div>
<% } %>
</div>
<table class="details" width="100%">
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblName" 
                runat="server" 
                Text="Message priority:" 
                IsRequired="true"
                ToolTip="The priority of the message." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:DropDownList ID="ddlPriority" runat="server" ></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblFrom"
                runat="server"
                Text="From:" 
                IsRequired="true"
                ToolTip="From address."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtFrom" 
                runat="server"
                Maxlength="120" 
                ErrorMessage="From is a required field." >
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblFromName" 
                runat="server" 
                Text="From name:" 
                ToolTip="From name."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtFromName" runat="server" MaxLength="120" ></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblTo" 
                runat="server" 
                Text="To:" 
                IsRequired="true"
                ToolTip="To address."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtTo" 
                runat="server" 
                MaxLength="120"
                ErrorMessage="To address is a requied field.">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblToName" 
                runat="server" 
                Text="To name:" 
                ToolTip="To name."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtToName" runat="server" MaxLength="120" ></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblCC" 
                runat="server" 
                Text="Cc:" 
                ToolTip="Cc address."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtCc" runat="server" MaxLength="120"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblBcc" 
                runat="server" Text="Bcc:" 
                ToolTip="Bcc address."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtBcc" runat="server" MaxLength="120"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblSubject" 
                runat="server" 
                Text="Subject:" 
                ToolTip="Message subject."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtSubject" runat="server" MaxLength="300" ></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblBody" 
                runat="server" 
                Text="Body:"
                ToolTip="Message body."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <FCKeditorV2:FCKeditor ID="txtBody" runat="server" AutoDetectLanguage="false" Height="350"></FCKeditorV2:FCKeditor>
        </td>
    </tr>
    <% if (this.Action != ActionType.Create)
       { %>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblCreatedOnTitle" 
                runat="server"
                Text="Created on:"
                ToolTip="Date/Time message added to queue." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrCreatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblSendTries" 
                runat="server" 
                Text="Send attempts" 
                ToolTip="The number of times to attempt to send this message."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:NumericTextBox 
                ID="txtSendTries"
                runat="server" 
                RequiredErrorMessage="Enter send tries" 
                MinimumValue="0" 
                MaximumValue="999999"
                Value="0" 
                RangeErrorMessage="The value must be from 0 to 999999">
            </SA:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblSendOnTitle" 
                runat="server" 
                Text="Sent on:" 
                ToolTip="The date/time message was sent."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrSentOn" runat="server"></asp:Literal>
        </td>
    </tr>
    <% } %>
</table>