<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="WSPNote-Delete.aspx.cs" Inherits="IFRAME.Admin.WSPNote_DeletePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Account" />
       <div class="horizontal-line" ></div>
        <table style="margin-bottom:20px" cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:60px" ><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-salons.png" %>' alt="salons" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle;text-align:right;" >
              
              </td>
           </tr>
        </table>
        <table class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td class="title" >
                 Note:
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrNote" runat="server" EnableViewState="false" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 Created:
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrCreatedOn" runat="server" EnableViewState="false" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ></td>
              <td class="data-item" >
                  <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                  <asp:Button ID="btnDelete" runat="server" SkinID="SubmitButton" OnClick="btnDelete_Click" meta:resourceKey="btnDelete" />
              </td>
           </tr>
        </table>
    </asp:Panel>
</asp:Content>
