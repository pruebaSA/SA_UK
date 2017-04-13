<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="SalonPortal.Modules.Menu" %>
<!--[if lt IE 7]>
    <style type="text/css" >
        .menu-module ul li a:hover
        {
            background-image:none;
        }
    </style>
<![endif]-->
<div class="menu-module">
    <ul>
       <% if (Request.Url.LocalPath.ToLowerInvariant() == "/default.aspx" ||
              Request.Url.LocalPath.ToLowerInvariant() == "/securearea/default.aspx" ||
              Request.Url.LocalPath.ToLowerInvariant() == "/securearea/dashboard.aspx" ||
              Request.Url.LocalPath.ToLowerInvariant() == "/securearea/appointments.aspx" ||
              Request.Url.LocalPath.ToLowerInvariant() == "/login.aspx")
          { %>
       <li class="selected" >
       <% }
          else
          { %>
       <li>
       <% } %>
          <a href="<%= Page.ResolveUrl("~/default.aspx") %>" >
             <%= base.GetGlobalResourceObject("Modules", "Menu_Tab_Home") %>
          </a>
       </li>
    </ul>
</div>