<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="Reviews.aspx.cs" Inherits="SalonAddict.ReviewsPage" %>
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
   #wrapper-book #page-content .saloninfo { position:relative; height:120px; }
   #wrapper-book #page-content .saloninfo .name { position:absolute; top:5px; left:0px; font-size:17px; font-weight:bold; }
   #wrapper-book #page-content .saloninfo .location { position:absolute; top:22px; left:0px; font-size:12px; }
   #wrapper-book #page-content .saloninfo .logo { position:absolute; top:0px; right:5px; padding:5px; border:solid 1px #eee; }
   #wrapper-book #page-content .saloninfo .options { position:absolute; top:65px; left:0px; }
   #wrapper-book #page-content .saloninfo .rating { position:absolute; top:125px; left:603px; color:#ef286a; }
   #wrapper-book .search-text { position:relative; font-family:Arial; font-size:11px; font-weight:normal; margin-bottom:5px;  }
   #wrapper-book .wrapper-nextprevious { position:absolute; right:0px; top:0px; }
   #wrapper-book #page-content .review-item { width:520px; padding:10px; margin:10px; background:#f7f7f7; -moz-border-radius: 6px; -webkit-border-radius: 6px; -khtml-border-radius: 6px; border-radius: 6px; }
</style>
<div id="wrapper-book" >
<div class="wrapper-sortmenu" >
   <a class="history" href="javascript:history.go(-1)" ><%= base.GetLocalResourceObject("lblPreviousPage.Text")%></a>
</div>
<div id="page-content" >
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
     <div class="rating" >
        <b style="font-size:36px;line-height:40px;"><asp:Literal ID="ltrRatingScore" runat="server" EnableViewState="false" ></asp:Literal>%</b>
        <div></div>
        <strong><%= base.GetLocalResourceObject("ltrOverallRating.Text") %></strong>
        <div><asp:Literal ID="ltrTotalReviews" runat="server" EnableViewState="false" ></asp:Literal></div>
     </div>
 </div>
 <asp:Repeater ID="rptr" runat="server" EnableViewState="false" >
   <ItemTemplate>
   <div class="review-item" >
     <table class="details" cellpadding="0" cellspacing="0" >
        <tr>
           <td class="title" ><p><strong><%# Math.Round((decimal)Eval("RatingScore"), 0) %>%</strong></p></td>
           <td class="data-item" >
              <p><%# System.Web.HttpUtility.HtmlEncode(Eval("ReviewText").ToString()) %></p>
              <div style="font-size:11px;" >
                  <%# String.Format(base.GetLocalResourceObject("lblReviewSignature.Text").ToString(), 
                       ((DateTime)Eval("CreatedOn")).ToString("dd MMM yyyy"),
                       (Convert.ToBoolean(Eval("Anonymous").ToString()))? base.GetLocalResourceObject("lblAnonymous.Text").ToString() : Eval("FirstName").ToString()) %>
              </div>
           </td>
        </tr>
     </table>
    </div>
   </ItemTemplate>
</asp:Repeater>
<div style="padding:15px" >
<%= base.GetLocalResourceObject("ltrReviewsExplained.Text") %>
</div>
</div>
</div>
</asp:Content>
