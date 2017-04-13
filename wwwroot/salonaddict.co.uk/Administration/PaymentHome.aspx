<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="PaymentHome.aspx.cs" Inherits="SalonAddict.Administration.PaymentHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-sales.png" alt="" />
    Payment Settings Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
           Use the links on this page to manage payment settings for your store.
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="PaymentProviders.aspx" title="Manage Payment Providers.">Manage Payment Providers </a>
                </div>
                <div class="description">
                    <p>
                        Payment providers are used to calculate prices on customer orders. There are a number of
                        different providers that can be used, each with it's own configuration options.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="PaymentMethods.aspx" title="Manage Payment Methods.">Manage Payment Methods</a>
                </div>
                <div class="description">
                    <p>
                        Payment methods available for a customer to make a payment.
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
</asp:Content>
