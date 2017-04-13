<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="SalonPortal.Templates.Footer" %>
<div class="footer-template" >
    <ul class="quick-link">
      <li>
         <a href="<%= Page.ResolveUrl("~/default.aspx") %>" ><%= base.GetGlobalResourceObject("Modules", "Footer_Link_Home") %></a>
      </li>
      <li>|</li>
      <li>
         <a href="<%= Page.ResolveUrl("~/privacy-policy.aspx") %>" ><%= base.GetGlobalResourceObject("Modules", "Footer_Link_Privacy")%></a>
      </li>
      <li>|</li>
      <li>
        <a href="<%= Page.ResolveUrl("~/terms-and-conditions.aspx") %>" ><%= base.GetGlobalResourceObject("Modules", "Footer_Link_Terms")%></a> 
      </li>
      <% if (SalonPortal.SAContext.Current.User != null)
         { %>
      <li>|</li>
      <li>
        <a href="<%= Page.ResolveUrl("~/SecureArea/SupportTicket.aspx") %>" ><%= base.GetGlobalResourceObject("Modules", "Footer_Link_Contact")%></a> 
      </li>
      <% } %>
   </ul>
</div>