<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="SalonAddict.SearchPage" %>
<%@ Register TagPrefix="SA" TagName="Search" Src="~/Templates/SearchPanelTwo.ascx" %>
<%@ Register TagPrefix="SA" TagName="SortMenu" Src="~/Templates/SortMenuOne.ascx" %>
<%@ Register TagPrefix="SA" TagName="Availability" Src="~/Templates/BusinessAvailabilityOne.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="NextPreviousDate" Src="~/Templates/ChangeSearchDateOne.ascx" %>

<%@ MasterType VirtualPath="~/MasterPages/TwoColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TwoColumnSideContentPlaceHolder" runat="server">
    <SA:Search ID="SearchPanel" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TwoColumnContentPlaceHolder" runat="server">
<style type="text/css" >
   #wrapper-search { position:relative; }
   #wrapper-search .wrapper-sortmenu { padding-left:350px; }
   #wrapper-search #prog-canvas { position:absolute; top:0px; left:0px; width:100%; height:100%; z-index:99; background:#fff; }
   #wrapper-search #prog-image { padding-top:20px; padding-bottom:20px; width:32px; height:32px; margin:0 auto; }
   #wrapper-search .results { position:relative; margin-top:10px; margin-left:5px; margin-bottom:10px; padding:9px; border:solid 1px #eee; }
   #wrapper-search .search-text { position:relative; font-family:Arial; font-size:11px; font-weight:normal; margin-bottom:5px;  }
   #wrapper-search .wrapper-nextprevious { position:absolute; right:-2px; top:0px; }
</style>
<div id="wrapper-search" >
     <div class="wrapper-sortmenu" >
        <SA:SortMenu ID="SortMenu" runat="server" OnSelectedIndexChanged="SortMenu_SelectedIndexChanged" />
     </div>
     <div class="results" >
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
            ItemPlaceholderID="Item"
            EnableViewState="false" >
            <LayoutTemplate>
               <asp:PlaceHolder ID="Group" runat="server" ></asp:PlaceHolder>
            </LayoutTemplate>
            <GroupTemplate>
               <asp:PlaceHolder ID="Item" runat="server" ></asp:PlaceHolder>
            </GroupTemplate>
            <ItemTemplate>
               <SA:Availability ID="Availability" runat="server" DataItem='<%# Container.DataItem %>' ></SA:Availability>
            </ItemTemplate>
         </asp:ListView>
         <SA:Pager ID="Pager" runat="server" PageSize="30" CssClass="pager" OnPageIndexChanged="Pager_PageIndexChanged" meta:resourceKey="Pager" ></SA:Pager>
     </div>
</div>
</asp:Content>

