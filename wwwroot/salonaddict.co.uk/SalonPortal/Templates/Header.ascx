<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="SalonPortal.Templates.Header" %>
<%@ Import Namespace="SalonAddict.Common" %>
<div class="header-template" >
    <a href="<%= Page.ResolveUrl("~/") %>" class="logo" ></a>
    <% if (HttpContext.Current.User.Identity.IsAuthenticated)
       { %>
        <div class="datetime" >
           <%=  DateTime.Now.ToFriendlyTimeFormat(SalonPortal.SAHttpApplication.DefaultTimeZone).ToString("f")%>
        </div>
    <% } %>
</div>