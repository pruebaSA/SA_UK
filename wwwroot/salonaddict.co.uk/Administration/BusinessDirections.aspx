<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BusinessDirections.aspx.cs" Inherits="SalonAddict.Administration.BusinessDirections" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >
<div class="section-header">
    <div class="title">
        <img src="images/ico-customers.png" alt="Businesses" />
        Manage Directions <asp:HyperLink ID="hlBack" runat="server" ></asp:HyperLink>
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new directions" />
    </div>
</div>
<br />
<asp:GridView 
    ID="gv" 
    runat="server" 
    AutoGenerateColumns="false" >
  <Columns>
     <asp:TemplateField HeaderText="Instructions" >
        <ItemTemplate>
           <%# Eval("Instructions") %>
        </ItemTemplate>
     </asp:TemplateField>
     <asp:TemplateField HeaderText="Display Order">
         <ItemTemplate>
            <%# Eval("DisplayOrder")%>
         </ItemTemplate>
         <ItemStyle Width="100px" />
     </asp:TemplateField>
     <asp:TemplateField HeaderText="Edit" ItemStyle-Width="10%" >
        <ItemTemplate>
            <a href='<%# "BusinessDirectionsDetails.aspx?BusinessGUID=" + base.BusinessGUID + "&BusinessDirectionsID=" + Eval("BusinessDirectionsID") %>' title="Edit business directions" >
                Edit
            </a>
        </ItemTemplate>
        <ItemStyle Width="50px" HorizontalAlign="Center" />
      </asp:TemplateField>
  </Columns>
</asp:GridView>
</asp:Content>
