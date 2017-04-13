<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Navbar.ascx.cs" Inherits="SalonAddict.Administration.Modules.Navbar" %>
<%@ Import Namespace="SalonAddict.Common" %>

<div class="navbar-module">
    <div class="date-time">
        <%=  DateTime.Now.ToFriendlyTimeFormat(TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")).ToString("f")%>
    </div>
    <div class="breadcrumb" >
        <asp:SiteMapPath ID="smp" runat="server" RenderCurrentNodeAsLink="true" PathSeparatorStyle-Font-Bold="true"/>
    </div>
</div>