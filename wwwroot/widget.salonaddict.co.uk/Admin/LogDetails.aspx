<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="LogDetails.aspx.cs" Inherits="IFRAME.Admin.LogDetailsPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Audit" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-audit.png" %>' alt="audit" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
                 <asp:Button ID="btnDelete" runat="server" SkinID="SubmitButton" OnClick="btnDelete_Click" meta:resourceKey="btnDelete" />
              </td>
           </tr>
        </table>
        <table class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrLogType.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrLogType" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrMessage.Text") %>
              </td>
              <td class="data-item" >
                 <div style="max-width:550px;overflow:hidden;" >
                    <asp:Literal ID="ltrMessage" runat="server" EnableViewState="false" ></asp:Literal>
                 </div>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrException.Text") %>
              </td>
              <td class="data-item" >
                 <div style="max-width:550px;overflow:hidden;" >
                    <asp:Literal ID="ltrException" runat="server" EnableViewState="false" ></asp:Literal>
                 </div>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrUser.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrUser" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrPageURL.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrPageURL" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrReferrerURL.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrReferrerURL" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrUserHostAddress.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrUserHostAddress" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrCreatedOn.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrCreatedOn" runat="server" ></asp:Literal>
              </td>
           </tr>
          <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrParams.Text") %>
              </td>
              <td class="data-item" >
                  <asp:Panel ID="pnlParamsHeader" runat="server" Width="80px" >
                      <u style="cursor:pointer" ><%= base.GetLocaleResourceString("ltrViewDetails.Text") %></u>
                  </asp:Panel>
                  <asp:Panel ID="pnlParamsContent" runat="server" >
                      <asp:Literal ID="ltrParameters" runat="server" EnableViewState="false" ></asp:Literal>
                  </asp:Panel>
                  <ajaxToolkit:BalloonPopupExtender 
                    ID="pnlPopout" 
                    runat="server" 
                    BalloonPopupControlID="pnlParamsContent" 
                    BalloonSize="Large" 
                    BalloonStyle="Rectangle"
                    CacheDynamicResults="false"
                    DisplayOnClick="true"
                    DisplayOnMouseOver="false"
                    DisplayOnFocus="false"
                    Position="TopRight"
                    TargetControlID="pnlParamsHeader"
                    OffsetX="0"
                    OffsetY="0"
                    UseShadow="true" />
              </td>
           </tr>
        </table>
     </asp:Panel>
</asp:Content>
