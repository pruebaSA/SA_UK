<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentTopic.ascx.cs" EnableViewState="false" Inherits="SalonAddict.Modules.ContentTopic" %>

<div class="wrapper-contenttopic" >
    <% if(this.ShowHeader)
       { %>
    <div class="title-content">
        <asp:Literal ID="ltrTitle" runat="server" EnableViewState="false" ></asp:Literal>
    </div>
    <div class="clear"></div>
    <% } %>
    <% if(this.ShowBody)
       { %>
    <div class="body-content">
        <asp:Literal ID="ltrBody" runat="server" EnableViewState="false" ></asp:Literal>
    </div>
    <% } %>
</div>