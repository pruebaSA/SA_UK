<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="CatalogHome.aspx.cs" Inherits="SalonAddict.Administration.CatalogHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-catalog.png" alt="" />
    Catalog Home
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
                    <a href="ServicesHome.aspx" title="Manage Services">Services</a>
                </div>
                <div class="description">
                    <p>
                        Manage services types, categories and definitions.
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
</asp:Content>
