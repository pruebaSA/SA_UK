<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Businesses.aspx.cs" Inherits="SalonAddict.Administration.Businesses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-customers.png" alt="Businesses" />
        Manage Businesses
    </div>
    <div class="options">
        <asp:Button ID="btnExportXML" runat="server" Text="Export to XML" OnClick="btnExportXML_Click" ValidationGroup="ExportXML" ToolTip="Export business list to a xml file" />
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new business" OnClientClick="location.href='BusinessCreate.aspx';return false" />
    </div>
</div>
<br />
<asp:GridView 
    ID="gvBusinesses" 
    runat="server" 
    DataKeyNames="BusinessID"
    AutoGenerateColumns="False"
    Width="100%"
    AllowPaging="true"
    OnPageIndexChanging="gvBusinesses_PageIndexChanging"
    PageSize="25" >
    <Columns>
        <asp:TemplateField HeaderText="Name" >
            <ItemTemplate>
                <a href="BusinessDetails.aspx?BusinessGUID=<%#Eval("BusinessGUID")%>" title="Edit business details">
                    <%# Eval("Name") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Allow Email Notifications" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center"  >
            <ItemTemplate>
                <%# Eval("AllowEmailNotifications") %>
            </ItemTemplate>
        </asp:TemplateField>
         <asp:TemplateField HeaderText="Allow SMS Notifications" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center"  >
            <ItemTemplate>
                <%# Eval("AllowSMSNotifications") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Rating" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <%# Eval("RatingScore") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Is Guest" ItemStyle-Width="60px"  ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <%# Eval("IsGuest") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Published" ItemStyle-Width="60px"  ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <%# Eval("Published") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Created On" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <%# Convert.ToDateTime(Eval("CreatedOn").ToString()).ToShortDateString() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" ItemStyle-Width="60px" >
            <ItemTemplate>
                <a href='<%# "BusinessDetails.aspx?BusinessGUID=" + Eval("BusinessGUID") %>' title="Edit business" >
                    Edit
                </a>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
