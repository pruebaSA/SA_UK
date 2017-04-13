<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuickSearchDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.QuickSearchDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="LanguageType" Src="~/Administration/Modules/DropDownLists/LanguageDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="QuickSearchCategoryType" Src="~/Administration/Modules/DropDownLists/QuickSearchCategoryTypeDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="DecimalTextBox" Src="~/Administration/Modules/TextBoxes/DecimalTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>

<div class="section-header">
    <% if(this.Action == ActionType.Create)
       { %>
    <div class="title">
        <img src="images/ico-content.png" alt="" />
        Add a new quick search
        <a href="QuickSearches.aspx" title="Back to quick search list">(back to quick search list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save quick search" />
    </div>
    <% }
       else
       { %>
    <div class="title">
        <img src="images/ico-content.png" alt="" />
        Edit quick search details <a href="QuickSearches.aspx" title="Back to quick search list">(back to quick search list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save quick search" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete quick search" OnClientClick="return confirm('Are you sure?')" />
    </div>
    <% } %>
</div>
<table class="details" width="100%" >
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblLanguage" 
                runat="server" 
                Text="Language:" 
                IsRequired="true"
                ToolTip="Select a language for your quick search. A quick search can be created for each language your store supports."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:LanguageType ID="ddlLanguage" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblQuickSearchCategory" 
                runat="server" 
                Text="Category:" 
                IsRequired="true"
                ToolTip="The quick search category."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:QuickSearchCategoryType ID="ddlCategory" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblTitle" 
                runat="server" 
                Text="Title:" 
                IsRequired="true"
                ToolTip="The quick search title to display." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtTitle" 
                runat="server" 
                MaxLength="200"
                ErrorMessage="Title is a required field.">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblNavigateUrl" 
                runat="server" 
                Text="Navigate Url:" 
                IsRequired="true"
                ToolTip="The quick search fully qualified url (http://www.mydomain.com/)." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtNavigateUrl" 
                runat="server" 
                MaxLength="1000"
                ValidationExpression="http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&amp;=+]*)?"
                ValidationMessage="Url must start with http:// or https://"
                ErrorMessage="Navigation url is a required field.">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblDiscountAmount" 
                runat="server"
                IsRequired="false"
                Text="Old Price:" 
                ToolTip="The old price."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:DecimalTextBox 
                ID="txtOldPrice" 
                runat="server" 
                MinimumValue="0" 
                MaximumValue="999" 
                IsRequired="false"
                RangeErrorMessage="The value must be from 0 to 999">
            </SA:DecimalTextBox>
            &nbsp;
            [<%= SalonAddict.BusinessAccess.Implementation.CurrencyManager.PrimaryCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblPrice" 
                runat="server"
                IsRequired="false"
                Text="Price:" 
                ToolTip="The price."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:DecimalTextBox 
                ID="txtPrice" 
                runat="server" 
                MinimumValue="0" 
                MaximumValue="999" 
                IsRequired="false"
                RangeErrorMessage="The value must be from 0 to 999">
            </SA:DecimalTextBox>
            &nbsp;
            [<%= SalonAddict.BusinessAccess.Implementation.CurrencyManager.PrimaryCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblShowOnHomepage" 
                runat="server"
                IsRequired="false"
                Text="Homepage:" 
                ToolTip="Indicates whether to display the quick search on the homepage."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:CheckBox ID="cbShowOnHomepage" runat="server" Checked="true" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblShowOnRSS" 
                runat="server"
                IsRequired="false"
                Text="RSS Feeds:" 
                ToolTip="Indicates whether to display the quick search within the RSS feeds."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:CheckBox ID="cbShowOnRSS" runat="server" Checked="true" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblDisplayOrder" 
                runat="server" 
                Text="Display order:" 
                IsRequired="false"
                ToolTip="The display order for this quick search. 1 represents the top of the list." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:NumericTextBox  
                ID="txtDisplayOrder" 
                runat="server"
                IsRequired="false"
                Value="1" 
                MaxLength="9"
                RangeErrorMessage="The value must be from 1 to 999999"
                MinimumValue="1" 
                MaximumValue="999999">
             </SA:NumericTextBox>
        </td>
    </tr>
    <% if(this.Action == ActionType.Edit)
       { %>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblCreatedOn" 
                runat="server" 
                Text="Created on:" 
                IsRequired="false"
                ToolTip="The creation date." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Label ID="txtCreatedOn" runat="server" SkinID="TextBox" ></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblUpdatedOn" 
                runat="server" 
                Text="Updated on:" 
                IsRequired="false"
                ToolTip="The date last modified." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Label ID="txtUpdatedOn" runat="server" SkinID="TextBox" ></asp:Label>
        </td>
    </tr>
    <% } %>
</table>