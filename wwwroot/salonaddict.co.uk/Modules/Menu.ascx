<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" EnableViewState="false" Inherits="SalonAddict.Modules.Menu" %>
<div class="module-menu" >
   <ul>
      <li>
         <% if (this.SelectedMenuItem == MenuItem.Home)
            { %>
         <a href='<%= Page.ResolveUrl("~/") %>' class="selected" ><%= base.GetLocalResourceObject("hlHome.Text") %></a>
         <% }
            else
            { %>
         <a href='<%= Page.ResolveUrl("~/") %>' ><%= base.GetLocalResourceObject("hlHome.Text")%></a>
         <% } %>
      </li>
      <li>
         <% if (this.SelectedMenuItem == MenuItem.AboutUs)
            { %>
         <a href='<%= Page.ResolveUrl("~/what-we-do.aspx") %>' class="selected" ><%= base.GetLocalResourceObject("hlAboutUs.Text")%></a>
         <% }
            else
            { %>
         <a href='<%= Page.ResolveUrl("~/what-we-do.aspx") %>' ><%= base.GetLocalResourceObject("hlAboutUs.Text")%></a>
         <% } %>
      </li>
      <li>
         <% if (this.SelectedMenuItem == MenuItem.Savings)
            { %>
         <a href='<%= Page.ResolveUrl("~/savings.aspx") %>' class="selected" ><%= base.GetLocalResourceObject("hlSavings.Text") %></a>
         <% }
            else
            { %>
         <a href='<%= Page.ResolveUrl("~/savings.aspx") %>' ><%= base.GetLocalResourceObject("hlSavings.Text")%></a>
         <% } %>
      </li>
      <li>
         <% if (this.SelectedMenuItem == MenuItem.SuggestSalon)
            { %>
         <a href='<%= Page.ResolveUrl("~/suggest-salon.aspx") %>' class="selected" ><%= base.GetLocalResourceObject("hlSuggestSalon.Text")%></a>
         <% }
            else
            { %>
         <a href='<%= Page.ResolveUrl("~/suggest-salon.aspx") %>' ><%= base.GetLocalResourceObject("hlSuggestSalon.Text")%></a>
         <% } %>
      </li>
      <li>
         <% if (this.SelectedMenuItem == MenuItem.Blog)
            { %>
         <a href="http://blog.salonaddict.co.uk" class="selected" ><%= base.GetLocalResourceObject("hlBlog.Text")%></a>
         <% }
            else
            { %>
         <a href="http://blog.salonaddict.co.uk" ><%= base.GetLocalResourceObject("hlBlog.Text")%></a>
         <% } %>
      </li>
   </ul>
</div>