<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" EnableViewState="false" Inherits="SalonAddict.Modules.Header" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/Modules/Menu.ascx" %>
<div class="wrapper-header" > 
  <div class="header-center" >
     <div class="column-1" >
         <a href='<%= Page.ResolveUrl("~/") %>' >
            <img src='<%= Page.ResolveUrl("~/images/salonaddict.png") %>' alt="http://www.salonaddict.co.uk" />
         </a>
         <div class="slogan" >
            <%= base.GetLocalResourceObject("ltrSlogan.Text") %>
         </div>
     </div>
     <div class="column-2" >
        <div class="wrapper-menu" >
          <SA:Menu ID="Menu" runat="server" />
        </div>
     </div>
  </div>
</div>