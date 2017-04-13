<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Coupons.aspx.cs" Inherits="SalonAddict.Administration.CouponsPage" %>
<%@ Import Namespace="SalonAddict.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="Sales" />
        Manage Coupons
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new coupon" OnClientClick="location.href='CouponCreate.aspx';return false" />
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
        <asp:TemplateField HeaderText="Coupon Percentage" ItemStyle-Width="130px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <%# Math.Round((decimal)Eval("DiscountPercentage"), 2) %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Coupon Amount" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center" >
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
                <a href="CouponDetails.aspx?CouponID=<%#Eval("DiscountID")%>" title="Edit coupon details">
                   Edit
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
