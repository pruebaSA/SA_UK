<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Discounts.aspx.cs" Inherits="SalonAddict.Administration.Discounts" %>
<%@ Import Namespace="SalonAddict.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="Sales" />
        Manage Discounts
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new discount" OnClientClick="location.href='DiscountCreate.aspx';return false" />
    </div>
</div>
<br /><br />
<asp:GridView 
    ID="gv" 
    runat="server" 
    AutoGenerateColumns="False"
    Width="100%"
    DataKeyNames="DiscountID" >
    <Columns>
        <asp:TemplateField HeaderText="Name" >
            <ItemTemplate>
                <%# Eval("Name").ToString().HtmlEncode() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Use Percentage" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <%# Eval("UsePercentage") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Discount Percentage" ItemStyle-Width="130px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <%# Math.Round((decimal)Eval("DiscountPercentage"), 2) %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Discount Amount" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <%# Math.Round((decimal)Eval("DiscountAmount"), 2) %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Start Date" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <%# ((DateTime)Eval("StartDate")).ToShortDateString() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="End Date" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <%# ((DateTime)Eval("EndDate") == DateTime.MaxValue)? String.Empty : ((DateTime)Eval("EndDate")).ToShortDateString() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <a href="DiscountDetails.aspx?DiscountID=<%#Eval("DiscountID")%>" title="Edit discount details">
                   Edit
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
