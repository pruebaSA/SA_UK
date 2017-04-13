<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="WSP.aspx.cs" Inherits="IFRAME.Admin.WSPPage" %>
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

              </td>
           </tr>
        </table>
        <asp:Panel ID="pnlPlan" runat="server" >
            <table cellpadding="0" cellspacing="0" >
               <td width="500px" >
                    <table class="details" cellpadding="0" cellspacing="0" >
                       <tr>
                          <td class="title" >
                              <%= base.GetLocaleResourceString("ltrSalon.Text") %>
                          </td>
                          <td class="data-item" >
                             <asp:Literal ID="ltrSalon" runat="server" ></asp:Literal>
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                              <%= base.GetLocaleResourceString("ltrPlan.Text") %>
                          </td>
                          <td class="data-item" >
                             <asp:Literal ID="ltrPlan" runat="server" ></asp:Literal>
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                             <%= base.GetLocaleResourceString("ltrPlanPrice.Text") %>
                          </td>
                          <td class="data-item" >
                             <asp:Literal ID="ltrPrice" runat="server" ></asp:Literal>
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                              <%= base.GetLocaleResourceString("ltrPeriod.Text") %>
                          </td>
                          <td class="data-item" >
                             <asp:Literal ID="ltrPlanPeriod" runat="server" ></asp:Literal>
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                              <%= base.GetLocaleResourceString("ltrFeeWT.Text") %>
                          </td>
                          <td class="data-item" >
                             <asp:Literal ID="ltrFeeWT" runat="server" ></asp:Literal>
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
               </td>
               <td>
                   <p>&bull; <a href='<%= IFRMHelper.GetURL("wsp-history.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><u>View Plan History</u></a></p>
                   <% if (this.EditMode && this.TrialMode)
                      { %>
                   <p>&bull; <a href='<%= IFRMHelper.GetURL("wsp-extend.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><u>Extend Trial Period</u></a></p>
                   <% } %>
                   <% if(this.EditMode && !this.TrialMode)
                      { %>
                   <p>&bull; <a href='<%= IFRMHelper.GetURL("wsp-wt.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><u>Modify Transaction Fee (WT)</u></a></p>
                   <% } %>
                   <% if (this.EditMode)
                      { %>
                   <p>&bull; <a href='<%= IFRMHelper.GetURL("wsp-upgrade.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><u>Upgrade Plan</u></a></p>
                   <p>&bull; <a href='<%= IFRMHelper.GetURL("wsp-cancel.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><u>Cancel Current Plan</u></a></p>
                   <% } %>
               </td>
            </table>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
