<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="SalonAddict.Administration.Modules.Menu" %>
<div class="menu-module">
    <asp:Menu 
        ID="mnuAdmin" 
        runat="server" 
        CssSelectorClass="AdminMenu"
        DataSourceID="smdsMenu" 
        Orientation="Horizontal" 
        OnMenuItemDataBound="mnuAdmin_MenuItemDataBound" >
    </asp:Menu>
    <asp:SiteMapDataSource 
        ID="smdsMenu" 
        runat="server" 
        ShowStartingNode="false" 
        SiteMapProvider="SAAdminXmlSiteMapProvider"  />
</div>