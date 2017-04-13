<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Appointment-Cancel.aspx.cs" Inherits="IFRAME.SecureArea.Appointment_CancelPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Appointments" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-appointments.png" %>' alt="appointments" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
              </td>
           </tr>
        </table>
       <table class="details" cellspacing="0" cellpadding="0" >
          <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrConfirmationNo.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrConfirmationNo" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrClient.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrClient" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                  <%= base.GetLocaleResourceString("ltrService.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrService" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                  <%= base.GetLocaleResourceString("ltrEmployee.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrEmployee" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                  <%= base.GetLocaleResourceString("ltrDate.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrDate" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                  <%= base.GetLocaleResourceString("ltrTime.Text") %>
              </td>
              <td class="data-item" >
                  <asp:Literal ID="ltrTime" runat="server" ></asp:Literal>
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
     </asp:Panel>
</asp:Content>
