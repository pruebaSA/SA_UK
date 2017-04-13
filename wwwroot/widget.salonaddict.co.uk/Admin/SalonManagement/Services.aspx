<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Services.aspx.cs" Inherits="IFRAME.Admin.SalonManagement.ServicesPage" %>
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
                  <asp:Button ID="btnAdd" runat="server" SkinID="SubmitButton" OnClick="btnAdd_Click" meta:resourceKey="btnAdd" />
              </td>
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="ServiceId"
            OnRowEditing="gv_RowEditing"
            OnRowDeleting="gv_RowDeleting" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# System.Web.HttpUtility.HtmlEncode(Eval("Name").ToString()) %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <asp:Panel ID="pnlDescriptionHeader" runat="server" >
                          <center style="cursor:pointer;" ><u><%# String.IsNullOrEmpty(Eval("ShortDescription") + "") ? String.Empty : "<strong>...</strong>" %></u></center>
                      </asp:Panel>
                      <asp:Panel ID="pnlDescriptionContent" runat="server" >
                         <%# Eval("ShortDescription")%>
                      </asp:Panel>
                      <ajaxToolkit:BalloonPopupExtender 
                        ID="pnlServicePopout" 
                        runat="server" 
                        BalloonPopupControlID="pnlDescriptionContent" 
                        BalloonSize="Medium" 
                        BalloonStyle="Cloud"
                        CacheDynamicResults="false"
                        DisplayOnClick="true"
                        DisplayOnMouseOver="false"
                        DisplayOnFocus="false"
                        Position="TopLeft"
                        TargetControlID="pnlDescriptionHeader"
                        OffsetX="0"
                        OffsetY="0"
                        UseShadow="true" />
                 </ItemTemplate>
                 <ItemStyle Width="30px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <center><%# ((Decimal)Eval("Price")).ToString("C") %></center>
                 </ItemTemplate>
                 <ItemStyle Width="60px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <asp:Panel ID="pnlEmployeeCount" runat="server" >
                          <center style="cursor:pointer;" ><u><%# Eval("Employees.Count") %></u></center>
                      </asp:Panel>
                      <asp:Panel ID="pnlEmployeeList" runat="server" >
                         <asp:Repeater ID="rptrServices" runat="server" DataSource='<%# Eval("Employees") %>' >
                            <HeaderTemplate><ol></HeaderTemplate>
                            <ItemTemplate>
                                <li><%# Eval("DisplayText")%></li>
                            </ItemTemplate>
                            <FooterTemplate></ol></FooterTemplate>
                         </asp:Repeater>
                      </asp:Panel>
                      <ajaxToolkit:BalloonPopupExtender 
                        ID="pnlEmployeePopout" 
                        runat="server" 
                        BalloonPopupControlID="pnlEmployeeList" 
                        BalloonSize="Medium" 
                        BalloonStyle="Cloud"
                        CacheDynamicResults="false"
                        DisplayOnClick="true"
                        DisplayOnMouseOver="false"
                        DisplayOnFocus="false"
                        Position="TopLeft"
                        TargetControlID="pnlEmployeeCount"
                        OffsetX="0"
                        OffsetY="0"
                        UseShadow="true" />
                 </ItemTemplate>
                 <ItemStyle Width="80px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <center style="padding-top:5px" ><asp:CheckBox ID="cb" runat="server" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" Checked='<%# (bool)Eval("Published") %>' /></center>
                 </ItemTemplate>
                 <ItemStyle Width="50px" />
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
