<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Blacklist.aspx.cs" Inherits="SalonAddict.Administration.Blacklist" %>
<%@ Import Namespace="SalonAddict.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-blacklist.png" alt="" />
        Manage blacklist
    </div>
    <div class="options">
        <asp:Button ID="btnAddBannedIP" runat="server" Text="Add banned IP" ToolTip="Add IP to blacklist" OnClientClick="location.href='BannedIPAddressCreate.aspx';return false" />
        <asp:Button ID="btnAddBannedNetwork" runat="server" Text="Add banned network"  ToolTip="Add network to blacklist" OnClientClick="location.href='BannedIPNetworkCreate.aspx';return false" />
    </div>
</div>
<p>
</p>
<ajaxToolkit:TabContainer runat="server" ID="tabBlacklist" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pagIpAddress" HeaderText="IP addresses">
        <ContentTemplate>
            <asp:GridView 
                ID="gvBannedIpAddress" 
                runat="server" 
                DataKeyNames="BannedIPAddressID"
                AutoGenerateColumns="False" 
                Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="IP address" ItemStyle-Width="20%">
                        <ItemTemplate>
                            <a href="BannedIPAddressDetails.aspx?BannedIPAddressID=<%#Eval("BannedIPAddressID")%>" title="Edit IP address">
                                <%#Server.HtmlEncode(Eval("Address").ToString())%>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Comment" >
                        <ItemTemplate>
                            <%# Eval("Comment") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Created on" HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%# ((DateTime)Eval("CreatedOn")).ToFriendlyTimeFormat(TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")).ToString("f").ToString()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Updated on" HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%# ((DateTime)Eval("UpdatedOn")).ToFriendlyTimeFormat(TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")).ToString("f").ToString()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pagIpNetwork" HeaderText="IP networks">
        <ContentTemplate>
            <asp:GridView ID="gvBannedIpNetwork" runat="server" AutoGenerateColumns="False" Width="100%">
                <Columns>
                    <asp:BoundField DataField="BannedIpNetworkID" HeaderText="ID" Visible="False"></asp:BoundField>
                    <asp:TemplateField HeaderText="IP range" ItemStyle-Width="20%">
                        <ItemTemplate>
                            <a href="BannedIPNetworkDetails.aspx?BannedIpNetworkID=<%#Eval("BannedIpNetworkID")%>" title="Edit IP address">
                                <%# Server.HtmlEncode(Eval("StartAddress").ToString())%>-<%# Server.HtmlEncode(Eval("EndAddress").ToString())%>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Comment" HeaderText="Comment" ItemStyle-Width="20%" ></asp:BoundField>
                    <asp:BoundField DataField="IpException" HeaderText="IP exceptions" ItemStyle-Width="20%" ></asp:BoundField>
                    <asp:TemplateField HeaderText="Created on" HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%# ((DateTime)Eval("CreatedOn")).ToFriendlyTimeFormat(TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")).ToString("f").ToString()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Updated on" HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%"
                        ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%# ((DateTime)Eval("UpdatedOn")).ToFriendlyTimeFormat(TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")).ToString("f").ToString()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
</asp:Content>

