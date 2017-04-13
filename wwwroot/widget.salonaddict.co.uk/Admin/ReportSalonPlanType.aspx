<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="ReportSalonPlanType.aspx.cs" Inherits="IFRAME.Admin.ReportSalonPlanTypePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
<asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Reports" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-salons.png" %>' alt="salons" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
                  <asp:DropDownList ID="ddlPlanType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPlanType_SelectedIndexChanged" >
                      <asp:ListItem Text="View Trial Plans" Value="10" ></asp:ListItem>
                      <asp:ListItem Text="View Monthly Plans" Value="30" ></asp:ListItem>
                      <asp:ListItem Text="View Annual Plans" Value="100" ></asp:ListItem>
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
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <center><%# Eval("PhoneNumber")%></center>
                 </ItemTemplate>
                 <ItemStyle Width="80px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                    <center><%# ((DateTime)Eval("CreatedOnUtc")).ToString("MMM dd yyyy") %></center>
                 </ItemTemplate>
                 <ItemStyle Width="100px" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
        <div style="margin-top:10px;" >
            <IFRM:IFRMPager ID="cntrlPager" runat="server" PageSize="15" CssClass="pager" OnPageCreated="cntrlPager_PageCreated" meta:resourceKey="Pager" ></IFRM:IFRMPager>
        </div>
    </asp:Panel>
</asp:Content>
