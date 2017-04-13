<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="SalonAddict.Administration.Templates.Header" %>
<div class="header-template" >
    <a class="logo" href="<%= Page.ResolveUrl("~/Administration/") %>" ></a>
    <div id="header-links" >
        <a href="<%= Page.ResolveUrl("~/") %>" >Store</a> 
        &nbsp;|&nbsp;
        <% if (Roles.IsUserInRole("SYS_ADMIN"))
           { %>
        <asp:LinkButton ID="lbClearCache" runat="server" Text="Clear Cache" onclick="lbClearCache_Click" CausesValidation="false" ></asp:LinkButton>
        &nbsp;|&nbsp;
        <% } %>
        <a href="<%= Page.ResolveUrl("~/Logout.aspx") %>" >Logout</a>
    </div>
</div>