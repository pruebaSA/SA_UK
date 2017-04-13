<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Profile-Email-Edit.aspx.cs" Inherits="IFRAME.SecureArea.Profile_Email_EditPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" DefaultButton="btnSave" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Profile" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-profile.png" %>' alt="profile" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
           </tr>
        </table>
        <table style="margin-top:10px;" class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrEmail.Text") %></td>
              <td class="data-item" >
                  <asp:Literal ID="ltrEmail" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrEmailChange.Text") %></td>
              <td class="data-item" >
                  <asp:TextBox ID="txtEmailChanged" runat="server" SkinID="TextBox" MaxLength="120" ></asp:TextBox>
                  <asp:RequiredFieldValidator ID="valEmailChanged" runat="server" ControlToValidate="txtEmailChanged" Display="None" meta:resourceKey="valEmailChanged" ></asp:RequiredFieldValidator>
                  <ajaxToolkit:ValidatorCalloutExtender ID="valEmailChangedEx" runat="server" TargetControlID="valEmailChanged" EnableViewState="false" />
                  <asp:RegularExpressionValidator ID="valEmailChangedRegEx" runat="server" ControlToValidate="txtEmailChanged" Display="None" ValidationExpression=".+@.+\..+" meta:resourceKey="valEmailChangedRegEx" ></asp:RegularExpressionValidator>
                  <ajaxToolkit:ValidatorCalloutExtender ID="valEmailChangedRegExEx" runat="server" TargetControlID="valEmailChangedRegEx" EnableViewState="false" />
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrEmailConfirm.Text") %></td>
              <td class="data-item" >
                 <asp:TextBox ID="txtEmailConfirm" runat="server" SkinID="TextBox" MaxLength="120" ></asp:TextBox>
                 <asp:RequiredFieldValidator ID="valEmailConfirm" runat="server" ControlToValidate="txtEmailConfirm" Display="None" meta:resourceKey="valEmailConfirm" ></asp:RequiredFieldValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valEmailConfirmEx" runat="server" TargetControlID="valEmailConfirm" EnableViewState="false" />
                 <asp:CompareValidator ID="valEmailConfirmComp" runat="server" ControlToValidate="txtEmailConfirm" ControlToCompare="txtEmailChanged" Operator="Equal" Display="None" meta:resourceKey="valEmailConfirmComp" ></asp:CompareValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valEmailConfirmCompEx" runat="server" TargetControlID="valEmailConfirmComp" EnableViewState="false" />
              </td>
           </tr>
           <tr>
              <td class="title" ></td>
              <td class="data-item" >
                  <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButtonSecure" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                  <asp:Button ID="btnSave" runat="server" SkinID="SubmitButtonSecure" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
              </td>
           </tr>
        </table>
    </asp:Panel>
</asp:Content>
