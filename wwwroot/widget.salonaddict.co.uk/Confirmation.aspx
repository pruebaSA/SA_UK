<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Confirmation.aspx.cs" Inherits="IFRAME.ConfirmationPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
   <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
      <div style="position:relative;" >
         <h1 style="margin-bottom:35px;" ><asp:Literal ID="ltrHeader" runat="server" EnableViewState="false" ></asp:Literal></h1>
         <div style="position:absolute;top:26px;left:0px;font-size:11px;" >
            <asp:Literal ID="ltrAddress" runat="server" EnableViewState="false" ></asp:Literal>
         </div>
      </div>
      <asp:Panel ID="mini" runat="server" SkinID="MiniBoxPanel" >
         <h2>
            <asp:Label ID="lblService" runat="server" EnableViewState="false" ></asp:Label>
            <asp:Label ID="lblEmployee" runat="server" EnableViewState="false" ></asp:Label>
            <asp:Label ID="lblDate" runat="server" EnableViewState="false" Font-Bold="false" ></asp:Label>
         </h2>
         <p>
            <%= base.GetLocaleResourceString("ltrConfirmation.Text") %>
         </p>
      </asp:Panel>
      <p>&nbsp;</p>
      <table class="details" cellpadding="0" cellspacing="0" width="100%" >
         <tr>
            <td class="title" ></td>
            <td style="text-align:right" class="data-item" >
               <a href='<%= IFRMHelper.GetURL(Page.ResolveUrl("~/")) %>' ><u><%= base.GetLocaleResourceString("hlChangeSearch.Text") %></u></a>
            </td>
         </tr>
      </table>
   </asp:Panel>
</asp:Content>
