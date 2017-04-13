<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PromotionSummary.ascx.cs" EnableViewState="false" Inherits="SalonAddict.Modules.PromotionSummary" %>
<div class="module-promotion-summary" >
    <div class="header" >
       <a href='<%= Page.ResolveUrl("~/Promotions.aspx") %>' ><%= base.GetLocalResourceObject("lblPromotions.Text") %></a>
    </div>
    <div class="promotions" >
    <table cellpadding="0" cellspacing="0" >
       <tr>
          <td>
             <div class="hair-promotions" >
                <div class="title" ><a href='<%= Page.ResolveUrl("~/Promotions.aspx#hairdressing") %>' ><%= base.GetLocalResourceObject("lblHairdressing.Text") %></a></div>
                <div class="morelink" ><i><a href='<%= Page.ResolveUrl("~/Promotions.aspx#hairdressing") %>' ><%= base.GetLocalResourceObject("lblHairdressingAll.Text") %></a></i></div>
                <asp:ListView ID="lvh" runat="server" ItemPlaceholderID="Item" >
                   <LayoutTemplate>
                      <ol>
                         <asp:PlaceHolder ID="Item" runat="server" ></asp:PlaceHolder>
                      </ol>
                   </LayoutTemplate>
                   <ItemTemplate>
                      <li>
                          <a href='<%# Eval("URI.PathAndQuery") %>' ><%# Eval("Title") %></a>
                          <asp:Panel ID="pnl" runat="server" CssClass='<%# ((bool)Eval("IsSpecialOffer"))? "special-offer" : "description" %>' Visible='<%# (Eval("ShortDescription").ToString() != String.Empty) %>' >
                             <%# Eval("ShortDescription") %>
                          </asp:Panel>
                      </li>
                   </ItemTemplate>
                </asp:ListView>
             </div>
          </td>
          <td style="padding-left:30px;">
             <div class="beauty-promotions" >
                <div class="title" ><a href='<%= Page.ResolveUrl("~/Promotions.aspx#beauty") %>' ><%= base.GetLocalResourceObject("lblBeauty.Text") %></a></div>
                <div class="morelink" ><i><a href='<%= Page.ResolveUrl("~/Promotions.aspx#beauty") %>' ><%= base.GetLocalResourceObject("lblBeautyAll.Text") %></a></i></div>
                <asp:ListView ID="lvb" runat="server" ItemPlaceholderID="Item" >
                   <LayoutTemplate>
                      <ol>
                         <asp:PlaceHolder ID="Item" runat="server" ></asp:PlaceHolder>
                      </ol>
                   </LayoutTemplate>
                   <ItemTemplate>
                      <li>
                          <a href='<%# Eval("URI.PathAndQuery") %>' ><%# Eval("Title") %></a>
                          <asp:Panel ID="pnl" runat="server" CssClass='<%# ((bool)Eval("IsSpecialOffer"))? "special-offer" : "description" %>' Visible='<%# (Eval("ShortDescription").ToString() != String.Empty) %>' >
                             <%# Eval("ShortDescription") %>
                          </asp:Panel>
                      </li>
                   </ItemTemplate>
                </asp:ListView>
             </div>
          </td>
       </tr>
    </table>
    </div>
</div>