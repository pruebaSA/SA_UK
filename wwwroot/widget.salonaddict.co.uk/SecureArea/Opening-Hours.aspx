<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Opening-Hours.aspx.cs" Inherits="IFRAME.SecureArea.Opening_HoursPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Settings" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-settings.png" %>' alt="settings" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
           </tr>
        </table>
        <table cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:460px;" >
                <table style="margin-top:10px;" class="details" cellpadding="0" cellspacing="0" >
                   <tr>
                      <td class="title" >
                           <asp:CheckBox ID="cbMonday" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cbMonday_CheckedChanged" />
                           <%= base.GetLocaleResourceString("ltrMonday.Text") %>
                      </td>
                      <td class="data-item" >
                         <table cellpadding="0" cellspacing="0" >
                            <tr>
                               <td>
                                 <table cellpadding="0" cellspacing="0" >
                                    <tr>
                                       <td>
                                           <asp:DropDownList ID="ddlMondayStartHour" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="06" Value="06" ></asp:ListItem>
                                              <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                              <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                              <asp:ListItem Text="09" Value="09" ></asp:ListItem>
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
                                              <asp:ListItem Text="21" Value="21" ></asp:ListItem>
                                              <asp:ListItem Text="22" Value="22" ></asp:ListItem>
                                           </asp:DropDownList>
                                       </td>
                                       <td style="vertical-align:middle" >&nbsp;:&nbsp;</td>
                                       <td>
                                          <asp:DropDownList ID="ddlMondayStartMinute" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="00" Value="00" ></asp:ListItem>
                                              <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                          </asp:DropDownList>
                                       </td>
                                    </tr>
                                 </table>
                               </td>
                               <td style="width:40px;text-align:center;vertical-align:middle;font-size:28px;" >-</td>
                               <td>
                                 <table cellpadding="0" cellspacing="0" >
                                    <tr>
                                       <td>
                                           <asp:DropDownList ID="ddlMondayEndHour" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="06" Value="06" ></asp:ListItem>
                                              <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                              <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                              <asp:ListItem Text="09" Value="09" ></asp:ListItem>
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
                                              <asp:ListItem Text="21" Value="21" ></asp:ListItem>
                                              <asp:ListItem Text="22" Value="22" ></asp:ListItem>
                                           </asp:DropDownList>
                                       </td>
                                       <td style="vertical-align:middle" >&nbsp;:&nbsp;</td>
                                       <td>
                                          <asp:DropDownList ID="ddlMondayEndMinute" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="00" Value="00" ></asp:ListItem>
                                              <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                          </asp:DropDownList>
                                       </td>
                                    </tr>
                                 </table>
                               </td>
                            </tr>
                         </table>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <asp:CheckBox ID="cbTuesday" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cbTuesday_CheckedChanged" />
                         <%= base.GetLocaleResourceString("ltrTuesday.Text") %>
                      </td>
                      <td class="data-item" >
                         <table cellpadding="0" cellspacing="0" >
                            <tr>
                               <td>
                                 <table cellpadding="0" cellspacing="0" >
                                    <tr>
                                       <td>
                                           <asp:DropDownList ID="ddlTuesdayStartHour" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="06" Value="06" ></asp:ListItem>
                                              <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                              <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                              <asp:ListItem Text="09" Value="09" ></asp:ListItem>
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
                                              <asp:ListItem Text="21" Value="21" ></asp:ListItem>
                                              <asp:ListItem Text="22" Value="22" ></asp:ListItem>
                                           </asp:DropDownList>
                                       </td>
                                       <td style="vertical-align:middle" >&nbsp;:&nbsp;</td>
                                       <td>
                                          <asp:DropDownList ID="ddlTuesdayStartMinute" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="00" Value="00" ></asp:ListItem>
                                              <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                          </asp:DropDownList>
                                       </td>
                                    </tr>
                                 </table>
                               </td>
                               <td style="width:40px;text-align:center;vertical-align:middle;font-size:28px;" >-</td>
                               <td>
                                 <table cellpadding="0" cellspacing="0" >
                                    <tr>
                                       <td>
                                           <asp:DropDownList ID="ddlTuesdayEndHour" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="06" Value="06" ></asp:ListItem>
                                              <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                              <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                              <asp:ListItem Text="09" Value="09" ></asp:ListItem>
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
                                              <asp:ListItem Text="21" Value="21" ></asp:ListItem>
                                              <asp:ListItem Text="22" Value="22" ></asp:ListItem>
                                           </asp:DropDownList>
                                       </td>
                                       <td style="vertical-align:middle" >&nbsp;:&nbsp;</td>
                                       <td>
                                          <asp:DropDownList ID="ddlTuesdayEndMinute" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="00" Value="00" ></asp:ListItem>
                                              <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                          </asp:DropDownList>
                                       </td>
                                    </tr>
                                 </table>
                               </td>
                            </tr>
                         </table>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <asp:CheckBox ID="cbWednesday" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cbWednesday_CheckedChanged" />
                         <%= base.GetLocaleResourceString("ltrWednesday.Text") %>
                      </td>
                      <td class="data-item" >
                         <table cellpadding="0" cellspacing="0" >
                            <tr>
                               <td>
                                 <table cellpadding="0" cellspacing="0" >
                                    <tr>
                                       <td>
                                           <asp:DropDownList ID="ddlWednesdayStartHour" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="06" Value="06" ></asp:ListItem>
                                              <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                              <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                              <asp:ListItem Text="09" Value="09" ></asp:ListItem>
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
                                              <asp:ListItem Text="21" Value="21" ></asp:ListItem>
                                              <asp:ListItem Text="22" Value="22" ></asp:ListItem>
                                           </asp:DropDownList>
                                       </td>
                                       <td style="vertical-align:middle" >&nbsp;:&nbsp;</td>
                                       <td>
                                          <asp:DropDownList ID="ddlWednesdayStartMinute" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="00" Value="00" ></asp:ListItem>
                                              <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                          </asp:DropDownList>
                                       </td>
                                    </tr>
                                 </table>
                               </td>
                               <td style="width:40px;text-align:center;vertical-align:middle;font-size:28px;" >-</td>
                               <td>
                                 <table cellpadding="0" cellspacing="0" >
                                    <tr>
                                       <td>
                                           <asp:DropDownList ID="ddlWednesdayEndHour" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="06" Value="06" ></asp:ListItem>
                                              <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                              <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                              <asp:ListItem Text="09" Value="09" ></asp:ListItem>
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
                                              <asp:ListItem Text="21" Value="21" ></asp:ListItem>
                                              <asp:ListItem Text="22" Value="22" ></asp:ListItem>
                                           </asp:DropDownList>
                                       </td>
                                       <td style="vertical-align:middle" >&nbsp;:&nbsp;</td>
                                       <td>
                                          <asp:DropDownList ID="ddlWednesdayEndMinute" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="00" Value="00" ></asp:ListItem>
                                              <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                          </asp:DropDownList>
                                       </td>
                                    </tr>
                                 </table>
                               </td>
                            </tr>
                         </table>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <asp:CheckBox ID="cbThursday" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cbThursday_CheckedChanged" />
                         <%= base.GetLocaleResourceString("ltrThursday.Text") %>
                      </td>
                      <td class="data-item" >
                         <table cellpadding="0" cellspacing="0" >
                            <tr>
                               <td>
                                 <table cellpadding="0" cellspacing="0" >
                                    <tr>
                                       <td>
                                           <asp:DropDownList ID="ddlThursdayStartHour" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="06" Value="06" ></asp:ListItem>
                                              <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                              <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                              <asp:ListItem Text="09" Value="09" ></asp:ListItem>
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
                                              <asp:ListItem Text="21" Value="21" ></asp:ListItem>
                                              <asp:ListItem Text="22" Value="22" ></asp:ListItem>
                                           </asp:DropDownList>
                                       </td>
                                       <td style="vertical-align:middle" >&nbsp;:&nbsp;</td>
                                       <td>
                                          <asp:DropDownList ID="ddlThursdayStartMinute" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="00" Value="00" ></asp:ListItem>
                                              <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                          </asp:DropDownList>
                                       </td>
                                    </tr>
                                 </table>
                               </td>
                               <td style="width:40px;text-align:center;vertical-align:middle;font-size:28px;" >-</td>
                               <td>
                                 <table cellpadding="0" cellspacing="0" >
                                    <tr>
                                       <td>
                                           <asp:DropDownList ID="ddlThursdayEndHour" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="06" Value="06" ></asp:ListItem>
                                              <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                              <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                              <asp:ListItem Text="09" Value="09" ></asp:ListItem>
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
                                              <asp:ListItem Text="21" Value="21" ></asp:ListItem>
                                              <asp:ListItem Text="22" Value="22" ></asp:ListItem>
                                           </asp:DropDownList>
                                       </td>
                                       <td style="vertical-align:middle" >&nbsp;:&nbsp;</td>
                                       <td>
                                          <asp:DropDownList ID="ddlThursdayEndMinute" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="00" Value="00" ></asp:ListItem>
                                              <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                          </asp:DropDownList>
                                       </td>
                                    </tr>
                                 </table>
                               </td>
                            </tr>
                         </table>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <asp:CheckBox ID="cbFriday" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cbFriday_CheckedChanged" />
                         <%= base.GetLocaleResourceString("ltrFriday.Text") %>
                      </td>
                      <td class="data-item" >
                         <table cellpadding="0" cellspacing="0" >
                            <tr>
                               <td>
                                 <table cellpadding="0" cellspacing="0" >
                                    <tr>
                                       <td>
                                           <asp:DropDownList ID="ddlFridayStartHour" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="06" Value="06" ></asp:ListItem>
                                              <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                              <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                              <asp:ListItem Text="09" Value="09" ></asp:ListItem>
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
                                              <asp:ListItem Text="21" Value="21" ></asp:ListItem>
                                              <asp:ListItem Text="22" Value="22" ></asp:ListItem>
                                           </asp:DropDownList>
                                       </td>
                                       <td style="vertical-align:middle" >&nbsp;:&nbsp;</td>
                                       <td>
                                          <asp:DropDownList ID="ddlFridayStartMinute" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="00" Value="00" ></asp:ListItem>
                                              <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                          </asp:DropDownList>
                                       </td>
                                    </tr>
                                 </table>
                               </td>
                               <td style="width:40px;text-align:center;vertical-align:middle;font-size:28px;" >-</td>
                               <td>
                                 <table cellpadding="0" cellspacing="0" >
                                    <tr>
                                       <td>
                                           <asp:DropDownList ID="ddlFridayEndHour" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="06" Value="06" ></asp:ListItem>
                                              <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                              <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                              <asp:ListItem Text="09" Value="09" ></asp:ListItem>
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
                                              <asp:ListItem Text="21" Value="21" ></asp:ListItem>
                                              <asp:ListItem Text="22" Value="22" ></asp:ListItem>
                                           </asp:DropDownList>
                                       </td>
                                       <td style="vertical-align:middle" >&nbsp;:&nbsp;</td>
                                       <td>
                                          <asp:DropDownList ID="ddlFridayEndMinute" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="00" Value="00" ></asp:ListItem>
                                              <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                          </asp:DropDownList>
                                       </td>
                                    </tr>
                                 </table>
                               </td>
                            </tr>
                         </table>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                          <asp:CheckBox ID="cbSaturday" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cbSaturday_CheckedChanged" />
                          <%= base.GetLocaleResourceString("ltrSaturday.Text") %>
                      </td>
                      <td class="data-item" >
                         <table cellpadding="0" cellspacing="0" >
                            <tr>
                               <td>
                                 <table cellpadding="0" cellspacing="0" >
                                    <tr>
                                       <td>
                                           <asp:DropDownList ID="ddlSaturdayStartHour" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="06" Value="06" ></asp:ListItem>
                                              <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                              <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                              <asp:ListItem Text="09" Value="09" ></asp:ListItem>
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
                                              <asp:ListItem Text="21" Value="21" ></asp:ListItem>
                                              <asp:ListItem Text="22" Value="22" ></asp:ListItem>
                                           </asp:DropDownList>
                                       </td>
                                       <td style="vertical-align:middle" >&nbsp;:&nbsp;</td>
                                       <td>
                                          <asp:DropDownList ID="ddlSaturdayStartMinute" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="00" Value="00" ></asp:ListItem>
                                              <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                          </asp:DropDownList>
                                       </td>
                                    </tr>
                                 </table>
                               </td>
                               <td style="width:40px;text-align:center;vertical-align:middle;font-size:28px;" >-</td>
                               <td>
                                 <table cellpadding="0" cellspacing="0" >
                                    <tr>
                                       <td>
                                           <asp:DropDownList ID="ddlSaturdayEndHour" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="06" Value="06" ></asp:ListItem>
                                              <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                              <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                              <asp:ListItem Text="09" Value="09" ></asp:ListItem>
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
                                              <asp:ListItem Text="21" Value="21" ></asp:ListItem>
                                              <asp:ListItem Text="22" Value="22" ></asp:ListItem>
                                           </asp:DropDownList>
                                       </td>
                                       <td style="vertical-align:middle" >&nbsp;:&nbsp;</td>
                                       <td>
                                          <asp:DropDownList ID="ddlSaturdayEndMinute" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="00" Value="00" ></asp:ListItem>
                                              <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                          </asp:DropDownList>
                                       </td>
                                    </tr>
                                 </table>
                               </td>
                            </tr>
                         </table>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                          <asp:CheckBox ID="cbSunday" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cbSunday_CheckedChanged" />
                          <%= base.GetLocaleResourceString("ltrSunday.Text") %>
                      </td>
                      <td class="data-item" >
                         <table cellpadding="0" cellspacing="0" >
                            <tr>
                               <td>
                                 <table cellpadding="0" cellspacing="0" >
                                    <tr>
                                       <td>
                                           <asp:DropDownList ID="ddlSundayStartHour" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="06" Value="06" ></asp:ListItem>
                                              <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                              <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                              <asp:ListItem Text="09" Value="09" ></asp:ListItem>
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
                                              <asp:ListItem Text="21" Value="21" ></asp:ListItem>
                                              <asp:ListItem Text="22" Value="22" ></asp:ListItem>
                                           </asp:DropDownList>
                                       </td>
                                       <td style="vertical-align:middle" >&nbsp;:&nbsp;</td>
                                       <td>
                                          <asp:DropDownList ID="ddlSundayStartMinute" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="00" Value="00" ></asp:ListItem>
                                              <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                          </asp:DropDownList>
                                       </td>
                                    </tr>
                                 </table>
                               </td>
                               <td style="width:40px;text-align:center;vertical-align:middle;font-size:28px;" >-</td>
                               <td>
                                 <table cellpadding="0" cellspacing="0" >
                                    <tr>
                                       <td>
                                           <asp:DropDownList ID="ddlSundayEndHour" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="06" Value="06" ></asp:ListItem>
                                              <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                              <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                              <asp:ListItem Text="09" Value="09" ></asp:ListItem>
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
                                              <asp:ListItem Text="21" Value="21" ></asp:ListItem>
                                              <asp:ListItem Text="22" Value="22" ></asp:ListItem>
                                           </asp:DropDownList>
                                       </td>
                                       <td style="vertical-align:middle" >&nbsp;:&nbsp;</td>
                                       <td>
                                          <asp:DropDownList ID="ddlSundayEndMinute" runat="server" SkinID="DropDownList" >
                                              <asp:ListItem Text="00" Value="00" ></asp:ListItem>
                                              <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                          </asp:DropDownList>
                                       </td>
                                    </tr>
                                 </table>
                               </td>
                            </tr>
                         </table>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ></td>
                      <td class="data-item" >
                          <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButtonSecure" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                          <asp:Button ID="btnSave" runat="server" SkinID="SubmitButtonSecure" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
                      </td>
                   </tr>
                </table>
             </td>
             <td>
                 <ul>
                    <li><p><%= base.GetLocaleResourceString("ltrHelp.Text") %></p></li>
                 </ul>
             </td>
          </tr>
        </table>
     </asp:Panel>
</asp:Content>
