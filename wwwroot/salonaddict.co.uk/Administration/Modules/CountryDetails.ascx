<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CountryDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.CountryDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>

<div class="section-header">
<% if(this.Action == ActionType.Edit)
   { %>
    <div class="title">
        <img src="images/ico-configuration.png" alt="" />
        Edit country details <a href="Countries.aspx" title="Back to countries list">(back to countries list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save country" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete country" OnClientClick="return confirm('Are you sure?')" />
    </div>
<% } 
   else
   {%>
    <div class="title">
        <img src="images/ico-configuration.png" alt="" />
        Add a new country <a href="Countries.aspx" title="Back to countries list">(back to countries list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save country" />
    </div>
<% } %>
</div>
<table class="details">
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblName" 
                runat="server" 
                Text="Name:" 
                IsRequired="true"
                ToolTip="The name of the country." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtName" 
                runat="server" 
                MaxLength="50"
                ErrorMessage="Country name is a required field.">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblTwoLetterISOCode" 
                runat="server" 
                Text="Two letter ISO code:" 
                IsRequired="true"
                ToolTip="The two letter ISO code for this country. For a complete list of ISO codes go to: http://en.wikipedia.org/wiki/ISO_3166-1_alpha-2" 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtTwoLetterISOCode" 
                runat="server" 
                 MaxLength="2"
                ErrorMessage="Two letter ISO code is a required field." >
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblThreeLetterISOCode" 
                runat="server" 
                Text="Three letter ISO code:" 
                IsRequired="true"
                ToolTip="The three letter ISO code for this country. For a complete list of ISO codes go to: http://en.wikipedia.org/wiki/ISO_3166-1_alpha-3" 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtThreeLetterISOCode" 
                runat="server"
                 MaxLength="3"
                ErrorMessage="Three letter ISO code is a required field.">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblNumbericISOCode" 
                runat="server" 
                IsRequired="true"
                Text="Numeric ISO code:" 
                ToolTip="The numeric ISO code for this country. For a complete list of ISO codes go to: http://en.wikipedia.org/wiki/ISO_3166-1_numeric" 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:NumericTextBox 
                ID="txtNumericISOCode" 
                runat="server" 
                Value="1" 
                MaxLength="4"
                RequiredErrorMessage="Numeric ISO code is a required field." 
                RangeErrorMessage="The value must be from 1 to 9999"
                MinimumValue="1" 
                MaximumValue="9999">
            </SA:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblPublished" 
                runat="server" 
                Text="Published:" 
                ToolTip="Determines whether this country is published (visible for new account registrations and creation of shipping/billing addresses)." 
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
                Text="Display order:" ToolTip="The display order for this country. 1 represents the top of the list." 
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
</table>