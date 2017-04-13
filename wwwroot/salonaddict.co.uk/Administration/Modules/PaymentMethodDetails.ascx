<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentMethodDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.PaymentMethodDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>

<% if (this.Action == ActionType.Create)
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Add a new payment method <a href="PaymentMethods.aspx" title="Back to payment method list"> (back to payment method list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save payment method" />
    </div>
</div>
<% }
   else
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Edit payment method details <a href="PaymentMethods.aspx" title="Back to payment method list">(back to payment method list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save payment method" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete payment method" OnClientClick="return confirm('Are you sure?')" />
    </div>
</div>
<% } %>
<table class="details">
   <tr>
        <td class="title" style="vertical-align:top;">
            <SA:ToolTipLabel 
                ID="lblPicture" 
                runat="server" 
                Text="Picture:"
                ToolTip="The payment method's picture." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Image ID="imgPicture" runat="server" /><br />
            <p><asp:FileUpload ID="fuPicture" runat="server" /></p>
            <asp:Label ID="lblPictureError" runat="server" EnableViewState="false" CssClass="message-error" Text="only files of type *.(jpg|jpeg|png|gif) can be uploaded" Visible="false" ></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblName" 
                runat="server"
                Text="Name:" 
                IsRequired="true"
                ToolTip="The payment method name."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox  
                ID="txtName"  
                runat="server"
                MaxLength="50"
                ErrorMessage="Name is a required field.">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblSystemKeyword" 
                runat="server"
                Text="System keyword:" 
                IsRequired="true"
                ToolTip="The payment methods' system keyword."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox  
                ID="txtSystemKeyword"  
                runat="server"
                MaxLength="50"
                ErrorMessage="System keyword is a required field.">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblActive"
                runat="server"  
                Text="Active:" 
                ToolTip="Value indicating whether the method is active."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:CheckBox ID="cbActive" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblDisplayOrder" 
                runat="server"
                Text="Display order:"
                ToolTip="The display order of this payment method. 1 represents the top of the list."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:NumericTextBox 
                ID="txtDisplayOrder" 
                runat="server"
                Value="1" 
                RequiredErrorMessage="Display order is required" 
                MinimumValue="1"
                MaximumValue="99999"
                RangeErrorMessage="The value must be from 1 to 99999">
            </SA:NumericTextBox>
        </td>
    </tr>
</table>