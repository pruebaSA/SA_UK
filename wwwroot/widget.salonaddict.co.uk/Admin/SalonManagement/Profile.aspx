<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="IFRAME.Admin.SalonManagement.ProfilePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Profile" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-profile.png" %>' alt="profile" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
                  <asp:Button ID="btnEdit" runat="server" SkinID="SubmitButton" Visible="false" OnClick="btnEdit_Click" meta:resourceKey="btnEdit" />
              </td>
           </tr>
        </table>
        <table cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:550px;" >
                <table style="margin-top:10px" class="details" cellpadding="0" cellspacing="0" >
                   <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrName.Text") %></td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrName" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <% if (ltrPhone.Text != String.Empty)
                      { %>
                   <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrPhone.Text")%></td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrPhone" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <% } %>
                   <% if(ltrMobile.Text != String.Empty)
                      { %>
                   <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrMobile.Text") %></td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrMobile" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <% } %>
                   <% if (ltrEmail.Text != String.Empty)
                      { %>
                   <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrEmail.Text") %></td>
                      <td class="data-item" >
                          <asp:Literal ID="ltrEmail" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <% } %>
                </table>
              </td>
              <td>
                 <% if (false)
                    { %>
                 <p>&bull; <a href='<%= IFRMHelper.GetURL("profile-email-edit.aspx") %>' ><u><%= base.GetLocaleResourceString("hlEmailEdit.Text")%></u></a></p>
                 <p>&bull; <a href='<%= IFRMHelper.GetURL("profile-password-edit.aspx") %>' ><u><%= base.GetLocaleResourceString("hlPasswordEdit.Text")%></u></a></p>
                 <% } %>
              </td>
           </tr>
        </table>
    </asp:Panel>
</asp:Content>
