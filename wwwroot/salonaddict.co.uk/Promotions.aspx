<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" EnableViewState="false" AutoEventWireup="true" CodeBehind="Promotions.aspx.cs" Inherits="SalonAddict.Promotions" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <style type="text/css" >
       #hairdressing { margin-bottom:25px; }
       #hairdressing h1 { color:#212121; }
       #hairdressing ol { color:#212121; }
       #hairdressing ol li { padding:8px; }
       #hairdressing a { color:#212121; font-weight:bold; }
       #hairdressing .description { font-size:11px; font-family:Arial; padding:5px; color:#212121; }
       #hairdressing .special-offer { margin-top:5px; font-size:11px; font-family:Arial; padding:5px; border:solid 1px #000; }
       
       #beauty { margin-bottom:25px; }
       #beauty h1 { color:#ef2869; }
       #beauty ol { color:#ef2869; }
       #beauty ol li { padding:8px; }
       #beauty a { color:#ef2869; font-weight:bold;}
       #beauty .description { font-size:11px; font-family:Arial; padding:5px; color:#ef2869; }
       #beauty .special-offer { margin-top:5px; font-size:11px; font-family:Arial; padding:5px; color:#ef2869; border:solid 1px #ef2869; }
    </style>
    <div id="hairdressing" >
        <h1><%= base.GetLocalResourceObject("lblHairdressing.Text") %></h1>
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
    <div id="beauty" >
        <h1><%= base.GetLocalResourceObject("lblBeauty.Text") %></h1>
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
</asp:Content>
