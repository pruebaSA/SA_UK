<%@ Page Language="C#" MasterPageFile="~/MasterPages/Root.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SalonAddict.DefaultPage" %>
<%@ Register TagPrefix="SA" TagName="PromotionSummary" Src="~/Modules/PromotionSummary.ascx" %>
<%@ Register TagPrefix="SA" TagName="QuickSearchSummary" Src="~/Modules/QuickSearchSummary.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/Root.Master" %>

<%@ Register TagPrefix="SA" TagName="SlideShow" Src="~/Modules/SlideShow.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph" runat="server">
    <SA:SlideShow ID="SlideShow" runat="server" />
    <table cellpadding="0" cellspacing="0" width="950px" >
      <tr>
        <td>
            <SA:PromotionSummary ID="Promotions" runat="server"  />
        </td>
        <td style="padding-left:20px;">
            <SA:QuickSearchSummary ID="QuickSearches" runat="server" />
            <div style="margin-top:20px;" id="fb-root"></div><script type="text/javascript" language="javascript" src="http://connect.facebook.net/en_US/all.js#xfbml=1"></script><fb:like-box href="http://www.facebook.com/SalonAddict.co.uk" width="320" colorscheme="light" show_faces="true" stream="false" header="true"></fb:like-box>
        </td>
      </tr>
    </table>
</asp:Content>