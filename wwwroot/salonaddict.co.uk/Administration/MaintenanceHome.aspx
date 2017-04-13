<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="MaintenanceHome.aspx.cs" Inherits="SalonAddict.Administration.MaintenanceHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-system.png" alt="" />
    Maintenance Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
            Use the links on this page to purge clutter from the system.
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="PurgeGuestOrders.aspx" title="Purge guest orders.">Purge Guest Orders</a>
                </div>
                <div class="description">
                    <p>
                        This feature allows you to purge any guest orders to free up disk space.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="PurgeLeads.aspx" title="Purge leads.">Purge Leads</a>
                </div>
                <div class="description">
                    <p>
                        This feature allows you to purge leads to free up disk space.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="PurgeOrphanedPictures.aspx" title="Purge pictures.">Purge Pictures</a>
                </div>
                <div class="description">
                    <p>
                        This feature allows you to purge orphaned pictures to free up disk space.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="PurgeOrphanedOrders.aspx" title="Purge orphaned orders.">Purge Orphaned Orders</a>
                </div>
                <div class="description">
                    <p>
                        This feature allows you to purge any orphaned orders to free up disk space.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="PurgeStaffAvailability.aspx" title="Purge staff availability.">Purge Staff Availability</a>
                </div>
                <div class="description">
                    <p>
                        This feature allows you to purge staff availability no longer in use to free up disk space.
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
</asp:Content>
