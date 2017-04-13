<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Topic.ascx.cs" Inherits="SalonPortal.Modules.Topic" %>
<div class="topic-module" >
    <div class="htmlcontent">
        <% if(this.ShowHeader)
           { %>
        <div class="htmlcontent-title">
            <h2 class="htmlcontent-header">
                <asp:Literal ID="ltrTitle" runat="server" ></asp:Literal>
            </h2>
        </div>
        <div class="clear">
        </div>
        <% } %>
        <div class="htmlcontent-body">
            <asp:Label ID="ltrBody" runat="server" ></asp:Label>
        </div>
    </div>
</div>