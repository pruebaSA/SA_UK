<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeSlotsGrid.ascx.cs" Inherits="IFRAME.Modules.TimeSlotsGrid" %>
<table class="details" cellpadding="0" cellspacing="0">
    <tr>
        <td style="vertical-align: top;padding-top:3px;" class="title" >
            <asp:Label ID="lblDate" runat="server" ></asp:Label>
        </td>
        <td style="vertical-align:top;">
            <asp:MultiView ID="mv" runat="server" ActiveViewIndex="0" >
               <asp:View ID="v0" runat="server" >
                    <asp:ListView 
                        ID="lv" 
                        runat="server" 
                        DataKeyNames="Time"
                        GroupItemCount="6" 
                        GroupPlaceholderID="GroupPlaceholder"
                        ItemPlaceholderID="ItemPlaceholder" 
                        OnItemCreated="lv_ItemCreated"
                        OnSelectedIndexChanging="lv_SelectedIndexChanging" >
                        <LayoutTemplate>
                            <table cellpadding="0" cellspacing="0">
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
                                <asp:LinkButton ID="lb" runat="server" CommandName="SELECT" ></asp:LinkButton>
                            </td>
                        </ItemTemplate>
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