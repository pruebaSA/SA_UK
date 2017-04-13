<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="SettingVAT-Edit.aspx.cs" Inherits="IFRAME.Admin.SettingVAT_EditPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" DefaultButton="btnSave" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Settings" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-settings.png" %>' alt="settings" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >

              </td>
           </tr>
        </table>
        <table style="margin-top:10px" class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrVAT.Text") %></td>
              <td class="data-item" >
                 <asp:TextBox ID="txtVAT" runat="server" SkinID="TextBox" MaxLength="80" ></asp:TextBox>
                 <asp:RequiredFieldValidator ID="valVAT" runat="server" ControlToValidate="txtVAT" Display="None" meta:resourceKey="valVAT" ></asp:RequiredFieldValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valVATEx" runat="server" TargetControlID="valVAT" EnableViewState="false" />
                 <asp:RegularExpressionValidator ID="valVATRegex1" runat="server" ControlToValidate="txtVAT" Display="None" ValidationExpression="(GB)(\s*)([0-9\s]{9,14})" meta:resourceKey="valVATRegex1"></asp:RegularExpressionValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valVATRegex1Ex" runat="Server" TargetControlID="valVATRegex1" EnableViewState="false" />
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
