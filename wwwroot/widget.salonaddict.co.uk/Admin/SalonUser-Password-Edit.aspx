<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="SalonUser-Password-Edit.aspx.cs" Inherits="IFRAME.Admin.SalonUser_Password_EditPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" DefaultButton="btnSave" >
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
              <td class="title" ><%= base.GetLocaleResourceString("ltrUsername.Text") %></td>
              <td class="data-item" >
                  <asp:Literal ID="ltrUsername" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrSalon.Text") %></td>
              <td class="data-item" >
                  <asp:Literal ID="ltrSalon" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrPassword.Text") %></td>
              <td class="data-item" >
                 <asp:TextBox ID="txtPassword" runat="server" SkinID="TextBox" TextMode="Password" MaxLength="50" ></asp:TextBox>
                 <asp:RequiredFieldValidator ID="valPassword" runat="server" ControlToValidate="txtPassword" Display="None" meta:resourceKey="valPassword" ></asp:RequiredFieldValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valPasswordEx" runat="Server" TargetControlID="valPassword" EnableViewState="false" />
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrPasswordChange.Text") %></td>
              <td class="data-item" >
                  <asp:TextBox ID="txtPasswordChange" runat="server" SkinID="TextBox" TextMode="Password" MaxLength="50" ></asp:TextBox>
                  <asp:RequiredFieldValidator ID="valPasswordChange" runat="server" ControlToValidate="txtPasswordChange" Display="None" meta:resourceKey="valPasswordChange" ></asp:RequiredFieldValidator>
                  <ajaxToolkit:ValidatorCalloutExtender ID="valPasswordChangeEx" runat="Server" TargetControlID="valPasswordChange" EnableViewState="false" />
                  <asp:RegularExpressionValidator ID="valPasswordChangeRegEx" runat="server" ControlToValidate="txtPasswordChange" Display="None" ValidationExpression="^.{5,}$" meta:resourceKey="valPasswordChangeRegEx" ></asp:RegularExpressionValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valPasswordChangeRegExEx" runat="server" TargetControlID="valPasswordChangeRegEx" EnableViewState="false" />
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrPasswordConfirm.Text") %></td>
              <td class="data-item" >
                  <asp:TextBox ID="txtPasswordConfirm" runat="server" SkinID="TextBox" TextMode="Password" MaxLength="50" ></asp:TextBox>
                  <asp:RequiredFieldValidator ID="valPasswordConfirm" runat="server" ControlToValidate="txtPasswordConfirm" Display="None" meta:resourceKey="valPasswordConfirm" ></asp:RequiredFieldValidator>
                  <ajaxToolkit:ValidatorCalloutExtender ID="valPasswordConfirmEx" runat="server" TargetControlID="valPasswordConfirm" EnableViewState="false" />
                  <asp:CompareValidator ID="valPasswordConfirmComp" runat="server" ControlToValidate="txtPasswordConfirm" ControlToCompare="txtPasswordChange" Operator="Equal" Display="None" meta:resourceKey="valPasswordConfirmComp" ></asp:CompareValidator>
                  <ajaxToolkit:ValidatorCalloutExtender ID="valPasswordConfirmCompEx" runat="server" TargetControlID="valPasswordConfirmComp" EnableViewState="false" />
              </td>
           </tr>
           <tr>
              <td class="title" ></td>
              <td class="data-item" >
                  <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                  <asp:Button ID="btnSave" runat="server" SkinID="SubmitButton" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
                  <p>
                    <asp:Label ID="lblError" runat="server" EnableViewState="false" SkinID="ErrorLabel" ></asp:Label>
                  </p>
              </td>
           </tr>
        </table>
     </asp:Panel>
</asp:Content>
