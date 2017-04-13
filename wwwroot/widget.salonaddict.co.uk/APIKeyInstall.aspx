<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="APIKeyInstall.aspx.cs" Inherits="IFRAME.APIKeyInstallPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
   <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" EnableViewState="false" >
       <asp:MultiView ID="mv" runat="server" ActiveViewIndex="2" >
          <asp:View ID="v0" runat="server" >
              <h1><%= base.GetLocaleResourceString("v0.Header.Text") %></h1>
              <p><%= base.GetLocaleResourceString("v0.Content.Text") %></p>
          </asp:View>
          <asp:View ID="v1" runat="server" >
              <h1><%= base.GetLocaleResourceString("v1.Header.Text") %></h1>
              <p><%= base.GetLocaleResourceString("v1.Content.Text") %></p>
          </asp:View>
          <asp:View ID="v2" runat="server" >
              <h1><%= base.GetLocaleResourceString("v2.Header.Text") %></h1>
              <p><%= base.GetLocaleResourceString("v2.Content.Text") %></p>
          </asp:View>
       </asp:MultiView>
   </asp:Panel>
</asp:Content>
