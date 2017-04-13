<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="SalesHome.aspx.cs" Inherits="SalonAddict.Administration.SalesHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-sales.png" alt="" />
    Sales Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
            Use the links on this page to manage your sales orders and view sales related reports.
        </p>
    </div>
    <div class="options">
        <ul>
            <% if (Roles.IsUserInRole("OWNER") || Roles.IsUserInRole("VP_SALES") || Roles.IsUserInRole("SALES_MGR") || Roles.IsUserInRole("SALES") || Roles.IsUserInRole("CSR_MGR") || Roles.IsUserInRole("CSR"))
               { %>
            <li>
                <div class="title">
                    <a href="Orders.aspx" title="Manage Orders.">Manage Orders</a>
                </div>
                <div class="description">
                    <p>
                        Manage sales appointments. Capture payments and process open appointments.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="GuestOrders.aspx" title="Manage Guest Orders.">Manage Guest Orders</a>
                </div>
                <div class="description">
                    <p>
                        Manage sales appointments. Capture payments and process open appointments.
                    </p>
                </div>
            </li>
            <% if (Roles.IsUserInRole("OWNER") || Roles.IsUserInRole("VP_SALES") || Roles.IsUserInRole("SALES_MGR") || Roles.IsUserInRole("CSR_MGR") || Roles.IsUserInRole("CSR"))
               { %>
            <li>
                <div class="title">
                    <a href="GiftCards.aspx" title="Manage Gift Cards.">Manage Gift Cards</a>
                </div>
                <div class="description">
                    <p>
                        Manage user gift cards. Gift cards can be used for promotional and/or return management purposes.
                    </p>
                </div>
            </li>
            <% } %>
            <% } %>
            <% if(Roles.IsUserInRole("SYS_ADMIN"))
               { %>
            <li>
                <div class="title">
                    <a href="PaymentHome.aspx" title="Manage Pay.">Manage Payment Settings</a>
                </div>
                <div class="description">
                    <p>
                        Manage general payment settings for your store.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="TaxHome.aspx" title="View sales report.">Manage Tax Settings</a>
                </div>
                <div class="description">
                    <p>
                        Manage general tax settings for your store.
                    </p>
                </div>
            </li>
            <% } %>
            <% if (Roles.IsUserInRole("OWNER") || Roles.IsUserInRole("VP_SALES"))
               { %>
            <li>
                <div class="title">
                    <a href="Coupons.aspx" title="Manage coupons.">Coupons</a>
                </div>
                <div class="description">
                    <p>
                        Counpons can be used to deduct a set value or percentage from the total value of an 
                        order. You can set limitations and a date range to control when they are no longer valid.
                    </p>
                </div>
            </li>
            <% } %>
            
            <% if (Roles.IsUserInRole("SYS_ADMIN"))
               { %>
            <li>
                <div class="title">
                    <a href="Discounts.aspx" title="Manage Discounts.">Discounts</a>
                </div>
                <div class="description">
                    <p>
                        Discounts can be used to deduct a set value or percentage from individual items or the total value of an 
                        order. You can set a date range for a discount as well as generating coupon codes for your customers. 
                        Discounts can also be restricted to a specific business, allowing you to reward your higher 
                        value customers.
                    </p>
                </div>
            </li>
            <% } %>
        </ul>
    </div>
</div>
</asp:Content>
