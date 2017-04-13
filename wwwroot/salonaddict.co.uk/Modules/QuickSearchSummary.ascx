<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuickSearchSummary.ascx.cs" EnableViewState="false" Inherits="SalonAddict.Modules.QuickSearchSummary" %>
<div class="module-quicksearch-summary" >
<div class="header" ><a href='<%= Page.ResolveUrl("~/QuickSearch.aspx") %>'><%= base.GetLocalResourceObject("lblHeader.Text") %></a></div>
<div class="quicksearches" >
<div class="hair-quicksearch" >
    <div class="title" ><a href='<%= Page.ResolveUrl("~/QuickSearch.aspx#hairdressing") %>'><%= base.GetLocalResourceObject("lblHairdressing.Text") %></a></div>
    <asp:ListView ID="lvh" runat="server" ItemPlaceholderID="Item" >
       <LayoutTemplate>
          <ol>
             <asp:PlaceHolder ID="Item" runat="server" ></asp:PlaceHolder>
          </ol>
       </LayoutTemplate>
       <ItemTemplate>
          <li>
              <a href='<%# Eval("URI.PathAndQuery") %>' ><%# Eval("Title") %></a>
              <span class="pink" ><%= base.GetLocalResourceObject("lblPrice.Text") %> <%# SalonAddict.BusinessAccess.Implementation.CurrencyManager.FormatPrice((decimal)Eval("Price"), false) %></span>
          </li>
       </ItemTemplate>
    </asp:ListView>
</div>
<div class="beauty-quicksearch" >
    <div class="title" ><a href='<%= Page.ResolveUrl("~/QuickSearch.aspx#beauty") %>'><%= base.GetLocalResourceObject("lblBeauty.Text") %></a></div>
    <asp:ListView ID="lvb" runat="server" ItemPlaceholderID="Item" >
       <LayoutTemplate>
          <ol>
             <asp:PlaceHolder ID="Item" runat="server" ></asp:PlaceHolder>
          </ol>
       </LayoutTemplate>
       <ItemTemplate>
          <li>
              <a href='<%# Eval("URI.PathAndQuery") %>' ><%# Eval("Title") %></a>
              <span style="color:#212121"><%= base.GetLocalResourceObject("lblPrice.Text") %> <%# SalonAddict.BusinessAccess.Implementation.CurrencyManager.FormatPrice((decimal)Eval("Price"), false) %></span>
          </li>
       </ItemTemplate>
    </asp:ListView>
 </div>
 <div class="morelink" ><i><a href='<%= Page.ResolveUrl("~/QuickSearch.aspx") %>' ><%= base.GetLocalResourceObject("lblQuickSearchAll.Text") %></a></i></div>
 </div>
</div>