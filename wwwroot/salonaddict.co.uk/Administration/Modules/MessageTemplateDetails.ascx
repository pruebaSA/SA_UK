<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MessageTemplateDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.MessageTemplateDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>

<div class="section-header">
    <% if(this.Action == ActionType.Create)
       { %>
    <div class="title">
        <img src="images/ico-content.png" alt="" />
        Add a new message template
        <a href="MessageTemplates.aspx" title="Back to message templates list">(back to message templates list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save message template" />
    </div>
    <% }
       else
       { %>
    <div class="title">
        <img src="images/ico-content.png" alt="" />
        Edit message template details <a href="MessageTemplates.aspx" title="Back to message templates list">(back to message templates list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save message template" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete message template" OnClientClick="return confirm('Are you sure?')" />
    </div>
    <% } %>
</div>
<table class="details" width="100%" >
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblName" 
                runat="server" 
                Text="Name:" 
                IsRequired="true"
                ToolTip="The name of the message template."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtName" 
                runat="server" 
                ErrorMessage="Name is required">
            </SA:TextBox>
        </td>
    </tr>
</table>