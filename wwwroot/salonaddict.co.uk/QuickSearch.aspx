<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" EnableViewState="false"  AutoEventWireup="true" CodeBehind="QuickSearch.aspx.cs" Inherits="SalonAddict._QuickSearch" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <style type="text/css" >
       #hairdressing { margin-bottom:25px; }
       #hairdressing h1 { color:#212121; }
       #hairdressing ol { color:#212121; }
       #hairdressing ol li { padding:8px; }
       #hairdressing a { color:#212121; font-weight:bold; }
       
       #beauty { margin-bottom:25px; }
       #beauty h1 { color:#ef2869; }
       #beauty ol { color:#ef2869; }
       #beauty ol li { padding:8px; }
       #beauty a { color:#ef2869; font-weight:bold; }
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
              </li>
           </ItemTemplate>
        </asp:ListView>
    </div>
</asp:Content>