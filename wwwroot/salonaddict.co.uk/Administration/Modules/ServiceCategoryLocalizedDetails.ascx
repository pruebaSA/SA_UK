﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServiceCategoryLocalizedDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.ServiceCategoryLocalizedDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>

<div class="section-header">
<% if (this.Action == ActionType.Edit)
   { %>
   <div class="title">
        <img src="images/ico-catalog.png" alt="" />
        Edit service category details <a href="ServiceCategories.aspx" title="Back to service categories list">(back to service categories list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save service category details" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete service category details" OnClientClick="return confirm('Are you sure?')" />
    </div>
<% } 
   else
   {%>
    <div class="title">
        <img src="images/ico-catalog.png" alt="" />
        Add a new service category details <a href="ServiceCategories.aspx" title="Back to service categories list">(back to service categories list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save service category details" />
    </div>
<% } %>
</div>
<table class="details" width="100%" >
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblLanguageTitle" 
                runat="server" 
                Text="Language:" 
                ToolTip="The language that this service category is for."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:DropDownList ID="ddlLanguage" runat="server" ></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblServiceCategory" 
                runat="server" 
                Text="Service Category:" 
                ToolTip="The title of this service category (read only)."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrServiceCategory" runat="server"></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblTitle" 
                runat="server" 
                Text="Title:" 
                ToolTip="Service category display name."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtTitle" 
                runat="server" 
                ErrorMessage="Title is required">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblDescription" 
                runat="server" 
                Text="Description:" 
                IsRequired="false"
                ToolTip="The description of the service type."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtDescription" 
                runat="server" 
                MaxLength="400"
                IsRequired="false"
                ErrorMessage="Description is required">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblMetaTitle" 
                runat="server" 
                Text="Meta Title:" 
                IsRequired="false"
                ToolTip="The name to use in the meta title."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtMetaTitle" 
                runat="server" 
                MaxLength="100"
                IsRequired="false"
                ErrorMessage="Meta title is required">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblDisplayOrder" 
                runat="server" 
                Text="Display order:" ToolTip="The display order for this service type. 1 represents the top of the list." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:NumericTextBox  
                ID="txtDisplayOrder" 
                runat="server"
                Value="1" 
                MaxLength="4"
                RequiredErrorMessage="Display order is required" 
                RangeErrorMessage="The value must be from 1 to 99999"
                MinimumValue="1" 
                MaximumValue="99999">
             </SA:NumericTextBox>
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
                ToolTip="Date/Time service type details was created." 
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
                ToolTip="The date/time service type details was updated on."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrUpdatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
    <% } %>
</table>