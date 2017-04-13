<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="MarketingHome.aspx.cs" Inherits="SalonAddict.Administration.MarketingHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-marketing.gif" alt="" />
    Marketing Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
            Use the links on this page to manage your leads, opportunities and customer reviews.
        </p>
    </div>
    <div class="options">
        <ul>
         <% if (Roles.IsUserInRole("SYS_ADMIN"))
               { %>
            <li>
                <div class="title">
                    <a href="SMSProviders.aspx" title="Manage SMS providers.">Manage SMS providers </a>
                </div>
                <div class="description">
                    <p>
                        Manage and configure new or existing SMS providers.
                    </p>
                </div>
            </li>
            <% } %>
        <% if (Roles.IsUserInRole("OWNER") || Roles.IsUserInRole("VP_SALES") || Roles.IsUserInRole("SALES_MGR") || Roles.IsUserInRole("SALES"))
               { %>
            <li>
                <div class="title">
                    <a href="Leads.aspx" title="Manage leads.">Manage Leads </a>
                </div>
                <div class="description">
                    <p>
                        Manage incoming information to prospects, escalate leads to opportunities and create new business accounts.
                    </p>
                </div>
            </li>
            <% } %>
            <% if (Roles.IsUserInRole("OWNER") || Roles.IsUserInRole("MRK_MGR") || Roles.IsUserInRole("MRK"))
               { %>
            <li>
                <div class="title">
                    <a href="ReviewSessions.aspx" title="View review sessions.">Manage Review Sessions</a>
                </div>
                <div class="description">
                    <p>
                        View a report of pending review sessions.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="BusinessReviews.aspx" title="View business reviews.">Manage Business Reviews</a>
                </div>
                <div class="description">
                    <p>
                        View a report of all business customer reviews.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="StaffReviews.aspx" title="View staff reviews.">Manage Staff Reviews</a>
                </div>
                <div class="description">
                    <p>
                        View a report of all staff customer reviews.
                    </p>
                </div>
            </li>
            <% } %>
        </ul>
    </div>
</div>
</asp:Content>
