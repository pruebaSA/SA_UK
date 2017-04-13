<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="LocationManagementHome.aspx.cs" Inherits="SalonAddict.Administration.LocationManagementHome" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-title">
    <img src="images/ico-configuration.png" alt="" />
    Location Management Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
            Use the links on this page to manage location settings for your store such as Countries,
            State/Provinces, Counties, Cities/Towns and Areas.
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="Countries.aspx" title="Manage countries.">Manage Countries</a>
                </div>
                <div class="description">
                    <p>
                        Manage the countries that are enabled in your store. 
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="StateProvinces.aspx" title="Manage states / provinces.">Manage States / Provinces</a>
                </div>
                <div class="description">
                    <p>
                        Manage the states / provinces of countries in your store.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="Counties.aspx" title="Counties.">Manage Counties</a>
                </div>
                <div class="description">
                    <p>
                        Manage the counties in your store.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="CityTowns.aspx" title="Cities/Towns.">Manage Cities/Towns</a>
                </div>
                <div class="description">
                    <p>
                        Manage the cities/towns in your store.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="Areas.aspx" title="Areas.">Manage Areas</a>
                </div>
                <div class="description">
                    <p>
                        Manage the areas in your store. Areas can be defined to group counties and/or cities/towns.
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
</asp:Content>
