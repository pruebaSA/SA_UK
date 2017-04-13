<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServiceCategoryDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.ServiceCategoryDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>

<div class="section-header">
    <% if(this.Action == ActionType.Create)
       { %>
    <div class="title">
        <img src="images/ico-catalog.png" alt="" />
        Add a new service category
        <a href="ServiceCategories.aspx" title="Back to service category list">(back to service category list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save service category" />
    </div>
    <% }
       else
       { %>
    <div class="title">
        <img src="images/ico-catalog.png" alt="" />
        Edit service category details <a href="ServiceCategories.aspx" title="Back to service category list">(back to service category list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save service category" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete service category" OnClientClick="return confirm('Are you sure?')" />
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
                ToolTip="The name of the service category."
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
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblServiceTypes" 
                runat="server" 
                Text="Service type:" 
                IsRequired="true"
                ToolTip="The name of the service type."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:DropDownList ID="ddlServiceTypes" runat="server" ></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblDescription" 
                runat="server" 
                Text="Description:" 
                IsRequired="false"
                ToolTip="The description of the service category."
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
                IsRequired="true"
                ToolTip="The name to use in the meta title."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtMetaTitle" 
                runat="server" 
                MaxLength="100"
                ErrorMessage="Meta title is required">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblPublished" 
                runat="server" 
                Text="Published:" 
                ToolTip="Determines whether this service category is published and can therefore be selected by visitors to your store." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:CheckBox ID="cbPublished" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblDisplayOrder" 
                runat="server" 
                Text="Display order:" ToolTip="The display order for this service category. 1 represents the top of the list." 
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
    <% if (this.Action == ActionType.Edit)
       { %>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblCreatedOn" 
                runat="server" 
                Text="Created on:" 
                ToolTip="Date the service category was created." 
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
                Text="Updated on:" 
                ToolTip="Date the service category was updated on." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrUpdatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
<% } %>
</table>
