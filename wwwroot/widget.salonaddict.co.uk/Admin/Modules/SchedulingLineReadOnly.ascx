<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchedulingLineReadOnly.ascx.cs" EnableViewState="false" Inherits="IFRAME.Admin.Modules.SchedulingLineReadOnly" %>
<table class="details" cellpadding="0" cellspacing="0">
    <tr>
        <td style="vertical-align: top;min-width:120px;padding-top:3px;" class="title" >
            <asp:Label ID="lblDay" runat="server" ></asp:Label>
        </td>
        <td style="vertical-align:top;" >
            <asp:MultiView ID="mv" runat="server" ActiveViewIndex="0" >
               <asp:View ID="v0" runat="server" >
                <asp:ListView 
                    ID="lv" 
                    runat="server" 
                    GroupItemCount="5" 
                    GroupPlaceholderID="GroupPlaceholder"
                    ItemPlaceholderID="ItemPlaceholder" >
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
                             <asp:LinkButton ID="lb" runat="server" Text='<%# new DateTime(((TimeSpan)Eval("Time")).Ticks).ToString("HH:mm") + " (" + Eval("Slots") + ")" %>' OnClientClick="return false;" CssClass="lbutton-scheduled-time-readonly" ></asp:LinkButton>
                        </td>
                    </ItemTemplate>
                    <SelectedItemTemplate></SelectedItemTemplate>
                </asp:ListView>
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
