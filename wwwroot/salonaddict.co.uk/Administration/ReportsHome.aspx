<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ReportsHome.aspx.cs" Inherits="SalonAddict.Administration.ReportsHomePage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-system.png" alt="" />
    System Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
            Use the links on this page to view system reports.
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="ServiceAverageReport.aspx" title="View service report.">View Service Report</a>
                </div>
                <div class="description">
                    <p>
                        The service report breaks down the number of orders and revenue for each service category
                        by day, week, month and year. 
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
</asp:Content>
