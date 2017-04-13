<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="ReportPlanExpiry.aspx.cs" Inherits="IFRAME.Admin.ReportPlanExpiryPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Reports" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-reports.png" %>' alt="reports" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocalResourceObject("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle;" >
                  <asp:DropDownList ID="ddlPlanType" runat="server" AutoPostBack="true" SkinID="DropDownList" OnSelectedIndexChanged="ddlPlanType_SelectedIndexChanged" >
                      <asp:ListItem Text="View Trial Plans" Value="10" ></asp:ListItem>
                      <asp:ListItem Text="View Monthly Plans" Value="30" ></asp:ListItem>
                      <asp:ListItem Text="View Annual Plans" Value="100" ></asp:ListItem>
                  </asp:DropDownList>
                  &nbsp;&nbsp;
                  <asp:DropDownList ID="ddlExpiry" runat="server" AutoPostBack="true" SkinID="DropDownList" OnSelectedIndexChanged="ddlExpiry_SelectedIndexChanged" >
                      <asp:ListItem Text="Expiring in 1 day" Value="1" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 2 days" Value="2" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 3 days" Value="3" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 4 days" Value="4" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 5 days" Value="5" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 6 days" Value="6" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 7 days" Value="7" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 8 days" Value="8" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 9 days" Value="9" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 10 days" Value="10" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 11 days" Value="11" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 12 days" Value="12" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 13 days" Value="13" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 14 days" Value="14" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 15 days" Value="15" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 16 days" Value="16" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 17 days" Value="17" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 18 days" Value="18" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 19 days" Value="19" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 20 days" Value="20" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 21 days" Value="21" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 22 days" Value="22" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 23 days" Value="23" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 24 days" Value="24" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 25 days" Value="25" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 26 days" Value="26" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 27 days" Value="27" ></asp:ListItem>
                      <asp:ListItem Text="Expiring in 28 days" Value="28" ></asp:ListItem>
                  </asp:DropDownList>
              </td>
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="SalonId"
            EnableViewState="false" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("Name") %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("AddressLine1") + "<div>" + Eval("AddressLine3") + "<div>" %>
                 </ItemTemplate>
                 <ItemStyle Width="220px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("PlanDescription")%> (<%# ((double)int.Parse(Eval("PlanPrice").ToString()) / 100).ToString("C") %>)
                      <div style="padding-top:5px;" >
                           <%# IFRMHelper.FromUrlFriendlyDate(Eval("PlanStartDate").ToString()).ToString("dd MMM yyyy")%> - <%# IFRMHelper.FromUrlFriendlyDate(Eval("PlanEndDate").ToString()).ToString("dd MMM yyyy")%>
                      </div>
                 </ItemTemplate>
                 <ItemStyle Width="220px" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
        <div style="margin-top:10px;" >
            <IFRM:IFRMPager ID="cntrlPager" runat="server" PageSize="15" CssClass="pager" OnPageCreated="cntrlPager_PageCreated" meta:resourceKey="Pager" ></IFRM:IFRMPager>
        </div>
   </asp:Panel>
</asp:Content>