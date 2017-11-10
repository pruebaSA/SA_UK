<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Contact-Us.aspx.cs" Inherits="IFRAME.SecureArea.Contact_UsPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
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
        <table style="margin-top:10px" cellpadding="0" cellspacing="10" >
           <tr>
              <td><%= base.GetLocaleResourceString("ltrAddress.Text") %></td>
              <td>
                 <div style="margin-bottom:5px;">3rd Floor, 207 Regent Street.</div>
                 <div style="margin-bottom:5px;">London.</div>
                 <div style="margin-bottom:5px;">W1B 3HH</div>
                 <div>UK</div>
              </td>
           </tr>
           <tr>
              <td><%= base.GetLocaleResourceString("ltrPhone.Text") %></td>
              <td>+44 (0) 20 8123 4322</td>
           </tr>
        </table>
    </asp:Panel>
</asp:Content>
