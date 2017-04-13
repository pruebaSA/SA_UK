<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Appointment-Details.aspx.cs" Inherits="IFRAME.Admin.SalonManagement.Appointment_DetailsPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Appointments" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-appointments.png" %>' alt="appointments" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
                 <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
              </td>
           </tr>
        </table>
       <ajaxToolkit:TabContainer ID="tc" runat="server" ActiveTabIndex="0" >
          <ajaxToolkit:TabPanel ID="t1" runat="server" >
              <ContentTemplate>
               <table class="details" cellspacing="0" cellpadding="0" >
                  <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrConfirmationNo.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrConfirmationNo" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrClient.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrClient" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                          <%= base.GetLocaleResourceString("ltrPhone.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrPhone" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                          <%= base.GetLocaleResourceString("ltrEmail.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrEmail" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                          <%= base.GetLocaleResourceString("ltrService.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrService" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                          <%= base.GetLocaleResourceString("ltrEmployee.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrEmployee" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                          <%= base.GetLocaleResourceString("ltrDate.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrDate" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                          <%= base.GetLocaleResourceString("ltrTime.Text") %>
                      </td>
                      <td class="data-item" >
                          <asp:Literal ID="ltrTime" runat="server" ></asp:Literal>
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
          <ajaxToolkit:TabPanel ID="t2" runat="server" >
              <ContentTemplate>
                <div style="padding:6px" >
                    <%= base.GetLocaleResourceString("ltrFirstTimeAtSalon.Text") %>
                    &nbsp;
                    <asp:Literal ID="ltrFirstTimeAtSalon" runat="server" ></asp:Literal>
                    <div class="horizontal-line" ></div>
                    <asp:Literal ID="ltrSpecialRequest" runat="server" ></asp:Literal>
                </div>
              </ContentTemplate>
          </ajaxToolkit:TabPanel>
       </ajaxToolkit:TabContainer>
     </asp:Panel>
</asp:Content>
