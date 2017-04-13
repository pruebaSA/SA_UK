<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="ReportSalonBillingTotals.aspx.cs" Inherits="IFRAME.Admin.ReportSalonBillingTotalsPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Billing" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-reports.png" %>' alt="reports" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><asp:Literal ID="ltrHeader" runat="server" EnableViewState="false" ></asp:Literal></h1></td>
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
                      <%# ((double)((int)Eval("SumToday")) / 100).ToString("C")%> (<%# Eval("CountToday")%>)
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# ((double)((int)Eval("SumThisWeek")) / 100).ToString("C") %> (<%# Eval("CountThisWeek")%>)
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# ((double)((int)Eval("SumThisMonth")) / 100).ToString("C") %> (<%# Eval("CountThisMonth")%>)
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# ((double)((int)Eval("SumThisYear")) / 100).ToString("C") %> (<%# Eval("CountThisYear")%>)
                 </ItemTemplate>
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
   </asp:Panel>
</asp:Content>