<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="Book-Appointment.aspx.cs" Inherits="SalonAddict.Book_Appointment" %>
<%@ Register TagPrefix="SA" TagName="SearchPanel" Src="~/Templates/SearchPanelTwo.ascx" %>
<%@ Register TagPrefix="SA" TagName="SortMenu" Src="~/Templates/SortMenuTwo.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="SalonMenu" Src="~/Modules/SalonMenu.ascx" %>
<%@ Register TagPrefix="SA" TagName="NextPreviousDate" Src="~/Templates/ChangeSearchDateOne.ascx" %>
<%@ Register TagPrefix="SA" TagName="Availability" Src="~/Templates/StaffAvailabilityOne.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/TwoColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TwoColumnSideContentPlaceHolder" runat="server">
   <SA:SearchPanel ID="SearchPanel" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TwoColumnContentPlaceHolder" runat="server">
<style type="text/css" >
   #wrapper-search { position:relative; }
   .wrapper-sortmenu .history { position:absolute; top:10px; left:15px; }
   #wrapper-search .wrapper-sortmenu { padding-left:240px; }
   #wrapper-search .saloninfo { position:relative; height:130px; }
   #wrapper-search .saloninfo .name { position:absolute; top:5px; left:0px; font-size:17px; font-weight:bold; }
   #wrapper-search .saloninfo .location { position:absolute; top:22px; left:0px; font-size:12px; }
   #wrapper-search .saloninfo .logo { position:absolute; top:0px; right:5px; padding:5px; border:solid 1px #eee; }
   #wrapper-search .saloninfo .options { position:absolute; top:65px; left:0px; }
   #wrapper-search .results { position:relative; margin-top:10px; margin-left:5px; margin-bottom:10px; padding:9px; border:solid 1px #eee; }
   #wrapper-search .search-text { position:relative; height:15px; font-family:Arial; font-size:13px; padding-left:5px; font-weight:normal; margin-bottom:5px;  }
   #wrapper-search .wrapper-nextprevious { position:absolute; right:-2px; top:0px; }
</style>
<div id="wrapper-search" >
     <div class="wrapper-sortmenu" >
        <a class="history" href="javascript:history.go(-1)" ><%= base.GetLocalResourceObject("lblPreviousPage.Text")%></a>
        <SA:SortMenu ID="SortMenu" runat="server" OnSelectedIndexChanged="SortMenu_SelectedIndexChanged" />
     </div>
     <div class="results" >
     <div class="saloninfo" >
         <div class="name" >
             <asp:Literal ID="Name" runat="server" EnableViewState="false" ></asp:Literal>
         </div>
         <div class="location" >
             <asp:Literal ID="Address" runat="server" EnableViewState="false" ></asp:Literal>
             <h1><%= base.GetLocalResourceObject("lblCheaper.Text") %></h1>
         </div>
         <div class="logo" >
             <asp:Image ID="Logo" runat="server" EnableViewState="false" />
         </div>
         <div class="options" >
             <SA:SalonMenu ID="Menu" runat="server" OnMenuItemSelected="Menu_MenuItemSelected" />
         </div>
     </div>
     <div class="search-text" >
        <asp:Literal ID="lblResults" runat="server" ></asp:Literal>
        <asp:Panel ID="pnlNextPrevious" runat="server" CssClass="wrapper-nextprevious" >
           <SA:NextPreviousDate ID="NextPreviousDate" runat="server" OnPreviousWeek="NextPreviousDate_SelectedDateChanged" OnPreviousDay="NextPreviousDate_SelectedDateChanged" OnNextDay="NextPreviousDate_SelectedDateChanged" OnNextWeek="NextPreviousDate_SelectedDateChanged" />
        </asp:Panel>
     </div>
     <SA:Message ID="Message" runat="server" IsError="true" Visible="false" />
     <asp:ListView
        ID="lv" 
        runat="server"
        GroupItemCount="1" 
        GroupPlaceholderID="Group" 
        EnableViewState="true"
        ItemPlaceholderID="Item" >
        <LayoutTemplate>
           <asp:PlaceHolder ID="Group" runat="server" ></asp:PlaceHolder>
        </LayoutTemplate>
        <GroupTemplate>
           <asp:PlaceHolder ID="Item" runat="server" ></asp:PlaceHolder>
        </GroupTemplate>
        <ItemTemplate>
           <SA:Availability ID="Availability" runat="server" DataItem='<%# Container.DataItem %>' OnAvailabilitySelected="Availability_OnAvailabilitySelected" OnNextWeek="Availability_OnNextWeek" ></SA:Availability>
        </ItemTemplate>
     </asp:ListView>
    </div>
</div>
</asp:Content>
