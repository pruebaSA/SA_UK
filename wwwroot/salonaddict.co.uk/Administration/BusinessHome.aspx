<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BusinessHome.aspx.cs" Inherits="SalonAddict.Administration.BusinessHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-customers.png" alt="" />
    Business Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
            Use the links on this page to manage your businesses, their users and their roles.
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="Businesses.aspx" title="Manage businesses.">Manage Businesses</a>
                </div>
                <div class="description">
                    <p>
                        View business details such as contact information, SEO information, and address lists.
                        Assign your business users to specific roles (see role management below).
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="BusinessUsers.aspx" title="Manage business users.">Manage Users</a>
                </div>
                <div class="description">
                    <p>
                        View business user details such as contact information, address lists and linked businesses.
                        Assign your users to specific roles (see role management below).
                    </p>
                </div>
            </li>
            <% if (Roles.IsUserInRole("OWNER") || Roles.IsUserInRole("VP_SALES") || Roles.IsUserInRole("SALES_MGR"))
               { %>
            <li>
                <div class="title">
                    <a href="BusinessUserRoles.aspx" title="Manage business user roles.">Manage User Roles</a>
                </div>
                <div class="description">
                    <p>
                        User roles allow you to group your business users. A user can be a member of
                        more than one role and a role can have many users. 
                    </p>
                </div>
            </li>
            <% } %>
        </ul>
    </div>
</div>
</asp:Content>
