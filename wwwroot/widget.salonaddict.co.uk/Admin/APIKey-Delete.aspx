<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="APIKey-Delete.aspx.cs" Inherits="IFRAME.Admin.APIKey_DeletePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
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
                <table class="details" cellpadding="0" cellspacing="0" >
                   <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrSalon.Text") %></td>
                      <td class="data-item" >
                          <asp:Literal ID="ltrSalon" runat="server" ></asp:Literal>
                      </td>
                      <td></td>
                   </tr>
                  <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrVerificationToken.Text") %></td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrVerificationToken" runat="server" ></asp:Literal>
                      </td>
                      <td class="data-item" >
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrHttpReferer.Text") %></td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrHttpReferer" runat="server" ></asp:Literal>
                      </td>
                      <td class="data-item" ></td>
                   </tr>
                   <tr>
                      <td class="title" ></td>
                      <td class="data-item" colspan="2" >
                          <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                          <asp:Button ID="btnDelete" runat="server" SkinID="SubmitButton" OnClick="btnDelete_Click" meta:resourceKey="btnDelete" />
                      </td>
                   </tr>
                </table>
              </td>
           </tr>
        </table>
     </asp:Panel>
</asp:Content>