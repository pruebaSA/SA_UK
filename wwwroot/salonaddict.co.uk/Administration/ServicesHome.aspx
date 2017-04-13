<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ServicesHome.aspx.cs" Inherits="SalonAddict.Administration.ServicesHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-catalog.png" alt="" />
    Services Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
            Use the links on this page to manage all aspects of your catalog such as
            products, services, categories, and other attributes.
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="ServiceTypes.aspx" title="Manage Service types">Service Types</a>
                </div>
                <div class="description">
                    <p>
                        Service types are the hierarchical grouping for the services that define the business. 
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="ServiceCategories.aspx" title="Manage Service Categories">Service Categories</a>
                </div>
                <div class="description">
                    <p>
                        Service categories are the hierarchical grouping for the services in a business' catalog. 
                        They enable customers to find what they are looking for.
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
</asp:Content>
