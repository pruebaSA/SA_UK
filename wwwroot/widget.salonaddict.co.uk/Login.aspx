<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="IFRAME.LoginPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <h1><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1>
       <asp:MultiView ID="mv" runat="server" ActiveViewIndex="0" >
          <asp:View ID="v0" runat="server" >
           <asp:Label ID="lblLoginError" runat="server" SkinID="ErrorLabel" EnableViewState="false" ></asp:Label>
           <table class="details" cellpadding="0" cellspacing="0" >
              <tr>
                 <td class="title" ><%= base.GetLocaleResourceString("ltrUsername.Text") %></td>
                 <td class="data-item" >
                    <asp:TextBox ID="txtUsername" runat="server" SkinID="TextBox" MaxLength="30" meta:resourceKey="txtUsername" ></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valUsername" runat="server" ControlToValidate="txtUsername" Display="None" meta:resourceKey="valUsername" ></asp:RequiredFieldValidator>
                    <ajaxToolkit:ValidatorCalloutExtender ID="valUsernameEx" runat="Server" TargetControlID="valUsername" EnableViewState="false" />
                 </td>
              </tr>
              <tr>
                 <td class="title" ><%= base.GetLocaleResourceString("ltrPassword.Text") %></td>
                 <td class="data-item" >
                    <asp:TextBox ID="txtPassword" runat="server" SkinID="TextBox" TextMode="Password" MaxLength="20" meta:resourceKey="txtPassword" ></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valPassword" runat="server" ControlToValidate="txtPassword" Display="None" meta:resourceKey="valPassword" ></asp:RequiredFieldValidator>
                    <ajaxToolkit:ValidatorCalloutExtender ID="valPasswordEx" runat="Server" TargetControlID="valPassword" EnableViewState="false" />
                 </td>
              </tr>
              <tr>
                 <td class="title" >
                    
                 </td>
                 <td class="data-item" >
                    <asp:Button ID="btnLogin" runat="server" SkinID="SubmitButton" OnClick="btnLogin_Click" meta:resourceKey="btnLogin" />
                    <asp:Button ID="btnCancel1" runat="server" SkinID="SubmitButton" OnClick="btnCancel_Click" CausesValidation="false" meta:resourceKey="btnCancel" />
                 </td>
              </tr>
              <tr>
                 <td class="title" ></td>
                 <td class="data-item" >
                    <% if (false)
                       { %>
                    <a href="#" ><%= base.GetLocaleResourceString("hlPasswordRecovery.Text")%></a>
                    <% } %>
                 </td>
              </tr>
           </table>
          </asp:View>
          <asp:View ID="v1" runat="server" >
              <script type="text/javascript" language="javascript" src="js/jquery.popupWin.js" ></script>
              <input id="btnPopup" type="button" class="button-submit" value='<%= base.GetLocaleResourceString("btnPopup.Text") %>' />
              <script type="text/javascript" language="javascript" >
                  $('#btnPopup').popupWindow({ width: 550, height: 350, scrollbars: 1, centerScreen: 1, windowURL: '<%= IFRMHelper.GetURL(Page.ResolveUrl("~/safari-cookie-fix.aspx")) %>' });
              </script>
          </asp:View>
       </asp:MultiView>
    </asp:Panel>
</asp:Content>
