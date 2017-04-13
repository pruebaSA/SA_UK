<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Appointments.aspx.cs" Inherits="IFRAME.SecureArea.AppointmentsPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="IFRM" TagName="DateTextBox" Src="~/Modules/DateTextBox.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Appointments" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-appointments.png" %>' alt="appointments" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
              </td>
           </tr>
        </table>
        <table style="margin-top:15px" cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:100%" >
              <asp:UpdatePanel ID="up" runat="server" UpdateMode="Conditional" >
                <ContentTemplate>
                <asp:GridView 
                    ID="gv" 
                    runat="server" 
                    EnableViewState="false"
                    AutoGenerateColumns="False"
                    OnRowCreated="gv_RowCreated"
                    Width="100%" >
                   <Columns>
                      <asp:TemplateField>
                         <ItemTemplate>
                             <asp:HyperLink ID="hlMail" runat="server" NavigateUrl='<%# IFRMHelper.GetURL("appointment-details.aspx", String.Format("{0}={1}", Constants.QueryStrings.TICKET_ID, Eval("TicketId"))) %>' >
                                <asp:Image ID="imgMail" runat="server" />
                             </asp:HyperLink>
                         </ItemTemplate>
                         <ItemStyle Width="25px" />
                      </asp:TemplateField>
                      <asp:TemplateField>
                         <ItemTemplate>
                              <asp:Label ID="lblClient" runat="server" Text='<%# Eval("CustomerDisplayText") %>' ></asp:Label>
                         </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField>
                         <ItemTemplate>
                              <div style="margin-bottom:6px" >
                                  <asp:Label ID="lblService" runat="server" Text='<%# Eval("ServiceDisplayText") %>' ></asp:Label>
                              </div>
                         </ItemTemplate>
                      </asp:TemplateField>
                     <asp:TemplateField>
                         <ItemTemplate>
                             <asp:Label ID="lblDate" runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToString("ddd dd MMM yyyy") %>' ></asp:Label>
                         </ItemTemplate>
                         <ItemStyle Width="120px" />
                      </asp:TemplateField>
                      <asp:TemplateField>
                         <ItemTemplate>
                             <asp:Label ID="lblTime" runat="server" Text='<%# new DateTime(((TimeSpan)Eval("StartTime")).Ticks).ToString("HH:mm") %>' />
                         </ItemTemplate>
                         <ItemStyle Width="45px" />
                      </asp:TemplateField>
                      <asp:TemplateField>
                         <ItemTemplate>
                              £<asp:Label ID="lblTotal" runat="server" Text='<%# ((decimal)Eval("RowTotal")).ToString("#,#.00#") %>' ></asp:Label>
                         </ItemTemplate>
                         <ItemStyle Width="40px" />
                      </asp:TemplateField>
                      <asp:TemplateField>
                         <ItemTemplate>
                              <asp:HyperLink ID="hlDetails" runat="server" NavigateUrl='<%# IFRMHelper.GetURL("appointment-details.aspx", String.Format("{0}={1}", Constants.QueryStrings.TICKET_ID, Eval("TicketId"))) %>'><u><%= base.GetLocaleResourceString("hlDetails.Text") %></u></asp:HyperLink>
                         </ItemTemplate>
                         <ItemStyle Width="40px" />
                      </asp:TemplateField>
                   </Columns>
                </asp:GridView>
                </ContentTemplate>
                   <Triggers>
                      <asp:AsyncPostBackTrigger ControlID="Timer" EventName="Tick" />
                   </Triggers>
                </asp:UpdatePanel>
              </td>
           </tr>
        </table>
        <asp:Timer ID="Timer" runat="server" OnTick="Timer_Tick" ></asp:Timer>
    </asp:Panel>
</asp:Content>