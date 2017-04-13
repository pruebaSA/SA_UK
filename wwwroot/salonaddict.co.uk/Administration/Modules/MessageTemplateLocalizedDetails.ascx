<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MessageTemplateLocalizedDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.MessageTemplateLocalizedDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>

<div class="section-header">
<% if (this.Action == ActionType.Edit)
   { %>
   <div class="title">
        <img src="images/ico-content.png" alt="" />
        Edit message template details <a href="MessageTemplates.aspx" title="Back to message templates list">(back to message templates list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save message template" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete message template" OnClientClick="return confirm('Are you sure?')" />
    </div>
<% } 
   else
   {%>
    <div class="title">
        <img src="images/ico-configuration.png" alt="" />
        Add a new template details <a href="MessageTemplates.aspx" title="Back to message templates list">(back to message templates list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save details" />
    </div>
<% } %>
</div>
<table class="details" width="100%" 
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblLanguageTitle" 
                runat="server" 
                Text="Language:" 
                IsRequired="true"
                ToolTip="The language that this template is for."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:DropDownList ID="ddlLanguage" runat="server" ></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblName" 
                runat="server" 
                Text="Template:" 
                IsRequired="true"
                ToolTip="The name of this template (read only)."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrTemplate" runat="server"></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblSubject" 
                runat="server" 
                Text="Subject:" 
                IsRequired="true"
                ToolTip="The subject of the message (email). TIP - You can include tokens in your subject."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtSubject" 
                runat="server" 
                ErrorMessage="Subject is required">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblBody" 
                runat="server" 
                Text="Body:" 
                ToolTip="The body of your message."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <FCKeditorV2:FCKeditor 
                ID="txtBody" 
                runat="server" 
                AutoDetectLanguage="false" 
                Height="350"
                ToolbarSet="SACustom" >
            </FCKeditorV2:FCKeditor>
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
                ToolTip="Date/Time template was created." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrCreatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblUpdatedOn" 
                runat="server" 
                Text="Sent on:" 
                ToolTip="The date/time template updated on."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrUpdatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
    <% } %>
</table>