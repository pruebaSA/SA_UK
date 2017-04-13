<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="SalonSetup.aspx.cs" Inherits="IFRAME.Admin.SalonSetupPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Salons" />
       <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-salons.png" %>' alt="salons" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><asp:Literal ID="ltrHeader" runat="server" ></asp:Literal></h1></td>
              <td style="vertical-align:middle" >

              </td>
           </tr>
        </table>
        <table class="gridview" cellspacing="0" rules="all" border="1" style="border-collapse:collapse;" >
           <tr class="header-style">
		       <th scope="col"><%= base.GetLocaleResourceString("gv.Columns[0].HeaderText") %></th>
		       <th scope="col" width="100px" ><%= base.GetLocaleResourceString("gv.Columns[1].HeaderText") %></th>
		       <th scope="col" width="80px" ><%= base.GetLocaleResourceString("gv.Columns[2].HeaderText") %></th>
		   </tr>
		   <tr class="row-style">
		       <td>
		          <%= base.GetLocaleResourceString("hlEmployees.Text") %>
		       </td>
		       <td>
		          <center><asp:Literal ID="ltrEmployeeCount" runat="server" ></asp:Literal></center>
		       </td>
		       <td>
		          <center>
		             <asp:ImageButton ID="ibEditEmployees" runat="server" SkinID="GridEditImageButton" CommandName="Employees" OnCommand="ibEdit_Command" />
		          </center>
		       </td>
		   </tr>
		   <tr class="altrow-style">
		       <td>
		          <%= base.GetLocaleResourceString("hlSchedule.Text") %>
		       </td>
		       <td>
		           <center><asp:Literal ID="ltrScheduleCount" runat="server" ></asp:Literal></center>
		       </td>
		       <td>
		          <center>
		             <asp:ImageButton ID="ibEditSchedule" runat="server" SkinID="GridEditImageButton" CommandName="Schedule" OnCommand="ibEdit_Command" />
		          </center>
		       </td>
		   </tr>
        </table>
    </asp:Panel>
</asp:Content>