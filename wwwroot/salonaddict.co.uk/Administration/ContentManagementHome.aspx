<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ContentManagementHome.aspx.cs" Inherits="SalonAddict.Administration.ContentManagementHome" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-title">
    <img src="images/ico-content.png" alt="" />
    Content Management Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
            Use the links on this page to manage the dynamic content on your site such as topics, message templates and localization resources.
        </p>
    </div>
    <div class="options">
        <ul>
        <% if (Roles.IsUserInRole("OWNER") || Roles.IsUserInRole("VP_SALES") || Roles.IsUserInRole("SALES_MGR"))
               { %>
            <li>
                <div class="title">
                    <a href="Promotions.aspx" title="Manage promotions">Manage Quick Searches</a>
                </div>
                <div class="description">
                    <p>
                        Manage quick searches. Quick searches are predefined search criteria 
                        used to promote one-click user experience methodology.
                    </p>
                </div>
            </li>
            <% } %>
            <% if (Roles.IsUserInRole("OWNER") || Roles.IsUserInRole("MRK_MGR"))
               { %>
            <li>
                <div class="title">
                    <a href="Promotions.aspx" title="Manage promotions">Manage Promotions</a>
                </div>
                <div class="description">
                    <p>
                        Manage site promotions. Promotions are similar to quick searches with the exception that
                        they must be promotional and specific to a bookable service.
                    </p>
                </div>
            </li>
            <% } %>
            <% if (Roles.IsUserInRole("SYS_ADMIN"))
               { %>
            <li>
                <div class="title">
                    <a href="Topics.aspx" title="Manage topics">Manage Topics</a>
                </div>
                <div class="description">
                    <p>
                        Manage topics.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="TemplatesHome.aspx" title="Manage templates">Manage Templates</a>
                </div>
                <div class="description">
                    <p>
                        Manage templates for product, category and manufacturer pages. Also manage email
                        templates for the languages enabled in your store.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="LocaleStringResources.aspx" title="Manage localization resources">Manage Localization</a>
                </div>
                <div class="description">
                    <p>
                        Manage localized content in your store. This allows customers of different languages
                        to understand your content.
                    </p>
                </div>
            </li>
            <% } %>
        </ul>
    </div>
</div>
</asp:Content>
