﻿<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="SettingCompany-Edit.aspx.cs" Inherits="IFRAME.SecureArea.SettingCompany_EditPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" DefaultButton="btnSave" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Settings" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-settings.png" %>' alt="settings" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >

              </td>
           </tr>
        </table>
        <table style="margin-top:10px" class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrCompany.Text") %></td>
              <td class="data-item" >
                 <asp:TextBox ID="txtCompany" runat="server" SkinID="TextBox" MaxLength="80" ></asp:TextBox>
                 <asp:RequiredFieldValidator ID="valCompany" runat="server" ControlToValidate="txtCompany" Display="None" meta:resourceKey="valCompany" ></asp:RequiredFieldValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valCompanyEx" runat="server" TargetControlID="valCompany" EnableViewState="false" />
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
                  <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButtonSecure" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                  <asp:Button ID="btnSave" runat="server" SkinID="SubmitButtonSecure" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
                  <p>
                    <asp:Label ID="lblError" runat="server" EnableViewState="false" SkinID="ErrorLabel" ></asp:Label>
                  </p>
              </td>
           </tr>
        </table>
    </asp:Panel>
</asp:Content>
