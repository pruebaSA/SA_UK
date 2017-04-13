<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewsRSS.aspx.cs" EnableViewState="false" EnableTheming="false" MaintainScrollPositionOnPostback="false" Inherits="SalonAddict.rss.NewsRSS" %>
<%@ Import Namespace="SalonAddict.BusinessAccess.Configuration" %>
<%@ Import Namespace="SalonAddict.BusinessAccess.Implementation" %>
<head runat="server" visible="false" ></head>
<rss version="2.0">
 <channel> 
    <title><![CDATA[<asp:Literal ID="lblTitle" runat="server" ></asp:Literal>]]></title>
    <link><asp:Literal ID="lblLink" runat="server" ></asp:Literal></link>
    <description><asp:Literal ID="lblDescription" runat="server" ></asp:Literal></description>
    <language><asp:Literal ID="lblLanguage" runat="server" ></asp:Literal></language>
    <copyright><asp:Literal ID="lblCopyright" runat="server" ></asp:Literal></copyright>
    <asp:repeater id="rptr" runat="server">
        <ItemTemplate>
            <item>
              <title><![CDATA[<%# Eval("Title") %>]]></title>
              <author><![CDATA[<%# this.CompanyName %>]]></author>
              <description><![CDATA[<%# Eval("Description") %>]]></description>
              <link><![CDATA[<%# Eval("Link") %>]]></link>
              <pubDate><%# string.Format("{0:R}", (DateTime)Eval("CreatedOn"))%></pubDate>
          </item>
        </ItemTemplate> 
    </asp:repeater>
 </channel> 
</rss>