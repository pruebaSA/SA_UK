<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="WSP-Edit.aspx.cs" Inherits="IFRAME.Admin.WSP_EditPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Account" />
       <div class="horizontal-line" ></div>
        <table style="margin-bottom:20px" cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:60px" ><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-salons.png" %>' alt="salons" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle;text-align:right;" >
                  <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
              </td>
           </tr>
        </table>
        <ajaxToolkit:TabContainer ID="pnlTabs" runat="server" AutoPostBack="false" >
           <ajaxToolkit:TabPanel ID="tab1" runat="server" HeaderText="General" >
             <ContentTemplate>
                <table class="details" cellpadding="0" cellspacing="0" >
                    <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrAccount.Text") %>
                      </td>
                      <td class="data-item" >
                        <asp:Literal ID="ltrAccount" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrPlanType.Text") %>
                      </td>
                      <td class="data-item" >
                        <asp:Literal ID="ltrPlanType" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrPeriod.Text") %>
                      </td>
                      <td class="data-item" >
                        <asp:Literal ID="ltrPeriod" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrPlanPrice.Text") %>
                      </td>
                      <td class="data-item" >
                        <asp:Literal ID="ltrPlanPrice" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrTransFee.Text") %>
                      </td>
                      <td class="data-item" >
                        <asp:Literal ID="ltrTransFee" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrStatus.Text") %>
                      </td>
                      <td class="data-item" >
                        <asp:Literal ID="ltrStatus" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                </table>
             </ContentTemplate>
           </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    </asp:Panel>
</asp:Content>
