<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SalonPortal.SecureArea.Default" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/TwoColumn.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="TwoColumnSideContentPlaceHolder" runat="server">
   <SA:Menu ID="cntlMenu" runat="server" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="TwoColumnContentPlaceHolder" runat="server">
   <div class="section-header">
        <div class="title">
            <img src="<%= Page.ResolveUrl("~/SecureArea/images/ico-dashboard.png") %>" alt="" />
            <%= base.GetLocalResourceObject("Header.Text") %>
            <SA:BackLink ID="cntlBackLink" runat="server" />
        </div>
   </div>
   <ajaxToolkit:TabContainer ID="Salons" runat="server" >
      <ajaxToolkit:TabPanel ID="pnlBusinesses" runat="server" >
         <ContentTemplate>
           <asp:ListView 
               ID="lvNonGuest" 
               runat="server" 
               GroupItemCount="5" 
               GroupPlaceholderID="GroupPlaceHolder" 
               ItemPlaceholderID="ItemPlaceHolder"
               OnItemCommand="lv_ItemCommand" >
              <LayoutTemplate>
                  <table cellpadding="0" cellspacing="8" >
                     <asp:PlaceHolder ID="GroupPlaceHolder" runat="server" ></asp:PlaceHolder>
                  </table>
              </LayoutTemplate>
              <GroupTemplate>
                  <tr>
                     <asp:PlaceHolder ID="ItemPlaceHolder" runat="server" ></asp:PlaceHolder>
                  </tr>
              </GroupTemplate>
              <ItemTemplate>
                  <td style="vertical-align:top">
                     <div class="salon-logo" >
                     <asp:ImageButton 
                        ID="BusinessButton" 
                        runat="server" 
                        BorderColor="#e5e5e5" 
                        BorderWidth="1px" 
                        BorderStyle="Solid" 
                        CommandName="Impersonate"
                        CommandArgument='<%# Eval("BusinessID") %>'
                        ToolTip='<%# Eval("Name") %>'
                        ImageUrl='<%# SalonAddict.BusinessAccess.Implementation.MediaManager.GetBusinessPictureURL((int)Eval("PictureID")) %>' />
                     </div>
                     <div style="text-align:center" >
                        <small><%# Eval("Name") %></small>
                        <div><%# Eval("AddressLine1") %></div>
                     </div>
                  </td>
              </ItemTemplate>
           </asp:ListView>
         </ContentTemplate>
      </ajaxToolkit:TabPanel>
      <ajaxToolkit:TabPanel ID="pnlDemo" runat="server" >
         <ContentTemplate>
           <asp:ListView 
               ID="lvGuest" 
               runat="server" 
               GroupItemCount="5" 
               GroupPlaceholderID="GroupPlaceHolder" 
               ItemPlaceholderID="ItemPlaceHolder"
               OnItemCommand="lv_ItemCommand" >
              <LayoutTemplate>
                  <table cellpadding="0" cellspacing="8" >
                     <asp:PlaceHolder ID="GroupPlaceHolder" runat="server" ></asp:PlaceHolder>
                  </table>
              </LayoutTemplate>
              <GroupTemplate>
                  <tr>
                     <asp:PlaceHolder ID="ItemPlaceHolder" runat="server" ></asp:PlaceHolder>
                  </tr>
              </GroupTemplate>
              <ItemTemplate>
                  <td style="vertical-align:top">
                     <div class="salon-logo" >
                     <asp:ImageButton 
                        ID="BusinessButton" 
                        runat="server" 
                        BorderColor="#e5e5e5" 
                        BorderWidth="1px" 
                        BorderStyle="Solid" 
                        CommandName="Impersonate"
                        CommandArgument='<%# Eval("BusinessID") %>'
                        ToolTip='<%# Eval("Name") %>'
                        ImageUrl='<%# SalonAddict.BusinessAccess.Implementation.MediaManager.GetBusinessPictureURL((int)Eval("PictureID")) %>' />
                     </div>
                     <div style="text-align:center" >
                        <small><%# Eval("Name") %></small>
                        <div><%# Eval("AddressLine1") %></div>
                     </div>
                  </td>
              </ItemTemplate>
           </asp:ListView>
         </ContentTemplate>
      </ajaxToolkit:TabPanel>
   </ajaxToolkit:TabContainer>
</asp:Content>
