<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="TemplatesHome.aspx.cs" Inherits="SalonAddict.Administration.TemplatesHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-content.png" alt="" />
    Templates Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
            Use the links on this page to manage the various content templates used in your store.
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="SMSTemplates.aspx" title="Manage SMS message templates" >Manage SMS Templates</a>
                </div>
                <div class="description">
                    <p>
                        SMS message templates provide localized SMS output to the languages enabled.
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="MessageTemplates.aspx" title="Manage message templates" >Manage Message Templates</a>
                </div>
                <div class="description">
                    <p>
                        Message templates provide localized email output to the languages enabled.
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
</asp:Content>
