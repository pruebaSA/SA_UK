<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Navbar.ascx.cs" EnableViewState="false" Inherits="SalonAddict.Modules.Navbar" %>
<%@ Register TagPrefix="SA" TagName="NewsLetterSignUp" Src="~/Templates/NewsletterSignupOne.ascx" %>
<div class="wrapper-navbar" >
   <div class="navbar-center" >
       <div class="column-one" >
          <asp:LoginView ID="lv" runat="server" >
             <AnonymousTemplate></AnonymousTemplate>
             <LoggedInTemplate>
                 <div class="login-template" >
                 <% if (SalonAddict.SAContext.Current.User != null && SalonAddict.SAContext.Current.User.IsAdmin)
                    { %>
                 <%= base.GetLocalResourceObject("lblUser.Text") %>
                 <a href='<%= Page.ResolveUrl("~/Administration") %>' ><%= HttpContext.Current.User.Identity.Name %></a>
                 <% } %>
                 </div>
             </LoggedInTemplate>
          </asp:LoginView>
       </div>
       <div class="column-two" >
          <SA:NewsLetterSignUp ID="NewsLetterSignUp" runat="server" />
       </div>
   </div>
</div>