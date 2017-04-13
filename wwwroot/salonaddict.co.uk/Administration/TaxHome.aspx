<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="TaxHome.aspx.cs" Inherits="SalonAddict.Administration.TaxHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-sales.png" alt="" />
    Tax Settings Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
           Use the links on this page to manage tax settings for your store.
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="TaxProviders.aspx" title="Manage Tax Providers.">Manage Tax Providers </a>
                </div>
                <div class="description">
                    <p>
                        Tax providers are used to calculate tax on customer orders. There are a number of
                        different providers that can be used, each with it's own configuration options,
                        such as tax by location.
                    </p>
                </div>
            </li>
           <li>
                <div class="title">
                    <a href="TaxCategories.aspx" title="Manage Tax Categories.">Manage Tax Categories </a>
                </div>
                <div class="description">
                    <p>
                        Tax categories are used to calculate tax on customer orders by a specific provider.
                    </p>
                </div>
            </li>
           <li>
                <div class="title">
                    <a href="TaxRates.aspx" title="Manage Tax Rates.">Manage Tax Rates </a>
                </div>
                <div class="description">
                    <p>
                        Tax rates define the amount of tax to apply to each item within a customer order. 
                        Multiple tax rates are supported, each with it's own configuration options,
                        to become effective on a particular date.
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
</asp:Content>
