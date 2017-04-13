<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeBehind="NoScript.ascx.cs" Inherits="IFRAME.Modules.NoScript" %>
<noscript>
   <style type="text/css" >
       .master-onecolumn-center
       {
   	       display:none;
       }
   </style>
   <div class="module-noscript" >
       <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
           <h1><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1>
           <p>
              <%= base.GetLocaleResourceString("ltrParagraph.Text") %>
           </p>
           <p>
              <%= base.GetLocaleResourceString("ltrLink.Text") %>
           </p>
       </asp:Panel>
   </div>
</noscript>