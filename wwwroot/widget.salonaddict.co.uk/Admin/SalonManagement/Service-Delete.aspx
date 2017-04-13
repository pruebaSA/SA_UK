<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Service-Delete.aspx.cs" Inherits="IFRAME.Admin.SalonManagement.Service_DeletePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Services" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-services.png" %>' alt="services" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >

              </td>
           </tr>
        </table>
        <table style="margin-top:10px" class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrName.Text") %></td>
              <td class="data-item" >
                 <asp:Literal ID="ltrName" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrCategory.Text") %></td>
              <td class="data-item" >
                 <asp:Literal ID="ltrCategory" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrDescripton.Text") %></td>
              <td class="data-item" >
                 <asp:Literal ID="ltrDescription" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrPrice.Text") %></td>
              <td class="data-item" >
                 <asp:Literal ID="ltrPrice" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrDuration.Text") %></td>
              <td class="data-item" >
                 <asp:Literal ID="ltrDuration" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrActive.Text") %></td>
              <td class="data-item" >
                 <asp:CheckBox ID="cbActive" runat="server" Enabled="false" />
              </td>
           </tr>
           <tr>
              <td class="title" ></td>
              <td class="data-item" >
                  <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                  <asp:Button ID="btnDelete" runat="server" SkinID="SubmitButton" OnClick="btnDelete_Click" meta:resourceKey="btnDelete" />
              </td>
           </tr>
        </table>
     </asp:Panel>
</asp:Content>