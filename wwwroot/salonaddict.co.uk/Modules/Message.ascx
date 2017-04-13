<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Message.ascx.cs" EnableViewState="false" Inherits="SalonAddict.Modules.Message" %>
<div class="wrapper-message" >
    <% if (!String.IsNullOrEmpty(this.Text))
       { %>
        <asp:Label ID="lblMessage" runat="server" CssClass="message" EnableViewState="false" ></asp:Label>
    <%  }%>
</div>