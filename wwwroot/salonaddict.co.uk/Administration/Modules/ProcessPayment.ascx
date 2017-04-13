<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProcessPayment.ascx.cs" Inherits="SalonAddict.Administration.Modules.ProcessPayment" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="PaymentMethods" Src="~/Administration/Modules/DropDownLists/PaymentMethodDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="ExpiryMonth" Src="~/Administration/Modules/DropDownLists/MonthDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="ExpiryYear" Src="~/Administration/Modules/DropDownLists/YearDropDownList.ascx" %>

<table class="details" cellpadding="0" cellspacing="0" >
   <tr>
      <td class="title">
        <SA:ToolTipLabel 
            ID="lblCardHolderName" 
            runat="server" 
            Text="Cardholder Name:"
            IsRequired="true"
            ToolTip="Cardholder name." 
            ToolTipImage="~/Administration/images/ico-help.gif" />
      </td>
      <td class="data-item">
          <SA:TextBox ID="txtCardholderName" runat="server" MaxLength="100"  ErrorMessage="Cardholder name is a required field." />
      </td>
   </tr>
   <tr>
      <td class="title">
        <SA:ToolTipLabel 
            ID="lblCardNumber" 
            runat="server" 
            Text="Card Number:"
            IsRequired="true"
            ToolTip="Card number." 
            ToolTipImage="~/Administration/images/ico-help.gif" />
      </td>
      <td class="data-item">
          <SA:TextBox ID="txtCardNumber" runat="server" MaxLength="100"  ErrorMessage="Cardholder number is a required field." />
      </td>
   </tr>
   <tr>
      <td class="title">
        <SA:ToolTipLabel 
            ID="lblCardType" 
            runat="server" 
            Text="Card Type:"
            IsRequired="true"
            ToolTip="Card number." 
            ToolTipImage="~/Administration/images/ico-help.gif" />
      </td>
      <td class="data-item">
          <SA:PaymentMethods ID="ddlCardTypes" runat="server"  ErrorMessage="Card type is a required field." />
      </td>
   </tr>
   <tr>
      <td class="title" >
          <SA:ToolTipLabel 
            ID="lblCardExpiry" 
            runat="server" 
            Text="Expiry Date:"
            IsRequired="true"
            ToolTip="Card expiry." 
            ToolTipImage="~/Administration/images/ico-help.gif" />
      </td>
      <td class="data-item" >
          <table cellpadding="0" cellspacing="0" >
             <tr>
                 <td>
                    <SA:ExpiryMonth ID="ddlExpiryMonth" runat="server" />
                 </td>
                 <td>
                    <SA:ExpiryYear ID="ddlExpiryYear" runat="server" />
                 </td>
             </tr>
          </table>
      </td>
   </tr>
      <tr>
      <td class="title" >
          <SA:ToolTipLabel 
            ID="lblCardCode" 
            runat="server" 
            Text="Card Code"
            IsRequired="false"
            ToolTip="Card security code." 
            ToolTipImage="~/Administration/images/ico-help.gif" />
      </td>
      <td class="data-item" >
          <SA:TextBox ID="txtCardCode" runat="server" MaxLength="4" IsRequired="false" Width="50px" />
      </td>
   </tr>
</table>