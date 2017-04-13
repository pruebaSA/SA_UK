<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Employees.aspx.cs" Inherits="IFRAME.Admin.SalonManagement.EmployeesPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Employees" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-employees.png" %>' alt="employees" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
                  <asp:Button ID="btnAdd" runat="server" SkinID="SubmitButton" OnClick="btnAdd_Click" meta:resourceKey="btnAdd" />
              </td>
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="EmployeeId"
            OnRowEditing="gv_RowEditing"
            OnRowDeleting="gv_RowDeleting" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# System.Web.HttpUtility.HtmlEncode(Eval("Employee.DisplayText").ToString()) %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <asp:Panel ID="pnlServiceCount" runat="server" >
                          <center style="cursor:pointer;" ><u><%# Eval("Services.Count") %></u></center>
                      </asp:Panel>
                      <asp:Panel ID="pnlServiceList" runat="server" >
                         <asp:Repeater ID="rptrServices" runat="server" DataSource='<%# Eval("Services") %>' >
                            <HeaderTemplate><ol></HeaderTemplate>
                            <ItemTemplate>
                                <li><%# String.Format("{0} £{1}", Eval("Name"), ((Decimal)Eval("Price")).ToString("#,#.00#"))%></li>
                            </ItemTemplate>
                            <FooterTemplate></ol></FooterTemplate>
                         </asp:Repeater>
                      </asp:Panel>
                      <ajaxToolkit:BalloonPopupExtender 
                        ID="pnlServicePopout" 
                        runat="server" 
                        BalloonPopupControlID="pnlServiceList" 
                        BalloonSize="Medium" 
                        BalloonStyle="Cloud"
                        CacheDynamicResults="false"
                        DisplayOnClick="true"
                        DisplayOnMouseOver="false"
                        DisplayOnFocus="false"
                        Position="TopLeft"
                        TargetControlID="pnlServiceCount"
                        OffsetX="0"
                        OffsetY="0"
                        UseShadow="true" />
                 </ItemTemplate>
                 <ItemStyle Width="80px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                    <center>
                        <asp:ImageButton ID="ibEdit" runat="server" SkinID="GridEditImageButton" CommandName="Edit" meta:resourceKey="ibEdit" />
                        &nbsp;
                        <asp:ImageButton ID="ibRemove" runat="server" SkinID="GridRemoveImageButton" CommandName="Delete" meta:resourceKey="ibRemove" />
                    </center>
                 </ItemTemplate>
                 <ItemStyle Width="60px" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>
