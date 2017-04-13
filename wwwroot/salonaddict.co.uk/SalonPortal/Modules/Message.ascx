<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Message.ascx.cs" Inherits="SalonPortal.Modules.Message" %>
<span class="message-module" >
    <% if (!String.IsNullOrEmpty(this.Text))
       { %>
        <asp:Label ID="lblMessage" runat="server" EnableViewState="false" CssClass="message" ></asp:Label>
        <% if (this.Auto)
           { %>
        <script type="text/javascript" language="javascript" >
            $("#<%= lblMessage.ClientID %>").fadeOut(8000);
        </script>
        <% } %>
        
    <%  }%>
</span>