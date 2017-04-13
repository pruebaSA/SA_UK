<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ConfigurationHome.aspx.cs" Inherits="SalonAddict.Administration.ConfigurationHome" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-title">
    <img src="images/ico-configuration.png" alt="" />
    Configuration Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
            Use the links on this page to manage various storefront settings including security,
            display/SEO, location and currencies.
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="StoreGlobalSettings.aspx" title="Global Settings.">Global Settings</a>
                </div>
                <div class="description">
                    <p>
                        Manage common settings.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="Blacklist.aspx" title="Blacklist.">Blacklist</a>
                </div>
                <div class="description">
                    <p>
                        Use blacklist to manage general store security settings. Manage various groups or individual ipaddresses
                        that can access the store.
                    </p>
                </div>
            </li>
           <li>
                <div class="title">
                    <a href="Currencies.aspx" title="Currencies.">Currencies</a>
                </div>
                <div class="description">
                    <p>
                        Use currencies to manage primary exchange rates, primary store currencies and forex exchange rates.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="Languages.aspx" title="Languages.">Languages</a>
                </div>
                <div class="description">
                    <p>
                        Manage various language and culture settings for your store.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="LocationManagementHome.aspx" title="Location Management.">Manage Locations</a>
                </div>
                <div class="description">
                    <p>
                        Manage locations such as countries, state/provinces, counties, cities, and areas(regions/districts).
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="Settings.aspx" title="All settings.">All Settings</a>
                </div>
                <div class="description">
                    <p>
                        View all the settings for your store.
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
</asp:Content>
