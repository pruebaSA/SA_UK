<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Closed.aspx.cs" Inherits="IFRAME.ClosedPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <h1><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1>
       <p>
         <%= base.GetLocaleResourceString("ltrParagraphOne.Text") %>
      </p>
      <p>&nbsp;</p>
      <center>
         <a href='<%= IFRMHelper.GetURL(Page.ResolveUrl("~/")) %>' ><u><%= base.GetLocaleResourceString("hlChangeSearch.Text") %></u></a>
      </center>
    </asp:Panel>
</asp:Content>
