<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="Map.aspx.cs" Inherits="SalonAddict.MapPage" %>
<%@ Register TagPrefix="SA" TagName="SearchPanel" Src="~/Templates/SearchPanelTwo.ascx" %>
<%@ Register TagPrefix="SA" TagName="SalonMenu" Src="~/Modules/SalonMenu.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/TwoColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TwoColumnSideContentPlaceHolder" runat="server">
   <SA:SearchPanel ID="SearchPanel" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TwoColumnContentPlaceHolder" runat="server">
<style type="text/css" >
   #wrapper-book { position:relative; }
   #wrapper-book .wrapper-sortmenu { position:relative; height:38px; }
   #wrapper-book .wrapper-sortmenu .history { position:absolute; top:12px; left:20px; font-size:11px; }
   #wrapper-book .wrapper-sortmenu .tempate-sortmenutwo { position:absolute; right:0px; top:0px; width:500px; }
   #wrapper-book #page-content { position:relative; margin-top:10px; margin-left:5px; margin-bottom:10px; padding:9px; border:solid 1px #eee; }
   #wrapper-book #page-content .saloninfo { position:relative; height:130px; }
   #wrapper-book #page-content .saloninfo .name { position:absolute; top:5px; left:0px; font-size:17px; font-weight:bold; }
   #wrapper-book #page-content .saloninfo .location { position:absolute; top:22px; left:0px; font-size:12px; }
   #wrapper-book #page-content .saloninfo .logo { position:absolute; top:0px; right:5px; padding:5px; border:solid 1px #eee; }
   #wrapper-book #page-content .saloninfo .options { position:absolute; top:65px; left:0px; }
   #wrapper-book .search-text { position:relative; font-family:Arial; font-size:11px; font-weight:normal; margin-bottom:5px;  }
   #wrapper-book .wrapper-nextprevious { position:absolute; right:0px; top:0px; }
   #wrapper-book #page-content .review-item { padding:10px; margin:10px; background:#f7f7f7; }
   #wrapper-book #page-content .review-item .rating { margin-bottom:5px; font-weight:bold; }
   #wrapper-book #page-content .review-item .description { margin-bottom:5px; }
   #wrapper-book #page-content .review-item .signature { font-size:11px; }
</style>
<div id="wrapper-book" >
<div class="wrapper-sortmenu" >
   <a class="history" href="javascript:history.go(-1)" ><%= base.GetLocalResourceObject("lblPreviousPage.Text")%></a>
</div>
<div id="page-content" >
 <div class="saloninfo" >
     <div class="name" >
         <asp:Literal ID="Name" runat="server" ></asp:Literal>
     </div>
     <div class="location" >
         <asp:Literal ID="Address1" runat="server" EnableViewState="false" ></asp:Literal>
         <asp:Literal ID="Address2" runat="server" EnableViewState="false" ></asp:Literal>
         <asp:Literal ID="ZipPostalCode" runat="server" EnableViewState="false" ></asp:Literal>
         <h1><%= base.GetLocalResourceObject("lblCheaper.Text") %></h1>
     </div>
     <div class="logo" >
         <asp:Image ID="Logo" runat="server" EnableViewState="false" />
     </div>
     <div class="options" >
         <SA:SalonMenu ID="Menu" runat="server" OnMenuItemSelected="Menu_MenuItemSelected" />
     </div>
 </div>
 <style type="text/css" >
   #map_canvas { width:100%; height:300px; border:solid 1px #888; }
 </style>
 <p><asp:Literal ID="ltrDescription" runat="server" EnableViewState="false" ></asp:Literal></p>
 <div id="map_canvas" ></div>
 <%= SalonAddict.BusinessAccess.Implementation.ConfigurationManager.GetSettingValue("Common.GoogleMapsScript")%>
 <style type="text/css" >
  .instructions { padding-top:25px; padding-bottom:25px; width:700px; }
</style>
 <asp:ListView 
    ID="lvDirections" 
    runat="server" 
    GroupItemCount="1"
    GroupPlaceholderID="GroupPlaceHolderID" 
    ItemPlaceholderID="ItemPlaceHolderID" >
    <LayoutTemplate>
         <asp:PlaceHolder ID="GroupPlaceHolderID" runat="server" ></asp:PlaceHolder>
    </LayoutTemplate>
    <GroupTemplate>
         <asp:PlaceHolder ID="ItemPlaceHolderID" runat="server" ></asp:PlaceHolder>
    </GroupTemplate>
    <ItemTemplate>
        <div class="instructions" ><%# Eval("Instructions")%></div>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <div class="instructions" style="border-top:solid 1px #e7e7e7" ><%# Eval("Instructions")%></div>
    </AlternatingItemTemplate>
</asp:ListView>
</div>
</div>
</asp:Content>
