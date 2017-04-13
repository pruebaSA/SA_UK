<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LocaleStringResourceDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.LocaleStringResourceDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>

<div class="section-header">
    <% if(this.Action == ActionType.Create)
       { %>
    <div class="title">
        <img src="images/ico-content.png" alt="" />
        Add a new resource string
        <a href="LocaleStringResources.aspx" title="Back to resources list" >(Back to resources list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save resource string" />
    </div>
    <% }
       else
       { %>
    <div class="title">
        <img src="images/ico-content.png" alt="" />
        Edit resource string
        <a href="LocaleStringResources.aspx" title="Back to resources list" >(Back to resources list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save resource string" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete resource string" OnClientClick="return confirm('Are you sure?');" />
    </div>
    <% } %>
</div>
<table class="details">
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblLanguage" 
                runat="server" 
                Text="Language:" 
                IsRequired="true"
                ToolTip="The language that this resource is for." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:DropDownList ID="ddlLanguage" runat="server" ></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lbl" 
                runat="server" 
                Text="Resource name:" 
                IsRequired="true"
                ToolTip="The resource string identifier." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtResourceName" 
                runat="server" 
                MaxLength="100"
                ErrorMessage="Resource name is required">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblResourceValue" 
                runat="server" 
                Text="Resource value:" 
                ToolTip="The resource string value." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtResourceValue" 
                runat="server" 
                MaxLength="1000"
                IsRequired="false">
            </SA:TextBox>
        </td>
    </tr>
</table>