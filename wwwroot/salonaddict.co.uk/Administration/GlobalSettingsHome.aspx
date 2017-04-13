<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="GlobalSettingsHome.aspx.cs" Inherits="SalonAddict.Administration.GlobalSettingsHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-configuration.png" alt="" />
    Global Settings Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
            Use global settings to manage general settings, SEO/Display settings, image
            settings and mail settings.
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="StoreGlobalSettings.aspx" title="Site.">Site</a>
                </div>
                <div class="description">
                    <p>
                        User global settings to manage general site settings.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="BookingGlobalSettings.aspx" title="SalonPortal.">Salon Portal</a>
                </div>
                <div class="description">
                    <p>
                        User global settings to manage general Salon Portal settings.
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
</asp:Content>
