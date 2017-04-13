<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="SystemHome.aspx.cs" Inherits="SalonAddict.Administration.SystemHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-system.png" alt="" />
    System Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
            Use the links on this page to view system logs and queues.
        </p>
    </div>
    <div class="options">
        <ul>
        <% if (Roles.IsUserInRole("SYS_ADMIN"))
           { %>
            <li>
                <div class="title">
                    <a href="Logs.aspx" title="View system log.">View System Log</a>
                </div>
                <div class="description">
                    <p>
                        The sytem log captures events that may occur whilst your store is online. If
                        you or your customers experience any problems with your store, you can check the
                        system log to see the cause of the problem.
                    </p>
                </div>
            </li>
         <% } %>
            <li>
                <div class="title">
                    <a href="MessageQueue.aspx" title="View message queue.">View Message Queue</a>
                </div>
                <div class="description">
                    <p>
                        Manage the system message queue. View the status of queued emails and all previously
                        sent emails.
                    </p>
                </div>
            </li>
             <li>
                <div class="title">
                    <a href="SMSQueue.aspx" title="View SMS queue.">View SMS Queue</a>
                </div>
                <div class="description">
                    <p>
                        Manage the system SMS queue. View the status of queued SMS and all previously
                        sent SMS messages.
                    </p>
                </div>
            </li>
        <% if (Roles.IsUserInRole("SYS_ADMIN"))
               { %>
            <li>
                <div class="title">
                    <a href="MaintenanceHome.aspx" title="Maintenance Home.">Maintenance</a>
                </div>
                <div class="description">
                    <p>
                        Maintain system clutter to free up disk space.
                    </p>
                </div>
            </li>
         <% } %>
        </ul>
    </div>
</div>
</asp:Content>
