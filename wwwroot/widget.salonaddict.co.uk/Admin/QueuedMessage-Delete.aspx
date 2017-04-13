<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="QueuedMessage-Delete.aspx.cs" Inherits="IFRAME.Admin.QueuedMessage_DeletePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
<asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Reports" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-reports.png" %>' alt="reports" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
              
              </td>
           </tr>
        </table>
        <table class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrFrom.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrFrom" runat="server" EnableViewState="false" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrTo.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrTo" runat="server" EnableViewState="false" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrSubject.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrSubject" runat="server" EnableViewState="false" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrSendTries.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrSendTries" runat="server" EnableViewState="false" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrCreatedOn.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrCreatedOn" runat="server" EnableViewState="false" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ></td>
              <td class="data-item" colspan="2" >
                  <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                  <asp:Button ID="btnDelete" runat="server" SkinID="SubmitButton" UseSubmitBehavior="false" OnClick="btnDelete_Click" meta:resourceKey="btnDelete" />
              </td>
           </tr>
        </table>
</asp:Panel>
</asp:Content>
