<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BusinessAvailabilityOne.ascx.cs" Inherits="SalonAddict.Templates.BusinessAvailabilityOne" %>
<div class="template-businessavailabilityone" >
    <div class="logo" >
        <asp:HyperLink ID="Logo" runat="server" ></asp:HyperLink>
    </div>
    <div class="name" >
        <asp:HyperLink ID="Name" runat="server" ></asp:HyperLink>
    </div>
    <div class="location" >
        <asp:Literal ID="Location" runat="server" ></asp:Literal>
    </div>
    <div class="description" >
        <asp:Literal ID="Description" runat="server" ></asp:Literal>
    </div>
    <div class="rating" >
        <asp:HyperLink ID="Rating" runat="server" ></asp:HyperLink>
    </div>
    <div class="price" >
        <asp:Literal ID="lblPrice" runat="server" ></asp:Literal>
        <asp:Literal ID="Price" runat="server" ></asp:Literal>
    </div>
    <div class="button" >
        <asp:HyperLink ID="Submit" runat="server" ></asp:HyperLink>
    </div>
</div>