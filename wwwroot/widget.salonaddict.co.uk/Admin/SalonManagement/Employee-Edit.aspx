<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Employee-Edit.aspx.cs" Inherits="IFRAME.Admin.SalonManagement.Employee_EditPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Employees" />
        <div class="horizontal-line" ></div>
        <table style="margin-bottom:20px" cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:60px" ><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-employees.png" %>' alt="employee" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle;text-align:right;" >
                 <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                 <asp:Button ID="btnSave" runat="server" SkinID="SubmitButton" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
              </td>
           </tr>
        </table>
        <ajaxToolkit:TabContainer ID="pnlTabs" runat="server" AutoPostBack="false" >
           <ajaxToolkit:TabPanel ID="pnlInfo" runat="server" HeaderText="General" >
             <ContentTemplate>
                 <asp:Panel ID="pnl2" runat="server" >
                     <table style="margin-top:10px;" cellpadding="0" cellspacing="0" >
                        <tr>
                          <td style="width:500px" >
                            <table style="margin-top:10px" class="details" cellpadding="0" cellspacing="0" >
                               <tr>
                                  <td class="title" ><%= base.GetLocaleResourceString("ltrName.Text") %></td>
                                  <td class="data-item" >
                                     <asp:TextBox ID="txtName" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                                     <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtName" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                                     <ajaxToolkit:ValidatorCalloutExtender ID="valNameEx" runat="server" TargetControlID="valName" EnableViewState="false" />
                                  </td>
                               </tr>
                               <tr>
                                  <td class="title" style="vertical-align:top;"><%= base.GetLocaleResourceString("ltrAvailability.Text") %></td>
                                  <td class="data-item" >
                                     <asp:Panel ID="pnlMonday" runat="server" style="padding-bottom:4px;" >
                                        <asp:CheckBox ID="cbMonday" runat="server" Checked="true" /> <%= base.GetLocaleResourceString("cbMonday.Text") %>
                                     </asp:Panel>
                                     <asp:Panel ID="pnlTuesday" runat="server" style="padding-bottom:4px;" >
                                        <asp:CheckBox ID="cbTuesday" runat="server" Checked="true" /> <%= base.GetLocaleResourceString("cbTuesday.Text") %>
                                     </asp:Panel>
                                     <asp:Panel ID="pnlWednesday" runat="server" style="padding-bottom:4px;" >
                                        <asp:CheckBox ID="cbWednesday" runat="server" Checked="true" /> <%= base.GetLocaleResourceString("cbWednesday.Text") %>
                                     </asp:Panel>
                                     <asp:Panel ID="pnlThursday" runat="server" style="padding-bottom:4px;" >
                                        <asp:CheckBox ID="cbThursday" runat="server" Checked="true" /> <%= base.GetLocaleResourceString("cbThursday.Text") %>
                                     </asp:Panel>
                                     <asp:Panel ID="pnlFriday" runat="server" style="padding-bottom:4px;" >
                                        <asp:CheckBox ID="cbFriday" runat="server" Checked="true" /> <%= base.GetLocaleResourceString("cbFriday.Text") %>
                                     </asp:Panel>
                                     <asp:Panel ID="pnlSaturday" runat="server" style="padding-bottom:4px;" >
                                        <asp:CheckBox ID="cbSaturday" runat="server" Checked="true" /> <%= base.GetLocaleResourceString("cbSaturday.Text") %>
                                     </asp:Panel>
                                     <asp:Panel ID="pnlSunday" runat="server" style="padding-bottom:4px;" >
                                        <asp:CheckBox ID="cbSunday" runat="server" Checked="true" /> <%= base.GetLocaleResourceString("cbSunday.Text") %>
                                     </asp:Panel>
                                  </td>
                               </tr>
                            </table>
                          </td>
                          <td>
                             <%= base.GetLocaleResourceString("ltrHelp.Text") %>
                          </td>
                        </tr>
                     </table>
                 </asp:Panel>
             </ContentTemplate>
           </ajaxToolkit:TabPanel>
           <ajaxToolkit:TabPanel ID="pnlServices" runat="server" HeaderText="Qualified Services" >
              <ContentTemplate>
                <div style="margin-left:2px;" ><%= base.GetLocaleResourceString("Select") %> &nbsp; <span style="cursor:pointer;" onclick="select('ALL');" ><u><%= base.GetLocaleResourceString("All") %></u></span> &nbsp;|&nbsp; <span style="cursor:pointer;" onclick="select('NONE');" ><u><%= base.GetLocaleResourceString("None") %></u></span></div>
                <script type="text/javascript" language="javascript" >
                    function select(val) {
                        var checked = (val == "ALL");
                        var gv = document.getElementById('<%= gv.ClientID %>');
                        var inputs = gv.getElementsByTagName("input");
                        var cb = new Array();
                        for (var i = 0; i < inputs.length; i++) {
                            if (inputs[i].type.toLowerCase() == "checkbox") {
                                inputs[i].checked = checked;
                            }
                        }
                    }
                </script>
                <asp:GridView 
                    ID="gv" 
                    runat="server" 
                    ShowHeader="false"
                    AutoGenerateColumns="False" 
                    DataKeyNames="ServiceId" >
                   <Columns>
                      <asp:TemplateField>
                         <ItemTemplate>
                             <asp:CheckBox ID="cb" runat="server" />
                         </ItemTemplate>
                         <ItemStyle Width="50px" />
                      </asp:TemplateField>
                      <asp:TemplateField>
                         <ItemTemplate>
                              <%# String.Format("{0} £{1}", System.Web.HttpUtility.HtmlEncode(Eval("Name").ToString()), ((Decimal)(Eval("Price"))).ToString("#,#.00#"))%>
                         </ItemTemplate>
                      </asp:TemplateField>
                   </Columns>
                </asp:GridView>
              </ContentTemplate>
           </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
     </asp:Panel>
</asp:Content>
