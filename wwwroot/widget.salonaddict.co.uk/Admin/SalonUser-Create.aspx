<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="SalonUser-Create.aspx.cs" Inherits="IFRAME.Admin.SalonUser_CreatePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Salons" />
       <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-salons.png" %>' alt="salons" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
               
              </td>
           </tr>
        </table>
        <table class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrSalon.Text") %></td>
              <td class="data-item" >
                  <asp:Literal ID="ltrSalon" runat="server" ></asp:Literal>
              </td>
              <td></td>
           </tr>
          <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrUsername.Text") %></td>
              <td class="data-item" >
                 <asp:TextBox ID="txtUsername" runat="server" SkinID="TextBox" MaxLength="50" ></asp:TextBox>
                 <asp:RequiredFieldValidator ID="valUsername" runat="server" ControlToValidate="txtUsername" Display="None" meta:resourceKey="valUsername" ></asp:RequiredFieldValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="txtUsernameEx" runat="server" TargetControlID="valUsername" EnableViewState="false" />
                 <asp:RegularExpressionValidator ID="valUsernameRegex" runat="server" ControlToValidate="txtUsername" Display="None" />
                 <ajaxToolkit:ValidatorCalloutExtender ID="valUsernameRegexEx" runat="server" TargetControlID="valUsernameRegex" EnableViewState="false" />
              </td>
              <td class="data-item" >
                  <asp:Button ID="btnCheck" runat="server" SkinID="SubmitButtonMini" CausesValidation="false" OnClick="btnCheck_Click" meta:resourceKey="btnCheck" />
                  &nbsp; <asp:Label ID="lblCheck" runat="server" EnableViewState="false" ></asp:Label>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrPassword.Text") %></td>
              <td class="data-item" >
                 <asp:TextBox ID="txtPassword" runat="server" SkinID="TextBox" MaxLength="50" ></asp:TextBox>
                 <asp:RequiredFieldValidator ID="valPassword" runat="server" ControlToValidate="txtPassword" Display="None" meta:resourceKey="valPassword" ></asp:RequiredFieldValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valPasswordEx" runat="server" TargetControlID="valPassword" EnableViewState="false" />
                 <asp:RegularExpressionValidator ID="valPasswordRegex" runat="server" ControlToValidate="txtPassword" Display="None" />
                 <ajaxToolkit:ValidatorCalloutExtender ID="valPasswordRegexEx" runat="server" TargetControlID="valPasswordRegex" EnableViewState="false" />
              </td>
              <td></td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrFirstName.Text") %></td>
              <td class="data-item" >
                 <asp:TextBox ID="txtFirstName" runat="server" SkinID="TextBox" MaxLength="50" ></asp:TextBox>
                 <asp:RequiredFieldValidator ID="valFirstName" runat="server" ControlToValidate="txtFirstName" Display="None" meta:resourceKey="valFirstName" ></asp:RequiredFieldValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valFirstNameEX" runat="Server" TargetControlID="valFirstName" EnableViewState="false" />
                 <asp:RegularExpressionValidator ID="valFirstNameRegEx1" runat="server" ControlToValidate="txtFirstName" Display="None" ValidationExpression="[^0-9]+" meta:resourceKey="valFirstNameRegEx1"></asp:RegularExpressionValidator>
                 <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="valFirstNameRegExEx1" TargetControlID="valFirstNameRegEx1" EnableViewState="false" />
              </td>
              <td></td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrLastName.Text") %></td>
              <td class="data-item" >
                 <asp:TextBox ID="txtLastName" runat="server" SkinID="TextBox" MaxLength="50" ></asp:TextBox>
                 <asp:RegularExpressionValidator ID="valLastNameRegEx1" runat="server" ControlToValidate="txtLastName" Display="None" ValidationExpression="[^0-9]+" meta:resourceKey="valLastNameRegEx1"></asp:RegularExpressionValidator>
                 <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="valLastNameRegEx1Ex" TargetControlID="valLastNameRegEx1" EnableViewState="false" />
              </td>
              <td></td>
           </tr>
            <tr>
                 <td class="title" ><%= base.GetLocaleResourceString("ltrEmail.Text") %></td>
                 <td class="data-item" >
                   <asp:TextBox ID="txtEmail" runat="server" SkinID="TextBox" ></asp:TextBox>
                   <asp:RegularExpressionValidator ID="valEmailRegEx" runat="server" ControlToValidate="txtEmail" Display="None" ValidationExpression=".+@.+\..+" meta:resourceKey="valEmailRegEx" ></asp:RegularExpressionValidator>
                   <ajaxToolkit:ValidatorCalloutExtender ID="valEmailRegExEx" runat="server" TargetControlID="valEmailRegEx" EnableViewState="false" />
                 </td>
            </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrPhone.Text") %></td>
              <td class="data-item" >
                 <asp:TextBox ID="txtPhone" runat="server" SkinID="TextBox" MaxLength="18" ></asp:TextBox>
                 <asp:RegularExpressionValidator ID="valPhoneRegEx" runat="server" ControlToValidate="txtPhone" Display="None" ValidationExpression="[0-9 ]{9,18}" meta:resourceKey="valPhoneRegEx"></asp:RegularExpressionValidator>
                 <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="valPhoneRegExEx" TargetControlID="valPhoneRegEx" EnableViewState="false" />
              </td>
              <td></td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrMobile.Text") %></td>
              <td class="data-item" >
                 <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                       <td>
                            <asp:DropDownList ID="ddlMobileArea" runat="server" SkinID="DropDownList" >
                                 <asp:ListItem Text="" Value="" ></asp:ListItem>
                                 <asp:ListItem Text="074" Value="074" ></asp:ListItem>
                                 <asp:ListItem Text="075" Value="075" ></asp:ListItem>
                                 <asp:ListItem Text="077" Value="077" ></asp:ListItem>
                                 <asp:ListItem Text="078" Value="078" ></asp:ListItem>
                                 <asp:ListItem Text="079" Value="079" ></asp:ListItem>
                              </asp:DropDownList>
                       </td>
                       <td style="text-align:right;">
                          <asp:TextBox ID="txtMobile" runat="server" Width="120px" SkinID="TextBox" MaxLength="9" ></asp:TextBox>
                          <asp:RegularExpressionValidator ID="valMobileRegEx" runat="server" ControlToValidate="txtMobile" Display="None" ValidationExpression="([0-9]{5,9})" meta:resourceKey="valMobileRegEx"></asp:RegularExpressionValidator>
                          <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="valMobileRegExEx" TargetControlID="valMobileRegEx" EnableViewState="false" />
                       </td>
                    </tr>
                 </table>
              </td>
              <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
           </tr>
           <tr>
              <td class="title" ></td>
              <td class="data-item" colspan="2" >
                  <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                  <asp:Button ID="btnSave" runat="server" SkinID="SubmitButton" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
              </td>
           </tr>
        </table>
    </asp:Panel>
</asp:Content>