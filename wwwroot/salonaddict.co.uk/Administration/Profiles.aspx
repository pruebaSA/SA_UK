<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Profiles.aspx.cs" Inherits="SalonAddict.Administration.Profiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-partner.png" alt="Sales" />
        Manage White-Label Profiles
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new profile" OnClientClick="location.href='ProfileCreate.aspx';return false" />
    </div>
</div>
<br />
<asp:GridView 
    ID="gv" 
    runat="server" 
    DataKeyNames="ProfileID"
    AutoGenerateColumns="False"
    Width="100%"
    AllowPaging="true"
    OnPageIndexChanging="gv_PageIndexChanging"
    PageSize="20" >
    <Columns>
        <asp:TemplateField HeaderText="Profile Name"  >
            <ItemTemplate>
                <a href="ProfileDetails.aspx?ProfileID=<%#Eval("ProfileID")%>" title="Edit profile">
                    <%# Eval("Name") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Theme" ItemStyle-Width="120px"  >
            <ItemTemplate>
                <%# Eval("Theme") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Billing Cycle" ItemStyle-Width="180px"  >
            <ItemTemplate>
                <%# Convert.ToDateTime(Eval("ActiveOn").ToString()).ToShortDateString()%> -&nbsp;
                <asp:MultiView ID="mv" runat="server" ActiveViewIndex='<%# (((DateTime)Eval("ExpiresOn")) == DateTime.MaxValue)? 0 : 1 %>' >
                   <asp:View ID="v1" runat="server" >
                       (<i>present</i>)
                   </asp:View>
                   <asp:View ID="v2" runat="server" >
                       <%# Convert.ToDateTime(Eval("ExpiresOn").ToString()).ToShortDateString() %>
                   </asp:View>
                </asp:MultiView>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Active" ItemStyle-Width="80px"  >
            <ItemTemplate>
                <%# Eval("Active") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Created On" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <%# Convert.ToDateTime(Eval("CreatedOn").ToString()).ToShortDateString() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField ItemStyle-Width="50px" >
            <ItemTemplate>
                <a href="ProfileDetails.aspx?ProfileID=<%#Eval("ProfileID")%>" title="Edit profile">
                    Edit
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
