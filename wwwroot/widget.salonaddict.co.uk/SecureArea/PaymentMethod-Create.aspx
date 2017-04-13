<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="PaymentMethod-Create.aspx.cs" Inherits="IFRAME.SecureArea.PaymentMethod_CreatePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" DefaultButton="btnSave" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Account" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-account.png" %>' alt="account" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >

              </td>
           </tr>
        </table>
        <table style="margin-top:10px;" class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td colspan="2" >
                 <asp:Label ID="lblError" runat="server" SkinID="ErrorLabel" EnableViewState="false" ></asp:Label>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrAlias.Text") %></td>
              <td class="data-item" >
                 <asp:TextBox ID="txtAlias" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                 <asp:RequiredFieldValidator ID="valAlias" runat="server" ControlToValidate="txtAlias" Display="None" meta:resourceKey="valAlias" ></asp:RequiredFieldValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valAliasEx" runat="Server" TargetControlID="valAlias" EnableViewState="false" />
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrCardType.Text") %></td>
              <td class="data-item" >
                 <asp:DropDownList ID="ddlCardType" runat="server" SkinID="DropDownList" Width="190px" >
                    <asp:ListItem Text="Visa" Value="VISA" ></asp:ListItem>
                    <asp:ListItem Text="MasterCard" Value="MASTERCARD" ></asp:ListItem>
                    <asp:ListItem Text="Laser" Value="LASER" ></asp:ListItem>
                    <asp:ListItem Text="American Express" Value="AMEX" ></asp:ListItem>
                    <asp:ListItem Text="Maestro" Value="MAESTRO" ></asp:ListItem>
                 </asp:DropDownList>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrCardName.Text") %></td>
              <td class="data-item" >
                 <asp:TextBox ID="txtCardName" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                 <asp:RequiredFieldValidator ID="valCardName" runat="server" ControlToValidate="txtCardName" Display="None" meta:resourceKey="valCardName" ></asp:RequiredFieldValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valCardNameEx" runat="Server" TargetControlID="valCardName" EnableViewState="false" />
                 <asp:RegularExpressionValidator ID="valCardNameRegex1" runat="server" ControlToValidate="txtCardName" Display="None" ValidationExpression="[^0-9]+" meta:resourceKey="valCardNameRegex1"></asp:RegularExpressionValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valCardNameRegex1Ex" runat="Server" TargetControlID="valCardNameRegex1" EnableViewState="false" />
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrCardNumber.Text") %></td>
              <td class="data-item" >
                 <asp:TextBox ID="txtCardNumber" runat="server" SkinID="TextBox" MaxLength="20" ></asp:TextBox>
                 <asp:RequiredFieldValidator ID="valCardNumber" runat="server" ControlToValidate="txtCardNumber" Display="None" meta:resourceKey="valCardNumber" ></asp:RequiredFieldValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valCardNumberEx" runat="Server" TargetControlID="valCardNumber" EnableViewState="false" />
                 <asp:RegularExpressionValidator ID="valCardNumberRegex1" runat="server" ControlToValidate="txtCardNumber" Display="None" ValidationExpression="[\d\s-]+" meta:resourceKey="valCardNumberRegex1"></asp:RegularExpressionValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valCardNumberRegex1Ex" runat="Server" TargetControlID="valCardNumberRegex1" EnableViewState="false" />
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrExpiry.Text") %></td>
              <td class="data-item" >
                 <table cellpadding="0" cellspacing="0" >
                    <tr>
                       <td>
                          <asp:DropDownList ID="ddlExpiryMonth" runat="server" SkinID="DropDownList" Width="60px" ></asp:DropDownList>
                       </td>
                       <td style="padding-left:10px" >
                          <asp:DropDownList ID="ddlExpiryYear" runat="server" SkinID="DropDownList" Width="90px" ></asp:DropDownList>
                       </td>
                       <td style="width:180px" >&nbsp;</td>
                    </tr>
                 </table>
              </td>
           </tr>
           <tr><td colspan="2" >&nbsp;</td></tr>
           <tr>
              <td class="title" ></td>
              <td class="data-item" >
                 <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                 <asp:Button ID="btnSave" runat="server" SkinID="SubmitButton" OnClick="btnSave_Click" OnClientClick="if(Page_ClientValidate()){this.disabled=true;}" UseSubmitBehavior="false" meta:resourceKey="btnSave" />
                 <p>By clicking "Save" you agree to our <a href="#" onclick="$('#dialog').dialog({ title:'Terms and Conditions', width:480, height:250, position: ['center',50] });" ><u>Terms and Conditions.</u></a></p>
              </td>
           </tr>
        </table>
        <div id="dialog" style="display:none;" >
            <p>
                Your access to the SalonAddict Reservation Widget is valid for the full duration of the subscription plan 
                you have selected (the “Contract Period”). A Contract Period equal to 1 calendar year applies to all 
                subscription plans. Billing periods may vary. At the end of the Contract Period you will be automatically 
                re-billed (“Automatic Renewal”), and your account will enter a new Contract Period.
                <div>&nbsp;</div>
            </p>
        </div>
    </asp:Panel>
</asp:Content>
