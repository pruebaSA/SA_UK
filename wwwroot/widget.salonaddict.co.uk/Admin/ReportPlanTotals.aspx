<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="ReportPlanTotals.aspx.cs" Inherits="IFRAME.Admin.ReportPlanTotalsPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Reports" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-reports.png" %>' alt="reports" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocalResourceObject("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle;" ></td>
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False" 
            EnableViewState="false" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                     <center><%# Eval("OverallTrial") %></center>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <center><%# Eval("OverallMonthly") %></center>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <center><%# Eval("OverallAnnual") %></center>
                 </ItemTemplate>
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
        <p>
           <%= base.GetLocaleResourceString("ltrSalonTotal.Text") %> <asp:Literal ID="ltrSalonCount" runat="server" ></asp:Literal>
        </p>
   </asp:Panel>
</asp:Content>