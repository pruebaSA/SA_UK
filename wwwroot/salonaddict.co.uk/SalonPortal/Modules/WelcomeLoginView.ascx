<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WelcomeLoginView.ascx.cs" Inherits="SalonPortal.Modules.WelcomeLoginView" %>
<asp:LoginView ID="LoginView" runat="server" >
   <LoggedInTemplate>
      <div class="welcome-login-view-module" >
          <ul>
            <li>
               <%= String.Format(
                   base.GetGlobalResourceObject("Modules", "WelcomeLoginView_Welcome_Message").ToString(),
                   HttpContext.Current.User.Identity.Name)
               %> 
               ( <a href="<%= Page.ResolveUrl("~/SecureArea/AccountDetails.aspx") %>" ><%= base.GetGlobalResourceObject("Modules", "WelcomeLoginView_Link_Account_Text") %></a> )
            </li>
            <li>&nbsp; | &nbsp;</li>
            <li>
              <a href="<%= Page.ResolveUrl("~/Logout.aspx") %>" class="highlight" >
                 <%= base.GetGlobalResourceObject("Modules", "WelcomeLoginView_Link_Logout_Text") %>
              </a>
            </li>
          </ul>
      </div>
   </LoggedInTemplate>
</asp:LoginView>