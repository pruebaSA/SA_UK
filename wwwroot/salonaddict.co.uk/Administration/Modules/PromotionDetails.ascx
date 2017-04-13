<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PromotionDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.PromotionDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="BusinessList" Src="~/Administration/Modules/DropDownLists/BusinessDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="LanguageType" Src="~/Administration/Modules/DropDownLists/LanguageDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="PromotionCategoryType" Src="~/Administration/Modules/DropDownLists/PromotionCategoryTypeDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="DecimalTextBox" Src="~/Administration/Modules/TextBoxes/DecimalTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>

<div class="section-header">
    <% if(this.Action == ActionType.Create)
       { %>
    <div class="title">
        <img src="images/ico-content.png" alt="" />
        Add a new promotion
        <a href="Promotions.aspx" title="Back to quick search list">(back to promotion list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save promotion" />
    </div>
    <% }
       else
       { %>
    <div class="title">
        <img src="images/ico-content.png" alt="" />
        Edit promotion details <a href="Promotions.aspx" title="Back to promotion list">(back to promotion list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save promotion" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete promotion" OnClientClick="return confirm('Are you sure?')" />
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
                ToolTip="Select a language for promotion. A promotion can be created for each language your store supports."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:LanguageType ID="ddlLanguage" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblPromotionCategory" 
                runat="server" 
                Text="Category:" 
                IsRequired="true"
                ToolTip="The promotion category."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:PromotionCategoryType ID="ddlCategory" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblBusiness" 
                runat="server" 
                Text="Business:" 
                IsRequired="false"
                ToolTip="The promoted business."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:BusinessList 
                ID="ddlBusiness" 
                DefaultText="Choose a Business"
                DefaultValue="0"
                runat="server" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblTitle" 
                runat="server" 
                Text="Title:" 
                IsRequired="true"
                ToolTip="The promotion title to display." 
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
                ID="lblDescription" 
                runat="server" 
                Text="Description:" 
                IsRequired="false"
                ToolTip="The promotion title to display." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtDescription" 
                runat="server" 
                MaxLength="200"
                IsRequired="false" >
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblOldPrice" 
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
                ID="lblIsSpecialOffer" 
                runat="server"
                IsRequired="false"
                Text="Special Offer:" 
                ToolTip="Indicates whether the promotion is a special offer. (Used to highlight the promotion on the site)."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:CheckBox ID="cbIsSpecialOffer" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblPublished" 
                runat="server"
                IsRequired="false"
                Text="Publish:" 
                ToolTip="Indicates whether the promotion is published. (Used to display the promotion on the site)."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:CheckBox ID="cbPublished" runat="server" />
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