<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeBehind="Footer.ascx.cs" Inherits="IFRAME.Modules.Footer" %>
<div class="module-footer" >
  <div class="terms-privacy" >
      <%= base.GetLocaleResourceString("ltrTerms.Text") %> | <%= base.GetLocaleResourceString("ltrPrivacy.Text") %>
      <br />
      <%= String.Format(base.GetLocaleResourceString("ltrCopyright.Text"), DateTime.Today.ToString("yyyy")) %>
  </div>
  <div class="partner-statement" >
      <asp:Literal ID="ltrPartnerStatement" runat="server" EnableViewState="false" ></asp:Literal>
      <asp:LoginView ID="lv" runat="server" EnableViewState="false" >
         <AnonymousTemplate>
             <a href='<%= IFRMHelper.GetURL(Page.ResolveUrl("~/login.aspx")) %>' ><%= base.GetLocaleResourceString("hlSignIn.Text") %></a>
         </AnonymousTemplate>
      </asp:LoginView>
  </div>
  <div class="poweredby" >
     <a class="poweredby-logo" href="http://www.salonaddict.co.uk" target="_blank" ></a>
  </div>
</table>