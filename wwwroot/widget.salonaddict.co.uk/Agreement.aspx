<%@ Page Language="C#" MasterPageFile="~/MasterPages/Root.Master" AutoEventWireup="true" CodeBehind="Agreement.aspx.cs" Inherits="IFRAME.AgreementPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph" runat="server">
   <style type="text/css" >
      h1 { font-size:17px; }
      h2 { font-size:15px; }
      h3 { font-size:13px; }
      p { font-size:11px; }
   </style>
   <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
      <img src='<%= Page.ResolveUrl(String.Format("~/App_Themes/{0}/images/salonaddict.png", base.Page.Theme)) %>' height="60px" alt="SalonAddict" />
      <h1><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1>
      <h2><%= base.GetLocaleResourceString("ltrUserAgreement.Text") %></h2>
      <p>
        <%= base.GetLocaleResourceString("ltrUserAgreement.Paragraph[0].Text") %>
      </p>
      <p>
        <%= base.GetLocaleResourceString("ltrUserAgreement.Paragraph[1].Text") %>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrWhatWeDo.Text") %></h3>
      <p>
        <%= base.GetLocaleResourceString("ltrWhatWeDo.Paragraph[0].Text")%>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrNoShow.Text") %></h3>
      <p>
        <%= base.GetLocaleResourceString("ltrNoShow.Paragraph[0].Text")%>
      </p>
      <p>
        <%= base.GetLocaleResourceString("ltrNoShow.Paragraph[1].Text")%>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrPrivacy.Text") %></h3>
      <p>
        <%= base.GetLocaleResourceString("ltrPrivacy.Paragraph[0].Text")%>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrUsage.Text") %></h3>
      <p>
        <%= base.GetLocaleResourceString("ltrUsage.Paragraph[0].Text")%>
      </p>
      <p>
        <%= base.GetLocaleResourceString("ltrUsage.Paragraph[1].Text")%>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrEmailPolicy.Text") %></h3>
      <p>
        <%= base.GetLocaleResourceString("ltrEmailPolicy.Paragraph[0].Text")%>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrIntellectualProperty.Text") %></h3>
      <p>
         <%= base.GetLocaleResourceString("ltrIntellectualProperty.Paragraph[0].Text") %>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrReviews.Text") %></h3>
      <p>
         <%= base.GetLocaleResourceString("ltrReviews.Paragraph[0].Text") %>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrAccess.Text") %></h3>
      <p>
         <%= base.GetLocaleResourceString("ltrAccess.Paragraph[0].Text")%>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrLiability.Text") %></h3>
      <p>
         <%= base.GetLocaleResourceString("ltrLiability.Paragraph[0].Text")%>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrDisclaimer.Text") %></h3>
      <p>
         <%= base.GetLocaleResourceString("ltrDisclaimer.Paragraph[0].Text")%>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrIdemnity.Text") %></h3>
      <p>
         <%= base.GetLocaleResourceString("ltrIdemnity.Paragraph[0].Text")%>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrThirdParty.Text") %></h3>
      <p>
         <%= base.GetLocaleResourceString("ltrThirdParty.Paragraph[0].Text")%>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrSeverability.Text") %></h3>
      <p>
         <%= base.GetLocaleResourceString("ltrSeverability.Paragraph[0].Text")%>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrNoAssignment.Text") %></h3>
      <p>
         <%= base.GetLocaleResourceString("ltrNoAssignment.Paragraph[0].Text")%>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrWaiver.Text") %></h3>
      <p>
         <%= base.GetLocaleResourceString("ltrWaiver.Paragraph[0].Text")%>
      </p>
      <h3><%= base.GetLocaleResourceString("ltrChoiceOfLaw.Text") %></h3>
      <p>
         <%= base.GetLocaleResourceString("ltrChoiceOfLaw.Paragraph[0].Text")%>
      </p>
      <p>&nbsp;</p>
      <p>
         <%= base.GetLocaleResourceString("ltrRevisionDate.Text") %>
      </p>
   </asp:Panel>
</asp:Content>
