<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="CustomersHome.aspx.cs" Inherits="SalonAddict.Administration.CustomersHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-customers.png" alt="" />
    Customers Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
            Use the links on this page to manage your customers and their roles.
        </p>
    </div>
    <div class="options">
        <ul>
        <% if (Roles.IsUserInRole("SYS_ADMIN"))
           { %>
            <li>
                <div class="title">
                    <a href="Customers.aspx" title="Manage customers.">Manage Customers</a>
                </div>
                <div class="description">
                    <p>
                        View customer details such as contact information, address lists and past orders.
                        Assign your customers to specific roles (see role management below).
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="CustomerRoles.aspx" title="Manage customer roles.">Manage Customer Roles</a>
                </div>
                <div class="description">
                    <p>
                        Customer roles allow you to group your customers. A customer can be a member of
                        more than one role and a role can have many customers. 
                    </p>
                </div>
            </li>
        <% } %>
            <li>
                <div class="title">
                    <a href="BusinessHome.aspx" title="Manage businesses.">Manage Businesses</a>
                </div>
                <div class="description">
                    <p>
                        View business details such as contact information, address lists and business users.
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
</asp:Content>
