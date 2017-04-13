<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="PartnershipHome.aspx.cs" Inherits="SalonAddict.Administration.PartnershipHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-partner.png" alt="" />
    Partnership Home
</div>
<div class="homepage">
    <div class="intro">
        <p>
            Use the links on this page to manage your profiles and their white-label accounts.
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="Profiles.aspx" title="Manage Profiles.">Manage Profiles</a>
                </div>
                <div class="description">
                    <p>
                        View partner profile details and assign profiles to specific white-label accounts.
                    </p>
                </div>
            </li>
         </ul>
      </div>
   </div>
</asp:Content>
