<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="SalonPortal.SecureArea.Modules.Menu" %>
<div class="secure-area-menu-module">
    <div class="title">
       <%= base.GetGlobalResourceObject("Modules", "SecureAreaMenu_Title") %>
    </div>
    <% if (SalonPortal.SAContext.Current.User.BusinessID != 0)
       { %>
    <asp:Menu 
        ID="mnuSecureArea" 
        runat="server" 
        CssSelectorClass="secure-area-menu"
        DataSourceID="smdsMenu" 
        Orientation="Vertical" 
        OnMenuItemDataBound="mnuSecureArea_MenuItemDataBound" >
    </asp:Menu>
    <asp:SiteMapDataSource 
        ID="smdsMenu" 
        runat="server" 
        ShowStartingNode="false" 
        SiteMapProvider="SASecureAreaSiteMap" />
    <% } %>
</div>