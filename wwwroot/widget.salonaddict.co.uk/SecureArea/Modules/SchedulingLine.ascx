<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchedulingLine.ascx.cs" Inherits="IFRAME.SecureArea.Modules.SchedulingLine" %>
<table cellpadding="0" cellspacing="0" >
   <tr>
      <td style="vertical-align:middle;">
        <table class="details" cellpadding="0" cellspacing="0">
            <tr>
                <td style="vertical-align:top;min-width:120px;padding-top:3px;" class="title" >
                    <asp:Label ID="lblDay" runat="server" ></asp:Label>
                </td>
                <td style="vertical-align:top;" >
                    <asp:ListView 
                        ID="lv" 
                        runat="server" 
                        GroupItemCount="5" 
                        GroupPlaceholderID="GroupPlaceholder"
                        ItemPlaceholderID="ItemPlaceholder" 
                        OnItemCreated="lv_ItemCreated"
                        OnItemUpdating="lv_ItemUpdating"
                        OnItemDeleting="lv_ItemDeleting" >
                        <LayoutTemplate>
                             <table cellpadding="0" cellspacing="0" >
                                 <asp:PlaceHolder ID="GroupPlaceholder" runat="server"></asp:PlaceHolder>
                             </table>
                        </LayoutTemplate>
                        <GroupTemplate>
                            <tr>
                                <asp:PlaceHolder ID="ItemPlaceholder" runat="server"></asp:PlaceHolder>
                            </tr>
                        </GroupTemplate>
                        <ItemTemplate>
                            <td>
                                 <asp:LinkButton ID="lb" runat="server" Text='<%# new DateTime(((TimeSpan)Eval("Time")).Ticks).ToString("HH:mm") + " (" + Eval("Slots") + ")" %>' OnClientClick="return false;" CssClass="lbutton-scheduled-time" ></asp:LinkButton>
                                 <asp:Panel ID="pnlEdit" runat="server" >
                                    <table cellpadding="0" cellspacing="4" >
                                       <tr>
                                          <td style="vertical-align:middle;" >
                                             <%= base.GetLocaleResourceString("ltrSlots.Text") %>
                                          </td>
                                          <td>
                                             <asp:DropDownList ID="ddlSlots" runat="server"  SkinID="DropDownList" >
                                                <asp:ListItem Text="1" Value="1" ></asp:ListItem>
                                                <asp:ListItem Text="2" Value="2" ></asp:ListItem>
                                                <asp:ListItem Text="3" Value="3" ></asp:ListItem>
                                                <asp:ListItem Text="4" Value="4" ></asp:ListItem>
                                                <asp:ListItem Text="5" Value="5" ></asp:ListItem>
                                                <asp:ListItem Text="6" Value="6" ></asp:ListItem>
                                                <asp:ListItem Text="7" Value="7" ></asp:ListItem>
                                                <asp:ListItem Text="8" Value="8" ></asp:ListItem>
                                                <asp:ListItem Text="9" Value="9" ></asp:ListItem>
                                                <asp:ListItem Text="10" Value="10" ></asp:ListItem>
                                                <asp:ListItem Text="11" Value="11" ></asp:ListItem>
                                                <asp:ListItem Text="12" Value="12" ></asp:ListItem>
                                                <asp:ListItem Text="13" Value="13" ></asp:ListItem>
                                                <asp:ListItem Text="14" Value="14" ></asp:ListItem>
                                                <asp:ListItem Text="15" Value="15" ></asp:ListItem>
                                                <asp:ListItem Text="16" Value="16" ></asp:ListItem>
                                                <asp:ListItem Text="17" Value="17" ></asp:ListItem>
                                                <asp:ListItem Text="18" Value="18" ></asp:ListItem>
                                                <asp:ListItem Text="19" Value="19" ></asp:ListItem>
                                                <asp:ListItem Text="20" Value="20" ></asp:ListItem>
                                             </asp:DropDownList>
                                          </td>
                                       </tr>
                                       <tr>
                                          <td colspan="2" >&nbsp;</td>
                                       </tr>
                                       <tr>
                                          <td colspan="2">
                                              <asp:Button ID="btnUpdate" runat="server" SkinID="SubmitButtonMini" CommandName="Update" CommandArgument='<%# Eval("Time") %>' meta:resourceKey="btnUpdate" />
                                              <asp:Button ID="btnDelete" runat="server" SkinID="SubmitButtonMini" CommandName="Delete" meta:resourceKey="btnDelete" />
                                          </td>
                                       </tr>
                                    </table>
                                 </asp:Panel>
                                 <ajaxToolkit:BalloonPopupExtender 
                                    ID="pnlEditEx" 
                                    runat="server"
                                    BalloonPopupControlID="pnlEdit" 
                                    BalloonSize="Small" 
                                    BalloonStyle="Rectangle"
                                    CacheDynamicResults="false"
                                    DisplayOnClick="true"
                                    DisplayOnMouseOver="false"
                                    DisplayOnFocus="false"
                                    Position="TopLeft"
                                    OffsetX="15"
                                    OffsetY="10"
                                    TargetControlID="lb"
                                    UseShadow="false" />
                            </td>
                        </ItemTemplate>
                        <SelectedItemTemplate></SelectedItemTemplate>
                    </asp:ListView>
                </td>
            </tr>
         </table>
      </td>
      <td style="vertical-align:top;" >
         <asp:MultiView ID="mv" runat="server" ActiveViewIndex="0" >
            <asp:View ID="v0" runat="server" >
                 <asp:LinkButton ID="lbAdd" runat="server" SkinID="SchedulingAddButton" OnClientClick="return false;" meta:resourceKey="lbAdd" ></asp:LinkButton>
                 <asp:Panel ID="pnlAdd" runat="server" >
                    <table cellpadding="0" cellspacing="8" >
                       <tr>
                          <td style="vertical-align:middle" ><%= base.GetLocaleResourceString("ltrTime.Text") %></td>
                          <td>
                             <asp:DropDownList ID="ddlTimes" runat="server" SkinID="DropDownList" ></asp:DropDownList>
                          </td>
                       </tr>
                       <tr>
                          <td style="vertical-align:middle" ><%= base.GetLocaleResourceString("ltrSlots.Text") %></td>
                          <td>
                             <asp:DropDownList ID="ddlSlots" runat="server" Enabled="false" SkinID="DropDownList" AutoPostBack="true" OnSelectedIndexChanged="ddlSlots_SelectedIndexChanged" >
                                <asp:ListItem Text="" Value="" ></asp:ListItem>
                                <asp:ListItem Text="1" Value="1" ></asp:ListItem>
                                <asp:ListItem Text="2" Value="2" ></asp:ListItem>
                                <asp:ListItem Text="3" Value="3" ></asp:ListItem>
                                <asp:ListItem Text="4" Value="4" ></asp:ListItem>
                                <asp:ListItem Text="5" Value="5" ></asp:ListItem>
                                <asp:ListItem Text="6" Value="6" ></asp:ListItem>
                                <asp:ListItem Text="7" Value="7" ></asp:ListItem>
                                <asp:ListItem Text="8" Value="8" ></asp:ListItem>
                                <asp:ListItem Text="9" Value="9" ></asp:ListItem>
                                <asp:ListItem Text="10" Value="10" ></asp:ListItem>
                                <asp:ListItem Text="11" Value="11" ></asp:ListItem>
                                <asp:ListItem Text="12" Value="12" ></asp:ListItem>
                                <asp:ListItem Text="13" Value="13" ></asp:ListItem>
                                <asp:ListItem Text="14" Value="14" ></asp:ListItem>
                                <asp:ListItem Text="15" Value="15" ></asp:ListItem>
                                <asp:ListItem Text="16" Value="16" ></asp:ListItem>
                                <asp:ListItem Text="17" Value="17" ></asp:ListItem>
                                <asp:ListItem Text="18" Value="18" ></asp:ListItem>
                                <asp:ListItem Text="19" Value="19" ></asp:ListItem>
                                <asp:ListItem Text="20" Value="20" ></asp:ListItem>
                             </asp:DropDownList>
                          </td>
                       </tr>
                    </table>
                     <script type="text/javascript" language="javascript" >
                         var __times = document.getElementById('<%= ddlTimes.ClientID %>');

                         __times.onchange = function() {
                             var ddlSlots = document.getElementById('<%= ddlSlots.ClientID %>');
                             if (this.options[this.selectedIndex].value == '') {
                                 ddlSlots.selectedIndex = 0;
                                 ddlSlots.disabled = true;
                             }
                             else {
                                 ddlSlots.disabled = false;
                             }
                         }
                     </script>
                 </asp:Panel>
                 <ajaxToolkit:BalloonPopupExtender 
                    ID="pnlAddEx" 
                    runat="server"
                    BalloonPopupControlID="pnlAdd" 
                    BalloonSize="Small" 
                    BalloonStyle="Rectangle"
                    CacheDynamicResults="false"
                    DisplayOnClick="true"
                    DisplayOnMouseOver="false"
                    DisplayOnFocus="false"
                    Position="TopLeft"
                    OffsetX="15"
                    OffsetY="10"
                    TargetControlID="lbAdd"
                    UseShadow="false" />
            </asp:View>
            <asp:View ID="v1" runat="server" >
                <div style="padding-top:3px;" >
                   <%= base.GetLocaleResourceString("ltrClosed.Text") %>
                </div>
            </asp:View>
         </asp:MultiView>
      </td>
   </tr>
</table>