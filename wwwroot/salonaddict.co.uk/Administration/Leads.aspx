<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Leads.aspx.cs" Inherits="SalonAddict.Administration.Leads" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-marketing.gif" alt="Marketing" />
        Manage Leads
    </div>
    <div class="options">
        <asp:Button ID="btnExportXML" runat="server" Text="Export to XML" OnClick="btnExportXML_Click" ValidationGroup="ExportXML" ToolTip="Export lead list to a xml file" />
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new lead" OnClientClick="location.href='LeadCreate.aspx';return false" />
    </div>
</div>
<br />
<asp:DropDownList ID="ddlSortBy" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSortBy_SelectedIndexChanged" >
   <asp:ListItem Text="Filter Records:" Value="0" ></asp:ListItem>
   <asp:ListItem Text="New" Value="10" ></asp:ListItem>
   <asp:ListItem Text="Open" Value="20" ></asp:ListItem>
   <asp:ListItem Text="Contacted" Value="30" ></asp:ListItem>
   <asp:ListItem Text="Opportunity" Value="40" ></asp:ListItem>
   <asp:ListItem Text="Disqualified" Value="50" ></asp:ListItem>
</asp:DropDownList>
<br /><br />
<asp:GridView 
    ID="gvLeads" 
    runat="server" 
    AutoGenerateColumns="False"
    Width="100%"
    DataKeyNames="LeadID"
    OnPageIndexChanging="gvLeads_PageIndexChanging" 
    AllowPaging="true" 
    PageSize="15">
    <Columns>
        <asp:TemplateField HeaderText="Topic" >
            <ItemTemplate>
                <%# Eval("Topic") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Company" >
            <ItemTemplate>
                <%# Eval("Company") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Phone" ItemStyle-Width="80px" >
            <ItemTemplate>
                <%# Eval("PhoneNumber") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Source" >
            <ItemTemplate>
                <%# Eval("SourceFullName") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Source Email" >
            <ItemTemplate>
                <%# Eval("Email") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Source Mobile" >
            <ItemTemplate>
                <%# Eval("SourceMobileNumber") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Status" ItemStyle-Width="50px" >
            <ItemTemplate>
                <%# Eval("LeadStatusType") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <a href="LeadDetails.aspx?LeadID=<%#Eval("LeadID")%>" title="Edit lead details">
                   Edit
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Created on" ItemStyle-Width="100px" >
            <ItemTemplate>
                <%# ((DateTime)Eval("CreatedOn")).ToString() %>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
