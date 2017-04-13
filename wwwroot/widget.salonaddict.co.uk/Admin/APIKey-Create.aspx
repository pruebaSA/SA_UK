<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="APIKey-Create.aspx.cs" Inherits="IFRAME.Admin.APIKey_CreatePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>
<%@ Register TagPrefix="IFRM" TagName="DateTextBox" Src="~/Modules/DateTextBox.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" DefaultButton="btnSave" >
       <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Salons" />
       <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:15px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-salons.png" %>' alt="salons" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >

              </td>
           </tr>
        </table>
        <table cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td>
                <asp:Label ID="lblError" runat="server" SkinID="ErrorLabel" EnableViewState="false" ></asp:Label>
                <table class="details" cellpadding="0" cellspacing="0" >
                   <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrSalon.Text") %></td>
                      <td class="data-item" >
                          <asp:Literal ID="ltrSalon" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                  <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrVerificationToken.Text") %></td>
                      <td class="data-item" >
                         <asp:TextBox ID="txtVerificationToken" runat="server" SkinID="TextBox" Enabled="false" MaxLength="50" Width="260px" ></asp:TextBox>
                         <asp:RequiredFieldValidator ID="valVerificationToken" runat="server" ControlToValidate="txtVerificationToken" Display="None" meta:resourceKey="valVerificationToken" ></asp:RequiredFieldValidator>
                         <ajaxToolkit:ValidatorCalloutExtender ID="valVerificationTokenEx" runat="server" TargetControlID="valVerificationToken" EnableViewState="false" />
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrHttpReferer.Text") %></td>
                      <td class="data-item" >
                         <asp:TextBox ID="txtHttpReferer" runat="server" SkinID="TextBox" MaxLength="800" Width="400px" ></asp:TextBox>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ></td>
                      <td class="data-item" >
                          <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                          <asp:Button ID="btnSave" runat="server" SkinID="SubmitButton" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
                      </td>
                   </tr>
                </table>
              </td>
           </tr>
        </table>
     </asp:Panel>
</asp:Content>

