<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="SalonPaymentMethod-Activate.aspx.cs" Inherits="IFRAME.Admin.SalonPaymentMethod_ActivatePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
<asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Billing" />
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
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrCard.Text") %></td>
              <td class="data-item" >
                 <asp:Literal ID="ltrCard" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrCardNumber.Text") %></td>
              <td class="data-item" >
                 <asp:Literal ID="ltrCardNumber" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrExpiry.Text") %></td>
              <td class="data-item" >
                 <asp:Literal ID="ltrExpiry" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ></td>
              <td class="data-item" colspan="2" >
                  <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                  <asp:Button ID="btnActivate" runat="server" SkinID="SubmitButton" OnClick="btnActivate_Click" meta:resourceKey="btnActivate" />
              </td>
           </tr>
        </table>
</asp:Panel>
</asp:Content>
